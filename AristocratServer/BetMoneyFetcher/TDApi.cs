using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace BetMoneyFetcher
{
    internal class TDApi
    {
        private static TDApi _sInstance = new TDApi();
        public static TDApi Instance
        {
            get { return _sInstance; }
        }
        private string _apiKey = "td_game.6TpCDrN5EA58AVFIRkRXFq";
        private string _apiURL = "https://api.td-cube.com/";

        public async Task<bool> createUser(string strUserID, string strPassword)
        {
            try
            {
                SignupUserRequest request = new SignupUserRequest();
                request.username = strUserID;
                request.password = strPassword;
                request.nickname = strUserID;
                request.country = "KR";
                request.language = "ko";

                string strURL = string.Format("{0}api/v1/auth/signup", _apiURL);
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("apikey", _apiKey);
                string strPostContent = JsonConvert.SerializeObject(request);
                var response = await httpClient.PostAsync(strURL, new StringContent(strPostContent, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<string> loginUser(string strUserID, string strPassword)
        {
            try
            {
                SigninUserRequest request = new SigninUserRequest();
                request.username = strUserID;
                request.password = strPassword;

                string  strURL          = string.Format("{0}api/v1/auth/signin", _apiURL);
                var     httpClient      = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("apikey", _apiKey);
                string strPostContent   = JsonConvert.SerializeObject(request);
                var     response        = await httpClient.PostAsync(strURL, new StringContent(strPostContent, Encoding.UTF8, "application/json"));
                string  strResponse     = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                SigninUserResponse loginResponse = JsonConvert.DeserializeObject<SigninUserResponse>(strResponse);
                return loginResponse.accessToken;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> launchGame(string strAccessToken, string strSymbol)
        {
            try
            {
                LaunchRequest request   = new LaunchRequest();
                request.provider        = "PragmaticPlay";
                request.symbol          = strSymbol;
                request.cashierUrl      = "";
                request.lobbyUrl        = "";
                request.device          = "desktop";
                request.skin            = "1";
                request.language        = "ko";

                string strURL = string.Format("{0}api/v1/games/launch", _apiURL);
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", strAccessToken));
                string strPostContent = JsonConvert.SerializeObject(request);
                var response = await httpClient.PostAsync(strURL, new StringContent(strPostContent, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string strResponse = await response.Content.ReadAsStringAsync();

                LaunchResponse launchResponse = JsonConvert.DeserializeObject<LaunchResponse>(strResponse);
                return launchResponse.gameURL;
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<string>> getAllPragmaticPlayGames(string strAccessToken)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", strAccessToken));

                string  strURL      = string.Format("{0}api/v1/games?provider=PragmaticPlay", _apiURL);
                var     response    = await httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();

                string strResponse = await response.Content.ReadAsStringAsync();

                GameListResponse gameListResponse = JsonConvert.DeserializeObject<GameListResponse>(strResponse);
                List<string> strGameSymbols = new List<string>();
                for(int i = 0; i < gameListResponse.items.Count; i++)
                {
                    if (gameListResponse.items[i].gameTypeID == "vs")
                        strGameSymbols.Add(gameListResponse.items[i].gameID);
                }
                return strGameSymbols;
            }
            catch
            {
                return null;
            }
        }
    }
    class SignupUserRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string nickname { get; set; }
        public string language { get; set; }
        public string country  { get; set; }
    }
    class SigninUserRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    class SigninUserResponse
    {
        public int      status          { get; set; }
        public string   accessToken     { get; set; }
        public string   refreshToken    { get; set; }
    }
    class PragmaticGame
    {
        public string gameID        { get; set; }
        public string gameTypeID    { get; set; }
    }
    class GameListResponse
    {
        public List<PragmaticGame> items { get; set; }
    }
    class LaunchRequest
    {
        public string symbol { get; set; }
        public string provider { get; set; }
        public string device { get; set; }
        public string language { get; set; }
        public string skin { get; set; }
        public string cashierUrl { get; set; }
        public string lobbyUrl { get; set; }
    }
    class LaunchResponse
    {
        public int      status  { get; set; }
        public string   gameURL { get; set; }
    }
}
