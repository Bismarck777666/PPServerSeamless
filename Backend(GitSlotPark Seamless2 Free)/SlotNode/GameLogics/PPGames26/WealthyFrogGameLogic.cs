using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WealthyFrogGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5wfrog";
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
                return "def_s=8,7,4,9,8,6,7,4,9,8,3,7,7,6,6&cfgs=1&reel1=8,3,6,6,6,7,7,7,4,4,4,3,4,7,5,1,7,8,6,9,9,9,8,9,9,4,4&ver=2&reel0=8,8,8,7,7,7,4,6,6,6,5,7,6,1,4,3,6,8,9,8,7,9&def_sb=9,9,9,5,6&def_sa=8,8,8,7,7&reel3=6,6,6,9,9,9,7,6,6,8,8,8,4,8,3,5,3,9,9,9,6,6,5,4,8,1&reel2=4,9,9,9,9,8,8,8,3,6,8,9,5,4,7,8,5,6,1,7,6,8,7,8&reel4=9,9,9,5,6,6,6,3,9,6,9,3,5,6,7,8,8,8,6,3,1,8,6,8,4,4&scatters=1~250,50,10,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={rtps:{regular:\"96.50\"},props:{max_rnd_sim:\"0\",max_rnd_hr:\"40445\",max_rnd_win:\"1000\"}}&wl_i=tbm~1000&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~0,0,0~1,1,1&bonuses=0&fsbonus=&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,1000,100,0,0;1000,200,50,0,0;1000,200,50,0,0;200,50,20,0,0;200,50,20,0,0;200,40,20,0,0;200,40,20,5,0";
            }
        }
	
	
        #endregion
        public WealthyFrogGameLogic()
        {
            _gameID = GAMEID.WealthyFrog;
            GameName = "WealthyFrog";
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
