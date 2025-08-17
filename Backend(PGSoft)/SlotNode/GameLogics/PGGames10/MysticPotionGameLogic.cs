using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlotGamesNode.GameLogics;

namespace BackendServer.GameLogics
{
    internal class MysticPotionGameLogic : BasePGSlotGame
    {
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 75.0; }
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
                return "{\"si\":{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"ssaw\":0.00,\"cp\":null,\"sc\":0,\"fs\":null,\"gmb\":0,\"gm\":0,\"cgm\":0,\"bsp\":[[0,1,6,7],[14,15,20,21],[28,29,34,35]],\"bspb\":null,\"bsm\":[15,2,50,2,2,2],\"cls\":null,\"imw\":false,\"twbm\":0.0,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[2,2,6,0,9,9,2,2,6,8,1,8,7,7,5,5,3,3,3,3,5,5,7,7,8,1,8,6,4,4,9,9,0,6,4,4],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":5.48,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"ssaw\":0.00,\"cp\":null,\"sc\":1,\"fs\":null,\"gmb\":1,\"gm\":1,\"cgm\":1,\"bsp\":null,\"bspb\":null,\"bsm\":null,\"cls\":null,\"imw\":false,\"twbm\":0.0,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[2,2,2,9,9,9,6,6,6,8,8,8,7,7,5,5,5,5,3,3,3,3,7,7,8,8,8,6,6,6,9,9,9,4,4,4],\"sid\":\"1796877796505687041\",\"psid\":\"1796877796505687041\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":5.48,\"blab\":5.48,\"bl\":5.48,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.73,\"max\":96.73}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public MysticPotionGameLogic()
        {
            _gameID = GAMEID.MysticPotion;
            GameName = "MysticPotion";
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
                    for (int i = 0; i < swValueArrays.Count; i++)
                        jsonParams["sw"][pair.Key][i] = convertWinByBet((double)swValueArrays[i], currentBet);
                }
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
            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);
        }
    }
}
