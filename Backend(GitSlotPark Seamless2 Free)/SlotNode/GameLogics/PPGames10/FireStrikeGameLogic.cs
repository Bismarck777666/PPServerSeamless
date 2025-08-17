using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FireStrikeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10firestrike";
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
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "def_s=8,11,7,9,14,8,10,4,2,14,4,10,7,2,14&cfgs=2485&reel1=9,10,6,2,2,2,12,7,9,10,8,11,3,7,13,4,9,11,5,6,13,7,10,4,6,14,14,14,6,8,9,10,11,6,9,7,10,8,5,11,12,7,9,6,10,5,11,8,3&ver=2&reel0=11,8,13,2,2,2,7,11,9,13,7,12,10,7,13,9,8,12,11,10,7,12,4,13,6,10,12,11,14,14,14,13,6,12,8,11,3,6,5,9,7,12,4,13,6&def_sb=13,7,4,10,8&def_sa=14,14,7,14,8&reel3=5,12,8,2,2,2,10,8,9,12,5,11,6,10,3,12,4,9,7,11,3,13,14,14,14,7,10,5,11,12,4,6,10,5,13,4,8,11,12,10,5,11,13,6,12,10,6,13,9,8,12,6,11,10,5,8,13&reel2=8,12,11,2,2,2,6,13,8,10,13,6,14,14,14,8,12,9,6,10,7,13,3,6,9,8,4,10,9,7,13,12,10,9,5,13,6,7,12,9,8,10,13,9,12,14,14,14,6,13&bonusInit=[{bgid:0,bgt:33,bg_i:\"1,2,8,25,50,150,500,1000,2500,5000,25000\",bg_i_mask:\"wp,wp,wp,wp,wp,wp,wp,wp,wp,wp,wp\"}]&reel4=11,8,12,2,2,2,13,4,12,8,10,13,9,7,11,10,6,13,3,9,8,12,5,13,7,11,6,10,4,11,8,10,12,7,11,8,14,14,14,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&wilds=2~2000,500,250,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,300,150,0,0;500,200,100,0,0;300,150,75,0,0;250,100,30,0,0;200,80,25,0,0;150,60,20,0,0;125,50,15,0,0;100,40,10,0,0;100,40,10,0,0;50,20,5,0,0;50,20,5,0,0;0,0,0,0,0";
            }
        }
	
	
        #endregion
        public FireStrikeGameLogic()
        {
            _gameID = GAMEID.FireStrike;
            GameName = "FireStrike";
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
