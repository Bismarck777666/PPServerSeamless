using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FruitRainbowGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40frrainbow";
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
            get { return 40; }
        }
        protected override int ServerResLineCount
        {
            get { return 40; }
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
                return "def_s=8,4,10,5,9,8,4,10,5,9,8,4,10,5,9,8,4,10,5,9&cfgs=2916&reel1=7,7,7,11,11,11,11,11,1,10,10,10,10,8,8,8,4,4,4,4,9,9,9,6,6,6,3,3,5,5&ver=2&reel0=3,6,6,6,8,8,8,8,1,4,4,9,9,9,9,11,11,11,5,5,5,7,7,7,10,10,10,10&def_sb=5,7,5,3,5&def_sa=6,5,7,3,8&reel3=6,6,6,3,3,3,1,8,8,8,8,9,9,9,9,7,7,7,4,4,4,10,10,10,11,11,11,11,5,5,5,5&reel2=5,5,5,9,9,9,1,11,11,11,4,4,4,10,10,10,10,8,8,8,7,7,7,6,6,6,3&reel4=5,5,5,1,7,7,7,6,6,6,9,9,9,9,11,11,11,11,4,4,4,4,10,10,10,10,3,3,3,7,7,7,8,8,8&scatters=1~500,125,5,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=5.00,10.00,15.00,20.00,25.00,50.00,75.00,100.00,125.00,190.00,250.00,375.00,625.00,1250.00,1875.00,2500.00&defc=25.00&wilds=2~2000,1000,100,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,500,90,0,0;600,200,50,0,0;500,150,40,0,0;400,100,30,0,0;300,90,20,0,0;250,80,15,0,0;200,60,10,0,0;140,50,10,0,0;100,50,5,0,0";
            }
        }
	
	
        #endregion
        public FruitRainbowGameLogic()
        {
            _gameID = GAMEID.FruitRainbow;
            GameName = "FruitRainbow";
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
