using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class ScarabRichesGameLogic : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "scarab_riches";
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
                return "{\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":40,\"board\":[[3,13,2],[2,12,6],[5,13,2],[2,12,4],[3,13,2]],\"counter\":0,\"feature_board\":[[3,13,2],[2,12,6],[5,13,2],[2,12,4],[3,13,2]],\"is_extra_feature\":false,\"lines\":25,\"portal_level\":0,\"round_bet\":1000,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[25],\"bets\":[4,8,10,16,30,40,60,100,160,200,400,600,1000,1600],\"big_win\":[25,50,100],\"client_scarab_prob\":20,\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"lines\":[25],\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,2,2,2,1],[1,0,0,0,1],[2,2,1,2,2],[0,0,1,0,0],[1,1,2,1,1],[1,1,0,1,1],[0,2,0,2,0],[2,0,2,0,2],[0,1,0,1,0],[2,1,2,1,2],[1,0,1,0,1],[1,2,1,2,1],[0,1,1,1,0],[2,1,1,1,2],[0,2,2,2,0],[2,0,0,0,2],[2,0,1,0,2],[0,2,1,2,0],[1,0,2,0,1],[1,2,0,2,1]],\"paytable\":{\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":15,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"multiplier\":15,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"11\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":300,\"occurrences\":5,\"type\":\"lb\"}],\"12\":[{\"multiplier\":25,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":400,\"occurrences\":5,\"type\":\"lb\"}],\"13\":[{\"freespins\":10,\"multiplier\":0,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"freespins\":[[2,3,4,5,6,7,8,9,10,11,13],[2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11,12,13],[2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11,12,13]],\"spins\":[[2,3,4,5,6,7,8,9,10,11,13],[2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11,12,13],[2,3,4,5,6,7,8,9,10,11,12],[2,3,4,5,6,7,8,9,10,11,12,13]]},\"rows\":3,\"symbols\":[{\"id\":2,\"name\":\"el_10\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_J\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_Q\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_K\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_A\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_cat\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_dog\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_horus\",\"type\":\"line\"},{\"id\":10,\"name\":\"el_nefertiti\",\"type\":\"line\"},{\"id\":11,\"name\":\"el_pharaoh\",\"type\":\"line\"},{\"id\":12,\"name\":\"el_wild\",\"type\":\"wild\"},{\"id\":13,\"name\":\"el_scatter\",\"type\":\"scat\"},{\"id\":14,\"name\":\"hidden\",\"type\":\"hide\"}],\"symbols_hide\":[14],\"symbols_line\":[2,3,4,5,6,7,8,9,10,11],\"symbols_scat\":[13],\"symbols_wild\":[12],\"transitions\":[{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"init\",\"win\":false},\"dst\":\"spins\",\"src\":\"none\"},{\"act\":{\"args\":[\"bet_per_line\",\"lines\"],\"bet\":true,\"cheat\":true,\"name\":\"spin\",\"win\":true},\"dst\":\"spins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_init\",\"win\":false},\"dst\":\"freespins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"freespin\",\"win\":true},\"dst\":\"freespins\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"freespins\"}],\"version\":\"a\"}";
            }
        }
        #endregion
        public ScarabRichesGameLogic()
        {
            _gameID = GAMEID.ScarabRiches;
            GameName = "ScarabRiches";
        }
    }
}
