using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlotGamesNode.GameLogics
{
    class MajesticTreasuresGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"orl\":[0,5,3,7,2,9,1,7,2,8,3,7,2,8,4,7,2,8,1,9,2,8,3,5,0],\"sc\":0,\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"ogs\":[2,5,9,12,15,19,22],\"gs\":[2,5,9,12,15,19,22],\"gsd\":null,\"rns\":null,\"rngs\":null,\"ssaw\":0.0,\"rc\":false,\"ogml\":1,\"gml\":1,\"cp\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[0,5,3,7,2,9,1,7,2,8,3,7,2,8,4,7,2,8,1,9,2,8,3,5,0],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.01,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.68,\"max\":96.68}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public MajesticTreasuresGameLogic()
        {
            _gameID = GAMEID.MajesticTreasures;
            GameName = "MajesticTreasures";
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
            //Line Win Amount
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["cp"]))
            {
                string strLw = jsonParams["cp"].ToString();
                var lineWins = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, double[]>>>>(strLw);
                foreach (var pair1 in lineWins)
                {
                    foreach (var pair2 in pair1.Value)
                    {
                        foreach (var pair3 in pair2.Value)
                        {
                            for (int i = 0; i < pair3.Value.Length; i++)
                                pair3.Value[i] = convertWinByBet(pair3.Value[i], currentBet);
                        }
                    }                    
                }
                jsonParams["cp"] = JObject.FromObject(lineWins);
            }
        }
    }

}
