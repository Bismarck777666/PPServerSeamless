using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using SlotGamesNode.Database;
using StackExchange.Redis;
using GITProtocol;
using Newtonsoft.Json;

namespace SlotGamesNode.HTTPService
{
    public class HTTPWorkActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger                = Logging.GetLogger(Context);
        public HTTPWorkActor()
        {
            ReceiveAsync<HTTPEnterGameRequest>          (onEnterGame);
            ReceiveAsync<FromHTTPSessionMessage>        (onFromMessage);
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
                GAMEID gameID = PGGamesSnapshot.Instance.findGameIDFromString(request.GameIdentifier);
                if (gameID == GAMEID.None)
                {
                    await closeHttpSession(request.UserID, request.SessionToken, strUserActorPath);
                    Sender.Tell(HTTPEnterGameResults.INVALIDGAMEID);
                    return;
                }
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
                    Sender.Tell(ToHTTPSessionMsgResults.INVALIDTOKEN);
                    return;
                }

                object response = await Context.System.ActorSelection(strUserActorPath).Ask<object>(new FromConnRevMessage(fromMessage.SessionToken, fromMessage.Message), TimeSpan.FromSeconds(10));
                if(response is string)
                {
                    if((string) response == "invalidaction")
                        await closeHttpSession(fromMessage.UserID, fromMessage.SessionToken as string, strUserActorPath);

                    Sender.Tell(ToHTTPSessionMsgResults.INVALIDTOKEN);
                }
                else
                {
                    Sender.Tell(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::onFromMessage {0}", ex);
                Sender.Tell(ToHTTPSessionMsgResults.INVALIDTOKEN);
            }
        }
        private long getUnixMiliTimestamp()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            return unixTimeMilliseconds;
        }
        private async Task sendEnterRequestToUserActor(string strUserID, GAMEID gameID, string strUserActorPath, string strSessionToken)
        {
            try
            {
                GITMessage gitMessage = new GITMessage(MsgCodes.ENTERGAME);
                gitMessage.Append((int) gameID);

                object response = await Context.System.ActorSelection(strUserActorPath).Ask<object>(new FromConnRevMessage(strSessionToken, gitMessage), TimeSpan.FromSeconds(10));
                if(response is string)
                {
                    await closeHttpSession(strUserID, strSessionToken, strUserActorPath);
                    Sender.Tell(new HTTPEnterGameResponse(HTTPEnterGameResults.INVALIDACTION));
                    return;
                }

                GITMessage  responseMessage = response as GITMessage;
                byte        result          = (byte) responseMessage.Pop();
                if (result != 0)
                {
                    Sender.Tell(new HTTPEnterGameResponse(HTTPEnterGameResults.INVALIDGAMEID));
                    return;
                }
                double balance              = (double)responseMessage.Pop();
                string strPgGameConfig      = (string)responseMessage.Pop();
                string strLastGameResult    = (string)responseMessage.Pop();

                PGGameConfig    pgGameConfig    = JsonConvert.DeserializeObject<PGGameConfig>(strPgGameConfig);
                dynamic         lastGameResult  = null;
                if(!string.IsNullOrEmpty(strLastGameResult))
                    lastGameResult = JsonConvert.DeserializeObject<dynamic>(strLastGameResult);

                Sender.Tell(new HTTPEnterGameResponse(pgGameConfig, lastGameResult, balance));
                return;
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::sendEnterRequestToUserActor {0}", ex);
            }
            Sender.Tell(new HTTPEnterGameResponse(HTTPEnterGameResults.INVALIDGAMEID));
        }

        private async Task closeHttpSession(string strUserID, string strSessionToken, string strUserActorPath)
        {
            try
            {
                await RedisDatabase.RedisCache.HashDeleteAsync("onlineusers", strSessionToken);
                Context.System.ActorSelection(strUserActorPath).Tell(new HttpSessionClosed(strSessionToken));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::closeHttpSession {0}", ex);
            }
        }

       
    }    
}
