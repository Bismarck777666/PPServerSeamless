using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class SafariWildsGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"twbm\":0.00,\"wp\":null,\"lw\":null,\"ogm\":1,\"gm\":1,\"sc\":0,\"bwp\":null,\"now\":1296,\"nowpr\":null,\"snww\":null,\"esb\":{\"0\":[2,3],\"1\":[7,8,9],\"2\":[11,12,13],\"3\":[14,15],\"4\":[16,17],\"5\":[19,20,21],\"6\":[24,25,26],\"7\":[29,30]},\"ebb\":{\"0\":{\"fp\":2,\"lp\":3,\"bt\":2,\"ls\":1},\"1\":{\"fp\":7,\"lp\":9,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":13,\"bt\":2,\"ls\":1},\"3\":{\"fp\":14,\"lp\":15,\"bt\":2,\"ls\":1},\"4\":{\"fp\":16,\"lp\":17,\"bt\":2,\"ls\":1},\"5\":{\"fp\":19,\"lp\":21,\"bt\":2,\"ls\":1},\"6\":{\"fp\":24,\"lp\":26,\"bt\":2,\"ls\":1},\"7\":{\"fp\":29,\"lp\":30,\"bt\":2,\"ls\":1}},\"es\":{\"0\":[2,3],\"1\":[7,8,9],\"2\":[11,12,13],\"3\":[14,15],\"4\":[16,17],\"5\":[19,20,21],\"6\":[24,25,26],\"7\":[29,30]},\"eb\":{\"0\":{\"fp\":2,\"lp\":3,\"bt\":2,\"ls\":1},\"1\":{\"fp\":7,\"lp\":9,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":13,\"bt\":2,\"ls\":1},\"3\":{\"fp\":14,\"lp\":15,\"bt\":2,\"ls\":1},\"4\":{\"fp\":16,\"lp\":17,\"bt\":2,\"ls\":1},\"5\":{\"fp\":19,\"lp\":21,\"bt\":2,\"ls\":1},\"6\":{\"fp\":24,\"lp\":26,\"bt\":2,\"ls\":1},\"7\":{\"fp\":29,\"lp\":30,\"bt\":2,\"ls\":1}},\"rs\":null,\"fs\":null,\"ssaw\":0.00,\"ptbr\":null,\"orl\":[6,9,3,3,12,7,1,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,7,1,4,4,4,6,9,5,5,12],\"imw\":false,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[6,9,3,3,12,7,1,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,7,1,4,4,4,6,9,5,5,12],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":21.65,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public SafariWildsGameLogic()
        {
            _gameID = GAMEID.SafariWilds;
            GameName = "SafariWilds";
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
        }
    }
}
