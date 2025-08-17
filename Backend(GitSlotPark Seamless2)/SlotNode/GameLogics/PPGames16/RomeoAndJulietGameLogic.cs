using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class RomeoAndJulietGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25romeoandjuliet";
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
                return "def_s=12,7,11,10,8,9,8,5,6,7,8,6,12,11,9&cfgs=1188&reel1=14,5,11,13,6,12,7,11,8,12,6,13,14,5,12,13,5,10,9,12,7,13,8,11,9,13,7,14,8,12,9,14,13,10,9,11,14,10,11,6,12,10&ver=2&reel0=14,5,11,13,6,12,7,11,8,12,6,13,14,5,12,11,2,10,9,12,13,10,14,11,9,13,7,14,8,12,9,14,13,10,12,11,14,10,13,8,14,12,11,10,13,12,10,7,14,9,11,7,10,9,13,8,14,8,10,13&reel3=14,5,11,13,6,12,7,11,8,12,6,13,14,5,12,14,5,10,9,12,7,13,8,11,9,13,7,14,8,12,9,14,6,10,9,11,14,10,13,14,11,10&reel2=14,5,11,13,6,12,7,11,8,12,6,13,11,5,12,11,6,10,9,12,7,10,8,14,9,13,7,14,8,12,9,14,7,10,9,11,14,10,13,8,10,13&reel4=14,5,11,13,6,12,7,11,8,12,6,13,14,5,12,11,3,10,9,12,7,10,8,11,9,13,7,14,8,12,9,14,13,10,9,11,14,10,13,8,14,12,11,10,13,5,12,6,14,13,11,12,10,9,13,8,14,11,10,6&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&pos=0,0,0,0,0&wilds=2~0,0,0,0,0~1,1,1,1,1;3~0,0,0,0,0~1,1,1,1,1;4~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,350,50,0,0;750,250,45,0,0;300,100,30,0,0;200,75,25,0,0;150,60,20,0,0;100,25,10,0,0;100,25,10,0,0;50,15,5,0,0;50,10,5,0,0;50,10,5,0,0";
            }
        }
	
	
        #endregion
        public RomeoAndJulietGameLogic()
        {
            _gameID = GAMEID.RomeoAndJuliet;
            GameName = "RomeoAndJuliet";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
	
    }
}
