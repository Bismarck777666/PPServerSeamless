using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class LuckyDragonsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs50chinesecharms";
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
                return "def_s=12,7,3,11,3,7,9,3,5,3,5,1,4,6,3,6,8,12,10,3&cfgs=2226&reel1=9,2,5,10,3,3,3,3,3,3,3,12,6,8,7,1,4,11&ver=2&reel0=6,3,3,3,3,3,3,3,7,7,11,3,10,4,5,9,8,3,12&def_sb=6,10,9,3,3&def_sa=12,10,3,3,3&reel3=12,3,3,3,3,7,4,1,5,2,3,10,9,11,6,3,8&reel2=4,12,11,1,10,9,3,3,3,3,3,3,3,6,5,7,3,8,2&reel4=10,12,9,7,3,3,3,3,4,8,5,3,3,11,6&scatters=1~2,2,2,0,0~6,6,6,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=4.00,8.00,12.00,16.00,20.00,40.00,60.00,80.00,100.00,150.00,200.00,300.00,500.00,1000.00,1500.00,2000.00&defc=20.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;800,200,50,10,0;400,120,25,0,0;300,80,20,0,0;300,80,20,0,0;300,80,20,0,0;150,50,10,0,0;100,25,5,0,0;100,25,5,0,0;50,10,2,0,0;50,10,2,0,0";
            }
        }
	
	
        #endregion
        public LuckyDragonsGameLogic()
        {
            _gameID = GAMEID.LuckyDragons;
            GameName = "LuckyDragons";
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
