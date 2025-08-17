using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class PinataWinsGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"orl\":[7,2,6,10,6,3,1,4,8,0,3,7,6,3,1,4,7,2,6,10],\"ssaw\":0.00,\"rns\":null,\"gsp\":[4,5,10,11,12,13],\"cgsp\":null,\"ngsp\":null,\"wsp\":null,\"mf\":{\"4\":2,\"5\":0,\"10\":10,\"11\":0,\"12\":2,\"13\":0},\"pgm\":1,\"gm\":1,\"cgm\":0,\"sc\":0,\"ptr\":null,\"ir\":false,\"crtw\":0.0,\"imw\":false,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[7,2,6,10,6,3,1,4,8,0,3,7,6,3,1,4,7,2,6,10],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":5.61,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"orl\":[7,2,6,10,6,3,8,4,8,5,3,7,6,3,8,4,7,2,6,10],\"ssaw\":0.00,\"rns\":null,\"gsp\":[4,5,10,11,12,13],\"cgsp\":null,\"ngsp\":null,\"wsp\":null,\"mf\":{\"4\":0,\"5\":0,\"10\":2,\"11\":0,\"12\":0,\"13\":0},\"pgm\":1,\"gm\":1,\"cgm\":1,\"sc\":0,\"ptr\":null,\"ir\":false,\"crtw\":0.0,\"imw\":false,\"fs\":null,\"gwt\":-1,\"fb\":2,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[7,2,6,10,6,3,8,4,8,5,3,7,6,3,8,4,7,2,6,10],\"sid\":\"1781657515667361795\",\"psid\":\"1781657515667361795\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":5.61,\"blab\":5.61,\"bl\":5.61,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":[2,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public PinataWinsGameLogic()
        {
            _gameID = GAMEID.PinataWins;
            GameName = "PinataWins";
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

        }
    }
}
