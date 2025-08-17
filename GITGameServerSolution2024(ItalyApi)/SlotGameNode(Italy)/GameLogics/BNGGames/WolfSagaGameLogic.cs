using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SlotGamesNode.GameLogics
{
    class WolfSagaGameLogic : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "wolf_saga";
            }
        }
        protected override string InitResultString
        {
            get
            {
                return "{\"achievements\":null,\"actions\":[\"spin\"],\"bonus\":null,\"current\":\"spins\",\"freespins\":null,\"last_action\":\"init\",\"last_args\":{},\"last_win\":null,\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":80,\"board\":[[11,11,11],[2,2,2],[8,1,6],[8,3,4],[5,1,10]],\"lines\":25,\"moons\":[{\"position\":0,\"reel\":0,\"value\":8000},{\"position\":1,\"reel\":0,\"value\":200000},{\"position\":2,\"reel\":0,\"value\":30000}],\"moons_count\":3,\"reelset_number\":1,\"round_bet\":2000,\"round_win\":0,\"total_win\":0,\"total_win_state\":0,\"winlines\":[],\"winscatters\":[]},\"version\":1}";
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 25;
            }
        }

        protected override string SettingString
        {
            get
            {
                return "{\"add_freespins_rounds_granted\":3,\"bet_factor\":[25],\"bets\":[4,8,10,16,20,30,40,60,80,100,160,200,300,400,600,800,1000,1600,2000],\"big_win\":[20,30,50],\"coefficient1\":[0.06,0.07,0.08],\"coefficient2\":[2.1,1.9,1.7,1.4,1.25,1.05,0.9,0.65,0.25,0.02],\"coefficient_reducing_nominals\":[0.2,0.33,0.79,1,1,1,1,1,1,1,1,1,1,1,1],\"coefficient_reducing_probability\":0.2,\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"freespins_rounds_granted\":5,\"grand_jp_value\":1000,\"init_board_freespins\":[[7,6,10],[3,3,3],[3,3,3],[3,3,3],[6,4,7]],\"init_moons\":[{\"position\":0,\"reel\":0,\"value\":4},{\"position\":1,\"reel\":0,\"value\":100},{\"position\":2,\"reel\":0,\"value\":15}],\"lines\":[25],\"moon_probability\":[320,260,200,120,60,40,30,20,15,12,8,5,4,2],\"moon_probability_giant\":[220,200,180,160,130,100,50,40,30,20,10,5],\"moon_values\":[1,2,3,4,5,6,7,8,9,10,15,20,30,100],\"moon_values_giant\":[1,2,3,4,5,6,7,8,9,10,15,20],\"new_blast_probability_limit\":{\"5\":15,\"6\":15,\"7\":16,\"8\":17,\"9\":18,\"10\":20,\"11\":21,\"12\":24,\"13\":25,\"14\":27,\"15\":27},\"new_moon_probability_limit\":1000,\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,0,0,0,1],[1,2,2,2,1],[0,0,1,2,2],[2,2,1,0,0],[1,2,1,0,1],[1,0,1,2,1],[0,1,1,1,0],[2,1,1,1,2],[0,1,0,1,0],[2,1,2,1,2],[1,1,0,1,1],[1,1,2,1,1],[0,0,2,0,0],[2,2,0,2,2],[0,2,2,2,0],[2,0,0,0,2],[1,2,0,2,1],[1,0,2,0,1],[0,2,0,2,0],[2,0,2,0,2]],\"paytable\":{\"1\":[{\"freespins\":5,\"multiplier\":1,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":3,\"multiplier\":3,\"occurrences\":9,\"trigger\":\"freespins\",\"type\":\"tb\"}],\"2\":[{\"multiplier\":25,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":500,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":25,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":500,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":400,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":15,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":300,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"11\":[{\"multiplier\":0,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":0,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":0,\"occurrences\":5,\"type\":\"lb\"}],\"12\":[{\"multiplier\":0,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":0,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":0,\"occurrences\":5,\"type\":\"lb\"}]},\"reelsamples\":{\"freespins\":[[2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11],[2,3,4,5,6,7,8,9,10]],\"spins_1\":[[1,2,3,4,5,6,7,8,9,10,11],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_2\":[[1,2,3,4,5,6,7,8,9,10,11],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_3\":[[1,2,3,4,5,6,7,8,9,10,11],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_4\":[[1,2,3,4,5,6,7,8,9,10,11],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_5\":[[1,2,3,4,5,6,7,8,9,10,11],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_6\":[[1,2,3,4,5,6,7,8,9,10,11],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11]]},\"respins_rounds_granted\":3,\"rows\":3,\"symbols\":[{\"id\":1,\"name\":\"w_scat\",\"type\":\"scatter\"},{\"id\":2,\"name\":\"wild\",\"type\":\"wild\"},{\"id\":3,\"name\":\"moose\",\"type\":\"line\"},{\"id\":4,\"name\":\"lynx\",\"type\":\"line\"},{\"id\":5,\"name\":\"owl\",\"type\":\"line\"},{\"id\":6,\"name\":\"rabbit\",\"type\":\"line\"},{\"id\":7,\"name\":\"A\",\"type\":\"line\"},{\"id\":8,\"name\":\"K\",\"type\":\"line\"},{\"id\":9,\"name\":\"Q\",\"type\":\"line\"},{\"id\":10,\"name\":\"J\",\"type\":\"line\"},{\"id\":11,\"name\":\"moon\",\"type\":\"line\"},{\"id\":12,\"name\":\"boost\",\"type\":\"line\"}],\"symbols_line\":[3,4,5,6,7,8,9,10,11,12],\"symbols_scat\":[],\"symbols_scatter\":[1],\"symbols_wild\":[2],\"transitions\":[{\"act\":{\"args\":[],\"bet\":false,\"cheat\":false,\"name\":\"init\",\"win\":false},\"dst\":\"spins\",\"src\":\"none\"},{\"act\":{\"args\":[\"bet_per_line\",\"lines\"],\"bet\":true,\"cheat\":true,\"name\":\"spin\",\"win\":true},\"dst\":\"spins\",\"src\":\"spins\"},{\"act\":{\"args\":[],\"bet\":false,\"cheat\":false,\"name\":\"bonus_init\",\"win\":false},\"dst\":\"bonus\",\"src\":\"spins\"},{\"act\":{\"args\":[],\"bet\":false,\"cheat\":true,\"name\":\"respin\",\"win\":true},\"dst\":\"bonus\",\"src\":\"bonus\"},{\"act\":{\"args\":[],\"bet\":false,\"cheat\":false,\"name\":\"bonus_spins_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"bonus\"},{\"act\":{\"args\":[],\"bet\":false,\"cheat\":false,\"name\":\"freespin_init\",\"win\":false},\"dst\":\"freespins\",\"src\":\"spins\"},{\"act\":{\"args\":[],\"bet\":false,\"cheat\":true,\"name\":\"freespin\",\"win\":true},\"dst\":\"freespins\",\"src\":\"freespins\"},{\"act\":{\"args\":[],\"bet\":false,\"cheat\":false,\"name\":\"freespin_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"freespins\"},{\"act\":{\"args\":[],\"bet\":false,\"cheat\":false,\"name\":\"bonus_init\",\"win\":false},\"dst\":\"bonus\",\"src\":\"freespins\"},{\"act\":{\"args\":[],\"bet\":false,\"cheat\":false,\"name\":\"bonus_freespins_stop\",\"win\":false},\"dst\":\"freespins\",\"src\":\"bonus\"}],\"version\":\"a\"}";
            }
        }
        #endregion
        public WolfSagaGameLogic()
        {
            _gameID = GAMEID.WolfSaga;
            GameName = "WolfSaga";
        }

        protected override string convertInitResultString(int currency)
        {
            string initResultString = base.convertInitResultString(currency);

            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(initResultString);

            if (!object.ReferenceEquals(resultContext["spins"], null) && !object.ReferenceEquals(resultContext["spins"]["moons"], null))
            {
                for (int i = 0; i < resultContext["spins"]["moons"].Count; i++)
                {
                    int value = 0;
                    bool inNumeric = int.TryParse(resultContext["spins"]["moons"][i]["value"], out value);
                    if (inNumeric)
                        resultContext["spins"]["moons"][i]["value"] = value * new Currencies()._currencyInfo[currency].Rate;
                }
            }

            return JsonConvert.SerializeObject(resultContext);

        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet((object)resultContext, currentBet);
            string strCurrent = resultContext["current"];
            dynamic spinContext = resultContext[strCurrent];
            if (!object.ReferenceEquals(spinContext["moons"], null))
            {
                var moonsArray = spinContext["moons"] as JArray;
                for (int i = 0; i < moonsArray.Count; i++)
                {
                    if (moonsArray[i] != null)
                        moonsArray[i]["value"] = convertWinByBet((double)moonsArray[i]["value"], currentBet);
                }
            }
            
            if (!object.ReferenceEquals(spinContext["total_win_state"], null))
                spinContext["total_win_state"] = convertWinByBet((double)spinContext["total_win_state"], currentBet);


            if (strCurrent != "spins" && !object.ReferenceEquals(resultContext["spins"], null))
            {
                spinContext = resultContext["spins"];
                if (!object.ReferenceEquals(spinContext["bs"], null))
                {
                    var moonsArray = spinContext["moons"] as JArray;
                    for (int i = 0; i < moonsArray.Count; i++)
                    {
                        if (moonsArray[i] != null)
                            moonsArray[i]["value"] = convertWinByBet((double)moonsArray[i]["value"], currentBet);
                    }
                }
            }

        }

        protected override BaseBNGSlotSpinResult calculateResult(string strGlobalUserID, int currency, BaseBNGSlotBetInfo betInfo, string strSpinResponse, bool isFirst, BNGActionTypes action)
        {
            try
            {
                BaseBNGSlotSpinResult spinResult = new BaseBNGSlotSpinResult();
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(resultContext, betInfo.TotalBet);
                convertBetsByBet(resultContext, betInfo.BetPerLine, betInfo.TotalBet);

                string strCurrent = resultContext["current"];
                dynamic spinContext = resultContext[strCurrent];

                string strNextAction = (string)resultContext["actions"][0];
                spinResult.NextAction = convertStringToAction(strNextAction);

                double roundWin = 0.0;
                if (!object.ReferenceEquals(spinContext["round_win"], null) && spinContext["round_win"] != null)
                    roundWin = (double)spinContext["round_win"];

                double totalWin = (double)spinContext["total_win"];
                spinResult.TotalWin      = roundWin / 100.0;
                spinResult.LastWin       = 0.0;
                spinResult.TransactionID = Guid.NewGuid().ToString().Replace("-", "") + "1";

                if (action == BNGActionTypes.BONUSSTOP)
                    spinResult.TotalWin = 0.0;

                if (action == BNGActionTypes.SPIN || !_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    spinResult.RoundID = ((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString() + "001";
                else
                    spinResult.RoundID = _dicUserResultInfos[strGlobalUserID].RoundID;

                if (action == BNGActionTypes.FREESPINSTOP && totalWin > 0.0)
                    spinResult.LastWin = totalWin;
                else if (action == BNGActionTypes.BONUSSTOP && totalWin > 0.0)
                    spinResult.LastWin = totalWin;
                else if (roundWin > 0.0)
                    spinResult.LastWin = roundWin;
                else if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    spinResult.LastWin = _dicUserResultInfos[strGlobalUserID].LastWin;

                resultContext["last_win"] = spinResult.LastWin;
                resultContext["last_action"] = convertActionToString(action);
                if (action == BNGActionTypes.SPIN)
                {
                    resultContext["last_args"] = new JObject();
                    resultContext["last_args"]["bet_per_line"] = betInfo.BetPerLine;
                    resultContext["last_args"]["lines"] = betInfo.LineCount;
                }
                else
                {
                    resultContext["last_args"] = new JObject();
                }
                resultContext["math_version"] = "a";
                resultContext["version"] = 1;
                if (spinResult.NextAction == BNGActionTypes.SPIN)
                    resultContext["round_finished"] = true;
                else
                    resultContext["round_finished"] = false;

                spinResult.ResultString = JsonConvert.SerializeObject(resultContext);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
    }
}
