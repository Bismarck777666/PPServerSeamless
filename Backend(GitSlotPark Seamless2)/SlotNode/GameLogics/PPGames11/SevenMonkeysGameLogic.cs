using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class SevenMonkeysGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs7monkeys";
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
            get { return 7; }
        }
        protected override int ServerResLineCount
        {
            get { return 7; }
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
                return "def_s=2,3,4,3,2,2,3,4,3,2,2,3,4,3,2&cfgs=2172&reel1=5,5,5,5,5,6,6,6,6,6,6,6,6,5,2,2,2,2,2,2,2,2,6,6,2,6,7,7,7,7,7,7,7,7,6,4,4,4,4,4,4,7,7,1,3,3,3,3,3,4,4,5,6,7,5,4,4,7,7,6,6,7,6,7,6,5,3,7,5,7,3,4,7&ver=2&reel0=6,6,6,6,6,6,6,6,6,1,6,5,5,5,5,5,6,5,5,6,3,3,3,3,3,6,4,4,4,4,4,7,7,7,7,7,7,7,7,3,7,2,2,2,2,2,2,2,2,6,6,2,7,2,6,7,4,7,3,5,4,6,7,4,7,3,7,5,5,7,4,7,4&def_sb=7,7,7,7,7&def_sa=6,6,6,6,6&reel3=7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,4,4,4,4,4,5,5,5,5,5,7,7,7,1,5,3,3,3,3,5,4,3,5,4,4,6,6,6,6,6,6,6,6,5,7,6,6,7,3,7,5,6,6,4,7,2,6,4,7,6,4,1,6,6,7,6,3&reel2=7,7,7,7,7,7,7,7,3,3,3,3,6,6,6,6,6,6,6,6,3,6,4,4,4,4,4,7,5,5,5,5,5,7,4,5,2,2,2,2,2,2,2,2,7,2,6,3,1,6,7,7,7,7,6,5,2,4,3,6,6,5,4,4,7,6,5,5,4,3,6,1,7,7,6&reel4=7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,6,6,6,6,6,6,6,6,4,4,4,4,4,1,2,6,4,4,5,5,5,5,5,6,7,5,7,6,6,1,4,5,7,7,3,3,3,3,5,6,5,7,3,7,4,7,4,6,4,7,7,6,3,5,3,7,6,5,6&scatters=1~0,0,0,0,0~150,100,50,0,0~3,2,1,1,1&gmb=0,0,0&rt=d&sc=30.00,60.00,90.00,120.00,150.00,300.00,450.00,600.00,750.00,1000.00,1500.00,2000.00,3500.00,7000.00,10500.00,15000.00&defc=150.00&wilds=2~1500,400,50,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;1500,400,50,0,0;150,50,20,0,0;50,15,5,0,0;50,15,5,0,0;15,5,3,0,0;15,5,3,0,0";
            }
        }
	
	
        #endregion
        public SevenMonkeysGameLogic()
        {
            _gameID = GAMEID.SevenMonkeys;
            GameName = "SevenMonkeys";
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
