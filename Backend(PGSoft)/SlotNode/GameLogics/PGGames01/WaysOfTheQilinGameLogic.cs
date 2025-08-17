using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WaysOfTheQilinGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"bwp\":null,\"lw\":null,\"orl\":[8,4,7,10,1,12,2,2,2,3,3,3,0,0,0,6,11,1,2,0,9,12,7,7,4,4,0,5,5,5,12,5,11,11,6,1],\"now\":7776,\"nowpr\":[6,2,6,6,3,6],\"snww\":null,\"esb\":{\"1\":[6,7,8],\"2\":[9,10,11],\"3\":[24,25],\"4\":[27,28,29]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"3\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"4\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1}},\"es\":{\"1\":[6,7,8],\"2\":[9,10,11],\"3\":[24,25],\"4\":[27,28,29]},\"eb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"3\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"4\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1}},\"ptbr\":null,\"ssaw\":0.00,\"rs\":null,\"fs\":null,\"ssi\":null,\"sc\":0,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.05,\"rl\":[8,4,7,10,1,12,2,2,2,3,3,3,0,0,0,6,11,1,2,0,9,12,7,7,4,4,0,5,5,5,12,5,11,11,6,1],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":46.56,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"bwp\":null,\"lw\":null,\"orl\":[8,4,7,10,9,12,2,2,2,3,3,3,5,4,4,4,11,8,2,0,9,12,7,7,4,4,10,5,5,5,12,5,11,11,6,7],\"now\":5184,\"nowpr\":[6,2,4,6,3,6],\"snww\":null,\"esb\":{\"1\":[6,7,8],\"2\":[9,10,11],\"3\":[13,14,15],\"4\":[24,25],\"5\":[27,28,29]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":2,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":2,\"ls\":1},\"3\":{\"fp\":13,\"lp\":15,\"bt\":2,\"ls\":1},\"4\":{\"fp\":24,\"lp\":25,\"bt\":2,\"ls\":1},\"5\":{\"fp\":27,\"lp\":29,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[6,7,8],\"2\":[9,10,11],\"3\":[13,14,15],\"4\":[24,25],\"5\":[27,28,29]},\"eb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":2,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":2,\"ls\":1},\"3\":{\"fp\":13,\"lp\":15,\"bt\":2,\"ls\":1},\"4\":{\"fp\":24,\"lp\":25,\"bt\":2,\"ls\":1},\"5\":{\"fp\":27,\"lp\":29,\"bt\":2,\"ls\":1}},\"ptbr\":null,\"ssaw\":0.0,\"rs\":null,\"fs\":null,\"ssi\":null,\"sc\":0,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.05,\"rl\":[8,4,7,10,9,12,2,2,2,3,3,3,5,4,4,4,11,8,2,0,9,12,7,7,4,4,10,5,5,5,12,5,11,11,6,7],\"sid\":\"1762869609528090626\",\"psid\":\"1762869609528090626\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.69,\"max\":96.69}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public WaysOfTheQilinGameLogic()
        {
            _gameID = GAMEID.WaysOfTheQilin;
            GameName = "WaysOfTheQilin";
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

        }
    }
}
