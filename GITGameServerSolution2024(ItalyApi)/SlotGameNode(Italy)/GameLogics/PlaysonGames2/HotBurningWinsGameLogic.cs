using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class HotBurningWinsGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "hot_burning_wins";
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
                return "<server><source game-ver=\"280521\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"seven\" count=\"3\" coef=\"600\"/><combination symbol=\"2\" name=\"bell\" count=\"3\" coef=\"400\"/><combination symbol=\"3\" name=\"bar\" count=\"3\" coef=\"200\"/><combination symbol=\"4\" name=\"melon\" count=\"3\" coef=\"150\"/><combination symbol=\"5\" name=\"grape\" count=\"3\" coef=\"150\"/><combination symbol=\"6\" name=\"plum\" count=\"3\" coef=\"70\"/><combination symbol=\"7\" name=\"lemon\" count=\"3\" coef=\"70\"/><combination symbol=\"8\" name=\"orange\" count=\"3\" coef=\"70\"/><combination symbol=\"9\" name=\"cherry\" count=\"3\" coef=\"30\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1\"/><payline id=\"2\" path=\"2,2,2\"/><payline id=\"3\" path=\"3,3,3\"/><payline id=\"4\" path=\"1,2,3\"/><payline id=\"5\" path=\"3,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"seven\"/><symbol id=\"2\" title=\"bell\"/><symbol id=\"3\" title=\"bar\"/><symbol id=\"4\" title=\"melon\"/><symbol id=\"5\" title=\"grape\"/><symbol id=\"6\" title=\"plum\"/><symbol id=\"7\" title=\"lemon\"/><symbol id=\"8\" title=\"orange\"/><symbol id=\"9\" title=\"cherry\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"9\" symbols=\"2,6,1,9,5,7,4,8,3\"/><reel id=\"2\" length=\"9\" symbols=\"2,3,7,4,8,9,1,5,6\"/><reel id=\"3\" length=\"9\" symbols=\"2,9,5,3,6,8,1,7,4\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"9\" symbols=\"2,6,1,9,5,7,4,8,3\"/><reel id=\"2\" length=\"9\" symbols=\"2,3,7,4,8,9,1,5,6\"/><reel id=\"3\" length=\"9\" symbols=\"2,9,5,3,6,8,1,7,4\"/></reels></slot><shift server=\"0,0,0\" reel_set=\"1\" reel1=\"7,4,7\" reel2=\"6,1,9\" reel3=\"4,2,8\"/><game total_bet_mult=\"10\"/><delivery id=\"484413-5223676491980743656206799\" action=\"create\"/></server>";
            }
        }
        #endregion
        public HotBurningWinsGameLogic()
        {
            _gameID = GAMEID.HotBurningWins;
            GameName = "HotBurningWins";
        }
    }
}
