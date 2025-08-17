using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class GoldExpressGameLogic : BaseBNGHillSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "gold_express";
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 20;
            }
        }
        protected override string InitResultString
        {
            get
            {
                return "{\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":125,\"board\":[[2,9,9,9],[3,10,10,10],[9,9,9,9],[9,9,9,4],[4,12,12,12]],\"boost_pay\":0,\"bs_v\":[[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,\"mini\",25000,12500]],\"bs_values\":[[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,0,0,0],[0,20,10,5]],\"hill\":[0,0],\"lines\":20,\"round_bet\":2500,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        protected override string SettingString
        {
            get
            {
                return "{\"additional_fs_bagchance\":30,\"additional_fs_dec\":10,\"bet_factor\":[20],\"bets\":[10,20,25,40,50,100,125,200,250,500,750,1000,1500,2500,3000,5000],\"big_win\":[20,50,80],\"bonus_symbols\":[1,2,3,4,5,6,7,8,9,10,12,14,16,\"mini\",\"minor\",\"major\"],\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"fs_retrigger\":8,\"hidden_chance_modifier\":{\"1\":1,\"2\":1,\"3\":1,\"4\":1},\"jackpots\":{\"grand\":2000,\"major\":150,\"mini\":20,\"minor\":50},\"lines\":[20],\"paylines\":[[0,0,0,0,0],[3,3,3,3,3],[1,1,1,1,1],[2,2,2,2,2],[0,1,2,1,0],[3,2,1,2,3],[2,1,0,1,2],[1,2,3,2,1],[0,1,0,1,0],[3,2,3,2,3],[1,0,1,0,1],[2,3,2,3,2],[1,2,1,2,1],[2,1,2,1,2],[0,1,1,1,0],[2,1,1,1,2],[1,0,0,0,1],[2,3,3,3,2],[1,2,2,2,1],[3,2,2,2,3]],\"paytable\":{\"1\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"11\":[{\"freespins\":8,\"multiplier\":1,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":8,\"multiplier\":10,\"occurrences\":4,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":8,\"multiplier\":50,\"occurrences\":5,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"freespins\":[[1,2,3,4,5,6,7,8,9,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12]],\"last_freespins\":[[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11]],\"spins\":[[1,2,3,4,5,6,7,8,9,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12]]},\"respins_granted\":3,\"rows\":4,\"small_win\":2,\"symbols\":[{\"id\":1,\"name\":\"el_01\",\"type\":\"line\"},{\"id\":2,\"name\":\"el_02\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_03\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_04\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_05\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_06\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_07\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_08\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_09\",\"type\":\"line\"},{\"id\":10,\"name\":\"el_wild\",\"type\":\"wild\"},{\"id\":11,\"name\":\"el_scatter\",\"type\":\"scat\"},{\"id\":12,\"name\":\"el_bonus\",\"type\":\"scat\"},{\"id\":13,\"name\":\"el_boost\",\"type\":\"scat\"},{\"id\":14,\"name\":\"empty\",\"type\":\"scat\"},{\"id\":15,\"name\":\"hidden\",\"type\":\"scat\"}],\"symbols_line\":[1,2,3,4,5,6,7,8,9],\"symbols_scat\":[11,12,13,14,15],\"symbols_wild\":[10],\"transitions\":[{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"init\",\"win\":false},\"dst\":\"spins\",\"src\":\"none\"},{\"act\":{\"args\":[\"bet_per_line\",\"lines\"],\"bet\":true,\"cheat\":true,\"name\":\"spin\",\"win\":true},\"dst\":\"spins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_init\",\"win\":false},\"dst\":\"freespins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"freespin\",\"win\":true},\"dst\":\"freespins\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"bonus_init\",\"win\":false},\"dst\":\"bonus\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"bonus_init\",\"win\":false},\"dst\":\"bonus\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"respin\",\"win\":true},\"dst\":\"bonus\",\"src\":\"bonus\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"bonus_spins_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"bonus\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"bonus_freespins_stop\",\"win\":false},\"dst\":\"freespins\",\"src\":\"bonus\"}],\"version\":\"a\"}";
            }
        }
        #endregion
        public GoldExpressGameLogic()
        {
            _gameID = GAMEID.GoldExpress;
            GameName = "GoldExpress";
        }
    }
}
