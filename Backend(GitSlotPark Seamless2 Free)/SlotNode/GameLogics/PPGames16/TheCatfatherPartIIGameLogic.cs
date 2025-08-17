using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TheCatfatherPartIIGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs30catz";
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
            get { return 30; }
        }
        protected override int ServerResLineCount
        {
            get { return 30; }
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
                return "def_s=3,7,4,1,8,9,8,5,6,7,4,6,2,3,9&cfgs=1223&reel1=11,8,1,8,11,8,11,8,11,3,11,3,11,3,11,4,10,4,10,4,10,1,10,9,10,9,10,9,10,9,10,9,5,9,5,9,5,9,6,7,6,7,2,7,6,8,7,8,6,8,11,8&ver=2&reel0=11,3,11,3,11,3,11,3,11,3,11,3,11,10,4,10,4,10,4,10,4,8,4,7,4,8,4,8,4,10,5,10,5,9,5,9,5,9,5,9,5,9,5,9,6,9,6,7,6,1,6,7,6,7,6,7,6,2,7,6,8,7,8,7,8&reel3=11,8,1,8,11,8,11,8,11,3,11,3,11,3,11,4,10,4,10,4,10,1,10,9,10,9,10,9,10,9,10,9,5,9,5,9,5,9,6,7,6,7,2,7,6,8,7,8,6,8,11,8&reel2=9,3,11,3,11,4,11,4,11,5,11,5,11,5,11,5,11,1,10,6,10,6,10,6,10,7,10,7,10,7,10,1,10,8,10,8,1,8,9,8,9,8,9,8,9,1,9,8,9,8,2,9,11&reel4=11,3,11,3,11,3,11,3,11,3,11,3,11,10,4,10,4,10,4,10,4,8,4,7,4,8,4,8,4,10,5,10,5,9,5,9,5,9,5,9,5,9,5,9,6,9,6,7,6,1,6,7,6,7,6,7,6,2,7,6,8,7,8,7,8&scatters=1~15,5,1,0,0~10,10,10,0,0~1,1,1,1,1&gmb=0,0,0&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&pos=0,0,0,0,0&wilds=2~5000,250,50,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,50,25,0,0;250,40,20,0,0;150,30,15,0,0;100,25,10,0,0;75,25,10,0,0;50,20,5,0,0;45,15,5,0,0;40,15,4,0,0;35,15,3,0,0&rtp=94.28";
            }
        }
	
	
        #endregion
        public TheCatfatherPartIIGameLogic()
        {
            _gameID = GAMEID.TheCatfatherPartII;
            GameName = "TheCatfatherPartII";
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
