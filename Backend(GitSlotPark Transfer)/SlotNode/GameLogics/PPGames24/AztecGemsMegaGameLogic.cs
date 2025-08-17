using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class AztecGemsMegaGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysaztec";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 5; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
        }
        protected override int ROWS
        {
            get
            {
                return 8;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,3,3,4,5,9,6,3,3,4,5,5,7,3,11,11,8,11,11,11,11,11,11,11&cfgs=1&ver=3&def_sb=4,8,3&reel_set_size=6&def_sa=6,5,3&scatters=1~0,0,0~0,0,0~1,1,1&rt=d&gameInfo={rtps:{regular:\"96.58\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"30303030\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&sc=20.00,40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~15,0,0~1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0;0,0,0;0,0,0;10,0,0;8,0,0;5,0,0;4,0,0;3,0,0;2,0,0;1,0,0;0,0,0;0,0,0&reel_set0=8,6,4,3,8,5,6,3,7,9,9,6,6,6,7,9,6,4,7,7,9,7,3,6,8,5,9,7,7,7,3,6,9,7,9,7,6,8,6,7,3,7,6,9,9,9,3,9,6,9,7,8,7,6,5,8,8,6,9~8,6,9,7,4,9,3,5,8,5,9,6,5,5,5,9,6,9,8,8,6,9,5,4,7,6,5,6,6,6,7,5,8,8,4,6,8,8,9,4,9,8,9,8,8,8,3,8,9,9,4,4,5,5,6,9,9,5,6,9,9,9,9,6,4,8,4,8,5,8,7,6,9,5,5,4,6~3,8,8,6,8,7,4,8,3,3,3,7,5,4,7,9,4,8,3,7,5,5,5,3,4,9,6,7,3,8,7,9,7,7,7,5,4,9,9,8,4,3,7,5,8,8,8,5,9,5,8,4,5,9,4,8,9,9,9,7,3,7,8,8,7,9,5,3,9&reel_set2=3,4,5,6,6,7,9,5,7,5,4,9,3,8,7,7,8,7,5,8,4,8,8,7,7,5,9,9,7,7,8,7,9,6,7,5,4,7,7,8,6,6,9,9,6,8,9,6,9,6,9,3,5,5,9,6,4,5,9,8,7,6,4,8,8,9,8,8,5~6,9,4,9,8,6,7,8,7,8,6,6,7,8,9,4,6,5,8,8,8,4,8,7,8,7,5,8,9,6,8,5,6,7,9,6,7,7,9,9,6,9,6,3,3,4,3,9,3,8,4,7,5,7,8,9,9,7,2,8~3,4,9,4,9,7,6,3,6,7,7,7,8,2,7,9,7,8,4,5,8,8,5,8,8,8,7,9,9,5,6,8,8,9,6,9,5,7,9,9,9,7,9,4,7,7,8,9,4,8,5,3,7,9,6,8,6,7,3,5,7,8&reel_set1=4,4,9,8,7,7,4,5,8,7,6,5,7,9,8,8,8,3,7,5,5,6,9,9,7,9,8,7,8,9,3,6,9,9,9,8,4,3,7,9,9,4,8,8,9,5,8,7,2,8,9,9~8,6,6,9,6,7,2,7,8,8,9,4,6,7,6,6,6,9,7,3,8,9,9,3,7,7,2,9,5,6,9,9,6,8,8,8,9,8,9,9,6,8,6,3,4,9,6,5,9,7,8,9,9,9,4,7,6,5,7,9,9,3,8,4,8,9,7,8,8,7,4~9,8,5,8,3,7,6,9,6,7,9,4,9,6,5,5,7,9,8,8,7,8,8,8,7,8,6,3,9,4,8,2,6,9,5,3,7,5,4,8,9,5,4,4,3,9,9,9,7,6,9,6,7,8,4,5,9,9,7,9,5,6,8,7,9,2,4,8,7,6,4,9&reel_set4=4,9,9,6,5,5,5,9,6,5,8,5,8,8,8,9,6,4,4,9,9,9,8,5,9,6,4,8,8~6,9,6,7,3,4,7,7,7,9,6,7,4,9,6,7,9,9,9,3,7,3,7,9,9,4,3~8,3,5,3,7,7,3,8,7,7,7,8,5,5,8,7,3,8,5,8,8,8,7,5,7,8,7,3,3,7,5,3&reel_set3=5,8,8,3,8,5,7,8,7,7,7,8,7,3,8,3,5,7,8,8,8,3,5,8,3,7,5,3,8,3,7,5~8,9,4,9,5,5,5,4,8,5,9,6,6,8,8,8,4,9,9,4,6,8,9,9,9,6,9,9,5,8,5,5~7,9,6,3,7,4,7,7,7,4,9,9,6,3,7,3,4,9,9,9,6,6,9,9,7,7,9,4,3&reel_set5=2,2,2,2,2,2,2,2,2,2,2,2,2~2,2,2,2,2,2,2,2,2,2,2,2,2~2,2,2,2,2,2,2,2,2,2,2,2";
            }
        }
	
	
        #endregion
        public AztecGemsMegaGameLogic()
        {
            _gameID     = GAMEID.AztecGemsMega;
            GameName    = "AztecGemsMega";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "3";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            if (dicParams.ContainsKey("wlc_v"))
            {
                string[] strParts = dicParams["wlc_v"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strValues = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    strValues[1] = convertWinByBet(strValues[1], currentBet);
                    strParts[i] = string.Join("~", strValues);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);

            }

            base.convertWinsByBet(dicParams, currentBet);
        }
	
    }
}
