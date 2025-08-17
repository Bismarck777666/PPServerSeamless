using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class GemstonesGoldGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"ssaw\":0.00,\"orl\":[9,9,9,6,6,2,2,2,8,8,10,1,10,5,5,10,1,10,5,5,2,2,2,8,8,9,9,9,6,6],\"cgm\":0,\"gm\":1,\"rns\":null,\"twbm\":0.00,\"crtw\":0.0,\"ir\":false,\"imw\":false,\"sc\":0,\"mf\":{\"v\":{\"0\":0,\"1\":50,\"2\":0,\"3\":10,\"4\":0,\"5\":2},\"t\":{\"0\":-1,\"1\":2,\"2\":-1,\"3\":1,\"4\":-1,\"5\":0},\"l\":{\"0\":-1,\"1\":3,\"2\":-1,\"3\":3,\"4\":-1,\"5\":3}},\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[9,9,9,6,6,2,2,2,8,8,10,1,10,5,5,10,1,10,5,5,2,2,2,8,8,9,9,9,6,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":5.62,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"ssaw\":0.00,\"orl\":[9,9,9,6,6,2,2,2,8,8,10,10,10,5,5,10,10,10,5,5,2,2,2,8,8,9,9,9,6,6],\"cgm\":1,\"gm\":1,\"rns\":null,\"twbm\":0.00,\"crtw\":0.0,\"ir\":false,\"imw\":false,\"sc\":0,\"mf\":{\"v\":{\"0\":0,\"1\":2,\"2\":0,\"3\":2,\"4\":0,\"5\":2},\"t\":{\"0\":-1,\"1\":0,\"2\":-1,\"3\":0,\"4\":-1,\"5\":0},\"l\":{\"0\":-1,\"1\":3,\"2\":-1,\"3\":3,\"4\":-1,\"5\":3}},\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[9,9,9,6,6,2,2,2,8,8,10,10,10,5,5,10,10,10,5,5,2,2,2,8,8,9,9,9,6,6],\"sid\":\"1765353416139347456\",\"psid\":\"1765353416139347456\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":5.62,\"blab\":5.62,\"bl\":5.62,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.71,\"max\":96.71}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }

        public GemstonesGoldGameLogic()
        {
            _gameID = GAMEID.GemstonesGold;
            GameName = "GemstonesGold";
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

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                var swValues = jsonParams["sw"].ToObject<Dictionary<string, double>>();
                foreach (KeyValuePair<string, double> pair in swValues)
                {
                    jsonParams["sw"][pair.Key] = convertWinByBet((double)jsonParams["sw"][pair.Key], currentBet);
                }
            }
        }
    }
}
