using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class GalacticGemsGameLogic : BasePGSlotGame
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
                return 25;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"ssaw\":0.00,\"blk\":{\"bp\":[0,1,3,4,5,9,15,19,20,21,23,24],\"bpb\":[0,1,3,4,5,9,15,19,20,21,23,24],\"up\":null},\"snw\":null,\"orl\":[7,11,4,8,10,6,3,5,3,6,4,6,2,6,4,9,3,5,3,9,11,10,4,10,11],\"wp\":null,\"lw\":null,\"bns\":null,\"ewp\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":1,\"cs\":0.04,\"rl\":[7,11,4,8,10,6,3,5,3,6,4,6,2,6,4,9,3,5,3,9,11,10,4,10,11],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":9998.55,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.74,\"max\":96.74}},\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2161},\"jws\":null}";
            }
        }
        public GalacticGemsGameLogic()
        {
            _gameID = GAMEID.GalacticGems;
            GameName = "GalacticGems";
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
            if (!IsNullOrEmpty(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]) && !IsNullOrEmpty(jsonParams["bns"]["aw"]))
                jsonParams["bns"]["aw"] = convertWinByBet((double)jsonParams["bns"]["aw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]) && !IsNullOrEmpty(jsonParams["bns"]["twbm"]))
                jsonParams["bns"]["twbm"] = convertWinByBet((double)jsonParams["bns"]["twbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);
        }
    }
}
