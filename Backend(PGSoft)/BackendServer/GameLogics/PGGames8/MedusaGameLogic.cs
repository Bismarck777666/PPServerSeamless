using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MedusaGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"orl\":null,\"mrl\":null,\"wp\":null,\"lw\":null,\"fs\":null,\"f\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[9,6,7,2,4,8,6,9,7,0,9,6,4,7,8],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":95.29,\"max\":95.29}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public MedusaGameLogic()
        {
            _gameID = GAMEID.Medusa;
            GameName = "Medusa";
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

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["wa"]))
                jsonParams["fs"]["wa"] = convertWinByBet((double)jsonParams["fs"]["wa"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["nmbp"]))
                jsonParams["fs"]["nmbp"] = convertWinByBet((double)jsonParams["fs"]["nmbp"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["fbp"]))
                jsonParams["fs"]["fbp"] = convertWinByBet((double)jsonParams["fs"]["fbp"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["mbp"]))
                jsonParams["fs"]["mbp"] = convertWinByBet((double)jsonParams["fs"]["mbp"], currentBet);


            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["mbpl"]))
            {
                var jArray = jsonParams["fs"]["mbpl"] as JArray;
                for(int i = 0; i < jArray.Count; i++)
                    jArray[i] = convertWinByBet((double)jArray[i], currentBet);
            }

        }
    }
}
