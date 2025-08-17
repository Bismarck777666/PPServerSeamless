using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BuffaloMegawaysGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "buffalo_megaways";
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
                return "<server><source game-ver=\"230421\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"top1\" count=\"3\" coef=\"12\"/><combination symbol=\"1\" name=\"top1\" count=\"4\" coef=\"20\"/><combination symbol=\"1\" name=\"top1\" count=\"5\" coef=\"120\"/><combination symbol=\"1\" name=\"top1\" count=\"6\" coef=\"400\"/><combination symbol=\"2\" name=\"top2\" count=\"3\" coef=\"10\"/><combination symbol=\"2\" name=\"top2\" count=\"4\" coef=\"18\"/><combination symbol=\"2\" name=\"top2\" count=\"5\" coef=\"100\"/><combination symbol=\"2\" name=\"top2\" count=\"6\" coef=\"300\"/><combination symbol=\"3\" name=\"mid1\" count=\"3\" coef=\"8\"/><combination symbol=\"3\" name=\"mid1\" count=\"4\" coef=\"16\"/><combination symbol=\"3\" name=\"mid1\" count=\"5\" coef=\"80\"/><combination symbol=\"3\" name=\"mid1\" count=\"6\" coef=\"200\"/><combination symbol=\"4\" name=\"mid2\" count=\"3\" coef=\"6\"/><combination symbol=\"4\" name=\"mid2\" count=\"4\" coef=\"14\"/><combination symbol=\"4\" name=\"mid2\" count=\"5\" coef=\"60\"/><combination symbol=\"4\" name=\"mid2\" count=\"6\" coef=\"100\"/><combination symbol=\"5\" name=\"low1\" count=\"3\" coef=\"5\"/><combination symbol=\"5\" name=\"low1\" count=\"4\" coef=\"8\"/><combination symbol=\"5\" name=\"low1\" count=\"5\" coef=\"10\"/><combination symbol=\"5\" name=\"low1\" count=\"6\" coef=\"20\"/><combination symbol=\"6\" name=\"low2\" count=\"3\" coef=\"5\"/><combination symbol=\"6\" name=\"low2\" count=\"4\" coef=\"8\"/><combination symbol=\"6\" name=\"low2\" count=\"5\" coef=\"10\"/><combination symbol=\"6\" name=\"low2\" count=\"6\" coef=\"20\"/><combination symbol=\"7\" name=\"low3\" count=\"3\" coef=\"5\"/><combination symbol=\"7\" name=\"low3\" count=\"4\" coef=\"8\"/><combination symbol=\"7\" name=\"low3\" count=\"5\" coef=\"10\"/><combination symbol=\"7\" name=\"low3\" count=\"6\" coef=\"20\"/><combination symbol=\"8\" name=\"low4\" count=\"3\" coef=\"5\"/><combination symbol=\"8\" name=\"low4\" count=\"4\" coef=\"8\"/><combination symbol=\"8\" name=\"low4\" count=\"5\" coef=\"10\"/><combination symbol=\"8\" name=\"low4\" count=\"6\" coef=\"20\"/><combination symbol=\"9\" name=\"wild\" count=\"2\" coef=\"20\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"40\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"100\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"200\"/><combination symbol=\"9\" name=\"wild\" count=\"6\" coef=\"1000\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"4\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"4\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"5\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"5\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"6\" coef=\"0\"/><combination symbol=\"10\" name=\"scatter\" count=\"6\" coef=\"0\"/></combinations><symbols><symbol id=\"1\" title=\"top1\"/><symbol id=\"2\" title=\"top2\"/><symbol id=\"3\" title=\"mid1\"/><symbol id=\"4\" title=\"mid2\"/><symbol id=\"5\" title=\"low1\"/><symbol id=\"6\" title=\"low2\"/><symbol id=\"7\" title=\"low3\"/><symbol id=\"8\" title=\"low4\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"scatter\"/><symbol id=\"11\" title=\"bonus\"/><symbol id=\"12\" title=\"power\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"8\" symbols=\"7,3,6,8,2,5,1,4\"/><reel id=\"2\" length=\"8\" symbols=\"8,2,3,5,1,7,6,4\"/><reel id=\"3\" length=\"8\" symbols=\"7,2,4,6,1,3,5,8\"/><reel id=\"4\" length=\"8\" symbols=\"4,1,5,7,6,3,8,2\"/><reel id=\"5\" length=\"8\" symbols=\"1,6,8,4,3,2,5,7\"/><reel id=\"6\" length=\"8\" symbols=\"4,8,3,1,5,7,2,6\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"8\" symbols=\"7,3,6,8,2,5,1,4\"/><reel id=\"2\" length=\"8\" symbols=\"8,2,3,5,1,7,6,4\"/><reel id=\"3\" length=\"8\" symbols=\"7,2,4,6,1,3,5,8\"/><reel id=\"4\" length=\"8\" symbols=\"4,1,5,7,6,3,8,2\"/><reel id=\"5\" length=\"8\" symbols=\"1,6,8,4,3,2,5,7\"/><reel id=\"6\" length=\"8\" symbols=\"4,8,3,1,5,7,2,6\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"8\" symbols=\"7,3,6,8,2,5,1,4\"/><reel id=\"2\" length=\"8\" symbols=\"8,2,3,5,1,7,6,4\"/><reel id=\"3\" length=\"8\" symbols=\"7,2,4,6,1,3,5,8\"/><reel id=\"4\" length=\"8\" symbols=\"4,1,5,7,6,3,8,2\"/><reel id=\"5\" length=\"8\" symbols=\"1,6,8,4,3,2,5,7\"/><reel id=\"6\" length=\"8\" symbols=\"4,8,3,1,5,7,2,6\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"8\" symbols=\"7,3,6,8,2,5,1,4\"/><reel id=\"2\" length=\"8\" symbols=\"8,2,3,5,1,7,6,4\"/><reel id=\"3\" length=\"8\" symbols=\"7,2,4,6,1,3,5,8\"/><reel id=\"4\" length=\"8\" symbols=\"4,1,5,7,6,3,8,2\"/><reel id=\"5\" length=\"8\" symbols=\"1,6,8,4,3,2,5,7\"/><reel id=\"6\" length=\"8\" symbols=\"4,8,3,1,5,7,2,6\"/></reels></slot><shift server=\"0,0,0,0,0,0\" reel_set=\"1\" reel1=\"5,10,6\" reel2=\"9,2,3,1\" reel3=\"12,11,12\" reel4=\"9,7\" reel5=\"3,4,9\" reel6=\"7,3,11,11\"><bonus bonus_pos=\"2,8,14,17,23\" bonus_tb=\"0,3,0,4,8\" bonus_type=\"1,0,1,0,0\"/></shift><game power_tb=\"5,20,50,150,500,2000\" bonus_spins=\"3\" total_bet_mult=\"20\"/><delivery id=\"486894-2861356573989460062743891\" action=\"create\"/></server>";
            }
        }
        #endregion
        public BuffaloMegawaysGameLogic()
        {
            _gameID = GAMEID.BuffaloMegaways;
            GameName = "BuffaloMegaways";
        }
    }
}
