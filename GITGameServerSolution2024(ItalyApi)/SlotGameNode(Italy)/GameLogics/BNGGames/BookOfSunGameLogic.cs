using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BookOfSunGameLogic : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "book_of_sun";
            }
        }
        protected override string InitResultString
        {
            get
            {
                return "{\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":75,\"board\":[[1,5,10],[3,4,9],[1,10,2],[9,5,1],[10,2,3]],\"lines\":10,\"round_bet\":750,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 10;
            }
        }
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[10],\"bets\":[10,20,25,40,75,100,200,300,500,1000,1500,2500],\"big_win\":[40,70,100],\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"freespins_granted\":12,\"lines\":[10],\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,2,2,2,1],[1,0,0,0,1],[2,2,1,0,0],[0,0,1,2,2],[2,1,1,1,0]],\"paytable\":{\"1\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":5,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":750,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":5,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":750,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":5,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":400,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":2000,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":10,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":1000,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":5000,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"multiplier\":10,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":1000,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":5000,\"occurrences\":5,\"type\":\"lb\"}],\"11\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"tb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"tb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"tb\"}]},\"reelsamples\":{\"freespins\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"spins\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]]},\"rows\":3,\"scatter_pays\":{\"3\":2,\"4\":20,\"5\":200},\"symbols\":[{\"id\":1,\"name\":\"10\",\"type\":\"line\"},{\"id\":2,\"name\":\"j\",\"type\":\"line\"},{\"id\":3,\"name\":\"q\",\"type\":\"line\"},{\"id\":4,\"name\":\"k\",\"type\":\"line\"},{\"id\":5,\"name\":\"a\",\"type\":\"line\"},{\"id\":6,\"name\":\"bird\",\"type\":\"line\"},{\"id\":7,\"name\":\"scarabeus\",\"type\":\"line\"},{\"id\":8,\"name\":\"horus\",\"type\":\"line\"},{\"id\":9,\"name\":\"pharaoh\",\"type\":\"line\"},{\"id\":10,\"name\":\"scatter\",\"type\":\"wild\"},{\"id\":11,\"name\":\"book\",\"type\":\"scatter\"}],\"symbols_line\":[1,2,3,4,5,6,7,8,9],\"symbols_scatter\":[11],\"symbols_wild\":[10],\"transitions\":[{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"init\",\"win\":false},\"dst\":\"spins\",\"src\":\"none\"},{\"act\":{\"args\":[\"bet_per_line\",\"lines\"],\"bet\":true,\"cheat\":true,\"name\":\"spin\",\"win\":true},\"dst\":\"spins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_init\",\"win\":false},\"dst\":\"freespins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"freespin\",\"win\":true},\"dst\":\"freespins\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"freespins\"}],\"version\":\"a\"}";
            }
        }
        #endregion
        public BookOfSunGameLogic()
        {
            _gameID = GAMEID.BookOfSun;
            GameName = "BookOfSun";
        }
    }
}
