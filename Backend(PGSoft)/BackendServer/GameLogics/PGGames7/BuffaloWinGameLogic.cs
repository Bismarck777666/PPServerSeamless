using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BuffaloWinGameLogic : BasePGSlotGame
    {
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
                return 10;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"s\":0,\"wp\":null,\"lw\":null,\"sw\":null,\"scs\":null,\"wpl\":null,\"bwp\":null,\"bwsc\":null,\"es\":{\"1\":[2,3],\"2\":[4,5,6],\"3\":[8,9]},\"twbm\":0.0,\"orl\":null,\"ix\":false,\"sc\":0,\"gm\":1,\"bsc\":null,\"lrs\":null,\"rs\":null,\"fs\":null,\"acw\":0.0,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[2,1,6,6,0,0,0,2,3,3,2,7],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.71,\"max\":96.71}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public BuffaloWinGameLogic()
        {
            _gameID = GAMEID.BuffaloWin;
            GameName = "BuffaloWin";
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

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                var swValues = jsonParams["sw"].ToObject<Dictionary<string, object>>();
                foreach (KeyValuePair<string, object> pair in swValues)
                {
                    JArray swValueArrays = pair.Value as JArray;
                    if (swValueArrays.Count == 2)
                        jsonParams["sw"][pair.Key][1] = convertWinByBet((double)swValueArrays[1], currentBet);
                }
            }

            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);


        }
    }
}
