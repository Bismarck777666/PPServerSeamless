using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using FrontNode.Database;
using StackExchange.Redis;
using Akka.Cluster;
using GITProtocol;
using FrontNode.HTTPService.Models;

/***
 * 
 *          Created by Foresight(2021.03.18)
 */
namespace FrontNode.HTTPService
{
    public class HTTPAuthWorker : ReceiveActor
    {
        private IActorRef                   _dbReaderProxy  = null;
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);

        public HTTPAuthWorker(IActorRef dbReaderProxy)
        {
            _dbReaderProxy = dbReaderProxy;
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
        private string createNewSessionToken(string strUserToken)
        {
            string strSessionToken = strUserToken + "_" + Guid.NewGuid().ToString();
            return strSessionToken;
        }

        private async Task onOperatorVerifyRequest(HTTPOperatorVerifyRequest request)
        {
            try
            {
                UserLoginResponse loginResponse = await _dbReaderProxy.Ask<UserLoginResponse>(new UserLoginRequest(request.AgentID, request.UserID, request.PasswordMD5, "", PlatformTypes.WEB));
                if (loginResponse.Result != LoginResult.OK)
                {
                    Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.IDPASSWORDERROR));
                    return;
                }
                string strGameID = PGGamesSnapshot.Instance.findGameStringFromID(request.GameID);
                if (string.IsNullOrEmpty(strGameID))
                {
                    Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.INVALIDGAMEID));
                    return;
                }
                string strNewSesionToken = createNewSessionToken(loginResponse.GlobalUserID + "@" + loginResponse.PassToken);
                string strHashKey        = string.Format("{0}_tokens", loginResponse.GlobalUserID);

                //유저액터가 이미 존재하는가를 검사한다.
                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", loginResponse.GlobalUserID + "_path");
                if (!redisValue.IsNullOrEmpty)
                {
                    string strUserActorPath = (string)redisValue;
                    await RedisDatabase.RedisCache.HashSetAsync(strHashKey, strNewSesionToken, strUserActorPath);

                    Context.System.ActorSelection(strUserActorPath).Tell(new HttpSessionAdded(strNewSesionToken));
                    Sender.Tell(new HTTVerifyResponse(strNewSesionToken, strGameID, request.GameID, loginResponse.UserID, loginResponse.UserID, loginResponse.Currency.ToString()));
                    return;
                }

                //유저새로 로그인
                bool isNotOnline = await RedisDatabase.RedisCache.HashSetAsync("onlineusers", loginResponse.GlobalUserID, true, When.NotExists);
                if (isNotOnline)
                {
                    //새로운 세션토큰을 만든다.
                    IActorRef userActor = await Context.System.ActorSelection("/user/userRouter").Ask<IActorRef>(new CreateNewUserMessage(strNewSesionToken, loginResponse.UserDBID, loginResponse.UserID, loginResponse.UserBalance, loginResponse.PassToken, loginResponse.AgentDBID, loginResponse.AgentID, loginResponse.LastScoreCounter, loginResponse.Currency, loginResponse.IsAffiliate), TimeSpan.FromSeconds(10.0));
                    string strUserActorPath = userActor.Path.ToString();

                    await RedisDatabase.RedisCache.HashSetAsync(strHashKey, strNewSesionToken, strUserActorPath);
                    Sender.Tell(new HTTVerifyResponse(strNewSesionToken, strGameID, request.GameID, loginResponse.UserID, loginResponse.UserID, loginResponse.Currency.ToString()));
                    return;
                }
               
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onOperatorVerifyRequest {0}", ex);
            }
            Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.IDPASSWORDERROR));
        }

        private async Task<string> findUserActor(int agentID, string strUserID, string strSessionToken)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                string strHashKey      = string.Format("{0}_tokens", strGlobalUserID);

                RedisValue redisValue  = await RedisDatabase.RedisCache.HashGetAsync(strHashKey, strSessionToken);
                if (redisValue.IsNullOrEmpty)
                    return null;

                return (string) redisValue;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::findUserActor {0}", ex);
                return null;
            }
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
                string strUserActorPath = await findUserActor(request.AgentID, request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.INVALIDGAMEID));
                    return;
                }
                Context.System.ActorSelection(strUserActorPath).Tell(new UserVerifyRequest(request.Token, request.GameID, strGameID), Sender);
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onVerifyRequest {0}", ex);
            }
            Sender.Tell(new HTTVerifyResponse(HttpVerifyResults.IDPASSWORDERROR));
        }
        public class CheckUserPathFromRedis
        {
            public IActorRef    Sender          { get; private set; }
            public string       UserID          { get; private set; }
            public string       SessionToken    { get; private set; }
            public int          RetryCount      { get; private set; }

            public CheckUserPathFromRedis(string strUserID, string strSessionToken, IActorRef sender, int retryCount)
            {
                this.UserID         = strUserID;
                this.SessionToken   = strSessionToken;
                this.Sender         = sender;
                this.RetryCount     = retryCount;
            }
        }
    }
}
