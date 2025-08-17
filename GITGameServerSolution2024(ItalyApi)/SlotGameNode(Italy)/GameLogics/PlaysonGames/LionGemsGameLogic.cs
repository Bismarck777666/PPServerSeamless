using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class LionGemsGameLogic : BasePlaysonHillSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "lion_gems";
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 2, 3, 4, 5, 6, 8, 10, 15, 20, 30, 40, 50, 75, 100, 200, 300, 500, 750, 1000 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><source game-ver=\"200122\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low_4\" count=\"3\" coef=\"2\"/><combination symbol=\"1\" name=\"low_4\" count=\"4\" coef=\"4\"/><combination symbol=\"1\" name=\"low_4\" count=\"5\" coef=\"16\"/><combination symbol=\"2\" name=\"low_3\" count=\"3\" coef=\"2\"/><combination symbol=\"2\" name=\"low_3\" count=\"4\" coef=\"4\"/><combination symbol=\"2\" name=\"low_3\" count=\"5\" coef=\"16\"/><combination symbol=\"3\" name=\"low_2\" count=\"3\" coef=\"2\"/><combination symbol=\"3\" name=\"low_2\" count=\"4\" coef=\"4\"/><combination symbol=\"3\" name=\"low_2\" count=\"5\" coef=\"16\"/><combination symbol=\"4\" name=\"low_1\" count=\"3\" coef=\"2\"/><combination symbol=\"4\" name=\"low_1\" count=\"4\" coef=\"4\"/><combination symbol=\"4\" name=\"low_1\" count=\"5\" coef=\"16\"/><combination symbol=\"5\" name=\"mid_4\" count=\"3\" coef=\"4\"/><combination symbol=\"5\" name=\"mid_4\" count=\"4\" coef=\"8\"/><combination symbol=\"5\" name=\"mid_4\" count=\"5\" coef=\"40\"/><combination symbol=\"6\" name=\"mid_3\" count=\"3\" coef=\"4\"/><combination symbol=\"6\" name=\"mid_3\" count=\"4\" coef=\"10\"/><combination symbol=\"6\" name=\"mid_3\" count=\"5\" coef=\"48\"/><combination symbol=\"7\" name=\"mid_2\" count=\"3\" coef=\"4\"/><combination symbol=\"7\" name=\"mid_2\" count=\"4\" coef=\"12\"/><combination symbol=\"7\" name=\"mid_2\" count=\"5\" coef=\"56\"/><combination symbol=\"8\" name=\"mid_1\" count=\"3\" coef=\"4\"/><combination symbol=\"8\" name=\"mid_1\" count=\"4\" coef=\"16\"/><combination symbol=\"8\" name=\"mid_1\" count=\"5\" coef=\"64\"/><combination symbol=\"9\" name=\"top\" count=\"3\" coef=\"6\"/><combination symbol=\"9\" name=\"top\" count=\"4\" coef=\"20\"/><combination symbol=\"9\" name=\"top\" count=\"5\" coef=\"80\"/><combination symbol=\"10\" name=\"wild\" count=\"3\" coef=\"6\"/><combination symbol=\"10\" name=\"wild\" count=\"4\" coef=\"20\"/><combination symbol=\"10\" name=\"wild\" count=\"5\" coef=\"80\"/><combination symbol=\"11\" name=\"scatter\" count=\"3\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"2\"/><combination symbol=\"11\" name=\"scatter\" count=\"4\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"10\"/><combination symbol=\"11\" name=\"scatter\" count=\"5\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"100\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"4,4,4,4,4\"/><payline id=\"5\" path=\"1,2,3,2,1\"/><payline id=\"6\" path=\"2,3,4,3,2\"/><payline id=\"7\" path=\"4,3,2,3,4\"/><payline id=\"8\" path=\"3,2,1,2,3\"/><payline id=\"9\" path=\"1,2,1,2,1\"/><payline id=\"10\" path=\"2,3,2,3,2\"/><payline id=\"11\" path=\"3,4,3,4,3\"/><payline id=\"12\" path=\"4,3,4,3,4\"/><payline id=\"13\" path=\"3,2,3,2,3\"/><payline id=\"14\" path=\"2,1,2,1,2\"/><payline id=\"15\" path=\"1,2,2,2,1\"/><payline id=\"16\" path=\"2,3,3,3,2\"/><payline id=\"17\" path=\"3,4,4,4,3\"/><payline id=\"18\" path=\"4,3,3,3,4\"/><payline id=\"19\" path=\"3,2,2,2,3\"/><payline id=\"20\" path=\"2,1,1,1,2\"/><payline id=\"21\" path=\"1,1,2,1,1\"/><payline id=\"22\" path=\"2,2,3,2,2\"/><payline id=\"23\" path=\"3,3,4,3,3\"/><payline id=\"24\" path=\"4,4,3,4,4\"/><payline id=\"25\" path=\"3,3,2,3,3\"/><payline id=\"26\" path=\"2,2,1,2,2\"/><payline id=\"27\" path=\"1,1,3,1,1\"/><payline id=\"28\" path=\"2,2,4,2,2\"/><payline id=\"29\" path=\"4,4,2,4,4\"/><payline id=\"30\" path=\"3,3,1,3,3\"/></paylines><symbols><symbol id=\"1\" title=\"low_4\"/><symbol id=\"2\" title=\"low_3\"/><symbol id=\"3\" title=\"low_2\"/><symbol id=\"4\" title=\"low_1\"/><symbol id=\"5\" title=\"mid_4\"/><symbol id=\"6\" title=\"mid_3\"/><symbol id=\"7\" title=\"mid_2\"/><symbol id=\"8\" title=\"mid_1\"/><symbol id=\"9\" title=\"top\"/><symbol id=\"10\" title=\"wild\"/><symbol id=\"11\" title=\"scatter\"/><symbol id=\"12\" title=\"bonus\"/><symbol id=\"13\" title=\"hidden\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"12\" symbols=\"10,1,5,12,4,6,11,3,9,7,8,2\"/><reel id=\"2\" length=\"13\" symbols=\"10,4,7,3,5,1,6,2,11,9,12,13,8\"/><reel id=\"3\" length=\"13\" symbols=\"10,4,5,2,3,1,11,7,6,9,8,12,13\"/><reel id=\"4\" length=\"13\" symbols=\"10,2,5,4,6,3,7,8,12,13,11,9,1\"/><reel id=\"5\" length=\"13\" symbols=\"10,2,8,7,4,3,5,1,12,13,9,11,6\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"12\" symbols=\"10,1,5,12,4,6,11,3,9,7,8,2\"/><reel id=\"2\" length=\"13\" symbols=\"10,4,7,3,5,1,6,2,11,9,12,13,8\"/><reel id=\"3\" length=\"13\" symbols=\"10,4,5,2,1,11,3,7,6,9,8,12,13\"/><reel id=\"4\" length=\"13\" symbols=\"10,2,5,4,6,3,7,8,12,13,11,9,1\"/><reel id=\"5\" length=\"13\" symbols=\"10,2,8,7,4,3,5,1,12,13,9,11,6\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"12\" symbols=\"10,1,6,2,11,7,4,12,5,8,3,9\"/><reel id=\"2\" length=\"13\" symbols=\"10,3,2,8,1,11,6,4,7,5,12,13,9\"/><reel id=\"3\" length=\"13\" symbols=\"10,3,4,6,1,2,5,12,13,8,11,9,7\"/><reel id=\"4\" length=\"13\" symbols=\"10,4,3,2,5,11,6,12,13,8,1,7,9\"/><reel id=\"5\" length=\"13\" symbols=\"10,1,2,6,12,13,11,5,4,7,3,9,8\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"11\" symbols=\"10,1,6,2,11,7,4,5,8,3,9\"/><reel id=\"2\" length=\"11\" symbols=\"10,3,2,8,1,11,6,7,5,9,4\"/><reel id=\"3\" length=\"11\" symbols=\"10,3,4,6,1,2,8,11,5,9,7\"/><reel id=\"4\" length=\"11\" symbols=\"10,4,3,2,5,11,6,8,1,7,9\"/><reel id=\"5\" length=\"11\" symbols=\"10,1,2,6,11,5,7,3,9,4,8\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"10,10,10,10\" reel2=\"7,7,7,2\" reel3=\"11,3,3,3\" reel4=\"2,2,12,12\" reel5=\"10,10,10,10\"><bonus bonus_pos=\"13,18\" bonus_tb=\"3,8\"/></shift><game jackpots_tb=\"20,50,150,3000\" init_reel_multipliers=\"1,1,1,1,1\" bonus_spins=\"3\" total_bet_mult=\"10\"/><delivery id=\"447616-3463675452421242441157142\" action=\"create\"/></server>";
            }
        }
        #endregion
        public LionGemsGameLogic()
        {
            _gameID = GAMEID.LionGems;
            GameName = "LionGems";
        }
    }
}
