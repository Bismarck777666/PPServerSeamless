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
using System.Diagnostics;

namespace QueenApiNode.Agent
{
    class AgentActor : ReceiveActor
    {
        #region 에이전트정보
        private int     _dbID           = 0;
        private string  _strAgentID     = "";
        private double  _balance        = 0.0;
        private int     _currency       = 0;
        private string  _strAuthKey     = "";
        private bool    _isEnabled      = true;
        #endregion

        protected static RealExtensions.Epsilon _epsilion   = new RealExtensions.Epsilon(0.01);
        private          IActorRef              _dbReader   = null;
        private          IActorRef              _dbWriter   = null;
        private readonly ILoggingAdapter        _logger     = Logging.GetLogger(Context);

        private ThrottlePolicy _gameLogFetchPolicy   = new ThrottlePolicy() { Name = "GameLogFetch",  PerMinute = 11 };
        private ThrottlePolicy _userCashFlowPolicy   = new ThrottlePolicy() { Name = "CashFlowFetch", PerMinute = 30 };
        private ThrottlePolicy _allUserBalancePolicy = new ThrottlePolicy() { Name = "CashFlowFetch", PerMinute = 10 };

        public static Props Props(CreateNewAgentActorMsg message)
        {
            return Akka.Actor.Props.Create(() => new AgentActor(message));
        }
        public AgentActor(CreateNewAgentActorMsg message)
        {
            _dbReader   = message.DBReader;
            _dbWriter   = message.DBWriter;
            _dbID       = message.AgentDBID;
            _strAgentID = message.AgentID;
            _currency   = message.Currency;
            _balance    = message.Balance;
            _strAuthKey = message.AuthToken;

            ReceiveAsync<ApiConsistentRequest>  (onAPIRequest);
            Receive<SubtractEventMoneyRequest>  (onSubtractEventMoney);
            Receive<AddEventLeftMoneyRequest>   (onAddEventLeftMoney);

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

                if (request.Request is UserCreateRequest)
                    await onUserCreate(request.Request as UserCreateRequest);
                else if (request.Request is UserGetBalanceRequest)
                    await onUserGetBalance(request.Request as UserGetBalanceRequest);
                else if (request.Request is UserDepositRequest)
                    await onUserDeposit(request.Request as UserDepositRequest);
                else if (request.Request is UserWithdrawRequest)
                    await onUserWithdraw(request.Request as UserWithdrawRequest);
                else if (request.Request is GetAgentBalanceRequest)
                    onAgentGetBalance(request.Request as GetAgentBalanceRequest);
                else if (request.Request is GetGameListRequest)
                    onGetGameList(request.Request as GetGameListRequest);
                else if (request.Request is GetVenderRequest)
                    onGetProviderList(request.Request as GetVenderRequest);
                else if (request.Request is GetBetHistoryRequest)
                    await onUserGameLogRequest(request.Request as GetBetHistoryRequest);
                else if (request.Request is GetGameURLRequest)
                    await onUserGetGameURL(request.Request as GetGameURLRequest);
                else if (request.Request is GetUserCashFlowRequest)
                    await onUserGetCashFlow(request.Request as GetUserCashFlowRequest);
                else if (request.Request is GetAllUserBalanceRequest)
                    await onGetAllUserBalance();
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
                Regex regex = new Regex("^[a-zA-Z0-9]{4,25}$");
                Match match = regex.Match(request.userid);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.UserIDMismatchRule, "user id doesn't match condition")));
                    return;
                }
                bool bSuccess = false;
                try
                {
                    bSuccess = await _dbReader.Ask<bool>(new DBCreateUserRequest(request.userid, Guid.NewGuid().ToString(), _dbID, _strAgentID, _currency));

                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in WebsiteActor::onUserCreate {0}", ex);
                }
                if (!bSuccess)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.DuplicateUserID, "duplicated user id")));
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
        private void onGetGameList(GetGameListRequest request)
        {
            try
            {
                List<GameInfo> allGameInfos     = DBMonitorSnapshot.Instance.getAllGameInfo();
                List<GameInfo> agentGameInfos   = new List<GameInfo>();
                foreach(GameInfo info in allGameInfos)
                {
                    if (!DBMonitorSnapshot.Instance.isAgentGameClosed(_dbID, info.GameID))
                        agentGameInfos.Add(info);
                }

                List<ApiGame>  apiGames  = new List<ApiGame>();
                foreach(GameInfo gameInfo in agentGameInfos)
                {
                    if (request.provider != 0 && request.provider != gameInfo.GameProvider)
                        continue;
                    
                    ApiGame apiGame  = new ApiGame();
                    apiGame.name     = gameInfo.Name;
                    if(gameInfo.GameProvider == 1)
                        apiGame.vendorid = "PragmaticPlay";
                    else if (gameInfo.GameProvider == 2)
                        apiGame.vendorid = "Booongo";
                    else if (gameInfo.GameProvider == 3)
                        apiGame.vendorid = "CQ9";
                    else if (gameInfo.GameProvider == 4)
                        apiGame.vendorid = "Habanero";
                    else if (gameInfo.GameProvider == 5)
                        apiGame.vendorid = "Playson";

                    apiGame.provider = gameInfo.GameProvider;
                    apiGame.gameid   = gameInfo.GameID;
                    apiGame.symbol   = gameInfo.Symbol;
                    apiGame.iconurl1 = string.Format("{0}/assets/images/ImageIcons/{1}/{2}.png", ApiConfig.UserFrontUrl, apiGame.vendorid ,gameInfo.Symbol);
                    apiGame.iconurl2 = "";

                    apiGames.Add(apiGame);
                }
                Sender.Tell(JsonConvert.SerializeObject(new GetGameListResponse(apiGames)));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onGetGameList {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private void onGetProviderList(GetVenderRequest request)
        {
            List<ApiProvider> providers = new List<ApiProvider>()
            {
                new ApiProvider(){ provider = 1, vendorid = "PragmaticPlay" },
                new ApiProvider(){ provider = 2, vendorid = "Booongo"       },
                new ApiProvider(){ provider = 3, vendorid = "CQ9"           },
                new ApiProvider(){ provider = 4, vendorid = "Habanero"      },
                new ApiProvider(){ provider = 5, vendorid = "Playson"       },
            };
            Sender.Tell(JsonConvert.SerializeObject(new GetVendorResponse(providers)));
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
        private async Task onUserDeposit(UserDepositRequest request)
        {
            try
            {
                if (_balance.LT(request.amount, _epsilion))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.NotEnoughAgentBalance, "agent balance not enough")));
                    return;
                }

                if (request.amount <= 0.0 || double.IsNaN(request.amount) || double.IsInfinity(request.amount))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidAmount, "Invalid Amount")));
                    return;
                }

                DBUserDepositResponse userDepositResponse = await _dbReader.Ask<DBUserDepositResponse>(new DBUserDepositRequest(_dbID, request.userid, request.amount), TimeSpan.FromSeconds(5.0));
                if (!userDepositResponse.IsSuccess)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid userid")));
                }
                else
                {
                    _balance -= request.amount;
                    _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, -request.amount));
                    _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, request.userid, -request.amount, _balance + request.amount, _balance, AgentMoneyChangeModes.USERDEPOSIT, DateTime.UtcNow));
                    _dbWriter.Tell(new UserMoneyChangeItem(_strAgentID, request.userid, request.amount, UserMoneyChangeModes.DEPOSIT,
                        userDepositResponse.Before, userDepositResponse.After, DateTime.UtcNow));

                    Context.System.ActorSelection("/user/commServers").Tell(new ApiDepositMessage(_dbID, request.userid,
                        request.amount, userDepositResponse.LastScoreCounter));

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
        private async Task onUserWithdraw(UserWithdrawRequest request)
        {
            try
            {
                if ((request.amount <= 0.0 && request.amount != -1) || double.IsNaN(request.amount) || double.IsInfinity(request.amount))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidAmount, "Invalid Amount")));
                    return;
                }

                ApiWithdrawResponse response = await Context.System.ActorSelection("/user/commAuthWorkers").Ask<ApiWithdrawResponse>(
                    new ApiWithdrawRequest(_dbID, request.userid, request.amount), TimeSpan.FromSeconds(60.0));

                if (response.Result != 0)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                    return;
                }

                double amount = response.BeforeScore - response.AfterScore;
                _balance += amount;
                if (amount > 0.0)
                {
                    //에이전트머니변경이력을 디비에 저장한다.
                    _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, amount));
                    _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, request.userid, amount, _balance - amount, _balance, AgentMoneyChangeModes.USERWITHDRAW, DateTime.UtcNow));
                    _dbWriter.Tell(new UserMoneyChangeItem(_strAgentID, request.userid, -amount, UserMoneyChangeModes.WITHDRAW, response.BeforeScore, response.AfterScore, DateTime.UtcNow));
                }

                Sender.Tell(JsonConvert.SerializeObject(new UserWithdrawResponse(Math.Round(amount, 2), Math.Round(response.AfterScore, 2))));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserWithdraw {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
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
                
                GameInfo gameInfo = DBMonitorSnapshot.Instance.getGameInfo(request.gameid);
                if (gameInfo == null)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidGameID, "invalid game id")));
                    return;
                }

                if (DBMonitorSnapshot.Instance.isAgentGameClosed(_dbID, request.gameid))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidGameID, "invalid game id")));
                    return;
                }

                string token         = string.Format("{0}_{1}", response.Password, DateTime.Now.AddMinutes(3.0).ToString("yyyy-MM-dd HH:mm:ss"));

                List<string> langs = new List<string>() { "en", "it", "de", "fr", "ko", "ja", "id", "ms", "fi", "th", "tr", "vi", "zh", "bg", "el", "es", "pl", "pt", "ro", "ru" };
                if (!langs.Contains(request.lang))
                    request.lang = "en";

                string strGameURL = "";
                if (gameInfo.GameProvider == 1)
                    strGameURL = string.Format("{0}/pp/openGame?&agentid={1}&userid={2}&token={3}&symbol={4}&lang={5}",
                        ApiConfig.GameFrontUrl, _dbID, response.UserName, encryptString(token, ApiConfig.FrontTokenKey), gameInfo.Symbol,request.lang);
                else if(gameInfo.GameProvider == 2)
                    strGameURL = string.Format("{0}/bng/openGame?&agentid={1}&userid={2}&token={3}&symbol={4}&lang={5}",
                        ApiConfig.GameFrontUrl, _dbID, response.UserName, encryptString(token, ApiConfig.FrontTokenKey), gameInfo.Symbol, request.lang);
                else if (gameInfo.GameProvider == 3)
                {
                    string lang = request.lang;
                    if (request.lang == "zh")
                        lang = "zh-cn";
                    else if (request.lang == "vi")
                        lang = "vn";
                    else if (request.lang == "pt")
                        lang = "pt-br";

                    strGameURL = string.Format("{0}/cq9/openGame?&agentid={1}&userid={2}&token={3}&symbol={4}&lang={5}",
                        ApiConfig.GameFrontUrl, _dbID, response.UserName, encryptString(token, ApiConfig.FrontTokenKey), gameInfo.Symbol, lang);
                }
                else if (gameInfo.GameProvider == 4)
                    strGameURL = string.Format("{0}/habanero/openGame?&agentid={1}&userid={2}&token={3}&symbol={4}&lang={5}",
                        ApiConfig.GameFrontUrl, _dbID, response.UserName, encryptString(token, ApiConfig.FrontTokenKey), gameInfo.Symbol, request.lang);
                else if (gameInfo.GameProvider == 5)
                    strGameURL = string.Format("{0}/playson/openGame?&agentid={1}&userid={2}&token={3}&symbol={4}&lang={5}",
                        ApiConfig.GameFrontUrl, _dbID, response.UserName, encryptString(token, ApiConfig.FrontTokenKey), gameInfo.Symbol, request.lang);
                else if (gameInfo.GameProvider == 6)
                    strGameURL = string.Format("{0}/amatic/openGame?&agentid={1}&userid={2}&token={3}&symbol={4}&lang={5}",
                        ApiConfig.GameFrontUrl, _dbID, response.UserName, encryptString(token, ApiConfig.FrontTokenKey), gameInfo.Symbol, request.lang);
                else
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
                    return;
                }

                Sender.Tell(JsonConvert.SerializeObject(new GetGameURLResponse(strGameURL)));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserGetGameURL {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
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

        private async Task onUserGetCashFlow(GetUserCashFlowRequest request)
        {
            
            try
            {
                CheckResult checkResult = _userCashFlowPolicy.Check(new SimpleThrottleKey(_strAgentID));
                if (checkResult.IsThrottled)
                {
                    _logger.Warning("{0}'s user cash flow fetch request count overflowed", _strAgentID);
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.RateLimited, "rate limited")));
                    return;
                }

                DateTime beginTime;
                if(!DateTime.TryParseExact(request.begintime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out beginTime))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidDateTimeFormat, "invalid datetime format")));
                    return;
                }
                DateTime endTime = new DateTime(1, 1, 1);
                if (!string.IsNullOrEmpty(request.endtime) && !DateTime.TryParseExact(request.endtime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidDateTimeFormat, "invalid datetime format")));
                    return;
                }
                if (request.maxcount <= 0)
                    request.maxcount = 500;
                else if (request.maxcount > 500)
                    request.maxcount = 500;

                string response = await _dbReader.Ask<string>(new DBUserCashFlowRequest(_strAgentID, request.userid, beginTime, endTime, request.maxcount), TimeSpan.FromSeconds(5));
                Sender.Tell(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in AgentActor::onUserGameLogRequest {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private async Task onGetAllUserBalance()
        {
            try
            {
                CheckResult checkResult = _allUserBalancePolicy.Check(new SimpleThrottleKey(_strAgentID));
                if (checkResult.IsThrottled)
                {
                    _logger.Warning("{0}'s user cash flow fetch request count overflowed", _strAgentID);
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.RateLimited, "rate limited")));
                    return;
                }

                string response = await _dbReader.Ask<string>(new DBGetAllUserBalanceRequest(_dbID), TimeSpan.FromSeconds(5));
                Sender.Tell(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in AgentActor::onGetAllUserBalance {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
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

                string response = await _dbReader.Ask<string>(new DBUserGameLogRequest(_dbID, request.lastobjectid, request.logcount), TimeSpan.FromSeconds(5));
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
                _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, "Admin", request.amount, _balance - request.amount, _balance, AgentMoneyChangeModes.UPDEPOSIT, DateTime.UtcNow));

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
                _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, "Admin", -request.amount, _balance + request.amount, _balance, AgentMoneyChangeModes.UPWITHDRAW, DateTime.UtcNow));

                Sender.Tell(JsonConvert.SerializeObject(new GetBalanceResponse(Math.Round(_balance, 2))));
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onAgentWithdrawRequest {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private void onSubtractEventMoney(SubtractEventMoneyRequest request)
        {
            try
            {
                if (!_isEnabled)
                {
                    Sender.Tell(false);
                    return;
                }
                if (_balance.LT(request.EventMoney, _epsilion))
                {
                    Sender.Tell(false);
                    return;
                }

                string strUserID = request.UserID;

                _balance -= request.EventMoney;
                _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, -request.EventMoney));
                _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, strUserID, -request.EventMoney, _balance + request.EventMoney, _balance, AgentMoneyChangeModes.EVENTSUBTRACT, DateTime.UtcNow));
                Sender.Tell(true);
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onSubtractEventMoney {0}", ex);
            }
        }
        private void onAddEventLeftMoney(AddEventLeftMoneyRequest request)
        {
            try
            {
                _balance += request.LeftMoney;
                string strUserID = request.UserID;
                int index = strUserID.IndexOf("_");
                if (index >= 0 && index < strUserID.Length - 1)
                    strUserID = strUserID.Substring(index + 1);

                _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, request.LeftMoney));
                _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, strUserID, request.LeftMoney, _balance - request.LeftMoney, _balance, AgentMoneyChangeModes.EVENTADDLEFT, DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onSubtractEventMoney {0}", ex);
            }
        }
    }
}
