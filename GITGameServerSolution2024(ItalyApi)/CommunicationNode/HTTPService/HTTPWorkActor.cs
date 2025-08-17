using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using CommNode.Database;
using StackExchange.Redis;
using GITProtocol;

namespace CommNode.HTTPService
{
    public class HTTPWorkActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger                = Logging.GetLogger(Context);
        public HTTPWorkActor()
        {
            ReceiveAsync<HTTPEnterGameRequest>          (onEnterGame);
            ReceiveAsync<FromHTTPSessionMessage>        (onFromMessage);
            ReceiveAsync<HttpLogoutRequest>             (onLogoutRequest);
            ReceiveAsync<HttpGetMiniLobbyGamesMessage>  (onGetMiniLobbyGames);
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

        private async Task onGetMiniLobbyGames(HttpGetMiniLobbyGamesMessage request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.SessionToken);
                if (strUserActorPath == null)
                {
                    Sender.Tell(null);
                    return;
                }

                Sender.Tell(PPAllGamesSnapshot.Instance.MiniLobbyGames);
            }
            catch (Exception ex)
            {
                Sender.Tell(HTTPEnterGameResults.INVALIDTOKEN);
                _logger.Error("Exception has been occurred in HTTPWorkActor::onGetMiniLobbyGames {0}", ex);
            }
        }

        private async Task onEnterGame(HTTPEnterGameRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.SessionToken);
                if(strUserActorPath == null)
                {
                    Sender.Tell(HTTPEnterGameResults.INVALIDTOKEN);
                    return;
                }

                //게임아이디유효성검사
                
                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(request.GameType, request.GameIdentifier);
                if (gameID == 0)
                {
                    await closeHttpSession(request.UserID, request.SessionToken, strUserActorPath);
                    Sender.Tell(HTTPEnterGameResults.INVALIDGAMEID);
                    return;
                }
                
                //유저액터에 게임입장요청을 보낸다.
                await sendEnterRequestToUserActor(request.UserID, gameID, strUserActorPath, request.SessionToken);
            }
            catch (Exception ex)
            {
                Sender.Tell(HTTPEnterGameResults.INVALIDTOKEN);
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
                    Sender.Tell(new ToHTTPSessionMessage(ToHTTPSessionMsgResults.INVALIDTOKEN));
                    return;
                }

                object response = await Context.System.ActorSelection(strUserActorPath).Ask<object>(new FromConnRevMessage(fromMessage.SessionToken, fromMessage.Message), TimeSpan.FromSeconds(10));
                if(response is string)
                {
                    if((string) response == "invalidaction")
                        await closeHttpSession(fromMessage.UserID, fromMessage.SessionToken as string, strUserActorPath);

                    Sender.Tell(new ToHTTPSessionMessage(ToHTTPSessionMsgResults.INVALIDTOKEN));
                }
                else
                {
                    Sender.Tell(new ToHTTPSessionMessage((response as SendMessageToUser).Message));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::onFromMessage {0} {1}", ex, fromMessage.Message.MsgCode);
                Sender.Tell(new ToHTTPSessionMessage(ToHTTPSessionMsgResults.INVALIDTOKEN));
            }
        }
        
        private long getUnixMiliTimestamp()
        {
            DateTimeOffset now          = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds   = now.ToUnixTimeMilliseconds();
            return unixTimeMilliseconds;
        }
        
        private async Task sendEnterRequestToUserActor(string strUserID, int gameID, string strUserActorPath, string strSessionToken)
        {
            try
            {
                GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_ENTERGAME);
                gitMessage.Append((ushort)gameID);

                object response = await Context.System.ActorSelection(strUserActorPath).Ask<object>(new FromConnRevMessage(strSessionToken, gitMessage), TimeSpan.FromSeconds(10));

                //실패(유저의 불법액션이므로 세션을 종료한다.)
                if(response is string)
                {
                    await closeHttpSession(strUserID, strSessionToken, strUserActorPath);
                    Sender.Tell(HTTPEnterGameResults.INVALIDACTION);
                    return;
                }

                GITMessage  responseMessage = (response as SendMessageToUser).Message;
                byte        result          = (byte) responseMessage.Pop();
                if (result == 0)
                    Sender.Tell(HTTPEnterGameResults.OK);
                else
                    Sender.Tell(HTTPEnterGameResults.INVALIDGAMEID);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::tryEnterGameInUserActor {0}", ex);
                Sender.Tell(HTTPEnterGameResults.INVALIDGAMEID);
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
