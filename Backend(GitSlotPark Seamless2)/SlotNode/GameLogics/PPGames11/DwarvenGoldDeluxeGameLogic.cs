using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DwarvenGoldDeluxeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25dwarves_new";
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
            get { return 25; }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
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
                return "def_s=12,7,11,10,8,9,8,5,6,7,8,6,12,11,9&cfgs=2200&reel1=10,7,6,4,9,5,12,1,8,3,3,3,3,11,2&ver=2&reel0=10,4,6,12,3,3,3,3,3,9,11,8,5,7&def_sb=3,3,3,7,8&def_sa=10,4,6,12,3&reel3=9,10,6,5,1,3,3,3,12,11,3,8,7,2,4&reel2=5,4,10,3,3,3,8,3,6,11,12,7,2,9,1&reel4=3,3,3,7,8,11,3,6,2,9,5,10,12,4&scatters=1~0,0,0,0,0~7,7,7,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,200,75,0,0;400,120,25,0,0;300,80,20,0,0;300,80,20,0,0;300,80,20,0,0;150,50,10,0,0;100,25,5,0,0;100,25,5,0,0;50,10,2,0,0;50,10,2,0,0";
            }
        }
	
	
        #endregion
        public DwarvenGoldDeluxeGameLogic()
        {
            _gameID = GAMEID.DwarvenGoldDeluxe;
            GameName = "DwarvenGoldDeluxe";
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
