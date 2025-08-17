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
    class DragonLegendGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"wdi\":null,\"fs\":null,\"sc\":0,\"bns\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.03,\"rl\":[2,6,3,4,9,5,1,8,4,4,7,5,3,6,2],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.13,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"wdi\":null,\"fs\":null,\"sc\":0,\"bns\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.03,\"rl\":[2,6,3,4,9,5,1,8,4,4,7,5,3,6,2],\"sid\":\"1762870462511094272\",\"psid\":\"1762870462511094272\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":97.15,\"max\":97.15}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public DragonLegendGameLogic()
        {
            _gameID = GAMEID.DragonLegend;
            GameName = "DragonLegend";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.03, 0.15, 0.3 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            if (!IsNullOrEmpty(jsonParams["aw"]))
                jsonParams["aw"] = convertWinByBet((double)jsonParams["aw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ssaw"]))
                jsonParams["ssaw"] = convertWinByBet((double)jsonParams["ssaw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["tw"]))
                jsonParams["tw"] = convertWinByBet((double)jsonParams["tw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["aw"]))
                jsonParams["fs"]["aw"] = convertWinByBet((double)jsonParams["fs"]["aw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["np"]))
                jsonParams["np"] = convertWinByBet((double)jsonParams["np"], currentBet);

            if (!IsNullOrEmpty(jsonParams["lw"]))
            {
                string strLw = jsonParams["lw"].ToString();
                Dictionary<string, double[]> lineWins = JsonConvert.DeserializeObject<Dictionary<string, double[]>>(strLw);
                foreach (KeyValuePair<string, double[]> pair in lineWins)
                {
                    for(int i = 0; i < pair.Value.Length; i++)
                        pair.Value[i] = convertWinByBet(pair.Value[i], currentBet);
                }
                jsonParams["lw"] = JObject.FromObject(lineWins);
            }

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]["pl"]))
            {
                if(jsonParams["bns"]["pl"]["0"] != null)
                {
                    var plParams = jsonParams["bns"]["pl"]["0"] as JArray;
                    for (int i = 0; i < plParams.Count; i++)
                    {
                        plParams[i] = convertWinByBet((double)plParams[i], currentBet);
                    }
                }
                if (jsonParams["bns"]["pl"]["1"] != null)
                {
                    var plParams = jsonParams["bns"]["pl"]["1"] as JArray;
                    for (int i = 0; i < plParams.Count; i++)
                    {
                        plParams[i] = convertWinByBet((double)plParams[i], currentBet);
                    }
                }
                if (jsonParams["bns"]["pl"]["2"] != null)
                {
                    var plParams = jsonParams["bns"]["pl"]["2"] as JArray;
                    for (int i = 0; i < plParams.Count; i++)
                    {
                        plParams[i] = convertWinByBet((double)plParams[i], currentBet);
                    }
                }
            }
            if (!IsNullOrEmpty(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]) && !IsNullOrEmpty(jsonParams["bns"]["aw"]))
                jsonParams["bns"]["aw"] = convertWinByBet((double)jsonParams["bns"]["aw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]["pwd"]))
            {
                if (jsonParams["bns"]["pwd"]["paw"] != null)
                {
                    jsonParams["bns"]["pwd"]["paw"] = convertWinByBet((double)jsonParams["bns"]["pwd"]["paw"], currentBet);

                }
            }

        }
    }
}
