using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class NineHappyPharaohsGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "9_happy_pharaohs";
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
                return "<server><source game-ver=\"240521\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"scatter\" count=\"3\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"0\"/><combination symbol=\"2\" name=\"top\" count=\"3\" coef=\"150\"/><combination symbol=\"2\" name=\"top\" count=\"4\" coef=\"500\"/><combination symbol=\"2\" name=\"top\" count=\"5\" coef=\"2500\"/><combination symbol=\"3\" name=\"mid 1\" count=\"3\" coef=\"40\"/><combination symbol=\"3\" name=\"mid 1\" count=\"4\" coef=\"150\"/><combination symbol=\"3\" name=\"mid 1\" count=\"5\" coef=\"1000\"/><combination symbol=\"4\" name=\"mid 2\" count=\"3\" coef=\"40\"/><combination symbol=\"4\" name=\"mid 2\" count=\"4\" coef=\"150\"/><combination symbol=\"4\" name=\"mid 2\" count=\"5\" coef=\"1000\"/><combination symbol=\"5\" name=\"low 1\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"low 1\" count=\"4\" coef=\"40\"/><combination symbol=\"5\" name=\"low 1\" count=\"5\" coef=\"150\"/><combination symbol=\"6\" name=\"low 2\" count=\"3\" coef=\"20\"/><combination symbol=\"6\" name=\"low 2\" count=\"4\" coef=\"40\"/><combination symbol=\"6\" name=\"low 2\" count=\"5\" coef=\"150\"/><combination symbol=\"7\" name=\"low 3\" count=\"3\" coef=\"20\"/><combination symbol=\"7\" name=\"low 3\" count=\"4\" coef=\"40\"/><combination symbol=\"7\" name=\"low 3\" count=\"5\" coef=\"150\"/><combination symbol=\"8\" name=\"low 4\" count=\"3\" coef=\"20\"/><combination symbol=\"8\" name=\"low 4\" count=\"4\" coef=\"40\"/><combination symbol=\"8\" name=\"low 4\" count=\"5\" coef=\"150\"/><combination symbol=\"9\" name=\"mask_scatter\" count=\"3\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"1\"/><combination symbol=\"9\" name=\"mask_scatter\" count=\"4\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"5\"/><combination symbol=\"9\" name=\"mask_scatter\" count=\"5\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"9\" name=\"mask_scatter\" count=\"6\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"50\"/><combination symbol=\"9\" name=\"mask_scatter\" count=\"7\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"100\"/><combination symbol=\"9\" name=\"mask_scatter\" count=\"8\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"500\"/><combination symbol=\"9\" name=\"mask_scatter\" count=\"9\" coef=\"0\" multi_coef=\"1\" multi_coef2=\"2000\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"1,1,2,1,1\"/><payline id=\"7\" path=\"3,3,2,3,3\"/><payline id=\"8\" path=\"2,3,3,3,2\"/><payline id=\"9\" path=\"2,1,1,1,2\"/><payline id=\"10\" path=\"2,1,2,1,2\"/><payline id=\"11\" path=\"2,3,2,3,2\"/><payline id=\"12\" path=\"1,2,1,2,1\"/><payline id=\"13\" path=\"3,2,3,2,3\"/><payline id=\"14\" path=\"2,2,1,2,2\"/><payline id=\"15\" path=\"2,2,3,2,2\"/><payline id=\"16\" path=\"1,2,2,2,1\"/><payline id=\"17\" path=\"3,2,2,2,3\"/><payline id=\"18\" path=\"3,1,1,1,3\"/><payline id=\"19\" path=\"1,3,3,3,1\"/><payline id=\"20\" path=\"1,3,1,3,1\"/></paylines><symbols><symbol id=\"1\" title=\"scatter\"/><symbol id=\"2\" title=\"top\"/><symbol id=\"3\" title=\"mid 1\"/><symbol id=\"4\" title=\"mid 2\"/><symbol id=\"5\" title=\"low 1\"/><symbol id=\"6\" title=\"low 2\"/><symbol id=\"7\" title=\"low 3\"/><symbol id=\"8\" title=\"low 4\"/><symbol id=\"9\" title=\"mask_scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"9\" symbols=\"4,1,8,3,5,7,9,2,6\"/><reel id=\"2\" length=\"8\" symbols=\"4,7,3,8,5,6,2,9\"/><reel id=\"3\" length=\"9\" symbols=\"9,3,4,8,1,5,2,6,7\"/><reel id=\"4\" length=\"8\" symbols=\"3,5,2,6,8,7,4,9\"/><reel id=\"5\" length=\"9\" symbols=\"6,1,7,5,8,2,4,3,9\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"9\" symbols=\"4,1,8,3,5,7,9,2,6\"/><reel id=\"2\" length=\"8\" symbols=\"4,7,3,8,5,6,2,9\"/><reel id=\"3\" length=\"9\" symbols=\"9,3,4,8,1,5,2,6,7\"/><reel id=\"4\" length=\"8\" symbols=\"3,5,2,6,8,7,4,9\"/><reel id=\"5\" length=\"9\" symbols=\"6,1,7,5,8,2,4,3,9\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"9\" symbols=\"4,1,8,3,5,2,7,9,6\"/><reel id=\"2\" length=\"8\" symbols=\"4,7,3,8,5,9,2,6\"/><reel id=\"3\" length=\"9\" symbols=\"9,3,4,8,1,5,6,2,7\"/><reel id=\"4\" length=\"8\" symbols=\"3,5,2,6,8,9,7,4\"/><reel id=\"5\" length=\"9\" symbols=\"6,1,7,5,8,2,4,3,9\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"9\" symbols=\"4,1,8,3,5,2,7,9,6\"/><reel id=\"2\" length=\"8\" symbols=\"4,7,3,8,5,9,2,6\"/><reel id=\"3\" length=\"9\" symbols=\"9,3,4,8,5,6,2,1,7\"/><reel id=\"4\" length=\"8\" symbols=\"3,5,2,6,8,9,7,4\"/><reel id=\"5\" length=\"9\" symbols=\"6,1,7,5,8,2,4,3,9\"/></reels><reels id=\"5\"><reel id=\"1\" length=\"9\" symbols=\"4,1,8,3,5,7,9,2,6\"/><reel id=\"2\" length=\"8\" symbols=\"4,7,3,8,5,9,2,6\"/><reel id=\"3\" length=\"9\" symbols=\"9,3,4,8,5,6,2,1,7\"/><reel id=\"4\" length=\"8\" symbols=\"3,5,2,6,8,9,7,4\"/><reel id=\"5\" length=\"9\" symbols=\"6,1,7,5,8,2,4,3,9\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"5,2,8\" reel2=\"7,8,3\" reel3=\"9,9,3\" reel4=\"5,7,8\" reel5=\"6,1,7\"/><delivery id=\"485335-3241515802205541356033927\" action=\"create\"/></server>";
            }
        }
        #endregion
        public NineHappyPharaohsGameLogic()
        {
            _gameID = GAMEID.NineHappyPharaohs;
            GameName = "NineHappyPharaohs";
        }
    }
}
