using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DragonHatchGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"sw\":null,\"sc\":null,\"cb\":0,\"cbc\":0,\"orl\":[0,5,2,3,1,6,2,3,1,4,2,3,1,4,2,3,1,4,2,8,1,4,2,7,0],\"df\":null,\"mdf\":null,\"rns\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":10,\"cs\":0.02,\"rl\":[0,5,2,3,1,6,2,3,1,4,2,3,1,4,2,3,1,4,2,8,1,4,2,7,0],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":1.86,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"sw\":null,\"sc\":null,\"cb\":4,\"cbc\":0,\"orl\":[0,5,2,3,1,6,2,3,1,4,2,3,1,4,2,3,1,4,2,8,1,4,2,7,0],\"df\":null,\"mdf\":null,\"rns\":null,\"gwt\":-1,\"fb\":null,\"ctw\":1.0,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"1\":5.0},\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[0,5,2,3,1,6,2,3,1,4,2,3,1,4,2,3,1,4,2,8,1,4,2,7,0],\"sid\":\"1762869883185436674\",\"psid\":\"1762869883185436674\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.83,\"max\":96.83}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public DragonHatchGameLogic()
        {
            _gameID = GAMEID.DragonHatch;
            GameName = "DragonHatch";
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

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                var swValues = jsonParams["sw"].ToObject<Dictionary<string, object>>();
                foreach(KeyValuePair<string, object> pair in swValues)
                {
                    JToken swValue = pair.Value as JToken;
                    jsonParams["sw"][pair.Key]["wa"] = convertWinByBet((double)jsonParams["sw"][pair.Key]["wa"], currentBet);
                }
            }

        }

    }
}
