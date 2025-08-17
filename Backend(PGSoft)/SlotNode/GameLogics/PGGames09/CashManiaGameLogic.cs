using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class CashManiaGameLogic : BasePGSlotGame
    {
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"twbm\":0.0,\"fs\":null,\"imw\":false,\"rv\":[0.10,10.00,0.50,1.0,0.0,100.0,10.00,1.00,5.00],\"orl\":null,\"orv\":null,\"rsrl\":null,\"rsrv\":null,\"nfp\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[1,5,2,12,0,11,5,3,4],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":5.60,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"twbm\":0.0,\"fs\":null,\"imw\":false,\"rv\":[20.0,2000.0,100.0,1.0,0.0,5.0,2000.0,200.0,1000.0],\"orl\":null,\"orv\":null,\"rsrl\":null,\"rsrv\":null,\"nfp\":null,\"gwt\":-1,\"fb\":null,\"ctw\":2000.0,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"1\":2000.0},\"hashr\":null,\"ml\":10,\"cs\":2.0,\"rl\":[1,5,2,1,0,4,5,3,4],\"sid\":\"1770029370468074497\",\"psid\":\"1770029370468074497\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":5.60,\"blab\":5.60,\"bl\":5.60,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public CashManiaGameLogic()
        {
            _gameID = GAMEID.CashMania;
            GameName = "CashMania";
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

            if (!IsNullOrEmpty(jsonParams["rv"]))
            {
                var rvArray = jsonParams["rv"] as JArray;
                for(int i = 0; i < 3; i++)
                    rvArray[i] = Math.Round(convertWinByBet(rvArray[i].ToObject<double>(), currentBet), 2);

                for (int i = 6; i < 9; i++)
                    rvArray[i] = Math.Round(convertWinByBet(rvArray[i].ToObject<double>(), currentBet), 2);

            }
            if (!IsNullOrEmpty(jsonParams["orv"]))
            {
                var rvArray = jsonParams["orv"] as JArray;
                for (int i = 0; i < 3; i++)
                    rvArray[i] = Math.Round(convertWinByBet(rvArray[i].ToObject<double>(), currentBet), 2);

                for (int i = 6; i < 9; i++)
                    rvArray[i] = Math.Round(convertWinByBet(rvArray[i].ToObject<double>(), currentBet), 2);

            }
            if (!IsNullOrEmpty(jsonParams["rsrv"]))
            {
                var rvArray = jsonParams["rsrv"] as JArray;
                for (int i = 0; i < 3; i++)
                    rvArray[i] = Math.Round(convertWinByBet(rvArray[i].ToObject<double>(), currentBet), 2);

                for (int i = 6; i < 9; i++)
                    rvArray[i] = Math.Round(convertWinByBet(rvArray[i].ToObject<double>(), currentBet), 2);

            }
            if (!IsNullOrEmpty(jsonParams["rwsp"]))
            {
                var dictionary = jsonParams["rwsp"].ToObject<Dictionary<string, double>>();
                foreach(KeyValuePair<string, double> kvp in dictionary)
                {
                    jsonParams["rwsp"][kvp.Key] = convertWinByBet(kvp.Value, currentBet);
                }
            }
        }
    }
}
