using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BetMoneyFetcher
{
    internal class FetchWorker
    {
        private static FetchWorker _sInstance = new FetchWorker();
        public static FetchWorker Instance
        { 
            get { return _sInstance; } 
        }
        public async Task<string> getLaunchURL(string strUserID, string strSymbol)
        {
            string strAccessToken = await TDApi.Instance.loginUser(strUserID, "Abc123");
            string strLaunchURL   = await TDApi.Instance.launchGame(strAccessToken, strSymbol);
            return strLaunchURL;
        }
        private async Task<Dictionary<string, PPGameBetMoneyInfo>> loadMoneyInfoFile(string strCurrency)
        {
            string strFileName = string.Format("chipset({0}).info", strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> oldBetInfos = new Dictionary<string, PPGameBetMoneyInfo>();
            try
            {
                using (BinaryReader binReader = new BinaryReader(File.OpenRead(strFileName)))
                {
                    int unavailableCount = binReader.ReadInt32();
                    for (int i = 0; i < unavailableCount; i++)
                    {
                        string strSymbol = (string)binReader.ReadString();
                        oldBetInfos[strSymbol] = null;
                    }
                    int availableCount = binReader.ReadInt32();
                    for (int i = 0; i < availableCount; i++)
                    {
                        string strSymbol = binReader.ReadString();
                        PPGameBetMoneyInfo gameMoneyInfo = new PPGameBetMoneyInfo();
                        gameMoneyInfo.sc = binReader.ReadString();
                        gameMoneyInfo.defc = binReader.ReadString();
                        gameMoneyInfo.totalBetMin = binReader.ReadString();
                        gameMoneyInfo.totalBetMax = binReader.ReadString();

                        oldBetInfos[strSymbol] = gameMoneyInfo;
                    }
                }
            }
            catch
            {

            }
            return oldBetInfos;
        }

        protected Dictionary<string, string> splitResponseToParams(string strResponse)
        {
            string[] strParts = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts == null || strParts.Length == 0)
                return new Dictionary<string, string>();

            Dictionary<string, string> dicParamValues = new Dictionary<string, string>();
            for (int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParamValues.Length == 2)
                    dicParamValues[strParamValues[0]] = strParamValues[1];
                else if (strParamValues.Length == 1)
                    dicParamValues[strParamValues[0]] = null;
            }
            return dicParamValues;
        }

        public async Task fetchRTPInfo(string strCurrency)
        {
            string      strSymbolString = File.ReadAllText("gamesymbols.txt");
            string[]    strSymbols      = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, PPGameRTPInfo> dicRTPInfos = new Dictionary<string, PPGameRTPInfo>();
            for(int i = 0; i < strSymbols.Length; i++)
            {
                string      strLaunchURL = string.Format("https://rarenew-dk4.pragmaticplay.net/gs2c/playGame.do?key=token%3D3a080ccc-7d65-477f-9b8c-8709d1fe9dba%60%7C%60symbol%3D{0}%60%7C%60platform%3DWEB%60%7C%60language%3Den%60%7C%60currency%3DEUR%60%7C%60cashierUrl%3Dhttps%3A%2F%2Fstake.com%2Fdeposit%60%7C%60lobbyUrl%3Dhttps%3A%2F%2Fstake.com%2Fcasino%2Fhome&method=&isGameUrlApiCalled=true&stylename=rare_stake&lobbyGameSymbol=vs20fruitsw&ppkv=2&excludeMgckey", strSymbols[i]);
                PPGameInfo  gameInfo     = await getPPGameInfo(strLaunchURL, strSymbols[i]);
                if (gameInfo == null)
                    continue;

                string  strInitString   = await getInitString(gameInfo, strSymbols[i], strCurrency);
                var     dicParams       = splitResponseToParams(strInitString);
                PPGameRTPInfo rtpInfo   = new PPGameRTPInfo();
                if (dicParams.ContainsKey("rtp"))
                    rtpInfo.rtp = dicParams["rtp"];
                if (dicParams.ContainsKey("gameInfo"))
                    rtpInfo.gameInfo = dicParams["gameInfo"];

                dicRTPInfos[strSymbols[i]] = rtpInfo;
            }
            string strFileName = "gamertp.info";
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(dicRTPInfos.Count);
                foreach (KeyValuePair<string, PPGameRTPInfo> pair in dicRTPInfos)
                {
                    binWriter.Write(pair.Key);
                    if (string.IsNullOrEmpty(pair.Value.rtp))
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.rtp);

                    if (string.IsNullOrEmpty(pair.Value.gameInfo))
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.gameInfo);
                }
            }
        }
        public async Task fetchBetMoneyInfo(string strCurrency)
        {
            string      strSymbolString  = File.ReadAllText("gamesymbols.txt");
            string[]    strSymbols      = strSymbolString.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, bool> dicSymbols = new Dictionary<string, bool>();
            for(int i = 0; i < strSymbols.Length; i++)
            {
                if (!dicSymbols.ContainsKey(strSymbols[i]))
                    dicSymbols.Add(strSymbols[i], true);
                else
                    continue;
            }
            Dictionary<string, PPGameBetMoneyInfo> oldBetMoneyInfos = await loadMoneyInfoFile(strCurrency);
            Dictionary<string, PPGameBetMoneyInfo> betMoneyInfos    = new Dictionary<string, PPGameBetMoneyInfo>();

            List<string> notAvailableGames = new List<string>();
            foreach(string strSymbol in strSymbols)
            {
                if(oldBetMoneyInfos.ContainsKey(strSymbol))
                {
                    if (oldBetMoneyInfos[strSymbol] == null)
                    {
                        notAvailableGames.Add(strSymbol);
                        continue;
                    }

                    betMoneyInfos[strSymbol] = oldBetMoneyInfos[strSymbol];
                    continue;
                }

                string  strLaunchURL    = string.Format("https://demogamesfree.pragmaticplay.net/gs2c/openGame.do?lang=en&cur={1}&gameSymbol={0}", strSymbol, strCurrency);
                var     gameInfo        = await getPPGameInfo(strLaunchURL, strSymbol);
                if (gameInfo == null)
                {
                    notAvailableGames.Add(strSymbol);
                    continue;
                }

                string strInitString = await getInitString(gameInfo, strSymbol, strCurrency);

                PPGameBetMoneyInfo info = new PPGameBetMoneyInfo();
                info.sc                 = findStringBetween(strInitString, "&sc=", "&");
                info.defc               = findStringBetween(strInitString, "&defc=", "&");
                info.totalBetMax        = findStringBetween(strInitString, "&total_bet_max=", "&");
                info.totalBetMin        = findStringBetween(strInitString, "&total_bet_min=", "&");

                if (info.totalBetMax == null)
                    info.totalBetMax = "";
                if (info.totalBetMin == null)
                    info.totalBetMin = "";

                if (info.defc == null || info.sc == null)
                {

                }
                betMoneyInfos[strSymbol] = info;
            }

            string strFileName = string.Format("chipset({0}).info", strCurrency); 
            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
            {
                binWriter.Write(notAvailableGames.Count);
                for (int i = 0; i < notAvailableGames.Count; i++)
                    binWriter.Write(notAvailableGames[i]);

                binWriter.Write(betMoneyInfos.Count);
                foreach(KeyValuePair<string, PPGameBetMoneyInfo> pair in betMoneyInfos)
                {
                    binWriter.Write(pair.Key);
                    binWriter.Write(pair.Value.sc);
                    if(pair.Value.defc == null)
                        binWriter.Write("");
                    else
                        binWriter.Write(pair.Value.defc);

                    binWriter.Write(pair.Value.totalBetMin);
                    binWriter.Write(pair.Value.totalBetMax);
                }
            }
        }
        private string findStringBetween(string strContent, string strStart, string strEnd)
        {
            int startIndex = strContent.IndexOf(strStart);
            if (startIndex < 0)
                return null;

            startIndex += strStart.Length;
            int endIndex = strContent.IndexOf(strEnd, startIndex);
            if (endIndex < 0)
                return strContent.Substring(startIndex);
            else
                return strContent.Substring(startIndex, endIndex - startIndex);
        }
        private async Task<PPGameInfo> getPPGameInfo(string strURL, string strGameSymbol)
        {
            do
            {
                string strContent = "";
                try
                {
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.GetAsync(strURL);
                    response.EnsureSuccessStatusCode();

                    strContent = await response.Content.ReadAsStringAsync();
                    string strRedirectURL = response.RequestMessage.RequestUri.ToString();
                    string strGameConfig = findStringBetween(strContent, "gameConfig: '", "'");

                    JToken gameConfig = JToken.Parse(strGameConfig);
                    PPGameInfo info = new PPGameInfo();
                    info.GameServiceURL = gameConfig["gameService"].ToString();
                    info.Token = gameConfig["mgckey"].ToString();
                    if (gameConfig["replaySystemUrl"] == null)
                        info.IsReplaySupported = false;
                    else
                        info.IsReplaySupported = true;
                    info.DataPath = gameConfig["datapath"].ToString();
                    string strBootstrapURL = string.Format("{0}desktop/bootstrap.js", info.DataPath);
                    response = await httpClient.GetAsync(strBootstrapURL);
                    response.EnsureSuccessStatusCode();

                    strContent = await response.Content.ReadAsStringAsync();
                    string strUHTRevision = findStringBetween(strContent, ";UHT_REVISION={", "}");
                    info.CVer = findStringBetween(strUHTRevision, "desktop:'", "'");
                    return info;
                }
                catch (Exception ex)
                {
                    if (strGameSymbol == "vs75bronco")
                        return null;

                    if (!strContent.Contains("Sorry"))
                        continue;
                    else
                        return null;
                }
            } while (true);

        }
        private async Task<string> getInitString(PPGameInfo gameInfo, string strSymbol, string strCurrency)
        {
            HttpClient httpClient = new HttpClient();
            KeyValuePair<string, string>[] postParams = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("action", "doInit"),
                new KeyValuePair<string, string>("symbol", strSymbol),
                new KeyValuePair<string, string>("cver",   gameInfo.CVer),
                new KeyValuePair<string, string>("index", "1"),
                new KeyValuePair<string, string>("counter", "1"),
                new KeyValuePair<string, string>("repeat", "0"),
                new KeyValuePair<string, string>("mgckey", gameInfo.Token),
            };
            if(strCurrency == "KRW")
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin",  "https://modoogames-sg13.ppgames.net");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://modoogames-sg13.ppgames.net");
            }
            else if(strCurrency == "IDR")
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin", "https://bcgame-dk2.pragmaticplay.net");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://bcgame-dk2.pragmaticplay.net/gs2c/html5Game.do?jackpotid=0&gname=Starlight%20Christmas&extGame=1&ext=0&cb_target=exist_tab&symbol=vs20schristmas&jurisdictionID=99&lobbyUrl=https%253A%252F%252Fbc.fun%252Fcasino&cashierUrl=https%253A%252F%252Fbc.fun%252Fcasino%2523%252Fwallet%252Fdeposit%252FCHAIN&minimode=0&minilobby=true&mgckey=AUTHTOKEN@e1b757f40148103562d594a4b87c0afe0db698ce2e5081b2c22f7efe8130037b~stylename@bcgame_bcgame~SESSION@78abb0e9-51da-4061-8263-fd35debd0a79~SN@e4ca05e4&tabName=\r\n");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Not_A Brand\";v=\"8\", \"Chromium\";v=\"120\", \"Google Chrome\";v=\"120\"");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0");
            }
            else
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin",  "https://demogamesfree.pragmaticplay.net");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("referer", "https://demogamesfree.pragmaticplay.net");
            }
            var response = await httpClient.PostAsync(gameInfo.GameServiceURL, new FormUrlEncodedContent(postParams));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
    class PPGameInfo
    {
        public string   GameServiceURL { get; set; }
        public string   Token { get; set; }
        public bool     IsReplaySupported { get; set; }
        public string   CVer { get; set; }
        public string   DataPath { get; set; }
        public int      ServerLineCount { get; set; }
        public int      Rows { get; set; }
        public int      PurchaseMultiple { get; set; }
        public double   AnteBetMultiple { get; set; }
        public string   InitString { get; set; }
        public Dictionary<string, string> AdditionalInitParams { get; set; }

        public PPGameInfo()
        {
            this.AdditionalInitParams = new Dictionary<string, string>();
        }
    }
    class PPGameRTPInfo
    {
        public string rtp       { get; set; }
        public string gameInfo  { get; set; }
    }
    class PPGameBetMoneyInfo
    {
        public string sc            { get; set; }
        public string defc          { get; set; }
        public string totalBetMax   { get; set; }
        public string totalBetMin   { get; set; }
    }
}
