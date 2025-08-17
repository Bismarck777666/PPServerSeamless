using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MahjongWays2GameLogic : BasePGSlotGame
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
                return "{\"si\": {\"wp\":null,\"lw\":null,\"orl\":[7,6,7,9,10,9,8,3,8,3,1,4,5,2,4,6,0,2,0,10,4,3,8,3,1,4,5,2,7,6,7,9,10,9,8],\"snww\":null,\"ssb\":{\"1\":9,\"2\":11,\"3\":17,\"4\":23,\"5\":25},\"ss\":{\"1\":9,\"2\":11,\"3\":17,\"4\":23,\"5\":25},\"fs\":null,\"rs\":null,\"ssaw\":0.00,\"ptbr\":null,\"sc\":0,\"mi\":0,\"wm\":0,\"lwa\":0.00,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":10.0,\"rl\":[7,6,7,9,10,9,8,3,8,3,1,4,5,2,4,6,0,2,0,10,4,3,8,3,1,4,5,2,7,6,7,9,10,9,8],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":16486.50,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"orl\":[7,6,7,9,10,9,8,3,8,3,2,4,5,2,4,6,7,2,9,10,4,3,8,3,2,4,5,2,7,6,7,9,10,9,8],\"snww\":null,\"ssb\":null,\"ss\":null,\"fs\":null,\"rs\":null,\"ssaw\":0.0,\"ptbr\":null,\"sc\":0,\"mi\":0,\"wm\":1,\"lwa\":0.0,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.05,\"rl\":[7,6,7,9,10,9,8,3,8,3,2,4,5,2,4,6,7,2,9,10,4,3,8,3,2,4,5,2,7,6,7,9,10,9,8],\"sid\":\"1762869571867433984\",\"psid\":\"1762869571867433984\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.95,\"max\":96.95}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public MahjongWays2GameLogic()
        {
            _gameID = GAMEID.MahjongWays2;
            GameName = "MahjongWays2";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
    }
}
