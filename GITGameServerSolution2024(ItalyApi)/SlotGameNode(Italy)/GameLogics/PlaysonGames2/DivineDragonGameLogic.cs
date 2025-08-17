using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class DivineDragonGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "divine_dragon";
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
                return "<server><source game-ver=\"50121\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low 4\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"low 4\" count=\"4\" coef=\"15\"/><combination symbol=\"1\" name=\"low 4\" count=\"5\" coef=\"40\"/><combination symbol=\"2\" name=\"low 3\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"low 3\" count=\"4\" coef=\"15\"/><combination symbol=\"2\" name=\"low 3\" count=\"5\" coef=\"40\"/><combination symbol=\"3\" name=\"low 2\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"low 2\" count=\"4\" coef=\"15\"/><combination symbol=\"3\" name=\"low 2\" count=\"5\" coef=\"40\"/><combination symbol=\"4\" name=\"low 1\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"low 1\" count=\"4\" coef=\"15\"/><combination symbol=\"4\" name=\"low 1\" count=\"5\" coef=\"40\"/><combination symbol=\"5\" name=\"mid 4\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"mid 4\" count=\"4\" coef=\"40\"/><combination symbol=\"5\" name=\"mid 4\" count=\"5\" coef=\"100\"/><combination symbol=\"6\" name=\"mid 3\" count=\"3\" coef=\"30\"/><combination symbol=\"6\" name=\"mid 3\" count=\"4\" coef=\"60\"/><combination symbol=\"6\" name=\"mid 3\" count=\"5\" coef=\"125\"/><combination symbol=\"7\" name=\"mid 2\" count=\"3\" coef=\"40\"/><combination symbol=\"7\" name=\"mid 2\" count=\"4\" coef=\"80\"/><combination symbol=\"7\" name=\"mid 2\" count=\"5\" coef=\"150\"/><combination symbol=\"8\" name=\"mid 1\" count=\"3\" coef=\"50\"/><combination symbol=\"8\" name=\"mid 1\" count=\"4\" coef=\"100\"/><combination symbol=\"8\" name=\"mid 1\" count=\"5\" coef=\"250\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"100\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"200\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"1000\"/><combination symbol=\"11\" name=\"scatter\" count=\"3\" coef=\"0\"/><combination symbol=\"11\" name=\"scatter\" count=\"4\" coef=\"0\"/><combination symbol=\"11\" name=\"scatter\" count=\"5\" coef=\"0\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/><payline id=\"11\" path=\"1,2,2,2,3\"/><payline id=\"12\" path=\"2,3,2,1,2\"/><payline id=\"13\" path=\"2,1,2,3,2\"/><payline id=\"14\" path=\"1,2,1,2,1\"/><payline id=\"15\" path=\"3,2,3,2,3\"/><payline id=\"16\" path=\"2,2,1,2,2\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"1,3,1,3,1\"/><payline id=\"19\" path=\"3,1,3,1,3\"/><payline id=\"20\" path=\"3,1,2,1,3\"/></paylines><symbols><symbol id=\"1\" title=\"low 4\"/><symbol id=\"2\" title=\"low 3\"/><symbol id=\"3\" title=\"low 2\"/><symbol id=\"4\" title=\"low 1\"/><symbol id=\"5\" title=\"mid 4\"/><symbol id=\"6\" title=\"mid 3\"/><symbol id=\"7\" title=\"mid 2\"/><symbol id=\"8\" title=\"mid 1\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"bonus\"/><symbol id=\"11\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"1,2,5,9,4,3,6,10,8,7\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,3,7,2,11,5,6,10,4\"/><reel id=\"3\" length=\"11\" symbols=\"5,1,8,3,9,7,2,6,11,4,10\"/><reel id=\"4\" length=\"11\" symbols=\"8,1,9,5,6,4,3,11,2,7,10\"/><reel id=\"5\" length=\"10\" symbols=\"9,1,8,3,5,4,2,7,6,10\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"1,2,5,9,4,3,6,10,7,8\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,3,7,2,11,6,5,10,4\"/><reel id=\"3\" length=\"11\" symbols=\"5,1,8,3,9,7,2,11,6,4,10\"/><reel id=\"4\" length=\"11\" symbols=\"8,1,9,5,6,4,11,2,7,10,3\"/><reel id=\"5\" length=\"10\" symbols=\"9,1,8,3,5,4,2,7,6,10\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"6\" symbols=\"6,5,10,7,8,9\"/><reel id=\"2\" length=\"6\" symbols=\"9,7,6,5,8,10\"/><reel id=\"3\" length=\"6\" symbols=\"5,8,6,9,7,10\"/><reel id=\"4\" length=\"6\" symbols=\"8,5,6,7,9,10\"/><reel id=\"5\" length=\"6\" symbols=\"8,5,9,7,6,10\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"5\" symbols=\"6,5,7,8,9\"/><reel id=\"2\" length=\"5\" symbols=\"9,7,8,6,5\"/><reel id=\"3\" length=\"5\" symbols=\"5,8,6,9,7\"/><reel id=\"4\" length=\"5\" symbols=\"8,5,6,7,9\"/><reel id=\"5\" length=\"5\" symbols=\"8,5,9,7,6\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"4,7,3\" reel2=\"1,5,10\" reel3=\"2,10,10\" reel4=\"8,3,11\" reel5=\"9,9,1\"><bonus bonus_from_pos=\"11,7,12\" bonus_from_tb=\"3,25,4\"/></shift><game jackpots_tb=\"25,100,1000\" bonus_spins=\"3\"/><delivery id=\"488295-0247315760450183973797023\" action=\"create\"/></server>";
            }
        }
        #endregion
        public DivineDragonGameLogic()
        {
            _gameID = GAMEID.DivineDragon;
            GameName = "DivineDragon";
        }
    }
}
