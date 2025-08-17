using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BigHeistGameLogic : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "big_heist";
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 10;
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string InitResultString
        {
            get
            {
                return "{\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":10000,\"board\":[[6,3,11],[8,1,6],[1,11,4],[11,1,7],[10,2,11]],\"bs_v\":[[300000,0,0],[0,0,400000],[0,0,0],[0,0,0],[0,0,0]],\"bs_values\":[[3,0,0],[0,0,4],[0,0,0],[0,0,0],[0,0,0]],\"lines\":10,\"round_bet\":100000,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[10],\"bets\":[1000,2000,2500,4000,5000,7500,10000,12500,15000,20000,25000,30000,40000,50000,75000,100000,125000,150000,200000,250000,300000],\"big_win\":[20,40,60],\"bonus_symbols\":[2,3,4,5,7,9,10,12,15,20,30,50,200],\"collect_multipliers\":[1,2,3,10],\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"fs_retrigger\":10,\"lines\":[10],\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,2,2,2,1],[1,0,0,0,1],[2,2,1,0,0],[0,0,1,2,2],[2,1,1,1,0]],\"paytable\":{\"1\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":500,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":30,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":1000,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":5,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":1500,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"multiplier\":10,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":2000,\"occurrences\":5,\"type\":\"lb\"}],\"11\":[{\"freespins\":10,\"multiplier\":0,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":12,\"multiplier\":0,\"occurrences\":4,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":15,\"multiplier\":0,\"occurrences\":5,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"freespins\":[[1,2,3,4,5,6,7,8,9,10,12,13],[1,2,3,4,5,6,7,8,9,10,12,13],[1,2,3,4,5,6,7,8,9,10,12,13],[1,2,3,4,5,6,7,8,9,10,12,13],[1,2,3,4,5,6,7,8,9,10,12,13]],\"freespins_ag\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"spins\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins_2\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]]},\"rows\":3,\"small_win\":5,\"symbols\":[{\"id\":1,\"name\":\"el_01\",\"type\":\"line\"},{\"id\":2,\"name\":\"el_02\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_03\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_04\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_05\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_06\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_07\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_08\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_09\",\"type\":\"line\"},{\"id\":10,\"name\":\"el_10\",\"type\":\"line\"},{\"id\":11,\"name\":\"el_scatter_wild\",\"type\":\"scat\"},{\"id\":12,\"name\":\"el_collect\",\"type\":\"scat\"},{\"id\":13,\"name\":\"el_super_collect\",\"type\":\"scat\"}],\"symbols_line\":[1,2,3,4,5,6,7,8,9,10],\"symbols_scat\":[11,12,13],\"symbols_wild\":[null],\"version\":\"a\"}";
            }
        }
        #endregion

        public BigHeistGameLogic()
        {
            _gameID = GAMEID.BigHeist;
            GameName = "BigHeist";
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet((object)resultContext, currentBet);
            string strCurrent = resultContext["current"];
            List<string> strContexts = new List<string>();
            strContexts.Add(strCurrent);
            if (strCurrent != "spins")
                strContexts.Add("spins");

            foreach (var strContext in strContexts)
            {
                if (object.ReferenceEquals(resultContext[strContext], null))
                    continue;

                dynamic spinContext = resultContext[strContext];
                if (!object.ReferenceEquals(spinContext["collect_pay"], null) && spinContext["collect_pay"] != null)
                    spinContext["collect_pay"] = convertWinByBet((double)spinContext["collect_pay"], currentBet);

                if (!object.ReferenceEquals(spinContext["fs_win"], null) && spinContext["fs_win"] != null)
                    spinContext["fs_win"] = convertWinByBet((double)spinContext["fs_win"], currentBet);                
            }
        }
    }
}
