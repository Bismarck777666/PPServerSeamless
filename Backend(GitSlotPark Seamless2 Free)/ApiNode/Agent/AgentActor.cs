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
using static Akka.Actor.Status;

namespace QueenApiNode.Agent
{
    class AgentActor : ReceiveActor
    {
        #region 에이전트정보
        private int             _dbID       = 0;
        private string          _strAgentID = "";
        private string          _strAuthKey = "";
        private bool            _isEnabled  = true;
        private Currencies      _currency   = Currencies.USD;
        private Languages       _language   = Languages.EN;
        private 
        #endregion

        protected static RealExtensions.Epsilon _epsilion   = new RealExtensions.Epsilon(0.01);
        private          IActorRef              _dbReader   = null;
        private          IActorRef              _dbWriter   = null;
        private readonly ILoggingAdapter        _logger     = Logging.GetLogger(Context);

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
            _strAuthKey = message.AuthToken;
            _isEnabled  = message.IsEnabled;

            if(message.Currency >= 0 && message.Currency < (int) Currencies.COUNT)
                _currency   = (Currencies) message.Currency;
            
            if(message.Language >= 0 && message.Language < (int) Languages.COUNT)
                _language = (Languages) message.Language;

            ReceiveAsync<ApiConsistentRequest>  (onAPIRequest);
            Receive<AgentEnabledUpdated>        (onAgentEnabledUpdated);

        }
        private void onAgentEnabledUpdated(AgentEnabledUpdated message)
        {
            this._isEnabled = message.IsEnabled;
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
                if (request.Request is GetGameListRequest)
                    onGetGameList(request.Request as GetGameListRequest);
                else if (request.Request is UserAuthRequest)
                    await onUserAuthRequest(request.Request as UserAuthRequest);
                else if(request.Request is CreateFreeGameRequest)
                    await onCreateFreeGameRequest(request.Request as CreateFreeGameRequest);
                else if(request.Request is CancelFreeGameRequest)
                    await onCancelFreeGameRequest(request.Request as CancelFreeGameRequest);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onAPIRequest {0}", ex);
            }
        }
        private void onGetGameList(GetGameListRequest request)
        {
            try
            {
                List<GameInfo> gameInfos = DBMonitorSnapshot.Instance.getAllGameInfo();
                List<ApiGame>  apiGames  = new List<ApiGame>();

                foreach(GameInfo gameInfo in gameInfos)
                {
                    if (!ChipsetManager.Instance.isAvailableGame(_currency, gameInfo.Symbol))
                        continue;

                    if (gameInfo.Symbol == "vs20kraken2" || gameInfo.Symbol == "vs10bbfmission" || gameInfo.Symbol == "vs10bhallbnza2" 
                        || gameInfo.Symbol == "vs10booklight" || gameInfo.Symbol == "vs25luckwildpb" || gameInfo.Symbol == "vswaysmfreya"
                        || gameInfo.Symbol == "vs10gbseries")
                        continue;
                    
                    string sc = "";

                    try
                    {
                        sc = ChipsetManager.Instance.getSCOfGame(_currency, gameInfo.Symbol);
                    }
                    catch 
                    {
                        _logger.Error("getChipsetInfo Error ---> {0}, {1}", gameInfo.Symbol, _currency.ToString());
                    }
                    int lineCnt = ChipsetManager.Instance.getLinesOfGame(_currency, gameInfo.Symbol);

                    int minLevel    = 0, maxLevel = 0;
                    double miniBet   = 0.0;
                    getMinMaxChipLevel(sc, lineCnt, ref minLevel, ref maxLevel, ref miniBet);
                    ApiGame apiGame  = new ApiGame();
                    apiGame.name     = gameInfo.Name;
                    apiGame.vendorid = "Pragmatic Play";
                    apiGame.gameid   = gameInfo.GameID;
                    apiGame.symbol   = gameInfo.Symbol;
                    if(apiGame.symbol == "vs25kingdomsnojp")
                    {
                        apiGame.iconurl1 = "https://blackstone-hk1.ppgames.net/gs2c/lobby/icons/vs25kingdoms/vs25kingdoms_wide.jpg";
                        apiGame.iconurl2 = "https://blackstone-hk1.ppgames.net/gs2c/lobby/icons/vs25kingdoms/vs25kingdoms_narrow.jpg";
                    }
                    else if(apiGame.symbol == "vs20sweetarg" || apiGame.symbol == "vs20hotsake" || apiGame.symbol == "vs20olgamesx" || apiGame.symbol == "vs20mmuertx" ||
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

                    apiGame.minlevel    = minLevel;
                    apiGame.maxlevel    = maxLevel;
                    apiGame.miniBet     = miniBet;
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
        private void getMinMaxChipLevel(string sc, int lineCnt, ref int minLevel, ref int maxLevel, ref double miniBet)
        {
            try
            {
                string[] chips  = sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                double minChip  = double.Parse(chips[0]);
                double maxChip  = double.Parse(chips[chips.Length - 1]);
                minLevel        = (int)(minChip / minChip);
                maxLevel        = (int)(maxChip / minChip);
                miniBet          = minChip * lineCnt;
            }
            catch { 
                minLevel    = 0; 
                maxLevel    = 0;
                miniBet     = 0.0;
            }
        }
        private async Task onCreateFreeGameRequest(CreateFreeGameRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.userID))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "user id doesn't match condition")));
                    return;
                }
                Regex regex = new Regex("^[a-zA-Z0-9]{4,48}$");
                Match match = regex.Match(request.userID);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "user id doesn't match condition")));
                    return;
                }

                if (string.IsNullOrEmpty(request.freespinID))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "You should give freespinID non empty value.")));
                    return;
                }
                regex = new Regex("^[-a-zA-Z0-9]{1,36}$");
                match = regex.Match(request.freespinID);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "freespinID doesn't match condition")));
                    return;
                }

                List<GameInfo> gameInfos = DBMonitorSnapshot.Instance.getAllGameInfo();
                int index = gameInfos.FindIndex(_ => _.GameID == request.gameID);
                if(index == -1 || !ChipsetManager.Instance.isAvailableGame(_currency, gameInfos[index].Symbol))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "undefined gameid")));
                    return;
                }

                string sc = ChipsetManager.Instance.getSCOfGame(_currency, gameInfos[index].Symbol);
                int lineCnt = ChipsetManager.Instance.getLinesOfGame(_currency, gameInfos[index].Symbol);

                int minLevel = 0, maxLevel = 0;
                double minBet = 0.0;
                getMinMaxChipLevel(sc, lineCnt, ref minLevel, ref maxLevel, ref minBet);

                if (request.betlevel < minLevel)
                {
                    string strMsg = string.Format("betlevel should be equal or bigger than {0}", minLevel);
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, strMsg)));
                    return;
                }

                if (request.betlevel > maxLevel)
                {
                    string strMsg = string.Format("betlevel should be equal or less than {0}", maxLevel);
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, strMsg)));
                    return;
                }

                if (request.freespincount < 0 || request.freespincount > 1000)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "freespincount should be range(1, 1000)")));
                    return;
                }
                if (request.expiretime < DateTime.UtcNow)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "Expire time should be later than the current time")));
                    return;
                }

                int response = await _dbReader.Ask<int>(new DBCreateFreeGameRequest(_dbID, request.userID, request.gameID, request.freespincount, request.betlevel, request.expiretime, request.freespinID), TimeSpan.FromSeconds(10.0));
                if(response == 0)
                    Sender.Tell(JsonConvert.SerializeObject(new CreateFreeGameResponse()));
                else if(response == 1)
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "freespinid alreay exist!")));
                else
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onCreateFreeGameRequest {0} {1}", ex, _strAgentID);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private async Task onCancelFreeGameRequest(CancelFreeGameRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.userID))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "user id doesn't match condition")));
                    return;
                }
                Regex regex = new Regex("^[a-zA-Z0-9]{4,48}$");
                Match match = regex.Match(request.userID);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "user id doesn't match condition")));
                    return;
                }

                if (string.IsNullOrEmpty(request.freespinID))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "You should give freespinID non empty value.")));
                    return;
                }
                regex = new Regex("^[-a-zA-Z0-9]{1,36}$");
                match = regex.Match(request.freespinID);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "freespinID doesn't match condition")));
                    return;
                }

                int response = await _dbReader.Ask<int>(new DBCancelFreeGameRequest(_dbID, request.userID, request.freespinID), TimeSpan.FromSeconds(10.0));
                if(response == 0)
                    Sender.Tell(JsonConvert.SerializeObject(new CancelFreeGameResponse()));
                else if(response == 1)
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "already processed!")));
                else if (response == 2)
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "already canceled!")));
                else if (response == 3)
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "feespinid not exist!")));
                else
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onCreateFreeGameRequest {0} {1}", ex, _strAgentID);
                Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
            }
        }
        private async Task onUserAuthRequest(UserAuthRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.userID))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "user id doesn't match condition")));
                    return;
                }
                Regex regex = new Regex("^[a-zA-Z0-9]{4,48}$");
                Match match = regex.Match(request.userID);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "user id doesn't match condition")));
                    return;
                }

                DBGetUserInfoResponse response = await _dbReader.Ask<DBGetUserInfoResponse>(new DBGetUserInfoRequest(_dbID, request.userID), TimeSpan.FromSeconds(10.0));
                string strUserID = null;
                string strPassword = null;
                if (response == null)
                {
                    strUserID = request.userID;
                    strPassword = getMD5Hash(Guid.NewGuid().ToString());
                    bool bSuccess = await _dbReader.Ask<bool>(new DBCreateUserRequest(strUserID, strPassword, _dbID, _strAgentID, _currency, _language, request.isaffiliate));
                    if (!bSuccess)
                    {
                        Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "user id doesn't match condition")));
                        return;
                    }
                }
                else
                {
                    strUserID = response.UserName;
                    strPassword = response.Password;
                }
                GameInfo gameInfo = DBMonitorSnapshot.Instance.getGameInfo(request.gameid);
                if (gameInfo == null)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "undefined game id")));
                    return;
                }

                if (gameInfo.Symbol == "vs20kraken2" || gameInfo.Symbol == "vs10bbfmission" || gameInfo.Symbol == "vs10bhallbnza2"
                    || gameInfo.Symbol == "vs10booklight" || gameInfo.Symbol == "vs25luckwildpb" || gameInfo.Symbol == "vswaysmfreya"
                    || gameInfo.Symbol == "vs10gbseries")
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "undefined game id")));
                    return;
                }

                if (!ChipsetManager.Instance.isAvailableGame(_currency, gameInfo.Symbol))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "undefined game id")));
                    return;
                }
                string token         = string.Format("{0}_{1}", strPassword, DateTime.UtcNow.AddMinutes(5.0).ToString("yyyy-MM-dd HH:mm:ss"));                
                string strGameURL    = string.Format("{0}/pp/openGame?&agentid={1}&userid={2}&token={3}&symbol={4}&lang={5}&lobbyurl={6}", 
                    ApiConfig.GameFrontUrl, _dbID, strUserID, encryptString(token, ApiConfig.FrontTokenKey), gameInfo.Symbol, request.lang, request.lobbyUrl);

                Sender.Tell(JsonConvert.SerializeObject(new UserAuthResponse(strGameURL)));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AgentActor::onUserGetGameURL {0} {1}", ex, _strAgentID);
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
        private string getMD5Hash(string input)
        {
            try
            {
                MD5 md5Hash = MD5.Create();
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
