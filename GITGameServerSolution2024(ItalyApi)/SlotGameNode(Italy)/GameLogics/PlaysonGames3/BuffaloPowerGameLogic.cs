using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BuffaloPowerGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "buffalo_power";
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
                return "<server><source game-ver=\"100620\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low_4\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"low_4\" count=\"4\" coef=\"10\"/><combination symbol=\"1\" name=\"low_4\" count=\"5\" coef=\"50\"/><combination symbol=\"2\" name=\"low_3\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"low_3\" count=\"4\" coef=\"10\"/><combination symbol=\"2\" name=\"low_3\" count=\"5\" coef=\"50\"/><combination symbol=\"3\" name=\"low_2\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"low_2\" count=\"4\" coef=\"10\"/><combination symbol=\"3\" name=\"low_2\" count=\"5\" coef=\"50\"/><combination symbol=\"4\" name=\"low_1\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"low_1\" count=\"4\" coef=\"10\"/><combination symbol=\"4\" name=\"low_1\" count=\"5\" coef=\"50\"/><combination symbol=\"5\" name=\"mid_3\" count=\"3\" coef=\"5\"/><combination symbol=\"5\" name=\"mid_3\" count=\"4\" coef=\"25\"/><combination symbol=\"5\" name=\"mid_3\" count=\"5\" coef=\"100\"/><combination symbol=\"6\" name=\"mid_2\" count=\"3\" coef=\"5\"/><combination symbol=\"6\" name=\"mid_2\" count=\"4\" coef=\"30\"/><combination symbol=\"6\" name=\"mid_2\" count=\"5\" coef=\"150\"/><combination symbol=\"7\" name=\"mid_1\" count=\"3\" coef=\"5\"/><combination symbol=\"7\" name=\"mid_1\" count=\"4\" coef=\"40\"/><combination symbol=\"7\" name=\"mid_1\" count=\"5\" coef=\"200\"/><combination symbol=\"8\" name=\"top\" count=\"3\" coef=\"15\"/><combination symbol=\"8\" name=\"top\" count=\"4\" coef=\"50\"/><combination symbol=\"8\" name=\"top\" count=\"5\" coef=\"250\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"15\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"50\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"250\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"4\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"5\" coef=\"0\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"2,3,2,1,2\"/><payline id=\"11\" path=\"2,1,2,3,2\"/><payline id=\"12\" path=\"1,2,2,2,1\"/><payline id=\"13\" path=\"3,2,2,2,3\"/><payline id=\"14\" path=\"1,2,1,2,1\"/><payline id=\"15\" path=\"3,2,3,2,3\"/><payline id=\"16\" path=\"2,2,1,2,2\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"1,1,3,1,1\"/><payline id=\"19\" path=\"3,3,1,3,3\"/><payline id=\"20\" path=\"1,3,3,3,1\"/></paylines><symbols><symbol id=\"1\" title=\"low_4\"/><symbol id=\"2\" title=\"low_3\"/><symbol id=\"3\" title=\"low_2\"/><symbol id=\"4\" title=\"low_1\"/><symbol id=\"5\" title=\"mid_3\"/><symbol id=\"6\" title=\"mid_2\"/><symbol id=\"7\" title=\"mid_1\"/><symbol id=\"8\" title=\"top\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"scatter\"/><symbol id=\"11\" title=\"bonus\"/><symbol id=\"12\" title=\"jackpot\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"6,5,1,11,2,9,4,7,8,3\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,7,2,10,6,3,5,11,4\"/><reel id=\"3\" length=\"11\" symbols=\"7,5,9,6,11,3,10,4,8,2,1\"/><reel id=\"4\" length=\"11\" symbols=\"8,5,10,1,6,11,4,9,7,2,3\"/><reel id=\"5\" length=\"10\" symbols=\"8,1,5,3,9,7,4,2,6,11\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"6,5,1,11,2,9,4,7,8,3\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,7,2,10,6,3,5,11,4\"/><reel id=\"3\" length=\"11\" symbols=\"7,5,9,6,11,3,10,4,8,2,1\"/><reel id=\"4\" length=\"11\" symbols=\"8,5,10,1,6,11,4,9,7,2,3\"/><reel id=\"5\" length=\"10\" symbols=\"8,1,5,3,9,7,4,2,6,11\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"10\" symbols=\"6,5,1,11,2,9,7,8,3,4\"/><reel id=\"2\" length=\"10\" symbols=\"9,1,7,8,3,6,5,11,4,2\"/><reel id=\"3\" length=\"10\" symbols=\"5,8,6,9,7,11,3,4,2,1\"/><reel id=\"4\" length=\"10\" symbols=\"8,5,2,6,1,11,4,9,3,7\"/><reel id=\"5\" length=\"10\" symbols=\"8,1,5,3,9,7,6,4,11,2\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"9\" symbols=\"6,5,1,2,9,7,8,3,4\"/><reel id=\"2\" length=\"9\" symbols=\"9,1,7,8,3,6,5,4,2\"/><reel id=\"3\" length=\"9\" symbols=\"5,8,6,9,7,3,4,2,1\"/><reel id=\"4\" length=\"9\" symbols=\"8,5,2,6,1,4,9,3,7\"/><reel id=\"5\" length=\"9\" symbols=\"8,1,5,3,9,7,6,4,2\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"1,11,11\" reel2=\"6,9,9\" reel3=\"3,10,4\" reel4=\"5,6,8\" reel5=\"2,11,11\"><bonus bonus_pos=\"5,10,9,14\" bonus_tb=\"1,5,4,10\" bonus_type=\"0,0,0,0\"/></shift><shift_ext><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"2,7,8\" reel2=\"6,5,4\" reel3=\"6,9,9\" reel4=\"5,6,8\" reel5=\"8,1,1\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"5,6,5\" reel2=\"2,2,8\" reel3=\"6,9,9\" reel4=\"5,6,8\" reel5=\"6,2,8\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"8,4,4\" reel2=\"1,7,3\" reel3=\"6,9,9\" reel4=\"3,6,2\" reel5=\"8,5,7\"/></shift_ext><game jackpots_tb=\"5,20,50,150,500\" bonus_spins=\"3\"/><delivery id=\"1171125-098521528413306480744176\" action=\"create\"/></server>";
            }
        }
        #endregion

        public BuffaloPowerGameLogic()
        {
            _gameID     = GAMEID.BuffaloPower;
            GameName    = "BuffaloPower";
        }
    }
}
