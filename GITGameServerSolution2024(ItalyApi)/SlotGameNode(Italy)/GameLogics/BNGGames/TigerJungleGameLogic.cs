using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class TigerJungleGameLogic : BaseBNGHillSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "tiger_jungle";
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
                return "{\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":40,\"board\":[[11,2,7],[3,10,3],[9,9,9],[1,9,9],[11,11,11]],\"bs_v\":[[\"mini\",0,0],[0,0,0],[0,0,0],[0,0,0],[3000,\"major\",7000]],\"bs_values\":[[20,0,0],[0,0,0],[0,0,0],[0,0,0],[3,150,7]],\"hill\":[0,0],\"lines\":25,\"round_bet\":1000,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[25],\"bets\":[4,8,10,16,20,40,60,100,120,200,300,500,800],\"big_win\":[30,50,70],\"bonus_symbols\":[1,2,3,4,5,6,7,8,9,10,\"mini\",\"minor\",\"major\"],\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"fs_retrigger\":5,\"hill_spins_count\":{\"0\":3,\"1\":7,\"2\":15,\"3\":25,\"4\":25},\"jackpots\":{\"grand\":5000,\"major\":150,\"mini\":20,\"minor\":50},\"lines\":[25],\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,0,0,0,1],[1,2,2,2,1],[0,0,1,2,2],[2,2,1,0,0],[1,2,1,0,1],[1,0,1,2,1],[0,1,1,1,0],[2,1,1,1,2],[0,1,0,1,0],[2,1,2,1,2],[1,1,0,1,1],[1,1,2,1,1],[0,0,2,0,0],[2,2,0,2,2],[0,2,2,2,0],[2,0,0,0,2],[1,2,0,2,1],[1,0,2,0,1],[0,2,0,2,0],[2,0,2,0,2]],\"paytable\":{\"1\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":15,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":300,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"freespins\":8,\"multiplier\":1,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":8,\"multiplier\":10,\"occurrences\":4,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":8,\"multiplier\":100,\"occurrences\":5,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"freespins\":[[1,2,3,4,5,6,7,8,9],[1,2,3,4,5,6,7,8,9],[1,2,3,4,5,6,7,8,9],[1,2,3,4,5,6,7,8,9],[1,2,3,4,5,6,7,8,9]],\"spins\":[[1,2,3,4,5,6,7,8,11,10,9],[1,2,3,4,5,6,7,8,11,10,9],[1,2,3,4,5,6,7,8,11,10,9],[1,2,3,4,5,6,7,8,11,10,9],[1,2,3,4,5,6,7,8,11,10,9]]},\"respins_granted\":3,\"rows\":3,\"small_win\":5,\"spec_sym_bagvalue\":{\"1\":[9],\"2\":[9,9],\"3\":[9,9,9],\"4\":[11],\"5\":[11,11],\"6\":[11,11,11],\"7\":[10]},\"symbols\":[{\"id\":1,\"name\":\"el_01\",\"type\":\"line\"},{\"id\":2,\"name\":\"el_02\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_03\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_04\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_05\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_06\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_07\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_08\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_wild\",\"type\":\"wild\"},{\"id\":10,\"name\":\"el_scatter\",\"type\":\"scat\"},{\"id\":11,\"name\":\"el_bonus\",\"type\":\"scat\"},{\"id\":12,\"name\":\"el_collect\",\"type\":\"scat\"}],\"symbols_line\":[1,2,3,4,5,6,7,8],\"symbols_scat\":[10,11,12],\"symbols_wild\":[9],\"transitions\":[{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"init\",\"win\":false},\"dst\":\"spins\",\"src\":\"none\"},{\"act\":{\"args\":[\"bet_per_line\",\"lines\"],\"bet\":true,\"cheat\":true,\"name\":\"spin\",\"win\":true},\"dst\":\"spins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"freespin_init\",\"win\":false},\"dst\":\"freespins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"freespin\",\"win\":true},\"dst\":\"freespins\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"bonus_init\",\"win\":false},\"dst\":\"bonus\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"bonus_init\",\"win\":false},\"dst\":\"bonus\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"respin\",\"win\":true},\"dst\":\"bonus\",\"src\":\"bonus\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"bonus_spins_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"bonus\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"bonus_freespins_stop\",\"win\":false},\"dst\":\"freespins\",\"src\":\"bonus\"}],\"version\":\"a\"}";
            }
        }
        protected override int[] HillValues
        {
            get
            {
                return new int[] { 3, 7, 15, 25, 25, 1 };
            }
        }
        #endregion
        public TigerJungleGameLogic()
        {
            _gameID = GAMEID.TigerJungle;
            GameName = "TigerJungle";
        }
    }
}
