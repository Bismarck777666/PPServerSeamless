using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class AlchemyGoldGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected override double DefaultBetSize
        {
            get { return 0.2; }
        }
        protected override int DefaultBetLevel
        {
            get { return 5; }
        }
        protected override int BaseBet
        {
            get
            {
                return 10;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"nwp\":null,\"sw\":null,\"wsc\":null,\"cp\":null,\"cptw\":null,\"ssaw\":0.00,\"wpl\":null,\"orl\":null,\"twbm\":0.0,\"rns\":null,\"gsp\":[1,3,5,9,12,15,19,21,23],\"swp\":null,\"pswp\":null,\"swlp\":null,\"pswlp\":null,\"sc\":2,\"gm\":0,\"inw\":false,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[6,5,3,7,2,9,1,7,0,8,3,7,2,8,4,7,0,8,1,9,2,8,3,5,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.01,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"nwp\":null,\"sw\":null,\"wsc\":null,\"cp\":null,\"cptw\":null,\"ssaw\":0.0,\"wpl\":null,\"orl\":[9,6,3,7,2,6,3,7,2,8,3,7,2,8,4,7,2,8,4,6,2,8,4,6,9],\"twbm\":0.0,\"rns\":null,\"gsp\":null,\"swp\":null,\"pswp\":null,\"swlp\":null,\"pswlp\":null,\"sc\":0,\"gm\":0,\"inw\":false,\"rs\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[9,6,3,7,2,6,3,7,2,8,3,7,2,8,4,7,2,8,4,6,2,8,4,6,9],\"sid\":\"1762870235406283776\",\"psid\":\"1762870235406283776\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.78,\"max\":96.78}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public AlchemyGoldGameLogic()
        {
            _gameID = GAMEID.AlchemyGold;
            GameName = "AlchemyGold";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.02, 0.1, 0.2 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);
            try
            {
                if (!IsNullOrEmpty(jsonParams["ctw"]))
                    jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

                if (!IsNullOrEmpty(jsonParams["cptw"]))
                {
                    string strLw = jsonParams["cptw"].ToString();
                    double[] lineWins = JsonConvert.DeserializeObject<double[]>(strLw);
                    for (int i = 0; i < lineWins.Length; i++)
                    {
                        lineWins[i] = convertWinByBet(lineWins[i], currentBet);
                    }
                    jsonParams["cptw"] = JArray.FromObject(lineWins);
                }
                if (!IsNullOrEmpty(jsonParams["cp"]))
                {
                    string strLw = jsonParams["cp"].ToString();
                    Dictionary<string, double[][]> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double[][]>>(strLw);
                    foreach (KeyValuePair<string, double[][]> pair in lineWins)
                    {
                        for (int i = 0; i < pair.Value.Length; i++)
                        {
                            for (int j = 0; j < pair.Value[i].Length; j++)
                                pair.Value[i][j] = convertWinByBet(pair.Value[i][j], currentBet);
                        }
                    }
                    jsonParams["cp"] = JObject.FromObject(lineWins);
                }
            }
            catch(Exception ex)
            {

            }
        }
    }

}
