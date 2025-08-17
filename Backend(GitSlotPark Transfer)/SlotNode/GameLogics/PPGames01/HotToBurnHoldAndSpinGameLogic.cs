using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class HotToBurnHoldAndSpinGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20hburnhs";
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
            get
            {
                return 20;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 20;
            }
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
                return "def_s=9,10,3,7,5,7,10,6,3,9,5,4,6,9,7&cfgs=4746&ver=2&mo_s=11&def_sb=8,7,6,10,10&mo_v=20,40,60,80,100,120,140,160,180,200,220,240,260,280,300,320,340,360,380,400,2000,4000,6000,8000,10000,20000&reel_set_size=2&def_sa=8,10,3,10,5&bonusInit=[{bgid:1,bgt:51,mo_s:\"13,13,13,13,13,13,13,13,13,14,14,14,14,14,14,14,14,14,14,14,15,15,15,15,15,15\",mo_v:\"20,40,60,80,100,120,140,160,180,200,220,240,260,280,300,320,340,360,380,400,2000,4000,6000,8000,10000,20000\"}]&scatters=1~100,20,1,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2000000\",max_rnd_win:\"20000\"}}&wl_i=tbm~20000&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~5000,1000,100,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,1000,100,0,0;500,200,50,0,0;200,50,20,0,0;200,50,20,0,0;200,50,20,0,0;50,10,2,0,0;50,10,2,1,0;50,10,2,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&rtp=94.65&reel_set0=6,9,9,9,9,8,7,7,7,10,3,10,10,10,5,11,7,1,3,3,3,4,6,6,6,8,8,8,11,11,11,8,11,9,4,9,10,3,10,3,9,10,3,1,3,5,3,4,7,4,3,9,4,9,3,10,11,9,3,9,3,7,8,9,3,4,10,4,11,4,3,8,4,9,4,7,3,10,3,9,10,11,4,3,8,3,8,3,10,4,9,3,4,3,1,9,3,10,9,3,10,3,10,7,9,8,10,8,9,3,10,9,8,3,7,10,7,9~10,7,7,7,7,6,8,11,11,11,9,5,5,5,3,1,10,10,10,11,8,8,8,5,4,9,9,9,5,7,5,4,9,3,8,3,9,5,11,3,9,4,7,8,11,6,9,8,3,7,5,6,11,7,9,11,7,11,9,5,7,9,6,7,4,3,9,3,5,3,8,3,11,6,5,8,11,6,7,9,11,8,11,9,5,11,3,9,11,8,7,5,4,7,3,8,9,11,4,11,8,9,8,5,9,5,3,4,11,3,9,11,5,11,8,7,5,3,11,3,9,5,8,9,11,9,5,11,8,11~8,8,8,4,8,10,9,4,4,4,11,6,6,6,6,7,10,10,10,1,7,7,7,3,5,9,9,9,3,3,3,11,11,11,4,9~7,9,8,7,7,7,3,5,4,9,9,9,1,5,5,5,11,6,3,3,3,10,8,8,8,10,10,10,11,11,11,10,9,10,5,9,3,4,9,10,11,9,11,4,11,5,10,9,11,3,5,4,9,5,10,9,3,10,9,5,1,10,9,11,9,5,10,5,3,9,3,11,4,3,10,1,8,11,9,5,8,5,11,9,10,8,9,10,11,10,3,4,11,9,10,5,9,8,10,11,1,3,10,9,3,8,9,11,10,9,11,4,5~5,8,6,10,10,10,11,5,5,5,4,9,11,11,11,7,3,4,4,4,10,1,8,8,8,6,6,6,10,11,10,4,1,6,10,4,8,10,6,4,10,6,7,11,10,4&reel_set1=6,6,6,1,5,3,9,8,7,4,6,3,5,10,4,9,7,9,5,4~9,3,10,10,10,6,8,1,6,4,8,8,8,10,7,5,6,3,9,8,10,8,7,10,1,5,3,10,3,10~6,6,6,1,8,8,8,8,4,7,10,9,5,10,9,6,7,10,3,8,9,7,3,8,5,6,8,9,8,5,8,9,4,8,1,9,3,5,8,1,9,4,9,10,5,8,7,5,8~10,5,9,6,8,8,8,8,5,1,3,8,9,7,10,4,8,1,8,3,5,8,4,6~10,3,7,5,1,8,3,9,8,8,8,5,4,6,10,7,1,6,8,3,5,8,5,8,6,4,5,1,8,1,3,4,3,6,8,1,3,8,6,4,1,7,1,9,5,4,8,6,3,4";
            }
        }
        #endregion

        public HotToBurnHoldAndSpinGameLogic()
        {
            _gameID = GAMEID.HotToBurnHoldAndSpin;
            GameName = "HotToBurnHoldAndSpin";
        }

        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
        }

        protected override void copyBonusParamsToResultParams(Dictionary<string, string> bonusParams, Dictionary<string, string> resultParams)
        {
            base.copyBonusParamsToResultParams(bonusParams, resultParams);
            if (bonusParams.ContainsKey("s"))
                resultParams["s"] = bonusParams["s"];
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
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
