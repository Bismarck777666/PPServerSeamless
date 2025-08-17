using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class RoyalFortGameLogic : BasePlaysonHillSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "royal_fort";
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 1, 2, 3, 5, 7, 10, 20, 30, 50, 70, 100, 200, 300, 500, 700, 1000 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><source game-ver=\"60323\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"3\" name=\"top\" count=\"3\" coef=\"500\"/><combination symbol=\"4\" name=\"mid4\" count=\"3\" coef=\"300\"/><combination symbol=\"5\" name=\"mid3\" count=\"3\" coef=\"200\"/><combination symbol=\"6\" name=\"mid2\" count=\"3\" coef=\"160\"/><combination symbol=\"7\" name=\"mid1\" count=\"3\" coef=\"160\"/><combination symbol=\"8\" name=\"low4\" count=\"3\" coef=\"40\"/><combination symbol=\"9\" name=\"low3\" count=\"3\" coef=\"40\"/><combination symbol=\"10\" name=\"low2\" count=\"3\" coef=\"40\"/><combination symbol=\"11\" name=\"low1\" count=\"3\" coef=\"10\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1\"/><payline id=\"2\" path=\"2,2,2\"/><payline id=\"3\" path=\"3,3,3\"/><payline id=\"4\" path=\"1,2,3\"/><payline id=\"5\" path=\"3,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"diamond\"/><symbol id=\"2\" title=\"collect\"/><symbol id=\"3\" title=\"top\"/><symbol id=\"4\" title=\"mid4\"/><symbol id=\"5\" title=\"mid3\"/><symbol id=\"6\" title=\"mid2\"/><symbol id=\"7\" title=\"mid1\"/><symbol id=\"8\" title=\"low4\"/><symbol id=\"9\" title=\"low3\"/><symbol id=\"10\" title=\"low2\"/><symbol id=\"11\" title=\"low1\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"8,6,9,11,5,1,4,3,10,7\"/><reel id=\"2\" length=\"10\" symbols=\"5,3,6,11,7,4,2,8,10,9\"/><reel id=\"3\" length=\"10\" symbols=\"8,11,3,10,9,4,1,7,6,5\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"8,6,9,1,11,5,4,3,10,7\"/><reel id=\"2\" length=\"10\" symbols=\"5,3,6,11,7,4,2,8,10,9\"/><reel id=\"3\" length=\"10\" symbols=\"8,11,3,10,9,4,1,7,6,5\"/></reels></slot><shift server=\"0,0,0\" reel_set=\"1\" reel1=\"4,5,6\" reel2=\"4,11,2\" reel3=\"10,1,9\"><bonus><before_collect bonus_pos=\"7,5\" bonus_tb=\"0,5\" bonus_type=\"1,0\"/></bonus></shift><game total_bet_mult=\"10\" jackpots_tb=\"25,50,150,1000\" bonus_spins=\"3\" progress_level=\"0\"/><delivery id=\"2363656-892078120341672216913313\" action=\"create\"/></server>";
            }
        }
        #endregion
        public RoyalFortGameLogic()
        {
            _gameID     = GAMEID.RoyalFort;
            GameName    = "RoyalFort";
        }
    }
}
