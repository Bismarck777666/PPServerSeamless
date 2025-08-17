using Akka.Actor;
using Akka.Event;
using FrontNode.Database;
using GITProtocol;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace FrontNode.HTTPService
{
    public class HTTPAuthWorker : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public HTTPAuthWorker()
        {
            ReceiveAsync<HTTPAuthRequest>       (onCreateNewToken);
            ReceiveAsync<ApiUserBeginWithdraw>  (onApiUserBeginWithdraw);
            ReceiveAsync<ApiUserEndWithdraw>    (onApiUserEndWithdraw);
            Receive<string>                     (onProcCommand);
        }

        private void onProcCommand(string strCommand)
        {
            if (strCommand == "terminate")
                Self.Tell(PoisonPill.Instance);
        }

        private string createNewSessionToken(string strUserToken)
        {
            return strUserToken + "_" + Guid.NewGuid().ToString();
        }

        private async Task onCreateNewToken(HTTPAuthRequest request)
        {
            try
            {
                GameConfigItem gameConfigItem = DBMonitorSnapshot.Instance.getGameConfigFromSymbol(GameProviders.PP, request.GameSymbol);
                if (gameConfigItem == null)
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
                    return;
                }
                    
                UserLoginResponse loginResponse = await Context.System.ActorSelection("/user/dbproxy/readers").Ask<UserLoginResponse>((object)new UserLoginRequest(request.AgentID, request.UserID, request.PasswordMD5, request.IPAddress, PlatformTypes.WEB), TimeSpan.FromSeconds(10.0));
                if (loginResponse.Result != LoginResult.OK)
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.INVALIDGAMESYMBOL));
                    return;
                }
                if (await RedisDatabase.RedisCache.KeyExistsAsync((RedisKey)("withdraw_" + request.GlobalUserID)))
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
                    return;
                }

                string strNewSesionToken    = createNewSessionToken(loginResponse.GlobalUserID + "@" + loginResponse.PassToken);
                string strHashKey           = string.Format("{0}_tokens", loginResponse.GlobalUserID);

                //유저액터가 이미 존재하는가를 검사한다.
                RedisValue redisValue       = await RedisDatabase.RedisCache.HashGetAsync((RedisKey)"onlineusers", (RedisValue)(loginResponse.GlobalUserID + "_path"));
                if (!redisValue.IsNullOrEmpty)
                {
                    string strUserActorPath = (string)redisValue;
                    await RedisDatabase.RedisCache.HashSetAsync((RedisKey)strHashKey, (RedisValue)strNewSesionToken, (RedisValue)strUserActorPath);
                    Context.System.ActorSelection(strUserActorPath).Tell(new HttpSessionAdded(strNewSesionToken));
                    Sender.Tell(new HTTPAuthResponse(strNewSesionToken, loginResponse.Currency.ToString(), gameConfigItem.Name, gameConfigItem.Data));
                    return;
                }

                //유저새로 로그인
                bool isNotOnline = await RedisDatabase.RedisCache.HashSetAsync((RedisKey)"onlineusers", (RedisValue)loginResponse.GlobalUserID, (RedisValue)true, When.NotExists);
                if (isNotOnline)
                {
                    //새로운 세션토큰을 만든다.
                    IActorRef userActor = await Context.System.ActorSelection("/user/userRouter").Ask<IActorRef>(new CreateNewUserMessage(strNewSesionToken, loginResponse.UserDBID, loginResponse.UserID, loginResponse.UserBalance, loginResponse.PassToken, loginResponse.AgentDBID, loginResponse.AgentID, loginResponse.LastScoreCounter, loginResponse.Currency), TimeSpan.FromSeconds(10.0));
                    string strUserActorPath = userActor.Path.ToString();
                    
                    await RedisDatabase.RedisCache.HashSetAsync((RedisKey)strHashKey, (RedisValue)strNewSesionToken, (RedisValue)strUserActorPath);
                    Sender.Tell(new HTTPAuthResponse(strNewSesionToken, loginResponse.Currency.ToString(), gameConfigItem.Name, gameConfigItem.Data));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onCreateNewToken {0}", ex);
                Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
            }
        }

        private async Task onApiUserBeginWithdraw(ApiUserBeginWithdraw request)
        {
            try
            {
                await RedisDatabase.RedisCache.StringSetAsync((RedisKey)("withdraw_" + request.GlobalUserID), "", TimeSpan.FromSeconds(120.0));
                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync((RedisKey)"onlineusers", (RedisValue)(request.GlobalUserID + "_path"));
                if (!redisValue.IsNullOrEmpty)
                {
                    string strUserActorPath = (string)redisValue;
                    Context.System.ActorSelection(strUserActorPath).Tell(new QuitAndNotifyMessage(), Sender);
                }
                else
                    Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onApiUserBeginWithdraw {0}", ex);
                Sender.Tell(false);
            }
        }

        private async Task onApiUserEndWithdraw(ApiUserEndWithdraw request)
        {
            try
            {
                await RedisDatabase.RedisCache.KeyDeleteAsync((RedisKey)("withdraw_" + request.GlobalUserID));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onApiUserEndWithdraw {0}", ex);
            }
        }

        public class CheckUserPathFromRedis
        {
            public IActorRef    Sender          { get; private set; }
            public string       UserID          { get; private set; }
            public string       SessionToken    { get; private set; }
            public int          RetryCount      { get; private set; }

            public CheckUserPathFromRedis(string strUserID,string strSessionToken,IActorRef sender,int retryCount)
            {
                UserID          = strUserID;
                SessionToken    = strSessionToken;
                Sender          = sender;
                RetryCount      = retryCount;
            }
        }
    }
}
