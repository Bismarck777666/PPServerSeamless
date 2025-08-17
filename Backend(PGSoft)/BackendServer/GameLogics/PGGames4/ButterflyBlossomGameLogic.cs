using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class ButterflyBlossomGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"wp3x5\":null,\"wpl\":null,\"ptbr\":null,\"lw\":null,\"lwm\":null,\"rl3x5\":[5,6,7,2,0,8,3,1,4,2,0,8,5,6,7],\"swl\":[[6,3],[8,2],[14,1]],\"swlb\":[[6,3],[8,2],[14,1]],\"nswl\":null,\"rswl\":null,\"rs\":null,\"fs\":null,\"sc\":0,\"saw\":0.0,\"tlw\":0.0,\"gm\":1,\"gmi\":0,\"gml\":[1,2,3,5],\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.1,\"rl\":[1,5,6,7,4,2,0,8,0,3,1,4,4,2,0,8,1,5,6,7],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.41,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.74,\"max\":96.74}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public ButterflyBlossomGameLogic()
        {
            _gameID = GAMEID.ButterflyBlossom;
            GameName = "ButterflyBlossom";
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

            if (!IsNullOrEmpty(jsonParams["tlw"]))
                jsonParams["tlw"] = convertWinByBet((double)jsonParams["tlw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["saw"]))
                jsonParams["saw"] = convertWinByBet((double)jsonParams["saw"], currentBet);

        }
    }
}
