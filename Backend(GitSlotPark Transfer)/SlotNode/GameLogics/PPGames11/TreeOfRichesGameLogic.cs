using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TreeOfRichesGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1fortunetree";
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
                return "def_s=5,6,3,6,2,6,3,6,4&c_paytable=7~any~3,4,5~5,0,0~2&cfgs=2490&reel1=6,5,6,3,6,5,6,2,6,5,6,4,6,5,6,5,6,3,6,5,6,5,6,3,6,5,6,2,6,5,6,5,6,3,6,5,6,5,6,3,6,5,6,5,6,3,6,5,6,5,6,3,6,5,6,5,6,2,6,5,6,5,6,3,6,5,6,5,6,3,6,3&ver=2&reel0=6,5,6,5,6,4,6,2,6,5,6,5,6,4,6,5,6,5,6,4,6,5,6,5,6,4,6,5,6,5,6,5,6,5,6,5,6,5,6,2,6,5,6,5,6,4,6,5,6,4,6,5,6,5,6,4,6,5,6,4,6,3,6,4,6,5,6,4,6,5,6,4&def_sb=6,5,6&def_sa=6,5,6&reel2=6,3,6,4,6,3,6,5,6,4,6,4,6,2,6,4,6,3,6,4,6,4,6,3,6,4,6,4,6,4,6,3,6,4,6,4,6,4,6,2,6,3,6,4,6,4,6,4,6,3,6,4,6,3,6,4,6,4,6,3,6,4,6,4,6,2,6,3,6,4,6,4&scatters=1~0,0,0~0,0,0~0,0,0&gmb=0,0,0&rt=d&sc=200.00,400.00,600.00,800.00,1000.00,2000.00,3000.00,4000.00,5000.00,7500.00,10000.00,15000.00,25000.00,50000.00,75000.00,100000.00&defc=1000.00&wilds=2~288,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;288,0,0;88,0,0;58,0,0;28,0,0;0,0,0";
            }
        }
	
	
        #endregion
        public TreeOfRichesGameLogic()
        {
            _gameID = GAMEID.TreeOfRiches;
            GameName = "TreeOfRiches";
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
