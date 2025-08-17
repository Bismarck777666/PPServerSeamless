using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BookOfGoldClassicGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "book_of_gold_classic";
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
                return "<server><source game-ver=\"40419\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"10\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"10\" count=\"4\" coef=\"25\"/><combination symbol=\"1\" name=\"10\" count=\"5\" coef=\"100\"/><combination symbol=\"2\" name=\"j\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"j\" count=\"4\" coef=\"25\"/><combination symbol=\"2\" name=\"j\" count=\"5\" coef=\"100\"/><combination symbol=\"3\" name=\"q\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"q\" count=\"4\" coef=\"25\"/><combination symbol=\"3\" name=\"q\" count=\"5\" coef=\"100\"/><combination symbol=\"4\" name=\"k\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"k\" count=\"4\" coef=\"40\"/><combination symbol=\"4\" name=\"k\" count=\"5\" coef=\"150\"/><combination symbol=\"5\" name=\"a\" count=\"3\" coef=\"5\"/><combination symbol=\"5\" name=\"a\" count=\"4\" coef=\"40\"/><combination symbol=\"5\" name=\"a\" count=\"5\" coef=\"150\"/><combination symbol=\"6\" name=\"med2\" count=\"2\" coef=\"5\"/><combination symbol=\"6\" name=\"med2\" count=\"3\" coef=\"30\"/><combination symbol=\"6\" name=\"med2\" count=\"4\" coef=\"100\"/><combination symbol=\"6\" name=\"med2\" count=\"5\" coef=\"750\"/><combination symbol=\"7\" name=\"med1\" count=\"2\" coef=\"5\"/><combination symbol=\"7\" name=\"med1\" count=\"3\" coef=\"30\"/><combination symbol=\"7\" name=\"med1\" count=\"4\" coef=\"100\"/><combination symbol=\"7\" name=\"med1\" count=\"5\" coef=\"750\"/><combination symbol=\"8\" name=\"top2\" count=\"2\" coef=\"5\"/><combination symbol=\"8\" name=\"top2\" count=\"3\" coef=\"40\"/><combination symbol=\"8\" name=\"top2\" count=\"4\" coef=\"400\"/><combination symbol=\"8\" name=\"top2\" count=\"5\" coef=\"2000\"/><combination symbol=\"9\" name=\"top1\" count=\"2\" coef=\"10\"/><combination symbol=\"9\" name=\"top1\" count=\"3\" coef=\"100\"/><combination symbol=\"9\" name=\"top1\" count=\"4\" coef=\"1000\"/><combination symbol=\"9\" name=\"top1\" count=\"5\" coef=\"5000\"/><combination symbol=\"10\" name=\"wild\" count=\"3\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"2\"/><combination symbol=\"10\" name=\"wild\" count=\"4\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"20\"/><combination symbol=\"10\" name=\"wild\" count=\"5\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"200\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"2,3,2,1,2\"/></paylines><symbols><symbol id=\"1\" title=\"10\"/><symbol id=\"2\" title=\"j\"/><symbol id=\"3\" title=\"q\"/><symbol id=\"4\" title=\"k\"/><symbol id=\"5\" title=\"a\"/><symbol id=\"6\" title=\"med2\"/><symbol id=\"7\" title=\"med1\"/><symbol id=\"8\" title=\"top2\"/><symbol id=\"9\" title=\"top1\"/><symbol id=\"10\" title=\"wild\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"5,3,4,7,6,2,1,8,9,10\"/><reel id=\"2\" length=\"10\" symbols=\"2,3,10,5,1,8,4,6,9,7\"/><reel id=\"3\" length=\"10\" symbols=\"5,1,6,2,3,7,10,4,8,9\"/><reel id=\"4\" length=\"10\" symbols=\"3,4,10,5,6,2,1,7,8,9\"/><reel id=\"5\" length=\"10\" symbols=\"1,10,3,2,8,9,5,4,7,6\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"9,5,6,3,2,8,10,4,7,1\"/><reel id=\"2\" length=\"10\" symbols=\"10,4,3,5,1,7,2,8,6,9\"/><reel id=\"3\" length=\"10\" symbols=\"2,5,10,1,4,8,3,7,6,9\"/><reel id=\"4\" length=\"10\" symbols=\"1,6,3,2,10,4,8,9,5,7\"/><reel id=\"5\" length=\"10\" symbols=\"5,7,1,2,10,4,8,9,6,3\"/></reels></slot><limit id=\"freespins_seq\" max=\"50\"/><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"4,3,7,5,1\" reel2=\"7,4,2,8,3\" reel3=\"3,7,5,6,2\" reel4=\"4,5,3,8,5\" reel5=\"5,1,7,4,2\"/><delivery id=\"1781611-209168155482444726383364\" action=\"create\"/></server>";
            }
        }
        #endregion
        public BookOfGoldClassicGameLogic()
        {
            _gameID = GAMEID.BookOfGoldClassic;
            GameName = "BookOfGoldClassic";
        }
    }
}
