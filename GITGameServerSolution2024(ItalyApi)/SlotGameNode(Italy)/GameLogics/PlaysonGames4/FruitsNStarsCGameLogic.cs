using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class FruitsNStarsCGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "fruits_n_stars_c";
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
                return "<server><source game-ver=\"56175\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"7\" count=\"3\" coef=\"100\"/><combination symbol=\"1\" name=\"7\" count=\"4\" coef=\"1000\"/><combination symbol=\"1\" name=\"7\" count=\"5\" coef=\"5000\"/><combination symbol=\"2\" name=\"melon\" count=\"3\" coef=\"50\"/><combination symbol=\"2\" name=\"melon\" count=\"4\" coef=\"200\"/><combination symbol=\"2\" name=\"melon\" count=\"5\" coef=\"500\"/><combination symbol=\"3\" name=\"grapes\" count=\"3\" coef=\"50\"/><combination symbol=\"3\" name=\"grapes\" count=\"4\" coef=\"200\"/><combination symbol=\"3\" name=\"grapes\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"plum\" count=\"3\" coef=\"20\"/><combination symbol=\"4\" name=\"plum\" count=\"4\" coef=\"50\"/><combination symbol=\"4\" name=\"plum\" count=\"5\" coef=\"200\"/><combination symbol=\"5\" name=\"orange\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"orange\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"orange\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"lemon\" count=\"3\" coef=\"20\"/><combination symbol=\"6\" name=\"lemon\" count=\"4\" coef=\"50\"/><combination symbol=\"6\" name=\"lemon\" count=\"5\" coef=\"200\"/><combination symbol=\"7\" name=\"cherry\" count=\"2\" coef=\"5\"/><combination symbol=\"7\" name=\"cherry\" count=\"3\" coef=\"20\"/><combination symbol=\"7\" name=\"cherry\" count=\"4\" coef=\"50\"/><combination symbol=\"7\" name=\"cherry\" count=\"5\" coef=\"200\"/><combination symbol=\"8\" name=\"disp\" count=\"3\" coef=\"2\"/><combination symbol=\"8\" name=\"disp\" count=\"4\" coef=\"10\"/><combination symbol=\"8\" name=\"disp\" count=\"5\" coef=\"50\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/></paylines><symbols><symbol id=\"1\" title=\"7\"/><symbol id=\"2\" title=\"melon\"/><symbol id=\"3\" title=\"grapes\"/><symbol id=\"4\" title=\"plum\"/><symbol id=\"5\" title=\"orange\"/><symbol id=\"6\" title=\"lemon\"/><symbol id=\"7\" title=\"cherry\"/><symbol id=\"8\" title=\"disp\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"99\" symbols=\"2,3,5,5,4,4,5,1,4,6,6,5,6,8,5,2,2,2,1,4,4,5,5,5,6,4,7,7,4,4,8,3,3,3,7,3,3,1,6,7,7,6,6,6,1,2,2,5,5,3,1,7,7,7,1,4,3,3,2,2,8,2,6,6,5,5,6,3,3,1,7,3,4,4,8,7,2,6,6,7,7,2,2,3,5,5,2,3,3,4,4,4,2,2,1,6,6,4,5\"/><reel id=\"2\" length=\"99\" symbols=\"5,5,2,1,5,5,4,8,6,2,5,6,6,6,5,5,3,3,8,6,3,3,2,2,1,5,4,4,1,3,6,6,1,7,7,5,3,3,1,7,3,3,8,4,7,2,2,5,7,7,7,5,5,2,2,1,7,7,4,5,5,3,3,6,6,1,3,4,6,6,1,4,4,5,6,7,4,4,2,3,2,2,7,7,2,4,4,8,2,3,6,4,3,4,4,2,2,6,6\"/><reel id=\"3\" length=\"98\" symbols=\"4,5,6,6,1,2,2,6,7,7,4,4,6,6,6,5,1,2,4,4,4,1,3,6,6,8,5,5,4,1,3,3,2,2,1,3,7,3,3,3,7,7,5,5,3,3,1,3,2,2,4,4,7,4,2,6,4,4,1,6,5,8,7,2,7,7,3,3,6,6,8,7,7,7,8,5,4,3,5,5,8,3,3,5,5,2,2,2,1,4,4,5,2,2,5,5,1,2\"/><reel id=\"4\" length=\"99\" symbols=\"5,5,4,4,3,4,6,3,8,5,7,7,4,3,3,2,3,5,5,5,8,7,3,3,1,5,5,2,2,4,4,5,5,6,6,4,7,7,8,3,3,7,7,2,3,3,5,1,4,4,6,4,2,1,2,2,6,6,5,8,2,2,6,7,4,4,6,6,1,4,2,2,2,3,1,7,7,7,5,6,6,5,5,7,1,6,3,2,2,4,4,1,3,3,6,6,1,6,2\"/><reel id=\"5\" length=\"97\" symbols=\"7,3,3,2,1,7,4,4,1,7,7,2,4,2,2,5,4,4,1,6,3,3,3,1,3,3,7,7,1,6,6,3,4,6,6,8,5,5,5,8,3,4,8,6,6,7,1,2,3,8,3,3,6,8,5,7,4,4,5,5,8,6,5,8,7,7,2,2,3,3,1,2,2,4,4,4,2,2,2,6,6,3,6,5,5,1,4,4,4,2,2,8,7,7,2,5,5\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"6,8,3\" reel2=\"7,2,1\" reel3=\"2,5,2\" reel4=\"4,1,4\" reel5=\"4,4,4\"/><delivery id=\"1909747-072636423077485659173431\" action=\"create\"/></server>";
            }
        }
        #endregion
        public FruitsNStarsCGameLogic()
        {
            _gameID = GAMEID.FruitsNStarsC;
            GameName = "FruitsNStarsC";
        }
    }
}
