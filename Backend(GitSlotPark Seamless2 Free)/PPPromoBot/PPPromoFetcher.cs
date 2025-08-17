using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Net.Http;
using Akka.Configuration;
using Akka.Event;
using Newtonsoft.Json;
using PPPromoBot.Database;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Runtime.InteropServices;
using System.Globalization;

namespace PPPromoBot
{
    public class PPPromoFetcher : ReceiveActor
    {
        private HttpClient                  _httpClient     = null;
        private string                      _strPPToken     = null;
        private int                         _gameID         = 0;
        private string                      _apiGameSymbol  = null;
        private string                      _gameSymbol     = null;
        private string                      _gameUrl        = null;
        private string                      _hostUrl        = null;
        private int                         _dbCount        = 0;
        private readonly ILoggingAdapter    _logger         = Context.GetLogger();

        public PPPromoFetcher(Config siteInfo,int dbCount)
        {
            _dbCount    = dbCount;

            if (siteInfo != null)
            {
                _gameSymbol = siteInfo.GetString("symbol");
                _gameUrl    = siteInfo.GetString("url");
                _hostUrl    = siteInfo.GetString("host");
            }

            ReceiveAsync<string>    (processCommand);
        }

        private async Task fetchMiniLobby()
        {
            try
            {
                _logger.Info("Getting MiniLobby...");
                
                string strHost  = "blackstone-hk1.ppgames.net";
                string strURL   = string.Format("https://{0}/gs2c/minilobby/games?mgckey={1}", strHost, _strPPToken);
                
                HttpResponseMessage response = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();
                string strContent       = await response.Content.ReadAsStringAsync();
                JToken miniLobbyGames   = JToken.Parse(strContent);
                miniLobbyGames["gameLaunchURL"] = "/pp/reOpenGame";
                for (int i = 0; i < _dbCount; ++i)
                {
                    await RedisDatabase.RedisCache(i).StringSetAsync("PPMiniLobby", miniLobbyGames.ToString());
                }
                
                _logger.Info("Saved MiniLobby Information");
            }
            catch
            {
            }
        }

