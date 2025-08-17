using Akka.Actor;
using Akka.Event;
using BrakePedal;
using GITProtocol;
using GITProtocol.Utils;
using Newtonsoft.Json;
using QueenApiNode.Database;
using QueenApiNode.HttpService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace QueenApiNode.Agent
{
    internal class AgentActor : ReceiveActor
    {
        #region 에이전트정보
        private int         _dbID       = 0;
        private string      _strAgentID = "";
        private double      _balance    = 0.0;
        private string      _strAuthKey = "";
        private bool        _isEnabled  = true;
        private Currencies  _currency   = Currencies.USD;
        private Languages   _language   = Languages.EN;
        #endregion

        private protected static RealExtensions.Epsilon _epsilion   = new RealExtensions.Epsilon(0.01);
        private IActorRef                               _dbReader   = null;
        private IActorRef                               _dbWriter   = null;
        private readonly ILoggingAdapter                _logger     = Context.GetLogger();

        private ThrottlePolicy _gameLogFetchPolicy      = new ThrottlePolicy(){ Name = "GameLogFetch",  PerMinute = 11 };
        private ThrottlePolicy _userCashFlowPolicy      = new ThrottlePolicy(){ Name = "CashFlowFetch", PerMinute = 30 };
        private ThrottlePolicy _allUserBalancePolicy    = new ThrottlePolicy(){ Name = "CashFlowFetch", PerMinute = 10 };

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
            _balance    = message.Balance;
            _strAuthKey = message.AuthToken;

            if (message.Currency >= (int)Currencies.USD && message.Currency < (int)Currencies.COUNT)
                _currency = (Currencies) message.Currency;

            if (message.Language >= (int)Languages.EN && message.Language < (int)Languages.COUNT)
                _language = (Languages) message.Language;

            ReceiveAsync<ApiConsistentRequest>(onAPIRequest);
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
                Regex regex = new Regex("^[a-zA-Z0-9]{4,16}$");
                Match match = regex.Match(request.userid);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.UserIDMismatchRule, "user id doesn't match condition")));
                    return;
                }
                
                bool bSuccess = false;
                try
                {
                    bSuccess = await _dbReader.Ask<bool>(new DBCreateUserRequest(request.userid, Guid.NewGuid().ToString(), _dbID, _strAgentID, _currency, _language));
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
                List<GameInfo>  allGameInfo = DBMonitorSnapshot.Instance.getAllGameInfo();
                List<ApiGame>   data        = new List<ApiGame>();
                foreach (GameInfo gameInfo in allGameInfo)
                {
                    if (gameInfo.Symbol == "vs20kraken2" || gameInfo.Symbol == "vs10bbfmission" || gameInfo.Symbol == "vs10bhallbnza2"
                        || gameInfo.Symbol == "vs10booklight" || gameInfo.Symbol == "vs25luckwildpb" || gameInfo.Symbol == "vswaysmfreya"
                        || gameInfo.Symbol == "vs10gbseries")
                        continue;

                    if (ChipsetManager.Instance.isAvailableGame(_currency, gameInfo.Symbol))
                    {
                        ApiGame apiGame = new ApiGame();
                        apiGame.name        = gameInfo.Name;
                        apiGame.vendorid    = "Pragmatic Play";
                        apiGame.gameid      = gameInfo.GameID;
                        apiGame.symbol      = gameInfo.Symbol;
                        if (apiGame.symbol == "vs25kingdomsnojp")
                        {
                            apiGame.iconurl1 = "https://blackstone-hk1.ppgames.net/gs2c/lobby/icons/vs25kingdoms/vs25kingdoms_wide.jpg";
                            apiGame.iconurl2 = "https://blackstone-hk1.ppgames.net/gs2c/lobby/icons/vs25kingdoms/vs25kingdoms_narrow.jpg";
                        }
                        else if (apiGame.symbol == "vs20sweetarg" || apiGame.symbol == "vs20hotsake" || apiGame.symbol == "vs20olgamesx" || apiGame.symbol == "vs20mmuertx" ||
                            apiGame.symbol == "vs10cndstbnnz" || apiGame.symbol == "vs20hadex" || apiGame.symbol == "vs20rizkbnz")
                        {
                            apiGame.iconurl1 = string.Format("https://demo.bismarckslot.com/assets/images/ImageIcons/PragmaticPlay/{0}_wide.jpg", gameInfo.Symbol);
                            apiGame.iconurl2 = string.Format("https://demo.bismarckslot.com/assets/images/ImageIcons/PragmaticPlay/{0}_narrow.jpg", gameInfo.Symbol);
                        }
                        else
                        {
                            apiGame.iconurl1 = string.Format("https://blackstone-hk1.ppgames.net/gs2c/lobby/icons/{0}/{0}_wide.jpg", gameInfo.Symbol);
                            apiGame.iconurl2 = string.Format("https://blackstone-hk1.ppgames.net/gs2c/lobby/icons/{0}/{0}_narrow.jpg", gameInfo.Symbol);
                        }
                        data.Add(apiGame);
                    }
                }
                Sender.Tell(JsonConvert.SerializeObject(new GetGameListResponse(data)));
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

        private async Task onUserDeposit(UserDepositRequest request)
        {
            try
            {
                double depositAmount = Math.Round(request.amount, 2);
                if (_balance.LT(depositAmount, _epsilion))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.NotEnoughAgentBalance, "agent balance not enough")));
                    return;
                }

                if (depositAmount <= 0.0 || double.IsNaN(depositAmount) || double.IsInfinity(depositAmount))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidAmount, "Invalid Amount")));
                    return;
                }

                DBUserDepositResponse userDepositResponse = await _dbReader.Ask<DBUserDepositResponse>(new DBUserDepositRequest(_dbID, request.userid, depositAmount), TimeSpan.FromSeconds(5.0));
                if (!userDepositResponse.IsSuccess)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid userid")));
                }
                else
                {
                    _balance -= depositAmount;
                    _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, -depositAmount));
                    _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, request.userid, depositAmount, _balance + depositAmount, _balance, AgentMoneyChangeModes.USERDEPOSIT, DateTime.Now));
                    _dbWriter.Tell(new UserMoneyChangeItem(_strAgentID, request.userid, depositAmount, UserMoneyChangeModes.DEPOSIT, userDepositResponse.Before, userDepositResponse.After, DateTime.Now));
                    
                    Context.System.ActorSelection("/user/userServers").Tell(new ApiUserDepositMessage(_dbID, request.userid, 
                        depositAmount, userDepositResponse.LastScoreCounter));
                    
                    Sender.Tell(JsonConvert.SerializeObject(new GetBalanceResponse(Math.Round(userDepositResponse.After, 2))));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in WebsiteActor::onUserDeposit {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }

        private async Task onUserWithdraw(UserWithdrawRequest request)
        {
            string strUserID        = request.userid;
            double withdrawAmount   = Math.Round(request.amount, 2);
            try
            {
                DBGetUserInfoResponse infoResponse = await _dbReader.Ask<DBGetUserInfoResponse>(new DBGetUserInfoRequest(_dbID, request.userid), TimeSpan.FromSeconds(5.0));
                if (infoResponse == null)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserWithdraw {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
                return;
            }

            try
            {
                bool quitResult = await Context.System.ActorSelection("/user/frontAuthWorkers").Ask<bool>(new ApiUserBeginWithdraw(_dbID, request.userid), TimeSpan.FromSeconds(10.0));
                if (!quitResult)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                }
                else
                {
                    ApiWithdrawResponse response = await _dbReader.Ask<ApiWithdrawResponse>(new ApiWithdrawRequest(_dbID, strUserID, withdrawAmount), TimeSpan.FromSeconds(31.0));
                    if (response.Result != 0)
                    {
                        Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidUserID, "invalid user id")));
                    }
                    else
                    {
                        double amount = Math.Round(response.BeforeScore - response.AfterScore, 2);
                        _balance = Math.Round(_balance + amount, 2);
                        if (amount > 0.0)
                        {
                            _dbWriter.Tell(new AgentScoreUpdateItem(_dbID, amount));
                            _dbWriter.Tell(new AgentMoneyChangeItem(_strAgentID, request.userid, amount, Math.Round(_balance - amount, 2), _balance, AgentMoneyChangeModes.USERWITHDRAW, DateTime.Now));
                            _dbWriter.Tell(new UserMoneyChangeItem(_strAgentID, request.userid, amount, UserMoneyChangeModes.WITHDRAW, response.BeforeScore, response.AfterScore, DateTime.Now));
                        }
                        Sender.Tell(JsonConvert.SerializeObject(new UserWithdrawResponse(Math.Round(amount, 2), Math.Round(response.AfterScore, 2))));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserWithdraw {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
            finally
            {
                Context.System.ActorSelection("/user/frontAuthWorkers").Tell(new ApiUserEndWithdraw(_dbID, strUserID));
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

                if (!ChipsetManager.Instance.isAvailableGame(_currency, gameInfo.Symbol))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidGameID, "invalid game id")));
                    return;
                }


                if (gameInfo.Symbol == "vs20kraken2" || gameInfo.Symbol == "vs10bbfmission" || gameInfo.Symbol == "vs10bhallbnza2"
                    || gameInfo.Symbol == "vs10booklight" || gameInfo.Symbol == "vs25luckwildpb" || gameInfo.Symbol == "vswaysmfreya"
                    || gameInfo.Symbol == "vs10gbseries")
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidGameID, "invalid game id")));
                    return;
                }

                string token        = string.Format("{0}_{1}", response.Password, DateTime.UtcNow.AddMinutes(5.0).ToString("yyyy-MM-dd HH:mm:ss"));
                string strGameURL   = string.Format("{0}/pp/openGame?&agentid={1}&userid={2}&token={3}&symbol={4}&lang={5}", ApiConfig.GameFrontUrl, _dbID, response.UserName, encryptString(token, ApiConfig.FrontTokenKey), gameInfo.Symbol, request.lang);
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
                aes.IV  = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return HttpServerUtility.UrlTokenEncode(array);
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
                if (!DateTime.TryParseExact(request.begintime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out beginTime))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidDateTimeFormat, "invalid datetime format")));
                    return;
                }
                
                DateTime endTime = new DateTime(1, 1, 1);
                if (!string.IsNullOrEmpty(request.endtime) && !DateTime.TryParseExact(request.begintime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out beginTime))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidDateTimeFormat, "invalid datetime format")));
                    return;
                }
                
                if (request.maxcount <= 0)
                    request.maxcount = 500;
                else if (request.maxcount > 500)
                    request.maxcount = 500;

                string response = await _dbReader.Ask<string>(new DBUserCashFlowRequest(_strAgentID, request.userid, beginTime, endTime, request.maxcount), TimeSpan.FromSeconds(5.0));
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

                string response = await _dbReader.Ask<string>(new DBGetAllUserBalanceRequest(_dbID), TimeSpan.FromSeconds(5.0));
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
                    
                string response = await _dbReader.Ask<string>(new DBUserGameLogRequest(_dbID, request.lastobjectid, request.logcount), TimeSpan.FromSeconds(5.0));
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
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onAgentWithdrawRequest {0}", ex);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
    }
}
