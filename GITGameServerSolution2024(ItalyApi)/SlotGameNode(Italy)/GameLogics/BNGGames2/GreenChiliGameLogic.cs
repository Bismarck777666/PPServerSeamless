using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class GreenChiliGameLogic : BaseBNGHillSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "green_chilli";
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 20;
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string InitResultString
        {
            get
            {
                return "{\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":5000,\"board\":[[11,11,11],[9,9,2],[9,1,6],[9,9,4],[11,11,8]],\"bs_v\":[[500000,200000,800000],[0,0,0],[0,0,0],[0,0,0],[\"minor\",100000,0]],\"bs_values\":[[5,2,8],[0,0,0],[0,0,0],[0,0,0],[40,1,0]],\"hill\":[0,0],\"lines\":20,\"round_bet\":100000,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[20],\"bets\":[1000,2000,2500,3750,5000,6250,7500,10000,12500,15000,20000,25000,30000,37500,50000,55000,62500,75000,100000,125000,150000],\"big_win\":[20,50,70],\"bonus_symbols\":[0.5,1,2,3,4,5,6,7,8,9,10,15,\"mini\",\"minor\",\"major\"],\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"fs_retrigger\":8,\"hill_max_level\":9,\"jackpots\":{\"grand\":2000,\"major\":100,\"mini\":20,\"minor\":40},\"lines\":[20],\"mult_bagvalue\":[2,3,4,5,10],\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,0,0,0,1],[1,2,2,2,1],[0,0,1,2,2],[2,2,1,0,0],[1,2,1,0,1],[1,0,1,2,1],[0,1,1,1,0],[2,1,1,1,2],[0,1,0,1,0],[2,1,2,1,2],[1,1,0,1,1],[1,1,2,1,1],[0,0,2,0,0],[2,2,0,2,2],[0,2,2,2,0]],\"paytable\":{\"1\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":120,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"freespins\":8,\"multiplier\":0,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"freespins\":[[1,2,3,4,5,6,7,8,9],[1,2,3,4,5,6,7,8,9],[1,2,3,4,5,6,7,8,9],[1,2,3,4,5,6,7,8,9],[1,2,3,4,5,6,7,8,9]],\"spins\":[[1,2,3,4,5,6,7,8,11,9],[1,2,3,4,5,6,7,8,11,9,10],[1,2,3,4,5,6,7,8,11,9,10],[1,2,3,4,5,6,7,8,11,9,10],[1,2,3,4,5,6,7,8,11,9]]},\"respins_granted\":3,\"rows\":3,\"symbols\":[{\"id\":1,\"name\":\"el_01\",\"type\":\"line\"},{\"id\":2,\"name\":\"el_02\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_03\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_04\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_05\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_06\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_07\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_08\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_wild\",\"type\":\"wild\"},{\"id\":10,\"name\":\"el_scatter\",\"type\":\"scat\"},{\"id\":11,\"name\":\"el_bonus\",\"type\":\"scat\"},{\"id\":12,\"name\":\"el_bonus_multiplier\",\"type\":\"scat\"}],\"symbols_line\":[1,2,3,4,5,6,7,8],\"symbols_scat\":[10,11,12],\"symbols_wild\":[9],\"version\":\"a\"}";
            }
        }
        protected override int[] HillValues
        {
            get
            {
                return new int[] { 3, 7, 10, 10, 10, 10, 10, 10, 10, 10 };
            }
        }
        #endregion
        public GreenChiliGameLogic()
        {
            _gameID = GAMEID.GreenChilli;
            GameName = "GreenChilli";
        }
        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet((object)resultContext, currentBet);
            string strCurrent = resultContext["current"];
            dynamic spinContext = resultContext[strCurrent];
            if (!object.ReferenceEquals(spinContext["bs"], null))
            {
                var bsArray = spinContext["bs"] as JArray;
                for (int i = 0; i < bsArray.Count; i++)
                {
                    if (bsArray[i] != null)
                    {
                        double outValue = 0.0;
                        if (double.TryParse(bsArray[i]["value"].ToString(), out outValue))
                            bsArray[i]["value"] = convertWinByBet((double)bsArray[i]["value"], currentBet);
                    }
                }
            }
            if (!object.ReferenceEquals(spinContext["new_bs"], null))
            {
                var bsArray = spinContext["new_bs"] as JArray;
                for (int i = 0; i < bsArray.Count; i++)
                {
                    if (bsArray[i] != null)
                    {
                        double outValue = 0.0;
                        if (double.TryParse(bsArray[i]["value"].ToString(), out outValue))
                            bsArray[i]["value"] = convertWinByBet((double)bsArray[i]["value"], currentBet);
                    }
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
                        {
                            double outValue = 0.0;
                            if (double.TryParse(bsArray[i]["value"].ToString(), out outValue))
                                bsArray[i]["value"] = convertWinByBet((double)bsArray[i]["value"], currentBet);
                        }
                    }
                }
            }

        }


    }
}
