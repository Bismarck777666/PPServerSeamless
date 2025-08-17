using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BookOfGoldGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "book_of_gold";
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
                return "<server><source game-ver=\"130918\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"10\" count=\"3\" coef=\"4\"/><combination symbol=\"1\" name=\"10\" count=\"4\" coef=\"8\"/><combination symbol=\"1\" name=\"10\" count=\"5\" coef=\"80\"/><combination symbol=\"2\" name=\"j\" count=\"3\" coef=\"4\"/><combination symbol=\"2\" name=\"j\" count=\"4\" coef=\"8\"/><combination symbol=\"2\" name=\"j\" count=\"5\" coef=\"80\"/><combination symbol=\"3\" name=\"q\" count=\"3\" coef=\"4\"/><combination symbol=\"3\" name=\"q\" count=\"4\" coef=\"8\"/><combination symbol=\"3\" name=\"q\" count=\"5\" coef=\"80\"/><combination symbol=\"4\" name=\"k\" count=\"3\" coef=\"4\"/><combination symbol=\"4\" name=\"k\" count=\"4\" coef=\"12\"/><combination symbol=\"4\" name=\"k\" count=\"5\" coef=\"120\"/><combination symbol=\"5\" name=\"a\" count=\"3\" coef=\"4\"/><combination symbol=\"5\" name=\"a\" count=\"4\" coef=\"12\"/><combination symbol=\"5\" name=\"a\" count=\"5\" coef=\"120\"/><combination symbol=\"6\" name=\"med2\" count=\"2\" coef=\"4\"/><combination symbol=\"6\" name=\"med2\" count=\"3\" coef=\"12\"/><combination symbol=\"6\" name=\"med2\" count=\"4\" coef=\"50\"/><combination symbol=\"6\" name=\"med2\" count=\"5\" coef=\"500\"/><combination symbol=\"7\" name=\"med1\" count=\"2\" coef=\"4\"/><combination symbol=\"7\" name=\"med1\" count=\"3\" coef=\"12\"/><combination symbol=\"7\" name=\"med1\" count=\"4\" coef=\"50\"/><combination symbol=\"7\" name=\"med1\" count=\"5\" coef=\"500\"/><combination symbol=\"8\" name=\"top2\" count=\"2\" coef=\"4\"/><combination symbol=\"8\" name=\"top2\" count=\"3\" coef=\"15\"/><combination symbol=\"8\" name=\"top2\" count=\"4\" coef=\"100\"/><combination symbol=\"8\" name=\"top2\" count=\"5\" coef=\"1000\"/><combination symbol=\"9\" name=\"top1\" count=\"2\" coef=\"8\"/><combination symbol=\"9\" name=\"top1\" count=\"3\" coef=\"20\"/><combination symbol=\"9\" name=\"top1\" count=\"4\" coef=\"200\"/><combination symbol=\"9\" name=\"top1\" count=\"5\" coef=\"2000\"/><combination symbol=\"10\" name=\"wild\" count=\"3\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"2\"/><combination symbol=\"10\" name=\"wild\" count=\"4\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"10\" name=\"wild\" count=\"5\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"200\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"2,3,2,1,2\"/></paylines><symbols><symbol id=\"1\" title=\"10\"/><symbol id=\"2\" title=\"j\"/><symbol id=\"3\" title=\"q\"/><symbol id=\"4\" title=\"k\"/><symbol id=\"5\" title=\"a\"/><symbol id=\"6\" title=\"med2\"/><symbol id=\"7\" title=\"med1\"/><symbol id=\"8\" title=\"top2\"/><symbol id=\"9\" title=\"top1\"/><symbol id=\"10\" title=\"wild\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"4,5,7,2,1,6,9,3,8,10\"/><reel id=\"2\" length=\"10\" symbols=\"1,10,2,4,9,3,7,5,8,6\"/><reel id=\"3\" length=\"10\" symbols=\"1,2,9,3,6,4,10,5,7,8\"/><reel id=\"4\" length=\"10\" symbols=\"1,8,3,5,4,7,2,6,9,10\"/><reel id=\"5\" length=\"10\" symbols=\"1,3,9,2,8,5,7,4,6,10\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"5,2,6,4,8,1,9,10,3,7\"/><reel id=\"2\" length=\"10\" symbols=\"8,1,3,6,2,10,4,7,5,9\"/><reel id=\"3\" length=\"10\" symbols=\"2,5,6,3,1,4,10,7,9,8\"/><reel id=\"4\" length=\"10\" symbols=\"2,10,1,4,7,3,9,6,5,8\"/><reel id=\"5\" length=\"10\" symbols=\"6,3,7,1,4,9,5,8,2,10\"/></reels></slot><limit id=\"freespins_seq\" max=\"50\"/><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"4,7,2,4,9\" reel2=\"3,6,5,2,8\" reel3=\"5,8,3,4,8\" reel4=\"2,5,10,4,3\" reel5=\"1,8,4,2,6\"/><delivery id=\"1903117-147150221721903436943215\" action=\"create\"/></server>";
            }
        }
        #endregion
        public BookOfGoldGameLogic()
        {
            _gameID = GAMEID.BookOfGold;
            GameName = "BookOfGold";
        }
    }
}
