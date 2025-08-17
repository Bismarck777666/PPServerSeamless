using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class HotChilliGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs9hotroll";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return false;
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 9; }
        }
        protected override int ServerResLineCount
        {
            get { return 9; }
        }
        protected override int ROWS
        {
            get
            {
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=8,4,8,3,8,3,8,4,8&c_paytable=9~explicit~5,2,5~5000;10~explicit~5,3,5~3000;11~explicit~5,4,5~2000;12~explicit~5,5,5~1000;13~any_with_added_wild_multiplier~6~100,0,0;14~any_with_added_wild_multiplier~7~75,0,0;15~any_with_added_wild_multiplier~6,7~50,0,0;16~any_with_added_wild_multiplier~8~30,0,0;17~any_with_added_wild_multiplier~9~20,0,0;18~any_with_added_wild_multiplier~10~10,0,0;19~any_with_added_wild_multiplier~8,9,10~5,0,0;20~anywhere_on_line~11~10,5,2&cfgs=2456&reel1=5,6,12,9,12,7,12,11,12,8,12,10,12,8,12,8,12,6,12,9,12,7,12,9,12,10,12,4,12,9,12,8,12,9,12,10,12,7,12,9,12,6,2,6,12,9,12,8,12,9,12,9,12,8,12,10,12,9,12,3,12,9,12,7,12,8,12,6,12&ver=2&reel0=6,12,8,12,10,12,5,12,6,12,10,12,9,12,7,12,7,12,10,12,8,12,11,12,10,12,7,12,7,12,10,12,7,12,8,12,10,12,6,5,6,12,10,12,9,12,7,12,10,12,9,12,10,12,8,12&def_sb=8,9,10&def_sa=5,6,7&reel2=10,12,9,12,10,12,8,12,8,12,5,12,10,12,9,12,10,12,10,12,7,12,9,12,8,12,11,12,10,12,8,12,6,5,6,12,10,12,7,12,8,12,10,12,9,12,8,12,9,12,8,12,10,12,6,12,11,12,8,12,7,12,10,12,8,12,10,12,8,12,9,12&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,800.00,1000.00,1500.00,3000.00,5000.00,8500.00,12000.00&defc=100.00&wilds=2~0,0,0~1,1,5;3~0,0,0~1,1,4;4~0,0,0~1,1,3;5~0,0,0~1,1,2&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0";
            }
        }
	
	
        #endregion
        public HotChilliGameLogic()
        {
            _gameID = GAMEID.HotChilli;
            GameName = "HotChilli";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
	
    }
}
