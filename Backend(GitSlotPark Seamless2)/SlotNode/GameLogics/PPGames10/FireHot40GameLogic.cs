using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FireHot40GameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40firehot";
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
            get { return 40; }
        }
        protected override int ServerResLineCount
        {
            get { return 40; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,6,3,1,5,4,5,4,4,7,3,9,11,2,8,11,8,9,4,6&cfgs=6208&ver=3&def_sb=11,10,3,4,5&reel_set_size=1&def_sa=3,7,10,3,9&scatters=1~100,25,3,0,0~0,0,0,0,0~1,1,1,1,1;11~0,0,20,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"9799643\"}}&sc=5.00,10.00,15.00,20.00,25.00,50.00,75.00,100.00,125.00,190.00,250.00,375.00,625.00,1250.00,1875.00,2500.00&defc=25.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2700,230,50,10,0;520,110,40,0,0;520,110,40,0,0;230,60,20,0,0;100,35,10,0,0;100,35,10,0,0;100,35,10,0,0;100,35,10,0,0;0,0,0,0,0&reel_set0=10,8,6,9,7,7,8,1,7,10,7,7,5,4,4,4,4,8,10,7,6,6,10,3,10,7,6,7,10,10,11,8,5,7,7,7,7,4,10,11,9,6,8,3,4,8,9,8,6,4,4,9,10,5,5,5,5,10,7,10,9,11,8,9,10,7,8,10,4,3,10,3,6,6,6,6,8,9,4,5,8,3,8,7,6,6,8,7,4,8,3,3,3,3,7,10,5,5,8,8,11,9,7,5,8,4,3,9,8,9,10,10,10,10,8,6,1,9,9,3,6,7,9,10,4,1,10,9,9,9,9,6,11,3,4,4,10,10,8,9,6,3,5,8,6,8,4,8,8,8,8,4,10,6,3,5,9,10,6,9,8,8,9,9,10,10,11,4~5,6,5,8,10,8,4,9,10,7,7,7,7,5,9,4,3,8,7,4,6,10,7,2,9,9,9,9,7,9,7,7,6,7,7,8,8,5,10,10,10,10,4,10,1,8,7,9,3,10,10,9,10,5,5,5,6,3,8,5,8,7,9,8,5,8,8,8,8,9,4,9,8,10,8,7,7,5,10,7,4,4,4,4,9,6,7,10,7,7,3,9,5,9,6,6,6,6,8,5,8,10,6,6,7,8,10,9,6,3,3,3,3,5,5,10,8,9,10,9,8,9,9,3,1~10,7,3,8,8,5,7,8,2,9,9,9,9,4,5,10,6,9,10,9,10,3,9,5,4,4,4,4,9,5,10,9,10,3,1,7,3,6,10,10,10,10,9,10,5,3,6,7,8,5,8,6,6,7,7,7,7,4,7,11,8,1,10,9,3,11,9,5,5,5,5,8,10,3,5,8,7,3,4,3,8,5,3,3,3,3,7,10,10,3,5,7,7,6,8,9,6,6,6,6,5,4,4,9,7,7,4,10,10,4,5,8,8,8,8,5,6,4,3,4,4,9,7,9,11,6,3~10,8,5,8,9,5,10,8,5,4,9,5,4,7,6,10,8,8,8,8,5,5,6,1,9,8,4,10,10,6,9,4,8,4,8,10,6,9,9,9,9,5,9,6,4,6,9,6,4,5,3,4,6,3,1,7,1,10,8,4,4,4,4,8,4,8,3,5,6,9,6,9,4,2,5,3,9,5,10,5,10,10,10,10,7,9,10,3,4,8,10,8,5,4,2,10,5,10,8,5,5,7,7,7,7,4,9,5,4,9,6,3,10,9,8,1,10,8,6,8,10,6,7,6,6,6,6,5,9,9,5,7,9,8,10,5,6,9,9,7,8,10,8,5,5,5,5,8,6,3,9,6,10,4,9,3,4,8,6,4,7,6,4,3,3,3,3,8,10,9,3,3,6,10,9,4,6,8,10,4,6,4,3,10,4,5~4,10,6,9,5,3,7,4,8,8,3,4,3,8,8,8,8,4,5,9,5,7,6,10,10,8,9,10,8,4,10,6,6,6,6,10,6,8,3,4,3,7,7,10,1,3,4,7,6,10,7,7,7,7,7,9,9,4,4,1,9,11,6,6,1,7,5,8,4,3,3,3,3,7,8,9,9,10,7,10,8,7,8,6,11,7,6,3,9,9,9,9,5,4,4,3,7,8,6,8,6,4,9,10,7,3,5,5,5,5,5,3,3,9,9,11,10,9,4,8,3,10,7,6,9,10,10,10,10,10,9,5,8,5,8,3,6,5,8,6,11,9,4,5,4,4,4,4,10,10,7,5,3,7,4,3,6,10,8,6,9,6,7,3";
            }
        }
	
	
        #endregion
        public FireHot40GameLogic()
        {
            _gameID = GAMEID.FireHot40;
            GameName = "FireHot40";
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
