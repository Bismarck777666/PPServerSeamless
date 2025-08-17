using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BurningWinsX2GameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "burning_wins_x2";
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
                return "<server><source game-ver=\"220921\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"seven\" count=\"3\" coef=\"1500\"/><combination symbol=\"2\" name=\"bell\" count=\"3\" coef=\"500\"/><combination symbol=\"3\" name=\"bar\" count=\"3\" coef=\"100\"/><combination symbol=\"4\" name=\"plum\" count=\"3\" coef=\"70\"/><combination symbol=\"5\" name=\"orange\" count=\"3\" coef=\"70\"/><combination symbol=\"6\" name=\"lemon\" count=\"3\" coef=\"70\"/><combination symbol=\"7\" name=\"cherry\" count=\"3\" coef=\"70\"/><combination symbol=\"8\" name=\"x\" count=\"3\" coef=\"10\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1\"/><payline id=\"2\" path=\"2,2,2\"/><payline id=\"3\" path=\"3,3,3\"/><payline id=\"4\" path=\"1,2,3\"/><payline id=\"5\" path=\"3,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"seven\"/><symbol id=\"2\" title=\"bell\"/><symbol id=\"3\" title=\"bar\"/><symbol id=\"4\" title=\"plum\"/><symbol id=\"5\" title=\"orange\"/><symbol id=\"6\" title=\"lemon\"/><symbol id=\"7\" title=\"cherry\"/><symbol id=\"8\" title=\"x\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"8\" symbols=\"2,7,8,4,5,3,1,6\"/><reel id=\"2\" length=\"8\" symbols=\"2,8,5,3,6,7,1,4\"/><reel id=\"3\" length=\"8\" symbols=\"2,7,8,1,4,5,3,6\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"8\" symbols=\"2,7,8,4,5,3,1,6\"/><reel id=\"2\" length=\"8\" symbols=\"2,8,5,3,6,7,1,4\"/><reel id=\"3\" length=\"8\" symbols=\"2,7,8,1,4,5,3,6\"/></reels></slot><shift server=\"0,0,0\" reel_set=\"1\" reel1=\"3,1,6\" reel2=\"7,8,1\" reel3=\"8,1,4\"/><game total_bet_mult=\"10\"/><delivery id=\"478819-6212864682891486971604224\" action=\"create\"/></server>";
            }
        }
        #endregion

        public BurningWinsX2GameLogic()
        {
            _gameID = GAMEID.BurningWinsX2;
            GameName = "BurningWinsX2";
        }
    }
}
