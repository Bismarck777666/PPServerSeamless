using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class SpiritOfEgyptGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "spirit_of_egypt";
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
                return "<server><source game-ver=\"270721\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low 4\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"low 4\" count=\"4\" coef=\"15\"/><combination symbol=\"1\" name=\"low 4\" count=\"5\" coef=\"30\"/><combination symbol=\"2\" name=\"low 3\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"low 3\" count=\"4\" coef=\"15\"/><combination symbol=\"2\" name=\"low 3\" count=\"5\" coef=\"30\"/><combination symbol=\"3\" name=\"low 2\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"low 2\" count=\"4\" coef=\"15\"/><combination symbol=\"3\" name=\"low 2\" count=\"5\" coef=\"30\"/><combination symbol=\"4\" name=\"low 1\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"low 1\" count=\"4\" coef=\"15\"/><combination symbol=\"4\" name=\"low 1\" count=\"5\" coef=\"30\"/><combination symbol=\"5\" name=\"mid 4\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"mid 4\" count=\"4\" coef=\"40\"/><combination symbol=\"5\" name=\"mid 4\" count=\"5\" coef=\"80\"/><combination symbol=\"6\" name=\"mid 3\" count=\"3\" coef=\"30\"/><combination symbol=\"6\" name=\"mid 3\" count=\"4\" coef=\"60\"/><combination symbol=\"6\" name=\"mid 3\" count=\"5\" coef=\"120\"/><combination symbol=\"7\" name=\"mid 2\" count=\"3\" coef=\"40\"/><combination symbol=\"7\" name=\"mid 2\" count=\"4\" coef=\"80\"/><combination symbol=\"7\" name=\"mid 2\" count=\"5\" coef=\"160\"/><combination symbol=\"8\" name=\"mid 1\" count=\"3\" coef=\"50\"/><combination symbol=\"8\" name=\"mid 1\" count=\"4\" coef=\"100\"/><combination symbol=\"8\" name=\"mid 1\" count=\"5\" coef=\"250\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"100\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"200\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"500\"/><combination symbol=\"11\" name=\"scatter\" count=\"3\" coef=\"0\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/><payline id=\"11\" path=\"1,2,2,2,3\"/><payline id=\"12\" path=\"2,3,2,1,2\"/><payline id=\"13\" path=\"2,1,2,3,2\"/><payline id=\"14\" path=\"1,2,1,2,1\"/><payline id=\"15\" path=\"3,2,3,2,3\"/><payline id=\"16\" path=\"2,2,1,2,2\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"1,3,1,3,1\"/><payline id=\"19\" path=\"3,1,3,1,3\"/><payline id=\"20\" path=\"3,1,2,1,3\"/></paylines><symbols><symbol id=\"1\" title=\"low 4\"/><symbol id=\"2\" title=\"low 3\"/><symbol id=\"3\" title=\"low 2\"/><symbol id=\"4\" title=\"low 1\"/><symbol id=\"5\" title=\"mid 4\"/><symbol id=\"6\" title=\"mid 3\"/><symbol id=\"7\" title=\"mid 2\"/><symbol id=\"8\" title=\"mid 1\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"bonus\"/><symbol id=\"11\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"1,2,5,9,4,3,6,10,8,7\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,3,7,2,11,5,6,10,4\"/><reel id=\"3\" length=\"11\" symbols=\"5,1,8,3,11,9,7,2,6,4,10\"/><reel id=\"4\" length=\"11\" symbols=\"8,1,9,5,6,4,11,2,7,10,3\"/><reel id=\"5\" length=\"10\" symbols=\"9,1,8,3,5,4,2,7,6,10\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"1,2,5,9,4,3,6,10,7,8\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,3,7,2,11,6,5,10,4\"/><reel id=\"3\" length=\"11\" symbols=\"5,1,8,3,11,9,7,2,6,4,10\"/><reel id=\"4\" length=\"11\" symbols=\"8,1,9,5,6,4,11,2,7,10,3\"/><reel id=\"5\" length=\"10\" symbols=\"9,1,8,3,5,4,2,7,6,10\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"10\" symbols=\"1,3,10,2,7,6,4,8,9,5\"/><reel id=\"2\" length=\"10\" symbols=\"9,7,2,1,6,3,8,4,5,10\"/><reel id=\"3\" length=\"10\" symbols=\"1,8,2,9,7,6,3,5,4,10\"/><reel id=\"4\" length=\"10\" symbols=\"8,1,6,7,3,9,4,2,10,5\"/><reel id=\"5\" length=\"10\" symbols=\"8,5,4,9,1,7,6,2,3,10\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"9\" symbols=\"1,2,7,6,3,4,8,9,5\"/><reel id=\"2\" length=\"9\" symbols=\"9,7,2,1,6,3,8,4,5\"/><reel id=\"3\" length=\"9\" symbols=\"1,8,2,9,7,6,3,5,4\"/><reel id=\"4\" length=\"9\" symbols=\"8,1,6,7,3,9,4,2,5\"/><reel id=\"5\" length=\"9\" symbols=\"8,5,4,9,1,7,6,2,3\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"10,10,3\" reel2=\"11,3,5\" reel3=\"7,2,6\" reel4=\"9,9,5\" reel5=\"10,10,10\"><bonus bonus_pos=\"0,5,4,9,14\"/></shift><game jackpots_tb=\"25,250,5000\" bonus_spins=\"3\"/><shift_ext><shift server=\"0,0,0,0,0\" reel_set=\"3\" reel1=\"8,4,2\" reel2=\"8,4,2\" reel3=\"10,10,1\" reel4=\"6,7,3\" reel5=\"2,10,10\"/><shift server=\"0,0,0,0,0\" reel_set=\"3\" reel1=\"3,10,10\" reel2=\"8,4,2\" reel3=\"2,9,9\" reel4=\"4,3,2\" reel5=\"4,3,2\"/><shift server=\"0,0,0,0,0\" reel_set=\"3\" reel1=\"3,10,10\" reel2=\"3,9,9\" reel3=\"4,2,1\" reel4=\"4,2,1\" reel5=\"10,10,4\"/></shift_ext><delivery id=\"479724-6293871122580747817074264\" action=\"create\"/></server>";
            }
        }
        #endregion
        public SpiritOfEgyptGameLogic()
        {
            _gameID = GAMEID.SpiritOfEgypt;
            GameName = "SpiritOfEgypt";
        }
    }
}
