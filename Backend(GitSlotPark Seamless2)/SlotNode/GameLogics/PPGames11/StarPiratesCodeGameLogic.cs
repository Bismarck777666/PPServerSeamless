using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class StarPiratesCodeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10starpirate";
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
                return "def_s=7,5,4,3,6,7,9,4,3,8,6,6,4,9,8&cfgs=4648&ver=2&def_sb=3,8,4,7,3&reel_set_size=3&def_sa=7,4,6,3,3&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2765296\",max_rnd_win:\"2500\"}}&wl_i=tbm~2500&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;150,60,20,0,0;80,30,12,0,0;30,12,6,0,0;24,10,4,0,0;20,4,2,0,0;12,2,1,0,0;12,2,1,0,0&reel_set0=3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5,7,7,7,6,6,6,5,5,5~9,9,9,8,8,3,3,7,7,7,6,6,3,3,3,8,8,8,4,4,4,6,6,6,5,5,5,6,6,6,2~9,9,8,8,8,9,9,4,4,4,9,9,7,7,9,9,5,5,9,3,3,9,9,3,3,3,9,9,9,7,7,7,5,5,5,6,6,6,7,7,7,6,6,6,2~9,9,9,8,8,9,9,9,3,3,3,8,8,3,3,3,7,7,7,6,6,8,8,9,9,9,8,8,6,6,3,3,3,8,8,8,4,4,4,6,6,6,5,5,5,6,6,6,2~9,9,9,8,8,8,3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5&accInit=[{id:1,mask:\"cp;mp\"},{id:2,mask:\"cp;mp\"},{id:3,mask:\"cp;mp\"},{id:6,mask:\"cp;mp\"},{id:7,mask:\"cp;mp\"},{id:8,mask:\"cp;mp\"},{id:11,mask:\"cp;mp\"},{id:12,mask:\"cp;mp\"},{id:13,mask:\"cp;mp\"}]&reel_set2=9,9,9,8,8,8,3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5,2,2,2,9,9,9~9,9,9,8,8,8,3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5,2,2,2,9,9,9~9,9,9,8,8,8,3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5,2,2,2,9,9,9~9,9,9,8,8,8,3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5,2,2,2,9,9,9~9,9,9,8,8,8,3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5,2,2,2,9,9,9&reel_set1=9,9,9,8,8,8,3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5~9,9,9,7,7,7,5,5,5,3,3,3~8,8,8,6,6,6,4,4,4~9,9,9,7,7,7,5,5,5,3,3,3~9,9,9,8,8,8,3,3,3,9,9,9,8,8,8,4,4,4,7,7,7,6,6,6,5,5,5";
            }
        }
	
	
        #endregion
        public StarPiratesCodeGameLogic()
        {
            _gameID = GAMEID.StarPiratesCode;
            GameName = "StarPiratesCode";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
	
    }
}
