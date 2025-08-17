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
using System.Net.Http;
using Akka;

namespace QueenApiNode.Agent
{
    class AgentActor : ReceiveActor
    {
        #region 에이전트정보
        private int             _dbID       = 0;
        private string          _strAgentID = "";
        private string          _strAuthKey = "";
        private bool            _isEnabled  = true;
        private int             _currency = 0;
        private 
        #endregion

        protected static RealExtensions.Epsilon _epsilion   = new RealExtensions.Epsilon(0.01);
        private          IActorRef              _dbReader   = null;
        private          IActorRef              _dbWriter   = null;
        private readonly ILoggingAdapter        _logger     = Logging.GetLogger(Context);
        private ThrottlePolicy _gameLogFetchPolicy = new ThrottlePolicy() { Name = "GameLogFetch", PerMinute = 11 };


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
            _currency = message.Currency;

            ReceiveAsync<ApiConsistentRequest>  (onAPIRequest);

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
                else if (request.Request is GetBetHistoryRequest)
                    await onUserGameLogRequest(request.Request as GetBetHistoryRequest);

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
                    ApiGame apiGame         = new ApiGame();
                    apiGame.title           = gameInfo.Title;
                    apiGame.thumbnail       = string.Format("https://aristoback.{0}.com/slots/cover-img/" + gameInfo.CoverImg, ApiConfig.Domain);
                    apiGame.thumbnail       = apiGame.thumbnail.Replace("\r", "").Replace("\n", "");
                    apiGame.name            = (string)gameInfo.Name;
                    apiGame.gameid          = gameInfo.GameID.ToString();
                    apiGame.weekturnover    = gameInfo.WeekTurnover;
                    apiGame.monthturnover   = gameInfo.MonthTurnover;
                    apiGame.releasedate     = gameInfo.ReleaseDate;
                    int gameType            = gameInfo.GameProvider;
                    if (gameType == 1)
                        apiGame.provider = "ARISTOCRAT";
                    else
                        apiGame.provider = "ARISTOCRAT";

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
        private async Task onUserAuthRequest(UserAuthRequest request)
        {
            try
            {
                Regex regex = new Regex("^[a-zA-Z0-9]{4,48}$");
                Match match = regex.Match(request.userID);
                if (!match.Success)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "The user ID does not match the criteria. The user ID must contain only numbers and alphabetic characters.")));
                    return;
                }

                DBGetUserInfoResponse response = await _dbReader.Ask<DBGetUserInfoResponse>(new DBGetUserInfoRequest(_dbID, request.userID), TimeSpan.FromSeconds(5.0));
                string strUserID   = null;
                string strPassword = null;
                if (response == null)
                {
                    strUserID       = request.userID;
                    strPassword     = getMD5Hash(Guid.NewGuid().ToString());
                    bool bSuccess   = await _dbReader.Ask<bool>(new DBCreateUserRequest(strUserID, strPassword, _dbID, _strAgentID));
                    if(!bSuccess)
                    {
                        Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "Duplicate user ID. Please use another user ID.")));
                        return;
                    }
                }
                else
                {
                    strUserID   = response.UserName;
                    strPassword = response.Password;
                }
                GameInfo gameInfo = DBMonitorSnapshot.Instance.getGameInfo(request.gameid);
                if (gameInfo == null)
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.WrongInputParameters, "undefined game id")));
                    return;
                }

                if (gameInfo.GameProvider == 3)
                {
                    string strGameSymbol = gameInfo.Symbol;
                    string token         = string.Format("{0}_{1}", strPassword, DateTime.Now.AddMinutes(3.0).ToString("yyyy-MM-dd HH:mm:ss"));

                    //string strGameURL = _currency != 7 ? string.Format("https://evoclient.{4}.com/pp/openGame?&userid={0}&token={1}&symbol={2}&lang={3}", strUserID, encryptString(token, "fd973747b689447fb360c89f9612baa8"), strGameSymbol,
                    //    string.IsNullOrEmpty(request.lang) ? "en" : request.lang, ApiConfig.Domain)
                    //    : string.Format("https://evoclient.{4}.net/pp/openGame?&userid={0}&token={1}&symbol={2}&lang={3}", strUserID, encryptString(token, "fd973747b689447fb360c89f9612baa8"), strGameSymbol,
                    //    string.IsNullOrEmpty(request.lang) ? "en" : request.lang, ApiConfig.Domain);
                    string strGameURL = "";
                    if (request.gameid == 1000)
                    {
                        strGameURL = string.Format("http://localhost:5155/{2}/index.html?device=desktop&nogslang={3}&nogsmode=real&nogscurrency=THB&lobbyurl=http%3A%2F%2Flocalhost%3A5155&sessionid={1}&nogsserver=http%3A%2F%2Flocalhost%3A5155%2F1000%2F&countrycode=ZH&locale=zh_CN&id_domain=sslgamesasia&bbm=0&userid={0}&gamesymbol={4}", response.UserName, Convert.ToBase64String(Encoding.UTF8.GetBytes(token)), request.gameid, string.IsNullOrEmpty(request.lang) ? "en" : request.lang, strGameSymbol);
                    }
                    else
                    {
                        strGameURL = string.Format("http://localhost:5155/{2}/index.html?session={1}&language={3}&s=7&userid={0}&gamesymbol={4}", response.UserName, Convert.ToBase64String(Encoding.UTF8.GetBytes(token)), request.gameid, string.IsNullOrEmpty(request.lang) ? "en" : request.lang, strGameSymbol);
                    }

                    Sender.Tell(JsonConvert.SerializeObject(new UserAuthResponse(strGameURL)));
                }
                else
                {
                    var httpClient                    = new HttpClient();
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer 3tgsH6SGQdDTJeRf7KYk6bwHkXaDKhrXFXRWdTpdW1hyJimoZgD6cJU25YxkOyGJ");
                    APUserAuthRequest userAuthRequest = new APUserAuthRequest();
                    userAuthRequest.userID  = request.userID;
                    userAuthRequest.agentID = _strAgentID;
                    userAuthRequest.gameID  = request.gameid;
                    userAuthRequest.lang    = string.IsNullOrEmpty(request.lang) ? "en" : request.lang;
                    string strPostContent   = JsonConvert.SerializeObject(userAuthRequest);
                    var httpResponse = await httpClient.PostAsync(string.Format("https://advent.{0}.com/userAuth", ApiConfig.Domain), new StringContent(strPostContent, Encoding.UTF8, "application/json"));
                    httpResponse.EnsureSuccessStatusCode();
                    string strContent = await httpResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<APUserAuthResponse>(strContent);
                    if (!string.IsNullOrEmpty(apiResponse.url))
                        Sender.Tell(JsonConvert.SerializeObject(new UserAuthResponse(apiResponse.url)));
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
}
