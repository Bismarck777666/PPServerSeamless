using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class JokerExpand40GameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "joker_expand_40";
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
                return "<server><source game-ver=\"20718\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"wild\" count=\"3\" coef=\"0\"/><combination symbol=\"1\" name=\"wild\" count=\"4\" coef=\"0\"/><combination symbol=\"1\" name=\"wild\" count=\"5\" coef=\"0\"/><combination symbol=\"2\" name=\"seven\" count=\"2\" coef=\"5\"/><combination symbol=\"2\" name=\"seven\" count=\"3\" coef=\"40\"/><combination symbol=\"2\" name=\"seven\" count=\"4\" coef=\"200\"/><combination symbol=\"2\" name=\"seven\" count=\"5\" coef=\"2000\"/><combination symbol=\"3\" name=\"melon\" count=\"3\" coef=\"20\"/><combination symbol=\"3\" name=\"melon\" count=\"4\" coef=\"100\"/><combination symbol=\"3\" name=\"melon\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"grapes\" count=\"3\" coef=\"20\"/><combination symbol=\"4\" name=\"grapes\" count=\"4\" coef=\"100\"/><combination symbol=\"4\" name=\"grapes\" count=\"5\" coef=\"500\"/><combination symbol=\"5\" name=\"pear\" count=\"3\" coef=\"10\"/><combination symbol=\"5\" name=\"pear\" count=\"4\" coef=\"40\"/><combination symbol=\"5\" name=\"pear\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"plum\" count=\"3\" coef=\"5\"/><combination symbol=\"6\" name=\"plum\" count=\"4\" coef=\"10\"/><combination symbol=\"6\" name=\"plum\" count=\"5\" coef=\"50\"/><combination symbol=\"7\" name=\"orange\" count=\"3\" coef=\"5\"/><combination symbol=\"7\" name=\"orange\" count=\"4\" coef=\"10\"/><combination symbol=\"7\" name=\"orange\" count=\"5\" coef=\"50\"/><combination symbol=\"8\" name=\"lemon\" count=\"3\" coef=\"5\"/><combination symbol=\"8\" name=\"lemon\" count=\"4\" coef=\"10\"/><combination symbol=\"8\" name=\"lemon\" count=\"5\" coef=\"50\"/><combination symbol=\"9\" name=\"cherry\" count=\"3\" coef=\"5\"/><combination symbol=\"9\" name=\"cherry\" count=\"4\" coef=\"10\"/><combination symbol=\"9\" name=\"cherry\" count=\"5\" coef=\"50\"/><combination symbol=\"10\" name=\"disp\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"11\" name=\"scatter\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"3\"/><combination symbol=\"11\" name=\"scatter\" count=\"4\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"11\" name=\"scatter\" count=\"5\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"100\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"1,2,2,2,2\"/><payline id=\"3\" path=\"1,1,1,1,2\"/><payline id=\"4\" path=\"1,1,1,2,3\"/><payline id=\"5\" path=\"1,2,3,2,1\"/><payline id=\"6\" path=\"1,1,2,1,1\"/><payline id=\"7\" path=\"1,2,2,2,1\"/><payline id=\"8\" path=\"1,2,3,3,3\"/><payline id=\"9\" path=\"2,2,2,2,1\"/><payline id=\"10\" path=\"2,1,1,1,2\"/><payline id=\"11\" path=\"2,2,1,2,2\"/><payline id=\"12\" path=\"2,3,3,3,3\"/><payline id=\"13\" path=\"2,2,2,2,2\"/><payline id=\"14\" path=\"2,1,1,1,1\"/><payline id=\"15\" path=\"2,2,2,2,3\"/><payline id=\"16\" path=\"2,3,4,4,4\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"2,2,2,3,4\"/><payline id=\"19\" path=\"2,3,4,3,2\"/><payline id=\"20\" path=\"2,3,3,3,2\"/><payline id=\"21\" path=\"3,3,3,2,1\"/><payline id=\"22\" path=\"3,2,2,2,3\"/><payline id=\"23\" path=\"3,3,2,3,3\"/><payline id=\"24\" path=\"3,2,1,2,3\"/><payline id=\"25\" path=\"3,3,3,3,2\"/><payline id=\"26\" path=\"3,2,1,1,1\"/><payline id=\"27\" path=\"3,3,3,3,3\"/><payline id=\"28\" path=\"3,2,2,2,2\"/><payline id=\"29\" path=\"3,3,4,3,3\"/><payline id=\"30\" path=\"3,4,4,4,4\"/><payline id=\"31\" path=\"3,3,3,3,4\"/><payline id=\"32\" path=\"3,4,4,4,3\"/><payline id=\"33\" path=\"4,4,3,4,4\"/><payline id=\"34\" path=\"4,3,3,3,4\"/><payline id=\"35\" path=\"4,4,4,3,2\"/><payline id=\"36\" path=\"4,3,2,3,4\"/><payline id=\"37\" path=\"4,4,4,4,3\"/><payline id=\"38\" path=\"4,3,3,3,3\"/><payline id=\"39\" path=\"4,3,2,2,2\"/><payline id=\"40\" path=\"4,4,4,4,4\"/></paylines><symbols><symbol id=\"1\" title=\"wild\"/><symbol id=\"2\" title=\"seven\"/><symbol id=\"3\" title=\"melon\"/><symbol id=\"4\" title=\"grapes\"/><symbol id=\"5\" title=\"pear\"/><symbol id=\"6\" title=\"plum\"/><symbol id=\"7\" title=\"orange\"/><symbol id=\"8\" title=\"lemon\"/><symbol id=\"9\" title=\"cherry\"/><symbol id=\"10\" title=\"disp\"/><symbol id=\"11\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"2,5,3,7,4,6,8,11,9,10\"/><reel id=\"2\" length=\"10\" symbols=\"1,3,5,4,9,7,6,2,8,11\"/><reel id=\"3\" length=\"11\" symbols=\"1,2,6,3,11,4,10,5,9,7,8\"/><reel id=\"4\" length=\"10\" symbols=\"8,5,4,1,3,11,9,2,7,6\"/><reel id=\"5\" length=\"10\" symbols=\"10,4,2,5,9,3,6,8,7,11\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"10,4,5,6\" reel2=\"6,6,6,3\" reel3=\"2,7,7,6\" reel4=\"7,7,7,7\" reel5=\"5,3,10,4\"/><delivery id=\"1903501-354103480286223336931833\" action=\"create\"/></server>";
            }
        }
        #endregion
        public JokerExpand40GameLogic()
        {
            _gameID = GAMEID.JokerExpand40;
            GameName = "JokerExpand40";
        }
    }
}
