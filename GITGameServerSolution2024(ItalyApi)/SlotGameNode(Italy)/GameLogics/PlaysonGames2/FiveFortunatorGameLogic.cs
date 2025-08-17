using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class FiveFortunatorGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "5_fortunator";
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
                return "<server><source game-ver=\"240521\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"2\" name=\"sevens\" count=\"2\" coef=\"10\"/><combination symbol=\"2\" name=\"sevens\" count=\"3\" coef=\"40\"/><combination symbol=\"2\" name=\"sevens\" count=\"4\" coef=\"200\"/><combination symbol=\"2\" name=\"sevens\" count=\"5\" coef=\"2000\"/><combination symbol=\"3\" name=\"dice\" count=\"3\" coef=\"30\"/><combination symbol=\"3\" name=\"dice\" count=\"4\" coef=\"100\"/><combination symbol=\"3\" name=\"dice\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"bell\" count=\"3\" coef=\"30\"/><combination symbol=\"4\" name=\"bell\" count=\"4\" coef=\"100\"/><combination symbol=\"4\" name=\"bell\" count=\"5\" coef=\"500\"/><combination symbol=\"5\" name=\"bar\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"bar\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"bar\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"spades\" count=\"3\" coef=\"10\"/><combination symbol=\"6\" name=\"spades\" count=\"4\" coef=\"25\"/><combination symbol=\"6\" name=\"spades\" count=\"5\" coef=\"100\"/><combination symbol=\"7\" name=\"clubs\" count=\"3\" coef=\"10\"/><combination symbol=\"7\" name=\"clubs\" count=\"4\" coef=\"25\"/><combination symbol=\"7\" name=\"clubs\" count=\"5\" coef=\"100\"/><combination symbol=\"8\" name=\"diamonds\" count=\"3\" coef=\"10\"/><combination symbol=\"8\" name=\"diamonds\" count=\"4\" coef=\"25\"/><combination symbol=\"8\" name=\"diamonds\" count=\"5\" coef=\"100\"/><combination symbol=\"9\" name=\"hearts\" count=\"3\" coef=\"10\"/><combination symbol=\"9\" name=\"hearts\" count=\"4\" coef=\"25\"/><combination symbol=\"9\" name=\"hearts\" count=\"5\" coef=\"100\"/><combination symbol=\"10\" name=\"disp\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"11\" name=\"scatter\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"3\"/><combination symbol=\"11\" name=\"scatter\" count=\"4\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"11\" name=\"scatter\" count=\"5\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"100\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/></paylines><symbols><symbol id=\"1\" title=\"wild\"/><symbol id=\"2\" title=\"sevens\"/><symbol id=\"3\" title=\"dice\"/><symbol id=\"4\" title=\"bell\"/><symbol id=\"5\" title=\"bar\"/><symbol id=\"6\" title=\"spades\"/><symbol id=\"7\" title=\"clubs\"/><symbol id=\"8\" title=\"diamonds\"/><symbol id=\"9\" title=\"hearts\"/><symbol id=\"10\" title=\"disp\"/><symbol id=\"11\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"2,5,7,6,4,9,8,11,10,3\"/><reel id=\"2\" length=\"10\" symbols=\"1,5,4,9,3,2,6,7,8,11\"/><reel id=\"3\" length=\"11\" symbols=\"1,9,3,4,11,7,6,5,2,8,10\"/><reel id=\"4\" length=\"10\" symbols=\"8,5,7,4,6,1,3,11,9,2\"/><reel id=\"5\" length=\"10\" symbols=\"10,4,2,9,3,5,6,7,8,11\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"2,5,7,6,9,8,11,4,10,3\"/><reel id=\"2\" length=\"10\" symbols=\"1,5,4,9,3,2,7,8,11,6\"/><reel id=\"3\" length=\"11\" symbols=\"1,9,3,4,11,7,6,5,2,8,10\"/><reel id=\"4\" length=\"10\" symbols=\"8,5,7,4,6,1,3,11,9,2\"/><reel id=\"5\" length=\"10\" symbols=\"10,4,2,9,3,5,6,7,8,11\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"9,8,8\" reel2=\"9,3,2\" reel3=\"7,6,6\" reel4=\"6,1,8\" reel5=\"9,4,3\"/><delivery id=\"484736-6529305561009490977175293\" action=\"create\"/></server>";
            }
        }
        #endregion
        public FiveFortunatorGameLogic()
        {
            _gameID = GAMEID.FiveFortunator;
            GameName = "FiveFortunator";
        }
    }
}
