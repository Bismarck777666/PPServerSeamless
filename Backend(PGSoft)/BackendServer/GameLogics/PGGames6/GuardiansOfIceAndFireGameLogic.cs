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
    class GuardiansOfIceAndFireGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"norl\":null,\"wp\":null,\"lw\":null,\"lwm\":null,\"snw\":null,\"slw\":0.0,\"slwm\":0.0,\"sc\":0,\"stw\":0.0,\"stwm\":0.0,\"fs\":null,\"orl\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.05,\"rl\":[4,6,7,1,5,3,0,9,2,2,9,0,3,5,1,7,6,4],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.12,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.7,\"max\":96.7}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public GuardiansOfIceAndFireGameLogic()
        {
            _gameID = GAMEID.GuardiansOfIceAndFire;
            GameName = "GuardiansOfIceAndFire";
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
            if (!IsNullOrEmpty(jsonParams["slw"]))
                jsonParams["slw"] = convertWinByBet((double)jsonParams["slw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["slwm"]))
                jsonParams["slwm"] = convertWinByBet((double)jsonParams["slwm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["stw"]))
                jsonParams["stw"] = convertWinByBet((double)jsonParams["stw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["stwm"]))
                jsonParams["stwm"] = convertWinByBet((double)jsonParams["stwm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["stw"]))
                jsonParams["fs"]["stw"] = convertWinByBet((double)jsonParams["fs"]["stw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["stwm"]))
                jsonParams["fs"]["stwm"] = convertWinByBet((double)jsonParams["fs"]["stwm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["slw"]))
                jsonParams["fs"]["slw"] = convertWinByBet((double)jsonParams["fs"]["slw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["slwm"]))
                jsonParams["fs"]["slwm"] = convertWinByBet((double)jsonParams["fs"]["slwm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["lw"]))
            {
                string strLw = jsonParams["fs"]["lw"].ToString();
                Dictionary<string, double> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double>>(strLw);
                Dictionary<string, double> convertedLineWins = new Dictionary<string, double>();
                foreach (KeyValuePair<string, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["fs"]["lw"] = JObject.FromObject(convertedLineWins);
            }
            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["lwm"]))
            {
                string strLw = jsonParams["fs"]["lwm"].ToString();
                Dictionary<string, double> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double>>(strLw);
                Dictionary<string, double> convertedLineWins = new Dictionary<string, double>();
                foreach (KeyValuePair<string, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["fs"]["lwm"] = JObject.FromObject(convertedLineWins);
            }

        }
    }
}
