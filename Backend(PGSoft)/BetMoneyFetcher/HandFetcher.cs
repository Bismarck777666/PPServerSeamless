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
    internal class HandFetcher
    {
        private static HandFetcher _sInstance = new HandFetcher();
        public  static HandFetcher Instance
        {
            get { return _sInstance; }
        }
        private Random _random = new Random((int)DateTime.Now.Ticks);

        public async Task fetch(string strCurrency, int multiple)
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
                        var betMoneyInfo = fetchBetMoney(strGameIds[i], multiple);
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
        
        private BetMoneyInfo fetchBetMoney(string strGameId, int multiple)
        {
            var betMoneyInfo = new BetMoneyInfo();

            List<double> csList = new List<double>() { 0.01, 0.1, 1 };
            foreach (double cs in csList) 
            {
                switch (strGameId)
                {
                    case "1368367":
                    case "1666445":
                    case "38":
                    case "1682240":
                    case "90":
                    case "97":
                    case "1804577":
                    case "114":
                    case "1420892":
                    case "94":
                    case "1827457":
                    case "58":
                    case "57":
                    case "36":
                    case "95":
                    case "1879752":
                    case "108":
                    case "1451122":
                    case "98":
                    case "1799745":
                    case "29":
                    case "1543462":
                    case "1338274":
                    case "1397455":
                    case "3":
                    case "100":
                        betMoneyInfo.cs.Add((cs * multiple * 2).ToString());
                        betMoneyInfo.defaultcs = (0.1 * multiple * 2).ToString();
                        break;
                    case "68":
                    case "1850016":
                    case "1695365":
                    case "126":
                    case "1786529":
                        betMoneyInfo.cs.Add((cs * multiple * 4).ToString());
                        betMoneyInfo.defaultcs = (0.1 * multiple * 4).ToString();
                        break;
                    case "39":
                        betMoneyInfo.cs.Add((cs * multiple * 20).ToString());
                        betMoneyInfo.defaultcs = (0.1 * multiple * 20).ToString();
                        break;
                    default:
                        betMoneyInfo.cs.Add((cs * multiple).ToString());
                        betMoneyInfo.defaultcs = (0.1 * multiple).ToString();
                        break;
                }
            }

            betMoneyInfo.ml                 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            betMoneyInfo.defaultbetlevel    = 5;
            return betMoneyInfo;
        }
    }
}
