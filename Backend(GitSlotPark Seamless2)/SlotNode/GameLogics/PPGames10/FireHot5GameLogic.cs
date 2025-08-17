using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FireHot5GameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5firehot";
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
                return "def_s=4,8,4,1,8,3,7,8,3,10,11,9,11,2,5&cfgs=6214&ver=3&def_sb=3,10,7,4,5&reel_set_size=1&def_sa=11,10,4,2,10&scatters=1~100,25,3,0,0~0,0,0,0,0~1,1,1,1,1;11~0,0,20,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2053033750\"}}&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2700,230,50,10,0;520,110,40,0,0;520,110,40,0,0;230,60,20,0,0;100,35,10,0,0;100,35,10,0,0;100,35,10,0,0;100,35,10,0,0;0,0,0,0,0&reel_set0=7,4,3,10,11,8,5,7,7,9,10,6,8,11,10,10,10,10,6,6,9,5,10,5,5,10,7,6,9,6,1,10,9,4,7,7,7,7,9,10,4,8,5,10,7,10,7,5,7,9,10,6,10,3,5,5,5,8,7,10,7,8,9,6,1,10,8,10,5,3,10,7,10,6,6,6,6,3,9,8,3,8,10,11,6,9,5,1,10,4,11,4,8,4,4,4,11,9,4,8,9,3,10,3,7,9,6,1,4,8,9,9,9,9,5,7,8,8,10,10,3,8,1,10,8,8,9,1,8,8,8,8,5,6,10,10,7,7,10,9,6,8,9,5,6,9,10,7,4,7~7,4,5,9,10,7,9,7,5,9,9,3,8,5,9,3,7,7,7,7,9,5,6,5,10,10,5,5,10,9,8,7,10,10,9,7,5,9,9,9,9,8,7,9,8,5,8,5,8,9,9,7,3,1,7,4,5,4,7,10,10,10,10,9,1,8,4,10,10,9,4,10,7,2,6,8,10,8,4,5,5,5,5,8,10,2,7,9,1,9,10,7,5,7,6,3,7,8,3,8,8,8,8,1,9,10,8,10,8,7,6,9,5,8,6,10,5,10,4,10,4,4,4,4,8,7,6,9,6,6,4,4,9,7,8,3,9,5,6,3,7,6,6,6,10,7,7,10,7,8,8,10,4,8,5,4,4,5,7,4,8,9,9~5,4,2,6,8,8,6,4,9,9,9,9,10,8,8,7,7,10,6,7,11,3,4,4,4,4,6,5,7,11,9,8,3,5,1,7,10,10,10,9,9,3,8,5,4,7,10,1,6,7,7,7,7,4,7,10,9,10,7,9,2,5,5,5,5,3,1,5,6,11,7,7,8,10,9,6,6,6,6,10,4,4,7,8,10,6,4,8,7,8,8,8,8,4,6,9,6,9,3,6,7,11,8,6~9,4,10,5,6,4,6,7,8,6,4,10,4,8,8,8,8,10,6,8,8,6,6,5,10,8,4,10,4,5,9,9,9,9,5,5,10,7,3,9,8,9,10,9,7,7,8,9,10,4,4,4,4,7,7,4,5,4,9,6,6,9,10,9,2,8,7,10,10,10,10,8,5,4,1,7,8,1,8,10,4,7,10,7,6,7,7,7,7,6,9,5,3,6,1,8,7,9,9,5,7,4,10,8,6,6,6,6,3,6,3,8,1,7,3,7,3,5,3,9,6,7,8,5,5,5,5,4,7,3,10,8,3,2,9,9,4,4,6,4,5,8,6~8,8,9,3,5,9,9,7,4,8,8,8,8,9,9,1,4,9,9,8,4,5,6,6,6,6,10,7,3,6,4,8,5,8,7,3,7,7,7,7,4,6,3,10,4,11,7,6,10,9,9,9,9,11,7,8,8,4,11,10,10,9,6,5,5,5,5,10,4,8,6,9,6,10,5,5,1,10,10,10,10,1,4,8,3,7,7,4,4,6,9,4,4,4,4,10,7,7,9,9,11,9,8,8,7,3,6";
            }
        }
	
	
        #endregion
        public FireHot5GameLogic()
        {
            _gameID = GAMEID.FireHot5;
            GameName = "FireHot5";
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
