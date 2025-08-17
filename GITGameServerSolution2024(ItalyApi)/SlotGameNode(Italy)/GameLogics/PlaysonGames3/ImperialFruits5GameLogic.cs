using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class ImperialFruits5GameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "imperial_fruits_5";
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 4, 6, 8, 10, 15, 20, 30, 40, 50, 75, 100, 200, 300, 500, 750, 1000, 2000 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><source game-ver=\"220119\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"wild\" count=\"3\" coef=\"0\"/><combination symbol=\"1\" name=\"wild\" count=\"4\" coef=\"0\"/><combination symbol=\"1\" name=\"wild\" count=\"5\" coef=\"0\"/><combination symbol=\"2\" name=\"seven\" count=\"2\" coef=\"10\"/><combination symbol=\"2\" name=\"seven\" count=\"3\" coef=\"40\"/><combination symbol=\"2\" name=\"seven\" count=\"4\" coef=\"200\"/><combination symbol=\"2\" name=\"seven\" count=\"5\" coef=\"2000\"/><combination symbol=\"3\" name=\"melon\" count=\"3\" coef=\"30\"/><combination symbol=\"3\" name=\"melon\" count=\"4\" coef=\"100\"/><combination symbol=\"3\" name=\"melon\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"grapes\" count=\"3\" coef=\"30\"/><combination symbol=\"4\" name=\"grapes\" count=\"4\" coef=\"100\"/><combination symbol=\"4\" name=\"grapes\" count=\"5\" coef=\"500\"/><combination symbol=\"5\" name=\"pear\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"pear\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"pear\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"plum\" count=\"3\" coef=\"10\"/><combination symbol=\"6\" name=\"plum\" count=\"4\" coef=\"25\"/><combination symbol=\"6\" name=\"plum\" count=\"5\" coef=\"100\"/><combination symbol=\"7\" name=\"orange\" count=\"3\" coef=\"10\"/><combination symbol=\"7\" name=\"orange\" count=\"4\" coef=\"25\"/><combination symbol=\"7\" name=\"orange\" count=\"5\" coef=\"100\"/><combination symbol=\"8\" name=\"lemon\" count=\"3\" coef=\"10\"/><combination symbol=\"8\" name=\"lemon\" count=\"4\" coef=\"25\"/><combination symbol=\"8\" name=\"lemon\" count=\"5\" coef=\"100\"/><combination symbol=\"9\" name=\"cherry\" count=\"3\" coef=\"10\"/><combination symbol=\"9\" name=\"cherry\" count=\"4\" coef=\"25\"/><combination symbol=\"9\" name=\"cherry\" count=\"5\" coef=\"100\"/><combination symbol=\"10\" name=\"disp\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"11\" name=\"scatter\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"3\"/><combination symbol=\"11\" name=\"scatter\" count=\"4\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"11\" name=\"scatter\" count=\"5\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"100\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/></paylines><symbols><symbol id=\"1\" title=\"wild\"/><symbol id=\"2\" title=\"seven\"/><symbol id=\"3\" title=\"melon\"/><symbol id=\"4\" title=\"grapes\"/><symbol id=\"5\" title=\"pear\"/><symbol id=\"6\" title=\"plum\"/><symbol id=\"7\" title=\"orange\"/><symbol id=\"8\" title=\"lemon\"/><symbol id=\"9\" title=\"cherry\"/><symbol id=\"10\" title=\"disp\"/><symbol id=\"11\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"2,5,7,6,9,8,11,4,10,3\"/><reel id=\"2\" length=\"10\" symbols=\"1,5,4,9,3,2,7,8,11,6\"/><reel id=\"3\" length=\"11\" symbols=\"1,9,3,4,11,7,5,2,8,6,10\"/><reel id=\"4\" length=\"10\" symbols=\"8,5,7,4,6,1,3,11,9,2\"/><reel id=\"5\" length=\"10\" symbols=\"10,4,2,9,3,5,6,7,8,11\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"9,9,4\" reel2=\"8,5,9\" reel3=\"8,3,1\" reel4=\"6,1,8\" reel5=\"4,7,7\"/><delivery id=\"1897629-968058886047567786481459\" action=\"create\"/></server>";
            }
        }
        #endregion

        public ImperialFruits5GameLogic()
        {
            _gameID     = GAMEID.ImperialFruits5;
            GameName    = "ImperialFruits5";
        }
    }
}
