using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class SpiritedWondersGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return false; }
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"bwp\":null,\"snww\":null,\"ssaw\":0.00,\"twbm\":0.0,\"now\":13500,\"nowpr\":[5,2,3,6,5,3,5],\"orl\":[8,7,6,13,2,2,2,2,12,10,0,4,4,4,5,5,5,1,9,11,3,3,3,3,8,7,6,13],\"esb\":{\"1\":[4,5,6,7],\"2\":[11,12,13],\"3\":[14,15,16],\"4\":[20,21,22,23]},\"ebb\":{\"1\":{\"fp\":4,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":13,\"bt\":2,\"ls\":1},\"3\":{\"fp\":14,\"lp\":16,\"bt\":2,\"ls\":1},\"4\":{\"fp\":20,\"lp\":23,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[4,5,6,7],\"2\":[11,12,13],\"3\":[14,15,16],\"4\":[20,21,22,23]},\"eb\":{\"1\":{\"fp\":4,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":13,\"bt\":2,\"ls\":1},\"3\":{\"fp\":14,\"lp\":16,\"bt\":2,\"ls\":1},\"4\":{\"fp\":20,\"lp\":23,\"bt\":2,\"ls\":1}},\"spc\":0,\"sc\":0,\"gm\":1,\"msp\":null,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.05,\"rl\":[8,7,6,13,2,2,2,2,12,10,0,4,4,4,5,5,5,1,9,11,3,3,3,3,8,7,6,13],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.01,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"bwp\":null,\"snww\":null,\"ssaw\":0.0,\"twbm\":0.0,\"now\":900,\"nowpr\":[5,2,2,3,3,5],\"orl\":null,\"esb\":null,\"ebb\":null,\"es\":null,\"eb\":null,\"spc\":0,\"sc\":0,\"gm\":1,\"msp\":null,\"rs\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.05,\"rl\":[8,7,6,13,2,2,2,2,12,10,10,4,4,4,5,5,5,9,9,11,3,3,3,3,8,7,6,13],\"sid\":\"1762870379266716164\",\"psid\":\"1762870379266716164\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.7,\"max\":96.7}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public SpiritedWondersGameLogic()
        {
            _gameID = GAMEID.SpiritedWonders;
            GameName = "SpiritedWonders";
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
        }
    }

}
