using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class HockeyLeagueWildMatchGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs9hockey";
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
                return "def_s=3,7,4,1,8,9,8,5,6,7,4,6,2,3,9&cfgs=1116&reel1=1,3,10,9,2,9,7,7,6,9,5,7,10,9,1,5,8,10,7,9,10,5,8,9,3,5,9,5,8,7,10,4,7,9,10,6,5,9,10,8,5,6,7,10,6,9,10,7,8,10,6,9,8,10,9,9,7,7,9,10,6,10,7,4,6,10,7,9,8,10,9,6,9,8,9,4,6,10,1,10,8,1,10,9,1,10,9&ver=2&reel0=1,3,7,9,2,5,7,6,10,6,9,10,7,5,1,8,10,7,9,8,9,10,5,7,9,3,5,5,8,9,5,8,9,7,5,9,6,10,9,4,10,8,5,6,7,10,8,5,6,10,6,7,10,9,6,10,8,10,9,6,7,6,9,10,8,4,10,6,9,6,10,8,4,6,10,6,9,7,6,9,8,10,4,10,6,9,6,8,10,4,7,10,6,9,7,1,10,8,1,10,9,1,10,9&reel3=3,10,9,2,9,7,8,10,6,5,9,10,8,1,10,8,10,5,7,9,10,8,9,3,9,5,6,9,10,6,9,4,5,7,9,7,9,8,10,8,5,9,7,6,5,7,6,7,9,7,6,10,7,10,6,7,10,9,7,10,6,10,4,10,6,10,7,10,7,9,7,10,7,10,6,4,9,1,8,10,1,10,9&reel2=3,7,9,2,10,7,5,5,8,7,9,10,9,1,7,9,7,5,10,9,6,10,5,3,9,7,9,5,9,8,5,4,10,7,5,5,6,10,6,10,5,7,10,6,9,5,7,10,7,9,9,8,7,9,6,10,7,9,7,8,10,4,10,8,10,6,10,9,7,4,9,7,9,7,6,10,7,10,9,7,4,9,7,7,10,6,10,9,1,7,10,1,7,9,1,10,9&reel4=3,7,9,2,7,10,5,10,5,7,8,5,9,1,5,10,6,5,10,7,5,9,7,5,3,9,10,8,9,5,7,8,10,6,9,5,9,7,10,8,9,5,7,10,4,5,7,7,10,6,7,10,9,7,9,6,7,9,6,7,10,7,10,8,10,9,7,9,6,9,4,7,9,6,10,9,6,7,10,6,10,8,7,9,10,7,6,9,4,7,9,6,10,9,6,1,8,10,1,10,9&scatters=1~20,5,2,0,0~25,15,10,0,0~3,3,3,1,1&gmb=0,0,0&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&pos=0,0,0,0,0&wilds=2~5000,250,50,5,1~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,100,50,0,0;100,40,20,0,0;70,15,10,0,0;45,15,10,0,0;20,10,7,0,0;20,8,5,0,0;15,7,3,0,0;15,4,2,0,0";
            }
        }
	
	
        #endregion
        public HockeyLeagueWildMatchGameLogic()
        {
            _gameID = GAMEID.HockeyLeagueWildMatch;
            GameName = "HockeyLeagueWildMatch";
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
