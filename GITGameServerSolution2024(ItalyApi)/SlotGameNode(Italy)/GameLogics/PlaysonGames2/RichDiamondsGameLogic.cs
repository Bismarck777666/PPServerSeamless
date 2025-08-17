using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class RichDiamondsGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "rich_diamonds";
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
                return "<server><source game-ver=\"160421\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low 4\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"low 4\" count=\"4\" coef=\"20\"/><combination symbol=\"1\" name=\"low 4\" count=\"5\" coef=\"50\"/><combination symbol=\"2\" name=\"low 3\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"low 3\" count=\"4\" coef=\"20\"/><combination symbol=\"2\" name=\"low 3\" count=\"5\" coef=\"50\"/><combination symbol=\"3\" name=\"low 2\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"low 2\" count=\"4\" coef=\"20\"/><combination symbol=\"3\" name=\"low 2\" count=\"5\" coef=\"50\"/><combination symbol=\"4\" name=\"low 1\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"low 1\" count=\"4\" coef=\"20\"/><combination symbol=\"4\" name=\"low 1\" count=\"5\" coef=\"50\"/><combination symbol=\"5\" name=\"mid 3\" count=\"3\" coef=\"10\"/><combination symbol=\"5\" name=\"mid 3\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"mid 3\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"mid 2\" count=\"3\" coef=\"15\"/><combination symbol=\"6\" name=\"mid 2\" count=\"4\" coef=\"100\"/><combination symbol=\"6\" name=\"mid 2\" count=\"5\" coef=\"300\"/><combination symbol=\"7\" name=\"mid 1\" count=\"3\" coef=\"20\"/><combination symbol=\"7\" name=\"mid 1\" count=\"4\" coef=\"150\"/><combination symbol=\"7\" name=\"mid 1\" count=\"5\" coef=\"400\"/><combination symbol=\"8\" name=\"top\" count=\"3\" coef=\"25\"/><combination symbol=\"8\" name=\"top\" count=\"4\" coef=\"250\"/><combination symbol=\"8\" name=\"top\" count=\"5\" coef=\"500\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"25\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"250\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"500\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"2\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"low 4\"/><symbol id=\"2\" title=\"low 3\"/><symbol id=\"3\" title=\"low 2\"/><symbol id=\"4\" title=\"low 1\"/><symbol id=\"5\" title=\"mid 3\"/><symbol id=\"6\" title=\"mid 2\"/><symbol id=\"7\" title=\"mid 1\"/><symbol id=\"8\" title=\"top\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"scatter\"/><symbol id=\"11\" title=\"bonus\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"11\" symbols=\"2,5,3,4,10,7,9,1,8,11,6\"/><reel id=\"2\" length=\"10\" symbols=\"1,2,4,3,6,7,9,5,8,11\"/><reel id=\"3\" length=\"11\" symbols=\"5,2,4,8,10,1,3,9,6,7,11\"/><reel id=\"4\" length=\"10\" symbols=\"5,2,4,8,1,3,6,9,11,7\"/><reel id=\"5\" length=\"11\" symbols=\"5,4,2,10,1,6,8,9,7,3,11\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"11\" symbols=\"2,5,3,4,10,7,9,1,8,11,6\"/><reel id=\"2\" length=\"10\" symbols=\"1,2,4,3,6,7,9,5,8,11\"/><reel id=\"3\" length=\"11\" symbols=\"5,2,4,8,10,1,3,9,6,7,11\"/><reel id=\"4\" length=\"10\" symbols=\"5,2,4,8,1,3,6,9,11,7\"/><reel id=\"5\" length=\"11\" symbols=\"5,4,2,10,1,6,8,9,7,3,11\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"9\" symbols=\"3,5,4,1,7,9,2,6,8\"/><reel id=\"2\" length=\"11\" symbols=\"2,3,1,8,5,4,6,9,11,7,10\"/><reel id=\"3\" length=\"11\" symbols=\"2,3,1,8,5,4,6,9,11,7,10\"/><reel id=\"4\" length=\"11\" symbols=\"2,3,1,8,5,4,6,9,11,7,10\"/><reel id=\"5\" length=\"9\" symbols=\"1,4,2,6,3,5,9,7,8\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"9\" symbols=\"3,5,4,1,7,9,2,6,8\"/><reel id=\"2\" length=\"10\" symbols=\"2,3,1,8,5,4,6,9,7,10\"/><reel id=\"3\" length=\"10\" symbols=\"2,3,1,8,5,4,6,9,7,10\"/><reel id=\"4\" length=\"10\" symbols=\"2,3,1,8,5,4,6,9,7,10\"/><reel id=\"5\" length=\"9\" symbols=\"1,4,2,6,3,5,9,7,8\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"11,11,1\" reel2=\"7,9,9\" reel3=\"11,6,5\" reel4=\"9,9,9\" reel5=\"8,6,10\"><bonus bonus_pos=\"0,2,5\" bonus_tb=\"5,7,14\"/></shift><shift_ext><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"7,2,1\" reel2=\"4,4,4\" reel3=\"4,4,4\" reel4=\"4,4,4\" reel5=\"8,1,1\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"8,5,4\" reel2=\"3,3,3\" reel3=\"3,3,3\" reel4=\"3,3,3\" reel5=\"6,1,7\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"8,5,4\" reel2=\"1,1,1\" reel3=\"1,1,1\" reel4=\"1,1,1\" reel5=\"1,4,2\"/></shift_ext><game jackpots_tb=\"30,100,2000\" bonus_spins=\"3\" big_symbol_pos=\"1,2,3,6,7,8,11,12,13\"/><delivery id=\"487854-2469118900972266061813500\" action=\"create\"/></server>";
            }
        }
        #endregion
        public RichDiamondsGameLogic()
        {
            _gameID = GAMEID.RichDiamonds;
            GameName = "RichDiamonds";
        }
    }
}
