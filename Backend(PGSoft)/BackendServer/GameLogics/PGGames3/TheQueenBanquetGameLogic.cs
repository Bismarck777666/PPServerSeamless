using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TheQueenBanquetGameLogic : BasePGSlotGame
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
            get { return 0.1; }
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
                return "{\"si\":{\"wp\":null,\"twp\":null,\"bwp\":null,\"lw\":null,\"lwa\":0.00,\"orl\":[8,4,1,10,7,0,0,0,11,10,2,2,2,3,3,2,2,2,3,3,0,9,12,7,7,12,5,1,11,6],\"trl\":[6,3,3,2],\"now\":0,\"nowpr\":null,\"snww\":null,\"esb\":{\"1\":[5,6,7],\"2\":[10,11,12],\"3\":[13,14],\"4\":[15,16,17],\"5\":[18,19]},\"ebb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":10,\"lp\":12,\"bt\":1,\"ls\":1},\"3\":{\"fp\":13,\"lp\":14,\"bt\":1,\"ls\":2},\"4\":{\"fp\":15,\"lp\":17,\"bt\":1,\"ls\":1},\"5\":{\"fp\":18,\"lp\":19,\"bt\":1,\"ls\":2}},\"esbor\":null,\"ebbor\":null,\"es\":{\"1\":[5,6,7],\"2\":[10,11,12],\"3\":[13,14],\"4\":[15,16,17],\"5\":[18,19]},\"eb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":10,\"lp\":12,\"bt\":1,\"ls\":1},\"3\":{\"fp\":13,\"lp\":14,\"bt\":1,\"ls\":2},\"4\":{\"fp\":15,\"lp\":17,\"bt\":1,\"ls\":1},\"5\":{\"fp\":18,\"lp\":19,\"bt\":1,\"ls\":2}},\"ssaw\":0.00,\"pr\":null,\"tptbr\":null,\"sc\":0,\"rs\":null,\"fs\":null,\"csr\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.05,\"rl\":[8,4,1,10,7,0,0,0,11,10,2,2,2,3,3,2,2,2,3,3,0,9,12,7,7,12,5,1,11,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.62,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.71,\"max\":96.71}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public TheQueenBanquetGameLogic()
        {
            _gameID = GAMEID.TheQueenBanquet;
            GameName = "TheQueenBanquet";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);

            //Line Win Amount
            if (!IsNullOrEmpty(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["lwa"]))
                jsonParams["fs"]["lwa"] = convertWinByBet((double)jsonParams["fs"]["lwa"], currentBet);

            if (!IsNullOrEmpty(jsonParams["lwa"]))
                jsonParams["lwa"] = convertWinByBet((double)jsonParams["lwa"], currentBet);

        }
    }
}
