using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class LegendOfCleopatraMegaGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "legend_of_cleopatra_megaways";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get
            {
                return true;
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get
            {
                return 100.0;
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
                return "<server><source game-ver=\"50221\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"top1\" count=\"2\" coef=\"40\"/><combination symbol=\"1\" name=\"top1\" count=\"3\" coef=\"80\"/><combination symbol=\"1\" name=\"top1\" count=\"4\" coef=\"150\"/><combination symbol=\"1\" name=\"top1\" count=\"5\" coef=\"400\"/><combination symbol=\"1\" name=\"top1\" count=\"6\" coef=\"800\"/><combination symbol=\"2\" name=\"top2\" count=\"3\" coef=\"12\"/><combination symbol=\"2\" name=\"top2\" count=\"4\" coef=\"25\"/><combination symbol=\"2\" name=\"top2\" count=\"5\" coef=\"40\"/><combination symbol=\"2\" name=\"top2\" count=\"6\" coef=\"80\"/><combination symbol=\"3\" name=\"mid1\" count=\"3\" coef=\"10\"/><combination symbol=\"3\" name=\"mid1\" count=\"4\" coef=\"20\"/><combination symbol=\"3\" name=\"mid1\" count=\"5\" coef=\"30\"/><combination symbol=\"3\" name=\"mid1\" count=\"6\" coef=\"60\"/><combination symbol=\"4\" name=\"mid2\" count=\"3\" coef=\"8\"/><combination symbol=\"4\" name=\"mid2\" count=\"4\" coef=\"15\"/><combination symbol=\"4\" name=\"mid2\" count=\"5\" coef=\"20\"/><combination symbol=\"4\" name=\"mid2\" count=\"6\" coef=\"40\"/><combination symbol=\"5\" name=\"low1\" count=\"3\" coef=\"3\"/><combination symbol=\"5\" name=\"low1\" count=\"4\" coef=\"5\"/><combination symbol=\"5\" name=\"low1\" count=\"5\" coef=\"10\"/><combination symbol=\"5\" name=\"low1\" count=\"6\" coef=\"15\"/><combination symbol=\"6\" name=\"low2\" count=\"3\" coef=\"3\"/><combination symbol=\"6\" name=\"low2\" count=\"4\" coef=\"5\"/><combination symbol=\"6\" name=\"low2\" count=\"5\" coef=\"10\"/><combination symbol=\"6\" name=\"low2\" count=\"6\" coef=\"15\"/><combination symbol=\"7\" name=\"low3\" count=\"3\" coef=\"3\"/><combination symbol=\"7\" name=\"low3\" count=\"4\" coef=\"5\"/><combination symbol=\"7\" name=\"low3\" count=\"5\" coef=\"10\"/><combination symbol=\"7\" name=\"low3\" count=\"6\" coef=\"15\"/><combination symbol=\"8\" name=\"low4\" count=\"3\" coef=\"3\"/><combination symbol=\"8\" name=\"low4\" count=\"4\" coef=\"5\"/><combination symbol=\"8\" name=\"low4\" count=\"5\" coef=\"10\"/><combination symbol=\"8\" name=\"low4\" count=\"6\" coef=\"15\"/><combination symbol=\"9\" name=\"low5\" count=\"3\" coef=\"2\"/><combination symbol=\"9\" name=\"low5\" count=\"4\" coef=\"3\"/><combination symbol=\"9\" name=\"low5\" count=\"5\" coef=\"6\"/><combination symbol=\"9\" name=\"low5\" count=\"6\" coef=\"10\"/><combination symbol=\"10\" name=\"low6\" count=\"3\" coef=\"2\"/><combination symbol=\"10\" name=\"low6\" count=\"4\" coef=\"3\"/><combination symbol=\"10\" name=\"low6\" count=\"5\" coef=\"6\"/><combination symbol=\"10\" name=\"low6\" count=\"6\" coef=\"10\"/></combinations><symbols><symbol id=\"1\" title=\"top1\"/><symbol id=\"2\" title=\"top2\"/><symbol id=\"3\" title=\"mid1\"/><symbol id=\"4\" title=\"mid2\"/><symbol id=\"5\" title=\"low1\"/><symbol id=\"6\" title=\"low2\"/><symbol id=\"7\" title=\"low3\"/><symbol id=\"8\" title=\"low4\"/><symbol id=\"9\" title=\"low5\"/><symbol id=\"10\" title=\"low6\"/><symbol id=\"11\" title=\"wild\"/><symbol id=\"12\" title=\"empty\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"10\" symbols=\"7,3,6,10,8,2,5,1,4,9\"/><reel id=\"2\" length=\"11\" symbols=\"8,2,3,5,9,4,10,7,6,1,11\"/><reel id=\"3\" length=\"11\" symbols=\"7,9,2,4,6,10,3,5,1,8,11\"/><reel id=\"4\" length=\"11\" symbols=\"4,1,10,5,7,6,9,3,8,2,11\"/><reel id=\"5\" length=\"11\" symbols=\"1,6,10,9,8,4,3,2,5,7,11\"/><reel id=\"6\" length=\"10\" symbols=\"4,10,8,9,3,5,7,2,6,1\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"10\" symbols=\"7,3,6,10,8,2,5,1,4,9\"/><reel id=\"2\" length=\"11\" symbols=\"8,11,2,3,5,9,4,10,7,6,1\"/><reel id=\"3\" length=\"11\" symbols=\"7,9,2,4,6,10,3,5,1,8,11\"/><reel id=\"4\" length=\"11\" symbols=\"4,1,10,5,7,6,9,3,8,2,11\"/><reel id=\"5\" length=\"11\" symbols=\"1,6,10,9,8,4,3,2,5,7,11\"/><reel id=\"6\" length=\"10\" symbols=\"4,10,8,9,3,5,7,2,6,1\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"10\" symbols=\"7,9,2,4,6,10,3,5,1,8\"/><reel id=\"2\" length=\"11\" symbols=\"4,10,8,9,3,5,7,2,6,11,1\"/><reel id=\"3\" length=\"11\" symbols=\"8,7,10,4,9,6,3,1,2,5,11\"/><reel id=\"4\" length=\"11\" symbols=\"3,5,8,6,10,1,2,9,4,7,11\"/><reel id=\"5\" length=\"11\" symbols=\"5,6,7,3,10,11,9,8,2,4,1\"/><reel id=\"6\" length=\"10\" symbols=\"4,8,7,5,6,10,2,9,3,1\"/></reels></slot><shift server=\"0,0,0,0,0,0\" reel_set=\"1\" reel1=\"7,7,7,10,10\" reel2=\"1,2,2,2,2\" reel3=\"3,7,7,4,4,4\" reel4=\"2,1,3\" reel5=\"4,11,7,10,3\" reel6=\"1,2,4,8,9\" top_row_multipliers=\"3,1,2,2\"/><game free_game_cost=\"100\" total_bet_mult=\"20\" base_ways=\"200704\"/><window><over_size up=\"7,2,10,8,9,7\" down=\"10,5,9,5,7,10\" left=\"10\" right=\"2\" top_row_multipliers=\"3,1,2,2\"/></window><delivery id=\"562383-9155227033436084086101178\" action=\"create\"/></server>";
            }
        }
        #endregion

        public LegendOfCleopatraMegaGameLogic()
        {
            _gameID     = GAMEID.LegendOfCleopatraMega;
            GameName    = "LegendOfCleopatraMega";
        }
    }
}
