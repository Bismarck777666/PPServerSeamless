using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class DiamondFortGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "diamond_fort";
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
                return "<server><source game-ver=\"251121\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"3\" name=\"mid4\" count=\"3\" coef=\"300\"/><combination symbol=\"4\" name=\"mid3\" count=\"3\" coef=\"100\"/><combination symbol=\"5\" name=\"mid2\" count=\"3\" coef=\"100\"/><combination symbol=\"6\" name=\"mid1\" count=\"3\" coef=\"50\"/><combination symbol=\"7\" name=\"low4\" count=\"3\" coef=\"20\"/><combination symbol=\"8\" name=\"low3\" count=\"3\" coef=\"20\"/><combination symbol=\"9\" name=\"low2\" count=\"3\" coef=\"20\"/><combination symbol=\"10\" name=\"low1\" count=\"3\" coef=\"20\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1\"/><payline id=\"2\" path=\"2,2,2\"/><payline id=\"3\" path=\"3,3,3\"/><payline id=\"4\" path=\"1,2,3\"/><payline id=\"5\" path=\"3,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"diamond\"/><symbol id=\"3\" title=\"mid4\"/><symbol id=\"4\" title=\"mid3\"/><symbol id=\"5\" title=\"mid2\"/><symbol id=\"6\" title=\"mid1\"/><symbol id=\"7\" title=\"low4\"/><symbol id=\"8\" title=\"low3\"/><symbol id=\"9\" title=\"low2\"/><symbol id=\"10\" title=\"low1\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"9\" symbols=\"8,5,9,10,4,7,6,3,1\"/><reel id=\"2\" length=\"9\" symbols=\"8,5,9,10,4,7,6,3,1\"/><reel id=\"3\" length=\"9\" symbols=\"8,5,9,10,4,7,6,3,1\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"9\" symbols=\"8,5,1,4,6,9,3,7,10\"/><reel id=\"2\" length=\"9\" symbols=\"8,5,1,4,6,9,3,7,10\"/><reel id=\"3\" length=\"9\" symbols=\"8,5,1,4,6,9,3,7,10\"/></reels></slot><shift server=\"0,0,0\" reel_set=\"1\" reel1=\"7,5,3\" reel2=\"1,1,10\" reel3=\"4,3,6\"><bonus bonus_pos=\"1,4\" bonus_tb=\"7,12\"/></shift><game jackpots_tb=\"25,150,1000\" bonus_spins=\"3\" total_bet_mult=\"10\"/><delivery id=\"478024-2464369319600036080179813\" action=\"create\"/></server>";
            }
        }
        #endregion

        public DiamondFortGameLogic()
        {
            _gameID = GAMEID.DiamondFort;
            GameName = "DiamondFort";
        }
    }
}
