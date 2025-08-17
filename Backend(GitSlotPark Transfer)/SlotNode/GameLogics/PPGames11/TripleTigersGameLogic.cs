using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TripleTigersGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1tigers";
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
            get { return 1; }
        }
        protected override int ServerResLineCount
        {
            get { return 1; }
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
                return "def_sb=6,7,8&def_sa=11,8,9&rt=d&def_s=11,11,11,3,4,5,11,11,11&c_paytable=12~any~8,9,10~10,0,0~2&cfgs=1&reel1=11,8,11,10,11,8,11,6,11,8,11,3,11,9,11,7,11,5,11,9,11,8,11,9,11,8,11,4,11,10,11,8,11,7,11,6&ver=2&reel0=10,11,5,11,8,11,6,8,7,11,3,11,8,11,4,11,9,11,9,11,10,6,11,9&reel2=4,11,8,11,10,11,9,11,8,11,6,11,8,11,10,11,9,11,3,11,8,11,10,11,7,11,8,11,10,11,5,11,4,11,7&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&sc=200.00,400.00,600.00,800.00,1000.00,2000.00,3000.00,4000.00,5000.00,7500.00,10000.00,15000.00,25000.00,50000.00,75000.00,100000.00&defc=1000.00&wilds=2~0,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;500,0,0;300,0,0;200,0,0;100,0,0;80,0,0;50,0,0;30,0,0;20,0,0;0,0,0";
            }
        }
	
	
        #endregion
        public TripleTigersGameLogic()
        {
            _gameID = GAMEID.TripleTigers;
            GameName = "TripleTigers";
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
