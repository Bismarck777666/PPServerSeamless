using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol.Utils;

namespace SlotGamesNode.GameLogics
{
    class DinoDropGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5tdragresk";
            }
        }
        protected override bool SupportReplay
        {
            get { return false; }
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
                return "def_s=4,5,6,4,5,6,4,5,6&cfgs=1&reel1=3,3,3,9,9,9,9,9,9,8,8,8,8,8,8,10,10,10,10,10,10,9,9,9,7,7,7,7,7,7,5,5,5,5,5,5,6,6,6,6,6,6,4,4,4,4,4,4,10,10,10,10,10,10,8,8,8,5,5,5&ver=2&reel0=3,3,3,4,4,4,4,4,4,8,8,8,8,8,8,7,7,7,7,7,7,9,9,9,10,10,10,10,10,10,5,5,5,5,5,5,6,6,6,6,6,6,9,9,9,9,9,9,8,8,8,10,10,10,10,10,10,4,4,4&def_sb=4,5,6&def_sa=4,5,6&reel2=3,3,3,10,10,10,10,10,10,9,9,9,6,6,6,6,6,6,10,10,10,10,8,8,8,8,8,8,7,7,7,7,7,7,5,5,5,5,5,5,4,4,4,4,4,4,8,8,8,9,9,9,9,9,9,6,6,6&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&gameInfo={rtps:{regular:\"96.47\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"67114094\",max_rnd_win:\"1240\"}}&wl_i=tbm~1240&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~1000,0,0~1,1,1;11~1000,0,0~1,1,1&bonuses=0&fsbonus=&ntp=0.00&paytable=0,0,0;0,0,0;0,0,0;300,0,0;200,0,0;100,0,0;50,0,0;25,0,0;15,0,0;10,0,0;5,0,0;0,0,0";
            }
        }
	
	
        #endregion
        public DinoDropGameLogic()
        {
            _gameID = GAMEID.DinoDrop;
            GameName = "DinoDrop";
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
