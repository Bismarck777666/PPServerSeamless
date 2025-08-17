using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class UltraFortGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "ultra_fort";
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
                return "<server><source game-ver=\"250820\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"top\" count=\"2\" coef=\"10\"/><combination symbol=\"1\" name=\"top\" count=\"3\" coef=\"50\"/><combination symbol=\"1\" name=\"top\" count=\"4\" coef=\"200\"/><combination symbol=\"1\" name=\"top\" count=\"5\" coef=\"1000\"/><combination symbol=\"2\" name=\"mid3\" count=\"3\" coef=\"40\"/><combination symbol=\"2\" name=\"mid3\" count=\"4\" coef=\"100\"/><combination symbol=\"2\" name=\"mid3\" count=\"5\" coef=\"400\"/><combination symbol=\"3\" name=\"mid2\" count=\"3\" coef=\"20\"/><combination symbol=\"3\" name=\"mid2\" count=\"4\" coef=\"50\"/><combination symbol=\"3\" name=\"mid2\" count=\"5\" coef=\"200\"/><combination symbol=\"4\" name=\"mid1\" count=\"3\" coef=\"20\"/><combination symbol=\"4\" name=\"mid1\" count=\"4\" coef=\"50\"/><combination symbol=\"4\" name=\"mid1\" count=\"5\" coef=\"200\"/><combination symbol=\"5\" name=\"low4\" count=\"3\" coef=\"10\"/><combination symbol=\"5\" name=\"low4\" count=\"4\" coef=\"30\"/><combination symbol=\"5\" name=\"low4\" count=\"5\" coef=\"100\"/><combination symbol=\"6\" name=\"low3\" count=\"3\" coef=\"10\"/><combination symbol=\"6\" name=\"low3\" count=\"4\" coef=\"30\"/><combination symbol=\"6\" name=\"low3\" count=\"5\" coef=\"100\"/><combination symbol=\"7\" name=\"low2\" count=\"3\" coef=\"10\"/><combination symbol=\"7\" name=\"low2\" count=\"4\" coef=\"30\"/><combination symbol=\"7\" name=\"low2\" count=\"5\" coef=\"100\"/><combination symbol=\"8\" name=\"low1\" count=\"3\" coef=\"10\"/><combination symbol=\"8\" name=\"low1\" count=\"4\" coef=\"30\"/><combination symbol=\"8\" name=\"low1\" count=\"5\" coef=\"100\"/><combination symbol=\"9\" name=\"bonus\" count=\"3\" coef=\"0\"/><combination symbol=\"9\" name=\"bonus\" count=\"4\" coef=\"0\"/><combination symbol=\"9\" name=\"bonus\" count=\"5\" coef=\"0\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"top\"/><symbol id=\"2\" title=\"mid3\"/><symbol id=\"3\" title=\"mid2\"/><symbol id=\"4\" title=\"mid1\"/><symbol id=\"5\" title=\"low4\"/><symbol id=\"6\" title=\"low3\"/><symbol id=\"7\" title=\"low2\"/><symbol id=\"8\" title=\"low1\"/><symbol id=\"9\" title=\"bonus\"/><symbol id=\"10\" title=\"mystery\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"8\" symbols=\"8,2,6,3,1,7,5,4\"/><reel id=\"2\" length=\"8\" symbols=\"7,2,5,4,8,3,6,1\"/><reel id=\"3\" length=\"9\" symbols=\"6,2,7,1,5,8,4,3,9\"/><reel id=\"4\" length=\"8\" symbols=\"8,2,7,1,6,5,4,3\"/><reel id=\"5\" length=\"8\" symbols=\"8,2,7,1,6,5,4,3\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"8\" symbols=\"8,2,5,1,6,3,7,4\"/><reel id=\"2\" length=\"8\" symbols=\"7,2,5,1,6,4,3,8\"/><reel id=\"3\" length=\"9\" symbols=\"6,2,7,1,5,3,8,4,9\"/><reel id=\"4\" length=\"8\" symbols=\"8,2,7,1,6,5,4,3\"/><reel id=\"5\" length=\"8\" symbols=\"8,2,7,1,6,5,4,3\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"4,4,7\" reel2=\"5,3,3\" reel3=\"6,1,5\" reel4=\"8,2,2\" reel5=\"4,4,8\"/><game bonus_spins=\"3\" total_bet_mult=\"10\"/><delivery id=\"440098-7950873680159902642384880\" action=\"create\"/></server>";
            }
        }
        #endregion
        public UltraFortGameLogic()
        {
            _gameID = GAMEID.UltraFort;
            GameName = "UltraFort";
        }
    }
}
