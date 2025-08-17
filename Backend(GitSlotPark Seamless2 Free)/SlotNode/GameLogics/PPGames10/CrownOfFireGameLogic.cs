using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class CrownOfFireGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10crownfire";
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
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "def_s=2,10,1,5,10,4,11,11,3,8,5,8,6,5,7&cfgs=6344&ver=3&def_sb=4,8,8,3,8&reel_set_size=1&def_sa=2,7,10,3,7&scatters=1~100,25,3,0,0~0,0,0,0,0~1,1,1,1,1;2~0,0,20,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"18508301\"}}&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&wilds=3~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;4500,250,50,10,0;550,120,40,0,0;550,120,40,0,0;250,60,20,0,0;100,40,10,0,0;100,40,10,0,0;100,40,10,0,0;100,40,10,0,0&reel_set0=11,11,10,8,4,7,1,10,10,5,2,7,8,10,8,8,8,6,11,10,9,4,9,5,4,8,11,10,7,10,4,7,8,11,11,11,1,8,6,7,10,5,8,4,6,4,9,7,8,5,9,8,10,10,10,8,7,8,9,6,11,2,9,5,11,1,11,9,11,9,10,7,7,7,9,11,11,5,9,6,8,6,10,9,9,2,5,6,10,9,9,9,11,5,9,11,11,7,8,8,7,10,9,5,9,7,10,2,8~9,4,10,7,9,5,4,9,7,10,6,10,7,11,8,6,8,6,8,8,8,10,4,10,8,3,10,11,5,6,11,5,8,8,10,11,4,8,8,4,10,10,10,10,5,10,7,10,8,10,9,5,10,11,10,4,11,7,6,11,6,8,10,10,11,11,11,7,8,9,11,8,5,8,6,11,11,5,6,4,6,6,11,10,11,9,8,9,9,9,1,6,11,3,7,10,10,8,7,6,8,6,11,9,9,1,8,11,8,7,7,7,11,8,9,7,11,6,11,4,6,5,8,10,11,1,8,5,3,6,11,10,10~8,11,8,7,10,9,11,4,8,4,5,8,8,7,9,11,10,10,10,8,9,3,2,8,6,9,11,9,5,10,6,5,11,10,11,4,10,8,8,8,6,4,6,10,8,9,8,1,7,6,9,5,8,9,7,2,7,7,7,10,8,1,9,4,10,2,11,6,8,7,9,8,4,7,11,7,2,11,11,11,6,11,5,10,1,8,7,4,7,9,5,10,6,11,1,7,1,9,9,9,7,9,11,10,4,3,4,10,6,11,5,11,6,2,10,11,11,7,8~11,4,7,1,5,10,7,11,7,9,10,9,10,9,7,5,9,9,9,4,8,9,10,5,10,4,9,10,8,6,7,10,6,1,10,10,10,11,8,9,11,10,9,6,9,9,11,6,10,5,7,10,9,7,9,8,8,8,11,5,6,1,5,8,8,4,9,7,10,5,10,10,1,9,9,11,11,11,7,4,6,5,10,10,11,5,10,9,3,11,8,9,9,5,8,9,7,7,7,5,11,10,5,6,10,8,7,11,7,7,10,7,4,3,4,10,4~11,8,9,7,5,11,8,10,10,1,5,8,8,7,10,11,9,9,9,10,7,4,10,8,10,11,8,4,9,9,1,10,6,4,10,10,10,11,9,10,5,11,4,10,8,5,9,2,8,6,11,6,10,6,7,7,7,4,8,9,6,10,1,6,7,4,2,7,5,7,5,2,8,8,8,9,8,7,2,5,9,7,9,10,5,9,7,7,9,6,4,11,11,11,8,10,11,10,11,8,9,11,11,2,11,6,9,4,5,1,4,8,11";
            }
        }
	
	
        #endregion
        public CrownOfFireGameLogic()
        {
            _gameID = GAMEID.CrownOfFire;
            GameName = "CrownOfFire";
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
