using Akka.Actor;
using Akka.Event;
using GITProtocol;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using CommNode.Database;
using Akka.Cluster;

namespace CommNode.HTTPService
{
    public class HTTPCallbackWorkActor : ReceiveActor
    {
        private IActorRef _dbReaderProxy = null;
        private IActorRef _dbWriterProxy = null;
        private IActorRef _redisWriter   = null;
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public HTTPCallbackWorkActor(IActorRef dbReaderProxy, IActorRef dbWriterProxy, IActorRef redisWriter)
        {
            _dbReaderProxy = dbReaderProxy;
            _dbWriterProxy = dbWriterProxy;
            _redisWriter   = redisWriter;

            ReceiveAsync<CallbackGetBalanceRequest> (onGetBalance);
            ReceiveAsync<CallbackWithdrawRequest>   (onWithdrawRequest);
            ReceiveAsync<CallbackDepositRequest>    (onDepositRequest);
            ReceiveAsync<CallbackRollbackRequest>   (onRollbackRequest);
            Receive<string>                         (onProcCommand);
        }
        private void onProcCommand(string strCommand)
        {
            if (strCommand == "terminate")
            {
                Self.Tell(PoisonPill.Instance);
            }
        }
        public static string createDataSign(string key, string message)
        {
            var hmac = System.Security.Cryptography.HMAC.Create("HMACSHA256");
            hmac.Key = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }
        private async Task onGetBalance(CallbackGetBalanceRequest request)
        {
            try
            {
                string strSign = createDataSign(HTTPServiceConfig.Instance.CallbackSecretKey, string.Format("{0}{1}{2}", request.agentID,
                    request.userID, request.gameID));
                if (strSign != request.sign)
                {
                    Sender.Tell(new CallbackGetBalanceResponse(3, "Invalid Sign", 0.0M));
                    return;
                }
                string strUserActorPath = await getUserActorPath(request.userID);
                if(strUserActorPath == null)
                {
                    Sender.Tell(new CallbackGetBalanceResponse(5, "User Id not found", 0.0M));
                    return;
                }
                var response = await Context.System.ActorSelection(strUserActorPath).Ask<CallbackGetBalanceResponse>(request, TimeSpan.FromSeconds(10.0));
                Sender.Tell(response);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPCallbackWorkActor::onGetBalance {0}", ex);
                Sender.Tell(new CallbackGetBalanceResponse(1, "General Error", 0.0M));
            }
        }
        private async Task onWithdrawRequest(CallbackWithdrawRequest request)
        {
            try
            {
                string strSign = createDataSign(HTTPServiceConfig.Instance.CallbackSecretKey, string.Format("{0}{1}{2}{3}{4}{5}", request.agentID,
                    request.userID, request.amount.ToString("0.00"), request.transactionID, request.roundID, request.gameID));
                if (strSign != request.sign)
                {
                    Sender.Tell(new CallbackWithdrawResponse(3, "Invalid Sign"));
                    return;
                }
                string strUserActorPath = await getUserActorPath(request.userID);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new CallbackWithdrawResponse(5, "User Id not found"));
                    return;
                }

