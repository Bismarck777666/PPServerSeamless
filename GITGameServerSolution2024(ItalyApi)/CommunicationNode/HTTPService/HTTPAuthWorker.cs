using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using CommNode.Database;
using StackExchange.Redis;
using Akka.Cluster;
using GITProtocol;
using CommNode.HTTPService.Models;

/***
 * 
 *          Created by Foresight(2021.03.18)
 *          인증조작을 진행하는 클래스
 *          유저의 아이디, 비번을 받아 인증하여 결과에 따라 세션토큰을 발급한다.
 * 
 */
namespace CommNode.HTTPService
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

            ReceiveAsync<HTTPAuthRequest>           (onCreateNewToken);     
            ReceiveAsync<ApiWithdrawRequest>        (onApiWithdrawMessage);
            Receive<string>                         (onProcCommand);
        }

        private void onProcCommand(string strCommand)
        {
            if (strCommand == "terminate")
            {
                Self.Tell(PoisonPill.Instance);
            }
        }

        private async Task onCreateNewToken(HTTPAuthRequest request)
        {
            try
            {
                //디비에서 조회한다.
                UserLoginResponse loginResponse = await Context.System.ActorSelection("/user/dbproxy/readers").Ask<UserLoginResponse>(new UserLoginRequest(request.AgentID, request.UserID, request.PasswordMD5, request.IPAddress, PlatformTypes.WEB));
                if(loginResponse.Result != LoginResult.OK)
                {
                    //실패결과 리턴
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
                    return;
                }

                bool exists = await RedisDatabase.RedisCache.KeyExistsAsync("withdraw_" + loginResponse.GlobalUserID);
                if(exists)
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
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
                    Sender.Tell(new HTTPAuthResponse(strNewSesionToken));
                    return;
                }

                //유저새로 로그인
                bool isNotOnline = await RedisDatabase.RedisCache.HashSetAsync("onlineusers", loginResponse.GlobalUserID, true, StackExchange.Redis.When.NotExists);
                if (isNotOnline)
                {
                    //새로운 세션토큰을 만든다.
                    IActorRef   userActor           = await Context.System.ActorSelection("/user/userManager").Ask<IActorRef>(new CreateNewUserMessage(strNewSesionToken, _dbReaderProxy, _dbWriterProxy, _redisWriter, loginResponse, PlatformTypes.WEB));
                    string      strUserActorPath    = getActorRemotePath(userActor);

                    await RedisDatabase.RedisCache.HashSetAsync(strHashKey, strNewSesionToken, strUserActorPath);
                    Sender.Tell(new HTTPAuthResponse(strNewSesionToken));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onCreateNewToken {0}", ex);
                Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
            }
        }

        private async Task onApiWithdrawMessage(ApiWithdrawRequest request)
        {
            try
            {
                string strUserActorPath = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", request.GlobalUserID + "_path");
                if (!string.IsNullOrEmpty(strUserActorPath))
                    await Context.System.ActorSelection(strUserActorPath).Ask(new QuitAndNotifyMessage(), TimeSpan.FromSeconds(5.0));
                
                ApiWithdrawResponse response = await _dbReaderProxy.Ask<ApiWithdrawResponse>(request, TimeSpan.FromSeconds(31.0));
                Sender.Tell(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onApiWithdrawMessage {0}", ex.ToString());
                Sender.Tell(new ApiWithdrawResponse(2, 0.0, 0.0));
            }
        }

        private string getActorRemotePath(IActorRef actor)
        {
            string  strActorPath        = actor.Path.ToString();
            string  strClusterAddress   = Cluster.Get(Context.System).SelfAddress.ToString();
            int     start               = strActorPath.IndexOf("/user");
            string  strRemotePath       = strClusterAddress + strActorPath.Substring(start);
            return  strRemotePath;
        }

        private string createNewSessionToken(string strUserToken)
        {
            string strSessionToken = strUserToken + "_" + Guid.NewGuid().ToString();
            return strSessionToken;
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
