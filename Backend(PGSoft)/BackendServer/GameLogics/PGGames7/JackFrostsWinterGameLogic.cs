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
    class JackFrostsWinterGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"acw\":0.0,\"wp\":null,\"wwp\":null,\"sw\":null,\"symc\":null,\"wpl\":null,\"lw\":null,\"twbm\":0.0,\"ix\":false,\"wec\":null,\"sc\":0,\"sp\":null,\"gm\":1,\"agmfb\":0,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[7,2,4,4,1,3,3,2,8,2,5,5],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.7,\"max\":96.7}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public JackFrostsWinterGameLogic()
        {
            _gameID = GAMEID.JackFrostsWinter;
            GameName = "JackFrostsWinter";
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
            if (!IsNullOrEmpty(jsonParams["acw"]))
                jsonParams["acw"] = convertWinByBet((double)jsonParams["acw"], currentBet);
            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["sw"]))
            {
                string strLw = jsonParams["sw"].ToString();
                Dictionary<string, double[]> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double[]>>(strLw);
                foreach (KeyValuePair<string, double[]> pair in lineWins)
                    pair.Value[1] = convertWinByBet(pair.Value[1], currentBet);
                jsonParams["sw"] = JObject.FromObject(lineWins);
            }
        }
    }
}