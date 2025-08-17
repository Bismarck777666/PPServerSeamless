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

        public async Task fetch(string strCurrency)
        {
            string strText = File.ReadAllText("gameids.txt");
            string[] strGameIds = strText.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<int, BetMoneyInfo> betMoneyInfos = new Dictionary<int, BetMoneyInfo>();
            string strFileName = string.Format("Chipset({0}).info", strCurrency);
            if (File.Exists(strFileName))
            {
                using (var reader = new BinaryReader(new FileStream(strFileName, FileMode.Open)))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        int gameId          = reader.ReadInt32();
                        int defaultBetLevel = reader.ReadInt32();
                        string defaultcs    = reader.ReadString();

                        BetMoneyInfo betMoneyInfo = new BetMoneyInfo();
                        betMoneyInfo.defaultbetlevel    = defaultBetLevel;
                        betMoneyInfo.defaultcs          = defaultcs;
                        betMoneyInfo.cs                 = new List<string>();
                        int csCount = reader.ReadInt32();
                        for (int j = 0; j < csCount; j++)
                            betMoneyInfo.cs.Add(reader.ReadString());
                        int mlCount = reader.ReadInt32();
                        for (int j = 0; j < mlCount; j++)
                            betMoneyInfo.ml.Add(reader.ReadInt32());

                        betMoneyInfos[gameId] = betMoneyInfo;
                    }
                }
            }

            for (int i = 0; i < strGameIds.Length; i++)
            {
                if (betMoneyInfos.ContainsKey(int.Parse(strGameIds[i])))
                    continue;

                if (strGameIds[i] == "1572362" || strGameIds[i] == "128" || strGameIds[i] == "112" || strGameIds[i] == "123" || strGameIds[i] == "124" || strGameIds[i] == "119")
                {
                    betMoneyInfos.Add(int.Parse(strGameIds[i]), betMoneyInfos[87]);
                }
                else
                {
                    try
                    {
                        var betMoneyInfo = await fetchBetMoney(strGameIds[i]);
                        betMoneyInfos.Add(int.Parse(strGameIds[i]), betMoneyInfo);
                    }
                    catch(Exception ex)
                    {
                        betMoneyInfos.Add(int.Parse(strGameIds[i]), betMoneyInfos[87]);
                    }
                }
            }


            using (var writer = new BinaryWriter(new FileStream(strFileName, FileMode.Create)))
            {
                writer.Write(betMoneyInfos.Count);
                foreach(KeyValuePair<int, BetMoneyInfo> kvp in betMoneyInfos)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.defaultbetlevel);
                    writer.Write(kvp.Value.defaultcs);
                    writer.Write(kvp.Value.cs.Count);
                    for (int i = 0; i < kvp.Value.cs.Count; i++)
                        writer.Write(kvp.Value.cs[i]);

                    writer.Write(kvp.Value.ml.Count);
                    for (int i = 0; i < kvp.Value.ml.Count; i++)
                        writer.Write(kvp.Value.ml[i]);

                }
            }
        }
        protected string genRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        //USD IDR SYP IQD MYR

        /*
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

            string strVerifySessionURL = string.Format("https://api.pgsoft-games.com/web-api/auth/session/v2/verifySession?traceId={0}", genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("btt", "2"),
                            new KeyValuePair<string, string>("vc", "2"),
                            new KeyValuePair<string, string>("pf", "1"),
                            new KeyValuePair<string, string>("l", "en"),
                            new KeyValuePair<string, string>("gi", strGameId),
                            new KeyValuePair<string, string>("tk", "null"),
                            new KeyValuePair<string, string>("otk", "ca7094186b309ee149c55c8822e7ecf2"),
            });

            HttpResponseMessage message = await httpClient.PostAsync(strVerifySessionURL, postContent);
            message.EnsureSuccessStatusCode();

            string strContent = await message.Content.ReadAsStringAsync();
            JToken jToken = JToken.Parse(strContent);
            var strToken = jToken["dt"]["tk"].ToString();
            string strGameURL = jToken["dt"]["geu"].ToString();
            var strGameSymbol = findStringBetwwen(strGameURL, "game-api/", "/");

            string strGetURL = string.Format("https://api.pg-demo.com/web-api/game-proxy/v2/GameName/Get?traceId={0}", genRandomId(8));
            postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("lang", "en"),
                            new KeyValuePair<string, string>("btt", "2"),
                            new KeyValuePair<string, string>("atk", strToken),
                            new KeyValuePair<string, string>("pf", "1"),
                            new KeyValuePair<string, string>("gid", strGameId)
            });
            message = await httpClient.PostAsync(strGetURL, postContent);
            message.EnsureSuccessStatusCode();
            strContent = await message.Content.ReadAsStringAsync();

            strGetURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/GameInfo/Get?traceId={1}", strGameSymbol, genRandomId(8));
            postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("btt", "2"),
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

            betMoneyInfo.defaultcs = jToken["dt"]["ls"]["si"]["cs"].ToString();
            betMoneyInfo.defaultbetlevel = jToken["dt"]["ls"]["si"]["ml"].ToObject<int>();
            return betMoneyInfo;
        }
        */

        //ARS
        /*
        private async Task<BetMoneyInfo> fetchBetMoney(string strGameId)
        {

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://m.pgf-nmu2nd.com/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://m.pgf-nmu2nd.com");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"106\", \"Google Chrome\";v=\"106\", \"Not; A = Brand\";v=\"99\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");

            string strVerifySessionURL = string.Format("https://api.pgf-nmu2nd.com/web-api/auth/session/v2/verifySession?traceId={0}", genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("btt", "1"),
                            new KeyValuePair<string, string>("vc", "0"),
                            new KeyValuePair<string, string>("pf", "1"),
                            new KeyValuePair<string, string>("l", "en"),
                            new KeyValuePair<string, string>("gi", strGameId),
                            new KeyValuePair<string, string>("tk", "300DA7C7-9483-4DC6-B560-CBB56B41AB79"),
                            new KeyValuePair<string, string>("otk", "E035921F-1CAB-41A8-9F76-AB721FA11B0D"),
            });

            HttpResponseMessage message = await httpClient.PostAsync(strVerifySessionURL, postContent);
            message.EnsureSuccessStatusCode();

            string strContent = await message.Content.ReadAsStringAsync();
            JToken jToken = JToken.Parse(strContent);
            var strToken = jToken["dt"]["tk"].ToString();
            string strGameURL = jToken["dt"]["geu"].ToString();
            var strGameSymbol = findStringBetwwen(strGameURL, "game-api/", "/");

            string strGetURL = string.Format("https://api.pg-demo.com/web-api/game-proxy/v2/GameName/Get?traceId={0}", genRandomId(8));
            postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("lang", "en"),
                            new KeyValuePair<string, string>("btt", "2"),
                            new KeyValuePair<string, string>("atk", strToken),
                            new KeyValuePair<string, string>("pf", "1"),
                            new KeyValuePair<string, string>("gid", strGameId)
            });
            message = await httpClient.PostAsync(strGetURL, postContent);
            message.EnsureSuccessStatusCode();
            strContent = await message.Content.ReadAsStringAsync();

            strGetURL = string.Format("https://api.pg-demo.com/game-api/{0}/v2/GameInfo/Get?traceId={1}", strGameSymbol, genRandomId(8));
            postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                            new KeyValuePair<string, string>("btt", "2"),
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

            betMoneyInfo.defaultcs = jToken["dt"]["ls"]["si"]["cs"].ToString();
            betMoneyInfo.defaultbetlevel = jToken["dt"]["ls"]["si"]["ml"].ToObject<int>();
            return betMoneyInfo;
        }
        */

        //MAD
        /*
        private async Task<BetMoneyInfo> fetchBetMoney(string strGameId)
        {

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://m.pgf-nmu2nd.com/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://m.pgf-nmu2nd.com");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"106\", \"Google Chrome\";v=\"106\", \"Not; A = Brand\";v=\"99\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");

            string strVerifySessionURL = string.Format("https://api.pgf-nmu2nd.com/web-api/auth/session/v2/verifyOperatorPlayerSession?traceId={0}", genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                    new KeyValuePair<string, string>("btt", "1"),
                    new KeyValuePair<string, string>("vc", "2"),
                    new KeyValuePair<string, string>("pf", "1"),
                    new KeyValuePair<string, string>("l", "en"),
                    new KeyValuePair<string, string>("gi", strGameId),
                    new KeyValuePair<string, string>("os", "289710_2897_UVPhn4zyCUCDvdBxhTCF"),
                    new KeyValuePair<string, string>("otk", "CDF4A180-80AF-44EC-93DD-F908767BCF7F"),
            });


            HttpResponseMessage message = await httpClient.PostAsync(strVerifySessionURL, postContent);
            message.EnsureSuccessStatusCode();

            string strContent = await message.Content.ReadAsStringAsync();
            JToken jToken = JToken.Parse(strContent);
            var strToken = jToken["dt"]["tk"].ToString();
            string strGameURL = jToken["dt"]["geu"].ToString();
            var strGameSymbol = findStringBetwwen(strGameURL, "game-api/", "/");

            string strGetURL = string.Format("https://api.pgf-nmu2nd.com/game-api/{0}/v2/GameInfo/Get?traceId={1}", strGameSymbol, genRandomId(8));
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

            betMoneyInfo.defaultcs = jToken["dt"]["ls"]["si"]["cs"].ToString();
            betMoneyInfo.defaultbetlevel = jToken["dt"]["ls"]["si"]["ml"].ToObject<int>();
            return betMoneyInfo;
        }
        */

        //TRY
        /*
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

            string strVerifySessionURL = string.Format("https://api.pgf-nmu2nd.com/web-api/auth/session/v2/verifyOperatorPlayerSession?traceId={0}", genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                    new KeyValuePair<string, string>("btt", "1"),
                    new KeyValuePair<string, string>("vc", "2"),
                    new KeyValuePair<string, string>("pf", "1"),
                    new KeyValuePair<string, string>("l", "en"),
                    new KeyValuePair<string, string>("gi", strGameId),
                    new KeyValuePair<string, string>("os", "2720284_2720_gTQqQ962tUi1AeJDxeg"),
                    new KeyValuePair<string, string>("otk", "237A70A6-3028-6570-23A3-65957E0B004A"),
            });


            HttpResponseMessage message = await httpClient.PostAsync(strVerifySessionURL, postContent);
            message.EnsureSuccessStatusCode();

            string strContent = await message.Content.ReadAsStringAsync();
            JToken jToken = JToken.Parse(strContent);
            var strToken = jToken["dt"]["tk"].ToString();
            string strGameURL = jToken["dt"]["geu"].ToString();
            var strGameSymbol = findStringBetwwen(strGameURL, "game-api/", "/");

            string strGetURL = string.Format("https://api.pgf-nmu2nd.com/game-api/{0}/v2/GameInfo/Get?traceId={1}", strGameSymbol, genRandomId(8));
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

            betMoneyInfo.defaultcs = jToken["dt"]["ls"]["si"]["cs"].ToString();
            betMoneyInfo.defaultbetlevel = jToken["dt"]["ls"]["si"]["ml"].ToObject<int>();
            return betMoneyInfo;
        }
        */

        //THB, RT, 
        /*
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

            string strVerifySessionURL = string.Format("https://api.aty7j.com/web-api/auth/session/v2/verifyOperatorPlayerSession?traceId={0}", genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                    new KeyValuePair<string, string>("btt", "1"),
                    new KeyValuePair<string, string>("vc", "2"),
                    new KeyValuePair<string, string>("pf", "1"),
                    new KeyValuePair<string, string>("l", "en"),
                    new KeyValuePair<string, string>("gi", strGameId),
                    new KeyValuePair<string, string>("os", "C474D35445914615AC133DDA5858B653LSMeyJ0b2tlbiI6IjUwNTE2N2I0NjcxZTlkZmI1ZDk3YTFhNTFlMGVjMGZlIiwidXNlcm5hbWUiOiIwVE5TMDEifQ"),
                    new KeyValuePair<string, string>("otk", "7B6D4C96-04AF-4041-88C6-7A5052DA6455"),
            });


            HttpResponseMessage message = await httpClient.PostAsync(strVerifySessionURL, postContent);
            message.EnsureSuccessStatusCode();

            string strContent = await message.Content.ReadAsStringAsync();
            JToken jToken = JToken.Parse(strContent);
            var strToken = jToken["dt"]["tk"].ToString();
            string strGameURL = jToken["dt"]["geu"].ToString();
            var strGameSymbol = findStringBetwwen(strGameURL, "game-api/", "/");

            string strGetURL = string.Format("https://api.aty7j.com/game-api/{0}/v2/GameInfo/Get?traceId={1}", strGameSymbol, genRandomId(8));
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

            betMoneyInfo.defaultcs = jToken["dt"]["ls"]["si"]["cs"].ToString();
            betMoneyInfo.defaultbetlevel = jToken["dt"]["ls"]["si"]["ml"].ToObject<int>();
            return betMoneyInfo;
        }
        */

        //BRL
        private async Task<BetMoneyInfo> fetchBetMoney(string strGameId)
        {

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://m.1adz85lbv.com/");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://m.1adz85lbv.com");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"106\", \"Google Chrome\";v=\"106\", \"Not; A = Brand\";v=\"99\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "same-site");

            string strVerifySessionURL = string.Format("https://api.1adz85lbv.com/web-api/auth/session/v2/verifyOperatorPlayerSession?traceId={0}", genRandomId(8));
            FormUrlEncodedContent postContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                    new KeyValuePair<string, string>("btt", "1"),
                    new KeyValuePair<string, string>("vc", "2"),
                    new KeyValuePair<string, string>("pf", "1"),
                    new KeyValuePair<string, string>("l", "en"),
                    new KeyValuePair<string, string>("gi", strGameId),
                    new KeyValuePair<string, string>("os", "a7kbetbr-28102118-413bd3940837403679ecc9423bbae5dffd0219181409de4d17d9836fc243794f"),
                    new KeyValuePair<string, string>("otk", "N-63f50781-6028-44c0-b81d-7828cfc75868"),
            });


            HttpResponseMessage message = await httpClient.PostAsync(strVerifySessionURL, postContent);
            message.EnsureSuccessStatusCode();

            string strContent = await message.Content.ReadAsStringAsync();
            JToken jToken = JToken.Parse(strContent);
            var strToken = jToken["dt"]["tk"].ToString();
            string strGameURL = jToken["dt"]["geu"].ToString();
            var strGameSymbol = findStringBetwwen(strGameURL, "game-api/", "/");

            string strGetURL = string.Format("https://api.1adz85lbv.com/game-api/{0}/v2/GameInfo/Get?traceId={1}", strGameSymbol, genRandomId(8));
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

            betMoneyInfo.defaultcs = jToken["dt"]["ls"]["si"]["cs"].ToString();
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
