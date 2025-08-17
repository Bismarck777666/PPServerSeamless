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
    class EmojiRichesGameLogic : BasePGSlotGame
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
                return 10;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"orl\":[8,8,6,6,9,9,2,0,2,2,7,7,3,1,3,4,4,4,4,4,4,3,1,3,7,7,2,2,0,2,9,9,6,6,8,8],\"wp\":null,\"sw\":null,\"lw\":null,\"sc\":2,\"wsc\":null,\"wpl\":null,\"cls\":null,\"gml\":1,\"stw\":0.0,\"rs\":null,\"fs\":null,\"acw\":0.0,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[8,8,6,6,9,9,2,0,2,2,7,7,3,1,3,4,4,4,4,4,4,3,1,3,7,7,2,2,0,2,9,9,6,6,8,8],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"orl\":[8,8,6,6,9,9,2,2,2,2,7,7,3,3,3,4,4,4,4,4,4,3,3,3,7,7,2,2,2,2,9,9,6,6,8,8],\"wp\":null,\"sw\":null,\"lw\":null,\"sc\":0,\"wsc\":{\"9\":7},\"wpl\":null,\"cls\":null,\"gml\":1,\"stw\":0.8,\"rs\":null,\"fs\":null,\"acw\":0.8,\"gwt\":-1,\"fb\":null,\"ctw\":0.8,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":{\"9\":[4.0]},\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[8,8,6,6,9,9,2,2,2,2,7,7,3,3,3,4,4,4,4,4,4,3,3,3,7,7,2,2,2,2,9,9,6,6,8,8],\"sid\":\"1762870815583414274\",\"psid\":\"1762870815583414274\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.78,\"max\":96.78}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public EmojiRichesGameLogic()
        {
            _gameID = GAMEID.EmojiRiches;
            GameName = "EmojiRiches";
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

            if (!IsNullOrEmpty(jsonParams["stw"]))
                jsonParams["stw"] = convertWinByBet((double)jsonParams["stw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["acw"]))
                jsonParams["acw"] = convertWinByBet((double)jsonParams["acw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["sw"]))
            {
                string strLw = jsonParams["sw"].ToString();
                Dictionary<string, double[]> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double[]>>(strLw);
                foreach (KeyValuePair<string, double[]> pair in lineWins)
                {
                    for (int i = 0; i < pair.Value.Length; i++)
                    {
                        pair.Value[i] = convertWinByBet(pair.Value[i], currentBet);
                    }
                }
                jsonParams["sw"] = JObject.FromObject(lineWins);
            }
        }
    }
}
