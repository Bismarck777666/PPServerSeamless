using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class ThreeFruitsWin2HitGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "3_fruits_win_double_hit";
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
                return "<server><source game-ver=\"230221\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"seven\" count=\"3\" coef=\"50\"/><combination symbol=\"1\" name=\"seven\" count=\"4\" coef=\"500\"/><combination symbol=\"1\" name=\"seven\" count=\"5\" coef=\"2500\"/><combination symbol=\"2\" name=\"melon\" count=\"3\" coef=\"30\"/><combination symbol=\"2\" name=\"melon\" count=\"4\" coef=\"150\"/><combination symbol=\"2\" name=\"melon\" count=\"5\" coef=\"500\"/><combination symbol=\"3\" name=\"grapes\" count=\"3\" coef=\"30\"/><combination symbol=\"3\" name=\"grapes\" count=\"4\" coef=\"150\"/><combination symbol=\"3\" name=\"grapes\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"plum\" count=\"3\" coef=\"15\"/><combination symbol=\"4\" name=\"plum\" count=\"4\" coef=\"50\"/><combination symbol=\"4\" name=\"plum\" count=\"5\" coef=\"200\"/><combination symbol=\"5\" name=\"orange\" count=\"3\" coef=\"15\"/><combination symbol=\"5\" name=\"orange\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"orange\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"lemon\" count=\"3\" coef=\"10\"/><combination symbol=\"6\" name=\"lemon\" count=\"4\" coef=\"25\"/><combination symbol=\"6\" name=\"lemon\" count=\"5\" coef=\"150\"/><combination symbol=\"7\" name=\"cherry\" count=\"3\" coef=\"10\"/><combination symbol=\"7\" name=\"cherry\" count=\"4\" coef=\"25\"/><combination symbol=\"7\" name=\"cherry\" count=\"5\" coef=\"150\"/><combination symbol=\"8\" name=\"scatter\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"1\"/><combination symbol=\"8\" name=\"scatter\" count=\"4\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"5\"/><combination symbol=\"8\" name=\"scatter\" count=\"5\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"15\"/><combination symbol=\"8\" name=\"scatter\" count=\"6\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"40\"/><combination symbol=\"8\" name=\"scatter\" count=\"7\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"100\"/><combination symbol=\"8\" name=\"scatter\" count=\"8\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"500\"/><combination symbol=\"8\" name=\"scatter\" count=\"9\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"2000\"/><combination symbol=\"8\" name=\"scatter\" count=\"10\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"5000\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/></paylines><symbols><symbol id=\"1\" title=\"seven\"/><symbol id=\"2\" title=\"melon\"/><symbol id=\"3\" title=\"grapes\"/><symbol id=\"4\" title=\"plum\"/><symbol id=\"5\" title=\"orange\"/><symbol id=\"6\" title=\"lemon\"/><symbol id=\"7\" title=\"cherry\"/><symbol id=\"8\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"7\" symbols=\"1,7,3,4,6,5,2\"/><reel id=\"2\" length=\"7\" symbols=\"1,5,2,6,4,7,3\"/><reel id=\"3\" length=\"7\" symbols=\"1,6,2,5,4,3,7\"/><reel id=\"4\" length=\"7\" symbols=\"5,1,4,3,7,6,2\"/><reel id=\"5\" length=\"7\" symbols=\"7,2,4,5,3,6,1\"/><reel id=\"6\" length=\"7\" symbols=\"1,7,3,4,6,5,2\"/><reel id=\"7\" length=\"7\" symbols=\"1,5,2,6,4,7,3\"/><reel id=\"8\" length=\"7\" symbols=\"1,6,2,5,4,3,7\"/><reel id=\"9\" length=\"7\" symbols=\"5,1,4,3,7,6,2\"/><reel id=\"10\" length=\"7\" symbols=\"7,2,4,5,3,6,1\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"7\" symbols=\"1,7,3,4,6,5,2\"/><reel id=\"2\" length=\"7\" symbols=\"1,5,2,6,4,7,3\"/><reel id=\"3\" length=\"7\" symbols=\"1,6,2,5,4,3,7\"/><reel id=\"4\" length=\"7\" symbols=\"5,1,4,3,7,6,2\"/><reel id=\"5\" length=\"7\" symbols=\"7,2,4,5,3,6,1\"/><reel id=\"6\" length=\"7\" symbols=\"1,7,3,4,6,5,2\"/><reel id=\"7\" length=\"7\" symbols=\"1,5,2,6,4,7,3\"/><reel id=\"8\" length=\"7\" symbols=\"1,6,2,5,4,3,7\"/><reel id=\"9\" length=\"7\" symbols=\"5,1,4,3,7,6,2\"/><reel id=\"10\" length=\"7\" symbols=\"7,2,4,5,3,6,1\"/></reels></slot><shift server=\"0,0,0,0,0,0,0,0,0,0\" reel_set=\"1\" reel1=\"1,7,7\" reel2=\"7,8,5\" reel3=\"6,6,2\" reel4=\"4,4,3\" reel5=\"2,2,4\" reel6=\"3,4,4\" reel7=\"1,5,5\" reel8=\"7,7,1\" reel9=\"5,8,5\" reel10=\"2,2,4\"/><game total_bet_mult=\"20\" visible_fields=\"2\"/><delivery id=\"497494-1594066900293411890268861\" action=\"create\"/></server>";
            }
        }
        #endregion
        public ThreeFruitsWin2HitGameLogic()
        {
            _gameID = GAMEID.ThreeFruitsWin2Hit;
            GameName = "ThreeFruitsWin2Hit";
        }
    }
}
