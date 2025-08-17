using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class PlushieWinsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1dragon888";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 1; }
        }
        protected override int ServerResLineCount
        {
            get { return 1; }
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
                return "def_s=3,6,5,6,3,6,4,6,4&cfgs=1&reel1=3,6,5,6,3,6,4,6,5,6,5,6,3,6,3,6,3,6&ver=2&reel0=5,6,3,6,3,6,4,6,4,6,4,6,5,6&def_sb=6,5,4&def_sa=6,3,3&reel2=4,6,4,6,3,6,4,6,5,6,5,6,5,6&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&gameInfo={rtps:{regular:\"96.84\"},props:{max_rnd_sim:\"0\",max_rnd_hr:\"807\",max_rnd_win:\"100\"}}&wl_i=tbm~100&sc=200.00,400.00,600.00,800.00,1000.00,2000.00,3000.00,4000.00,5000.00,7500.00,10000.00,15000.00,25000.00,50000.00,75000.00,100000.00&defc=1000.00&wilds=2~0,0,0~1,1,1&bonuses=0&fsbonus=&ntp=0.00&paytable=0,0,0;0,0,0;0,0,0;100,0,0;50,0,0;25,0,0;0,0,0";
            }
        }
	
	
        #endregion
        public PlushieWinsGameLogic()
        {
            _gameID = GAMEID.PlushieWins;
            GameName = "PlushieWins";
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
