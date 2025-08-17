using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class FloatingDragonHoldAndSpinGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10floatdrg";
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
                return 10;
            }
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
                return "def_s=9,6,11,5,10,7,6,10,9,9,11,7,5,6,12&cfgs=4084&ver=2&def_sb=7,8,8,9,5&reel_set_size=5&def_sa=12,9,8,9,5&bonusInit=[{bgid:0,bgt:48,mo_s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,15,15,15,15,15\",mo_v:\"10,20,30,40,50,60,70,80,90,100,110,120,140,160,180,200,1000,2000,3000,4000,49930\"}]&scatters=1~0,0,0,0,0~20,15,10,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"628535\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=0.02,0.03,0.05,0.10,0.15,0.20,0.50,1.00,2.00,4.00,6.00,8.00,10.00&defc=0.20&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=13,7,8,9,11,1,6,7,13,8,12,7,6,11,7,6,9,8,8,13,5,11,6,9,5,11,4,13,9,11,5,9,10,1,13,11,10,5,4,11,6,9,13,4,7,9,13,7,5,11,12,13,9,8,8,8,8,8~7,5,4,10,8,12,4,9,7,4,10,8,12,6,11,7,4,10,8,8,13,4,5,12,4,7,1,10,6,12,10,7,12,5,10,4,6,5,12,7,4,6,12,1,13,10,11,5,4,12,6,1,10,9,8,8,8,8,8~7,13,1,4,7,8,0,0,0,12,4,6,8,11,5,4,6,1,10,5,6,7,8,8,13,0,10,6,5,4,9,6,7,12,5,7,1,11,4,7,5,4,9,8,8,8,8,8~5,6,8,7,1,13,6,1,9,8,7,5,12,4,10,8,8,11,7,4,6,1,8,8,8,8,8~6,5,7,8,12,1,5,9,8,6,4,13,1,8,8,11,1,7,10,8,8,8,8,8&accInit=[{id:0,mask:\"cp\"},{id:1,mask:\"cp; mp\"}]&reel_set2=13,7,8,9,11,1,6,7,13,8,12,7,6,11,7,6,9,8,8,13,5,11,6,9,5,11,4,13,9,11,5,9,10,1,13,11,10,5,4,11,6,9,13,4,7,9,13,7,5,11,12,13,9,8,8,8,8,8~7,5,4,10,8,12,4,9,7,4,10,8,12,6,11,7,4,10,8,8,13,4,5,12,4,7,1,10,6,12,10,7,12,5,10,4,6,5,12,7,4,6,12,1,13,10,11,5,4,12,6,1,10,9,8,8,8,8,8~7,13,1,4,7,8,0,0,0,12,4,6,8,11,5,4,6,1,10,5,6,7,8,8,13,0,10,6,5,4,9,6,7,12,5,7,1,11,4,7,5,4,9,8,8,8,8,8~5,6,8,7,1,13,6,1,9,8,7,5,12,4,10,8,8,11,7,4,6,1,8,8,8,8,8~6,5,7,8,12,1,5,9,8,6,4,13,1,8,8,11,1,7,10,8,8,8,8,8&reel_set1=13,7,8,9,11,1,6,7,13,8,12,7,6,11,7,6,9,8,8,13,5,11,6,9,5,11,4,13,9,11,5,9,10,1,13,11,10,5,4,11,6,9,13,4,7,9,13,7,5,11,12,13,9,8,8,8,8,8~7,5,4,10,8,12,4,9,7,4,10,8,12,6,11,7,4,10,8,8,13,4,5,12,4,7,1,10,6,12,10,7,12,5,10,4,6,5,12,7,4,6,12,1,13,10,11,5,4,12,6,1,10,9,8,8,8,8,8~7,13,1,4,7,8,0,0,0,12,4,6,8,11,5,4,6,1,10,5,6,7,8,8,13,0,10,6,5,4,9,6,7,12,5,7,1,11,4,7,5,4,9,8,8,8,8,8~5,6,8,7,1,13,6,1,9,8,7,5,12,4,10,8,8,11,7,4,6,1,8,8,8,8,8~6,5,7,8,12,1,5,9,8,6,4,13,1,8,8,11,1,7,10,8,8,8,8,8&reel_set4=13,7,8,9,11,2,6,7,13,8,12,7,6,11,7,6,9,8,8,13,5,11,6,9,5,11,4,13,9,11,5,9,10,2,13,11,10,5,4,11,6,9,13,4,7,9,13,7,5,11,12,13,9,8,8,8,8,8~7,5,4,10,8,12,4,9,7,4,10,8,12,6,11,7,4,10,8,8,13,4,5,12,4,7,2,10,6,12,10,7,12,5,10,4,6,5,12,7,4,6,12,2,13,10,11,5,4,12,6,2,10,9,8,8,8,8,8~7,13,2,4,7,8,12,4,6,8,11,5,4,6,2,10,5,6,7,8,8,13,10,6,5,4,9,6,7,12,5,7,2,11,4,7,5,4,9,8,8,8,8,8~5,6,8,7,2,13,6,2,9,8,7,5,12,4,10,8,8,11,7,4,6,2,8,8,8,8,8~6,5,7,8,12,2,5,9,2,8,6,4,13,8,8,11,2,7,10,8,8,8,8,8&reel_set3=13,7,8,9,11,1,6,7,13,8,12,7,6,11,7,6,9,8,8,13,5,11,6,9,5,11,4,13,9,11,5,9,10,1,13,11,10,5,4,11,6,9,13,4,7,9,13,7,5,11,12,13,9,8,8,8,8,8~7,5,4,10,8,12,4,9,7,4,10,8,12,6,11,7,4,10,8,8,13,4,5,12,4,7,1,10,6,12,10,7,12,5,10,4,6,5,12,7,4,6,12,1,13,10,11,5,4,12,6,1,10,9,8,8,8,8,8~7,13,1,4,7,8,0,0,0,12,4,6,8,11,5,4,6,1,10,5,6,7,8,8,13,0,10,6,5,4,9,6,7,12,5,7,1,11,4,7,5,4,9,8,8,8,8,8~5,6,8,7,1,13,6,1,9,8,7,5,12,4,10,8,8,11,7,4,6,1,8,8,8,8,8~6,5,7,8,12,1,5,9,8,6,4,13,1,8,8,11,1,7,10,8,8,8,8,8";
            }
        }
        #endregion

        public FloatingDragonHoldAndSpinGameLogic()
        {
            _gameID = GAMEID.FloatingDragonHoldAndSpin;
            GameName = "FloatingDragonHoldAndSpin";
        }

        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams,int currency, double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams,currency, userBalance, index, counter);
            dicParams["reel_set"] = "0";
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
