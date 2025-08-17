using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvertBatch
{
    class Converter
    {

        protected static Converter _sInstance = new Converter();
        public static Converter Instance 
        { 
            get { return _sInstance;} 
        }
        public async Task startConvert()
        {
            string[] strFiles = Directory.GetFiles("F:\\Jobs\\MYCasino\\SlotCityCasinoServer\\SlotGamesNode\\GameLogics\\PPGames", "*.cs");

            string strToken = "AUTHTOKEN@53f290c88a3fef98f1d8d01d3fc4a82a354c0cd590461420e4b30f3f014f5d58~stylename@khmer_kgmcd~SESSION@a5bc583c-8073-4133-aae1-d3cc50b97bbe~SN@b28ea95d";
            for(int i = 0; i < strFiles.Length; i++)
            {
                string strContent    = File.ReadAllText(strFiles[i]);
                string strInitString = findInitString(strContent);
                if (string.IsNullOrEmpty(strInitString))
                    continue;

                string strSymbol = findSymbol(strContent);
                if (string.IsNullOrEmpty(strSymbol))
                    continue;

                InitStringResult initResult = await getDestInitString(strSymbol, strToken);

                var sourceParams    = splitResponseToParams(strInitString);
                var targetParams    = splitResponseToParams(initResult.InitString);

                sourceParams["sc"] = targetParams["sc"];
                sourceParams["defc"] = targetParams["defc"];
                if (sourceParams.ContainsKey("total_bet_min") && targetParams.ContainsKey("total_bet_min"))
                    sourceParams["total_bet_min"] = targetParams["total_bet_min"];

                if (sourceParams.ContainsKey("total_bet_max") && targetParams.ContainsKey("total_bet_max"))
                    sourceParams["total_bet_max"] = targetParams["total_bet_max"];


                string strResult = convertKeyValuesToString(sourceParams);
                strContent       = strContent.Replace(strInitString, strResult);

                string strNewPath = strFiles[i].Replace("PPGames", "PPGames2");
                File.WriteAllText(strNewPath, strContent);
                strToken = initResult.NewToken;
            }
        }
        protected string findInitString(string strFileContent)
        {
            string strStartPattern = "InitDataString\r\n        {\r\n            get\r\n            {\r\n                return \"";
            string strEndPattern   = "\";\r\n            }";
            int beginIndex = strFileContent.IndexOf(strStartPattern);
            if (beginIndex < 0)
                return null;
            beginIndex += strStartPattern.Length;
            int endIndex = strFileContent.IndexOf(strEndPattern, beginIndex);
            return strFileContent.Substring(beginIndex, endIndex - beginIndex);
        }
        protected string findSymbol(string strFileContent)
        {
            string strStartPattern  = "SymbolName\r\n        {\r\n            get\r\n            {\r\n                return \"";
            string strEndPattern    = "\";\r\n            }";
            int beginIndex = strFileContent.IndexOf(strStartPattern);
            if (beginIndex < 0)
                return null;
            beginIndex += strStartPattern.Length;
            int endIndex = strFileContent.IndexOf(strEndPattern, beginIndex);
            return strFileContent.Substring(beginIndex, endIndex - beginIndex);
        }

        protected async Task<InitStringResult> getDestInitString(string strSymbol, string strToken)
        {
            try
            {
                var     httpClient  = new HttpClient();
                string  strURL      = string.Format("https://khmergaming-tw1.ppslot001.net/gs2c/minilobby/start?mgckey={1}&gameSymbol={0}", strSymbol, strToken);
                var     response    = await httpClient.GetAsync(strURL);
                response.EnsureSuccessStatusCode();

                strURL = response.RequestMessage.RequestUri.ToString();
                string strPattern = "mgckey=";
                int beginIndex    = strURL.IndexOf(strPattern);
                if (beginIndex < 0)
                    return null;

                beginIndex          += strPattern.Length;
                int endIndex        = strURL.IndexOf("&", beginIndex);
                if (endIndex < 0)
                    strToken = strURL.Substring(beginIndex);
                else
                    strToken = strURL.Substring(beginIndex, endIndex - beginIndex);
                string strContent   = await response.Content.ReadAsStringAsync();

                string strGameConfig = findStringBetween(strContent, "gameConfig: '", "'");
                JToken gameConfig    = JToken.Parse(strGameConfig);

                string strGameServiceURL = gameConfig["gameService"].ToString();

                KeyValuePair<string, string>[] postParams = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("action", "doInit"),
                    new KeyValuePair<string, string>("symbol", strSymbol),
                    new KeyValuePair<string, string>("cver", "135022"),
                    new KeyValuePair<string, string>("index", "1"),
                    new KeyValuePair<string, string>("counter", "1"),
                    new KeyValuePair<string, string>("repeat", "0"),
                    new KeyValuePair<string, string>("mgckey", strToken)
                };

                response = await httpClient.PostAsync(strGameServiceURL, new FormUrlEncodedContent(postParams));
                response.EnsureSuccessStatusCode();

                string strInitString = await response.Content.ReadAsStringAsync();
                InitStringResult result = new InitStringResult();
                result.InitString   = strInitString;
                result.NewToken     = strToken;
                return result;
            }
            catch
            {
                return null;
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
        protected string convertKeyValuesToString(Dictionary<string, string> keyValues)
        {
            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> pair in keyValues)
            {
                if (pair.Value == null)
                    parts.Add(string.Format("{0}=", pair.Key));
                else
                    parts.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", parts.ToArray());
        }

    }
    class InitStringResult
    {
        public string InitString { get; set; }
        public string NewToken   { get; set; }
    }
}
