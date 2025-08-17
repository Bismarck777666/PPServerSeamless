using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class SunnyFruitsGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "sunny_fruits";
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
                return "<server><source game-ver=\"130120\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"cherry\" count=\"3\" coef=\"5\"/><combination symbol=\"1\" name=\"cherry\" count=\"4\" coef=\"15\"/><combination symbol=\"1\" name=\"cherry\" count=\"5\" coef=\"50\"/><combination symbol=\"2\" name=\"lemon\" count=\"3\" coef=\"5\"/><combination symbol=\"2\" name=\"lemon\" count=\"4\" coef=\"15\"/><combination symbol=\"2\" name=\"lemon\" count=\"5\" coef=\"50\"/><combination symbol=\"3\" name=\"orange\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"orange\" count=\"4\" coef=\"15\"/><combination symbol=\"3\" name=\"orange\" count=\"5\" coef=\"50\"/><combination symbol=\"4\" name=\"plum\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"plum\" count=\"4\" coef=\"15\"/><combination symbol=\"4\" name=\"plum\" count=\"5\" coef=\"50\"/><combination symbol=\"5\" name=\"grape\" count=\"3\" coef=\"20\"/><combination symbol=\"5\" name=\"grape\" count=\"4\" coef=\"50\"/><combination symbol=\"5\" name=\"grape\" count=\"5\" coef=\"150\"/><combination symbol=\"6\" name=\"melon\" count=\"3\" coef=\"20\"/><combination symbol=\"6\" name=\"melon\" count=\"4\" coef=\"50\"/><combination symbol=\"6\" name=\"melon\" count=\"5\" coef=\"150\"/><combination symbol=\"7\" name=\"pear\" count=\"3\" coef=\"20\"/><combination symbol=\"7\" name=\"pear\" count=\"4\" coef=\"50\"/><combination symbol=\"7\" name=\"pear\" count=\"5\" coef=\"150\"/><combination symbol=\"8\" name=\"seven\" count=\"3\" coef=\"25\"/><combination symbol=\"8\" name=\"seven\" count=\"4\" coef=\"75\"/><combination symbol=\"8\" name=\"seven\" count=\"5\" coef=\"250\"/><combination symbol=\"10\" name=\"bonus\" count=\"3\" coef=\"0\"/><combination symbol=\"10\" name=\"bonus\" count=\"4\" coef=\"0\"/><combination symbol=\"10\" name=\"bonus\" count=\"5\" coef=\"0\"/></combinations><paylines><payline id=\"1\" path=\"2,2,2,2,2\"/><payline id=\"2\" path=\"1,1,1,1,1\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"1,2,3,2,1\"/><payline id=\"5\" path=\"3,2,1,2,3\"/><payline id=\"6\" path=\"2,3,3,3,2\"/><payline id=\"7\" path=\"2,1,1,1,2\"/><payline id=\"8\" path=\"3,3,2,1,1\"/><payline id=\"9\" path=\"1,1,2,3,3\"/><payline id=\"10\" path=\"3,2,2,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"cherry\"/><symbol id=\"2\" title=\"lemon\"/><symbol id=\"3\" title=\"orange\"/><symbol id=\"4\" title=\"plum\"/><symbol id=\"5\" title=\"grape\"/><symbol id=\"6\" title=\"melon\"/><symbol id=\"7\" title=\"pear\"/><symbol id=\"8\" title=\"seven\"/><symbol id=\"9\" title=\"wild\"/><symbol id=\"10\" title=\"bonus\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"9\" symbols=\"6,2,5,1,10,4,7,3,8\"/><reel id=\"2\" length=\"10\" symbols=\"9,1,8,3,7,2,6,5,10,4\"/><reel id=\"3\" length=\"10\" symbols=\"5,1,8,3,9,7,10,6,4,2\"/><reel id=\"4\" length=\"10\" symbols=\"8,1,9,4,5,6,10,2,7,3\"/><reel id=\"5\" length=\"10\" symbols=\"9,1,8,3,5,4,2,7,6,10\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"9\" symbols=\"6,2,5,1,10,4,7,3,8\"/><reel id=\"2\" length=\"10\" symbols=\"9,1,8,3,7,2,6,5,10,4\"/><reel id=\"3\" length=\"10\" symbols=\"5,1,8,3,9,7,10,6,4,2\"/><reel id=\"4\" length=\"10\" symbols=\"8,1,9,4,5,6,10,2,7,3\"/><reel id=\"5\" length=\"10\" symbols=\"9,1,8,3,5,4,2,7,6,10\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"10,5,2\" reel2=\"9,1,8\" reel3=\"3,6,1\" reel4=\"1,9,9\" reel5=\"4,10,10\"><bonus bonus_pos=\"0,9,14\" bonus_tb=\"5,1,6\"/></shift><game jackpots_tb=\"25,100,1000\" bonus_spins=\"3\"/><delivery id=\"1176177-974987854128171605647035\" action=\"create\"/></server>";
            }
        }
        #endregion

        public SunnyFruitsGameLogic()
        {
            _gameID     = GAMEID.SunnyFruits;
            GameName    = "SunnyFruits";
        }
    }
}
