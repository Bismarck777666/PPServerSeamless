using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DinnerDelightsGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"lwa\":0.0,\"wp\":null,\"lw\":null,\"wsc\":null,\"wpl\":null,\"orl\":[3,3,3,7,7,7,3,3,90,7,7,7,10,1,10,2,2,2,10,10,10,2,1,2,8,8,8,90,4,4,8,8,8,4,4,4],\"cls\":null,\"sm\":{\"8\":5,\"27\":2},\"bsm\":null,\"nmk\":[8,27],\"tm\":0,\"afw\":0.0,\"sc\":2,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.1,\"rl\":[3,3,3,7,7,7,3,3,90,7,7,7,10,1,10,2,2,2,10,10,10,2,1,2,8,8,8,90,4,4,8,8,8,4,4,4],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":1.85,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"lwa\":0.0,\"wp\":null,\"lw\":null,\"wsc\":null,\"wpl\":null,\"orl\":[3,3,3,7,7,7,3,3,3,7,7,7,10,10,10,2,2,2,10,10,10,2,2,2,8,8,8,4,4,4,8,8,8,4,4,4],\"cls\":null,\"sm\":null,\"bsm\":null,\"nmk\":null,\"tm\":1,\"afw\":0.0,\"sc\":0,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[3,3,3,7,7,7,3,3,3,7,7,7,10,10,10,2,2,2,10,10,10,2,2,2,8,8,8,4,4,4,8,8,8,4,4,4],\"sid\":\"1762869738909742080\",\"psid\":\"1762869738909742080\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public DinnerDelightsGameLogic()
        {
            _gameID = GAMEID.DinnerDelights;
            GameName = "DinnerDelights";
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

            if (!IsNullOrEmpty(jsonParams["lwa"]))
                jsonParams["lwa"] = convertWinByBet((double)jsonParams["lwa"], currentBet);

            //accumulatedWinBeforeMultiplier
            if (!IsNullOrEmpty(jsonParams["afw"]))
                jsonParams["afw"] = convertWinByBet((double)jsonParams["afw"], currentBet);

            
        }
    }
}
