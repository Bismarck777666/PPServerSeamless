using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class UltimateStrikerGameLogic : BasePGSlotGame
    {
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"bwp\":null,\"snww\":null,\"now\":4320,\"nowpr\":[6,2,4,5,3,6],\"es\":{\"1\":[6,7,8],\"2\":[9,10,11],\"3\":[12,13,14],\"4\":[21,22],\"5\":[24,25],\"6\":[27,28,29]},\"eb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"3\":{\"fp\":12,\"lp\":14,\"bt\":2,\"ls\":1},\"4\":{\"fp\":21,\"lp\":22,\"bt\":2,\"ls\":1},\"5\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"6\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1}},\"esb\":{\"1\":[6,7,8],\"2\":[9,10,11],\"3\":[12,13,14],\"4\":[21,22],\"5\":[24,25],\"6\":[27,28,29]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"2\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"3\":{\"fp\":12,\"lp\":14,\"bt\":2,\"ls\":1},\"4\":{\"fp\":21,\"lp\":22,\"bt\":2,\"ls\":1},\"5\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"6\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1}},\"ptbr\":null,\"orl\":[8,4,11,11,1,12,2,2,2,3,3,3,0,0,0,6,11,5,2,0,9,1,1,7,4,4,0,5,5,5,12,5,11,9,9,1],\"ssaw\":0.00,\"sc\":3,\"gm\":1,\"omf\":[1,2,3,5],\"mf\":[1,2,3,5],\"mi\":0,\"twbm\":0.0,\"crtw\":0.0,\"imw\":false,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,4,11,11,1,12,2,2,2,3,3,3,0,0,0,6,11,5,2,0,9,1,1,7,4,4,0,5,5,5,12,5,11,9,9,1],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":46.34,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.77,\"max\":96.77}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }

        public UltimateStrikerGameLogic()
        {
            _gameID = GAMEID.UltimateStriker;
            GameName = "UltimateStriker";
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
