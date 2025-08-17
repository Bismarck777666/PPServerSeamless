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
using System.Net.Http;
using Newtonsoft.Json;

namespace CommNode.HTTPService
{
    public class HTTPAuthWorker : ReceiveActor
    {
        private IActorRef _dbReaderProxy = null;
        private IActorRef _dbWriterProxy = null;
        private IActorRef _redisWriter = null;
        private HttpClient _httpClient = new HttpClient();

        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        public HTTPAuthWorker(IActorRef dbReaderProxy, IActorRef dbWriterProxy, IActorRef redisWriter)
        {
            _dbReaderProxy = dbReaderProxy;
            _dbWriterProxy = dbWriterProxy;
            _redisWriter = redisWriter;

            ReceiveAsync<HTTPAuthRequest>(onCreateNewToken);
            ReceiveAsync<CheckUserPathFromRedis>(onCheckUserPathFromRedis);
            ReceiveAsync<WithdrawRequest>(onUserWithdrawRequest);
            ReceiveAsync<ApiWithdrawRequest>(onApiWithdrawMessage);
            ReceiveAsync<ApiWithdrawCompleted>(onApiWithdrawCompleted);
            Receive<string>(onProcCommand);

        }

        private void onProcCommand(string strCommand)
        {
            if (strCommand != "terminate")
                return;

            Self.Tell(PoisonPill.Instance);
        }

