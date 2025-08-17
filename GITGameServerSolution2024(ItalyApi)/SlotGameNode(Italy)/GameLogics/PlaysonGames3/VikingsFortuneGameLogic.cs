using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class VikingsFortuneGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vikings_fortune";
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
                return "<server><source game-ver=\"40319\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"scatter\" count=\"3\" coef=\"8\" multi_coef=\"1\" multi_coef2=\"5\"/><combination symbol=\"2\" name=\"wild\" count=\"3\" coef=\"25\"/><combination symbol=\"2\" name=\"wild\" count=\"4\" coef=\"100\"/><combination symbol=\"2\" name=\"wild\" count=\"5\" coef=\"500\"/><combination symbol=\"3\" name=\"top1\" count=\"3\" coef=\"25\"/><combination symbol=\"3\" name=\"top1\" count=\"4\" coef=\"100\"/><combination symbol=\"3\" name=\"top1\" count=\"5\" coef=\"500\"/><combination symbol=\"4\" name=\"top2\" count=\"3\" coef=\"20\"/><combination symbol=\"4\" name=\"top2\" count=\"4\" coef=\"80\"/><combination symbol=\"4\" name=\"top2\" count=\"5\" coef=\"300\"/><combination symbol=\"5\" name=\"med1\" count=\"3\" coef=\"15\"/><combination symbol=\"5\" name=\"med1\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"med1\" count=\"5\" coef=\"200\"/><combination symbol=\"6\" name=\"med2\" count=\"3\" coef=\"10\"/><combination symbol=\"6\" name=\"med2\" count=\"4\" coef=\"40\"/><combination symbol=\"6\" name=\"med2\" count=\"5\" coef=\"150\"/><combination symbol=\"7\" name=\"low1\" count=\"3\" coef=\"5\"/><combination symbol=\"7\" name=\"low1\" count=\"4\" coef=\"25\"/><combination symbol=\"7\" name=\"low1\" count=\"5\" coef=\"50\"/><combination symbol=\"8\" name=\"low2\" count=\"3\" coef=\"5\"/><combination symbol=\"8\" name=\"low2\" count=\"4\" coef=\"25\"/><combination symbol=\"8\" name=\"low2\" count=\"5\" coef=\"50\"/><combination symbol=\"9\" name=\"low3\" count=\"3\" coef=\"5\"/><combination symbol=\"9\" name=\"low3\" count=\"4\" coef=\"25\"/><combination symbol=\"9\" name=\"low3\" count=\"5\" coef=\"50\"/><combination symbol=\"10\" name=\"low4\" count=\"3\" coef=\"5\"/><combination symbol=\"10\" name=\"low4\" count=\"4\" coef=\"25\"/><combination symbol=\"10\" name=\"low4\" count=\"5\" coef=\"50\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,1,1,1,2\"/><payline id=\"7\" path=\"2,3,3,3,2\"/><payline id=\"8\" path=\"1,1,2,3,3\"/><payline id=\"9\" path=\"3,3,2,1,1\"/><payline id=\"10\" path=\"2,3,2,1,2\"/><payline id=\"11\" path=\"2,1,2,3,2\"/><payline id=\"12\" path=\"1,2,2,2,1\"/><payline id=\"13\" path=\"3,2,2,2,3\"/><payline id=\"14\" path=\"1,2,1,2,1\"/><payline id=\"15\" path=\"3,2,3,2,3\"/><payline id=\"16\" path=\"2,2,1,2,2\"/><payline id=\"17\" path=\"2,2,3,2,2\"/><payline id=\"18\" path=\"1,1,3,1,1\"/><payline id=\"19\" path=\"3,3,1,3,3\"/><payline id=\"20\" path=\"1,3,3,3,1\"/><payline id=\"21\" path=\"3,1,1,1,3\"/><payline id=\"22\" path=\"2,3,1,3,2\"/><payline id=\"23\" path=\"2,1,3,1,2\"/><payline id=\"24\" path=\"1,3,1,3,1\"/><payline id=\"25\" path=\"3,1,3,1,3\"/></paylines><symbols><symbol id=\"1\" title=\"scatter\"/><symbol id=\"2\" title=\"wild\"/><symbol id=\"3\" title=\"top1\"/><symbol id=\"4\" title=\"top2\"/><symbol id=\"5\" title=\"med1\"/><symbol id=\"6\" title=\"med2\"/><symbol id=\"7\" title=\"low1\"/><symbol id=\"8\" title=\"low2\"/><symbol id=\"9\" title=\"low3\"/><symbol id=\"10\" title=\"low4\"/><symbol id=\"11\" title=\"bonus\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"11\" symbols=\"9,6,8,7,1,4,2,10,3,11,5\"/><reel id=\"2\" length=\"10\" symbols=\"10,9,7,8,5,4,2,6,3,11\"/><reel id=\"3\" length=\"11\" symbols=\"6,9,7,3,1,10,8,2,5,4,11\"/><reel id=\"4\" length=\"10\" symbols=\"6,9,7,3,10,8,5,2,11,4\"/><reel id=\"5\" length=\"11\" symbols=\"6,7,9,1,10,5,3,2,4,8,11\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"11\" symbols=\"9,6,8,7,1,4,2,10,3,11,5\"/><reel id=\"2\" length=\"10\" symbols=\"10,9,7,8,5,4,2,6,3,11\"/><reel id=\"3\" length=\"11\" symbols=\"6,9,7,3,1,10,8,2,5,4,11\"/><reel id=\"4\" length=\"10\" symbols=\"6,9,7,3,10,8,5,2,11,4\"/><reel id=\"5\" length=\"11\" symbols=\"6,7,9,1,10,5,3,2,4,8,11\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"8,1,3\" reel2=\"5,8,10\" reel3=\"4,9,7\" reel4=\"4,10,7\" reel5=\"11,11,3\"/><shift_ext><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"7,1,6\" reel2=\"4,8,10\" reel3=\"6,10,11\" reel4=\"4,10,7\" reel5=\"10,9,4\"><big_symbol id=\"3\" offset=\"7\" w=\"3\" h=\"2\"/></shift><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"5,7,9\" reel2=\"10,6,11\" reel3=\"10,5,6\" reel4=\"6,9,3\" reel5=\"1,10,7\"><big_symbol id=\"11\" offset=\"6\" w=\"2\" h=\"2\"/><bonus bonus_pos=\"6,7,11,12\" bonus_tb=\"7,7,7,7\"/></shift><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"5,6,8\" reel2=\"11,11,11\" reel3=\"7,3,1\" reel4=\"9,3,6\" reel5=\"9,5,10\"><big_symbol id=\"4\" offset=\"11\" w=\"3\" h=\"1\"/><bonus bonus_pos=\"1,6\" bonus_tb=\"4,2\"/></shift></shift_ext><game jackpots_tb=\"30,100,1000\" bonus_spins=\"3\"><bonus bonus_pos=\"4,9\" bonus_tb=\"18,3\"/></game><delivery id=\"1897130-885120376537735604885013\" action=\"create\"/></server>";
            }
        }
        #endregion
        public VikingsFortuneGameLogic()
        {
            _gameID = GAMEID.VikingsFortune;
            GameName = "VikingsFortune";
        }
    }
}
