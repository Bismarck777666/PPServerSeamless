using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class StickyPiggyGameLogic : BaseBNGMultiFreeGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "sticky_piggy";
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
                return "{\"actions\":[\"spin\",\"buy_spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":5000,\"board\":[[3,13,7],[11,2,12],[2,12,5],[12,2,10],[6,13,3]],\"lines\":20,\"round_bet\":100000,\"round_win\":0,\"total_win\":0,\"wild_mps\":[[1,1,1],[1,1,3],[1,3,1],[3,1,1],[1,1,1]]},\"version\":1}";
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[20],\"bets\":[1000,2000,2500,3750,5000,6250,7500,10000,12500,15000,20000,25000,30000,37500,50000,55000,62500,75000,100000,125000,150000],\"big_win\":[20,30,60,100,150],\"cells\":15,\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"freespins_buying_price\":{\"3\":100,\"4\":200,\"5\":400},\"init_board_freespins\":[[11,11,11],[9,7,10],[2,10,5],[10,6,8],[11,11,11]],\"init_wild_mps\":[[1,1,1],[1,1,3],[1,3,1],[3,1,1],[1,1,1]],\"line_symbols\":[1,2,3,4,5,6,7,8,9,10,11],\"lines\":[20],\"pattern_bf\":{\"0\":[0,1,2,3,4],\"1\":[0,1,3,4,2],\"2\":[0,1,4,2,3],\"3\":[0,2,3,4,1],\"4\":[0,2,4,1,3],\"5\":[0,3,4,2,1],\"6\":[1,2,3,0,4],\"7\":[1,2,4,3,0],\"8\":[1,3,4,0,2],\"9\":[2,3,4,1,0]},\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,0,0,0,1],[1,2,2,2,1],[0,0,1,2,2],[2,2,1,0,0],[1,2,1,0,1],[1,0,1,2,1],[0,1,1,1,0],[2,1,1,1,2],[0,1,0,1,0],[2,1,2,1,2],[1,1,0,1,1],[1,1,2,1,1],[0,0,2,0,0],[2,2,0,2,2],[0,2,2,2,0]],\"paytable\":{\"1\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":5,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":5,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":5,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":8,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":12,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":25,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":300,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"multiplier\":35,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":500,\"occurrences\":5,\"type\":\"lb\"}],\"11\":[{\"multiplier\":50,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":750,\"occurrences\":5,\"type\":\"lb\"}],\"13\":[{\"freespins\":9,\"multiplier\":1,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":12,\"multiplier\":10,\"occurrences\":4,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":15,\"multiplier\":50,\"occurrences\":5,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"pick_src\":14,\"pick_wage\":[0,0,15,19,21],\"reelsamples\":{\"bf_3\":[[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,8,10],[1,2,3,4,5,7,9,11]],\"bf_4\":[[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,8,9,10,11]],\"bf_5\":[[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13],[1,2,3,4,5,6,7,13]],\"f\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11]],\"f_paid_3\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11]],\"f_paid_4\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11]],\"f_paid_5\":[[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11,12],[1,2,3,4,5,6,7,8,9,10,11]],\"m_1\":[[1,2,3,4,5,6,7,8,9,10,11,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,13]],\"m_2\":[[1,2,3,4,5,6,7,8,9,10,11,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,13]]},\"rows\":3,\"scat_id\":13,\"shifter_path\":\"../../shifter_data/sHiftEr_ReElsets\",\"symbols\":[{\"id\":1,\"name\":\"10\",\"type\":\"line\"},{\"id\":2,\"name\":\"J\",\"type\":\"line\"},{\"id\":3,\"name\":\"Q\",\"type\":\"line\"},{\"id\":4,\"name\":\"K\",\"type\":\"line\"},{\"id\":5,\"name\":\"A\",\"type\":\"line\"},{\"id\":6,\"name\":\"keys\",\"type\":\"line\"},{\"id\":7,\"name\":\"newspaper\",\"type\":\"line\"},{\"id\":8,\"name\":\"thief_blue\",\"type\":\"line\"},{\"id\":9,\"name\":\"thief_green\",\"type\":\"line\"},{\"id\":10,\"name\":\"lady\",\"type\":\"line\"},{\"id\":11,\"name\":\"banker\",\"type\":\"line\"},{\"id\":12,\"name\":\"wild\",\"type\":\"wild\"},{\"id\":13,\"name\":\"scatter\",\"type\":\"scat\"}],\"symbols_line\":[1,2,3,4,5,6,7,8,9,10,11],\"symbols_scat\":[13],\"symbols_wild\":[12],\"version\":\"a\",\"wild_id\":12,\"wild_mp_3_wage3_f\":345,\"wild_mp_3_wage3_m\":400,\"wild_mp_f_src\":5,\"wild_mp_m_src\":11}";
            }
        }
        #endregion

        public StickyPiggyGameLogic()
        {
            _gameID     = GAMEID.StickyPiggy;
            GameName    = "StickyPiggy";
        }

    }
}
