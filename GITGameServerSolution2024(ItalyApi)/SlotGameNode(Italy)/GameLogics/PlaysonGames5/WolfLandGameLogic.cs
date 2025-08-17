using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class WolfLandGameLogic : BasePlaysonHillSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "wolf_land";
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
                return "<server><source game-ver=\"160223\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low_4\" count=\"3\" coef=\"2\"/><combination symbol=\"1\" name=\"low_4\" count=\"4\" coef=\"8\"/><combination symbol=\"1\" name=\"low_4\" count=\"5\" coef=\"20\"/><combination symbol=\"2\" name=\"low_3\" count=\"3\" coef=\"2\"/><combination symbol=\"2\" name=\"low_3\" count=\"4\" coef=\"8\"/><combination symbol=\"2\" name=\"low_3\" count=\"5\" coef=\"20\"/><combination symbol=\"3\" name=\"low_2\" count=\"3\" coef=\"2\"/><combination symbol=\"3\" name=\"low_2\" count=\"4\" coef=\"8\"/><combination symbol=\"3\" name=\"low_2\" count=\"5\" coef=\"20\"/><combination symbol=\"4\" name=\"low_1\" count=\"3\" coef=\"4\"/><combination symbol=\"4\" name=\"low_1\" count=\"4\" coef=\"8\"/><combination symbol=\"4\" name=\"low_1\" count=\"5\" coef=\"20\"/><combination symbol=\"5\" name=\"mid_4\" count=\"3\" coef=\"4\"/><combination symbol=\"5\" name=\"mid_4\" count=\"4\" coef=\"20\"/><combination symbol=\"5\" name=\"mid_4\" count=\"5\" coef=\"60\"/><combination symbol=\"6\" name=\"mid_3\" count=\"3\" coef=\"6\"/><combination symbol=\"6\" name=\"mid_3\" count=\"4\" coef=\"30\"/><combination symbol=\"6\" name=\"mid_3\" count=\"5\" coef=\"100\"/><combination symbol=\"7\" name=\"mid_2\" count=\"3\" coef=\"8\"/><combination symbol=\"7\" name=\"mid_2\" count=\"4\" coef=\"40\"/><combination symbol=\"7\" name=\"mid_2\" count=\"5\" coef=\"120\"/><combination symbol=\"8\" name=\"mid_1\" count=\"3\" coef=\"10\"/><combination symbol=\"8\" name=\"mid_1\" count=\"4\" coef=\"100\"/><combination symbol=\"8\" name=\"mid_1\" count=\"5\" coef=\"200\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"10\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"100\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"200\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"2\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"4,4,4,4,4\"/><payline id=\"5\" path=\"1,2,3,2,1\"/><payline id=\"6\" path=\"2,3,4,3,2\"/><payline id=\"7\" path=\"3,2,1,2,3\"/><payline id=\"8\" path=\"4,3,2,3,4\"/><payline id=\"9\" path=\"1,2,1,2,1\"/><payline id=\"10\" path=\"2,3,2,3,2\"/><payline id=\"11\" path=\"3,4,3,4,3\"/><payline id=\"12\" path=\"2,1,2,1,2\"/><payline id=\"13\" path=\"3,2,3,2,3\"/><payline id=\"14\" path=\"4,3,4,3,4\"/><payline id=\"15\" path=\"1,2,2,2,1\"/><payline id=\"16\" path=\"2,3,3,3,2\"/><payline id=\"17\" path=\"3,4,4,4,3\"/><payline id=\"18\" path=\"2,1,1,1,2\"/><payline id=\"19\" path=\"3,2,2,2,3\"/><payline id=\"20\" path=\"4,3,3,3,4\"/><payline id=\"21\" path=\"1,1,2,1,1\"/><payline id=\"22\" path=\"2,2,3,2,2\"/><payline id=\"23\" path=\"3,3,4,3,3\"/><payline id=\"24\" path=\"2,2,1,2,2\"/><payline id=\"25\" path=\"3,3,2,3,3\"/></paylines><symbols><symbol id=\"1\" title=\"low_4\"/><symbol id=\"2\" title=\"low_3\"/><symbol id=\"3\" title=\"low_2\"/><symbol id=\"4\" title=\"low_1\"/><symbol id=\"5\" title=\"mid_4\"/><symbol id=\"6\" title=\"mid_3\"/><symbol id=\"7\" title=\"mid_2\"/><symbol id=\"8\" title=\"mid_1\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"scatter\"/><symbol id=\"11\" title=\"bonus\"/><symbol id=\"12\" title=\"boost\"/><symbol id=\"13\" title=\"hidden\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"11\" symbols=\"5,10,2,11,1,3,8,9,4,7,6\"/><reel id=\"2\" length=\"10\" symbols=\"7,3,5,2,6,4,11,1,8,9\"/><reel id=\"3\" length=\"12\" symbols=\"5,8,6,10,1,3,4,7,2,11,12,9\"/><reel id=\"4\" length=\"10\" symbols=\"8,4,2,6,3,1,5,9,7,11\"/><reel id=\"5\" length=\"11\" symbols=\"5,4,3,10,6,1,9,11,2,8,7\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"11\" symbols=\"5,10,1,11,3,8,2,4,9,6,7\"/><reel id=\"2\" length=\"10\" symbols=\"7,3,6,5,2,11,8,4,9,1\"/><reel id=\"3\" length=\"12\" symbols=\"7,6,2,4,10,11,12,8,5,9,1,3\"/><reel id=\"4\" length=\"10\" symbols=\"7,4,2,11,5,8,3,1,6,9\"/><reel id=\"5\" length=\"11\" symbols=\"5,4,3,10,1,6,8,2,11,7,9\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"11\" symbols=\"6,10,2,11,4,3,8,7,1,9,5\"/><reel id=\"2\" length=\"10\" symbols=\"6,1,5,7,2,3,4,8,11,9\"/><reel id=\"3\" length=\"12\" symbols=\"8,5,3,12,4,10,7,2,6,1,9,11\"/><reel id=\"4\" length=\"10\" symbols=\"8,4,2,11,3,9,1,5,7,6\"/><reel id=\"5\" length=\"11\" symbols=\"5,4,11,3,10,1,6,7,2,8,9\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"11\" symbols=\"5,4,10,2,1,6,3,8,7,11,9\"/><reel id=\"2\" length=\"10\" symbols=\"7,3,5,2,11,4,8,9,1,6\"/><reel id=\"3\" length=\"11\" symbols=\"3,5,8,10,6,4,7,1,2,9,11\"/><reel id=\"4\" length=\"10\" symbols=\"7,4,9,2,11,5,8,3,1,6\"/><reel id=\"5\" length=\"11\" symbols=\"5,4,3,10,6,11,7,2,8,1,9\"/></reels><reels id=\"5\"><reel id=\"1\" length=\"11\" symbols=\"5,10,2,4,11,3,9,1,6,8,7\"/><reel id=\"2\" length=\"10\" symbols=\"7,5,1,2,3,8,4,6,11,9\"/><reel id=\"3\" length=\"11\" symbols=\"8,5,6,7,4,10,3,1,9,2,11\"/><reel id=\"4\" length=\"10\" symbols=\"8,4,2,11,5,3,1,9,6,7\"/><reel id=\"5\" length=\"11\" symbols=\"5,1,9,4,3,10,6,7,2,8,11\"/></reels><reels id=\"6\"><reel id=\"1\" length=\"12\" symbols=\"5,3,10,2,1,6,8,4,7,13,11,9\"/><reel id=\"2\" length=\"11\" symbols=\"7,3,11,5,6,1,2,4,8,13,9\"/><reel id=\"3\" length=\"13\" symbols=\"8,5,12,6,1,4,2,13,10,11,3,7,9\"/><reel id=\"4\" length=\"11\" symbols=\"8,4,2,11,13,5,7,1,3,9,6\"/><reel id=\"5\" length=\"12\" symbols=\"5,7,1,3,10,6,11,13,4,2,8,9\"/></reels><reels id=\"7\"><reel id=\"1\" length=\"10\" symbols=\"5,3,10,2,1,6,8,4,7,9\"/><reel id=\"2\" length=\"9\" symbols=\"7,3,5,6,1,2,4,8,9\"/><reel id=\"3\" length=\"10\" symbols=\"8,5,6,1,10,4,2,3,7,9\"/><reel id=\"4\" length=\"9\" symbols=\"8,4,2,5,7,1,3,9,6\"/><reel id=\"5\" length=\"10\" symbols=\"5,7,1,3,10,6,4,2,8,9\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"9,9,9,9\" reel2=\"11,11,11,2\" reel3=\"3,3,10,1\" reel4=\"1,1,9,9\" reel5=\"3,11,11,2\"><bonus bonus_pos=\"1,6,9,11,14\" bonus_tb=\"10,8,5,5,100\"/></shift><game last_nfs_win=\"0\" jackpots_tb=\"20,50,100,3000\" bonus_spins=\"3\" total_bet_mult=\"10\"/><delivery id=\"2364588-905989552025130668422301\" action=\"create\"/></server>";
            }
        }
        #endregion
        public WolfLandGameLogic()
        {
            _gameID     = GAMEID.WolfLand;
            GameName    = "WolfLand";
        }
    }
}
