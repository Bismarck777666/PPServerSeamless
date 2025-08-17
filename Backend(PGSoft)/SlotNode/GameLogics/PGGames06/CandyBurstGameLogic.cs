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
    class CandyBurstGameLogic : BasePGSlotGame
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
                return 20;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"orl\":[9,2,2,3,3,9,4,9,4,5,1,5,6,6,6,8,8,8,2,2,2,7,7,7,5,1,5,4,9,4,9,3,3,2,2,9],\"wp\":null,\"sw\":null,\"cp\":null,\"sc\":2,\"wsc\":null,\"wpl\":null,\"ssaw\":0.0,\"pwp\":null,\"cls\":null,\"fs\":null,\"pft\":null,\"tgml\":0,\"stw\":0.0,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[9,2,2,3,3,9,4,9,4,5,1,5,6,6,6,8,8,8,2,2,2,7,7,7,5,1,5,4,9,4,9,3,3,2,2,9],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.13,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"orl\":[9,2,2,3,3,9,4,9,4,5,7,5,6,6,6,8,8,8,2,2,2,7,7,7,5,6,5,4,9,4,9,3,3,2,2,9],\"wp\":null,\"sw\":null,\"cp\":null,\"sc\":0,\"wsc\":null,\"wpl\":null,\"ssaw\":0.0,\"pwp\":null,\"cls\":null,\"fs\":null,\"pft\":null,\"tgml\":1,\"stw\":0.0,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[9,2,2,3,3,9,4,9,4,5,7,5,6,6,6,8,8,8,2,2,2,7,7,7,5,6,5,4,9,4,9,3,3,2,2,9],\"sid\":\"1762870412330464256\",\"psid\":\"1762870412330464256\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.95,\"max\":96.95}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public CandyBurstGameLogic()
        {
            _gameID = GAMEID.CandyBurst;
            GameName = "CandyBurst";
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

            if (!IsNullOrEmpty(jsonParams["stw"]))
                jsonParams["stw"] = convertWinByBet((double)jsonParams["stw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["sw"]))
            {
                string strLw = jsonParams["sw"].ToString();
                Dictionary<string, double[]> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double[]>>(strLw);
                foreach (KeyValuePair<string, double[]> pair in lineWins)
                {
                    for (int i = 0; i < pair.Value.Length; i++)
                    {
                        pair.Value[i] = convertWinByBet(pair.Value[i], currentBet);
                    }
                }
                jsonParams["sw"] = JObject.FromObject(lineWins);
            }
            if (!IsNullOrEmpty(jsonParams["cp"]))
            {
                string strLw = jsonParams["cp"].ToString();
                Dictionary<string, CandyBonanzaCP> lineWins = JsonConvert.DeserializeObject<Dictionary<string, CandyBonanzaCP>>(strLw);
                foreach (KeyValuePair<string, CandyBonanzaCP> pair in lineWins)
                {
                    foreach (KeyValuePair<string, double[]> pair2 in pair.Value.ctw)
                    {
                        for (int i = 0; i < pair2.Value.Length; i++)
                            pair2.Value[i] = convertWinByBet(pair2.Value[i], currentBet);
                    }
                }
                jsonParams["cp"] = JObject.FromObject(lineWins);
            }
        }
    }
}
