using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class SevensNFruits6GameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "sevens_n_fruits_6r";
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
                return "<server><source game-ver=\"240220\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"seven\" count=\"3\" coef=\"100\"/><combination symbol=\"1\" name=\"seven\" count=\"4\" coef=\"1000\"/><combination symbol=\"1\" name=\"seven\" count=\"5\" coef=\"5000\"/><combination symbol=\"1\" name=\"seven\" count=\"6\" coef=\"15000\"/><combination symbol=\"2\" name=\"melon\" count=\"3\" coef=\"50\"/><combination symbol=\"2\" name=\"melon\" count=\"4\" coef=\"200\"/><combination symbol=\"2\" name=\"melon\" count=\"5\" coef=\"500\"/><combination symbol=\"2\" name=\"melon\" count=\"6\" coef=\"2000\"/><combination symbol=\"3\" name=\"grapes\" count=\"3\" coef=\"50\"/><combination symbol=\"3\" name=\"grapes\" count=\"4\" coef=\"200\"/><combination symbol=\"3\" name=\"grapes\" count=\"5\" coef=\"500\"/><combination symbol=\"3\" name=\"grapes\" count=\"6\" coef=\"2000\"/><combination symbol=\"4\" name=\"plum\" count=\"3\" coef=\"20\"/><combination symbol=\"4\" name=\"plum\" count=\"4\" coef=\"50\"/><combination symbol=\"4\" name=\"plum\" count=\"5\" coef=\"200\"/><combination symbol=\"4\" name=\"plum\" count=\"6\" coef=\"500\"/><combination symbol=\"5\" name=\"orange\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"orange\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"orange\" count=\"5\" coef=\"200\"/><combination symbol=\"5\" name=\"orange\" count=\"6\" coef=\"500\"/><combination symbol=\"6\" name=\"lemon\" count=\"3\" coef=\"20\"/><combination symbol=\"6\" name=\"lemon\" count=\"4\" coef=\"50\"/><combination symbol=\"6\" name=\"lemon\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"lemon\" count=\"6\" coef=\"500\"/><combination symbol=\"7\" name=\"cherry\" count=\"2\" coef=\"5\"/><combination symbol=\"7\" name=\"cherry\" count=\"3\" coef=\"20\"/><combination symbol=\"7\" name=\"cherry\" count=\"4\" coef=\"50\"/><combination symbol=\"7\" name=\"cherry\" count=\"5\" coef=\"200\"/><combination symbol=\"7\" name=\"cherry\" count=\"6\" coef=\"500\"/><combination symbol=\"8\" name=\"scatter\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"1\"/><combination symbol=\"8\" name=\"scatter\" count=\"4\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"5\"/><combination symbol=\"8\" name=\"scatter\" count=\"5\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"25\"/><combination symbol=\"8\" name=\"scatter\" count=\"6\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"100\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,3,2,1\"/><payline id=\"5\" path=\"3,2,1,1,2,3\"/></paylines><symbols><symbol id=\"1\" title=\"seven\"/><symbol id=\"2\" title=\"melon\"/><symbol id=\"3\" title=\"grapes\"/><symbol id=\"4\" title=\"plum\"/><symbol id=\"5\" title=\"orange\"/><symbol id=\"6\" title=\"lemon\"/><symbol id=\"7\" title=\"cherry\"/><symbol id=\"8\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"8\" symbols=\"1,4,3,7,8,2,6,5\"/><reel id=\"2\" length=\"8\" symbols=\"1,6,2,5,7,8,4,3\"/><reel id=\"3\" length=\"8\" symbols=\"1,6,7,8,5,2,4,3\"/><reel id=\"4\" length=\"8\" symbols=\"1,5,6,3,4,8,2,7\"/><reel id=\"5\" length=\"8\" symbols=\"1,7,4,6,3,2,5,8\"/><reel id=\"6\" length=\"8\" symbols=\"1,5,3,7,8,6,2,4\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"8\" symbols=\"1,4,3,7,8,2,6,5\"/><reel id=\"2\" length=\"8\" symbols=\"1,6,2,5,7,8,4,3\"/><reel id=\"3\" length=\"8\" symbols=\"1,6,4,8,5,2,3,7\"/><reel id=\"4\" length=\"8\" symbols=\"1,5,6,4,8,3,2,7\"/><reel id=\"5\" length=\"8\" symbols=\"1,7,4,6,2,5,8,3\"/><reel id=\"6\" length=\"8\" symbols=\"1,5,7,3,8,6,2,4\"/></reels></slot><shift server=\"0,0,0,0,0,0\" reel_set=\"1\" reel1=\"4,3,4\" reel2=\"7,7,7\" reel3=\"4,3,3\" reel4=\"8,3,3\" reel5=\"3,3,1\" reel6=\"6,6,1\"/><game total_bet_mult=\"10\"/><delivery id=\"1172634-687997040545968233470044\" action=\"create\"/></server>";
            }
        }
        #endregion

        public SevensNFruits6GameLogic()
        {
            _gameID = GAMEID.SevensNFruits6;
            GameName = "SevensNFruits6";
        }
    }
}
