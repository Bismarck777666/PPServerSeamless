using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MasterChensFortuneGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs9chen";
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
                return "def_s=7,3,9,11,8,5,12,3,7,4,11,9,10,3,12&cfgs=1755&reel1=9,10,12,4,7,6,1,11,9,7,3,9,10,2,12,12,12,10,11,9,9,5,12,9,7,11,8,9,5,10,8,8,9,9&ver=2&reel0=7,11,5,12,9,8,4,8,7,10,8,10,6,8,10,7,12,12,8,3,12,7,8,11,6,11,11,1,9,8,8,10,5,11,9&def_sb=8,10,8,11,7&def_sa=8,9,10,11,12&reel3=8,1,11,8,11,7,11,12,2,11,10,11,8,4,10,10,9,2,9,4,5,7,7,11,12,9,11,7,6,3,6,11,8,11&reel2=3,9,11,8,5,7,10,11,10,6,7,7,12,10,2,12,11,8,9,2,9,1,12,4,12,12,10,8,10,9,10,10,6&reel4=4,12,8,8,12,9,7,7,6,9,11,12,12,10,5,8,12,5,12,11,7,12,6,11,12,10,7,1,8,9,5,3,12&scatters=1~100,10,5,1,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,800.00,1000.00,1500.00,3000.00,5000.00,8500.00,12000.00&defc=100.00&wilds=2~0,0,0,0,0~2,2,2,2,2&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,500,100,10,2;2500,250,50,5,0;1250,100,20,3,0;750,100,20,3,0;500,30,10,0,0;300,25,5,0,0;200,20,5,0,0;150,20,5,0,0;125,15,5,0,0;100,15,5,0,0";
            }
        }
	
	
        #endregion
        public MasterChensFortuneGameLogic()
        {
            _gameID = GAMEID.MasterChensFortune;
            GameName = "MasterChensFortune";
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
