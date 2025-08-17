using GITProtocol;
using SlotGamesNode.GameLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendServer.GameLogics
{
    internal class ZombieOutbreakGameLogic : BasePGSlotGame
    {
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"sc\":0,\"wc\":0,\"orl\":[5,2,2,3,0,99,99,99,99,8,4,1,11,0,99,99,99,99,5,2,2,3],\"wm\":{\"1\":[10,15,25],\"3\":[1,2,3]},\"lml\":0,\"rml\":0,\"gm\":1,\"snww\":null,\"wpl\":null,\"ptbr\":null,\"crtw\":0.0,\"cc\":0,\"rns\":null,\"twbm\":0.0,\"ssaw\":0.0,\"imw\":false,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[5,2,2,3,0,99,99,99,99,8,4,1,11,0,99,99,99,99,5,2,2,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":5.12,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"sc\":0,\"wc\":0,\"orl\":[5,2,7,7,10,10,9,5,4,8,11,11,6,10,10,9,5,4,5,2,7,7],\"wm\":{},\"lml\":0,\"rml\":0,\"gm\":1,\"snww\":null,\"wpl\":null,\"ptbr\":null,\"crtw\":0.0,\"cc\":0,\"rns\":null,\"twbm\":0.0,\"ssaw\":0.0,\"imw\":false,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[5,2,7,7,10,10,9,5,4,8,11,11,6,10,10,9,5,4,5,2,7,7],\"sid\":\"1802273545275970560\",\"psid\":\"1802273545275970560\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":5.12,\"blab\":5.12,\"bl\":5.12,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.76,\"max\":96.76}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public ZombieOutbreakGameLogic()
        {
            _gameID     = GAMEID.ZombieOutbreak;
            GameName    = "ZombieOutbreak";
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
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

        }
    }
}
