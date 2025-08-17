using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class LittleGemGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5littlegem";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=9,3,11,6,6,11,5,9,11&cfgs=5460&ver=2&reel_set_size=2&def_sb=3,4,7&def_sa=8,7,5&bonusInit=[{bgid:0,bgt:48,mo_s:\"13,13,13,13,13,13,13,13,13,14,14,14,14,14,14,14,15,15,15,15,15\",mo_v:\"5,10,15,20,25,30,35,40,45,50,55,60,70,80,90,100,500,1000,1500,2000,2500\"}]&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&gameInfo={}&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~250,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;250,0,0;150,0,0;100,0,0;80,0,0;80,0,0;20,0,0;20,0,0;20,0,0;20,0,0;0,0,0;0,0,0;0,0,0;0,0,0&reel_set0=9,9,11,7,11,11,4,9,9,11,9,11,11,11,9,11,11,11,9,11,3,9,11,9,11,11,11,11,11,2,7,11,8,10,9,9,9,9,9,10,11,11,9,7,9,10,11,9,8,11,11,11,11,11,10,9,11,10,10,10,10,10,10,9,9,7,5,11,9,9,11,9,11,6,5,11,4,8,9,9,9,3,9,9,11,9,11,9,11,9,9,11,9,9,9,9,9,9,9,9,5,4,10,11,9,11,9,10,8,11,11,9,5,9,4,11,11,9,11,11,9,8,7,6,11,9,9,9,11,8,8,8,8,8,8,8,8,8,8,8,9,8,11,9,9,11,11,5,9,5,11,11,9,11,9,10,9,10,8,6,11,9,9,4,11,11,2,9,9~9,10,9,7,11,8,11,9,11,7,8,8,6,8,7,5,11,11,11,11,9,7,5,11,8,10,11,5,9,7,9,9,5,8,8,2,9,9,10,10,10,10,8,3,10,6,7,6,11,8,8,7,8,7,9,7,6,11,6,10,9,9,9,9,9,9,9,7,10,10,11,9,9,5,7,5,10,8,3,11,6,8,10,11,8,8,8,4,0,7,10,10,8,5,6,9,2,9,10,7,7,4,11,6,11,6,6,6,11,7,10,8,10,9,9,6,7,6,10,3,7,5,7,6,6,7,0,0,0,7,11,10,7,11,0,9,7,11,4,10,11,6,8,7,3,8,4,10~9,3,8,6,8,10,10,8,8,10,11,10,5,9,8,6,8,10,10,8,8,10,7,8,8,10,8,10,10,10,8,10,6,8,10,10,10,8,9,11,11,11,11,11,11,11,11,10,8,10,5,10,10,10,8,8,10,2,3,10,10,10,10,8,8,8,8,4,8,10,7,10,10,3,8,10,10,10,9,8,8,10,9,11,8,11,8,9,10,10,10,10,10,10,10,10,10,6,8,8,8,9,9,8,11,11,9,10,3,11,8,8,8,6,10,10,5,4,8,4,10,8,8,10,8,7,6,10,10,3,8,5,8,10,6,7,10,8,9,9,9,9,9,9,8,10,8,8,7,10,10,10,10,10,5,8,10,11,10,9,8,4,8,8,3,11,8,11,8,11,8,10,3,10,8,8,8,10,10,10,8,4,8,8,6,8,8,8,8,8,8,8,10,10,8,10,3,10,10,8,8,9,8,8,11,10,10,10,10,8,8,8,3,10,2,10,10,8,6,8,8,10,8,8,8,10,10,8,2,10,10,8,10,8&reel_set1=10,8,10,10,8,7,2,10,8,8,9,8,8,3,8,10,3,8,10,10,5,8,4,11,11,11,11,11,11,11,11,8,8,10,8,10,8,2,6,8,5,8,6,10,3,10,11,10,8,10,8,8,10,8,9,10,10,10,10,10,10,10,10,10,8,8,11,10,10,8,8,10,10,11,10,8,9,10,8,10,8,8,4,10,8,10,8,5,9,9,9,9,9,9,10,10,6,9,10,8,8,3,10,6,8,10,10,11,10,10,8,9,10,8,10,8,8,9,11,8,8,8,8,8,8,8,10,10,7,8,11,10,8,8,10,11,8,6,8,10,8,10,3,9,10,10,7,8,10,8,8,4~8,8,3,8,9,10,11,6,6,5,11,10,11,11,11,11,9,8,8,9,11,10,11,8,11,9,7,10,10,10,10,11,8,6,7,11,5,5,8,2,5,10,11,9,7,9,9,9,9,9,9,9,7,10,8,10,7,10,7,6,8,9,6,10,8,8,8,7,7,10,7,7,3,6,8,9,6,0,9,11,8,6,6,6,11,4,9,10,3,6,11,7,9,7,7,8,7,0,0,0,9,6,10,7,5,5,7,11,9,10,7,4,7,6~11,11,4,11,10,9,9,5,11,11,4,9,9,11,9,11,8,9,9,9,11,11,11,7,9,9,8,9,10,9,11,11,11,11,11,4,11,11,11,9,9,11,9,9,11,5,9,10,3,11,4,11,11,9,9,9,9,11,11,9,11,9,8,11,11,9,6,9,9,10,10,10,10,10,10,9,11,11,11,9,11,11,5,11,4,10,11,9,5,3,7,4,9,9,9,9,11,2,11,11,8,9,11,9,11,11,9,8,9,9,9,9,9,9,9,9,11,9,6,5,10,11,4,10,11,9,6,9,11,8,7,5,8,8,11,11,7,9,10,6,11,11,11,9,10,9,11,10,9,9,8,8,8,8,8,8,8,8,8,8,8,11,9,9,9,9,9,7,11,9,9,11,9,5,11,11,7,11,9,9,11,11,9,8,9,9,2,11,9,9,9,11,11,11,11,9";
            }
        }
	
	
        #endregion
        public LittleGemGameLogic()
        {
            _gameID = GAMEID.LittleGem;
            GameName = "LittleGem";
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
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "gsf_r", "gsf" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("s") && spinParams.ContainsKey("s"))
                resultParams["s"] = spinParams["s"];
            return resultParams;
        }
    }
}
