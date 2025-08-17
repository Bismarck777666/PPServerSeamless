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
    internal class NinjaRaccoonFrenzyGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"snww\":null,\"wpl\":null,\"twbm\":0.00,\"rtw\":0.00,\"mwp\":null,\"mlw\":null,\"msnww\":null,\"mwpl\":null,\"mrtw\":0.00,\"ssaw\":0.00,\"mrl\":[11,11,11,0,3,3,5,5,5,0,0,4,13,1,13,0,0,0],\"orl\":[2,2,2,8,8,0,12,1,12,6,0,0,9,9,9,0,0,0],\"omrl\":[11,11,11,0,3,3,5,5,5,0,0,4,13,1,13,0,0,0],\"awip\":[[5,1],[10,1],[11,1],[15,1],[16,1],[17,1],[3,2],[9,2],[10,2],[15,2],[16,2],[17,2]],\"wip\":[[5,1],[10,1],[11,1],[15,1],[16,1],[17,1]],\"mwip\":[[3,2],[9,2],[10,2],[15,2],[16,2],[17,2]],\"gm\":1,\"sc\":2,\"rns\":null,\"mrns\":null,\"imw\":false,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[2,2,2,8,8,0,12,1,12,6,0,0,9,9,9,0,0,0],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":21.65,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"snww\":null,\"wpl\":null,\"twbm\":0.0,\"rtw\":0.0,\"mwp\":null,\"mlw\":null,\"msnww\":null,\"mwpl\":null,\"mrtw\":0.0,\"ssaw\":0.0,\"mrl\":[11,11,11,3,3,3,5,5,5,4,4,4,13,13,13,10,10,10],\"orl\":[2,2,2,8,8,8,12,12,12,6,6,6,9,9,9,7,7,7],\"omrl\":[11,11,11,3,3,3,5,5,5,4,4,4,13,13,13,10,10,10],\"awip\":null,\"wip\":null,\"mwip\":null,\"gm\":1,\"sc\":0,\"rns\":null,\"mrns\":null,\"imw\":false,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":16.8,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"1\":{\"4\":25.0,\"8\":3.0,\"10\":8.0},\"2\":{\"3\":6.0,\"11\":1.0,\"13\":1.0}},\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[2,2,2,8,8,8,12,12,12,6,6,6,9,9,9,7,7,7],\"sid\":\"1762871348733998084\",\"psid\":\"1762871348733998084\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[3,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.82,\"max\":96.82}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public NinjaRaccoonFrenzyGameLogic()
        {
            _gameID = GAMEID.NinjaRaccoonFrenzy;
            GameName = "NinjaRaccoonFrenzy";
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

            if (!IsNullOrEmpty(jsonParams["mrtw"]))
                jsonParams["mrtw"] = convertWinByBet((double)jsonParams["mrtw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["mlw"]))
            {
                string strLw = jsonParams["mlw"].ToString();
                Dictionary<int, double> lineWins = JsonConvert.DeserializeObject<Dictionary<int, double>>(strLw);
                Dictionary<int, double> convertedLineWins = new Dictionary<int, double>();
                foreach (KeyValuePair<int, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["mlw"] = JObject.FromObject(convertedLineWins);
            }
            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["tb"]))
                jsonParams["fs"]["tb"] = convertWinByBet((double)jsonParams["fs"]["tb"], currentBet);
        }
    }
}
