using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BookOfGoldDhGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "book_of_gold_dh";
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
                return "<server><source game-ver=\"250321\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"10\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"10\" count=\"4\" coef=\"25\"/><combination symbol=\"1\" name=\"10\" count=\"5\" coef=\"100\"/><combination symbol=\"2\" name=\"j\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"j\" count=\"4\" coef=\"25\"/><combination symbol=\"2\" name=\"j\" count=\"5\" coef=\"100\"/><combination symbol=\"3\" name=\"q\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"q\" count=\"4\" coef=\"25\"/><combination symbol=\"3\" name=\"q\" count=\"5\" coef=\"100\"/><combination symbol=\"4\" name=\"k\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"k\" count=\"4\" coef=\"40\"/><combination symbol=\"4\" name=\"k\" count=\"5\" coef=\"150\"/><combination symbol=\"5\" name=\"a\" count=\"3\" coef=\"5\"/><combination symbol=\"5\" name=\"a\" count=\"4\" coef=\"40\"/><combination symbol=\"5\" name=\"a\" count=\"5\" coef=\"150\"/><combination symbol=\"6\" name=\"med2\" count=\"2\" coef=\"5\"/><combination symbol=\"6\" name=\"med2\" count=\"3\" coef=\"30\"/><combination symbol=\"6\" name=\"med2\" count=\"4\" coef=\"100\"/><combination symbol=\"6\" name=\"med2\" count=\"5\" coef=\"750\"/><combination symbol=\"7\" name=\"med1\" count=\"2\" coef=\"5\"/><combination symbol=\"7\" name=\"med1\" count=\"3\" coef=\"30\"/><combination symbol=\"7\" name=\"med1\" count=\"4\" coef=\"100\"/><combination symbol=\"7\" name=\"med1\" count=\"5\" coef=\"750\"/><combination symbol=\"8\" name=\"top2\" count=\"2\" coef=\"5\"/><combination symbol=\"8\" name=\"top2\" count=\"3\" coef=\"40\"/><combination symbol=\"8\" name=\"top2\" count=\"4\" coef=\"400\"/><combination symbol=\"8\" name=\"top2\" count=\"5\" coef=\"2000\"/><combination symbol=\"9\" name=\"top1\" count=\"2\" coef=\"10\"/><combination symbol=\"9\" name=\"top1\" count=\"3\" coef=\"100\"/><combination symbol=\"9\" name=\"top1\" count=\"4\" coef=\"1000\"/><combination symbol=\"9\" name=\"top1\" count=\"5\" coef=\"5000\"/><combination symbol=\"10\" name=\"wild\" count=\"3\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"1\"/><combination symbol=\"10\" name=\"wild\" count=\"4\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"10\"/><combination symbol=\"10\" name=\"wild\" count=\"5\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"100\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"2,3,2,1,2\"/></paylines><symbols><symbol id=\"1\" title=\"10\"/><symbol id=\"2\" title=\"j\"/><symbol id=\"3\" title=\"q\"/><symbol id=\"4\" title=\"k\"/><symbol id=\"5\" title=\"a\"/><symbol id=\"6\" title=\"med2\"/><symbol id=\"7\" title=\"med1\"/><symbol id=\"8\" title=\"top2\"/><symbol id=\"9\" title=\"top1\"/><symbol id=\"10\" title=\"wild\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"1,4,7,3,2,5,9,10,6,8\"/><reel id=\"2\" length=\"10\" symbols=\"1,2,4,10,5,8,6,3,7,9\"/><reel id=\"3\" length=\"10\" symbols=\"1,3,9,2,8,6,4,7,5,10\"/><reel id=\"4\" length=\"10\" symbols=\"1,2,5,3,4,7,8,9,6,10\"/><reel id=\"5\" length=\"10\" symbols=\"1,8,3,7,4,5,9,2,6,10\"/><reel id=\"6\" length=\"10\" symbols=\"1,4,7,3,2,5,9,10,6,8\"/><reel id=\"7\" length=\"10\" symbols=\"1,2,4,10,5,8,6,3,7,9\"/><reel id=\"8\" length=\"10\" symbols=\"1,3,9,2,8,6,4,7,5,10\"/><reel id=\"9\" length=\"10\" symbols=\"1,2,5,3,4,7,8,9,6,10\"/><reel id=\"10\" length=\"10\" symbols=\"1,8,3,7,4,5,9,2,6,10\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"1,4,7,3,2,5,9,10,6,8\"/><reel id=\"2\" length=\"10\" symbols=\"1,2,4,10,5,8,6,3,7,9\"/><reel id=\"3\" length=\"10\" symbols=\"1,3,9,2,8,6,4,7,5,10\"/><reel id=\"4\" length=\"10\" symbols=\"1,2,5,3,4,7,8,9,6,10\"/><reel id=\"5\" length=\"10\" symbols=\"1,8,3,7,4,5,9,2,6,10\"/><reel id=\"6\" length=\"10\" symbols=\"1,4,7,3,2,5,9,10,6,8\"/><reel id=\"7\" length=\"10\" symbols=\"1,2,4,10,5,8,6,3,7,9\"/><reel id=\"8\" length=\"10\" symbols=\"1,3,9,2,8,6,4,7,5,10\"/><reel id=\"9\" length=\"10\" symbols=\"1,2,5,3,4,7,8,9,6,10\"/><reel id=\"10\" length=\"10\" symbols=\"1,8,3,7,4,5,9,2,6,10\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"10\" symbols=\"5,9,3,6,8,2,10,4,7,1\"/><reel id=\"2\" length=\"10\" symbols=\"4,3,7,5,1,2,8,6,10,9\"/><reel id=\"3\" length=\"10\" symbols=\"2,5,1,10,4,8,3,7,6,9\"/><reel id=\"4\" length=\"10\" symbols=\"1,6,3,2,10,4,8,9,5,7\"/><reel id=\"5\" length=\"10\" symbols=\"5,7,2,1,10,4,8,9,3,6\"/><reel id=\"6\" length=\"10\" symbols=\"5,9,3,6,8,2,10,4,7,1\"/><reel id=\"7\" length=\"10\" symbols=\"4,3,7,5,1,2,8,6,10,9\"/><reel id=\"8\" length=\"10\" symbols=\"2,5,1,10,4,8,3,7,6,9\"/><reel id=\"9\" length=\"10\" symbols=\"1,6,3,2,10,4,8,9,5,7\"/><reel id=\"10\" length=\"10\" symbols=\"5,7,2,1,10,4,8,9,3,6\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"10\" symbols=\"5,9,3,6,8,2,10,4,7,1\"/><reel id=\"2\" length=\"10\" symbols=\"4,3,7,5,1,2,8,6,10,9\"/><reel id=\"3\" length=\"10\" symbols=\"2,5,1,10,4,8,3,7,6,9\"/><reel id=\"4\" length=\"10\" symbols=\"1,6,3,2,10,4,8,9,5,7\"/><reel id=\"5\" length=\"10\" symbols=\"5,7,2,1,10,4,8,9,3,6\"/><reel id=\"6\" length=\"10\" symbols=\"5,9,3,6,8,2,10,4,7,1\"/><reel id=\"7\" length=\"10\" symbols=\"4,3,7,5,1,2,8,6,10,9\"/><reel id=\"8\" length=\"10\" symbols=\"2,5,1,10,4,8,3,7,6,9\"/><reel id=\"9\" length=\"10\" symbols=\"1,6,3,2,10,4,8,9,5,7\"/><reel id=\"10\" length=\"10\" symbols=\"5,7,2,1,10,4,8,9,3,6\"/></reels><reels id=\"5\"><reel id=\"1\" length=\"10\" symbols=\"5,9,3,6,8,2,10,4,7,1\"/><reel id=\"2\" length=\"10\" symbols=\"4,3,7,5,1,2,8,6,10,9\"/><reel id=\"3\" length=\"10\" symbols=\"2,5,1,10,4,8,3,7,6,9\"/><reel id=\"4\" length=\"10\" symbols=\"1,6,3,2,10,4,8,9,5,7\"/><reel id=\"5\" length=\"10\" symbols=\"5,7,2,1,10,4,8,9,3,6\"/><reel id=\"6\" length=\"10\" symbols=\"5,9,3,6,8,2,10,4,7,1\"/><reel id=\"7\" length=\"10\" symbols=\"4,3,7,5,1,2,8,6,10,9\"/><reel id=\"8\" length=\"10\" symbols=\"2,5,1,10,4,8,3,7,6,9\"/><reel id=\"9\" length=\"10\" symbols=\"1,6,3,2,10,4,8,9,5,7\"/><reel id=\"10\" length=\"10\" symbols=\"5,7,2,1,10,4,8,9,3,6\"/></reels></slot><shift server=\"0,0,0,0,0,0,0,0,0,0\" reel_set=\"1\" reel1=\"2,7,3,1,10\" reel2=\"3,2,1,6,2\" reel3=\"3,9,2,1,8\" reel4=\"3,2,10,1,4\" reel5=\"6,1,8,3,7\" reel6=\"8,4,3,8,1\" reel7=\"4,10,5,2,8\" reel8=\"1,8,6,3,8\" reel9=\"1,3,7,1,5\" reel10=\"2,1,10,2,8\"/><game total_bet_mult=\"20\" visible_fields=\"2\"/><delivery id=\"487366-1895473647970943513695128\" action=\"create\"/></server>";
            }
        }
        #endregion
        public BookOfGoldDhGameLogic()
        {
            _gameID = GAMEID.BookOfGoldDh;
            GameName = "BookOfGoldDh";
        }
    }
}
