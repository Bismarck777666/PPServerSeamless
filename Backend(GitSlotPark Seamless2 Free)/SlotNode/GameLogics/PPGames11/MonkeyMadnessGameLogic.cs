using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MonkeyMadnessGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs9madmonkey";
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
                return "def_s=8,4,8,3,8,3,8,4,8&c_paytable=9~any~5,6,7~4,0,0~2&cfgs=2239&reel1=5,8,7,8,4,8,3,8,3,8,2,8,5,8,5,8,7,8,7,8,7,8,6,8,4,8,2,8,6,8&ver=2&reel0=5,8,6,8,4,8,7,8,2,8,7,8,3,8,5,8,7,8,6,8,3,8,6,8,4,8,6,8,5,8,5,8,7,8&def_sb=6,8,4&def_sa=5,6,8&reel2=6,8,4,8,7,8,5,8,6,8,7,8,6,8,3,8,7,8,5,8,7,8,5,8,4,8,5,8,2,8,7,8,6,8&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,800.00,1000.00,1500.00,3000.00,5000.00,8500.00,12000.00&defc=100.00&wilds=2~1000,0,0~1,9,3&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;50,0,0;30,0,0;20,0,0;12,0,0;8,0,0;0,0,0";
            }
        }
	
	
        #endregion
        public MonkeyMadnessGameLogic()
        {
            _gameID = GAMEID.MonkeyMadness;
            GameName = "MonkeyMadness";
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
