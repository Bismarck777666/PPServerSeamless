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
    
    class FortuneRabbitGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"orl\":[2,2,0,99,8,8,8,8,2,2,0,99],\"ift\":false,\"iff\":false,\"cpf\":{\"1\":{\"p\":4,\"bv\":1000.000,\"m\":500.0},\"2\":{\"p\":5,\"bv\":40.000,\"m\":20.0},\"3\":{\"p\":6,\"bv\":10.000,\"m\":5.0},\"4\":{\"p\":7,\"bv\":1.000,\"m\":0.5}},\"cptw\":0.0,\"crtw\":0.0,\"imw\":false,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.02,\"rl\":[2,2,0,99,8,8,8,8,2,2,0,99],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.01,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public FortuneRabbitGameLogic()
        {
            _gameID = GAMEID.FortuneRabbit;
            GameName = "FortuneRabbit";
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
            
            if (!IsNullOrEmpty(jsonParams["cpf"]))
            {
                string strLw = jsonParams["cpf"].ToString();
                Dictionary<string, FortuneRabbitCPF> lineWins = JsonConvert.DeserializeObject<Dictionary<string, FortuneRabbitCPF>>(strLw);
                foreach (KeyValuePair<string, FortuneRabbitCPF> pair in lineWins)
                {
                    pair.Value.bv = convertWinByBet(pair.Value.bv, currentBet);
                    pair.Value.m  = convertWinByBet(pair.Value.m,  currentBet);
                }
                jsonParams["cpf"] = JObject.FromObject(lineWins);
            }
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["cptw"]))
                jsonParams["cptw"] = convertWinByBet((double)jsonParams["cptw"], currentBet);

        }
        class FortuneRabbitCPF
        {
            public double p     { get; set; }
            public double bv    { get; set; }
            public double m     { get; set; }
        }
    }

}
