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
    class ReelLoveGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"lwbm\":null,\"wa\":0.0,\"orl\":[2,2,2,5,1,8,3,3,3,7,0,6,4,4,4],\"fs\":null,\"gm\":null,\"gtf\":null,\"sc\":0,\"sp\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[2,2,2,5,1,8,3,3,3,7,0,6,4,4,4],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"lwbm\":null,\"wa\":0.0,\"orl\":[2,2,2,5,1,8,3,3,3,7,0,6,4,4,4],\"fs\":null,\"gm\":[{\"gs\":2,\"m\":1,\"isw\":false},{\"gs\":3,\"m\":1,\"isw\":false},{\"gs\":4,\"m\":1,\"isw\":false}],\"gtf\":null,\"sc\":0,\"sp\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[2,2,2,5,1,8,3,3,3,7,0,6,4,4,4],\"sid\":\"1762871058811063298\",\"psid\":\"1762871058811063298\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.96,\"max\":96.96}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public ReelLoveGameLogic()
        {
            _gameID = GAMEID.ReelLove;
            GameName = "ReelLove";
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
            if (!IsNullOrEmpty(jsonParams["lwbm"]))
            {
                string strLw = jsonParams["lwbm"].ToString();
                Dictionary<string, double> lineWins          = JsonConvert.DeserializeObject<Dictionary<string, double>>(strLw);
                Dictionary<string, double> convertedLineWins = new Dictionary<string, double>();
                foreach (KeyValuePair<string, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["lwbm"] = JObject.FromObject(convertedLineWins);
            }

        }
    }
}
