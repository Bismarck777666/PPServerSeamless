using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class StarsNFruits2HitGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "stars_n_fruits_double_hit";
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
                return "<server><source game-ver=\"220121\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"seven\" count=\"3\" coef=\"100\"/><combination symbol=\"1\" name=\"seven\" count=\"4\" coef=\"1000\"/><combination symbol=\"1\" name=\"seven\" count=\"5\" coef=\"5000\"/><combination symbol=\"2\" name=\"melon\" count=\"3\" coef=\"40\"/><combination symbol=\"2\" name=\"melon\" count=\"4\" coef=\"200\"/><combination symbol=\"2\" name=\"melon\" count=\"5\" coef=\"400\"/><combination symbol=\"3\" name=\"grapes\" count=\"3\" coef=\"40\"/><combination symbol=\"3\" name=\"grapes\" count=\"4\" coef=\"200\"/><combination symbol=\"3\" name=\"grapes\" count=\"5\" coef=\"400\"/><combination symbol=\"4\" name=\"plum\" count=\"3\" coef=\"20\"/><combination symbol=\"4\" name=\"plum\" count=\"4\" coef=\"50\"/><combination symbol=\"4\" name=\"plum\" count=\"5\" coef=\"200\"/><combination symbol=\"5\" name=\"orange\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"orange\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"orange\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"lemon\" count=\"3\" coef=\"20\"/><combination symbol=\"6\" name=\"lemon\" count=\"4\" coef=\"50\"/><combination symbol=\"6\" name=\"lemon\" count=\"5\" coef=\"200\"/><combination symbol=\"7\" name=\"cherry\" count=\"2\" coef=\"2\"/><combination symbol=\"7\" name=\"cherry\" count=\"3\" coef=\"20\"/><combination symbol=\"7\" name=\"cherry\" count=\"4\" coef=\"50\"/><combination symbol=\"7\" name=\"cherry\" count=\"5\" coef=\"200\"/><combination symbol=\"8\" name=\"scatter\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"1\"/><combination symbol=\"8\" name=\"scatter\" count=\"4\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"5\"/><combination symbol=\"8\" name=\"scatter\" count=\"5\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"15\"/><combination symbol=\"8\" name=\"scatter\" count=\"6\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"40\"/><combination symbol=\"8\" name=\"scatter\" count=\"7\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"100\"/><combination symbol=\"8\" name=\"scatter\" count=\"8\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"500\"/><combination symbol=\"8\" name=\"scatter\" count=\"9\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"2000\"/><combination symbol=\"8\" name=\"scatter\" count=\"10\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"5000\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/></paylines><symbols><symbol id=\"1\" title=\"seven\"/><symbol id=\"2\" title=\"melon\"/><symbol id=\"3\" title=\"grapes\"/><symbol id=\"4\" title=\"plum\"/><symbol id=\"5\" title=\"orange\"/><symbol id=\"6\" title=\"lemon\"/><symbol id=\"7\" title=\"cherry\"/><symbol id=\"8\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"7\" symbols=\"1,4,2,3,5,7,6\"/><reel id=\"2\" length=\"7\" symbols=\"1,2,7,5,3,4,6\"/><reel id=\"3\" length=\"7\" symbols=\"1,5,7,3,2,6,4\"/><reel id=\"4\" length=\"7\" symbols=\"3,2,1,7,5,4,6\"/><reel id=\"5\" length=\"7\" symbols=\"1,4,5,2,6,3,7\"/><reel id=\"6\" length=\"7\" symbols=\"1,4,2,3,5,7,6\"/><reel id=\"7\" length=\"7\" symbols=\"1,2,7,5,3,4,6\"/><reel id=\"8\" length=\"7\" symbols=\"1,5,7,3,2,6,4\"/><reel id=\"9\" length=\"7\" symbols=\"3,2,1,7,5,4,6\"/><reel id=\"10\" length=\"7\" symbols=\"1,4,5,2,6,3,7\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"7\" symbols=\"1,4,2,3,5,7,6\"/><reel id=\"2\" length=\"7\" symbols=\"1,2,7,5,3,4,6\"/><reel id=\"3\" length=\"7\" symbols=\"1,5,7,3,2,6,4\"/><reel id=\"4\" length=\"7\" symbols=\"3,2,1,7,5,4,6\"/><reel id=\"5\" length=\"7\" symbols=\"1,4,5,2,6,3,7\"/><reel id=\"6\" length=\"7\" symbols=\"1,4,2,3,5,7,6\"/><reel id=\"7\" length=\"7\" symbols=\"1,2,7,5,3,4,6\"/><reel id=\"8\" length=\"7\" symbols=\"1,5,7,3,2,6,4\"/><reel id=\"9\" length=\"7\" symbols=\"3,2,1,7,5,4,6\"/><reel id=\"10\" length=\"7\" symbols=\"1,4,5,2,6,3,7\"/></reels></slot><shift server=\"0,0,0,0,0,0,0,0,0,0\" reel_set=\"1\" reel1=\"3,6,1\" reel2=\"1,2,7\" reel3=\"8,1,3\" reel4=\"5,5,4\" reel5=\"6,3,2\" reel6=\"6,1,2\" reel7=\"7,7,5\" reel8=\"1,8,3\" reel9=\"4,2,5\" reel10=\"3,1,4\"/><game total_bet_mult=\"20\" visible_fields=\"2\"/><delivery id=\"561784-7695385828862870770326538\" action=\"create\"/></server>";
            }
        }
        #endregion

        public StarsNFruits2HitGameLogic()
        {
            _gameID = GAMEID.StarsNFruits2Hit;
            GameName = "StarsNFruits2Hit";
        }
    }
}
