using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class PantherQueenGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25pantherqueen";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return true;
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
                return "def_s=4,11,5,3,7,8,4,2,11,3,5,7,11,5,11&cfgs=1304&ver=2&reel_set_size=2&aw=4&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&fs_aw=m~1;m~2;m~3;m~4;m~5;m~6;m~7;m~8;m~9;m~10;t&gmb=0,0,0&base_aw=m~1;m~2;m~3;m~4;m~5;m~6;m~7;m~8;m~9;m~10;t&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&def_aw=4&pos=0,0,0,0,0&wilds=2~400,50,25,0,0~1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,50,25,0,0;250,40,20,0,0;200,30,15,0,0;150,25,10,0,0;100,25,10,0,0;75,20,5,0,0;50,15,5,0,0;40,15,5,0,0;30,15,4,0,0;25,10,3,0,0&rtp=94.00&reel_set0=12,8,6,12,9,6,7,9,2,9,4,12,9,5,6,7,11,12,7,11,12,5,11,12,5,9,10,12,4,8,11,3,12,10,4,8,12,4,10,7~6,12,9,10,11,5,10,8,2,8,10,7,11,4,7,8,12,7,6,12,9,11,10,3,11,8,9,10,11,3,10,11,4,8,11,6,8,10,11,5~3,11,8,12,10,8,7,6,2,6,7,9,11,3,9,8,5,10,11,12,10,5,12,8,5,6,9,10,12,9,8,11,7,10,3,9,10,12,9,4~12,8,9,6,12,3,5,11,2,11,5,10,8,9,10,12,11,10,4,12,6,10,9,5,3,9,12,4,8,7,10,6,5,10,3,6,7,12,6,4~4,8,7,6,8,4,10,12,2,12,7,4,12,6,4,9,8,10,9,11,7,9,11,10,9,12,5,8,12,9,10,11,8,12,11,7,3,11,8,3&reel_set1=12,8,6,12,10,6,7,10,2,11,4,12,10,5,6,12,9,10,7,12,6,10,11,12,6,9,10,12,9,8,10,3,12,10,4,8,12,4,10,7~6,6,6,11,11,11,9,9,9,10,10,10,11,11,11,9,9,9,5,5,5,8,8,8,9,9,9,2,2,2,8,8,8,9,9,9,11,11,11,5,5,5,7,7,7,8,8,8,9,9,9,7,7,7,11,11,11,12,12,12,10,10,10,11,11,11,12,12,12,9,9,9,11,11,11,8,8,8,9,9,9,10,10,10,11,11,11,3,3,3,9,9,9,11,11,11,4,4,4,8,8,8,11,11,11,5,5,5,8,8,8,9,9,9,11,11,11,5,5,5~6,6,6,11,11,11,9,9,9,10,10,10,11,11,11,9,9,9,5,5,5,8,8,8,9,9,9,2,2,2,8,8,8,9,9,9,11,11,11,5,5,5,7,7,7,8,8,8,9,9,9,7,7,7,11,11,11,12,12,12,10,10,10,11,11,11,12,12,12,9,9,9,11,11,11,8,8,8,9,9,9,10,10,10,11,11,11,3,3,3,9,9,9,11,11,11,4,4,4,8,8,8,11,11,11,5,5,5,8,8,8,9,9,9,11,11,11,5,5,5~6,6,6,11,11,11,9,9,9,10,10,10,11,11,11,9,9,9,5,5,5,8,8,8,9,9,9,2,2,2,8,8,8,9,9,9,11,11,11,5,5,5,7,7,7,8,8,8,9,9,9,7,7,7,11,11,11,12,12,12,10,10,10,11,11,11,12,12,12,9,9,9,11,11,11,8,8,8,9,9,9,10,10,10,11,11,11,3,3,3,9,9,9,11,11,11,4,4,4,8,8,8,11,11,11,5,5,5,8,8,8,9,9,9,11,11,11,5,5,5~12,8,7,6,8,11,10,8,2,12,7,4,12,6,5,9,12,10,9,11,7,9,11,10,9,12,5,7,12,9,10,11,8,12,11,7,10,11,8,3&awt=6rl";
            }
        }
	
	
        #endregion
        public PantherQueenGameLogic()
        {
            _gameID = GAMEID.PantherQueen;
            GameName = "PantherQueen";
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
