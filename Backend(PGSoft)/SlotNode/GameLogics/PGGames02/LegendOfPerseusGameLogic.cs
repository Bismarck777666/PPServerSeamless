using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class LegendOfPerseusGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"snww\":null,\"wpl\":null,\"bwp\":null,\"now\":486,\"nowpr\":[3,3,3,3,2,3],\"twbm\":0.0,\"ssaw\":0.00,\"esb\":{\"1\":[0,1,5,6],\"2\":[3,4],\"3\":[8,9,13,14],\"4\":[10,11],\"5\":[15,16,17,20,21,22,25,26,27],\"6\":[23,24]},\"es\":{\"1\":[0,1,5,6],\"2\":[3,4],\"3\":[8,9,13,14],\"4\":[10,11],\"5\":[15,16,17,20,21,22,25,26,27],\"6\":[23,24]},\"esmb\":{\"1\":5,\"3\":3,\"5\":10},\"esm\":{\"1\":5,\"3\":3,\"5\":10},\"orl\":[2,2,4,1,1,2,2,0,9,9,1,1,7,9,9,3,3,3,8,8,3,3,3,1,1,3,3,3,12,6],\"sc\":3,\"gm\":1,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.1,\"rl\":[2,2,4,1,1,2,2,0,9,9,1,1,7,9,9,3,3,3,8,8,3,3,3,1,1,3,3,3,12,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":1.85,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"snww\":null,\"wpl\":null,\"bwp\":null,\"now\":1296,\"nowpr\":[4,3,4,3,3,3],\"twbm\":0.0,\"ssaw\":0.0,\"esb\":{\"1\":[0,1,5,6],\"2\":[8,9,13,14],\"3\":[15,16,17,20,21,22,25,26,27]},\"es\":{\"1\":[0,1,5,6],\"2\":[8,9,13,14],\"3\":[15,16,17,20,21,22,25,26,27]},\"esmb\":{\"1\":5,\"2\":3,\"3\":10},\"esm\":{\"1\":5,\"2\":3,\"3\":10},\"orl\":[2,2,4,11,11,2,2,7,9,9,6,6,7,9,9,3,3,3,8,8,3,3,3,5,5,3,3,3,12,6],\"sc\":0,\"gm\":1,\"rs\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[2,2,4,11,11,2,2,7,9,9,6,6,7,9,9,3,3,3,8,8,3,3,3,5,5,3,3,3,12,6],\"sid\":\"1762869797399322625\",\"psid\":\"1762869797399322625\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.77,\"max\":96.77}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public LegendOfPerseusGameLogic()
        {
            _gameID = GAMEID.LegendOfPerseus;
            GameName = "LegendOfPerseus";
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

            //totalWinBeforeMultiplier
            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);
        }
    }
}
