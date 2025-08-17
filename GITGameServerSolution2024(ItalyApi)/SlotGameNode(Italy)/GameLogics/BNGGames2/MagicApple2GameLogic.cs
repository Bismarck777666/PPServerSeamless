using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MagicApple2GameLogic : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "magic_apple_2";
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
                return "{\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bac\":[2,0],\"bet_per_line\":5000,\"board\":[[2,9,9,9],[3,10,10,10],[9,9,9,9],[9,9,9,1],[4,12,12,12]],\"bs_v\":[[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,\"minor\",100000,500000]],\"bs_values\":[[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,50,1,5]],\"lines\":20,\"round_bet\":100000,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string SettingString
        {
            get
            {
                return "{\"bac_max_level\":9,\"bet_factor\":[20],\"bets\":[1000,2000,2500,3750,5000,6250,7500,10000,12500,15000,20000,25000,37500,40000,50000,62500,75000,100000,125000,150000,200000],\"big_win\":[30,50,70],\"bonus_symbols\":[1,2,3,4,5,6,7,8,9,10,12,14,16,\"mini\",\"minor\",\"major\"],\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"fs_retrigger\":5,\"jackpots\":{\"grand\":5000,\"major\":150,\"mini\":20,\"minor\":50},\"lines\":[20],\"paylines\":[[0,0,0,0,0],[3,3,3,3,3],[1,1,1,1,1],[2,2,2,2,2],[0,1,2,1,0],[3,2,1,2,3],[2,1,0,1,2],[1,2,3,2,1],[0,1,0,1,0],[3,2,3,2,3],[1,0,1,0,1],[2,3,2,3,2],[1,2,1,2,1],[2,1,2,1,2],[0,1,1,1,0],[3,2,2,2,3],[1,0,0,0,1],[2,3,3,3,2],[1,2,2,2,1],[2,1,1,1,2]],\"paytable\":{\"1\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":15,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":15,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":15,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":15,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":120,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":140,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":160,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"multiplier\":25,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":5,\"type\":\"lb\"}],\"11\":[{\"freespins\":8,\"multiplier\":2,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"freespins\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"spins\":[[1,2,3,4,5,6,7,8,9,10,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,12]]},\"respins_granted\":3,\"rows\":4,\"symbols\":[{\"id\":1,\"name\":\"el_01\",\"type\":\"line\"},{\"id\":2,\"name\":\"el_02\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_03\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_04\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_05\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_06\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_07\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_08\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_09\",\"type\":\"line\"},{\"id\":10,\"name\":\"el_wild\",\"type\":\"wild\"},{\"id\":11,\"name\":\"el_scatter\",\"type\":\"scat\"},{\"id\":12,\"name\":\"el_bonus\",\"type\":\"scat\"},{\"id\":13,\"name\":\"el_mystery\",\"type\":\"scat\"}],\"symbols_line\":[1,2,3,4,5,6,7,8,9],\"symbols_scat\":[11,12,13],\"symbols_wild\":[10],\"version\":\"a\"}";
            }
        }
        
        #endregion
        public MagicApple2GameLogic()
        {
            _gameID = GAMEID.MagicApple2;
            GameName = "MagicApple2";
        }
    }
}
