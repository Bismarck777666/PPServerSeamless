using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlotGamesNode.GameLogics
{
    class ProsperityLionGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected override double DefaultBetSize
        {
            get { return 0.3; }
        }
        protected override int DefaultBetLevel
        {
            get { return 5; }
        }
        protected override int BaseBet
        {
            get
            {
                return 9;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"lw\":null,\"bns\":null,\"orl\":null,\"lf\":null,\"bf\":null,\"ldf\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.03,\"rl\":[9,6,7,2,9,8,6,9,7,0,9,6,4,9,8],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"bns\":null,\"orl\":[9,6,7,2,9,8,6,9,7,0,9,6,4,9,8],\"lf\":null,\"bf\":null,\"ldf\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.03,\"rl\":[9,6,7,2,9,8,6,9,7,0,9,6,4,9,8],\"sid\":\"1762871027525783553\",\"psid\":\"1762871027525783553\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":95.77,\"max\":95.77}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public ProsperityLionGameLogic()
        {
            _gameID = GAMEID.ProsperityLion;
            GameName = "ProsperityLion";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.03, 0.15, 0.3 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["lf"]) && IsArrayOrObject(jsonParams["lf"]) && !IsNullOrEmpty(jsonParams["lf"]["lsp"]))
                jsonParams["lf"]["lsp"] = convertWinByBet((double)jsonParams["lf"]["lsp"], currentBet);

            if (!IsNullOrEmpty(jsonParams["lf"]) && IsArrayOrObject(jsonParams["lf"]) && !IsNullOrEmpty(jsonParams["lf"]["lwa"]))
                jsonParams["lf"]["lwa"] = convertWinByBet((double)jsonParams["lf"]["lwa"], currentBet);

            if (!IsNullOrEmpty(jsonParams["lf"]) && !IsNullOrEmpty(jsonParams["lf"]["lpz"]))
            {
                string strLw = jsonParams["lf"]["lpz"].ToString();
                Dictionary<string, double> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double>>(strLw);
                Dictionary<string, double> convertedLineWins = new Dictionary<string, double>();
                foreach (KeyValuePair<string, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["lf"]["lpz"] = JObject.FromObject(convertedLineWins);
            }

            if (!IsNullOrEmpty(jsonParams["bf"]) && IsArrayOrObject(jsonParams["bf"]) && !IsNullOrEmpty(jsonParams["bf"]["bsp"]))
                jsonParams["bf"]["bsp"] = convertWinByBet((double)jsonParams["bf"]["bsp"], currentBet);

            if (!IsNullOrEmpty(jsonParams["bf"]) && IsArrayOrObject(jsonParams["bf"]) && !IsNullOrEmpty(jsonParams["bf"]["lwa"]))
                jsonParams["bf"]["lwa"] = convertWinByBet((double)jsonParams["bf"]["lwa"], currentBet);

            if (!IsNullOrEmpty(jsonParams["bf"]) && !IsNullOrEmpty(jsonParams["bf"]["bpz"]))
            {
                string strLw = jsonParams["bf"]["bpz"].ToString();
                Dictionary<string, double> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double>>(strLw);
                Dictionary<string, double> convertedLineWins = new Dictionary<string, double>();
                foreach (KeyValuePair<string, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["bf"]["bpz"] = JObject.FromObject(convertedLineWins);
            }
        }
    }
}
