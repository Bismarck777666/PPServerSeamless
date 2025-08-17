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
    class CaptainBountyGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return false; }
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"rns\":null,\"wm\":1,\"lwm\":null,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":5,\"cs\":0.01,\"rl\":[5,8,3,2,1,7,6,0,4,2,1,7,5,8,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":1.86,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"rns\":null,\"wm\":1,\"lwm\":null,\"rs\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.2,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"2\":4.0},\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[5,8,3,2,4,7,6,7,2,2,4,7,5,8,3],\"sid\":\"1762869868262097410\",\"psid\":\"1762869868262097410\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.15,\"max\":96.15}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public CaptainBountyGameLogic()
        {
            _gameID = GAMEID.CaptainBounty;
            GameName = "CaptainBounty";
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

            if (!IsNullOrEmpty(jsonParams["lwm"]))
            {
                string strLw = jsonParams["lwm"].ToString();
                Dictionary<string, double> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double>>(strLw);
                Dictionary<string, double> convertedLineWins = new Dictionary<string, double>();
                foreach (KeyValuePair<string, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["lwm"] = JObject.FromObject(convertedLineWins);
            }
            if (!IsNullOrEmpty(jsonParams["rs"]) && IsArrayOrObject(jsonParams["rs"]) && !IsNullOrEmpty(jsonParams["rs"]["bw"]))
                jsonParams["rs"]["bw"] = convertWinByBet((double)jsonParams["rs"]["bw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["rs"]) && IsArrayOrObject(jsonParams["rs"]) && !IsNullOrEmpty(jsonParams["rs"]["aw"]))
                jsonParams["rs"]["aw"] = convertWinByBet((double)jsonParams["rs"]["aw"], currentBet);

        }
    }
}
