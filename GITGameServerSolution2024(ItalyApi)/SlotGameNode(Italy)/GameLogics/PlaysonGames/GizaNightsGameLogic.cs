using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class GizaNightsGameLogic : BasePlaysonHillSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "giza_nights";
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
                return "<server><source game-ver=\"270622\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"low4\" count=\"3\" coef=\"4\"/><combination symbol=\"1\" name=\"low4\" count=\"4\" coef=\"8\"/><combination symbol=\"1\" name=\"low4\" count=\"5\" coef=\"40\"/><combination symbol=\"2\" name=\"low3\" count=\"3\" coef=\"4\"/><combination symbol=\"2\" name=\"low3\" count=\"4\" coef=\"8\"/><combination symbol=\"2\" name=\"low3\" count=\"5\" coef=\"40\"/><combination symbol=\"3\" name=\"low2\" count=\"3\" coef=\"4\"/><combination symbol=\"3\" name=\"low2\" count=\"4\" coef=\"8\"/><combination symbol=\"3\" name=\"low2\" count=\"5\" coef=\"40\"/><combination symbol=\"4\" name=\"low1\" count=\"3\" coef=\"4\"/><combination symbol=\"4\" name=\"low1\" count=\"4\" coef=\"8\"/><combination symbol=\"4\" name=\"low1\" count=\"5\" coef=\"40\"/><combination symbol=\"5\" name=\"mid4\" count=\"3\" coef=\"4\"/><combination symbol=\"5\" name=\"mid4\" count=\"4\" coef=\"16\"/><combination symbol=\"5\" name=\"mid4\" count=\"5\" coef=\"80\"/><combination symbol=\"6\" name=\"mid3\" count=\"3\" coef=\"4\"/><combination symbol=\"6\" name=\"mid3\" count=\"4\" coef=\"20\"/><combination symbol=\"6\" name=\"mid3\" count=\"5\" coef=\"120\"/><combination symbol=\"7\" name=\"mid2\" count=\"3\" coef=\"4\"/><combination symbol=\"7\" name=\"mid2\" count=\"4\" coef=\"24\"/><combination symbol=\"7\" name=\"mid2\" count=\"5\" coef=\"160\"/><combination symbol=\"8\" name=\"mid1\" count=\"3\" coef=\"8\"/><combination symbol=\"8\" name=\"mid1\" count=\"4\" coef=\"32\"/><combination symbol=\"8\" name=\"mid1\" count=\"5\" coef=\"200\"/><combination symbol=\"9\" name=\"wild\" count=\"3\" coef=\"12\"/><combination symbol=\"9\" name=\"wild\" count=\"4\" coef=\"48\"/><combination symbol=\"9\" name=\"wild\" count=\"5\" coef=\"240\"/><combination symbol=\"10\" name=\"scatter\" count=\"3\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"1\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"2,3,2,1,2\"/><payline id=\"11\" path=\"2,1,2,3,2\"/><payline id=\"12\" path=\"1,2,2,2,1\"/><payline id=\"13\" path=\"3,2,2,2,3\"/><payline id=\"14\" path=\"1,2,1,2,1\"/><payline id=\"15\" path=\"3,2,3,2,3\"/><payline id=\"16\" path=\"2,2,1,2,2\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"1,1,3,1,1\"/><payline id=\"19\" path=\"3,3,1,3,3\"/><payline id=\"20\" path=\"1,3,3,3,1\"/><payline id=\"21\" path=\"3,1,1,1,3\"/><payline id=\"22\" path=\"2,3,1,3,2\"/><payline id=\"23\" path=\"2,1,3,1,2\"/><payline id=\"24\" path=\"1,3,1,3,1\"/><payline id=\"25\" path=\"3,1,3,1,3\"/></paylines><symbols><symbol id=\"1\" title=\"low4\"/><symbol id=\"2\" title=\"low3\"/><symbol id=\"3\" title=\"low2\"/><symbol id=\"4\" title=\"low1\"/><symbol id=\"5\" title=\"mid4\"/><symbol id=\"6\" title=\"mid3\"/><symbol id=\"7\" title=\"mid2\"/><symbol id=\"8\" title=\"mid1\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"scatter\"/><symbol id=\"11\" title=\"bonus\"/><symbol id=\"12\" title=\"boost\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"6,5,1,7,11,3,2,4,8,9\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,5,10,2,7,6,3,11,4\"/><reel id=\"3\" length=\"12\" symbols=\"8,6,9,3,10,5,11,12,2,4,7,1\"/><reel id=\"4\" length=\"11\" symbols=\"8,9,1,4,7,10,5,2,3,11,6\"/><reel id=\"5\" length=\"10\" symbols=\"8,1,3,5,7,4,2,6,9,11\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"6,5,1,7,11,3,2,4,8,9\"/><reel id=\"2\" length=\"11\" symbols=\"9,1,8,5,10,2,7,6,3,11,4\"/><reel id=\"3\" length=\"12\" symbols=\"8,6,9,3,10,5,11,12,2,4,7,1\"/><reel id=\"4\" length=\"11\" symbols=\"8,9,1,11,4,7,10,5,2,3,6\"/><reel id=\"5\" length=\"10\" symbols=\"9,8,1,3,5,7,4,2,11,6\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"6\" symbols=\"7,6,5,11,8,9\"/><reel id=\"2\" length=\"7\" symbols=\"5,8,7,10,6,9,11\"/><reel id=\"3\" length=\"8\" symbols=\"5,9,7,6,8,10,11,12\"/><reel id=\"4\" length=\"7\" symbols=\"5,9,7,10,6,8,11\"/><reel id=\"5\" length=\"6\" symbols=\"6,9,8,5,7,11\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"5\" symbols=\"7,6,5,8,9\"/><reel id=\"2\" length=\"6\" symbols=\"5,8,7,10,6,9\"/><reel id=\"3\" length=\"6\" symbols=\"5,9,7,6,8,10\"/><reel id=\"4\" length=\"6\" symbols=\"5,9,7,10,6,8\"/><reel id=\"5\" length=\"5\" symbols=\"6,9,8,5,7\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"4,4,4\" reel2=\"8,8,8\" reel3=\"1,1,1\" reel4=\"7,7,7\" reel5=\"2,2,2\"/><game jackpots_tb=\"20,50,150,5000\" bonus_spins=\"3\" total_bet_mult=\"20\"/><delivery id=\"211546-1061964181692332858395233\" action=\"create\"/></server>";
            }
        }
        #endregion
        public GizaNightsGameLogic()
        {
            _gameID     = GAMEID.GizaNights;
            GameName    = "GizaNights";
        }
    }
}
