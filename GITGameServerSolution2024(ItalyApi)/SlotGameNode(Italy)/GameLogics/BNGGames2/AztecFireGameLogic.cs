using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class AztecFireGameLogic : BaseBNGAchieveGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "aztec_fire";
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
                return "{\"achievements\":{\"level\":0,\"level_percent\":0,\"number\":0,\"total_percent\":0},\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":5000,\"board\":[[11,11,11,11],[9,9,9,9],[1,10,2,2],[8,8,8,5],[6,7,7,7]],\"bs\":[{\"position\":0,\"reel\":0,\"value\":400000},{\"position\":1,\"reel\":0,\"value\":10000000},{\"position\":2,\"reel\":0,\"value\":600000},{\"position\":3,\"reel\":0,\"value\":1200000}],\"bs_count\":4,\"lines\":20,\"reelset_number\":0,\"round_bet\":100000,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string SettingString
        {
            get
            {
                return "{\"Bet_Multiplier\":20,\"bet_factor\":[20],\"bets\":[1000,2000,2500,3750,5000,6250,7500,10000,10500,12500,15000,20000,25000,30000,37500,40000,50000,62500,75000,85000,100000],\"big_win\":[20,30,50],\"bs_values\":[20,30,40,50,60,70,80,100,120,160,200,240,400,1000,2000,20000],\"bs_values_reels\":{\"1\":240,\"2\":200,\"3\":100,\"4\":80,\"5\":60,\"6\":45,\"8\":26,\"10\":20,\"12\":18,\"20\":18,\"50\":10,\"100\":4,\"1000\":1,\"1.5\":220,\"2.5\":120},\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"init_board_freespins\":[[11,11,11,11],[9,9,9,9],[8,8,6,6],[6,7,7,7],[10,5,5,5]],\"init_bs\":[{\"position\":0,\"reel\":0,\"value\":80},{\"position\":1,\"reel\":0,\"value\":2000},{\"position\":2,\"reel\":0,\"value\":120},{\"position\":3,\"reel\":0,\"value\":240}],\"jackpot_values\":{\"grand\":20000,\"major\":2000,\"mini\":400,\"minor\":1000,\"royal\":200000},\"jackpots\":{\"20\":\"mini\",\"50\":\"minor\",\"100\":\"major\",\"1000\":\"grand\",\"10000\":\"royal\"},\"key_thresholds\":[10,15,20,25],\"lines\":[20],\"min_num_of_bs_for_bonus\":6,\"num_thresholds\":4,\"paylines\":[[0,0,0,0,0],[1,1,1,1,1],[2,2,2,2,2],[3,3,3,3,3],[0,1,2,1,0],[1,2,3,2,1],[2,1,0,1,2],[3,2,1,2,3],[0,1,0,1,0],[1,2,1,2,1],[2,3,2,3,2],[1,0,1,0,1],[2,1,2,1,2],[3,2,3,2,3],[0,1,1,1,0],[1,2,2,2,1],[2,3,3,3,2],[1,0,0,0,1],[2,1,1,1,2],[3,2,2,2,3]],\"paytable\":{\"1\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":5,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":5,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":5,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":5,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":6,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":7,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":35,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":8,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":120,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":400,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"freespins\":8,\"multiplier\":2,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reducing_fall\":0.9,\"reducing_jps_by_pos\":{\"grand\":[0,0.45,0.95,0.95,0.95,0.95,0.95,0.95],\"major\":[0,0,0.45,0.9,0.9,0.9,0.9,0.9],\"mini\":[0.8,0.4,0,0,0.8,0.8,0.8,0.8],\"minor\":[0.45,0,0,0.45,0.85,0.85,0.85,0.85]},\"reelsamples\":{\"freespins_0\":[[5,6,7,8,9,10,11],[5,6,7,8,9,11],[5,6,7,8,9,10,11],[5,6,7,8,9,11],[5,6,7,8,9,10,11]],\"freespins_1\":[[5,6,7,8,9,10,11],[5,6,7,8,9,11],[5,6,7,8,9,10,11],[5,6,7,8,9,11],[5,6,7,8,9,10,11]],\"spins_0\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_1\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_2\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_3\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_4\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11]]},\"rows\":4,\"rows_bonus\":8,\"symbols\":[{\"id\":1,\"name\":\"J\",\"type\":\"line\"},{\"id\":2,\"name\":\"Q\",\"type\":\"line\"},{\"id\":3,\"name\":\"K\",\"type\":\"line\"},{\"id\":4,\"name\":\"A\",\"type\":\"line\"},{\"id\":5,\"name\":\"frog\",\"type\":\"line\"},{\"id\":6,\"name\":\"toucan\",\"type\":\"line\"},{\"id\":7,\"name\":\"jaguar\",\"type\":\"line\"},{\"id\":8,\"name\":\"warrior\",\"type\":\"line\"},{\"id\":9,\"name\":\"wild\",\"type\":\"wild\"},{\"id\":10,\"name\":\"t_scat\",\"type\":\"scatter\"},{\"id\":11,\"name\":\"bonus\",\"type\":\"bonus\"},{\"id\":12,\"name\":\"hidden\",\"type\":\"hide\"}],\"symbols_bonus\":[11],\"symbols_hide\":[12],\"symbols_line\":[1,2,3,4,5,6,7,8],\"symbols_scatter\":[10],\"symbols_wild\":[9],\"version\":\"a\"}";
            }
        }
        #endregion
        public AztecFireGameLogic()
        {
            _gameID = GAMEID.AztecFire;
            GameName = "AztecFire";
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
