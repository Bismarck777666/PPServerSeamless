using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class ShiningHot5GameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5sh";
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
                return "def_s=3,8,7,3,8,5,7,3,4,6,4,6,8,3,8&cfgs=5960&ver=3&def_sb=3,7,8,5,7&reel_set_size=1&def_sa=5,8,6,4,8&scatters=1~500,15,5,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2534663460\",max_rnd_win:\"2500\"}}&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~2500,700,50,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1200,250,15,0,0;230,50,15,0,0;230,50,15,0,0;110,25,10,0,0;110,25,10,0,0;110,25,10,0,0&reel_set0=7,7,4,4,7,8,4,8,7,3,6,8,4,4,4,4,6,5,2,3,6,3,8,7,2,1,3,7,4,7,7,7,8,8,4,4,6,4,5,7,7,6,7,3,6,6,6,7,8,8,4,6,6,4,6,4,4,3,3,8,5,5,5,8,6,7,5,4,6,8,5,3,6,8,8,7,8,8,8,8,7,7,8,2,7,6,7,3,3,6,7,8,3,3,3,3,7,7,8,3,1,3,5,5,4,4,8,3,2,2,2,6,8,1,6,4,4,6,6,3,6,7,4,6,4~7,8,5,6,5,8,8,4,5,6,6,6,7,8,8,5,6,6,4,3,7,5,5,5,8,7,8,3,7,4,8,2,6,7,8,8,8,2,8,3,7,3,5,5,6,6,1,7,7,7,5,6,3,4,6,8,7,6,5,7,4,4,4,1,5,1,6,7,6,4,8,8,4,2,2,2,7,5,5,8,6,7,6,3,1,5,3,3,3,3,5,7,8,8,5,6,7,6,8,7~5,3,6,3,3,1,8,4,3,5,8,8,8,6,4,6,7,5,7,4,7,6,5,3,4,4,4,6,3,8,8,5,5,2,3,4,3,7,3,3,3,3,5,5,6,5,4,3,8,4,6,3,6,7,6,6,6,3,4,7,4,2,7,6,6,5,3,3,5,5,5,8,5,6,7,6,8,4,5,6,4,5,2,2,2,1,4,8,4,3,5,6,8,6,5,7,7,7,1,3,1,6,8,4,8,5,8,6,4,5,3~5,8,7,8,3,7,7,3,3,8,4,7,7,7,7,6,3,4,4,7,5,5,7,5,1,4,7,4,8,8,8,3,4,2,7,4,8,1,7,6,1,7,8,7,4,4,4,3,7,5,8,8,1,4,6,6,5,6,8,7,6,6,6,4,2,7,5,5,2,6,4,8,8,7,2,5,5,5,5,4,8,7,4,4,3,4,6,8,8,7,5,8,2,2,2,2,8,3,7,4,5,8,6,5,8,2,5,8,7,3,3,3,3,2,5,5,4,4,6,7,5,4,5,6,5,8,4~6,8,7,3,6,7,7,3,5,2,7,7,7,7,8,6,8,8,4,4,7,3,7,8,4,5,5,5,5,8,3,8,3,7,8,7,4,6,6,8,6,6,6,6,6,8,3,8,8,6,4,6,7,1,2,5,3,3,3,3,4,2,3,6,4,5,4,2,2,6,3,8,8,8,8,6,1,3,6,3,7,8,7,5,1,8,4,4,4,4,7,7,5,4,4,7,6,4,3,3,2,2,2,2,7,3,6,7,4,5,6,4,2,6,7,3";
            }
        }
	
	
        #endregion
        public ShiningHot5GameLogic()
        {
            _gameID = GAMEID.ShiningHot5;
            GameName = "ShiningHot5";
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
