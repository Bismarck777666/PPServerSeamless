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
    class RavePartyFeverGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"sw\":null,\"sc\":null,\"wpl\":null,\"lw\":null,\"orl\":[4,4,1,1,1,3,3,4,4,8,0,8,3,3,6,2,1,5,1,2,7,6,2,1,8,1,2,7,6,2,1,5,1,2,7,4,4,8,0,8,3,3,4,4,1,1,1,3,3],\"ssaw\":0.00,\"fp\":[0,8,23],\"ft\":[0,0,0],\"pft\":[0,0,0],\"cft\":0,\"rns\":null,\"gm\":1,\"ngm\":1,\"twbm\":0.0,\"cp\":null,\"fft\":{\"65\":0},\"iff\":false,\"sfp\":null,\"tfpp\":null,\"ff\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[4,4,1,1,1,3,3,4,4,8,0,8,3,3,6,2,1,5,1,2,7,6,2,1,8,1,2,7,6,2,1,5,1,2,7,4,4,8,0,8,3,3,4,4,1,1,1,3,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"sw\":null,\"sc\":null,\"wpl\":null,\"lw\":null,\"orl\":[4,4,1,1,1,3,3,4,4,8,8,8,3,3,6,2,1,5,1,2,7,6,2,1,5,1,2,7,6,2,1,5,1,2,7,4,4,8,8,8,3,3,4,4,1,1,1,3,3],\"ssaw\":0.0,\"fp\":[0,8,23],\"ft\":[0,0,0],\"pft\":[0,0,0],\"cft\":0,\"rns\":null,\"gm\":1,\"ngm\":1,\"twbm\":0.0,\"cp\":null,\"fft\":{\"65\":0},\"iff\":false,\"sfp\":null,\"tfpp\":null,\"ff\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[4,4,1,1,1,3,3,4,4,8,8,8,3,3,6,2,1,5,1,2,7,6,2,1,5,1,2,7,6,2,1,5,1,2,7,4,4,8,8,8,3,3,4,4,1,1,1,3,3],\"sid\":\"1762871178784954882\",\"psid\":\"1762871178784954882\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.73,\"max\":96.73}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public RavePartyFeverGameLogic()
        {
            _gameID = GAMEID.RavePartyFever;
            GameName = "RavePartyFever";
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
            if (!IsNullOrEmpty(jsonParams["cp"]))
            {
                string strLw = jsonParams["cp"].ToString();
                Dictionary<string, double[][]> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double[][]>>(strLw);
                foreach (KeyValuePair<string, double[][]> pair in lineWins)
                {
                    for (int i = 0; i < pair.Value.Length; i++)
                    {
                        for (int j = 0; j < pair.Value[i].Length; j++)
                            pair.Value[i][j] = convertWinByBet(pair.Value[i][j], currentBet);
                    }
                }
                jsonParams["cp"] = JObject.FromObject(lineWins);
            }
        }
    }
}
