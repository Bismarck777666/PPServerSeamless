using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class LuckyPiggyGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"snww\":null,\"ssaw\":0.00,\"orl\":[9,2,4,8,10,3,6,1,9,0,12,1,2,0,11,1,3,0,3,0,2,0],\"sc\":0,\"gm\":1,\"swp\":[7,11,15],\"nswp\":[7,11,15],\"rns\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.1,\"rl\":[9,2,4,8,10,3,6,1,9,0,12,1,2,0,11,1,3,0,3,0,2,0],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":1.86,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"snww\":null,\"ssaw\":0.0,\"orl\":[9,2,4,8,10,3,6,12,9,7,12,3,2,8,11,6,3,8,3,11,2,7],\"sc\":0,\"gm\":1,\"swp\":null,\"nswp\":null,\"rns\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.9,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"10\":3.0},\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[9,2,4,8,10,3,6,12,9,7,12,3,2,8,11,6,3,8,3,11,2,7],\"sid\":\"1762869934397860353\",\"psid\":\"1762869934397860353\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.79,\"max\":96.79}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public LuckyPiggyGameLogic()
        {
            _gameID = GAMEID.LuckyPiggy;
            GameName = "LuckyPiggy";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
    }
}
