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
    internal class DragonHatch2GameLogic : BasePGSlotGame
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
                return 10;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"sw\":null,\"cp\":null,\"wsc\":null,\"wpl\":null,\"lw\":null,\"cls\":null,\"orl\":[7,5,2,3,1,5,2,3,1,4,2,3,0,4,2,3,1,4,2,5,1,4,2,5,7],\"df\":null,\"swp\":[12],\"swpb\":null,\"imw\":false,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[7,5,2,3,1,5,2,3,1,4,2,3,0,4,2,3,1,4,2,5,1,4,2,5,7],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":47.74,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.76,\"max\":96.76}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public DragonHatch2GameLogic()
        {
            _gameID = GAMEID.DragonHatch2;
            GameName = "DragonHatch2";
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
            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                var swValues = jsonParams["sw"].ToObject<Dictionary<string, object>>();
                foreach (KeyValuePair<string, object> pair in swValues)
                {
                    JArray swValue = pair.Value as JArray;
                    for(int i = 0; i < swValue.Count; i++)
                        swValue[i] = convertWinByBet((double)swValue[i], currentBet);
                }
                jsonParams["sw"] = JToken.FromObject(swValues);
            }
            if (!IsNullOrEmpty(jsonParams["cp"]) && IsArrayOrObject(jsonParams["cp"]))
            {
                var swValues = jsonParams["cp"].ToObject<Dictionary<string, object>>();
                foreach (KeyValuePair<string, object> pair in swValues)
                {
                    JArray swValueArray = pair.Value as JArray;
                    for (int i = 0; i < swValueArray.Count; i++)
                    {
                        JArray swValue = swValueArray[i] as JArray;
                        for (int j = 0; j < swValue.Count; j++)
                            swValue[j] = convertWinByBet((double)swValue[j], currentBet);

                    }
                }
                jsonParams["cp"] = JToken.FromObject(swValues);
            }
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);
        }
    }
}
