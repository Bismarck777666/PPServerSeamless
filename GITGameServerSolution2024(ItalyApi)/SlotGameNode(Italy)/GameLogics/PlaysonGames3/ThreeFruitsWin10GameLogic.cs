using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class ThreeFruitsWin10GameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "3_fruits_win_10";
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
                return "<server><source game-ver=\"250319\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"7\" count=\"3\" coef=\"50\"/><combination symbol=\"1\" name=\"7\" count=\"4\" coef=\"500\"/><combination symbol=\"1\" name=\"7\" count=\"5\" coef=\"2500\"/><combination symbol=\"2\" name=\"melon\" count=\"3\" coef=\"30\"/><combination symbol=\"2\" name=\"melon\" count=\"4\" coef=\"150\"/><combination symbol=\"2\" name=\"melon\" count=\"5\" coef=\"500\"/><combination symbol=\"3\" name=\"grapes\" count=\"3\" coef=\"30\"/><combination symbol=\"3\" name=\"grapes\" count=\"4\" coef=\"150\"/><combination symbol=\"3\" name=\"grapes\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"plum\" count=\"3\" coef=\"15\"/><combination symbol=\"4\" name=\"plum\" count=\"4\" coef=\"50\"/><combination symbol=\"4\" name=\"plum\" count=\"5\" coef=\"200\"/><combination symbol=\"5\" name=\"orange\" count=\"3\" coef=\"15\"/><combination symbol=\"5\" name=\"orange\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"orange\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"lemon\" count=\"3\" coef=\"10\"/><combination symbol=\"6\" name=\"lemon\" count=\"4\" coef=\"25\"/><combination symbol=\"6\" name=\"lemon\" count=\"5\" coef=\"150\"/><combination symbol=\"7\" name=\"cherry\" count=\"3\" coef=\"10\"/><combination symbol=\"7\" name=\"cherry\" count=\"4\" coef=\"25\"/><combination symbol=\"7\" name=\"cherry\" count=\"5\" coef=\"150\"/><combination symbol=\"8\" name=\"disp\" count=\"3\" coef=\"2\"/><combination symbol=\"8\" name=\"disp\" count=\"4\" coef=\"10\"/><combination symbol=\"8\" name=\"disp\" count=\"5\" coef=\"50\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"7\"/><symbol id=\"2\" title=\"melon\"/><symbol id=\"3\" title=\"grapes\"/><symbol id=\"4\" title=\"plum\"/><symbol id=\"5\" title=\"orange\"/><symbol id=\"6\" title=\"lemon\"/><symbol id=\"7\" title=\"cherry\"/><symbol id=\"8\" title=\"disp\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"8\" symbols=\"1,7,3,4,6,8,5,2\"/><reel id=\"2\" length=\"8\" symbols=\"1,5,2,8,6,4,7,3\"/><reel id=\"3\" length=\"8\" symbols=\"1,6,2,5,8,4,3,7\"/><reel id=\"4\" length=\"8\" symbols=\"5,1,4,3,7,6,8,2\"/><reel id=\"5\" length=\"8\" symbols=\"7,2,4,8,5,3,6,1\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"5,1,1\" reel2=\"8,4,4\" reel3=\"4,4,4\" reel4=\"4,4,4\" reel5=\"8,5,5\"/><delivery id=\"1781949-135394971870554518972437\" action=\"create\"/></server>";
            }
        }
        #endregion
        public ThreeFruitsWin10GameLogic()
        {
            _gameID = GAMEID.ThreeFruitsWin10;
            GameName = "ThreeFruitsWin10";
        }
    }
}
