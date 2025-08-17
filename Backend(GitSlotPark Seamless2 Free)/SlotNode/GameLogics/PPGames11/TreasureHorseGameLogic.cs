using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TreasureHorseGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs18mashang";
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
            get { return 18; }
        }
        protected override int ServerResLineCount
        {
            get { return 18; }
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
                return "def_s=3,6,4,4,3,6,5,4,3&cfgs=2257&ver=2&reel_set_size=2&def_sb=4,6,5&def_sa=8,9,6&scatters=1~0,0,0~8,0,0~1,1,1&fs_aw=m~2;m~3;m~4;m~5;m~6&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,400.00,500.00,750.00,1500.00,2500.00,4250.00,6000.00&defc=50.00&wilds=2~1000,0,0~1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0;0,0,0;0,0,0;400,0,0;150,0,0;75,0,0;40,0,0;20,0,0;10,0,0;5,0,0&reel_set0=9,6,3,9,1,8,5,4,8,2,7,9,8,6,7,8,5,1,2,5,8,4,3,8~7,1,8,3,9,5,6,7,2,8,8,1,3,4,5,2,6,8,1~5,3,9,2,6,8,1,7,9,3,2,9,4,6,8,1,5,7,2&reel_set1=5,7,7,7,4,5,5,5,6,3,3,3,1,9,8,8,8,4,4,4,7,6,6,6,1,8,5,3,2,2,2,9,9,9,8,3,8~4,1,9,9,9,6,6,6,5,2,2,2,8,6,7,4,9,3,1,6,5,5,5,8,8,8,2,7,7,7,3,9,7,4,4,4,3,3,3~1,8,4,2,9,6,7,3,9,9,9,5,3,3,3,7,7,7,8,8,8,2,2,2,8,6,6,6,9,1,8,6,5,5,5,9,4,7&awt=6rl";
            }
        }
	
	
        #endregion
        public TreasureHorseGameLogic()
        {
            _gameID = GAMEID.TreasureHorse;
            GameName = "TreasureHorse";
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
