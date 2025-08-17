using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WildGladiatorsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25gladiator";
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
                return "def_s=4,11,5,2,7,8,4,1,11,3,5,7,11,5,11&cfgs=2202&reel1=12,8,9,4,5,11,2,7,11,6,11,6,8,7,10,8,3,9,12,10,10&ver=2&reel0=11,5,2,9,10,9,6,12,5,1,8,8,12,3,6,12,11,4,7&def_sb=6,7,3,12,9&def_sa=11,5,2,9,10&reel3=4,11,12,5,6,8,10,3,3,9,10,8,2,5,10,12,12,4,7,11&reel2=8,5,9,6,3,11,7,12,11,1,9,5,8,1,2,4,10,7,6,3&reel4=6,7,3,12,9,4,1,11,8,10,8,9,12,2,7,8,5&aw=6&scatters=1~1,1,1,0,0~10,10,10,0,0~1,1,1,1,1&fs_aw=m~1;m~2;m~3;m~4;m~5;m~6;m~7;m~8;m~9;m~10;t&gmb=0,0,0&rt=d&base_aw=m~1;m~2;m~3;m~4;m~5;m~6;m~7;m~8;m~9;m~10;t&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&def_aw=6&wilds=2~400,50,25,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,50,25,0,0;250,40,20,0,0;200,30,15,0,0;150,25,10,0,0;100,25,10,0,0;75,20,5,0,0;50,15,5,0,0;40,15,4,0,0;30,15,3,0,0;25,10,3,0,0&awt=6rl";
            }
        }
	
	
        #endregion
        public WildGladiatorsGameLogic()
        {
            _gameID = GAMEID.WildGladiators;
            GameName = "WildGladiators";
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
