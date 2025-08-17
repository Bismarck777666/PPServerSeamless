using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class FruitsAndJokers40GameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "fruits_and_jokers_40";
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
                return "<server><source game-ver=\"11018\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"joker\" count=\"3\" coef=\"25\"/><combination symbol=\"1\" name=\"joker\" count=\"4\" coef=\"250\"/><combination symbol=\"1\" name=\"joker\" count=\"5\" coef=\"750\"/><combination symbol=\"2\" name=\"seven\" count=\"3\" coef=\"10\"/><combination symbol=\"2\" name=\"seven\" count=\"4\" coef=\"50\"/><combination symbol=\"2\" name=\"seven\" count=\"5\" coef=\"150\"/><combination symbol=\"3\" name=\"melon\" count=\"3\" coef=\"10\"/><combination symbol=\"3\" name=\"melon\" count=\"4\" coef=\"30\"/><combination symbol=\"3\" name=\"melon\" count=\"5\" coef=\"90\"/><combination symbol=\"4\" name=\"grapes\" count=\"3\" coef=\"10\"/><combination symbol=\"4\" name=\"grapes\" count=\"4\" coef=\"30\"/><combination symbol=\"4\" name=\"grapes\" count=\"5\" coef=\"90\"/><combination symbol=\"5\" name=\"lemon\" count=\"3\" coef=\"5\"/><combination symbol=\"5\" name=\"lemon\" count=\"4\" coef=\"15\"/><combination symbol=\"5\" name=\"lemon\" count=\"5\" coef=\"45\"/><combination symbol=\"6\" name=\"orange\" count=\"3\" coef=\"5\"/><combination symbol=\"6\" name=\"orange\" count=\"4\" coef=\"15\"/><combination symbol=\"6\" name=\"orange\" count=\"5\" coef=\"45\"/><combination symbol=\"7\" name=\"cherry\" count=\"3\" coef=\"5\"/><combination symbol=\"7\" name=\"cherry\" count=\"4\" coef=\"15\"/><combination symbol=\"7\" name=\"cherry\" count=\"5\" coef=\"45\"/><combination symbol=\"8\" name=\"disp\" count=\"3\" coef=\"5\"/><combination symbol=\"8\" name=\"disp\" count=\"4\" coef=\"20\"/><combination symbol=\"8\" name=\"disp\" count=\"5\" coef=\"400\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"1,2,2,2,2\"/><payline id=\"3\" path=\"1,1,1,1,2\"/><payline id=\"4\" path=\"1,1,1,2,3\"/><payline id=\"5\" path=\"1,2,3,2,1\"/><payline id=\"6\" path=\"1,1,2,1,1\"/><payline id=\"7\" path=\"1,2,2,2,1\"/><payline id=\"8\" path=\"1,2,3,3,3\"/><payline id=\"9\" path=\"2,2,2,2,1\"/><payline id=\"10\" path=\"2,1,1,1,2\"/><payline id=\"11\" path=\"2,2,1,2,2\"/><payline id=\"12\" path=\"2,3,3,3,3\"/><payline id=\"13\" path=\"2,2,2,2,2\"/><payline id=\"14\" path=\"2,1,1,1,1\"/><payline id=\"15\" path=\"2,2,2,2,3\"/><payline id=\"16\" path=\"2,3,4,4,4\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"2,2,2,3,4\"/><payline id=\"19\" path=\"2,3,4,3,2\"/><payline id=\"20\" path=\"2,3,3,3,2\"/><payline id=\"21\" path=\"3,3,3,2,1\"/><payline id=\"22\" path=\"3,2,2,2,3\"/><payline id=\"23\" path=\"3,3,2,3,3\"/><payline id=\"24\" path=\"3,2,1,2,3\"/><payline id=\"25\" path=\"3,3,3,3,2\"/><payline id=\"26\" path=\"3,2,1,1,1\"/><payline id=\"27\" path=\"3,3,3,3,3\"/><payline id=\"28\" path=\"3,2,2,2,2\"/><payline id=\"29\" path=\"3,3,4,3,3\"/><payline id=\"30\" path=\"3,4,4,4,4\"/><payline id=\"31\" path=\"3,3,3,3,4\"/><payline id=\"32\" path=\"3,4,4,4,3\"/><payline id=\"33\" path=\"4,4,3,4,4\"/><payline id=\"34\" path=\"4,3,3,3,4\"/><payline id=\"35\" path=\"4,4,4,3,2\"/><payline id=\"36\" path=\"4,3,2,3,4\"/><payline id=\"37\" path=\"4,4,4,4,3\"/><payline id=\"38\" path=\"4,3,3,3,3\"/><payline id=\"39\" path=\"4,3,2,2,2\"/><payline id=\"40\" path=\"4,4,4,4,4\"/></paylines><symbols><symbol id=\"1\" title=\"joker\"/><symbol id=\"2\" title=\"seven\"/><symbol id=\"3\" title=\"melon\"/><symbol id=\"4\" title=\"grapes\"/><symbol id=\"5\" title=\"lemon\"/><symbol id=\"6\" title=\"orange\"/><symbol id=\"7\" title=\"cherry\"/><symbol id=\"8\" title=\"disp\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"8\" symbols=\"1,8,2,3,5,6,4,7\"/><reel id=\"2\" length=\"8\" symbols=\"1,7,2,3,8,6,5,4\"/><reel id=\"3\" length=\"8\" symbols=\"1,3,2,7,8,4,5,6\"/><reel id=\"4\" length=\"8\" symbols=\"1,2,3,5,4,7,6,8\"/><reel id=\"5\" length=\"8\" symbols=\"1,4,3,2,6,5,8,7\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"3,3,4,4\" reel2=\"6,8,2,2\" reel3=\"4,4,4,8\" reel4=\"1,1,1,1\" reel5=\"5,5,1,1\"/><delivery id=\"1901784-553231956167736687464893\" action=\"create\"/></server>";
            }
        }
        #endregion
        public FruitsAndJokers40GameLogic()
        {
            _gameID = GAMEID.FruitsAndJokers40;
            GameName = "FruitsAndJokers40";
        }
    }
}
