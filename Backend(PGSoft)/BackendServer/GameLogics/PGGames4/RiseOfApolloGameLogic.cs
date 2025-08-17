using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class RiseOfApolloGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"sc\":0,\"wp\":null,\"bwp\":null,\"lw\":null,\"snww\":null,\"ssaw\":0.00,\"twbm\":0.0,\"now\":1920,\"nowpr\":[4,2,4,5,3,4],\"ptbr\":null,\"orl\":[8,4,4,4,1,12,2,2,2,3,3,3,0,0,0,6,11,5,2,0,9,1,1,7,4,4,0,5,5,5,12,5,11,11,11,1],\"esb\":{\"1\":[1,2,3],\"2\":[6,7,8],\"3\":[9,10,11],\"4\":[12,13,14],\"5\":[21,22],\"6\":[24,25],\"7\":[27,28,29],\"8\":[32,33,34]},\"ebb\":{\"1\":{\"fp\":1,\"lp\":3,\"bt\":2,\"ls\":1},\"2\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"3\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"4\":{\"fp\":12,\"lp\":14,\"bt\":2,\"ls\":1},\"5\":{\"fp\":21,\"lp\":22,\"bt\":2,\"ls\":1},\"6\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"7\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1},\"8\":{\"fp\":32,\"lp\":34,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[1,2,3],\"2\":[6,7,8],\"3\":[9,10,11],\"4\":[12,13,14],\"5\":[21,22],\"6\":[24,25],\"7\":[27,28,29],\"8\":[32,33,34]},\"eb\":{\"1\":{\"fp\":1,\"lp\":3,\"bt\":2,\"ls\":1},\"2\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"3\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"4\":{\"fp\":12,\"lp\":14,\"bt\":2,\"ls\":1},\"5\":{\"fp\":21,\"lp\":22,\"bt\":2,\"ls\":1},\"6\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"7\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1},\"8\":{\"fp\":32,\"lp\":34,\"bt\":2,\"ls\":1}},\"wbwp\":null,\"fmbwf\":{\"m\":1,\"bwsk\":null,\"bwsp\":null},\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.05,\"rl\":[8,4,4,4,1,12,2,2,2,3,3,3,0,0,0,6,11,5,2,0,9,1,1,7,4,4,0,5,5,5,12,5,11,11,11,1],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.41,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.78,\"max\":96.78}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public RiseOfApolloGameLogic()
        {
            _gameID = GAMEID.RiseOfApollo;
            GameName = "RiseOfApollo";
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
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

        }
    }
}
