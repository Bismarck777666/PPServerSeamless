using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlotGamesNode.GameLogics;

namespace BackendServer.GameLogics
{
    internal class AnubisWrathGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"fs\":null,\"snww\":null,\"ssaw\":0.0,\"sc\":0,\"orl\":[5,11,10,7,12,1,2,3,8,4,0,11,12,1,2,3,5,11,10,7],\"rns\":null,\"ptbr\":null,\"ttmrl\":[3,50,3,10,3],\"ihttmrl\":[false,false,false,false,false],\"ich\":null,\"inttmh\":false,\"gm\":0,\"pgm\":0,\"crtw\":0.0,\"twbm\":0.0,\"imw\":false,\"icc\":false,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[5,11,10,7,12,1,2,3,8,4,0,11,12,1,2,3,5,11,10,7],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":5.48,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"fs\":null,\"snww\":null,\"ssaw\":0.0,\"sc\":0,\"orl\":[5,11,10,7,12,9,6,3,8,4,6,11,12,9,6,3,5,11,10,7],\"rns\":null,\"ptbr\":null,\"ttmrl\":[3,5,3,5,3],\"ihttmrl\":[false,false,false,false,false],\"ich\":[false,false,false,false,false],\"inttmh\":false,\"gm\":0,\"pgm\":0,\"crtw\":0.0,\"twbm\":0.0,\"imw\":false,\"icc\":true,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[5,11,10,7,12,9,6,3,8,4,6,11,12,9,6,3,5,11,10,7],\"sid\":\"1796938616925197825\",\"psid\":\"1796938616925197825\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":5.48,\"blab\":5.48,\"bl\":5.48,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public AnubisWrathGameLogic()
        {
            _gameID = GAMEID.AnubisWrath;
            GameName = "AnubisWrath";
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
            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);


        }
    }
}
