using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MasterGemsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1mjokfp";
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
                return 1;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=5,6,3,6,8&cfgs=1&reel1=3,7,4,7,3,6,7,6,7,6,7,6,8,8,8,6,7,6,7,5,9,8&ver=2&reel0=3,4,5,6,4,5,6,7,8,4,5,6,4,5,9&def_sb=5,7,8,5,4&def_sa=4,8,7,6,5&reel3=9,9,9,8,7,8,7,8,7,8,7,8,8,7,8,7,8,3,8,4,8,5,8,6,8,7&reel2=8,7,5,8,2,4,8,7,3,8,7,2,6,7,8,6,8,6,8,4,5,6,8,6,9,8,6,8&reel4=8,8,3,8,8,4,8,8,5,8,8,5,8,8,6,8,5,8,8,5,7,9&scatters=1~0,0,0,0,0,0~0,0,0,0,0~0,0,0,0,0,0&gmb=0,0,0&rt=d&gameInfo={rtps:{regular:\"96.46\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"29411764\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=200.00,400.00,600.00,800.00,1000.00,2000.00,3000.00,4000.00,5000.00,7500.00,10000.00,15000.00,25000.00,50000.00,75000.00,100000.00&defc=1000.00&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&fsbonus=&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,50,15,0,0;50,25,10,0,0;25,15,8,0,0;15,10,5,0,0;12,8,4,0,0;10,5,2,0,0;8,3,1,0,0&t=symbol_count";
            }
        }
        #endregion
        
        public MasterGemsGameLogic()
        {
            _gameID     = GAMEID.MasterGems;
            GameName    = "MasterGems";
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
