using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class JasmineDreamsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20mvwild";
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
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
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
                return "def_s=5,10,4,6,9,3,9,6,3,10,5,8,8,4,7&cfgs=7242&ver=3&def_sb=6,7,5,3,7&reel_set_size=3&def_sa=6,8,6,3,8&scatters=1~20,5,2,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"3571429\",max_rnd_win:\"2500\"}}&wl_i=tbm~2500&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;600,100,40,5,0;400,60,30,0,0;200,40,20,0,0;200,40,20,0,0;100,20,10,0,0;100,20,10,0,0;40,10,5,0,0;40,10,5,0,0&reel_set0=9,5,3,9,10,3,7,6,5,7,8,3,3,3,1,4,10,3,6,7,10,9,5,6,9,10,9,7,9~1,9,4,5,4,8,9,8,6,10,2,2,2,3,10,2,7,10,6,4,10,4,10,8,7~7,9,4,8,4,9,5,9,10,7,10,8,5,9,6,4,2,3,8,2,2,2,1,10,1,8,4,6,7,9,4,10,8,2,8,2,8,6,8,9~8,6,3,10,6,7,4,2,6,9,10,7,4,9,5,7,6,10,6,9,5,1,2,2,2,10,2,9,10,6,7,8,7,10,7,10,7,5,8,9,10,1,10,9,5,7,10,7,2,9,10,9~6,8,3,8,4,8,5,9,5,6,4,10,3,3,3,7,3,9,8,7,10,7,8,6,1,9,8,10,5&reel_set2=9,5,3,9,10,3,7,6,5,7,8,3,3,3,1,4,10,3,6,7,10,9,5,6,9,10,9,7,9~1,9,4,5,4,8,9,8,6,10,2,2,2,3,10,2,7,10,6,4,10,4,10,8,7~7,9,4,8,4,9,5,9,10,7,10,8,5,9,6,4,2,3,8,2,2,2,1,10,1,8,4,6,7,9,4,10,8,2,8,2,8,6,8,9~8,6,3,10,6,7,4,2,6,9,10,7,4,9,5,7,6,10,6,9,5,1,2,2,2,10,2,9,10,6,7,8,7,10,7,10,7,5,8,9,10,1,10,9,5,7,10,7,2,9,10,9~6,8,3,8,4,8,5,9,5,6,4,10,3,3,3,7,3,9,8,7,10,7,8,6,1,9,8,10,5&reel_set1=5,7,9,3,8,9,8,10,8,7,9,7,5,3,6,3,3,3,4,7,5,7,6,7,4,6,3,9,6,8,4,10,3,10,7,6~7,8,6,9,7,3,8,5,2,2,2,9,2,8,5,6,8,7,10,4,3,3,3,9,6,9,10,2,4,3,2,8,2~3,6,3,6,3,6,8,7,8,4,6,9,4,8,10,5,10,6,6,4,10,4,4,3,2,2,2,9,3,5,6,9,6,8,4,8,3,3,9,6,4,10,3,9,6,8,9,10,5,10,8,7,3,9,7,3,3,3,5,10,6,10,5,6,8,5,9,7,8,10,9,5,6,4,7,8,3,2,8,10,6,4,10,7~4,8,5,7,6,9,10,5,10,5,6,3,2,2,2,3,2,6,3,9,5,10,6,7,8,10,5,9,4,4,3,3,3,7,3,3,10,7,8,9,7,6,4,2,7,10,3~8,6,6,7,8,7,10,8,9,10,8,6,8,6,6,10,3,4,8,6,8,3,7,3,3,3,7,5,5,3,3,8,6,4,3,10,4,10,5,3,10,4,9,8,7,4,7,10,6,7,8,6";
            }
        }
	
	
        #endregion
        public JasmineDreamsGameLogic()
        {
            _gameID = GAMEID.JasmineDreams;
            GameName = "JasmineDreams";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

            
        }
	
    }
}
