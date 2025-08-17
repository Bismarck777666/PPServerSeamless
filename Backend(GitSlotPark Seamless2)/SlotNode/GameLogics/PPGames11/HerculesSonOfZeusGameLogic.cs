using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class HerculesSonOfZeusGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs50hercules";
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
            get { return 50; }
        }
        protected override int ServerResLineCount
        {
            get { return 50; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=12,7,3,11,3,7,9,3,5,3,5,1,4,6,3,6,8,12,10,3&cfgs=2228&reel1=8,3,3,3,3,3,3,6,4,3,1,11,10,7,3,5,2,12,9&ver=2&reel0=3,3,3,3,3,3,3,6,3,7,10,7,12,4,3,11,8,5,9&def_sb=7,11,4,6,3&def_sa=12,5,7,6,7&reel3=4,3,3,3,3,7,9,3,10,5,1,6,12,2,11,8,3&reel2=3,3,3,3,3,3,3,11,1,10,5,4,12,3,7,8,2,6,9&reel4=3,3,3,3,8,12,9,5,3,11,7,3,6,4,3,10&scatters=1~2,2,2,0,0~6,6,6,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=4.00,8.00,12.00,16.00,20.00,40.00,60.00,80.00,100.00,150.00,200.00,300.00,500.00,1000.00,1500.00,2000.00&defc=20.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;800,200,50,10,0;400,120,25,0,0;300,80,20,0,0;300,80,20,0,0;300,80,20,0,0;150,50,10,0,0;100,25,5,0,0;100,25,5,0,0;50,10,2,0,0;50,10,2,0,0";
            }
        }
	
	
        #endregion
        public HerculesSonOfZeusGameLogic()
        {
            _gameID = GAMEID.HerculesSonOfZeus;
            GameName = "HerculesSonOfZeus";
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
