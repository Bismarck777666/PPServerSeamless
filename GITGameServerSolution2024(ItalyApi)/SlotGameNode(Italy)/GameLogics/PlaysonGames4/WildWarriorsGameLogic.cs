using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class WildWarriorsGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "wild_warriors";
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
                return "<server><source game-ver=\"21018\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"wild\" count=\"2\" coef=\"15\"/><combination symbol=\"1\" name=\"wild\" count=\"3\" coef=\"60\"/><combination symbol=\"1\" name=\"wild\" count=\"4\" coef=\"300\"/><combination symbol=\"1\" name=\"wild\" count=\"5\" coef=\"1000\"/><combination symbol=\"2\" name=\"top1\" count=\"2\" coef=\"6\"/><combination symbol=\"2\" name=\"top1\" count=\"3\" coef=\"10\"/><combination symbol=\"2\" name=\"top1\" count=\"4\" coef=\"20\"/><combination symbol=\"2\" name=\"top1\" count=\"5\" coef=\"50\"/><combination symbol=\"3\" name=\"top2\" count=\"2\" coef=\"4\"/><combination symbol=\"3\" name=\"top2\" count=\"3\" coef=\"10\"/><combination symbol=\"3\" name=\"top2\" count=\"4\" coef=\"20\"/><combination symbol=\"3\" name=\"top2\" count=\"5\" coef=\"45\"/><combination symbol=\"4\" name=\"med1\" count=\"3\" coef=\"8\"/><combination symbol=\"4\" name=\"med1\" count=\"4\" coef=\"20\"/><combination symbol=\"4\" name=\"med1\" count=\"5\" coef=\"40\"/><combination symbol=\"5\" name=\"med2\" count=\"3\" coef=\"8\"/><combination symbol=\"5\" name=\"med2\" count=\"4\" coef=\"20\"/><combination symbol=\"5\" name=\"med2\" count=\"5\" coef=\"30\"/><combination symbol=\"6\" name=\"med3\" count=\"3\" coef=\"6\"/><combination symbol=\"6\" name=\"med3\" count=\"4\" coef=\"15\"/><combination symbol=\"6\" name=\"med3\" count=\"5\" coef=\"25\"/><combination symbol=\"7\" name=\"low1\" count=\"3\" coef=\"4\"/><combination symbol=\"7\" name=\"low1\" count=\"4\" coef=\"10\"/><combination symbol=\"7\" name=\"low1\" count=\"5\" coef=\"20\"/><combination symbol=\"8\" name=\"low2\" count=\"3\" coef=\"4\"/><combination symbol=\"8\" name=\"low2\" count=\"4\" coef=\"10\"/><combination symbol=\"8\" name=\"low2\" count=\"5\" coef=\"20\"/><combination symbol=\"9\" name=\"low3\" count=\"3\" coef=\"2\"/><combination symbol=\"9\" name=\"low3\" count=\"4\" coef=\"8\"/><combination symbol=\"9\" name=\"low3\" count=\"5\" coef=\"15\"/><combination symbol=\"10\" name=\"low4\" count=\"3\" coef=\"2\"/><combination symbol=\"10\" name=\"low4\" count=\"4\" coef=\"8\"/><combination symbol=\"10\" name=\"low4\" count=\"5\" coef=\"15\"/><combination symbol=\"11\" name=\"scatter\" count=\"7\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"2\"/><combination symbol=\"11\" name=\"scatter\" count=\"8\" coef=\"12\" multi_coef=\"1\" multi_coef2=\"2\"/><combination symbol=\"11\" name=\"scatter\" count=\"9\" coef=\"25\" multi_coef=\"1\" multi_coef2=\"2\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/><payline id=\"11\" path=\"1,2,2,2,3\"/><payline id=\"12\" path=\"2,3,2,1,2\"/><payline id=\"13\" path=\"2,1,2,3,2\"/><payline id=\"14\" path=\"1,2,1,2,1\"/><payline id=\"15\" path=\"3,2,3,2,3\"/><payline id=\"16\" path=\"2,2,1,2,2\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"1,3,1,3,1\"/><payline id=\"19\" path=\"3,1,3,1,3\"/><payline id=\"20\" path=\"3,1,2,1,3\"/><payline id=\"21\" path=\"1,3,2,3,1\"/><payline id=\"22\" path=\"1,1,3,1,1\"/><payline id=\"23\" path=\"3,3,1,3,3\"/><payline id=\"24\" path=\"2,1,3,1,2\"/><payline id=\"25\" path=\"2,3,1,3,2\"/><payline id=\"26\" path=\"1,3,3,3,1\"/><payline id=\"27\" path=\"3,1,1,1,3\"/><payline id=\"28\" path=\"1,1,1,1,2\"/><payline id=\"29\" path=\"2,2,2,2,3\"/><payline id=\"30\" path=\"3,3,3,3,2\"/></paylines><symbols><symbol id=\"1\" title=\"wild\"/><symbol id=\"2\" title=\"top1\"/><symbol id=\"3\" title=\"top2\"/><symbol id=\"4\" title=\"med1\"/><symbol id=\"5\" title=\"med2\"/><symbol id=\"6\" title=\"med3\"/><symbol id=\"7\" title=\"low1\"/><symbol id=\"8\" title=\"low2\"/><symbol id=\"9\" title=\"low3\"/><symbol id=\"10\" title=\"low4\"/><symbol id=\"11\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"8,6,10,2,4,9,3,7,5,1\"/><reel id=\"2\" length=\"11\" symbols=\"11,3,9,1,6,8,2,7,4,10,5\"/><reel id=\"3\" length=\"11\" symbols=\"11,10,4,3,8,7,1,6,5,9,2\"/><reel id=\"4\" length=\"11\" symbols=\"11,10,4,3,8,2,7,6,1,5,9\"/><reel id=\"5\" length=\"10\" symbols=\"7,1,10,4,8,3,9,6,5,2\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"5,7,2,9,3,10,6,8,4,1\"/><reel id=\"2\" length=\"10\" symbols=\"6,8,9,1,3,2,7,5,4,10\"/><reel id=\"3\" length=\"10\" symbols=\"4,7,2,9,6,10,3,8,5,1\"/><reel id=\"4\" length=\"10\" symbols=\"2,10,1,9,7,5,3,8,6,4\"/><reel id=\"5\" length=\"10\" symbols=\"2,6,8,1,4,7,10,5,9,3\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"7,7,5,9,8\" reel2=\"9,9,11,11,11\" reel3=\"1,8,8,3,3\" reel4=\"11,11,11,11,11\" reel5=\"4,5,8,8,3\"/><shift_ext><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"4,4,4,4,4\" reel2=\"10,10,5,10,10\" reel3=\"10,10,10,10,10\" reel4=\"5,7,4,7,7\" reel5=\"10,10,1,1,7\"><big_symbol id=\"2\" offset=\"2\" w=\"3\" h=\"1\"/></shift><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"5,5,6,9,9\" reel2=\"4,4,4,4,4\" reel3=\"4,4,7,7,7\" reel4=\"1,1,7,7,2\" reel5=\"8,3,3,9,9\"><big_symbol id=\"3\" offset=\"6\" w=\"2\" h=\"2\"/></shift><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"7,2,10,10,10\" reel2=\"4,4,4,4,4\" reel3=\"1,8,8,1,1\" reel4=\"7,7,7,5,5\" reel5=\"2,2,6,6,6\"><big_symbol id=\"4\" offset=\"1\" w=\"3\" h=\"2\"/></shift></shift_ext><delivery id=\"1901365-637495008135173819010996\" action=\"create\"/></server>";
            }
        }
        #endregion

        public WildWarriorsGameLogic()
        {
            _gameID     = GAMEID.WildWarriors;
            GameName    = "WildWarriors";
        }
    }
}
