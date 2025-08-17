using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class VegasNightsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25vegas";
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
            get { return 25; }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
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
                return "def_s=5,8,7,3,8,10,11,3,8,3,11,7,8,5,9&cfgs=2585&ver=2&reel_set_size=4&def_sb=7,2,2,2,2&def_sa=8,6,10,9,6&scatters=1~0,0,2,0,0~5,5,5,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1;12~0,0,0,0,0~1,1,1,1,1;13~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;150,75,10,0,0;125,50,5,0,0;100,25,5,0,0;100,25,5,0,0;75,20,5,0,0;50,20,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=8,6,10,9,6,5,11,6,4,9,10,11,10,5,11,7,11,5,3~7,8,4,1,11,10,7,4,9,8,2,2,2,2,2,5,7,3,5,1,6,9,2,11,4,8,8~8,3,7,9,7,6,1,7,8,7,10,3,1,4,11,2,2,2,2,2,2,2,4,11,5,4~8,1,11,7,11,1,5,10,2,2,2,2,3,9,4,2,9,3,6,7,2~7,2,2,2,2,2,2,2,2,2,7,7,4,5,7,4,3,9,6,2,2,3,11,11,13,2,2,8,4,6,9,3,10,8,10,11,6,7,5,5,8,8,9,5,10,7,4,6,4,10,9&reel_set2=8,9,6,10,9,4,10,7,5,5,11,7,3,8,8,4,6,9,9,6,11,10~11,1,7,5,4,11,4,11,8,4,7,11,2,2,1,9,3,3,10,9,6,2,2,5~7,3,5,2,2,11,2,2,11,9,10,4,4,6,3,3,8,1,11,5,7,1,7~2,11,5,6,8,9,3,7,3,7,6,11,10,8,10,2,2,4,9,1~9,4,4,2,2,7,10,2,2,4,10,10,11,5,7,8,4,7,9,3,11,5,13,8,9,9,6,10,7,6,7,8,7,6,3,11,11,2,2,5,3&reel_set1=6,3,9,5,6,10,4,7,11,8,6,9,5,3,11,11,9,10,5,5~12,12,3,6,5,2,2,2,2,2,12,12,4,12,5,6,12,7,4,12,7~4,3,12,12,6,12,7,12,12,7,2,2,2,2,2,2,4,2,3,7,12,3,7,4,5~4,6,2,2,2,2,7,3,5,12,7,6,12,2,12,12,4,2~12,12,6,4,6,5,6,2,2,2,2,2,2,12,12,2,4,7,3,2,12,12,7,2,3,2,5,7,5,3,7,5,7,2,12,12&reel_set3=11,9,7,8,6,10,10,8,7,9,5,6,3,5,9,3,11,4,4,9~12,7,12,5,5,2,2,6,12,12,3,2,2,4,7,7,4,12~2,12,4,4,2,2,12,5,7,12,12,6,7,12,4,7,2,2,3,5,3~4,6,12,12,2,2,12,3,7,12,5,7,4,2,3,12,2,12~7,12,2,12,12,2,2,4,2,7,3,12,4,7,12,3,7,12,6,12,12,3,12,5,12,2,4,4,7,2,12,12,3,6,5,7,6,12,5";
            }
        }
	
	
        #endregion
        public VegasNightsGameLogic()
        {
            _gameID = GAMEID.VegasNights;
            GameName = "VegasNights";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
	
    }
}
