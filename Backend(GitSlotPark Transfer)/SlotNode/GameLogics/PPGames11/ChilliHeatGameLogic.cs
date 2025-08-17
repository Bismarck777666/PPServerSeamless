using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class ChilliHeatGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25chilli";
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
                return "def_s=6,7,4,2,8,9,8,5,6,7,8,6,7,3,9&cfgs=2199&ver=2&mo_s=11&reel_set_size=2&def_sb=6,10,3,7,6&mo_v=25,50,75,100,125,150,175,200,250,350,400,450,500,600,750,2500&def_sa=3,7,6,11,9&mo_jp=750;2500;25000&scatters=1~0,0,1,0,0~0,0,8,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&mo_jp_mask=jp3;jp2;jp1&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,50,10,0,0;150,30,5,0,0;125,25,5,0,0;100,25,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=3,7,6,11,9,7,4,4,9,10,10,5,5,10,7,8,8,11,6,3,6,6~4,9,6,1,9,10,8,5,5,8,3,4,8,9,4,1,7,2,2,2,2,2,10,9,6,11,11,11~10,8,6,8,1,2,2,2,2,2,2,3,5,11,11,11,8,4,11,8,2,10,9,3,7,2,8,4,5,6,1~3,7,11,11,11,3,5,10,8,7,8,7,2,2,2,2,2,2,2,2,1,9,10,6,6,10,4,7,4,3,5,11,8,6,9,1,2,7~6,10,3,7,6,5,3,4,11,11,11,8,8,4,5,3,6,10,2,2,2,2,7,9,10,7,7,10,9,5,4,6,11&reel_set1=5,6,5,11,4,6,6,11,3,3,4~3,5,6,6,11,11,11,2,2,2,11,4,3,5,4,2,1,3~6,2,2,2,2,6,4,4,5,6,4,1,5,2,11,11,11,2,3,5,11~4,5,1,5,6,11,11,11,2,2,2,3,6,3,4,4,1,11,5,2~2,2,2,6,5,4,3,2,6,3,6,3,11,5,6,4";
            }
        }
	
	
        #endregion
        public ChilliHeatGameLogic()
        {
            _gameID = GAMEID.ChilliHeat;
            GameName = "ChilliHeat";
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
