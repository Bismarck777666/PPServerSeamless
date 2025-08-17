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
    class CandyBonanzaGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"orl\":[9,5,3,2,2,8,6,6,9,7,5,3,7,9,9,3,6,5,7,7,8,6,6,3,7,4,1,9,9,7,2,3,3,9,6,6],\"wp\":null,\"sw\":null,\"cp\":null,\"sc\":1,\"wsc\":null,\"wpl\":null,\"ssaw\":0.0,\"lw\":null,\"cls\":null,\"gml\":1,\"dpp\":[],\"stw\":0.0,\"wlp\":[],\"fs\":null,\"bspl\":[],\"bsplb\":null,\"bsp\":[],\"bspb\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":1,\"cs\":0.02,\"rl\":[9,5,3,2,2,8,6,6,9,7,5,3,7,9,9,3,6,5,7,7,8,6,6,3,7,4,1,9,9,7,2,3,3,9,6,6],\"sid\":\"1658080456962736128\",\"psid\":\"1658080456962736128\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.21,\"blab\":0.01,\"bl\":0.01,\"tb\":0.20,\"tbb\":0.20,\"tw\":0.00,\"np\":-0.20,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"orl\":[2,2,2,9,9,9,6,6,6,8,8,8,7,7,5,5,5,5,3,3,3,3,7,7,8,8,8,6,6,6,9,9,9,4,4,4],\"wp\":null,\"sw\":{\"7\":[2.0,2.0]},\"cp\":null,\"sc\":0,\"wsc\":{\"7\":7},\"wpl\":[13,14,15,21,22,26,27],\"ssaw\":0.0,\"lw\":null,\"cls\":null,\"gml\":1,\"dpp\":[],\"stw\":2.0,\"wlp\":[],\"fs\":null,\"bspl\":null,\"bsplb\":null,\"bsp\":null,\"bspb\":null,\"gwt\":-1,\"fb\":null,\"ctw\":2.0,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[2,2,2,9,9,9,6,6,6,8,8,8,7,7,5,5,5,5,3,3,3,3,7,7,8,8,8,6,6,6,9,9,9,4,4,4],\"sid\":\"1762870268419671555\",\"psid\":\"1762870268419671555\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.72,\"max\":96.72}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public CandyBonanzaGameLogic()
        {
            _gameID = GAMEID.CandyBonanza;
            GameName = "CandyBonanza";
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
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                var swValues = jsonParams["sw"].ToObject<Dictionary<string, object>>();
                foreach (KeyValuePair<string, object> pair in swValues)
                {
                    JArray swValueArrays = pair.Value as JArray;
                    for(int i = 0; i < swValueArrays.Count; i++)
                        jsonParams["sw"][pair.Key][i] = convertWinByBet((double)swValueArrays[i], currentBet);
                }
            }
            if (!IsNullOrEmpty(jsonParams["cp"]))
            {
                string strLw = jsonParams["cp"].ToString();
                Dictionary<string, CandyBonanzaCP> lineWins = JsonConvert.DeserializeObject<Dictionary<string, CandyBonanzaCP>>(strLw);
                foreach (KeyValuePair<string, CandyBonanzaCP> pair in lineWins)
                {
                    foreach(KeyValuePair<string, double[]> pair2 in pair.Value.ctw)
                    {
                        for (int i = 0; i < pair2.Value.Length; i++)
                            pair2.Value[i] = convertWinByBet(pair2.Value[i], currentBet);
                    }
                }
                jsonParams["cp"] = JObject.FromObject(lineWins);
            }
        }
    }
    class CandyBonanzaCP
    {
        public Dictionary<string, double[]> ctw { get; set; }
        public double sp { get; set; }
    }
}
