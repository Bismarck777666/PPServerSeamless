using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WildPixiesGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20wildpix";
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
                return "def_s=6,7,4,3,8,9,8,5,6,7,8,6,7,3,9&cfgs=2180&ver=2&reel_set_size=3&def_sb=9,3,7,11,12&def_sa=2,4,2,3,9&prg_cfg_m=s&scatters=1~0,0,0,0,0~0,0,8,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&prg_cfg=2&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~500,250,100,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,150,50,0,0;200,120,25,0,0;200,120,25,0,0;150,100,15,0,0;150,100,10,0,0;125,100,10,0,0;125,50,10,0,0;120,25,5,0,0;100,20,5,0,0;100,20,5,0,0&reel_set0=6,11,6,9,9,9,2,9,8,8,8,3,1,6,9,6,11,9,3,11,7,10,12,5,4~4,7,11,11,12,3,11,5,9,10,2,7,10,10,2,4,8,10,2,5,5,7,11,7,10,11,6,4,10,11,10,5,7~6,3,12,7,12,2,9,8,1,12,4,2,7,10,2,8,4,6,6,6,12,2,5,8,1,12,3,6,7,12,12,7,12,1,10,11,7,12,12,10~2,12,11,12,11,9,9,9,8,8,12,7,3,11,11,4,12,11,7,12,10,12,4,11,3,11,12,4,6,11,10,11,6,10,5,12,10,11,10,10,10,7,11,10,11,10,11,11,7,6,10,7,12,7~6,7,12,2,11,12,11,9,7,9,10,12,4,11,1,7,12,5,11,10,11,10,6,12,10,11,10,7,11,6,9,8,8,8,9,10,11,5,3,12,7,12,8,12,7,12,11,10,5&reel_set2=6,11,6,9,2,9,8,8,8,3,12,8,10,6,5,8,10,8,5,5,5,4,8,9,9,6,9,9,9,11,3,12,11,6,9,6,11,9,3,11,7~4,10,10,4,10,5,3,12,4,8,11,5,9,11,12,5,9,10,10,4,9,5,2,9,6,7,11,11,2,5,5,5,11,7,10,7,11,7,11,7,11,7,10,11,4,10,11,10,5,7~6,6,6,3,12,7,12,2,9,8,12,4,7,10,8,4,7,12,10,11,7,12,12,10,5~2,12,11,12,11,9,7,6,9,8,8,8,9,5,6,3,7,5,9,4,9,10,10,5,9,9,6,9,5,3,11,11,12,4,6,11,10,11,6,10,5,12,12,10,11,10,7,11,10,11,10,11,7~6,7,12,2,3,11,12,11,9,7,9,8,7,12,5,11,10,11,10,10,6,12,12,10,11,10,7,4,11,10,11,10,11,11,6,9,8,8,8,9,10,11,5,12,7,12,10,5&reel_set1=6,11,6,9,2,9,8,3,12,8,10,6,5,2,8,9,8,6,8,8,8,9,6,4,6,9,9,6,9,9,9,11,3,12,11,6,9,6,11,9,3,11,7~4,7,11,12,11,5,3,9,10,10,10,2,7,11,11,11,6,7,11,7,10,11,4,10,11,10,5,5,5,7,8~6,3,12,7,12,2,9,8,12,4,7,12,6,6,6,10,11,5,7,12,12,10~2,12,11,12,11,9,7,9,8,8,12,7,10,10,6,8,2,8,10,7,8,6,12,6,3,11,11,4,12,12,11,7,12,10,12,4,11,3,11,12,4,6,11,10,11,6,10,5,12,12,10,11,10,7,11,10,11,10,11,11,7,6,10,7,12,7~6,7,12,2,11,12,11,9,7,9,8,12,2,10,3,8,4,5,12,4,10,3,8,8,9,8,12,12,11,7,10,12,4,11,7,12,5,11,10,11,10,10,6,12,10,11,11,6,9,8,9,10,11,5,12,7,12,8,12,7,12,11,10,5";
            }
        }
	
	
        #endregion
        public WildPixiesGameLogic()
        {
            _gameID = GAMEID.WildPixies;
            GameName = "WildPixies";
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
