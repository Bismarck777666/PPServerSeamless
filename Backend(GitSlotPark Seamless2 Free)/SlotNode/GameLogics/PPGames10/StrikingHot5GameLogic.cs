using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class StrikingHot5GameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5strh";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return true;
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
                return "def_s=5,9,3,5,6,1,8,7,3,8,5,7,4,5,9&cfgs=6310&ver=3&def_sb=4,6,8,3,8&reel_set_size=1&def_sa=3,7,5,4,6&scatters=1~50,10,2,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2475210303\",max_rnd_win:\"5000\"}}&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,800,100,0,0;520,180,50,0,0;520,180,50,0,0;220,60,20,0,0;220,60,20,0,0;220,60,20,0,0;220,60,20,5,0&reel_set0=4,8,9,3,7,6,7,1,5,4,4,4,4,4,8,9,7,3,8,7,8,7,3,9,9,9,9,6,8,8,7,4,9,4,6,3,9,6,6,6,6,7,5,5,3,9,9,8,4,8,4,5,5,5,5,5,8,7,9,8,6,8,6,4,7,9,7,7,7,4,8,5,1,3,8,8,1,5,8,8,8,8,1,7,9,5,9,6,9,5,5,9,9,6~7,8,7,9,9,8,5,7,9,7,6,6,6,6,6,1,4,8,3,8,6,6,5,8,6,6,5,5,5,5,7,9,9,8,7,4,3,8,4,9,8,8,8,8,7,5,1,7,5,5,8,5,1,7,6,9,9,9,9,3,9,3,9,4,1,5,4,6,6,8,7,7,7,7,9,6,8,6,9,4,7,5,6,7,7,4,4,4,6,8,6,4,4,8,6,5,7,8,5,3,6~8,6,3,4,9,7,9,7,4,5,8,8,8,8,6,3,5,4,1,5,6,4,5,5,6,7,4,4,4,4,6,8,8,9,7,8,6,9,7,7,9,3,5,5,5,5,7,8,4,4,5,4,6,5,9,5,4,6,6,6,6,5,6,5,5,9,1,4,6,8,9,5,6,9,9,9,6,8,6,6,9,3,7,1,3,4,3,6,7,7,7,7,6,9,9,6,3,4,9,1,9,5,9,9,8~4,4,6,9,7,3,9,4,4,8,7,7,7,7,7,8,7,8,6,5,5,8,4,7,5,6,4,3,8,8,8,8,9,9,3,8,6,1,8,1,5,4,5,9,5,7,4,4,4,4,6,4,8,4,9,7,5,4,4,7,4,9,7,8,9,9,9,9,1,7,1,7,5,4,8,9,3,5,3,4,5,5,6,6,6,6,8,9,9,8,6,8,4,7,4,6,5,6,7,8,5,5,5,5,7,8,7,8,7,7,5,6,5,7,5,5,8,7,6~4,8,7,3,7,6,4,5,1,4,8,8,7,9,1,7,7,7,7,7,8,3,4,7,7,5,6,5,4,4,9,9,6,8,6,4,6,4,5,5,5,5,9,7,4,6,5,3,5,6,5,5,9,6,7,8,4,6,6,6,6,6,9,8,8,6,7,9,3,5,7,6,8,9,5,7,4,6,9,9,8,8,8,8,6,1,9,8,4,6,8,9,5,8,6,7,8,7,8,4,5,9,9,9,9,5,3,5,6,7,8,3,7,8,6,8,9,5,4,5,8,3,8,4,4,4,4,7,5,6,4,7,7,8,4,9,9,8,3,6,4,7,1,8,6,4";
            }
        }
	
	
        #endregion
        public StrikingHot5GameLogic()
        {
            _gameID = GAMEID.StrikingHot5;
            GameName = "StrikingHot5";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
	
    }
}