        private async Task<bool> fetchPromotions()
        {
            try
            {
                _logger.Info("Getting PP Active Promotions...");

                //사이트에서 직접 호출
                string strHost  = _hostUrl;
                string strURL   = string.Format("https://{0}/gs2c/promo/active/?symbol={2}&mgckey={1}", strHost, _strPPToken, _gameSymbol);

                HttpResponseMessage response = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();
                string strContent = await response.Content.ReadAsStringAsync();
                PPActivePromos activePromos = JsonConvert.DeserializeObject<PPActivePromos>(strContent);
                for (int i = 0; i < _dbCount; ++i)
                {
                    await RedisDatabase.RedisCache(i).StringSetAsync("ActivePromos", strContent);
                }

                strURL      = string.Format("https://{0}/gs2c/promo/tournament/details/?symbol={2}&mgckey={1}", strHost, _strPPToken, _gameSymbol);
                response    = await _httpClient.GetAsync(strURL);
                
                response.EnsureSuccessStatusCode();
                strContent = await response.Content.ReadAsStringAsync();
                for (int i = 0; i < _dbCount; ++i)
                {
                    await RedisDatabase.RedisCache(i).StringSetAsync("TournamentDetails", strContent);
                }

                strURL      = string.Format("https://{0}/gs2c/promo/race/details/?symbol={2}&mgckey={1}", strHost, _strPPToken, _gameSymbol);
                response    = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();
                strContent = await response.Content.ReadAsStringAsync();
                for (int i = 0; i < _dbCount; ++i)
                {
                    await RedisDatabase.RedisCache(i).StringSetAsync("RaceDetails", strContent);
                }

                strURL      = string.Format("https://{0}/gs2c/promo/race/prizes/?symbol={2}&mgckey={1}", strHost, _strPPToken, _gameSymbol);
                response    = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();
                strContent = await response.Content.ReadAsStringAsync();
                for (int i = 0; i < _dbCount; ++i)
                {
                    await RedisDatabase.RedisCache(i).StringSetAsync("RacePrizes", strContent);
                }

                List<int> tourmentIds = new List<int>();
                for (int i = 0; i < activePromos.tournaments.Count; ++i)
                {
                    if (!(activePromos.tournaments[i].status == "S"))
                        tourmentIds.Add(activePromos.tournaments[i].id);
                }

                strURL      = string.Format("https://{0}/gs2c/promo/tournament/v3/leaderboard/?symbol={2}&mgckey={1}&tournamentIDs={3}", strHost, _strPPToken, _gameSymbol, string.Join(",", tourmentIds.ToArray()));
                response    = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();
                strContent = await response.Content.ReadAsStringAsync();
                for (int i = 0; i < _dbCount; ++i)
                {
                    await RedisDatabase.RedisCache(i).StringSetAsync("TournamentLeaderboard", strContent, TimeSpan.FromDays(15.0));
                }

                RaceWinnersRequest request = new RaceWinnersRequest();
                request.raceIds = new List<int>();
                for (int i = 0; i < activePromos.races.Count; ++i)
                {
                    request.raceIds.Add(activePromos.races[i].id);
                }

                string strPayload = JsonConvert.SerializeObject(request);
                strURL      = string.Format("https://{0}/gs2c/promo/race/v3/winners/?symbol={2}&mgckey={1}", strHost, _strPPToken, _gameSymbol);
                response    = await _httpClient.PostAsync(strURL, new StringContent(strPayload, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                
                strContent = await response.Content.ReadAsStringAsync();
                for (int i = 0; i < _dbCount; ++i)
                {
                    await RedisDatabase.RedisCache(i).StringSetAsync("RaceWinners", strContent);
                }

                strURL      = string.Format("https://{0}/gs2c/promo/frb/available/?mgckey={1}", strHost, _strPPToken);
                response    = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();

                strContent = await response.Content.ReadAsStringAsync();
                for (int i = 0; i < _dbCount; ++i)
                {
                    await RedisDatabase.RedisCache(i).StringSetAsync("FrbAvailable", strContent);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PPPromoFetcher::fetchPromotions {0}", ex);
                return false;
            }
        }

        public static DateTime fromUnixTimestamp(int timestamp)
        {
            return new DateTime(1970, 1, 1).AddSeconds(timestamp);
        }

        private async Task processCommand(string strCommand)
        {
            if (strCommand == "tick")
            {
                try
                {
                    _logger.Info("open slot game with api..");
                    
                    string strURL   = await openSlotGameSpinWiz(_gameUrl);
                    //string strURL          = _gameUrl;
                    if (strURL == null)
                    {
                        _logger.Info("open slot game failed");
                    }
                    else
                    {
                        _logger.Info("Getting PP Token...");
                        bool ppTokenFromApiurl = await findPPTokenFromAPIURL(strURL);
                        if (ppTokenFromApiurl)
                        {
                            await fetchPromotions();
                            //await fetchMiniLobby();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occurred in PPPromoFetcher::processCommand {0}", (object)ex);
                }
                finally
                {
                    _httpClient = null;
                    _strPPToken = null;
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(120.0), Self, "tick", Self);
                }
            }
        }

        public async Task<string> getGameLaunchURL(string strUserCode,string strToken,string strGameID,string strReturnURL)
        {
            try
            {
                string strRequestJson   = JsonConvert.SerializeObject(new GetGameURLRequest()
                {
                    mode        = "real",
                    usercode    = strUserCode,
                    token       = strToken,
                    game        = strGameID,
                    lang        = "ko",
                    return_url  = " "
                });
                string strAuthKey       = createMD5("96016048287144200" + strRequestJson);
                string strURL           = string.Format("{0}/getgameurl", "https://prd-sdv2-api.slotsdiamond.com/apiprod-slotsdiamond");
                
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + strAuthKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", "1290");
                HttpResponseMessage response = await httpClient.PostAsync(strURL, new StringContent(strRequestJson, Encoding.UTF8, "application/json"));
                string strResponse = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                GetGameURLResponse resp = JsonConvert.DeserializeObject<GetGameURLResponse>(strResponse);

                if (resp == null || resp.status != "0")
                    return null;

                return resp.data.return_url;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> getMegafonLaunchURL()
        {
            try
            {
                _httpClient = new HttpClient();

                string lunchPayLoad = "{\"brandId\":\"pragmatic_PM\",\"gameId\":\"6565156d8714414bb231b01a\",\"image\":\"https://test1.prerelease-env.biz/game_pic/rec/325/vs25goldparty.png\",\"integrationProvider\":\"PM\",\"name\":\"Gold Party\",\"providerId\":\"pragmatic\",\"status\":true,\"tags\":[\"Treasures\",\"Adventure\"],\"token\":\"IPL28hs2JVzreim86194-jugar\",\"playerId\":\"277951_86194\",\"userId\":\"655d4ae63ad36797f65fabad\",\"currency\":\"ARS\",\"language\":\"es\"}";
                HttpResponseMessage response = await _httpClient.PostAsync("https://casino2.jcasino.live/v1/casino/launch_game", new StringContent(lunchPayLoad, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                string strContent = await response.Content.ReadAsStringAsync();
                dynamic lunchRes = JsonConvert.DeserializeObject<dynamic>(strContent);

                if ((string)lunchRes["result"] == "success")
                {
                    string strURL = lunchRes["message"];
                    return strURL;
                }
                return string.Empty;
            }
            catch 
            {
                return string.Empty;    
            }
        }

        private string createMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes    = Encoding.ASCII.GetBytes(input);
                byte[] hash     = md5.ComputeHash(bytes);

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; ++i)
                    stringBuilder.Append(hash[i].ToString("x2"));

                return stringBuilder.ToString();
            }
        }

        public async Task<ApiUserLoginResult> loginAccount(string strUserID, string strPassword)
        {
            try
            {
                string strRequestJson = JsonConvert.SerializeObject(new CreateAccountRequest()
                {
                    username = strUserID,
                    password = strPassword
                });

                string strAuthKey   = createMD5("96016048287144200" + strRequestJson);
                string strURL       = string.Format("{0}/createaccount", "https://prd-sdv2-api.slotsdiamond.com/apiprod-slotsdiamond");
                
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + strAuthKey);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("client_id", "1290");
                HttpResponseMessage response = await httpClient.PostAsync(strURL, new StringContent(strRequestJson, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string strResponse = await response.Content.ReadAsStringAsync();
                CreateAccountResponse resp = JsonConvert.DeserializeObject<CreateAccountResponse>(strResponse);

                if (resp == null || resp.status != "0")
                {
                    _logger.Info("login failed");
                    return null;
                }
                
                return new ApiUserLoginResult(resp.data.usercode, resp.data.token);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> openSlotGame()
        {
            try
            {
                ApiUserLoginResult loginResult = await loginAccount("promouser001", "Abc123abc123");
                if (loginResult == null)
                    return null;

                //string strURL = await getGameLaunchURL(loginResult.UserCode, loginResult.Token, _gameID.ToString(), "http://wrgame.uk/");
                string strURL = await getMegafonLaunchURL();
                return strURL;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PPPromoFetcher::openSlotGame {0}", ex);
                return null;
            }
        }

        public async Task<string> openSlotGameSpinWiz(string strURL)
        {
            try
            {
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
                HttpResponseMessage response = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();
                string strContent   = await response.Content.ReadAsStringAsync();
                string strSearchKey = "<iframe id=\"vision-game-wrapper\" src=\"";
                if (!strContent.Contains(strSearchKey))
                    return null;

                int iframeStartIndex    = strContent.IndexOf(strSearchKey) + strSearchKey.Length;
                int iframeEndIndex      = strContent.IndexOf("\"", iframeStartIndex);

                strURL = strContent.Substring(iframeStartIndex, iframeEndIndex - iframeStartIndex);

                //response = await _httpClient.GetAsync(strURL);
                //response.EnsureSuccessStatusCode();
                //strContent      = await response.Content.ReadAsStringAsync();
                //strSearchKey    = "<iframe src=\"";
                //if (!strContent.Contains(strSearchKey))
                //    return null;

                //iframeStartIndex        = strContent.IndexOf(strSearchKey) + strSearchKey.Length;
                //iframeEndIndex          = strContent.IndexOf("\"", iframeStartIndex);

                //strURL = strContent.Substring(iframeStartIndex, iframeEndIndex - iframeStartIndex);
                return strURL;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PPPromoFetcher::openSlotGameMoment {0}", ex);
                return null;
            }
        }

        private async Task<bool> findPPTokenFromAPIURL(string strURL)
        {
            try
            {
                _httpClient = new HttpClient();
                //strURL      = strURL.Substring(0, strURL.Length - 1) + "local";

                //_httpClient.DefaultRequestHeaders.Referrer = new Uri("https://casino-skin1.jcasino.live/");
                HttpResponseMessage response = await _httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();

                strURL      = response.RequestMessage.RequestUri.ToString();
                _strPPToken = findStringBetween(strURL, "&mgckey=", "&");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PPPromoFetcher::findPPTokenFromAPIURL {0}", ex);
                return false;
            }
        }

        private string findStringBetween(string strSource,string strStartPattern,string strEndPattern)
        {
            try
            {
                int startIndex  = strSource.IndexOf(strStartPattern) + strStartPattern.Length;
                int endIndex    = strSource.IndexOf(strEndPattern, startIndex);
                return strSource.Substring(startIndex, endIndex - startIndex);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class RaceWinnersRequest
    {
        public List<int> raceIds { get; set; }
    }

    public class GetGameURLRequest
    {
        public string   mode        { get; set; }
        public string   usercode    { get; set; }
        public string   game        { get; set; }
        public string   lang        { get; set; }
        public string   return_url  { get; set; }
        public string   token       { get; set; }
    }

    public class GetGameURLResponse
    {
        public string   status  { get; set; }
        public string   code    { get; set; }
        public string   message { get; set; }
        public Data     data    { get; set; }

        public class Data
        {
            public string return_url { get; set; }
        }
    }

    public class ApiUserLoginResult
    {
        public string   UserCode    { get; set; }
        public string   Token       { get; set; }

        public ApiUserLoginResult(string strUserCode, string strToken)
        {
            UserCode    = strUserCode;
            Token       = strToken;
        }
    }

    public class CreateAccountRequest
    {
        public string username  { get; set; }
        public string password  { get; set; }
    }

    public class CreateAccountResponse
    {
        public string   status      { get; set; }
        public string   code        { get; set; }
        public string   message     { get; set; }
        public Data     data        { get; set; }

        public class Data
        {
            public int      user_id     { get; set; }
            public string   username    { get; set; }
            public string   usercode    { get; set; }
            public string   token       { get; set; }
        }
    }

    public class SlotDiamondURLResponse
    {
        public string   game_url    { get; set; }
        public int      error       { get; set; }
    }
}
