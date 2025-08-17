using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FireHot20GameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20fh";
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
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
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
                return "def_s=1,5,7,4,7,11,10,5,2,10,3,6,9,4,8&cfgs=6220&ver=3&def_sb=11,7,6,3,8&reel_set_size=1&def_sa=11,9,5,2,6&scatters=1~100,25,3,0,0~0,0,0,0,0~1,1,1,1,1;11~0,0,20,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"38019144\"}}&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2700,230,50,10,0;520,110,40,0,0;520,110,40,0,0;230,60,20,0,0;100,35,10,0,0;100,35,10,0,0;100,35,10,0,0;100,35,10,0,0;0,0,0,0,0&reel_set0=3,9,4,5,7,9,8,8,7,4,8,4,11,10,10,10,10,6,3,7,7,9,10,11,8,1,10,11,7,9,7,7,7,7,10,5,7,4,8,9,9,5,7,8,8,10,10,8,5,5,5,5,8,10,7,9,9,10,7,3,9,3,10,6,10,5,6,6,6,6,7,10,5,6,10,8,9,10,4,1,4,10,5,4,4,4,4,10,8,9,11,3,10,3,8,10,7,6,3,9,9,8,9,9,9,9,7,10,10,9,8,6,9,1,5,4,5,10,4,6,10,8,8,8,8,6,9,1,6,5,7,10,10,8,6,8,10,11,8,6~8,1,10,5,9,10,1,8,6,7,7,7,7,10,8,9,10,8,1,10,4,9,2,9,9,9,9,3,2,5,5,9,4,5,9,3,5,10,10,10,10,8,7,4,9,9,10,9,7,7,5,5,5,5,3,6,7,8,7,5,6,7,9,8,8,8,8,6,4,5,7,5,10,8,8,7,4,4,4,4,10,6,5,7,3,9,4,8,7,10,6,6,6,6,4,7,9,9,7,4,8,9,10,4,7~8,9,6,10,4,7,4,9,10,2,7,5,8,9,9,9,9,7,6,5,3,6,3,9,10,8,4,10,4,6,3,9,4,4,4,4,6,7,1,6,3,8,1,6,7,6,8,3,8,4,10,10,10,10,6,7,11,2,7,6,7,4,9,8,7,5,3,6,7,7,7,7,11,6,8,7,7,9,4,7,6,11,7,5,5,5,10,10,6,4,10,4,9,6,5,8,1,9,3,7,6,6,6,6,10,1,5,3,8,5,8,9,7,7,8,2,9,4,7,8,8,8,8,6,10,7,9,4,4,8,10,6,5,8,7,8,11,9,7~5,3,7,9,3,9,7,7,5,7,8,10,8,8,8,8,9,8,4,6,7,6,5,3,9,7,6,9,9,9,9,10,7,7,9,2,6,6,7,6,10,8,1,5,4,4,4,4,5,6,1,7,10,3,10,8,6,8,7,3,4,10,10,10,10,6,8,4,4,9,9,7,1,10,5,9,9,8,7,7,7,7,10,9,4,8,6,5,4,10,9,7,9,10,4,5,6,6,6,6,4,10,4,8,8,4,6,3,8,6,6,3,8,5,5,5,5,4,8,9,9,8,5,2,10,7,10,7,1,4,8~6,11,10,6,9,7,8,5,6,7,7,8,9,8,8,8,8,7,3,7,11,9,11,9,4,3,8,10,3,9,6,6,6,6,4,10,3,7,8,5,1,4,10,4,9,3,7,5,10,7,7,7,7,9,9,7,8,9,8,4,8,10,4,7,6,9,9,11,9,9,9,9,6,4,8,1,10,1,4,4,8,8,9,10,9,9,6,5,5,5,5,8,9,4,9,7,1,8,6,10,3,5,10,4,8,5,10,10,10,10,6,7,4,8,7,6,8,6,7,4,5,7,9,8,6,4,4,4,4,9,3,8,9,7,3,6,10,8,4,5,7,5,4,10";
            }
        }
	
	
        #endregion
        public FireHot20GameLogic()
        {
            _gameID = GAMEID.FireHot20;
            GameName = "FireHot20";
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
