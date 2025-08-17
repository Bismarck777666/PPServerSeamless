using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class SuperGolfDriveGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"bwp\":null,\"orl\":[8,4,7,8,8,12,9,2,2,2,3,3,1,1,11,11,0,5,1,1,11,11,0,5,9,2,2,2,3,3,8,4,7,8,8,12],\"now\":8100,\"nowpr\":[6,3,5,5,3,6],\"snww\":null,\"esb\":{\"1\":[3],\"2\":[4],\"3\":[7,8,9],\"4\":[10,11],\"5\":[12,13],\"6\":[14],\"7\":[17],\"8\":[18,19],\"9\":[20],\"10\":[23],\"11\":[25,26,27],\"12\":[28,29],\"13\":[33],\"14\":[34]},\"ebb\":{\"1\":{\"fp\":3,\"lp\":3,\"bt\":1,\"ls\":1},\"2\":{\"fp\":4,\"lp\":4,\"bt\":1,\"ls\":1},\"3\":{\"fp\":7,\"lp\":9,\"bt\":1,\"ls\":1},\"4\":{\"fp\":10,\"lp\":11,\"bt\":2,\"ls\":1},\"5\":{\"fp\":12,\"lp\":13,\"bt\":2,\"ls\":1},\"6\":{\"fp\":14,\"lp\":14,\"bt\":1,\"ls\":1},\"7\":{\"fp\":17,\"lp\":17,\"bt\":1,\"ls\":1},\"8\":{\"fp\":18,\"lp\":19,\"bt\":2,\"ls\":1},\"9\":{\"fp\":20,\"lp\":20,\"bt\":1,\"ls\":1},\"10\":{\"fp\":23,\"lp\":23,\"bt\":1,\"ls\":1},\"11\":{\"fp\":25,\"lp\":27,\"bt\":1,\"ls\":1},\"12\":{\"fp\":28,\"lp\":29,\"bt\":2,\"ls\":1},\"13\":{\"fp\":33,\"lp\":33,\"bt\":1,\"ls\":1},\"14\":{\"fp\":34,\"lp\":34,\"bt\":1,\"ls\":1}},\"es\":{\"1\":[3],\"2\":[4],\"3\":[7,8,9],\"4\":[10,11],\"5\":[12,13],\"6\":[14],\"7\":[17],\"8\":[18,19],\"9\":[20],\"10\":[23],\"11\":[25,26,27],\"12\":[28,29],\"13\":[33],\"14\":[34]},\"eb\":{\"1\":{\"fp\":3,\"lp\":3,\"bt\":1,\"ls\":1},\"2\":{\"fp\":4,\"lp\":4,\"bt\":1,\"ls\":1},\"3\":{\"fp\":7,\"lp\":9,\"bt\":1,\"ls\":1},\"4\":{\"fp\":10,\"lp\":11,\"bt\":2,\"ls\":1},\"5\":{\"fp\":12,\"lp\":13,\"bt\":2,\"ls\":1},\"6\":{\"fp\":14,\"lp\":14,\"bt\":1,\"ls\":1},\"7\":{\"fp\":17,\"lp\":17,\"bt\":1,\"ls\":1},\"8\":{\"fp\":18,\"lp\":19,\"bt\":2,\"ls\":1},\"9\":{\"fp\":20,\"lp\":20,\"bt\":1,\"ls\":1},\"10\":{\"fp\":23,\"lp\":23,\"bt\":1,\"ls\":1},\"11\":{\"fp\":25,\"lp\":27,\"bt\":1,\"ls\":1},\"12\":{\"fp\":28,\"lp\":29,\"bt\":2,\"ls\":1},\"13\":{\"fp\":33,\"lp\":33,\"bt\":1,\"ls\":1},\"14\":{\"fp\":34,\"lp\":34,\"bt\":1,\"ls\":1}},\"ssaw\":0.00,\"ptbr\":null,\"sc\":2,\"gm\":0,\"bmtw\":0.0,\"ms\":0,\"crtw\":0.0,\"imw\":false,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,4,7,8,8,12,9,2,2,2,3,3,1,1,11,11,0,5,1,1,11,11,0,5,9,2,2,2,3,3,8,4,7,8,8,12],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.78,\"max\":96.78}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public SuperGolfDriveGameLogic()
        {
            _gameID = GAMEID.SuperGolfDrive;
            GameName = "SuperGolfDrive";
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
            if (!IsNullOrEmpty(jsonParams["bmtw"]))
                jsonParams["bmtw"] = convertWinByBet((double)jsonParams["bmtw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

        }
    }
}
