using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BurningFortGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "burning_fort";
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 4, 5, 6, 8, 10, 15, 20, 30, 40, 50, 75, 100, 200, 300, 500, 750, 1000, 2000 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><source game-ver=\"231221\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"seven\" count=\"3\" coef=\"300\"/><combination symbol=\"2\" name=\"bell\" count=\"3\" coef=\"200\"/><combination symbol=\"3\" name=\"dice\" count=\"3\" coef=\"100\"/><combination symbol=\"4\" name=\"bar1\" count=\"3\" coef=\"75\"/><combination symbol=\"5\" name=\"bar2\" count=\"3\" coef=\"75\"/><combination symbol=\"6\" name=\"spades\" count=\"3\" coef=\"35\"/><combination symbol=\"7\" name=\"clubs\" count=\"3\" coef=\"35\"/><combination symbol=\"8\" name=\"diamonds\" count=\"3\" coef=\"35\"/><combination symbol=\"9\" name=\"hearts\" count=\"3\" coef=\"35\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1\"/><payline id=\"2\" path=\"2,2,2\"/><payline id=\"3\" path=\"3,3,3\"/><payline id=\"4\" path=\"1,2,3\"/><payline id=\"5\" path=\"3,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"seven\"/><symbol id=\"2\" title=\"bell\"/><symbol id=\"3\" title=\"dice\"/><symbol id=\"4\" title=\"bar1\"/><symbol id=\"5\" title=\"bar2\"/><symbol id=\"6\" title=\"spades\"/><symbol id=\"7\" title=\"clubs\"/><symbol id=\"8\" title=\"diamonds\"/><symbol id=\"9\" title=\"hearts\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"9\" symbols=\"9,2,6,4,8,5,7,1,3\"/><reel id=\"2\" length=\"9\" symbols=\"9,2,6,4,8,5,7,1,3\"/><reel id=\"3\" length=\"9\" symbols=\"9,2,6,4,8,5,7,1,3\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"9\" symbols=\"9,2,6,4,8,5,7,1,3\"/><reel id=\"2\" length=\"9\" symbols=\"9,2,6,4,8,5,7,1,3\"/><reel id=\"3\" length=\"9\" symbols=\"9,2,6,4,8,5,7,1,3\"/></reels></slot><shift server=\"0,0,0\" reel_set=\"1\" reel1=\"6,4,8\" reel2=\"9,9,2\" reel3=\"7,1,5\"/><game total_bet_mult=\"5\"/><delivery id=\"447811-5122977059762073978539608\" action=\"create\"/></server>";
            }
        }
        #endregion
        public BurningFortGameLogic()
        {
            _gameID = GAMEID.BurningFort;
            GameName = "BurningFort";
        }
    }
}
