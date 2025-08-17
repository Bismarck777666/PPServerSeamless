using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BookOfGoldMultichanceGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "book_of_gold_multichance";
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
                return "<server><source game-ver=\"310720\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"10\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"10\" count=\"4\" coef=\"25\"/><combination symbol=\"1\" name=\"10\" count=\"5\" coef=\"100\"/><combination symbol=\"2\" name=\"j\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"j\" count=\"4\" coef=\"25\"/><combination symbol=\"2\" name=\"j\" count=\"5\" coef=\"100\"/><combination symbol=\"3\" name=\"q\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"q\" count=\"4\" coef=\"25\"/><combination symbol=\"3\" name=\"q\" count=\"5\" coef=\"100\"/><combination symbol=\"4\" name=\"k\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"k\" count=\"4\" coef=\"40\"/><combination symbol=\"4\" name=\"k\" count=\"5\" coef=\"150\"/><combination symbol=\"5\" name=\"a\" count=\"3\" coef=\"5\"/><combination symbol=\"5\" name=\"a\" count=\"4\" coef=\"40\"/><combination symbol=\"5\" name=\"a\" count=\"5\" coef=\"150\"/><combination symbol=\"6\" name=\"med2\" count=\"2\" coef=\"5\"/><combination symbol=\"6\" name=\"med2\" count=\"3\" coef=\"30\"/><combination symbol=\"6\" name=\"med2\" count=\"4\" coef=\"100\"/><combination symbol=\"6\" name=\"med2\" count=\"5\" coef=\"750\"/><combination symbol=\"7\" name=\"med1\" count=\"2\" coef=\"5\"/><combination symbol=\"7\" name=\"med1\" count=\"3\" coef=\"30\"/><combination symbol=\"7\" name=\"med1\" count=\"4\" coef=\"100\"/><combination symbol=\"7\" name=\"med1\" count=\"5\" coef=\"750\"/><combination symbol=\"8\" name=\"top2\" count=\"2\" coef=\"5\"/><combination symbol=\"8\" name=\"top2\" count=\"3\" coef=\"40\"/><combination symbol=\"8\" name=\"top2\" count=\"4\" coef=\"400\"/><combination symbol=\"8\" name=\"top2\" count=\"5\" coef=\"2000\"/><combination symbol=\"9\" name=\"top1\" count=\"2\" coef=\"10\"/><combination symbol=\"9\" name=\"top1\" count=\"3\" coef=\"100\"/><combination symbol=\"9\" name=\"top1\" count=\"4\" coef=\"1000\"/><combination symbol=\"9\" name=\"top1\" count=\"5\" coef=\"5000\"/><combination symbol=\"10\" name=\"scatterwild\" count=\"3\" coef=\"12\" multi_coef=\"1\" multi_coef2=\"2\"/><combination symbol=\"10\" name=\"scatterwild\" count=\"4\" coef=\"12\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"10\" name=\"scatterwild\" count=\"5\" coef=\"12\" multi_coef=\"1\" multi_coef2=\"200\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"3,2,2,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"10\"/><symbol id=\"2\" title=\"j\"/><symbol id=\"3\" title=\"q\"/><symbol id=\"4\" title=\"k\"/><symbol id=\"5\" title=\"a\"/><symbol id=\"6\" title=\"med2\"/><symbol id=\"7\" title=\"med1\"/><symbol id=\"8\" title=\"top2\"/><symbol id=\"9\" title=\"top1\"/><symbol id=\"10\" title=\"scatterwild\"/><symbol id=\"11\" title=\"hidden\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"1,4,7,3,2,5,9,10,6,8\"/><reel id=\"2\" length=\"10\" symbols=\"1,2,4,10,5,8,6,3,7,9\"/><reel id=\"3\" length=\"10\" symbols=\"1,3,9,2,8,6,4,7,5,10\"/><reel id=\"4\" length=\"10\" symbols=\"1,2,5,3,4,7,8,9,6,10\"/><reel id=\"5\" length=\"10\" symbols=\"1,8,3,7,4,5,9,2,6,10\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"1,4,7,3,2,5,9,10,6,8\"/><reel id=\"2\" length=\"10\" symbols=\"1,2,4,10,5,8,6,3,7,9\"/><reel id=\"3\" length=\"10\" symbols=\"1,3,9,2,8,6,4,7,5,10\"/><reel id=\"4\" length=\"10\" symbols=\"1,2,5,3,4,7,8,9,6,10\"/><reel id=\"5\" length=\"10\" symbols=\"1,8,3,7,4,5,9,2,6,10\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"11\" symbols=\"4,5,7,3,2,8,1,10,9,11,6\"/><reel id=\"2\" length=\"11\" symbols=\"1,4,3,5,10,6,2,8,7,9,11\"/><reel id=\"3\" length=\"11\" symbols=\"2,5,10,1,4,8,3,6,7,9,11\"/><reel id=\"4\" length=\"11\" symbols=\"1,7,9,3,2,10,4,8,5,6,11\"/><reel id=\"5\" length=\"11\" symbols=\"5,6,1,10,3,4,2,8,11,9,7\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"11\" symbols=\"1,4,2,5,8,3,11,6,7,10,9\"/><reel id=\"2\" length=\"11\" symbols=\"1,3,7,4,9,2,5,6,11,8,10\"/><reel id=\"3\" length=\"11\" symbols=\"1,2,5,8,3,7,10,4,6,11,9\"/><reel id=\"4\" length=\"11\" symbols=\"1,3,6,4,7,2,9,5,11,10,8\"/><reel id=\"5\" length=\"11\" symbols=\"1,2,10,4,3,7,6,8,9,11,5\"/></reels><reels id=\"5\"><reel id=\"1\" length=\"11\" symbols=\"1,4,11,2,5,8,3,6,7,10,9\"/><reel id=\"2\" length=\"11\" symbols=\"1,3,11,7,4,9,2,5,6,8,10\"/><reel id=\"3\" length=\"11\" symbols=\"11,1,2,5,10,8,3,7,4,6,9\"/><reel id=\"4\" length=\"11\" symbols=\"1,11,3,6,4,7,2,9,5,8,10\"/><reel id=\"5\" length=\"11\" symbols=\"11,1,2,4,8,3,7,6,9,10,5\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"5,3,9,4,5\" reel2=\"3,2,1,6,2\" reel3=\"2,10,5,6,1\" reel4=\"3,1,8,2,5\" reel5=\"2,1,10,2,8\"/><game free_game_cost=\"100\"/><delivery id=\"1148898-031926868264263264287405\" action=\"create\"/></server>";
            }
        }
        #endregion
        public BookOfGoldMultichanceGameLogic()
        {
            _gameID = GAMEID.BookOfGoldMultichance;
            GameName = "BookOfGoldMultichance";
        }
    }
}
