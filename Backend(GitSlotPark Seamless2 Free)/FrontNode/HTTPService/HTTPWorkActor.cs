using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using FrontNode.Database;
using StackExchange.Redis;
using GITProtocol;

namespace FrontNode.HTTPService
{
    public class HTTPWorkActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public HTTPWorkActor()
        {
            ReceiveAsync<HTTPEnterGameRequest>          (onEnterGame);
            ReceiveAsync<FromHTTPSessionMessage>        (onFromMessage);
            ReceiveAsync<HttpLogoutRequest>             (onLogoutRequest);
            Receive<string>                             (onProcCommand);
        }

        private void onProcCommand(string strCommand)
        {
            if (strCommand == "terminate")
            {
                Self.Tell(PoisonPill.Instance);
            }
        }
        
        private async Task<string> findUserActor(string strUserID, string strSessionToken)
        {
            try
            {
                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync(strUserID + "_tokens", strSessionToken);
                if (redisValue.IsNullOrEmpty)
                    return null;

                string strUserActorPath = (string)redisValue;
                return strUserActorPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::findUserActor {0}", ex);
                return null;
            }
        }

        private async Task onLogoutRequest(HttpLogoutRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                    return;

                await closeHttpSession(request.UserID, request.Token, strUserActorPath);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::onLogoutRequest {0}", ex);
            }
        }               
        private async Task onEnterGame(HTTPEnterGameRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.SessionToken);
                if(strUserActorPath == null)
                {
                    Sender.Tell("invalidtoken");
                    return;
                }

                //게임아이디유효성검사                
                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(request.GameType, request.GameIdentifier);
                if (gameID == 0)
                {
                    await closeHttpSession(request.UserID, request.SessionToken, strUserActorPath);
                    Sender.Tell("invalidgameid");
                    return;
                }
                
                //유저액터에 게임입장요청을 보낸다.
                sendEnterRequestToUserActor(request.UserID, gameID, strUserActorPath, request.SessionToken);
            }
            catch (Exception ex)
            {
                Sender.Tell("invalidtoken");
                _logger.Error("Exception has been occurred in HTTPWorkActor::onEnterGame {0}", ex);
            }
        }

        private async Task onFromMessage(FromHTTPSessionMessage fromMessage)
        {
            try
            {

                string strUserActorPath = await findUserActor(fromMessage.UserID, fromMessage.SessionToken);
                if (strUserActorPath == null)
                {
                    Sender.Tell("invalid user");
                    return;
                }
                Context.System.ActorSelection(strUserActorPath).Tell(new FromConnRevMessage(fromMessage.SessionToken, fromMessage.Message), Sender);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::onFromMessage {0} {1} {2}", fromMessage.UserID, fromMessage.SessionToken, ex);
                Sender.Tell("exception");
            }
        }        
        private void sendEnterRequestToUserActor(string strUserID, int gameID, string strUserActorPath, string strSessionToken)
        {
            try
            {
                GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_ENTERGAME);
                gitMessage.Append((ushort)gameID);
                Context.System.ActorSelection(strUserActorPath).Tell(new FromConnRevMessage(strSessionToken, gitMessage), Sender); 
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::tryEnterGameInUserActor {0} {1} {2}", strUserActorPath, gameID, ex);
            }
        }
        private async Task closeHttpSession(string strUserID, string strSessionToken, string strUserActorPath)
        {
            try
            {
                await RedisDatabase.RedisCache.HashDeleteAsync(strUserID + "_tokens", strSessionToken);
                Context.System.ActorSelection(strUserActorPath).Tell(new HttpSessionClosed(strSessionToken));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::closeHttpSession {0}", ex);
            }
        }

       
    }    
}
