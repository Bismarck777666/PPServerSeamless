using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class HotSafariGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25safari";
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
                return "def_s=4,11,5,2,7,8,4,1,11,3,5,7,11,5,11&cfgs=2213&reel1=12,6,9,10,10,2,5,11,3,9,8,8,11,7,6,8,7,11,4,10&ver=2&reel0=12,10,4,4,11,6,12,1,9,6,3,7,8,5,5,10,11,9,8,2&def_sb=1,8,9,12,9&def_sa=12,10,4,4,11&reel3=11,7,11,4,5,9,10,10,4,8,3,2,12,3,6,12,8,12,10&reel2=2,1,11,3,10,3,9,10,7,6,4,5,12,7,8,9,9,5,1,6&reel4=1,8,9,12,9,8,1,2,6,7,3,4,7,5,10,11,12&aw=6&scatters=1~1,1,1,0,0~10,10,10,0,0~1,1,1,1,1&fs_aw=m~1;m~2;m~3;m~4;m~5;m~6;m~7;m~8;m~9;m~10;t&gmb=0,0,0&rt=d&base_aw=m~1;m~2;m~3;m~4;m~5;m~6;m~7;m~8;m~9;m~10;t&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&def_aw=6&wilds=2~400,50,25,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,50,25,0,0;250,40,20,0,0;200,30,15,0,0;150,25,10,0,0;100,25,10,0,0;75,20,5,0,0;50,15,5,0,0;40,15,4,0,0;30,15,3,0,0;25,10,3,0,0&awt=6rl";
            }
        }
	
	
        #endregion
        public HotSafariGameLogic()
        {
            _gameID = GAMEID.HotSafari;
            GameName = "HotSafari";
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
