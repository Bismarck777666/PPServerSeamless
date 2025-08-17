using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class PearlBeautyGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "pearl_beauty";
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 1, 2, 3, 4, 5, 8, 10, 20, 30, 40, 60, 90, 100, 200, 300, 400, 500 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><source game-ver=\"60420\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"top1\" count=\"3\" coef=\"4\"/><combination symbol=\"1\" name=\"top1\" count=\"4\" coef=\"12\"/><combination symbol=\"1\" name=\"top1\" count=\"5\" coef=\"50\"/><combination symbol=\"2\" name=\"top2\" count=\"3\" coef=\"3\"/><combination symbol=\"2\" name=\"top2\" count=\"4\" coef=\"10\"/><combination symbol=\"2\" name=\"top2\" count=\"5\" coef=\"30\"/><combination symbol=\"3\" name=\"top3\" count=\"3\" coef=\"3\"/><combination symbol=\"3\" name=\"top3\" count=\"4\" coef=\"5\"/><combination symbol=\"3\" name=\"top3\" count=\"5\" coef=\"20\"/><combination symbol=\"4\" name=\"top4\" count=\"3\" coef=\"2\"/><combination symbol=\"4\" name=\"top4\" count=\"4\" coef=\"4\"/><combination symbol=\"4\" name=\"top4\" count=\"5\" coef=\"15\"/><combination symbol=\"5\" name=\"top5\" count=\"3\" coef=\"2\"/><combination symbol=\"5\" name=\"top5\" count=\"4\" coef=\"3\"/><combination symbol=\"5\" name=\"top5\" count=\"5\" coef=\"10\"/><combination symbol=\"6\" name=\"a\" count=\"3\" coef=\"1\"/><combination symbol=\"6\" name=\"a\" count=\"4\" coef=\"2\"/><combination symbol=\"6\" name=\"a\" count=\"5\" coef=\"3\"/><combination symbol=\"7\" name=\"k\" count=\"3\" coef=\"1\"/><combination symbol=\"7\" name=\"k\" count=\"4\" coef=\"2\"/><combination symbol=\"7\" name=\"k\" count=\"5\" coef=\"3\"/><combination symbol=\"8\" name=\"q\" count=\"3\" coef=\"1\"/><combination symbol=\"8\" name=\"q\" count=\"4\" coef=\"2\"/><combination symbol=\"8\" name=\"q\" count=\"5\" coef=\"3\"/><combination symbol=\"9\" name=\"j\" count=\"3\" coef=\"1\"/><combination symbol=\"9\" name=\"j\" count=\"4\" coef=\"2\"/><combination symbol=\"9\" name=\"j\" count=\"5\" coef=\"3\"/><combination symbol=\"10\" name=\"10\" count=\"3\" coef=\"1\"/><combination symbol=\"10\" name=\"10\" count=\"4\" coef=\"2\"/><combination symbol=\"10\" name=\"10\" count=\"5\" coef=\"3\"/><combination symbol=\"11\" name=\"9\" count=\"3\" coef=\"1\"/><combination symbol=\"11\" name=\"9\" count=\"4\" coef=\"2\"/><combination symbol=\"11\" name=\"9\" count=\"5\" coef=\"3\"/></combinations><symbols><symbol id=\"1\" title=\"top1\"/><symbol id=\"2\" title=\"top2\"/><symbol id=\"3\" title=\"top3\"/><symbol id=\"4\" title=\"top4\"/><symbol id=\"5\" title=\"top5\"/><symbol id=\"6\" title=\"a\"/><symbol id=\"7\" title=\"k\"/><symbol id=\"8\" title=\"q\"/><symbol id=\"9\" title=\"j\"/><symbol id=\"10\" title=\"10\"/><symbol id=\"11\" title=\"9\"/><symbol id=\"12\" title=\"bonus\"/><symbol id=\"13\" title=\"hidden\"/><symbol id=\"14\" title=\"wild\"/><symbol id=\"15\" title=\"bonuscollect\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"13\" symbols=\"5,4,2,3,6,12,7,13,8,9,1,11,10\"/><reel id=\"2\" length=\"14\" symbols=\"5,8,12,11,1,4,3,14,10,13,2,7,6,9\"/><reel id=\"3\" length=\"14\" symbols=\"1,3,10,12,11,13,4,5,2,14,7,8,6,9\"/><reel id=\"4\" length=\"14\" symbols=\"4,2,1,5,6,12,7,13,14,11,3,10,9,8\"/><reel id=\"5\" length=\"13\" symbols=\"4,11,8,2,12,9,13,5,1,7,3,10,6\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"13\" symbols=\"5,4,2,3,6,12,7,13,8,9,1,11,10\"/><reel id=\"2\" length=\"14\" symbols=\"5,8,12,11,1,4,3,14,10,13,2,7,6,9\"/><reel id=\"3\" length=\"14\" symbols=\"1,3,10,12,11,13,4,5,2,14,7,8,6,9\"/><reel id=\"4\" length=\"14\" symbols=\"4,2,1,5,6,12,7,13,14,11,3,10,9,8\"/><reel id=\"5\" length=\"13\" symbols=\"4,11,8,2,12,9,13,5,1,7,3,10,6\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"12,12,9\" reel2=\"2,2,5\" reel3=\"2,14,5\" reel4=\"3,3,3\" reel5=\"11,12,12\"><bonus bonus_pos=\"0,5,9,14\" bonus_tb=\"5,12,1,6\" bonus_type=\"0,0,0,0\"/></shift><game jackpots_tb=\"20,50,1000\" bonus_spins=\"3\" base_ways=\"243\" total_bet_mult=\"20\"/><delivery id=\"1149111-748236755101309902281465\" action=\"create\"/></server>";
            }
        }
        #endregion

        public PearlBeautyGameLogic()
        {
            _gameID     = GAMEID.PearlBeauty;
            GameName    = "PearlBeauty";
        }
    }
}
