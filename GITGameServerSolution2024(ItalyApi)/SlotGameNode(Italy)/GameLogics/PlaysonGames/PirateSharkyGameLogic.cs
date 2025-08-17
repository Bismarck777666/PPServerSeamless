using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class PirateSharkyGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "pirate_sharky";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get
            {
                return true;
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get
            {
                return 100.0;
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
                return "<server><source game-ver=\"200122\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"top\" count=\"2\" coef=\"5\"/><combination symbol=\"1\" name=\"top\" count=\"3\" coef=\"50\"/><combination symbol=\"1\" name=\"top\" count=\"4\" coef=\"200\"/><combination symbol=\"1\" name=\"top\" count=\"5\" coef=\"2000\"/><combination symbol=\"2\" name=\"mid_1\" count=\"3\" coef=\"30\"/><combination symbol=\"2\" name=\"mid_1\" count=\"4\" coef=\"150\"/><combination symbol=\"2\" name=\"mid_1\" count=\"5\" coef=\"1000\"/><combination symbol=\"3\" name=\"mid_2\" count=\"3\" coef=\"20\"/><combination symbol=\"3\" name=\"mid_2\" count=\"4\" coef=\"100\"/><combination symbol=\"3\" name=\"mid_2\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"mid_3\" count=\"3\" coef=\"20\"/><combination symbol=\"4\" name=\"mid_3\" count=\"4\" coef=\"100\"/><combination symbol=\"4\" name=\"mid_3\" count=\"5\" coef=\"500\"/><combination symbol=\"5\" name=\"low_1\" count=\"3\" coef=\"5\"/><combination symbol=\"5\" name=\"low_1\" count=\"4\" coef=\"25\"/><combination symbol=\"5\" name=\"low_1\" count=\"5\" coef=\"100\"/><combination symbol=\"6\" name=\"low_2\" count=\"3\" coef=\"5\"/><combination symbol=\"6\" name=\"low_2\" count=\"4\" coef=\"25\"/><combination symbol=\"6\" name=\"low_2\" count=\"5\" coef=\"100\"/><combination symbol=\"7\" name=\"low_3\" count=\"3\" coef=\"5\"/><combination symbol=\"7\" name=\"low_3\" count=\"4\" coef=\"25\"/><combination symbol=\"7\" name=\"low_3\" count=\"5\" coef=\"100\"/><combination symbol=\"8\" name=\"low_4\" count=\"3\" coef=\"5\"/><combination symbol=\"8\" name=\"low_4\" count=\"4\" coef=\"25\"/><combination symbol=\"8\" name=\"low_4\" count=\"5\" coef=\"100\"/><combination symbol=\"9\" name=\"low_5\" count=\"3\" coef=\"5\"/><combination symbol=\"9\" name=\"low_5\" count=\"4\" coef=\"25\"/><combination symbol=\"9\" name=\"low_5\" count=\"5\" coef=\"100\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"4\" coef=\"15\" multi_coef=\"1\" multi_coef2=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"5\" coef=\"20\" multi_coef=\"1\" multi_coef2=\"0\"/><combination symbol=\"11\" name=\"bonus\" count=\"3\" coef=\"10\"/><combination symbol=\"11\" name=\"bonus\" count=\"4\" coef=\"50\"/><combination symbol=\"11\" name=\"bonus\" count=\"5\" coef=\"200\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"4,4,4,4,4\"/><payline id=\"5\" path=\"1,2,3,2,1\"/><payline id=\"6\" path=\"2,3,4,3,2\"/><payline id=\"7\" path=\"4,3,2,3,4\"/><payline id=\"8\" path=\"3,2,1,2,3\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"2,2,3,4,4\"/><payline id=\"11\" path=\"3,3,2,1,1\"/><payline id=\"12\" path=\"4,4,3,2,2\"/></paylines><symbols><symbol id=\"1\" title=\"top\"/><symbol id=\"2\" title=\"mid_1\"/><symbol id=\"3\" title=\"mid_2\"/><symbol id=\"4\" title=\"mid_3\"/><symbol id=\"5\" title=\"low_1\"/><symbol id=\"6\" title=\"low_2\"/><symbol id=\"7\" title=\"low_3\"/><symbol id=\"8\" title=\"low_4\"/><symbol id=\"9\" title=\"low_5\"/><symbol id=\"10\" title=\"scatter\"/><symbol id=\"11\" title=\"bonus\"/><symbol id=\"12\" title=\"wild\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"11\" symbols=\"3,5,4,11,7,10,6,8,2,1,9\"/><reel id=\"2\" length=\"11\" symbols=\"3,11,5,4,7,10,6,8,2,1,9\"/><reel id=\"3\" length=\"11\" symbols=\"3,11,5,4,7,10,6,8,9,1,2\"/><reel id=\"4\" length=\"11\" symbols=\"3,11,5,4,7,10,6,8,1,9,2\"/><reel id=\"5\" length=\"11\" symbols=\"3,11,5,4,7,10,6,8,9,2,1\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"2,7,6,11,9,5,1,3,8,4\"/><reel id=\"2\" length=\"10\" symbols=\"2,7,6,11,9,5,1,3,8,4\"/><reel id=\"3\" length=\"10\" symbols=\"2,7,6,11,9,5,1,3,8,4\"/><reel id=\"4\" length=\"10\" symbols=\"2,7,11,6,9,5,1,3,8,4\"/><reel id=\"5\" length=\"10\" symbols=\"2,7,11,6,9,5,1,3,8,4\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"10\" symbols=\"9,3,7,1,5,4,6,11,8,2\"/><reel id=\"2\" length=\"10\" symbols=\"9,3,7,1,5,4,6,11,8,2\"/><reel id=\"3\" length=\"10\" symbols=\"9,3,7,1,5,4,6,11,8,2\"/><reel id=\"4\" length=\"10\" symbols=\"9,3,7,1,5,4,6,11,8,2\"/><reel id=\"5\" length=\"10\" symbols=\"9,11,3,7,1,5,4,6,8,2\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"10\" symbols=\"9,7,1,5,6,11,4,8,3,2\"/><reel id=\"2\" length=\"10\" symbols=\"9,7,1,5,6,11,4,8,3,2\"/><reel id=\"3\" length=\"10\" symbols=\"9,11,7,1,5,6,4,8,3,2\"/><reel id=\"4\" length=\"10\" symbols=\"9,11,7,1,5,6,4,8,3,2\"/><reel id=\"5\" length=\"10\" symbols=\"9,11,7,1,5,6,4,8,3,2\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"5,5,1,1\" reel2=\"4,4,7,7\" reel3=\"7,10,6,6\" reel4=\"11,11,11,11\" reel5=\"2,2,2,11\"><bonus bonus_pos=\"3,8,13,18,19\" bonus_tb=\"20,4,20,4,5\"/></shift><game free_game_cost=\"100\" total_bet_mult=\"10\"/><delivery id=\"361944-9322638283123765225343287\" action=\"create\"/></server>";
            }
        }
        #endregion
        public PirateSharkyGameLogic()
        {
            _gameID = GAMEID.PirateSharky;
            GameName = "PirateSharky";
        }
    }
}
