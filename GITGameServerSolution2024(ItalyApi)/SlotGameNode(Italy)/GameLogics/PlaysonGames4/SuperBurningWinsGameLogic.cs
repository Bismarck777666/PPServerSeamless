using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class SuperBurningWinsGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "super_burning_wins";
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 4, 6, 8, 10, 15, 20, 30, 40, 50, 75, 100, 200, 300, 500, 750, 1000, 2000 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><source game-ver=\"57735\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"seven\" count=\"3\" coef=\"750\"/><combination symbol=\"2\" name=\"bell\" count=\"3\" coef=\"250\"/><combination symbol=\"3\" name=\"bar\" count=\"3\" coef=\"50\"/><combination symbol=\"4\" name=\"plum\" count=\"3\" coef=\"35\"/><combination symbol=\"5\" name=\"orange\" count=\"3\" coef=\"35\"/><combination symbol=\"6\" name=\"lemon\" count=\"3\" coef=\"35\"/><combination symbol=\"7\" name=\"cherry\" count=\"3\" coef=\"35\"/><combination symbol=\"8\" name=\"x\" count=\"3\" coef=\"5\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1\"/><payline id=\"2\" path=\"2,2,2\"/><payline id=\"3\" path=\"3,3,3\"/><payline id=\"4\" path=\"1,2,3\"/><payline id=\"5\" path=\"3,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"seven\"/><symbol id=\"2\" title=\"bell\"/><symbol id=\"3\" title=\"bar\"/><symbol id=\"4\" title=\"plum\"/><symbol id=\"5\" title=\"orange\"/><symbol id=\"6\" title=\"lemon\"/><symbol id=\"7\" title=\"cherry\"/><symbol id=\"8\" title=\"x\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"8\" symbols=\"2,7,8,4,5,3,1,6\"/><reel id=\"2\" length=\"8\" symbols=\"2,8,5,3,6,7,1,4\"/><reel id=\"3\" length=\"8\" symbols=\"2,7,8,1,4,5,3,6\"/></reels></slot><shift server=\"0,0,0\" reel_set=\"1\" reel1=\"3,3,1\" reel2=\"1,7,7\" reel3=\"1,4,4\"/><delivery id=\"1904599-419035939334378209491991\" action=\"create\"/></server>";
            }
        }
        #endregion
        public SuperBurningWinsGameLogic()
        {
            _gameID = GAMEID.SuperBurningWins;
            GameName = "SuperBurningWins";
        }
    }
}
