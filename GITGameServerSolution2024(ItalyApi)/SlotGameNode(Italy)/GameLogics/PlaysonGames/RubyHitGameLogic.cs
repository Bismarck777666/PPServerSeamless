using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class RubyHitGameLogic : BasePlaysonBonusSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "ruby_hit";
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
                return "<server><source game-ver=\"40821\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"2\" name=\"top\" count=\"3\" coef=\"500\"/><combination symbol=\"3\" name=\"mid4\" count=\"3\" coef=\"300\"/><combination symbol=\"4\" name=\"mid3\" count=\"3\" coef=\"200\"/><combination symbol=\"5\" name=\"mid2\" count=\"3\" coef=\"160\"/><combination symbol=\"6\" name=\"mid1\" count=\"3\" coef=\"160\"/><combination symbol=\"7\" name=\"low4\" count=\"3\" coef=\"40\"/><combination symbol=\"8\" name=\"low3\" count=\"3\" coef=\"40\"/><combination symbol=\"9\" name=\"low2\" count=\"3\" coef=\"40\"/><combination symbol=\"10\" name=\"low1\" count=\"3\" coef=\"10\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1\"/><payline id=\"2\" path=\"2,2,2\"/><payline id=\"3\" path=\"3,3,3\"/><payline id=\"4\" path=\"1,2,3\"/><payline id=\"5\" path=\"3,2,1\"/></paylines><symbols><symbol id=\"1\" title=\"coin\"/><symbol id=\"2\" title=\"top\"/><symbol id=\"3\" title=\"mid4\"/><symbol id=\"4\" title=\"mid3\"/><symbol id=\"5\" title=\"mid2\"/><symbol id=\"6\" title=\"mid1\"/><symbol id=\"7\" title=\"low4\"/><symbol id=\"8\" title=\"low3\"/><symbol id=\"9\" title=\"low2\"/><symbol id=\"10\" title=\"low1\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"7,5,8,10,4,1,3,2,9,6\"/><reel id=\"2\" length=\"10\" symbols=\"4,5,10,6,3,1,7,9,8,2\"/><reel id=\"3\" length=\"10\" symbols=\"7,3,10,2,9,8,1,6,5,4\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"7,5,8,1,10,4,3,2,9,6\"/><reel id=\"2\" length=\"10\" symbols=\"4,6,5,10,3,1,7,9,8,2\"/><reel id=\"3\" length=\"10\" symbols=\"7,10,2,9,8,3,1,6,5,4\"/></reels></slot><shift server=\"0,0,0\" reel_set=\"1\" reel1=\"8,4,2\" reel2=\"1,5,10\" reel3=\"4,9,1\"/><shift_ext><shift server=\"0,0,0\" reel_set=\"0\" reel1=\"4,5,7\" reel2=\"3,9,10\" reel3=\"8,9,2\"/><shift server=\"0,0,0\" reel_set=\"0\" reel1=\"7,5,8\" reel2=\"8,2,5\" reel3=\"6,4,8\"/><shift server=\"0,0,0\" reel_set=\"0\" reel1=\"9,9,3\" reel2=\"8,5,10\" reel3=\"6,6,4\"/><shift server=\"0,0,0\" reel_set=\"0\" reel1=\"6,10,10\" reel2=\"6,3,5\" reel3=\"7,8,8\"/><shift server=\"0,0,0\" reel_set=\"0\" reel1=\"4,2,7\" reel2=\"8,10,6\" reel3=\"9,7,10\"/><shift server=\"0,0,0\" reel_set=\"0\" reel1=\"7,7,2\" reel2=\"8,10,6\" reel3=\"6,4,8\"/><shift server=\"0,0,0\" reel_set=\"0\" reel1=\"8,3,7\" reel2=\"9,4,7\" reel3=\"6,4,8\"/></shift_ext><game total_bet_mult=\"10\" jackpots_tb=\"25,150,1000\" bonus_spins=\"3\"/><delivery id=\"444898-3572875426204465483306413\" action=\"create\"/></server>";
            }
        }
        #endregion
        public RubyHitGameLogic()
        {
            _gameID = GAMEID.RubyHit;
            GameName = "RubyHit";
        }
    }
}
