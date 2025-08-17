using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class DiamondWinsGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "diamond_wins";
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
                return "<server><source game-ver=\"200720\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low_4\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"low_4\" count=\"4\" coef=\"15\"/><combination symbol=\"1\" name=\"low_4\" count=\"5\" coef=\"100\"/><combination symbol=\"2\" name=\"low_3\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"low_3\" count=\"4\" coef=\"15\"/><combination symbol=\"2\" name=\"low_3\" count=\"5\" coef=\"100\"/><combination symbol=\"3\" name=\"low_2\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"low_2\" count=\"4\" coef=\"15\"/><combination symbol=\"3\" name=\"low_2\" count=\"5\" coef=\"100\"/><combination symbol=\"4\" name=\"low_1\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"low_1\" count=\"4\" coef=\"15\"/><combination symbol=\"4\" name=\"low_1\" count=\"5\" coef=\"100\"/><combination symbol=\"5\" name=\"mid_3\" count=\"3\" coef=\"10\"/><combination symbol=\"5\" name=\"mid_3\" count=\"4\" coef=\"25\"/><combination symbol=\"5\" name=\"mid_3\" count=\"5\" coef=\"250\"/><combination symbol=\"6\" name=\"mid_2\" count=\"3\" coef=\"10\"/><combination symbol=\"6\" name=\"mid_2\" count=\"4\" coef=\"25\"/><combination symbol=\"6\" name=\"mid_2\" count=\"5\" coef=\"250\"/><combination symbol=\"7\" name=\"mid_1\" count=\"3\" coef=\"15\"/><combination symbol=\"7\" name=\"mid_1\" count=\"4\" coef=\"40\"/><combination symbol=\"7\" name=\"mid_1\" count=\"5\" coef=\"500\"/><combination symbol=\"8\" name=\"top\" count=\"3\" coef=\"20\"/><combination symbol=\"8\" name=\"top\" count=\"4\" coef=\"50\"/><combination symbol=\"8\" name=\"top\" count=\"5\" coef=\"1000\"/><combination symbol=\"10\" name=\"bonus\" count=\"3\" coef=\"0\"/><combination symbol=\"10\" name=\"bonus\" count=\"4\" coef=\"0\"/><combination symbol=\"10\" name=\"bonus\" count=\"5\" coef=\"0\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"low_4\"/><symbol id=\"2\" title=\"low_3\"/><symbol id=\"3\" title=\"low_2\"/><symbol id=\"4\" title=\"low_1\"/><symbol id=\"5\" title=\"mid_3\"/><symbol id=\"6\" title=\"mid_2\"/><symbol id=\"7\" title=\"mid_1\"/><symbol id=\"8\" title=\"top\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"bonus\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"9\" symbols=\"6,5,1,10,2,4,7,8,3\"/><reel id=\"2\" length=\"10\" symbols=\"9,1,8,7,2,6,3,5,10,4\"/><reel id=\"3\" length=\"10\" symbols=\"5,8,9,7,3,10,6,4,2,1\"/><reel id=\"4\" length=\"10\" symbols=\"8,9,5,1,6,10,4,7,2,3\"/><reel id=\"5\" length=\"10\" symbols=\"9,8,1,5,3,7,4,2,6,10\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"9\" symbols=\"6,5,1,10,2,4,7,8,3\"/><reel id=\"2\" length=\"10\" symbols=\"9,1,8,7,2,6,3,5,10,4\"/><reel id=\"3\" length=\"10\" symbols=\"5,8,9,7,3,10,6,4,2,1\"/><reel id=\"4\" length=\"10\" symbols=\"8,9,5,1,6,10,4,7,2,3\"/><reel id=\"5\" length=\"10\" symbols=\"9,8,1,5,3,7,4,2,6,10\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"8,4,7\" reel2=\"5,10,10\" reel3=\"3,7,1\" reel4=\"8,9,9\" reel5=\"4,4,10\"><bonus bonus_pos=\"6,11,14\" bonus_tb=\"15,3,6\"/></shift><game jackpots_tb=\"25,100,1000\" bonus_spins=\"3\"/><delivery id=\"1147200-726581915746856638294308\" action=\"create\"/></server>";
            }
        }
        #endregion

        public DiamondWinsGameLogic()
        {
            _gameID     = GAMEID.DiamondWins;
            GameName    = "DiamondWins";
        }
    }
}