                CallbackWithdrawResponse response = await Context.System.ActorSelection(strUserActorPath).Ask<CallbackWithdrawResponse>(request, TimeSpan.FromSeconds(10.0));
                Sender.Tell(response);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPCallbackWorkActor::onWithdrawRequest {0}", ex);
                Sender.Tell(new CallbackWithdrawResponse(1, "General Error"));

            }
        }
        private async Task onDepositRequest(CallbackDepositRequest request)
        {
            try
            {
                string strSign = createDataSign(HTTPServiceConfig.Instance.CallbackSecretKey, string.Format("{0}{1}{2}{3}{4}{5}{6}", request.agentID,
                    request.userID, request.amount.ToString("0.00"), request.refTransactionID, request.transactionID, request.roundID, request.gameID));

                if (strSign != request.sign)
                {
                    Sender.Tell(new CallbackDepositResponse(3, "Invalid Sign"));
                    return;
                }
                string strUserActorPath = await getUserActorPath(request.userID);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new CallbackDepositResponse(5, "User Id not found"));
                    return;
                }

                CallbackDepositResponse response = await Context.System.ActorSelection(strUserActorPath).Ask<CallbackDepositResponse>(request, TimeSpan.FromSeconds(10.0));
                Sender.Tell(response);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPCallbackWorkActor::onDepositRequest {0}", ex);
                Sender.Tell(new CallbackDepositResponse(1, "General Error"));

            }
        }
        private async Task onRollbackRequest(CallbackRollbackRequest request)
        {
            try
            {
                string strSign = createDataSign(HTTPServiceConfig.Instance.CallbackSecretKey, string.Format("{0}{1}{2}{3}", request.agentID,
                    request.userID, request.refTransactionID, request.gameID));

                if (strSign != request.sign)
                {
                    Sender.Tell(new CallbackRollbackResponse(3, "Invalid Sign"));
                    return;
                }
                string strUserActorPath = await getUserActorPath(request.userID);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new CallbackRollbackResponse(5, "User Id not found"));
                    return;
                }

                CallbackRollbackResponse response = await Context.System.ActorSelection(strUserActorPath).Ask<CallbackRollbackResponse>(request, TimeSpan.FromSeconds(10.0));
                Sender.Tell(response);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPCallbackWorkActor::onRollbackRequest {0}", ex);
                Sender.Tell(new CallbackRollbackResponse(1, "General Error"));

            }
        }
        private async Task<string> getUserActorPath(string strUserID)
        {
            try
            {
                string strApiUserToken  = strUserID + "apiToken";
                string strHashKey       = string.Format("{0}_tokens", strUserID);
                RedisValue redisValue   = await RedisDatabase.RedisCache.HashGetAsync(strHashKey, strApiUserToken);
                if (!redisValue.IsNullOrEmpty)
                    return redisValue.ToString();

                UserLoginResponse loginResponse = await Context.System.ActorSelection("/user/dbproxy/readers").Ask<UserLoginResponse>(new ApiUserLoginRequest(strUserID));
                if (loginResponse.Result != LoginResult.OK)
                    return null;

                bool exists = await RedisDatabase.RedisCache.KeyExistsAsync("withdraw_" + strUserID);
                if (exists)
                    return null;
 
                redisValue = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", strUserID + "_path");
                if (!redisValue.IsNullOrEmpty)
                {
                    string strUserActorPath = (string)redisValue;
                    await RedisDatabase.RedisCache.HashSetAsync(strHashKey, strApiUserToken, strUserActorPath);

                    Context.System.ActorSelection(strUserActorPath).Tell(new HttpSessionAdded(strApiUserToken));
                    return strUserActorPath;
                }

                bool isNotOnline = await RedisDatabase.RedisCache.HashSetAsync("onlineusers", strUserID, true, StackExchange.Redis.When.NotExists);
                if (isNotOnline)
                {
                    IActorRef userActor = await Context.System.ActorSelection("/user/userManager").Ask<IActorRef>(new CreateNewUserMessage(strApiUserToken, _dbReaderProxy, _dbWriterProxy, _redisWriter, loginResponse, PlatformTypes.WEB));
                    string strUserActorPath = getActorRemotePath(userActor);

                    await RedisDatabase.RedisCache.HashSetAsync(strHashKey, strApiUserToken, strUserActorPath);
                    return strUserActorPath;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPCallbackWorkActor::getUserActorPath {0}", ex);
                return null;
            }
        }
        private string getActorRemotePath(IActorRef actor)
        {
            string strActorPath = actor.Path.ToString();
            string strClusterAddress = Cluster.Get(Context.System).SelfAddress.ToString();
            int start = strActorPath.IndexOf("/user");
            string strRemotePath = strClusterAddress + strActorPath.Substring(start);
            return strRemotePath;
        }
    }
}
