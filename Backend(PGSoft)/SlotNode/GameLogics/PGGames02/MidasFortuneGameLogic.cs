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
    class MidasFortuneGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"orl\":[0,5,3,7,2,9,1,7,2,8,3,7,2,8,4,7,2,8,1,9,2,8,3,5,0],\"ssaw\":0.00,\"rns\":null,\"gsp\":[2,5,9,12,15,19,22],\"cgsp\":null,\"ngsp\":null,\"wgsp\":null,\"cwsp\":null,\"gpps\":[[0,3],[24,3]],\"cgm\":0,\"wsm\":1,\"am\":0,\"ptbr\":null,\"twbm\":0.0,\"cp\":null,\"sc\":0,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":20.0,\"rl\":[0,5,3,7,2,9,1,7,2,8,3,7,2,8,4,7,2,8,1,9,2,8,3,5,0],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":20794.50,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"sw\":{\"4\":[0.6,0.6],\"5\":[0.6,0.6],\"6\":[0.6,0.6],\"7\":[0.3,0.3],\"8\":[1.5,1.5],\"9\":[4.5,4.5]},\"wsc\":null,\"wpl\":null,\"orl\":[9,6,3,7,2,6,3,7,2,8,3,7,2,8,4,7,2,8,4,6,2,8,4,6,9],\"ssaw\":0.0,\"rns\":null,\"gsp\":[],\"cgsp\":null,\"ngsp\":null,\"wgsp\":null,\"cwsp\":null,\"gpps\":[],\"cgm\":0,\"wsm\":1,\"am\":0,\"ptbr\":null,\"twbm\":0.0,\"cp\":null,\"sc\":0,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":32.4,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"4\":[2.0],\"5\":[2.0],\"6\":[2.0],\"7\":[1.0],\"8\":[5.0],\"9\":[15.0]},\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[9,6,3,7,2,6,3,7,2,8,3,7,2,8,4,7,2,8,4,6,2,8,4,6,9],\"sid\":\"1762869816139453442\",\"psid\":\"1762869816139453442\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[3,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.73,\"max\":96.73}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public MidasFortuneGameLogic()
        {
            _gameID = GAMEID.MidasFortune;
            GameName = "MidasFortune";
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

            //totalWinBeforeMultiplier
            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["cp"]))
            {
                string strLw = jsonParams["cp"].ToString();
                Dictionary<int, double[][]> lineWins = JsonConvert.DeserializeObject<Dictionary<int, double[][]>>(strLw);
                foreach (KeyValuePair<int, double[][]> pair in lineWins)
                {
                    for(int i = 0; i < pair.Value.Length; i++)
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
