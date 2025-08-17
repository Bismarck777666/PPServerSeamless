using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
    class SantasGiftRushGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"rs\":null,\"fs\":null,\"srl\":null,\"hsf\":null,\"cpf\":null,\"orl\":null,\"isa\":false,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[9,4,5,7,5,5,4,9,8,6,3,8,4,7,9],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"rs\":null,\"fs\":null,\"srl\":null,\"hsf\":null,\"cpf\":null,\"orl\":[9,4,5,7,5,5,4,9,8,6,3,8,4,7,9],\"isa\":false,\"gwt\":-1,\"fb\":null,\"ctw\":3.75,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"4\":30.0,\"8\":15.0,\"21\":30.0},\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[9,4,5,7,5,5,4,9,8,6,3,8,4,7,9],\"sid\":\"1762871196111614466\",\"psid\":\"1762871196111614466\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.36,\"max\":96.36}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public SantasGiftRushGameLogic()
        {
            _gameID = GAMEID.SantasGiftRush;
            GameName = "SantasGiftRush";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            try
            {
                base.convertWinsByBet((object)jsonParams, currentBet);
                if (!IsNullOrEmpty(jsonParams["ctw"]))
                    jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

                if(!IsNullOrEmpty(jsonParams["cpf"]) && IsArrayOrObject(jsonParams["cpf"]))
                {
                    var cpfArray = jsonParams["cpf"] as JArray;
                    for(int i = 0; i < cpfArray.Count; i++)
                    {
                        if (!IsNullOrEmpty(cpfArray[i]["tw"]))
                            cpfArray[i]["tw"] = convertWinByBet((double)cpfArray[i]["tw"], currentBet);

                        if (!IsNullOrEmpty(cpfArray[i]["aw"]))
                            cpfArray[i]["aw"] = convertWinByBet((double)cpfArray[i]["aw"], currentBet);

                        if (!IsNullOrEmpty(cpfArray[i]["cp"]))
                            cpfArray[i]["cp"] = convertWinByBet((double)cpfArray[i]["cp"], currentBet);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
