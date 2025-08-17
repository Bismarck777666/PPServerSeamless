using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class DragonHoldAndSpinGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5drhs";
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
            get
            {
                return 5;
            }
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
                return "def_s=9,10,3,7,5,7,10,6,3,9,5,4,6,9,7&cfgs=4928&ver=2&mo_s=13;14;15&def_sb=8,7,6,10,10&mo_v=5,10,15,20,25,30,35,40,45;50,55,60,65,70,75,80,85,90,95,100;500,1000,1500,2000,2500,5000&reel_set_size=3&def_sa=8,10,3,10,5&balance_bonus=0.00&scatters=1~100,20,1,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={rtps:{regular:\"94.64\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"2000000\",max_rnd_win:\"20000\"}}&wl_i=tbm~20000&sc=0.01,0.02,0.05,0.08,0.10,0.15,0.20,0.25,0.40,0.50,0.75,1.00,2.00,3.00,5.00,10.00,15.00,20.00,25.00,30.00,35.00,40.00,50.00&defc=0.40&wilds=2~5000,1000,100,1,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,1000,100,0,0;500,200,50,0,0;200,50,20,0,0;200,50,20,0,0;200,50,20,0,0;50,10,2,0,0;50,10,2,1,0;50,10,2,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=6,9,9,9,9,8,7,7,7,10,3,10,10,10,5,13,7,1,3,3,3,4,6,6,6,8,8,8,13,13,13,8,14,9,4,9,10,3,10,3,9,10,3,1,3,5,3,4,7,4,3,9,4,9,3,10,13,9,3,9,3,7,8,9,3,4,10,4,14,4,3,8,4,9,4,7,3,10,3,9,10,15,4,3,8,3,8,3,10,4,9,3,4,3,1,9,3,10,9,3,10,3,10,7,9,8,10,8,9,3,10,9,8,3,7,10,7,9~10,7,7,7,7,6,8,13,13,14,9,5,5,5,3,1,10,10,10,13,8,8,8,5,4,9,9,9,5,7,5,4,9,3,8,3,9,5,13,3,9,4,7,8,13,6,9,8,3,7,5,6,13,7,9,13,7,14,9,5,7,9,6,7,4,3,9,3,5,3,8,3,15,6,5,8,13,6,7,9,13,8,13,9,5,13,3,9,13,8,7,5,4,7,3,8,9,13,4,13,8,9,8,5,9,5,3,4,13,3,9,13,5,13,8,7,5,3,13,3,9,5,8,9,13,9,5,13,8,13~8,8,8,4,8,10,9,4,4,4,13,6,6,6,6,7,10,10,10,1,7,7,7,3,5,9,9,9,3,3,3,15,13,14,4,9~7,9,8,7,7,7,3,5,4,9,9,9,1,5,5,5,13,6,3,3,3,10,8,8,8,10,10,10,15,13,13,10,9,10,5,9,3,4,9,10,13,9,13,4,14,5,10,9,13,3,5,4,9,5,10,9,3,10,9,5,1,10,9,13,9,5,10,5,3,9,3,13,4,3,10,1,8,13,9,5,8,5,13,9,10,8,9,10,13,10,3,4,13,9,10,5,9,8,10,14,1,3,10,9,3,8,9,13,10,9,13,4,5~5,8,6,10,10,10,14,5,5,5,4,9,13,13,13,7,3,4,4,4,10,1,8,8,8,6,6,6,10,13,10,4,1,6,10,4,8,10,6,4,10,6,7,15,10,4&reel_set2=12,12,12,12,12,15,13,13,12,13,13,13,12,13,12,12,14~12,12,13,12,12,12,12,12,13,12,12,12,13,12,13,13,13,14,15,12,12,12,13,13,13,12,12,13,12,13,13,12,14,15,14,12~13,13,12,15,12,12,12,12,13,13,12,14,13,13,13,12,12,13,12,14,12,12~12,12,12,12,12,13,14,12,12,13,12,12,13,13,13,13,12,12,15~15,12,13,12,13,15,12,12,12,12,12,12,13,13,12,12,13,12,13,12,13,14,12,13,13,13,12,13,14,12,13,14,12,12,12,12,12,12&reel_set1=6,6,6,1,5,3,9,8,7,4,6,3,5,10,4,9,7,9,5,4~9,3,10,10,10,6,8,1,6,4,8,8,8,10,7,5,6,3,9,8,10,8,7,10,1,5,3,10,3,10~6,6,6,1,8,8,8,8,4,7,10,9,5,10,9,6,7,10,3,8,9,7,3,8,5,6,8,9,8,5,8,9,4,8,1,9,3,5,8,1,9,4,9,10,5,8,7,5,8~10,5,9,6,8,8,8,8,5,1,3,8,9,7,10,4,8,1,8,3,5,8,4,6~10,3,7,5,1,8,3,9,8,8,8,5,4,6,10,7,1,6,8,3,5,8,5,8,6,4,5,1,8,1,3,4,3,6,8,1,3,8,6,4,1,7,1,9,5,4,8,6,3,4";
            }
        }
        #endregion
        public DragonHoldAndSpinGameLogic()
        {
            _gameID = GAMEID.DragonHoldAndSpin;
            GameName = "DragonHoldAndSpin";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams,int currency, double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams, currency, userBalance, index, counter);
            dicParams.Add("reel_set", "0");
        }

        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set", "gsf_r", "gsf" };
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
