using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WildCoasterGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"wpl\":null,\"ttwp\":null,\"btwp\":null,\"bwp\":null,\"lw\":null,\"snww\":null,\"ssaw\":0.00,\"twbm\":0.00,\"now\":6400,\"nowpr\":[4,4,5,5,4,4],\"ttrl\":[0,3,4,0],\"btrl\":[0,6,3,0],\"orl\":[8,4,1,10,0,0,0,0,1,2,2,9,8,8,11,5,0,0,0,0,9,5,1,11],\"ttorl\":[0,3,4,0],\"btorl\":[0,6,3,0],\"esb\":{\"1\":[4,5,6],\"2\":[9,10],\"3\":[12,13],\"4\":[16,17],\"5\":[18,19]},\"es\":{\"1\":[4,5,6],\"2\":[9,10],\"3\":[12,13],\"4\":[16,17],\"5\":[18,19]},\"gm\":1,\"sc\":3,\"ttwl\":{\"0\":3,\"3\":3},\"btwl\":{\"0\":3,\"3\":3},\"fwc\":[1,4],\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,4,1,10,0,0,0,0,1,2,2,9,8,8,11,5,0,0,0,0,9,5,1,11],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.62,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"wpl\":null,\"ttwp\":null,\"btwp\":null,\"bwp\":null,\"lw\":null,\"snww\":null,\"ssaw\":0.0,\"twbm\":0.0,\"now\":6400,\"nowpr\":[4,4,5,5,4,4],\"ttrl\":[7,3,4,10],\"btrl\":[12,6,3,12],\"orl\":[8,4,3,10,6,6,6,5,7,2,2,9,8,8,11,5,6,6,7,7,9,5,8,11],\"ttorl\":[7,3,4,10],\"btorl\":[12,6,3,12],\"esb\":{\"1\":[4,5,6],\"2\":[9,10],\"3\":[12,13],\"4\":[16,17],\"5\":[18,19]},\"es\":{\"1\":[4,5,6],\"2\":[9,10],\"3\":[12,13],\"4\":[16,17],\"5\":[18,19]},\"gm\":1,\"sc\":0,\"ttwl\":null,\"btwl\":null,\"fwc\":null,\"rs\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,4,3,10,6,6,6,5,7,2,2,9,8,8,11,5,6,6,7,7,9,5,8,11],\"sid\":\"1762870025422651904\",\"psid\":\"1762870025422651904\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.71,\"max\":96.71}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public WildCoasterGameLogic()
        {
            _gameID = GAMEID.WildCoaster;
            GameName = "WildCoaster";
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

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

            
        }
    }
}
