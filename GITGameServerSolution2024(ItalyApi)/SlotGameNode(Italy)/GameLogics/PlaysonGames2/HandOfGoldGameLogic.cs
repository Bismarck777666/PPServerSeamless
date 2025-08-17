using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class HandOfGoldGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "hand_of_gold";
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
                return "<server><source game-ver=\"110321\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"wild\" count=\"3\" coef=\"4\"/><combination symbol=\"1\" name=\"wild\" count=\"4\" coef=\"10\"/><combination symbol=\"1\" name=\"wild\" count=\"5\" coef=\"25\"/><combination symbol=\"1\" name=\"wild\" count=\"6\" coef=\"200\"/><combination symbol=\"2\" name=\"top1\" count=\"3\" coef=\"3\"/><combination symbol=\"2\" name=\"top1\" count=\"4\" coef=\"6\"/><combination symbol=\"2\" name=\"top1\" count=\"5\" coef=\"12\"/><combination symbol=\"2\" name=\"top1\" count=\"6\" coef=\"40\"/><combination symbol=\"3\" name=\"top2\" count=\"3\" coef=\"3\"/><combination symbol=\"3\" name=\"top2\" count=\"4\" coef=\"6\"/><combination symbol=\"3\" name=\"top2\" count=\"5\" coef=\"12\"/><combination symbol=\"3\" name=\"top2\" count=\"6\" coef=\"40\"/><combination symbol=\"4\" name=\"mid1\" count=\"3\" coef=\"2\"/><combination symbol=\"4\" name=\"mid1\" count=\"4\" coef=\"3\"/><combination symbol=\"4\" name=\"mid1\" count=\"5\" coef=\"8\"/><combination symbol=\"4\" name=\"mid1\" count=\"6\" coef=\"20\"/><combination symbol=\"5\" name=\"mid2\" count=\"3\" coef=\"2\"/><combination symbol=\"5\" name=\"mid2\" count=\"4\" coef=\"3\"/><combination symbol=\"5\" name=\"mid2\" count=\"5\" coef=\"8\"/><combination symbol=\"5\" name=\"mid2\" count=\"6\" coef=\"20\"/><combination symbol=\"6\" name=\"low1\" count=\"3\" coef=\"1\"/><combination symbol=\"6\" name=\"low1\" count=\"4\" coef=\"2\"/><combination symbol=\"6\" name=\"low1\" count=\"5\" coef=\"5\"/><combination symbol=\"6\" name=\"low1\" count=\"6\" coef=\"10\"/><combination symbol=\"7\" name=\"low2\" count=\"3\" coef=\"1\"/><combination symbol=\"7\" name=\"low2\" count=\"4\" coef=\"2\"/><combination symbol=\"7\" name=\"low2\" count=\"5\" coef=\"5\"/><combination symbol=\"7\" name=\"low2\" count=\"6\" coef=\"10\"/><combination symbol=\"8\" name=\"low3\" count=\"3\" coef=\"1\"/><combination symbol=\"8\" name=\"low3\" count=\"4\" coef=\"2\"/><combination symbol=\"8\" name=\"low3\" count=\"5\" coef=\"5\"/><combination symbol=\"8\" name=\"low3\" count=\"6\" coef=\"10\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"2\"/><combination symbol=\"10\" name=\"scatter\" count=\"4\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"10\"/><combination symbol=\"10\" name=\"scatter\" count=\"5\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"50\"/><combination symbol=\"10\" name=\"scatter\" count=\"6\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"200\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3,3\"/><payline id=\"4\" path=\"4,4,4,4,4,4\"/><payline id=\"5\" path=\"1,2,3,3,2,1\"/><payline id=\"6\" path=\"2,3,4,4,3,2\"/><payline id=\"7\" path=\"3,2,1,1,2,3\"/><payline id=\"8\" path=\"4,3,2,2,3,4\"/><payline id=\"9\" path=\"1,2,2,2,2,1\"/><payline id=\"10\" path=\"2,3,3,3,3,2\"/><payline id=\"11\" path=\"3,4,4,4,4,3\"/><payline id=\"12\" path=\"2,1,1,1,1,2\"/><payline id=\"13\" path=\"3,2,2,2,2,3\"/><payline id=\"14\" path=\"4,3,3,3,3,4\"/><payline id=\"15\" path=\"1,1,2,2,1,1\"/><payline id=\"16\" path=\"2,2,3,3,2,2\"/><payline id=\"17\" path=\"3,3,4,4,3,3\"/><payline id=\"18\" path=\"2,2,1,1,2,2\"/><payline id=\"19\" path=\"3,3,2,2,3,3\"/><payline id=\"20\" path=\"4,4,3,3,4,4\"/></paylines><symbols><symbol id=\"1\" title=\"wild\"/><symbol id=\"2\" title=\"top1\"/><symbol id=\"3\" title=\"top2\"/><symbol id=\"4\" title=\"mid1\"/><symbol id=\"5\" title=\"mid2\"/><symbol id=\"6\" title=\"low1\"/><symbol id=\"7\" title=\"low2\"/><symbol id=\"8\" title=\"low3\"/><symbol id=\"9\" title=\"wildactivator\"/><symbol id=\"10\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"8\" symbols=\"2,6,3,7,8,5,4,10\"/><reel id=\"2\" length=\"8\" symbols=\"2,6,5,4,3,7,8,10\"/><reel id=\"3\" length=\"8\" symbols=\"2,5,4,6,3,7,8,10\"/><reel id=\"4\" length=\"8\" symbols=\"2,6,10,7,3,5,8,4\"/><reel id=\"5\" length=\"8\" symbols=\"2,5,7,4,6,3,8,10\"/><reel id=\"6\" length=\"8\" symbols=\"2,6,3,8,5,4,7,10\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"8\" symbols=\"2,6,3,7,8,5,4,10\"/><reel id=\"2\" length=\"8\" symbols=\"2,6,5,4,3,7,8,10\"/><reel id=\"3\" length=\"8\" symbols=\"2,5,4,6,3,7,8,10\"/><reel id=\"4\" length=\"8\" symbols=\"2,6,7,3,5,8,4,10\"/><reel id=\"5\" length=\"8\" symbols=\"2,5,7,4,6,3,8,10\"/><reel id=\"6\" length=\"8\" symbols=\"2,6,3,8,5,4,7,10\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"8\" symbols=\"2,6,3,7,8,5,4,10\"/><reel id=\"2\" length=\"8\" symbols=\"2,6,5,4,3,7,8,10\"/><reel id=\"3\" length=\"8\" symbols=\"2,5,4,6,3,7,8,10\"/><reel id=\"4\" length=\"8\" symbols=\"2,6,10,7,3,5,8,4\"/><reel id=\"5\" length=\"8\" symbols=\"2,5,7,4,6,3,8,10\"/><reel id=\"6\" length=\"8\" symbols=\"2,6,3,8,5,4,7,10\"/></reels></slot><shift server=\"0,0,0,0,0,0\" reel_set=\"1\" reel1=\"8,8,3,6\" reel2=\"7,2,8,8\" reel3=\"6,6,10,7\" reel4=\"5,3,6,6\" reel5=\"4,4,2,8\" reel6=\"8,10,7,7\"/><game total_bet_mult=\"10\"/><delivery id=\"496212-7186670570463365280349015\" action=\"create\"/></server>";
            }
        }
        #endregion
        public HandOfGoldGameLogic()
        {
            _gameID = GAMEID.HandOfGold;
            GameName = "HandOfGold";
        }
    }
}
