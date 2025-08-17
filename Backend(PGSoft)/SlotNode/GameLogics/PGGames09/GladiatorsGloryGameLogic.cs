using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class GladiatorsGloryGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"wpl\":null,\"lw\":null,\"snww\":null,\"wm\":{\"4\":2,\"8\":3,\"13\":5},\"orl\":[7,5,9,3,20,1,12,2,20,0,6,11,3,20,1,12,7,5,9],\"rns\":null,\"ptbr\":null,\"cwsp\":null,\"gm\":1,\"sc\":2,\"imw\":false,\"crtw\":0.0,\"actw\":0.0,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[7,5,9,3,20,1,12,2,20,0,6,11,3,20,1,12,7,5,9],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":21.65,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"wpl\":null,\"lw\":null,\"snww\":null,\"wm\":null,\"orl\":[7,5,9,3,4,10,12,2,9,8,6,11,3,4,10,12,7,5,9],\"rns\":null,\"ptbr\":null,\"cwsp\":null,\"gm\":1,\"sc\":0,\"imw\":false,\"crtw\":0.0,\"actw\":0.0,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":1.2,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":{\"12\":1.0},\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[7,5,9,3,4,10,12,2,9,8,6,11,3,4,10,12,7,5,9],\"sid\":\"1762871332850168320\",\"psid\":\"1762871332850168320\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public GladiatorsGloryGameLogic()
        {
            _gameID = GAMEID.GladiatorsGlory;
            GameName = "GladiatorsGlory";
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
            if (!IsNullOrEmpty(jsonParams["actw"]))
                jsonParams["actw"] = convertWinByBet((double)jsonParams["actw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["ssaw"]))
                jsonParams["fs"]["ssaw"] = convertWinByBet((double)jsonParams["fs"]["ssaw"], currentBet);

        }
    }
}
