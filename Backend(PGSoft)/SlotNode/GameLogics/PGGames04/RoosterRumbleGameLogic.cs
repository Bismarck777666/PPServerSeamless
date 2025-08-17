using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class RoosterRumbleGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"otw\":0.0,\"ssaw\":0.0,\"snww\":null,\"wpl\":null,\"ptbr\":null,\"rns\":null,\"orl\":null,\"sc\":0,\"fs\":null,\"swlb\":{\"1\":{\"p\":6,\"ls\":3},\"2\":{\"p\":21,\"ls\":3}},\"swl\":{\"1\":{\"p\":6,\"ls\":3},\"2\":{\"p\":21,\"ls\":3}},\"gm\":1,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,5,2,13,5,2,0,2,12,7,1,6,4,11,7,1,6,4,11,5,3,0,3,12,8,5,3,13],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.41,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"otw\":0.0,\"ssaw\":0.0,\"snww\":null,\"wpl\":null,\"ptbr\":null,\"rns\":null,\"orl\":[2,11,3,5,13,8,9,7,4,10,9,12,7,11,5,3,13,7,6,6,11,8,4,12,12,11,4,9],\"sc\":0,\"fs\":null,\"swlb\":null,\"swl\":null,\"gm\":1,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,5,2,13,5,2,2,2,12,7,10,6,4,11,7,10,6,4,11,5,3,3,3,12,8,5,3,13],\"sid\":\"1762870169606085632\",\"psid\":\"1762870169606085632\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public RoosterRumbleGameLogic()
        {
            _gameID = GAMEID.RoosterRumble;
            GameName = "RoosterRumble";
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
            //Line Win Amount
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["otw"]))
                jsonParams["otw"] = convertWinByBet((double)jsonParams["otw"], currentBet);

        }
        
    }
}
