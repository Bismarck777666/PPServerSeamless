using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class CryptoGoldGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"bwp\":null,\"lw\":null,\"orl\":[8,4,7,3,5,12,1,1,6,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,2,2,6,1,1,8,1,9,11,11,6,5],\"now\":3888,\"nowpr\":[6,3,3,3,4,6],\"snww\":null,\"esb\":{\"1\":[6,7],\"2\":[9,10,11],\"3\":[12,13,14],\"4\":[16,17],\"5\":[19,20],\"6\":[21,22,23],\"7\":[24,25],\"8\":[27,28]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"3\":{\"fp\":12,\"lp\":14,\"bt\":2,\"ls\":1},\"4\":{\"fp\":16,\"lp\":17,\"bt\":2,\"ls\":1},\"5\":{\"fp\":19,\"lp\":20,\"bt\":2,\"ls\":1},\"6\":{\"fp\":21,\"lp\":23,\"bt\":2,\"ls\":1},\"7\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"8\":{\"fp\":27,\"lp\":28,\"bt\":2,\"ls\":1}},\"ebbf\":null,\"es\":{\"1\":[6,7],\"2\":[9,10,11],\"3\":[12,13,14],\"4\":[16,17],\"5\":[19,20],\"6\":[21,22,23],\"7\":[24,25],\"8\":[27,28]},\"eb\":{\"1\":{\"fp\":6,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"3\":{\"fp\":12,\"lp\":14,\"bt\":2,\"ls\":1},\"4\":{\"fp\":16,\"lp\":17,\"bt\":2,\"ls\":1},\"5\":{\"fp\":19,\"lp\":20,\"bt\":2,\"ls\":1},\"6\":{\"fp\":21,\"lp\":23,\"bt\":2,\"ls\":1},\"7\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"8\":{\"fp\":27,\"lp\":28,\"bt\":2,\"ls\":1}},\"rs\":null,\"ssaw\":0.00,\"fs\":null,\"wbwp\":null,\"pr\":null,\"sc\":0,\"mm\":0,\"mnm\":0,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":5,\"cs\":0.01,\"rl\":[8,4,7,3,5,12,1,1,6,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,2,2,6,1,1,8,1,9,11,11,6,5],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.41,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.71,\"max\":96.71}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public CryptoGoldGameLogic()
        {
            _gameID = GAMEID.CryptoGold;
            GameName = "CryptoGold";
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
