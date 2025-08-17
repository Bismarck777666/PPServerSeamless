using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class SpeedWinnerGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 50.0; }
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"twp\":null,\"trl\":[3,6,3,4],\"torl\":[3,6,3,4],\"bwp\":null,\"now\":9000,\"nowpr\":[5,3,5,6,4,5],\"snww\":null,\"esb\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[12,13],\"4\":[20,21],\"5\":[23,24]},\"ebb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":2,\"ls\":1},\"3\":{\"fp\":12,\"lp\":13,\"bt\":2,\"ls\":1},\"4\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"5\":{\"fp\":23,\"lp\":24,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[12,13],\"4\":[20,21],\"5\":[23,24]},\"eb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":2,\"ls\":1},\"3\":{\"fp\":12,\"lp\":13,\"bt\":2,\"ls\":1},\"4\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"5\":{\"fp\":23,\"lp\":24,\"bt\":2,\"ls\":1}},\"ptbr\":null,\"tptbr\":null,\"orl\":[8,4,1,10,7,2,2,2,10,10,0,0,1,1,10,0,9,2,7,7,2,2,0,5,5,9,5,1,11,6],\"rs\":null,\"fs\":null,\"sc\":0,\"md\":[[5,10],[17,2],[20,5]],\"tmd\":null,\"acw\":0.0,\"tgm\":17,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,4,1,10,7,2,2,2,10,10,0,0,1,1,10,0,9,2,7,7,2,2,0,5,5,9,5,1,11,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":1.87,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.72,\"max\":96.72}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public SpeedWinnerGameLogic()
        {
            _gameID = GAMEID.SpeedWinner;
            GameName = "SpeedWinner";
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

            if (!IsNullOrEmpty(jsonParams["acw"]))
                jsonParams["acw"] = convertWinByBet((double)jsonParams["acw"], currentBet);


        }
    }
}
