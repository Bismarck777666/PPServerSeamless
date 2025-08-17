using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class JokersCoinsGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "jokers_coins";
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
                return "<server><source game-ver=\"81221\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"3\" name=\"top\" count=\"3\" coef=\"1000\"/><combination symbol=\"4\" name=\"mid4\" count=\"3\" coef=\"300\"/><combination symbol=\"5\" name=\"mid3\" count=\"3\" coef=\"200\"/><combination symbol=\"6\" name=\"mid2\" count=\"3\" coef=\"160\"/><combination symbol=\"7\" name=\"mid1\" count=\"3\" coef=\"160\"/><combination symbol=\"8\" name=\"low4\" count=\"3\" coef=\"40\"/><combination symbol=\"9\" name=\"low3\" count=\"3\" coef=\"40\"/><combination symbol=\"10\" name=\"low2\" count=\"3\" coef=\"40\"/><combination symbol=\"11\" name=\"low1\" count=\"3\" coef=\"10\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1\"/><payline id=\"2\" path=\"2,2,2\"/><payline id=\"3\" path=\"3,3,3\"/><payline id=\"4\" path=\"1,2,3\"/><payline id=\"5\" path=\"3,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"coin\"/><symbol id=\"2\" title=\"hidden\"/><symbol id=\"3\" title=\"top\"/><symbol id=\"4\" title=\"mid4\"/><symbol id=\"5\" title=\"mid3\"/><symbol id=\"6\" title=\"mid2\"/><symbol id=\"7\" title=\"mid1\"/><symbol id=\"8\" title=\"low4\"/><symbol id=\"9\" title=\"low3\"/><symbol id=\"10\" title=\"low2\"/><symbol id=\"11\" title=\"low1\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"8,6,9,11,5,1,4,3,10,7\"/><reel id=\"2\" length=\"10\" symbols=\"5,3,6,11,7,4,1,8,10,9\"/><reel id=\"3\" length=\"10\" symbols=\"8,11,3,10,9,4,1,7,6,5\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"8,6,9,1,11,5,4,3,10,7\"/><reel id=\"2\" length=\"10\" symbols=\"5,3,6,11,7,4,1,8,10,9\"/><reel id=\"3\" length=\"10\" symbols=\"8,11,3,10,9,4,1,7,6,5\"/></reels></slot><shift server=\"0,0,0\" reel_set=\"1\" reel1=\"10,10,4\" reel2=\"9,3,10\" reel3=\"1,9,5\"><bonus bonus_pos=\"2\" bonus_tb=\"2\"/></shift><game total_bet_mult=\"10\" jackpots_tb=\"25,50,150,3000\" bonus_spins=\"3\"/><delivery id=\"475324-6760800816633917292035944\" action=\"create\"/></server>";
            }
        }
        #endregion

        public JokersCoinsGameLogic()
        {
            _gameID = GAMEID.JokersCoins;
            GameName = "JokersCoins";
        }
    }
}