        private async Task onApiWithdrawCompleted(ApiWithdrawCompleted request)
        {
            try
            {
                await RedisDatabase.RedisCache.KeyDeleteAsync("withdraw_" + request.UserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onApiWithdrawMessage {0}", ex);
            }
        }

        private async Task onApiWithdrawMessage(ApiWithdrawRequest request)
        {
            try
            {
                await RedisDatabase.RedisCache.StringSetAsync("withdraw_" + request.UserID, "", TimeSpan.FromSeconds(120.0));

                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", request.UserID + "_path");
                if (!redisValue.IsNullOrEmpty)
                {
                    string strUserActorPath = (string)redisValue;
                    Context.System.ActorSelection(strUserActorPath).Tell(new QuitAndNotifyMessage(), Sender);
                }
                else
                {
                    Sender.Tell(true);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onApiWithdrawMessage {0}", ex);
                Sender.Tell(false);
            }
        }
        private async Task onUserWithdrawRequest(WithdrawRequest request)
        {
            try
            {
                if (request.IsStart)
                {
                    UserLoginResponse loginResponse = await Context.System.ActorSelection("/user/dbproxy/readers").Ask<UserLoginResponse>(new UserLoginRequest(request.UserID, request.Password, "", PlatformTypes.WEB));
                    if (loginResponse.Result != LoginResult.OK)
                    {
                        Sender.Tell(WithdrawResults.INVALIDACCOUNT);
                        return;
                    }

                    await RedisDatabase.RedisCache.StringSetAsync("withdraw_" + request.UserID, "", TimeSpan.FromSeconds(60));

                    RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", request.UserID + "_path");
                    if (!redisValue.IsNullOrEmpty)
                    {
                        string strUserActorPath = (string)redisValue;
                        Context.System.ActorSelection(strUserActorPath).Tell(new QuitUserMessage(request.UserID));
                    }
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(3.5), Sender, WithdrawResults.OK, Self);
                }
                else
                {
                    UserLoginResponse loginResponse = await Context.System.ActorSelection("/user/dbproxy/readers").Ask<UserLoginResponse>(new UserLoginRequest(request.UserID, request.Password, "", PlatformTypes.WEB));
                    if (loginResponse.Result != LoginResult.OK)
                    {
                        Sender.Tell(WithdrawResults.INVALIDACCOUNT);
                        return;
                    }
                    await RedisDatabase.RedisCache.KeyDeleteAsync("withdraw_" + request.UserID);
                    Sender.Tell(WithdrawResults.OK);

                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onUserWithdrawRequest {0}", ex);
                Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));

            }
        }
        private async Task onCreateNewToken(HTTPAuthRequest request)
        {
            try
            {
                UserLoginResponse loginResponse = await Context.System.ActorSelection("/user/dbproxy/readers").Ask<UserLoginResponse>(new UserLoginRequest(request.UserID, request.PasswordMD5, request.IPAddress, PlatformTypes.WEB));
                if (loginResponse.Result != LoginResult.OK)
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
                    return;
                }

                bool exists = await RedisDatabase.RedisCache.KeyExistsAsync("withdraw_" + request.UserID);
                if (exists)
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
                    return;
                }
                if (DBMonitorSnapshot.Instance.IsNowMaintenance && loginResponse.AgentName != "devagent")
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.SERVERMAINTENANCE));
                    return;
                }
                string strGameData = DBMonitorSnapshot.Instance.getGameData(request.GameSymbol);
                if (string.IsNullOrEmpty(strGameData))
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
                    return;
                }
                if (loginResponse.IPRestriction && !await checkIPCountry(loginResponse.UserID, loginResponse.AgentCurrency, request.IPAddress))
                {
                    Sender.Tell(new HTTPAuthResponse(HttpAuthResults.COUNTRYMISMATCH));
                    return;
                }


                string strNewSesionToken = createNewSessionToken(loginResponse.UserID + "@" + request.PasswordMD5, (int)loginResponse.AgentCurrency);
                string strHashKey = string.Format("{0}_tokens", loginResponse.UserID);

                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", request.UserID + "_path");
                if (!redisValue.IsNullOrEmpty)
                {
                    string strUserActorPath = (string)redisValue;
                    await RedisDatabase.RedisCache.HashSetAsync(strHashKey, strNewSesionToken, strUserActorPath);

                    Context.System.ActorSelection(strUserActorPath).Tell(new HttpSessionAdded(strNewSesionToken));
                    Sender.Tell(new HTTPAuthResponse(strNewSesionToken, (int)loginResponse.AgentCurrency, strGameData));
                    return;
                }

                bool isNotOnline = await RedisDatabase.RedisCache.HashSetAsync("onlineusers", request.UserID, true, StackExchange.Redis.When.NotExists);
                if (isNotOnline)
                {
                    IActorRef userActor = await Context.System.ActorSelection("/user/userManager").
                        Ask<IActorRef>(new CreateNewUserMessage(strNewSesionToken, _dbReaderProxy, _dbWriterProxy, _redisWriter, loginResponse, PlatformTypes.WEB));
                    string strUserActorPath = getActorRemotePath(userActor);

                    await RedisDatabase.RedisCache.HashSetAsync(strHashKey, strNewSesionToken, strUserActorPath);
                    Sender.Tell(new HTTPAuthResponse(strNewSesionToken, (int)loginResponse.AgentCurrency, strGameData));
                    return;
                }

                Context.System.Scheduler.ScheduleTellOnce(0, Self, new CheckUserPathFromRedis(request.UserID, (int)loginResponse.AgentCurrency, strGameData, strNewSesionToken, Sender, 10), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPAuthWorker::onCreateNewToken {0}", ex);
                Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
            }
        }

        private async Task onCheckUserPathFromRedis(CheckUserPathFromRedis request)
        {
            try
            {
                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", request.UserID + "_path");
                if (!redisValue.IsNullOrEmpty)
                {
                    string strUserActorPath = (string)redisValue;

                    Context.ActorSelection(strUserActorPath).Tell(new HttpSessionAdded(request.SessionToken));

                    string strHashKey = string.Format("{0}_tokens", request.UserID);
                    await RedisDatabase.RedisCache.HashSetAsync(strHashKey, request.SessionToken, strUserActorPath);

                    Context.System.ActorSelection(strUserActorPath).Tell(new HttpSessionAdded(request.SessionToken));
                    request.Sender.Tell(new HTTPAuthResponse(request.SessionToken, request.Currency, request.GameData));
                    return;
                }

                if (request.RetryCount <= 0)
                {
                    request.Sender.Tell(new HTTPAuthResponse(HttpAuthResults.IDPASSWORDERROR));
                    return;
                }

                Context.System.Scheduler.ScheduleTellOnce(500, Self, new CheckUserPathFromRedis(request.UserID, request.Currency, request.GameData, request.SessionToken, request.Sender, request.RetryCount - 1), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::onCheckRedisUserToken {0}", ex);
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

        private string createNewSessionToken(string strUserToken, int currency)
        {
            string strSessionToken = strUserToken + "_" + Guid.NewGuid().ToString() + currency.ToString();
            if (currency == 10)
                strSessionToken = strUserToken + "_" + Guid.NewGuid().ToString() + "0-";
            return strSessionToken;
        }

        private async Task<bool> checkIPCountry(string userID, Currencies currency, string ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1" || ipAddress == "127.0.0.1")
                    return true;

                string strURL = string.Format("https://pro.ip-api.com/json/{0}?key=m18pcBm8ePPfVHZ", ipAddress);
                var response = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();
                var responseText = await response.Content.ReadAsStringAsync();
                IPApiResponse apiResponse = JsonConvert.DeserializeObject<IPApiResponse>(responseText);
                if (currency == Currencies.MYR && apiResponse.countryCode == "MY")
                    return true;
                else if (currency == Currencies.SGD && (apiResponse.countryCode == "SG" || apiResponse.countryCode == "BN"))
                    return true;
                else if (currency == Currencies.AUD && apiResponse.countryCode == "AU")
                    return true;
                else if (currency == Currencies.THB && apiResponse.countryCode == "TH")
                    return true;
                else if (currency == Currencies.USD && apiResponse.countryCode == "KH")
                    return true;
                else if (currency == Currencies.MMK && apiResponse.countryCode == "MM")
                    return true;
                else if (currency == Currencies.HKD && apiResponse.countryCode == "HK")
                    return true;
                else if (currency == Currencies.IDR && apiResponse.countryCode == "ID")
                    return true;
                else if (currency == Currencies.INR && apiResponse.countryCode == "IN")
                    return true;
                else if (currency == Currencies.BDT && apiResponse.countryCode == "BD")
                    return true;
                else if (currency == Currencies.CNY && apiResponse.countryCode == "CN")
                    return true;

                _logger.Error($"Wrong IP user: {userID}, {currency}, {ipAddress}, {apiResponse.countryCode}");

                return false;
                
            }
            catch
            {
                _logger.Error("IP API is not working");
                return true;
            }
        }

        public class CheckUserPathFromRedis
        {
            public IActorRef Sender { get; private set; }
            public string UserID { get; private set; }
            public int Currency { get; private set; }
            public string GameData { get; private set; }
            public string SessionToken { get; private set; }
            public int RetryCount { get; private set; }
            public CheckUserPathFromRedis(string strUserID, int currency, string gameData, string strSessionToken, IActorRef sender, int retryCount)
            {
                this.UserID = strUserID;
                this.Currency = currency;
                this.GameData = gameData;
                this.SessionToken = strSessionToken;
                this.Sender = sender;
                this.RetryCount = retryCount;
            }
        }
    }
}
