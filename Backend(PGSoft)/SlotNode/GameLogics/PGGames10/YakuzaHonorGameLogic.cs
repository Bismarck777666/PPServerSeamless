using GITProtocol;
using Newtonsoft.Json.Linq;
using SlotGamesNode.GameLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendServer.GameLogics
{
    internal class YakuzaHonorGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"orl\":[9,9,3,8,2,9,3,8,2,7,3,8,0,7,5,8,2,7,5,6,2,7,5,6,6],\"ssaw\":0,\"rns\":null,\"sc\":0,\"wdp\":null,\"pgmbc\":[0,0,0,0,0],\"gmbc\":[0,0,0,0,0],\"gm\":1,\"crtw\":0,\"imw\":false,\"ptbr\":null,\"fs\":null,\"cp\":null,\"gwt\":0,\"fb\":null,\"ctw\":0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[9,9,3,8,2,9,3,8,2,7,3,8,0,7,5,8,2,7,5,6,2,7,5,6,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0,\"blab\":0,\"bl\":16.42,\"tb\":0,\"tbb\":0,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"orl\":[9,9,3,8,4,9,3,8,4,7,3,8,4,7,5,8,4,7,5,6,4,7,5,6,6],\"ssaw\":0,\"rns\":null,\"sc\":0,\"wdp\":null,\"pgmbc\":[0,0,0,0,0],\"gmbc\":[0,0,0,0,0],\"gm\":1,\"crtw\":0,\"imw\":false,\"ptbr\":null,\"fs\":null,\"cp\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":{},\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[9,9,3,8,4,9,3,8,4,7,3,8,4,7,5,8,4,7,5,6,4,7,5,6,6],\"sid\":\"1849575344286338052\",\"psid\":\"1849575344286338052\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":16.42,\"blab\":16.42,\"bl\":16.42,\"tb\":0,\"tbb\":0,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":140,\"max\":140}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public YakuzaHonorGameLogic()
        {
            _gameID     = GAMEID.YakuzaHonor;
            GameName    = "YakuzaHonor";
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

            jsonParams["hashr"] = null;

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

            if (!IsNullOrEmpty(jsonParams["cp"]) && IsArrayOrObject(jsonParams["cp"]))
            {
                if (!IsNullOrEmpty(jsonParams["cp"]["ctw"]) && IsArrayOrObject(jsonParams["cp"]["ctw"]))
                {
                    var ctwValues = jsonParams["cp"]["ctw"].ToObject<Dictionary<string, object>>();
                    foreach (KeyValuePair<string, object> pair in ctwValues)
                    {
                        JArray ctwValueArray = pair.Value as JArray;
                        for (int i = 0; i < ctwValueArray.Count; i++)
                        {
                            JArray ctwValue = ctwValueArray[i] as JArray;
                            for (int j = 0; j < ctwValue.Count; j++)
                                ctwValue[j] = convertWinByBet((double)ctwValue[j], currentBet);

                        }
                    }
                    jsonParams["cp"]["ctw"] = JToken.FromObject(ctwValues);
                }
            }
        }
    }
}
