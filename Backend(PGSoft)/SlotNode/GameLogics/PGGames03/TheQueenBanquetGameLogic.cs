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
        protected override bool HasPurEnableOption
        {
            get { return true; }
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
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"twp\":null,\"bwp\":null,\"lw\":null,\"lwa\":0.0,\"orl\":[9,10,12,6,10,4,4,4,10,10,7,7,7,7,11,7,7,7,7,11,5,5,4,4,9,6,6,12,10,10],\"trl\":[8,9,9,9],\"now\":4800,\"nowpr\":[5,3,4,4,4,5],\"snww\":null,\"esb\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11,12,13],\"4\":[15,16,17,18],\"5\":[20,21],\"6\":[22,23]},\"ebb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":2,\"ls\":1},\"3\":{\"fp\":10,\"lp\":13,\"bt\":2,\"ls\":1},\"4\":{\"fp\":15,\"lp\":18,\"bt\":2,\"ls\":1},\"5\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"6\":{\"fp\":22,\"lp\":23,\"bt\":2,\"ls\":1}},\"esbor\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11],\"4\":[12,13],\"5\":[15,16,17],\"6\":[18,19],\"7\":[20,21],\"8\":[22,23]},\"ebbor\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":2,\"ls\":1},\"3\":{\"fp\":10,\"lp\":11,\"bt\":2,\"ls\":1},\"4\":{\"fp\":12,\"lp\":13,\"bt\":1,\"ls\":2},\"5\":{\"fp\":15,\"lp\":17,\"bt\":2,\"ls\":1},\"6\":{\"fp\":18,\"lp\":19,\"bt\":2,\"ls\":1},\"7\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"8\":{\"fp\":22,\"lp\":23,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11,12,13],\"4\":[15,16,17,18],\"5\":[20,21],\"6\":[22,23]},\"eb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":2,\"ls\":1},\"3\":{\"fp\":10,\"lp\":13,\"bt\":2,\"ls\":1},\"4\":{\"fp\":15,\"lp\":18,\"bt\":2,\"ls\":1},\"5\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"6\":{\"fp\":22,\"lp\":23,\"bt\":2,\"ls\":1}},\"ssaw\":0.0,\"pr\":null,\"tptbr\":null,\"sc\":2,\"rs\":null,\"fs\":null,\"csr\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.05,\"rl\":[9,10,12,6,10,4,4,4,10,10,7,7,7,7,11,7,7,7,7,11,5,5,4,4,9,6,6,12,10,10],\"sid\":\"1762870009811456000\",\"psid\":\"1762870009811456000\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
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
