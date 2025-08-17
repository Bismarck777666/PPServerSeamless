using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class GemSaviourConquestGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return false; }
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"orl\":[8,4,7,10,1,12,2,2,2,3,3,3,0,0,0,6,11,1,2,0,9,12,7,7,4,4,0,5,5,5,12,5,11,11,6,1],\"bwp\":null,\"now\":7776,\"nowpr\":[6,2,6,6,3,6],\"snww\":null,\"esb\":{\"1\":[6,7,8],\"2\":[9,10,11],\"3\":[24,25],\"4\":[27,28,29]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"3\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"4\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1}},\"es\":{\"1\":[6,7,8],\"2\":[9,10,11],\"3\":[24,25],\"4\":[27,28,29]},\"eb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"3\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"4\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1}},\"rs\":null,\"fs\":null,\"ssaw\":0.00,\"ptbr\":null,\"sc\":0,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[8,4,7,10,1,12,2,2,2,3,3,3,0,0,0,6,11,1,2,0,9,12,7,7,4,4,0,5,5,5,12,5,11,11,6,1],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.13,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.92,\"max\":96.92}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public GemSaviourConquestGameLogic()
        {
            _gameID = GAMEID.GemSaviourConquest;
            GameName = "GemSaviourConquest";
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

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["wa"]))
                jsonParams["fs"]["wa"] = convertWinByBet((double)jsonParams["fs"]["wa"], currentBet);

        }
    }
}
