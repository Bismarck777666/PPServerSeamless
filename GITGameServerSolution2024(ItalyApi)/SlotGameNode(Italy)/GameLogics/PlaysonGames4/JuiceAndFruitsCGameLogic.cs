using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class JuiceAndFruitsCGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "juice_and_fruits_c";
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
                return "<server><source game-ver=\"56175\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"Seven\" count=\"3\" coef=\"100\"/><combination symbol=\"1\" name=\"Seven\" count=\"4\" coef=\"1000\"/><combination symbol=\"1\" name=\"Seven\" count=\"5\" coef=\"5000\"/><combination symbol=\"2\" name=\"Bell\" count=\"3\" coef=\"50\"/><combination symbol=\"2\" name=\"Bell\" count=\"4\" coef=\"200\"/><combination symbol=\"2\" name=\"Bell\" count=\"5\" coef=\"500\"/><combination symbol=\"3\" name=\"WaterMelon\" count=\"3\" coef=\"50\"/><combination symbol=\"3\" name=\"WaterMelon\" count=\"4\" coef=\"200\"/><combination symbol=\"3\" name=\"WaterMelon\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"Plim\" count=\"3\" coef=\"20\"/><combination symbol=\"4\" name=\"Plim\" count=\"4\" coef=\"50\"/><combination symbol=\"4\" name=\"Plim\" count=\"5\" coef=\"200\"/><combination symbol=\"5\" name=\"Orange\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"Orange\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"Orange\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"Lemon\" count=\"3\" coef=\"20\"/><combination symbol=\"6\" name=\"Lemon\" count=\"4\" coef=\"50\"/><combination symbol=\"6\" name=\"Lemon\" count=\"5\" coef=\"200\"/><combination symbol=\"7\" name=\"Cherry\" count=\"3\" coef=\"20\"/><combination symbol=\"7\" name=\"Cherry\" count=\"4\" coef=\"50\"/><combination symbol=\"7\" name=\"Cherry\" count=\"5\" coef=\"200\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"3,2,2,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"Seven\"/><symbol id=\"2\" title=\"Bell\"/><symbol id=\"3\" title=\"WaterMelon\"/><symbol id=\"4\" title=\"Plim\"/><symbol id=\"5\" title=\"Orange\"/><symbol id=\"6\" title=\"Lemon\"/><symbol id=\"7\" title=\"Cherry\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"96\" symbols=\"3,3,6,6,6,4,4,4,5,5,5,7,7,7,4,4,4,1,3,3,2,2,4,4,4,3,3,5,5,5,6,6,6,2,2,5,5,5,7,7,7,2,2,1,5,5,5,2,2,6,6,6,7,7,7,1,3,3,6,6,6,1,3,3,7,7,7,4,4,4,2,2,7,7,7,5,5,5,4,4,4,1,2,2,4,4,4,3,3,6,6,6,5,5,5,1\"/><reel id=\"2\" length=\"96\" symbols=\"4,4,4,7,7,7,1,3,3,5,5,5,4,4,4,2,2,1,6,6,6,5,5,5,7,7,7,3,3,4,4,4,2,2,6,6,6,1,2,2,7,7,7,1,5,5,5,2,2,7,7,7,6,6,6,3,3,4,4,4,1,6,6,6,3,3,4,4,4,5,5,5,7,7,7,2,2,6,6,6,4,4,4,5,5,5,1,3,3,2,2,5,5,5,3,3\"/><reel id=\"3\" length=\"96\" symbols=\"4,4,4,6,6,6,5,5,5,7,7,7,2,2,4,4,4,6,6,6,3,3,1,6,6,6,2,2,1,7,7,7,5,5,5,1,2,2,5,5,5,4,4,4,3,3,5,5,5,7,7,7,4,4,4,1,3,3,2,2,4,4,4,5,5,5,7,7,7,3,3,6,6,6,1,3,3,7,7,7,5,5,5,4,4,4,2,2,1,3,3,6,6,6,2,2\"/><reel id=\"4\" length=\"96\" symbols=\"3,3,1,2,2,7,7,7,6,6,6,5,5,5,2,2,1,6,6,6,5,5,5,2,2,7,7,7,1,4,4,4,7,7,7,1,2,2,6,6,6,1,4,4,4,3,3,7,7,7,4,4,4,2,2,6,6,6,4,4,4,5,5,5,3,3,4,4,4,7,7,7,3,3,5,5,5,1,3,3,2,2,5,5,5,3,3,4,4,4,6,6,6,5,5,5\"/><reel id=\"5\" length=\"96\" symbols=\"3,3,1,5,5,5,6,6,6,3,3,7,7,7,4,4,4,1,3,3,7,7,7,5,5,5,4,4,4,2,2,5,5,5,1,6,6,6,4,4,4,2,2,1,4,4,4,5,5,5,3,3,2,2,6,6,6,5,5,5,4,4,4,2,2,7,7,7,3,3,5,5,5,6,6,6,2,2,1,4,4,4,6,6,6,3,3,7,7,7,2,2,1,7,7,7\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"6,2,6\" reel2=\"2,1,2\" reel3=\"5,2,7\" reel4=\"7,3,6\" reel5=\"4,4,1\"/><delivery id=\"1910320-277654814239559002954756\" action=\"create\"/></server>";
            }
        }
        #endregion
        public JuiceAndFruitsCGameLogic()
        {
            _gameID = GAMEID.JuiceAndFruitsC;
            GameName = "JuiceAndFruitsC";
        }
    }
}
