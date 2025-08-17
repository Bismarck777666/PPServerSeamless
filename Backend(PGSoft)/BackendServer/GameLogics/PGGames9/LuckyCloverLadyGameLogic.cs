using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class LuckyCloverLadyGameLogic : BasePGSlotGame
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
                return 20;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"nwpl\":null,\"ssaw\":0.00,\"orl\":[10,10,7,7,7,7,7,2,8,8,3,3,1,3,3,4,4,1,4,4,6,6,2,9,9,11,11,6,6,6],\"ptbr\":null,\"ptu\":null,\"nus\":0,\"inwsf\":false,\"ifa\":false,\"mf\":null,\"usf\":null,\"gm\":0,\"rns\":null,\"twbm\":0.0,\"crtw\":0.0,\"imw\":false,\"sc\":2,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[10,10,7,7,7,7,7,2,8,8,3,3,1,3,3,4,4,1,4,4,6,6,2,9,9,11,11,6,6,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":21.64,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.77,\"max\":96.77}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public LuckyCloverLadyGameLogic()
        {
            _gameID = GAMEID.LuckyCloverLady;
            GameName = "LuckyCloverLady";
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
            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                Dictionary<string, double> swValues = jsonParams["sw"].ToObject<Dictionary<string, double>>();
                foreach (KeyValuePair<string, double> pair in swValues)
                    jsonParams["sw"][pair.Key] = convertWinByBet(pair.Value, currentBet);
            }

        }
    }
}
