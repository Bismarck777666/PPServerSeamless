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
    class DoubleFortuneGameLogic : BasePGSlotGame
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
                return 30;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"lw\":null,\"lwm\":null,\"slw\":null,\"nk\":null,\"sc\":0,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":5,\"cs\":0.01,\"rl\":[8,16,9,11,5,18,1,2,4,12,6,17,7,15,10],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.41,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"lwm\":null,\"slw\":[0.0],\"nk\":null,\"sc\":0,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":{\"0\":null},\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[8,16,9,11,5,18,3,0,4,12,6,17,7,15,10],\"sid\":\"1762870075422953985\",\"psid\":\"1762870075422953985\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.22,\"max\":96.22}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public DoubleFortuneGameLogic()
        {
            _gameID = GAMEID.DoubleFortune;
            GameName = "DoubleFortune";
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
            //Line Win Amount
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["slw"]))
            {
                var slwArray = jsonParams["slw"] as JArray;
                if(slwArray != null)
                {
                    for(int i = 0; i < slwArray.Count; i++)
                    {
                        slwArray[i] = convertWinByBet((double)slwArray[i], currentBet);
                    }
                }
            }
            if (!IsNullOrEmpty(jsonParams["fs"]))
            {

                if (!IsNullOrEmpty(jsonParams["fs"]["slw"]))
                {
                    var slwArray = jsonParams["fs"]["slw"] as JArray;
                    if (slwArray != null)
                    {
                        for (int i = 0; i < slwArray.Count; i++)
                            slwArray[i] = convertWinByBet((double)slwArray[i], currentBet);
                    }
                }
                if (!IsNullOrEmpty(jsonParams["fs"]["lw"]))
                {
                    string strLw = jsonParams["fs"]["lw"].ToString();
                    Dictionary<int, double> lineWins = JsonConvert.DeserializeObject<Dictionary<int, double>>(strLw);
                    Dictionary<int, double> convertedLineWins = new Dictionary<int, double>();
                    foreach (KeyValuePair<int, double> pair in lineWins)
                    {
                        convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                    }
                    jsonParams["fs"]["lw"] = JObject.FromObject(convertedLineWins);
                }
            }
        }
    }
}
