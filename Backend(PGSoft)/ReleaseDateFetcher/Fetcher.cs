using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace BetMoneyFetcher
{
    internal class Fetcher
    {
        private static Fetcher _sInstance = new Fetcher();
        public  static Fetcher Instance
        {
            get { return _sInstance; }
        }
        private Random _random = new Random((int)DateTime.Now.Ticks);

        public async Task fetch()
        {
            string strText = File.ReadAllText("content.txt");
            JToken data = JToken.Parse(strText);
            var gameArray = data["dt"]["dagl"] as JArray;

            Dictionary<int, DateTime> dicReleaseDates = new Dictionary<int, DateTime>();
            for(int i = 0; i < gameArray.Count; i++)
            {
                int      gameId         = gameArray[i]["pgid"].ToObject<int>();
                DateTime releaseDate    = gameArray[i]["ct"].ToObject<DateTime>();

                dicReleaseDates[gameId] = releaseDate;
            }

            List<string> strLines = new List<string>();
            foreach(KeyValuePair<int, DateTime> kvp in dicReleaseDates)
            {
                string strLine = string.Format("UPDATE [gitslotpark_pgsoft].[dbo].[gameconfigs] SET releasedate = '{0}' WHERE gameid = {1}", kvp.Value.ToString("yyyy-MM-dd HH:mm:ss"), kvp.Key);
                strLines.Add(strLine);
            }
            string strQuery = string.Join("\r\n", strLines.ToArray());
        }
        protected string genRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        private async Task<BetMoneyInfo> fetchBetMoney(string strGameId)
        {

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://m.pgsoft-games.com/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://m.pgsoft-games.com");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"106\", \"Google Chrome\";v=\"106\", \"Not; A = Brand\";v=\"99\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");

            string strVerifySessionURL = string.Format("https://api.pg-nmga.com/web-api/auth/session/v2/verifyOperatorPlayerSession?traceId={0}", genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                    new KeyValuePair<string, string>("btt", "1"),
                    new KeyValuePair<string, string>("vc", "2"),
                    new KeyValuePair<string, string>("pf", "1"),
                    new KeyValuePair<string, string>("l", "en"),
                    new KeyValuePair<string, string>("gi", strGameId),
                    new KeyValuePair<string, string>("os", "MmY2NWQ3NTEtMDIzMi00ZjMzLWIwMTktNWZmMGMwOTRmNmQ3"),
                    new KeyValuePair<string, string>("otk", "F18990BD-9C7C-110D-D28A-40BACA1288B9"),
            });


            HttpResponseMessage message = await httpClient.PostAsync(strVerifySessionURL, postContent);
            message.EnsureSuccessStatusCode();

            string  strContent          = await message.Content.ReadAsStringAsync();
            JToken  jToken              = JToken.Parse(strContent);
            var     strToken            = jToken["dt"]["tk"].ToString();
            string  strGameURL          = jToken["dt"]["geu"].ToString();
            var     strGameSymbol       = findStringBetwwen(strGameURL, "game-api/", "/");

            string strGetURL = string.Format("https://api.pg-nmga.com/game-api/{0}/v2/GameInfo/Get?traceId={1}", strGameSymbol, genRandomId(8));
            postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("btt", "1"),
                            new KeyValuePair<string, string>("atk", strToken),
                            new KeyValuePair<string, string>("pf", "1"),
            });
            message = await httpClient.PostAsync(strGetURL, postContent);
            message.EnsureSuccessStatusCode();
            strContent = await message.Content.ReadAsStringAsync();

            var betMoneyInfo = new BetMoneyInfo();
            jToken = JToken.Parse(strContent);
            var csArray = jToken["dt"]["cs"] as JArray;
            for (int i = 0; i < csArray.Count; i++)
                betMoneyInfo.cs.Add(csArray[i].ToString());

            var mlArray = jToken["dt"]["ml"] as JArray;
            for (int i = 0; i < mlArray.Count; i++)
                betMoneyInfo.ml.Add(mlArray[i].ToObject<int>());

            betMoneyInfo.defaultcs       = jToken["dt"]["ls"]["si"]["cs"].ToString();
            betMoneyInfo.defaultbetlevel = jToken["dt"]["ls"]["si"]["ml"].ToObject<int>();
            return betMoneyInfo;
        }
        private string findStringBetwwen(string strSource, string strStart, string strEnd)
        {
            int startIndex = strSource.IndexOf(strStart) + strStart.Length;
            int endIndex = strSource.IndexOf(strEnd, startIndex);
            return strSource.Substring(startIndex, endIndex - startIndex);
        }
    }
    public class BetMoneyInfo
    {
        public string       defaultcs       { get; set; }
        public int          defaultbetlevel { get; set; }
        public List<string> cs              { get; set; }
        public List<int>    ml              { get; set; }

        public BetMoneyInfo()
        {
            this.cs = new List<string>();
            this.ml = new List<int>();
        }

    }
}
