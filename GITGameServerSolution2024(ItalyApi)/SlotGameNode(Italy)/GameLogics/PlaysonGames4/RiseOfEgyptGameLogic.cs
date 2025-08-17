using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class RiseOfEgyptGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "rise_of_egypt";
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
                return "<server><source game-ver=\"57764\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"J\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"J\" count=\"4\" coef=\"20\"/><combination symbol=\"1\" name=\"J\" count=\"5\" coef=\"100\"/><combination symbol=\"2\" name=\"Q\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"Q\" count=\"4\" coef=\"20\"/><combination symbol=\"2\" name=\"Q\" count=\"5\" coef=\"100\"/><combination symbol=\"3\" name=\"K\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"K\" count=\"4\" coef=\"20\"/><combination symbol=\"3\" name=\"K\" count=\"5\" coef=\"100\"/><combination symbol=\"4\" name=\"A\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"A\" count=\"4\" coef=\"20\"/><combination symbol=\"4\" name=\"A\" count=\"5\" coef=\"100\"/><combination symbol=\"5\" name=\"mid1\" count=\"3\" coef=\"10\"/><combination symbol=\"5\" name=\"mid1\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"mid1\" count=\"5\" coef=\"150\"/><combination symbol=\"6\" name=\"mid2\" count=\"3\" coef=\"10\"/><combination symbol=\"6\" name=\"mid2\" count=\"4\" coef=\"50\"/><combination symbol=\"6\" name=\"mid2\" count=\"5\" coef=\"150\"/><combination symbol=\"7\" name=\"mid3\" count=\"3\" coef=\"20\"/><combination symbol=\"7\" name=\"mid3\" count=\"4\" coef=\"100\"/><combination symbol=\"7\" name=\"mid3\" count=\"5\" coef=\"200\"/><combination symbol=\"8\" name=\"mid4\" count=\"3\" coef=\"20\"/><combination symbol=\"8\" name=\"mid4\" count=\"4\" coef=\"125\"/><combination symbol=\"8\" name=\"mid4\" count=\"5\" coef=\"250\"/><combination symbol=\"9\" name=\"mid5\" count=\"3\" coef=\"50\"/><combination symbol=\"9\" name=\"mid5\" count=\"4\" coef=\"150\"/><combination symbol=\"9\" name=\"mid5\" count=\"5\" coef=\"300\"/><combination symbol=\"10\" name=\"mid6\" count=\"3\" coef=\"100\"/><combination symbol=\"10\" name=\"mid6\" count=\"4\" coef=\"200\"/><combination symbol=\"10\" name=\"mid6\" count=\"5\" coef=\"400\"/><combination symbol=\"11\" name=\"scatter\" count=\"3\" coef=\"12\" multi_coef=\"1\" multi_coef2=\"2\"/><combination symbol=\"11\" name=\"scatter\" count=\"4\" coef=\"12\" multi_coef=\"1\" multi_coef2=\"15\"/><combination symbol=\"11\" name=\"scatter\" count=\"5\" coef=\"12\" multi_coef=\"1\" multi_coef2=\"50\"/><combination symbol=\"12\" name=\"wild\" count=\"3\" coef=\"0\"/><combination symbol=\"12\" name=\"wild\" count=\"4\" coef=\"0\"/><combination symbol=\"12\" name=\"wild\" count=\"5\" coef=\"0\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/><payline id=\"11\" path=\"1,2,2,2,3\"/><payline id=\"12\" path=\"2,3,2,1,2\"/><payline id=\"13\" path=\"2,1,2,3,2\"/><payline id=\"14\" path=\"1,2,1,2,1\"/><payline id=\"15\" path=\"3,2,3,2,3\"/><payline id=\"16\" path=\"2,2,1,2,2\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"1,3,1,3,1\"/><payline id=\"19\" path=\"3,1,3,1,3\"/><payline id=\"20\" path=\"3,1,2,1,3\"/></paylines><symbols><symbol id=\"1\" title=\"J\"/><symbol id=\"2\" title=\"Q\"/><symbol id=\"3\" title=\"K\"/><symbol id=\"4\" title=\"A\"/><symbol id=\"5\" title=\"mid1\"/><symbol id=\"6\" title=\"mid2\"/><symbol id=\"7\" title=\"mid3\"/><symbol id=\"8\" title=\"mid4\"/><symbol id=\"9\" title=\"mid5\"/><symbol id=\"10\" title=\"mid6\"/><symbol id=\"11\" title=\"scatter\"/><symbol id=\"12\" title=\"wild\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"11\" symbols=\"9,8,3,4,7,1,5,10,6,11,2\"/><reel id=\"2\" length=\"12\" symbols=\"5,1,6,12,10,3,4,2,8,11,9,7\"/><reel id=\"3\" length=\"12\" symbols=\"3,9,2,1,10,6,4,12,5,8,11,7\"/><reel id=\"4\" length=\"12\" symbols=\"8,10,4,9,3,11,2,6,12,1,7,5\"/><reel id=\"5\" length=\"11\" symbols=\"7,10,8,1,5,6,2,9,3,4,11\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"11\" symbols=\"1,8,5,3,4,7,2,9,10,6,11\"/><reel id=\"2\" length=\"12\" symbols=\"5,1,6,12,3,4,10,8,7,9,2,11\"/><reel id=\"3\" length=\"12\" symbols=\"3,5,2,9,10,1,6,8,4,12,11,7\"/><reel id=\"4\" length=\"12\" symbols=\"8,2,5,9,3,4,6,12,1,7,10,11\"/><reel id=\"5\" length=\"11\" symbols=\"7,1,8,5,6,10,9,2,4,3,11\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"10\" symbols=\"1,8,10,6,4,3,9,7,2,11\"/><reel id=\"2\" length=\"11\" symbols=\"6,9,1,10,12,3,4,2,8,11,7\"/><reel id=\"3\" length=\"11\" symbols=\"3,9,2,1,6,10,8,4,11,7,12\"/><reel id=\"4\" length=\"11\" symbols=\"8,2,4,9,3,6,7,12,1,11,10\"/><reel id=\"5\" length=\"10\" symbols=\"7,1,8,9,6,10,2,4,3,11\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"9\" symbols=\"7,8,3,4,10,1,9,2,11\"/><reel id=\"2\" length=\"10\" symbols=\"7,1,12,3,4,2,9,10,8,11\"/><reel id=\"3\" length=\"10\" symbols=\"3,7,2,1,9,10,8,4,12,11\"/><reel id=\"4\" length=\"10\" symbols=\"8,2,4,9,3,7,1,12,11,10\"/><reel id=\"5\" length=\"9\" symbols=\"7,1,8,2,9,4,3,10,11\"/></reels><reels id=\"5\"><reel id=\"1\" length=\"8\" symbols=\"1,8,3,10,4,2,9,11\"/><reel id=\"2\" length=\"9\" symbols=\"8,1,12,3,4,2,9,10,11\"/><reel id=\"3\" length=\"9\" symbols=\"3,8,9,2,1,10,4,12,11\"/><reel id=\"4\" length=\"9\" symbols=\"8,2,4,9,3,1,12,11,10\"/><reel id=\"5\" length=\"8\" symbols=\"8,1,10,2,9,4,3,11\"/></reels><reels id=\"6\"><reel id=\"1\" length=\"7\" symbols=\"1,9,3,4,2,10,11\"/><reel id=\"2\" length=\"8\" symbols=\"9,1,12,3,4,2,10,11\"/><reel id=\"3\" length=\"8\" symbols=\"3,9,2,1,10,4,12,11\"/><reel id=\"4\" length=\"8\" symbols=\"9,2,4,3,1,12,11,10\"/><reel id=\"5\" length=\"7\" symbols=\"9,1,2,10,4,3,11\"/></reels><reels id=\"7\"><reel id=\"1\" length=\"6\" symbols=\"10,3,4,1,2,11\"/><reel id=\"2\" length=\"7\" symbols=\"10,1,12,3,4,2,11\"/><reel id=\"3\" length=\"7\" symbols=\"3,10,2,1,4,12,11\"/><reel id=\"4\" length=\"7\" symbols=\"10,2,4,3,12,1,11\"/><reel id=\"5\" length=\"6\" symbols=\"10,1,2,4,3,11\"/></reels></slot><limit id=\"freespins_seq\" max=\"35\"/><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"10,9,9\" reel2=\"4,6,5\" reel3=\"1,3,3\" reel4=\"4,2,5\" reel5=\"7,2,2\"/><delivery id=\"1906944-890102154387335025030048\" action=\"create\"/></server>";
            }
        }
        #endregion
        public RiseOfEgyptGameLogic()
        {
            _gameID = GAMEID.RiseOfEgypt;
            GameName = "RiseOfEgypt";
        }
    }
}
