using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MagicBallGameLogic : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "magic_ball";
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
                return "{\"actions\":[\"spin\",\"buy_spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":20000,\"board\":[[1,5,10],[3,4,9],[1,10,2],[9,5,1],[10,2,3]],\"lines\":10,\"round_bet\":200000,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[10],\"bets\":[1000,2000,3000,5000,10000,20000,30000,50000,70000,100000,150000,200000,300000],\"big_win\":[30,50,70],\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"freespins_buying_price\":100,\"freespins_granted\":12,\"lines\":[10],\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,2,2,2,1],[1,0,0,0,1],[2,2,1,0,0],[0,0,1,2,2],[2,1,1,1,0]],\"paytable\":{\"1\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":5,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":750,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":5,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":750,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":5,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":400,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":2000,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":10,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":1000,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":5000,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"multiplier\":10,\"occurrences\":2,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":1000,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":5000,\"occurrences\":5,\"type\":\"lb\"}],\"11\":[{\"freespins\":12,\"multiplier\":2,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":12,\"multiplier\":20,\"occurrences\":4,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":12,\"multiplier\":200,\"occurrences\":5,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"buy\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"freespins\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"freespins_2\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"freespins_2_paid\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"freespins_3\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"freespins_3_paid\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"freespins_paid\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"spins\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]]},\"rows\":3,\"scatter_pays\":{\"3\":2,\"4\":20,\"5\":200},\"small_win\":2,\"symbols\":[{\"id\":1,\"name\":\"el_01\",\"type\":\"line\"},{\"id\":2,\"name\":\"el_02\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_03\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_04\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_05\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_06\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_07\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_08\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_09\",\"type\":\"line\"},{\"id\":10,\"name\":\"el_wild\",\"type\":\"wild\"},{\"id\":11,\"name\":\"el_scatter\",\"type\":\"scat\"},{\"id\":12,\"name\":\"hidden\",\"type\":\"hide\"}],\"symbols_hide\":[12],\"symbols_line\":[1,2,3,4,5,6,7,8,9],\"symbols_scat\":[11],\"symbols_wild\":[10],\"version\":\"a\"}";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }
        #endregion

        public MagicBallGameLogic()
        {
            _gameID = GAMEID.MagicBall;
            GameName = "MagicBall";
        }

    }
}
