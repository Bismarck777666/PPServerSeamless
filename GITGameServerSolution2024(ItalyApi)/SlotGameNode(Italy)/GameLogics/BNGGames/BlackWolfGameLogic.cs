using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
    class BlackWolfGameLogic : BaseBNGAchieveGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "black_wolf";
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 25;
            }
        }
        protected override string InitResultString
        {
            get
            {
                return "{\"achievements\":{\"level\":0,\"level_percent\":0,\"number\":0,\"total_percent\":0},\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":25,\"board\":[[11,11,11,11],[9,9,9,9],[8,1,6,3],[8,3,4,9],[5,1,10,2]],\"bs\":[{\"position\":0,\"reel\":0,\"value\":31250},{\"position\":1,\"reel\":0,\"value\":4375},{\"position\":2,\"reel\":0,\"value\":9375},{\"position\":3,\"reel\":0,\"value\":1875}],\"bs_count\":4,\"is_boost\":false,\"lines\":25,\"reelset_number\":0,\"round_bet\":625,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[25],\"bets\":[2,4,5,8,10,20,25,40,50,100,150,200,300,500,600,1000],\"big_win\":[20,30,50],\"bs_values_reels\":{\"1\":240,\"2\":220,\"3\":200,\"4\":120,\"5\":100,\"6\":80,\"7\":60,\"8\":45,\"9\":26,\"10\":20,\"15\":18,\"20\":14,\"50\":6,\"100\":2},\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"init_bs\":[{\"position\":0,\"reel\":0,\"value\":50},{\"position\":1,\"reel\":0,\"value\":7},{\"position\":2,\"reel\":0,\"value\":15},{\"position\":3,\"reel\":0,\"value\":3}],\"jackpots\":{\"20\":\"mini\",\"50\":\"minor\",\"100\":\"major\",\"1000\":\"grand\"},\"lines\":[25],\"paylines\":[[0,0,0,0,0],[1,1,1,1,1],[2,2,2,2,2],[3,3,3,3,3],[0,1,2,1,0],[1,2,3,2,1],[2,1,0,1,2],[3,2,1,2,3],[0,1,0,1,0],[1,2,1,2,1],[2,3,2,3,2],[1,0,1,0,1],[2,1,2,1,2],[3,2,3,2,3],[0,1,1,1,0],[1,2,2,2,1],[2,3,3,3,2],[1,0,0,0,1],[2,1,1,1,2],[3,2,2,2,3],[0,0,1,0,0],[1,1,2,1,1],[2,2,3,2,2],[1,1,0,1,1],[2,2,1,2,2]],\"paytable\":{\"1\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":15,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":75,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":300,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":25,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":500,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":25,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":500,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"freespins\":8,\"multiplier\":2,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"freespins_0\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"freespins_1\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_0\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_1\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_2\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_3\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_4\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]]},\"rows\":4,\"symbols\":[{\"id\":1,\"name\":\"J\",\"type\":\"line\"},{\"id\":2,\"name\":\"Q\",\"type\":\"line\"},{\"id\":3,\"name\":\"K\",\"type\":\"line\"},{\"id\":4,\"name\":\"A\",\"type\":\"line\"},{\"id\":5,\"name\":\"owl\",\"type\":\"line\"},{\"id\":6,\"name\":\"eagle\",\"type\":\"line\"},{\"id\":7,\"name\":\"lynx\",\"type\":\"line\"},{\"id\":8,\"name\":\"moose\",\"type\":\"line\"},{\"id\":9,\"name\":\"wild\",\"type\":\"wild\"},{\"id\":10,\"name\":\"w_scat\",\"type\":\"scatter\"},{\"id\":11,\"name\":\"bonus\",\"type\":\"scat\"},{\"id\":12,\"name\":\"boost\",\"type\":\"scat\"},{\"id\":13,\"name\":\"hidden\",\"type\":\"hide\"}],\"symbols_hide\":[13],\"symbols_line\":[1,2,3,4,5,6,7,8],\"symbols_scatter\":[10],\"symbols_wild\":[9],\"version\":\"a\"}";
            }
        }
        #endregion
        public BlackWolfGameLogic()
        {
            _gameID = GAMEID.BlackWolf;
            GameName = "BlackWolf";
        }

        protected override string convertInitResultString(int currency)
        {
            string initResultString = base.convertInitResultString(currency);

            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(initResultString);
            
            if (!object.ReferenceEquals(resultContext["spins"], null) && !object.ReferenceEquals(resultContext["spins"]["bs"], null))
            {
                foreach (var item in resultContext["spins"]["bs"])
                {
                    int value       = 0;
                    bool inNumeric  = int.TryParse(Convert.ToString(item["value"]), out value);
                    if (inNumeric)
                        item["value"] = value * new Currencies()._currencyInfo[currency].Rate;
                }
            }

            return JsonConvert.SerializeObject(resultContext);
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet((object) resultContext, currentBet);
            string strCurrent   = resultContext["current"];
            dynamic spinContext = resultContext[strCurrent];
            if (!object.ReferenceEquals(spinContext["bs"], null))
            {
                var bsArray = spinContext["bs"] as JArray;
                for(int i = 0; i < bsArray.Count; i++)
                {
                    if(bsArray[i] != null)
                        bsArray[i]["value"] = convertWinByBet((double)bsArray[i]["value"], currentBet);
                }
            }
            if (!object.ReferenceEquals(spinContext["new_bs"], null))
            {
                var bsArray = spinContext["new_bs"] as JArray;
                for (int i = 0; i < bsArray.Count; i++)
                {
                    if (bsArray[i] != null)
                        bsArray[i]["value"] = convertWinByBet((double)bsArray[i]["value"], currentBet);
                }
            }
            if (!object.ReferenceEquals(spinContext["total_win_state"], null))
                spinContext["total_win_state"] = convertWinByBet((double)spinContext["total_win_state"], currentBet);


            if (strCurrent != "spins" && !object.ReferenceEquals(resultContext["spins"], null))
            {
                spinContext = resultContext["spins"];
                if (!object.ReferenceEquals(spinContext["bs"], null))
                {
                    var bsArray = spinContext["bs"] as JArray;
                    for (int i = 0; i < bsArray.Count; i++)
                    {
                        if (bsArray[i] != null)
                            bsArray[i]["value"] = convertWinByBet((double)bsArray[i]["value"], currentBet);
                    }
                }
            }
            
        }
    }
}
