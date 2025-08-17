using GITProtocol;
using SlotGamesNode.GameLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendServer.GameLogics
{
    internal class ThreeCrazyPiggiesGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"wpl\":null,\"lw\":null,\"snww\":null,\"ssaw\":0,\"orl\":[10,9,5,8,6,1,11,7,2,3,4,6,1,11,7,10,9,5,8],\"sc\":2,\"wc\":0,\"imw\":false,\"gm\":1,\"mf\":false,\"cwf\":false,\"cwp\":null,\"dpf\":false,\"dpp\":null,\"twbm\":0,\"rns\":null,\"frns\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[10,9,5,8,6,1,11,7,2,3,4,6,1,11,7,10,9,5,8],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0,\"blab\":0,\"bl\":16.42,\"tb\":0,\"tbb\":0,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"wpl\":null,\"lw\":null,\"snww\":null,\"ssaw\":0,\"orl\":[10,9,13,8,6,12,11,7,2,3,4,6,12,11,7,10,9,13,8],\"sc\":0,\"wc\":0,\"imw\":false,\"gm\":1,\"mf\":false,\"cwf\":false,\"cwp\":null,\"dpf\":false,\"dpp\":null,\"twbm\":0,\"rns\":null,\"frns\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[10,9,13,8,6,12,11,7,2,3,4,6,12,11,7,10,9,13,8],\"sid\":\"1849583917003315200\",\"psid\":\"1849583917003315200\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":16.42,\"blab\":16.42,\"bl\":16.42,\"tb\":0,\"tbb\":0,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.72,\"max\":96.72}},\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2160},\"jws\":null}";
            }
        }
        public ThreeCrazyPiggiesGameLogic()
        {
            _gameID     = GAMEID.ThreeCrazyPiggies;
            GameName    = "ThreeCrazyPiggies";
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
