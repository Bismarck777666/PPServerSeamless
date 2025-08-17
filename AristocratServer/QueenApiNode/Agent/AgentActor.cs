using Akka.Actor;
using Akka.Event;
using BrakePedal;
using GITProtocol.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueenApiNode.HttpService;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using QueenApiNode.Database;
using System.Globalization;
using GITProtocol;
using System.IO;
using System.Security.Cryptography;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace QueenApiNode.Agent
{
    class AgentActor : ReceiveActor
    {
        #region 에이전트정보
        private int _dbID = 0;
        private string _strAgentID = "";
        private double _balance = 0.0;
        private string _strAuthKey = "";
        private string _strWhiteList = "";
        private bool _isEnabled = true;
        private int _moneyMode = 0;
        private int _currency = 0;
        #endregion

        protected static RealExtensions.Epsilon _epsilion = new RealExtensions.Epsilon(0.01);
        private IActorRef _dbReader = null;
        private IActorRef _dbWriter = null;
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        private ThrottlePolicy _gameLogFetchPolicy = new ThrottlePolicy() { Name = "GameLogFetch", PerMinute = 11 };

        public static Props Props(CreateNewAgentActorMsg message)
        {
            return Akka.Actor.Props.Create(() => new AgentActor(message));
        }
        public AgentActor(CreateNewAgentActorMsg message)
        {
            _dbReader = message.DBReader;
            _dbWriter = message.DBWriter;
            _dbID = message.AgentDBID;
            _strAgentID = message.AgentID;
            _balance = message.Balance;
            _strAuthKey = message.AuthToken;
            _strWhiteList = message.WhiteList;
            _moneyMode = message.MoneyMode;
            _currency = message.Currency;
            ReceiveAsync<ApiConsistentRequest>(onAPIRequest);
            Receive<AuthTokenUpdated>(onAgentUpdate);
        }

        private void onAgentUpdate(AuthTokenUpdated updated)
        {
            _strAuthKey = updated.AuthToken;
            _strWhiteList = updated.WhilteList;
        }

        private async Task onAPIRequest(ApiConsistentRequest request)
        {
            try
            {
                if (!_isEnabled)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.AgentDisabled, "Agent has been disabled")));
                    return;
                }

                if (!checkInWhiteList(request.IpAdress))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "IP address is not in whitelist")));
                    _logger.Info("Ipaddress is not in whitelist, agentid:{0}, requestip:{1}, whitelist:{2}", _strAgentID, request.IpAdress, _strWhiteList);
                    return;
                }

                if (request.Request is UserCreateRequest)
                    await onUserCreate(request.Request as UserCreateRequest);
                else if (request.Request is UserGetBalanceRequest)
                    await onUserGetBalance(request.Request as UserGetBalanceRequest);
                else if (request.Request is UserDepositRequest)
                    await onUserDeposit(request.Request as UserDepositRequest, request.IpAdress);
                else if (request.Request is UserWithdrawRequest)
                    await onUserWithdraw(request.Request as UserWithdrawRequest, request.IpAdress);
                else if (request.Request is GetAgentBalanceRequest)
                    onAgentGetBalance(request.Request as GetAgentBalanceRequest);
                else if (request.Request is GetGameListRequest)
                    await onGetGameList(request.Request as GetGameListRequest);
                else if (request.Request is GetBetHistoryRequest)
                    await onUserGameLogRequest(request.Request as GetBetHistoryRequest);
                else if (request.Request is GetGameURLRequest)
                    await onUserGetGameURL(request.Request as GetGameURLRequest);
                else if (request.Request is AgentDepositRequest)
                    onAgentDepositRequest(request.Request as AgentDepositRequest);
                else if (request.Request is AgentWithdrawRequest)
                    onAgentWithdrawRequest(request.Request as AgentWithdrawRequest);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onAPIRequest {0}", ex);
            }
        }
        private async Task onUserCreate(UserCreateRequest request)
        {
            try
            {
                Regex regex = new Regex("^[a-zA-Z0-9]{4,20}$");
                Match match = regex.Match(request.userid);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.UserIDMismatchRule, "The user ID does not match the criteria. The user ID must contain only numbers and alphabetic characters.")));
                    return;
                }
                bool bSuccess = false;
                try
                {
                    bSuccess = await _dbReader.Ask<bool>(new DBCreateUserRequest(request.userid, Guid.NewGuid().ToString(), _dbID, _strAgentID));
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in WebsiteActor::onUserCreate {0}", ex);
                }
                if (!bSuccess)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.DuplicateUserID, "Duplicate user ID. Please use another user ID.")));
                    return;
                }
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.OK, "OK")));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in AgentActor::onUserCreate {0}", ex);
            }
        }
        private void onAgentGetBalance(GetAgentBalanceRequest request)
        {
            try
            {
                Sender.Tell(JsonConvert.SerializeObject(new GetBalanceResponse(Math.Round(_balance, 2))));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserGetBalance {0}", ex);
            }
        }
        private async Task onGetGameList(GetGameListRequest request)
        {
            try
            {
                List<ApiGame> gameList = await _dbReader.Ask<List<ApiGame>>(request, TimeSpan.FromSeconds(5.0));
                Sender.Tell(JsonConvert.SerializeObject(new GetGameListResponse(gameList)));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onGetGameList {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private async Task onUserGetBalance(UserGetBalanceRequest request)
        {
            try
            {
                double userBalance = await _dbReader.Ask<double>(new DBGetUserBalanceRequest(_dbID, request.userid), TimeSpan.FromSeconds(5.0));
                if (double.IsNegativeInfinity(userBalance))
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                else
                    Sender.Tell(JsonConvert.SerializeObject(new GetBalanceResponse(Math.Round(userBalance, 2))));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserGetBalance {0}", ex);
            }
        }
        private async Task onUserDeposit(UserDepositRequest request, string ip)
        {
            try
            {
                double amount = Math.Round(request.amount, 2);
                if (_moneyMode == 2)
                    amount = Math.Round(amount / 1000.0, 2);

                if (_balance.LT(amount, _epsilion) || amount <= 0.0 || double.IsNaN(amount) || double.IsInfinity(amount))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.NotEnoughAgentBalance, "agent balance not enough")));
                    return;
                }

                DBUserDepositResponse userDepositResponse = await _dbReader.Ask<DBUserDepositResponse>(new DBUserDepositRequest(_dbID, request.userid, amount), TimeSpan.FromSeconds(5.0));
                if (!userDepositResponse.IsSuccess)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid userid")));
                }
                else
                {
                    _balance -= amount;
                    _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, -amount));
                    _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, request.userid, amount, _balance + amount, _balance, AgentMoneyChangeModes.USERDEPOSIT, DateTime.Now));
                    _dbWriter.Tell(new UserMoneyChangeItem(_strAgentID, request.userid, amount, UserMoneyChangeModes.DEPOSIT,
                        userDepositResponse.Before, userDepositResponse.After, ip, DateTime.Now));

                    Context.System.ActorSelection("/user/commServers").Tell(new ApiDepositMessage(_dbID, request.userid,
                        amount, userDepositResponse.LastScoreCounter));

                    Sender.Tell(JsonConvert.SerializeObject(new GetBalanceResponse(Math.Round(userDepositResponse.After, 2))));
                }
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in WebsiteActor::onUserDeposit {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private async Task onUserWithdraw(UserWithdrawRequest request, string ipAddress)
        {
            string strUserID = request.userid;
            try
            {
                DBGetUserInfoResponse infoResponse = await _dbReader.Ask<DBGetUserInfoResponse>(new DBGetUserInfoRequest(_dbID, request.userid), TimeSpan.FromSeconds(5.0));
                if (infoResponse == null)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                    return;
                }
                strUserID = infoResponse.UserName;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserWithdraw {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
                return;
            }

            try
            {
                double amount = Math.Round(request.amount, 2);
                if (_moneyMode == 2)
                {
                    if (amount > 0.0)
                        amount = Math.Round(amount / 1000.0, 2);
                }
                if (amount == 0.0 || double.IsNaN(amount) || double.IsInfinity(amount))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidAmount, "invalid amount")));
                    return;
                }
                bool quitResult = await Context.System.ActorSelection("/user/commAuthWorkers").Ask<bool>(
                    new ApiWithdrawRequest(_dbID, strUserID, amount), TimeSpan.FromSeconds(10.0));

                if (!quitResult)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "General Error")));
                    return;
                }
                ApiWithdrawResponse response = await _dbReader.Ask<ApiWithdrawResponse>(new ApiWithdrawRequest(_dbID, strUserID, amount), TimeSpan.FromSeconds(31.0));
                if (response.Result != 0)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                    return;
                }

                double withdrawedAmount = response.BeforeScore - response.AfterScore;
                _balance += withdrawedAmount;
                if (withdrawedAmount > 0.0)
                {
                    _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, amount));
                    _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, request.userid, amount, _balance - amount, _balance, AgentMoneyChangeModes.USERWITHDRAW, DateTime.Now));
                    _dbWriter.Tell(new UserMoneyChangeItem(_strAgentID, request.userid, amount, UserMoneyChangeModes.WITHDRAW, response.BeforeScore, response.AfterScore, ipAddress, DateTime.Now));
                }

                Sender.Tell(JsonConvert.SerializeObject(new UserWithdrawResponse(Math.Round(withdrawedAmount, 2), Math.Round(response.AfterScore, 2))));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserWithdraw {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
            finally
            {
                Context.System.ActorSelection("/user/commAuthWorkers").Tell(new ApiWithdrawCompleted(_dbID, strUserID));
            }
        }
        private async Task onUserGetGameURL(GetGameURLRequest request)
        {
            try
            {
                DBGetUserInfoResponse response = await _dbReader.Ask<DBGetUserInfoResponse>(new DBGetUserInfoRequest(_dbID, request.userid), TimeSpan.FromSeconds(5.0));
                if (response == null)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                    return;
                }
                if (!response.IsEnabled)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                    return;
                }
                if (request.gameid < 0)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidGameID, "invalid game id")));
                    return;
                }
                GAMETYPE gameType = DBMonitorSnapshot.Instance.getGameType(request.gameid);
                if (gameType == GAMETYPE.NONE)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidGameID, "invalid game id")));
                    return;
                }
                if (gameType == GAMETYPE.PP)
                {
                    string strGameSymbol = DBMonitorSnapshot.Instance.getGameSymbolFromID(gameType, request.gameid);
                    string token = string.Format("{0}_{1}", response.Password, DateTime.Now.AddMinutes(3.0).ToString("yyyy-MM-dd HH:mm:ss"));



                    string strGameURL = string.IsNullOrEmpty(ApiConfig.Local) ? _currency != 7 ? string.Format("https://{5}.{4}.com/pp/openGame?&userid={0}&token={1}&symbol={2}&lang={3}", response.UserName, encryptString(token, "fd973747b689447fb360c89f9612baa8"), strGameSymbol,
                        string.IsNullOrEmpty(request.lang) ? "en" : request.lang, ApiConfig.Domain, ApiConfig.SubGame) :
                         string.Format("https://{5}.{4}.net/pp/openGame?&userid={0}&token={1}&symbol={2}&lang={3}", response.UserName, encryptString(token, "fd973747b689447fb360c89f9612baa8"), strGameSymbol,
                        string.IsNullOrEmpty(request.lang) ? "en" : request.lang, ApiConfig.Domain, ApiConfig.SubGame)
                        : string.Format("{4}/pp/openGame?&userid={0}&token={1}&symbol={2}&lang={3}", response.UserName, encryptString(token, "fd973747b689447fb360c89f9612baa8"), strGameSymbol,
                        string.IsNullOrEmpty(request.lang) ? "en" : request.lang, ApiConfig.Local);

                    Sender.Tell(JsonConvert.SerializeObject(new GetGameURLResponse(strGameURL)));
                }
                else if (gameType == GAMETYPE.ARISTO)
                {
                    string strGameSymbol = DBMonitorSnapshot.Instance.getGameSymbolFromID(gameType, request.gameid);
                    string token = string.Format("{0}_{1}", response.Password, DateTime.Now.AddMinutes(3.0).ToString("yyyy-MM-dd HH:mm:ss"));
                    //encryptString(token, "fd973747b689447fb360c89f9612baa8")
                    string strGameURL = "";
                    if (request.gameid == 1000)
                    {
                        strGameURL = string.Format("http://localhost:5155/{2}/index.html?device=desktop&nogslang={3}&nogsmode=real&nogscurrency=THB&lobbyurl=http%3A%2F%2Flocalhost%3A5155&sessionid={1}&nogsserver=http%3A%2F%2Flocalhost%3A5155%2F1000%2F&countrycode=ZH&locale=zh_CN&id_domain=sslgamesasia&bbm=0&userid={0}&gamesymbol={4}", response.UserName, Convert.ToBase64String(Encoding.UTF8.GetBytes(token)), request.gameid, string.IsNullOrEmpty(request.lang) ? "en" : request.lang, strGameSymbol);
                    }
                    else
                    {
                        strGameURL = string.Format("http://localhost:5155/{2}/index.html?session={1}&language={3}&s=7&userid={0}&gamesymbol={4}", response.UserName, Convert.ToBase64String(Encoding.UTF8.GetBytes(token)), request.gameid, string.IsNullOrEmpty(request.lang) ? "en" : request.lang, strGameSymbol);
                    }

                    Sender.Tell(JsonConvert.SerializeObject(new GetGameURLResponse(strGameURL)));
                }
                else
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer 3tgsH6SGQdDTJeRf7KYk6bwHkXaDKhrXFXRWdTpdW1hyJimoZgD6cJU25YxkOyGJ");
                    APUserAuthRequest userAuthRequest = new APUserAuthRequest();
                    userAuthRequest.userID = request.userid;
                    userAuthRequest.agentID = _strAgentID;
                    userAuthRequest.gameID = request.gameid;
                    userAuthRequest.lang = string.IsNullOrEmpty(request.lang) ? "en" : request.lang;
                    string strPostContent = JsonConvert.SerializeObject(userAuthRequest);
                    var httpResponse = await httpClient.PostAsync(string.Format("https://{1}.{0}.com/userAuth", ApiConfig.Domain, ApiConfig.SUbAdvent), new StringContent(strPostContent, Encoding.UTF8, "application/json"));
                    httpResponse.EnsureSuccessStatusCode();
                    string strContent = await httpResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<APUserAuthResponse>(strContent);
                    if (!string.IsNullOrEmpty(apiResponse.url))
                        Sender.Tell(JsonConvert.SerializeObject(new GetGameURLResponse(apiResponse.url)));
                    else
                        Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserGetGameURL {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private string createNewSessionToken(string strUserToken, int currency)
        {
            string strSessionToken = strUserToken + "_" + Guid.NewGuid().ToString() + currency.ToString();
            if (currency == 10)
                strSessionToken = strUserToken + "_" + Guid.NewGuid().ToString() + "0-";
            return strSessionToken;
        }
        private string encryptString(string plainText, string key)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return System.Web.HttpServerUtility.UrlTokenEncode(array);
        }
        private async Task onUserGameLogRequest(GetBetHistoryRequest request)
        {
            try
            {
                CheckResult checkResult = _gameLogFetchPolicy.Check(new SimpleThrottleKey(_strAgentID));
                if (checkResult.IsThrottled)
                {
                    _logger.Warning("{0}'s game log fetch request count overflowed", _strAgentID);
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.RateLimited, "rate limited")));
                    return;
                }

                if (request.logcount <= 0)
                    request.logcount = 2000;
                else if (request.logcount > 2000)
                    request.logcount = 2000;

                string response = await _dbReader.Ask<string>(new DBUserGameLogRequest(_dbID, request.lastobjectid, request.logcount, request.startdate, request.enddate), TimeSpan.FromSeconds(5));
                Sender.Tell(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in AgentActor::onUserGameLogRequest {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }

        private void onAgentDepositRequest(AgentDepositRequest request)
        {
            try
            {
                if (request.amount < 0.0 || double.IsNaN(request.amount) || double.IsInfinity(request.amount))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidAmount, "Invalid Amount")));
                    return;
                }

                _balance += request.amount;
                _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, request.amount));
                _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, "Admin", request.amount, _balance - request.amount, _balance, AgentMoneyChangeModes.UPDEPOSIT, DateTime.Now));

                Sender.Tell(JsonConvert.SerializeObject(new GetBalanceResponse(Math.Round(_balance, 2))));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onAgentDepositRequest {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private void onAgentWithdrawRequest(AgentWithdrawRequest request)
        {
            try
            {
                if (request.amount < 0.0 || double.IsNaN(request.amount) || double.IsInfinity(request.amount) || _balance.LT(request.amount, _epsilion))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidAmount, "Invalid Amount")));
                    return;
                }

                _balance -= request.amount;
                _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, -request.amount));
                _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, "Admin", request.amount, _balance + request.amount, _balance, AgentMoneyChangeModes.UPWITHDRAW, DateTime.Now));

                Sender.Tell(JsonConvert.SerializeObject(new GetBalanceResponse(Math.Round(_balance, 2))));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onAgentWithdrawRequest {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private bool checkInWhiteList(string ip)
        {
            if (string.IsNullOrEmpty(_strWhiteList))
                return true;

            string[] whiteListArr = _strWhiteList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string whiteIp in whiteListArr)
            {
                if (whiteIp.Trim().Equals(ip.Trim(), StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }


    }
    public class APUserAuthRequest
    {
        public string agentID { get; set; }
        public string userID { get; set; }
        public string lang { get; set; }
        public int gameID { get; set; }
    }
    public class APUserAuthResponse
    {
        public int code { get; set; }
        public string url { get; set; }

    }
}
