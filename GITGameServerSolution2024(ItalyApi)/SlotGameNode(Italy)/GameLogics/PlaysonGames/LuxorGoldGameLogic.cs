using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class LuxorGoldGameLogic : BasePlaysonHillSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "luxor_gold";
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
                return "<server><source game-ver=\"200122\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low_4\" count=\"3\" coef=\"4\"/><combination symbol=\"1\" name=\"low_4\" count=\"4\" coef=\"8\"/><combination symbol=\"1\" name=\"low_4\" count=\"5\" coef=\"40\"/><combination symbol=\"2\" name=\"low_3\" count=\"3\" coef=\"4\"/><combination symbol=\"2\" name=\"low_3\" count=\"4\" coef=\"8\"/><combination symbol=\"2\" name=\"low_3\" count=\"5\" coef=\"40\"/><combination symbol=\"3\" name=\"low_2\" count=\"3\" coef=\"4\"/><combination symbol=\"3\" name=\"low_2\" count=\"4\" coef=\"8\"/><combination symbol=\"3\" name=\"low_2\" count=\"5\" coef=\"40\"/><combination symbol=\"4\" name=\"low_1\" count=\"3\" coef=\"4\"/><combination symbol=\"4\" name=\"low_1\" count=\"4\" coef=\"8\"/><combination symbol=\"4\" name=\"low_1\" count=\"5\" coef=\"40\"/><combination symbol=\"5\" name=\"mid_4\" count=\"3\" coef=\"4\"/><combination symbol=\"5\" name=\"mid_4\" count=\"4\" coef=\"16\"/><combination symbol=\"5\" name=\"mid_4\" count=\"5\" coef=\"80\"/><combination symbol=\"6\" name=\"mid_3\" count=\"3\" coef=\"4\"/><combination symbol=\"6\" name=\"mid_3\" count=\"4\" coef=\"20\"/><combination symbol=\"6\" name=\"mid_3\" count=\"5\" coef=\"120\"/><combination symbol=\"7\" name=\"mid_2\" count=\"3\" coef=\"4\"/><combination symbol=\"7\" name=\"mid_2\" count=\"4\" coef=\"24\"/><combination symbol=\"7\" name=\"mid_2\" count=\"5\" coef=\"160\"/><combination symbol=\"8\" name=\"mid_1\" count=\"3\" coef=\"8\"/><combination symbol=\"8\" name=\"mid_1\" count=\"4\" coef=\"32\"/><combination symbol=\"8\" name=\"mid_1\" count=\"5\" coef=\"200\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"12\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"48\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"240\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"1\"/><combination symbol=\"10\" name=\"scatter\" count=\"4\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"1\"/><combination symbol=\"10\" name=\"scatter\" count=\"5\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"1\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"2,3,2,1,2\"/><payline id=\"11\" path=\"2,1,2,3,2\"/><payline id=\"12\" path=\"1,2,2,2,1\"/><payline id=\"13\" path=\"3,2,2,2,3\"/><payline id=\"14\" path=\"1,2,1,2,1\"/><payline id=\"15\" path=\"3,2,3,2,3\"/><payline id=\"16\" path=\"2,2,1,2,2\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"1,1,3,1,1\"/><payline id=\"19\" path=\"3,3,1,3,3\"/><payline id=\"20\" path=\"1,3,3,3,1\"/><payline id=\"21\" path=\"3,1,1,1,3\"/><payline id=\"22\" path=\"2,3,1,3,2\"/><payline id=\"23\" path=\"2,1,3,1,2\"/><payline id=\"24\" path=\"1,3,1,3,1\"/><payline id=\"25\" path=\"3,1,3,1,3\"/></paylines><symbols><symbol id=\"1\" title=\"low_4\"/><symbol id=\"2\" title=\"low_3\"/><symbol id=\"3\" title=\"low_2\"/><symbol id=\"4\" title=\"low_1\"/><symbol id=\"5\" title=\"mid_4\"/><symbol id=\"6\" title=\"mid_3\"/><symbol id=\"7\" title=\"mid_2\"/><symbol id=\"8\" title=\"mid_1\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"scatter\"/><symbol id=\"11\" title=\"bonus\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"6,5,1,7,11,3,2,4,8,9\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,5,10,2,7,6,3,11,4\"/><reel id=\"3\" length=\"11\" symbols=\"8,6,9,3,10,5,11,2,4,7,1\"/><reel id=\"4\" length=\"11\" symbols=\"8,9,1,11,4,7,10,5,2,3,6\"/><reel id=\"5\" length=\"10\" symbols=\"9,8,1,3,5,7,4,2,6,11\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"6,5,1,7,11,3,2,4,8,9\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,5,10,2,7,6,3,11,4\"/><reel id=\"3\" length=\"11\" symbols=\"8,6,9,3,10,5,11,2,4,7,1\"/><reel id=\"4\" length=\"11\" symbols=\"8,9,1,11,4,7,10,5,2,3,6\"/><reel id=\"5\" length=\"10\" symbols=\"9,8,1,3,5,7,4,2,11,6\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"6\" symbols=\"7,6,5,11,8,9\"/><reel id=\"2\" length=\"7\" symbols=\"5,8,7,10,6,9,11\"/><reel id=\"3\" length=\"7\" symbols=\"5,9,7,6,8,10,11\"/><reel id=\"4\" length=\"7\" symbols=\"5,9,7,10,6,8,11\"/><reel id=\"5\" length=\"6\" symbols=\"6,9,8,5,7,11\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"5\" symbols=\"7,6,5,8,9\"/><reel id=\"2\" length=\"6\" symbols=\"5,8,7,10,6,9\"/><reel id=\"3\" length=\"6\" symbols=\"5,9,7,6,8,10\"/><reel id=\"4\" length=\"6\" symbols=\"5,9,7,10,6,8\"/><reel id=\"5\" length=\"5\" symbols=\"6,9,8,5,7\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"2,8,8\" reel2=\"5,5,10\" reel3=\"11,11,3\" reel4=\"8,7,4\" reel5=\"6,9,9\"><bonus bonus_pos=\"2,7\" bonus_tb=\"7,14\"/></shift><shift_ext><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"6,11,11\" reel2=\"8,7,8\" reel3=\"5,9,9\" reel4=\"6,8,5\" reel5=\"11,11,7\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"8,5,7\" reel2=\"7,6,11\" reel3=\"5,10,6\" reel4=\"11,8,7\" reel5=\"6,9,9\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"6,5,11\" reel2=\"11,7,8\" reel3=\"10,6,7\" reel4=\"5,9,9\" reel5=\"8,7,11\"/></shift_ext><game jackpots_tb=\"20,50,150,5000\" bonus_spins=\"3\" total_bet_mult=\"20\"/><delivery id=\"445010-3181081219583328625457282\" action=\"create\"/></server>";
            }
        }
        #endregion
        public LuxorGoldGameLogic()
        {
            _gameID = GAMEID.LuxorGold;
            GameName = "LuxorGold";
        }
    }
}
