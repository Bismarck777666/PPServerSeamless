using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using SlotGamesNode.Database;
using StackExchange.Redis;
using Akka.Cluster;
using GITProtocol;
using SlotGamesNode.HTTPService.Models;

namespace SlotGamesNode.HTTPService
{
    public class HTTPAuthWorker : ReceiveActor
    {
        private IActorRef                   _dbReaderProxy  = null;
        private IActorRef                   _dbWriterProxy  = null;
        private IActorRef                   _redisWriter    = null;
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);

        public HTTPAuthWorker(IActorRef dbReaderProxy, IActorRef dbWriterProxy, IActorRef redisWriter)
        {
            _dbReaderProxy  = dbReaderProxy;
            _dbWriterProxy  = dbWriterProxy;
            _redisWriter    = redisWriter;

            ReceiveAsync<HTTPOperatorVerifyRequest> (onOperatorVerifyRequest);
            ReceiveAsync<HTTPVerifyRequest>         (onVerifyRequest);            
            Receive<string>                         (onProcCommand);

        }

        private void onProcCommand(string strCommand)
        {
            if (strCommand != "terminate")
                return;

            Self.Tell(PoisonPill.Instance);
        }
        private async Task onVerifyRequest(HTTPVerifyRequest request)
        {
            try
            {
                string strGameID = PGGamesSnapshot.Instance.findGameStringFromID(request.GameID);
                if (string.IsNullOrEmpty(strGameID))
                {
                    Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.INVALIDGAMEID));
                    return;
                }
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.INVALIDGAMEID));
                    return;
                }
                Sender.Tell(new HTTVerifyResponse(request.Token, strGameID, request.GameID, request.UserID, request.UserID));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onVerifyRequest {0}", ex);
            }
            Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.IDPASSWORDERROR));
        }
        
        private async Task onOperatorVerifyRequest(HTTPOperatorVerifyRequest request)
        {
            try
            {
                UserLoginResponse loginResponse = await Context.System.ActorSelection("/user/dbproxy/readers").Ask<UserLoginResponse>(new UserLoginRequest(request.UserID, request.PasswordMD5));
                if(loginResponse.Result != LoginResult.OK)
                {
                    Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.IDPASSWORDERROR));
                    return;
                }
                string strGameID = PGGamesSnapshot.Instance.findGameStringFromID(request.GameID);
                if(string.IsNullOrEmpty(strGameID))
                {
                    Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.INVALIDGAMEID));
                    return;
                }

                string strNewSesionToken = createNewSessionToken(loginResponse.UserID + "@" + request.PasswordMD5);

                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", request.UserID);
                if (!redisValue.IsNullOrEmpty)
                {
                    await RedisDatabase.RedisCache.HashSetAsync("onlineusers", strNewSesionToken, true);
                    string strUserActorPath = string.Format("/user/userManager/{0}", request.UserID);
                    Context.System.ActorSelection(strUserActorPath).Tell(new HttpSessionAdded(strNewSesionToken));
                    Sender.Tell(new HTTVerifyResponse(strNewSesionToken, strGameID, request.GameID, loginResponse.UserID, loginResponse.UserID));
                    return;
                } 

                bool isNotOnline = await RedisDatabase.RedisCache.HashSetAsync("onlineusers", request.UserID, true, When.NotExists);
                if (isNotOnline)
                {
                    IActorRef   userActor = await Context.System.ActorSelection("/user/userManager").Ask<IActorRef>(new CreateNewUserMessage(strNewSesionToken, _dbReaderProxy, _dbWriterProxy, _redisWriter, loginResponse));
                    await RedisDatabase.RedisCache.HashSetAsync("onlineusers", strNewSesionToken, true);
                    Sender.Tell(new HTTVerifyResponse(strNewSesionToken, strGameID, request.GameID, loginResponse.UserID, loginResponse.UserID));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onVerifyRequest {0}", ex);
            }
            Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.IDPASSWORDERROR));
        }
        private string createNewSessionToken(string strUserToken)
        {
            string strSessionToken = strUserToken + "_" + Guid.NewGuid().ToString();
            return strSessionToken;
        }
        private async Task<string> findUserActor(string strUserID, string strSessionToken)
        {
            try
            {
                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", strSessionToken);
                if (redisValue.IsNullOrEmpty)
                    return null;

                string strUserActorPath = string.Format("/user/userManager/{0}", strUserID);
                return strUserActorPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::findUserActor {0}", ex);
                return null;
            }
        }

    }
}
