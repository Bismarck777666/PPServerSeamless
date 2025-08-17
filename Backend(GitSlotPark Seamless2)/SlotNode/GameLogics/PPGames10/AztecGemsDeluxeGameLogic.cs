using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class AztecGemsDeluxeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs9aztecgemsdx";
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
            get { return 9; }
        }
        protected override int ServerResLineCount
        {
            get { return 9; }
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
                return "def_s=3,5,4,7,3,5,6,4,7&c_paytable=9~any~3,4~10,0,0~2&cfgs=3245&reel1=2,3,3,3,3,7,5,6,6,6,8,4,4,4,6,4,7,7,7,5,5,5,4,3,5,3,7,4,3,4,6,5,6,4&ver=2&reel0=6,6,6,2,5,5,5,5,4,3,7,6,8,3,3,3,4,4,4,7,7,7,4,7,5,8,7&mo_s=8&def_sb=3,4,5&mo_v=5,8,18,28,58,68,88,108,128,288,888,900,2250&def_sa=3,4,5&reel2=4,2,3,6,5,8,4,4,4,7,6,6,6,5,5,5,7,7,7,5,7,5,6,7,6,5,7,5,6,5,6,5,7,3,6,2,5,8,5,2,3,5,3,5,7,5,6,5,6,2,7,2,5,6,5,3,5,6&bonusInit=[{bgid:2,bgt:42,bg_i:\"18,28,58,88,108,128,188,288,388,100,250,500,1000\",bg_i_mask:\"w,w,w,w,w,w,w,w,w,w,w,w,w\"},{bgid:3,bgt:42,bg_i:\"2,3,5,8,10\",bg_i_mask:\"wlm,wlm,wlm,wlm,wlm\"}]&mo_jp=900;2250;0&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{jp1:\"9000\",jp-units:\"coin\",jp3:\"2250\",jp2:\"4500\",jp4:\"900\"}}&mo_jp_mask=jp4;jp3;jp1&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,800.00,1000.00,1500.00,3000.00,5000.00,8500.00,12000.00&defc=100.00&wilds=2~250,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;88,0,0;58,0,0;28,0,0;18,0,0;8,0,0;0,0,0;0,0,0";
            }
        }
	
	
        #endregion
        public AztecGemsDeluxeGameLogic()
        {
            _gameID = GAMEID.AztecGemsDeluxe;
            GameName = "AztecGemsDeluxe";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
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

            string[] toCopyParams = new string[] { "g", "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "tw", "sw", "st", "wmt", "wmv", "rs_t", "rs_win", "gwm", "bw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;

                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
    }
}
