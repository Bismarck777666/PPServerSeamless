using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class LuckyMonkeyGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5luckymly";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 5; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
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
                return "def_s=6,2,2,4,7,3,6,2,2&cfgs=1&ver=3&def_sb=5,8,2&reel_set_size=2&def_sa=6,2,2&scatters=1~0,0,0~0,0,0~1,1,1&rt=d&gameInfo={rtps:{regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"4784689\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~250,0,0~1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0;0,0,0;0,0,0;50,0,0;20,0,0;10,0,0;5,0,0;3,0,0;2,0,0;0,0,0&reel_set0=7,5,5,7,4,3,4,8,7,5,7,3,3,3,4,6,4,7,5,5,8,8,6,5,6,7,8,4,4,4,4,4,4,4,6,3,7,7,5,6,3,6,7,5,8,7,6,5,5,5,5,6,7,7,8,4,4,7,8,7,7,8,5,7,6,6,6,6,6,6,8,4,7,8,6,7,8,2,4,4,5,6,8,7,7,7,7,6,4,4,6,5,8,6,2,5,7,8,6,3,8,8,8,8,3,8,8,5,7,6,5,8,5,6,6,7,8,8~6,8,6,2,6,2,6,6,8,2,2,2,2,2,4,3,5,2,8,8,6,8,3,8,4,3,3,3,2,6,7,5,6,7,5,7,6,6,8,4,4,4,4,3,6,7,8,5,8,6,8,4,7,8,5,5,5,4,6,8,5,5,3,5,7,7,6,6,6,6,6,4,4,6,5,8,8,4,7,8,5,8,7,7,7,3,7,2,7,2,5,2,5,2,7,8,8,8,8,8,7,2,8,7,3,6,6,5,3,5,8~6,2,8,8,4,8,7,6,5,3,7,5,7,2,2,2,5,8,8,6,6,7,6,5,8,6,6,8,6,5,5,6,6,6,6,6,8,3,7,8,5,8,7,7,6,7,4,8,7,7,7,6,3,7,8,4,2,2,6,5,4,7,3,7,7,4,8,8,8,3,5,5,7,2,6,7,7,8,4,4,8,8,6,8,8&reel_set1=3,8,7,8,3,5,3,5,4,7,6,8,4,3,3,3,4,3,8,7,3,4,8,4,5,5,4,6,8,5,4,4,4,2,4,7,6,6,7,7,6,8,6,3,6,8,8,5,5,5,5,6,2,3,8,6,3,4,2,7,7,6,7,4,5,6,6,6,6,6,6,3,6,8,8,4,8,8,7,6,6,5,7,7,4,7,7,7,2,6,7,5,5,4,8,8,7,4,3,3,5,8,8,8,8,3,8,6,8,5,5,6,4,5,8,4,7,4,5,7~6,7,5,5,6,2,4,2,2,2,7,8,3,7,6,7,8,6,5,3,3,3,3,7,6,7,5,4,6,8,3,3,4,4,4,4,6,5,3,8,3,2,4,7,3,5,5,5,4,3,2,8,4,2,7,2,8,6,6,6,6,5,2,4,5,3,4,8,5,7,7,7,5,6,4,8,6,8,5,8,8,8,8,3,8,8,7,6,5,7,6,7,8,3~5,3,8,7,5,3,6,7,5,5,6,4,4,4,8,6,8,4,7,3,6,2,4,7,7,8,5,5,5,3,6,8,5,4,7,7,4,6,7,5,6,4,6,6,6,6,6,3,8,7,8,8,4,8,3,3,4,6,7,4,7,7,7,6,5,8,3,5,5,7,6,5,8,2,4,2,8,8,8,6,7,7,6,7,7,3,5,7,6,3,5";
            }
        }
	
	
        #endregion
        public LuckyMonkeyGameLogic()
        {
            _gameID = GAMEID.LuckyMonkey;
            GameName = "LuckyMonkey";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "3";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
	
    }
}
