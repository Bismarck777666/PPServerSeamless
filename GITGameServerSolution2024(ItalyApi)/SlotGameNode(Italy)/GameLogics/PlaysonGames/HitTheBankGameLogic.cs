using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class HitTheBankGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "hit_the_bank";
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
                return "<server><source game-ver=\"81221\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"top\" count=\"3\" coef=\"8\"/><combination symbol=\"1\" name=\"top\" count=\"4\" coef=\"40\"/><combination symbol=\"1\" name=\"top\" count=\"5\" coef=\"200\"/><combination symbol=\"2\" name=\"mid1\" count=\"3\" coef=\"6\"/><combination symbol=\"2\" name=\"mid1\" count=\"4\" coef=\"30\"/><combination symbol=\"2\" name=\"mid1\" count=\"5\" coef=\"100\"/><combination symbol=\"3\" name=\"mid2\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"mid2\" count=\"4\" coef=\"25\"/><combination symbol=\"3\" name=\"mid2\" count=\"5\" coef=\"90\"/><combination symbol=\"4\" name=\"mid3\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"mid3\" count=\"4\" coef=\"20\"/><combination symbol=\"4\" name=\"mid3\" count=\"5\" coef=\"80\"/><combination symbol=\"5\" name=\"low1\" count=\"3\" coef=\"2\"/><combination symbol=\"5\" name=\"low1\" count=\"4\" coef=\"10\"/><combination symbol=\"5\" name=\"low1\" count=\"5\" coef=\"50\"/><combination symbol=\"6\" name=\"low2\" count=\"3\" coef=\"2\"/><combination symbol=\"6\" name=\"low2\" count=\"4\" coef=\"10\"/><combination symbol=\"6\" name=\"low2\" count=\"5\" coef=\"40\"/><combination symbol=\"7\" name=\"low3\" count=\"3\" coef=\"2\"/><combination symbol=\"7\" name=\"low3\" count=\"4\" coef=\"7\"/><combination symbol=\"7\" name=\"low3\" count=\"5\" coef=\"35\"/><combination symbol=\"8\" name=\"low4\" count=\"3\" coef=\"2\"/><combination symbol=\"8\" name=\"low4\" count=\"4\" coef=\"7\"/><combination symbol=\"8\" name=\"low4\" count=\"5\" coef=\"30\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"8\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"40\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"200\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"4,4,4,4,4\"/><payline id=\"5\" path=\"1,2,3,2,1\"/><payline id=\"6\" path=\"2,3,4,3,2\"/><payline id=\"7\" path=\"4,3,2,3,4\"/><payline id=\"8\" path=\"3,2,1,2,3\"/><payline id=\"9\" path=\"1,1,2,1,1\"/><payline id=\"10\" path=\"2,2,3,2,2\"/><payline id=\"11\" path=\"3,3,4,3,3\"/><payline id=\"12\" path=\"4,4,3,4,4\"/><payline id=\"13\" path=\"3,3,2,3,3\"/><payline id=\"14\" path=\"2,2,1,2,2\"/><payline id=\"15\" path=\"2,1,1,1,2\"/><payline id=\"16\" path=\"3,2,2,2,3\"/><payline id=\"17\" path=\"4,3,3,3,4\"/><payline id=\"18\" path=\"3,4,4,4,3\"/><payline id=\"19\" path=\"2,3,3,3,2\"/><payline id=\"20\" path=\"1,2,2,2,1\"/><payline id=\"21\" path=\"2,1,2,1,2\"/><payline id=\"22\" path=\"3,2,3,2,3\"/><payline id=\"23\" path=\"4,3,4,3,4\"/><payline id=\"24\" path=\"3,4,3,4,3\"/><payline id=\"25\" path=\"2,3,2,3,2\"/><payline id=\"26\" path=\"1,2,1,2,1\"/><payline id=\"27\" path=\"1,1,2,3,3\"/><payline id=\"28\" path=\"2,2,3,4,4\"/><payline id=\"29\" path=\"4,4,3,2,2\"/><payline id=\"30\" path=\"3,3,2,1,1\"/><payline id=\"31\" path=\"2,3,2,1,2\"/><payline id=\"32\" path=\"3,4,3,2,3\"/><payline id=\"33\" path=\"3,2,3,4,3\"/><payline id=\"34\" path=\"2,1,2,3,2\"/><payline id=\"35\" path=\"1,3,3,3,1\"/><payline id=\"36\" path=\"2,4,4,4,2\"/><payline id=\"37\" path=\"4,2,2,2,4\"/><payline id=\"38\" path=\"3,1,1,1,3\"/><payline id=\"39\" path=\"1,2,3,4,3\"/><payline id=\"40\" path=\"4,3,2,1,2\"/></paylines><symbols><symbol id=\"1\" title=\"top\"/><symbol id=\"2\" title=\"mid1\"/><symbol id=\"3\" title=\"mid2\"/><symbol id=\"4\" title=\"mid3\"/><symbol id=\"5\" title=\"low1\"/><symbol id=\"6\" title=\"low2\"/><symbol id=\"7\" title=\"low3\"/><symbol id=\"8\" title=\"low4\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"sc\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"1,5,2,6,7,3,8,10,4,9\"/><reel id=\"2\" length=\"10\" symbols=\"1,2,5,8,7,9,6,3,10,4\"/><reel id=\"3\" length=\"10\" symbols=\"1,8,3,7,6,4,10,9,2,5\"/><reel id=\"4\" length=\"10\" symbols=\"1,7,3,6,5,4,8,2,10,9\"/><reel id=\"5\" length=\"10\" symbols=\"1,3,7,10,8,5,4,2,9,6\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"6\" symbols=\"10,5,6,7,8,1\"/><reel id=\"2\" length=\"4\" symbols=\"1,2,4,3\"/><reel id=\"3\" length=\"6\" symbols=\"10,5,6,7,8,2\"/><reel id=\"4\" length=\"9\" symbols=\"1,2,3,4,9,5,6,7,8\"/><reel id=\"5\" length=\"6\" symbols=\"10,8,6,5,7,3\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"3,1,9,5\" reel2=\"7,10,5,6\" reel3=\"1,1,8,3\" reel4=\"2,3,9,3\" reel5=\"2,2,4,1\"/><game total_bet_mult=\"10\"/><delivery id=\"281375-2888082137445290637224489\" action=\"create\"/></server>";
            }
        }
        #endregion
        public HitTheBankGameLogic()
        {
            _gameID = GAMEID.HitTheBank;
            GameName = "HitTheBank";
        }
    }
}
