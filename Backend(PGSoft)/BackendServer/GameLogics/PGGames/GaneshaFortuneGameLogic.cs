using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class GaneshaFortuneGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"twp\":null,\"lw\":null,\"trl\":[3,0,2,4],\"torl\":[3,0,2,4],\"orl\":[8,4,1,10,7,2,2,2,3,3,0,0,6,11,10,0,9,12,7,7,4,4,0,5,5,12,5,1,11,6],\"bwp\":null,\"now\":10800,\"nowpr\":[5,3,6,6,4,5],\"snww\":null,\"esb\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[20,21],\"4\":[23,24]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":7,\"bt\":1,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":1,\"ls\":2},\"3\":{\"fp\":20,\"lp\":21,\"bt\":1,\"ls\":2},\"4\":{\"fp\":23,\"lp\":24,\"bt\":1,\"ls\":1}},\"es\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[20,21],\"4\":[23,24]},\"eb\":{\"1\":{\"fp\":6,\"lp\":7,\"bt\":1,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":1,\"ls\":2},\"3\":{\"fp\":20,\"lp\":21,\"bt\":1,\"ls\":2},\"4\":{\"fp\":23,\"lp\":24,\"bt\":1,\"ls\":1}},\"ssaw\":0.00,\"tptbr\":null,\"ptbr\":null,\"ptbrp\":null,\"wml\":1,\"esm\":null,\"sc\":0,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.05,\"rl\":[8,4,1,10,7,2,2,2,3,3,0,0,6,11,10,0,9,12,7,7,4,4,0,5,5,12,5,1,11,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.65,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.72,\"max\":96.72}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        protected override bool HasPurEnableOption
        {
            get
            {
                return true;
            }
        }
        public GaneshaFortuneGameLogic()
        {
            _gameID = GAMEID.GaneshaFortune;
            GameName = "GaneshaFortune";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
    }
}
