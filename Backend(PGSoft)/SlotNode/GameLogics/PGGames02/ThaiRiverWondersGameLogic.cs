using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class ThaiRiverWondersGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
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
                return "{\"si\":{\"sc\":0,\"wbwp\":null,\"wp\":null,\"twp\":null,\"lw\":null,\"trl\":[3,6,2,4],\"torl\":[3,6,2,4],\"bwp\":null,\"now\":7200,\"nowpr\":[5,3,4,6,4,5],\"snww\":null,\"esb\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11,12],\"4\":[20,21],\"5\":[23,24]},\"ebb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":1,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":1,\"ls\":2},\"3\":{\"fp\":10,\"lp\":12,\"bt\":1,\"ls\":3},\"4\":{\"fp\":20,\"lp\":21,\"bt\":1,\"ls\":2},\"5\":{\"fp\":23,\"lp\":24,\"bt\":1,\"ls\":1}},\"es\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11,12],\"4\":[20,21],\"5\":[23,24]},\"eb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":1,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":1,\"ls\":2},\"3\":{\"fp\":10,\"lp\":12,\"bt\":1,\"ls\":3},\"4\":{\"fp\":20,\"lp\":21,\"bt\":1,\"ls\":2},\"5\":{\"fp\":23,\"lp\":24,\"bt\":1,\"ls\":1}},\"fs\":null,\"rs\":null,\"ssaw\":0.00,\"ptbr\":null,\"tptbr\":null,\"orl\":[8,4,1,10,7,2,2,2,3,3,0,0,0,11,10,0,9,12,7,7,4,4,0,5,5,12,5,1,11,6],\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":10.0,\"rl\":[8,4,1,10,7,2,2,2,3,3,0,0,0,11,10,0,9,12,7,7,4,4,0,5,5,12,5,1,11,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":20794.50,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"sc\":1,\"wbwp\":[],\"wp\":null,\"twp\":null,\"lw\":null,\"trl\":[8,9,0,9],\"torl\":[8,9,0,9],\"bwp\":null,\"now\":3600,\"nowpr\":[5,3,3,4,4,5],\"snww\":null,\"esb\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11,12,13],\"4\":[17,18,19],\"5\":[20,21],\"6\":[22,23]},\"ebb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":2,\"ls\":1},\"3\":{\"fp\":10,\"lp\":13,\"bt\":2,\"ls\":1},\"4\":{\"fp\":17,\"lp\":19,\"bt\":2,\"ls\":1},\"5\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"6\":{\"fp\":22,\"lp\":23,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11,12,13],\"4\":[17,18,19],\"5\":[20,21],\"6\":[22,23]},\"eb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":2,\"ls\":1},\"3\":{\"fp\":10,\"lp\":13,\"bt\":2,\"ls\":1},\"4\":{\"fp\":17,\"lp\":19,\"bt\":2,\"ls\":1},\"5\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"6\":{\"fp\":22,\"lp\":23,\"bt\":2,\"ls\":1}},\"fs\":null,\"rs\":null,\"ssaw\":0.0,\"ptbr\":null,\"tptbr\":null,\"orl\":[9,10,12,6,10,4,4,4,10,10,7,7,7,7,11,10,10,3,3,3,5,5,4,4,9,6,6,12,10,10],\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.05,\"rl\":[9,10,12,6,10,4,4,4,10,10,7,7,7,7,11,10,10,3,3,3,5,5,4,4,9,6,6,12,10,10],\"sid\":\"1762869846615269890\",\"psid\":\"1762869846615269890\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.71,\"max\":96.71}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public ThaiRiverWondersGameLogic()
        {
            _gameID = GAMEID.ThaiRiverWonders;
            GameName = "ThaiRiverWonders";
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
            if (!IsNullOrEmpty(jsonParams["lwa"]))
                jsonParams["lwa"] = convertWinByBet((double)jsonParams["lwa"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["lwa"]))
                jsonParams["fs"]["lwa"] = convertWinByBet((double)jsonParams["fs"]["lwa"], currentBet);

        }
    }
}
