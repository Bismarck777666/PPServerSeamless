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
    internal class ForgeOfWealthGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"orl\":[9,9,9,3,3,2,2,2,8,8,10,1,10,5,5,10,1,10,5,5,2,2,2,8,8,9,9,9,3,3],\"atw\":0.00,\"twbm\":0.00,\"gm\":1,\"sc\":2,\"rm\":{\"6\":[0,0],\"8\":[0,0],\"21\":[0,0],\"23\":[0,0]},\"cmp\":null,\"acmp\":null,\"ima\":false,\"rns\":null,\"imw\":false,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.5,\"rl\":[9,9,9,3,3,2,2,2,8,8,10,1,10,5,5,10,1,10,5,5,2,2,2,8,8,9,9,9,3,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":10004.39,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"orl\":[9,9,9,3,3,2,2,2,8,8,10,10,10,5,5,10,10,10,5,5,2,2,2,8,8,9,9,9,3,3],\"atw\":0.0,\"twbm\":0.0,\"gm\":1,\"sc\":0,\"rm\":{\"6\":[0,0],\"8\":[0,0],\"21\":[0,0],\"23\":[0,0]},\"cmp\":null,\"acmp\":null,\"ima\":false,\"rns\":null,\"imw\":false,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[9,9,9,3,3,2,2,2,8,8,10,10,10,5,5,10,10,10,5,5,2,2,2,8,8,9,9,9,3,3],\"sid\":\"1762871398679722499\",\"psid\":\"1762871398679722499\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.74,\"max\":96.74}},\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2161},\"jws\":null}";
            }
        }
        public ForgeOfWealthGameLogic()
        {
            _gameID = GAMEID.ForgeOfWealth;
            GameName = "ForgeOfWealth";
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

            if (!IsNullOrEmpty(jsonParams["atw"]))
                jsonParams["atw"] = convertWinByBet((double)jsonParams["atw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                Dictionary<string, double> swValues = jsonParams["sw"].ToObject<Dictionary<string, double>>();
                foreach (KeyValuePair<string, double> pair in swValues)
                    jsonParams["sw"][pair.Key] = convertWinByBet(pair.Value, currentBet);
            }
        }
    }
}
