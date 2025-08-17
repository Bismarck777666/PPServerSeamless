using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class TheTweetyHouseGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20tweethouse";
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
                return "def_s=9,3,2,3,9,10,4,1,4,10,9,3,2,3,9&cfgs=5220&ver=2&reel_set_size=2&def_sb=8,10,8,11,7&def_sa=8,9,10,11,12&scatters=1~0,0,5,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&mbri=1,2,3&rt=d&gameInfo={rtps:{regular:\"95.51\"}}&sc=0.01,0.02,0.03,0.04,0.05,0.10,0.20,0.30,0.40,0.50,0.75,1.00,2.00,3.00,4.00,5.00&defc=0.10&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;750,150,50,0,0;500,100,35,0,0;300,60,25,0,0;200,40,20,0,0;150,25,12,0,0;100,20,8,0,0;50,10,5,0,0;50,10,5,0,0;25,5,2,0,0;25,5,2,0,0;25,5,2,0,0&reel_set0=9,8,12,8,10,7,5,11,4,1,3,7,10,13,1,6,9,13,6,11,12~3,6,8,13,7,10,9,11,10,9,6,5,12,2,4,8,11,12,13,7~4,9,13,12,13,7,8,12,6,1,2,10,11,7,5,11,3,10,8,9,6~2,6,10,7,11,13,12,5,9,3,6,7,12,9,13,8,10,11,4,8~8,11,7,6,13,9,10,5,1,12,6,3,8,4,7,10,13,12,11,9&reel_set1=12,5,11,9,13,8,13,3,3,3,10,12,11,10,13,11,8,8,9,6,9,10,12,6,3,7,4,7,5~13,11,7,9,4,12,7,3,10,9,8,13,11,10,13,5,6,9,2,7,6,10,12,8,11~6,12,10,13,7,12,5,10,8,7,2,13,3,6,9,8,11,8,5,12,9,4,11,10,9,13~13,9,5,7,13,6,12,11,6,10,13,12,9,7,8,10,4,2,8,7,5,9,11,3,12,8,6,10,11~13,12,11,7,10,11,7,13,4,9,12,6,10,3,3,3,8,6,11,8,9,13,7,9,5,8,12&mbr=1,1,1";
            }
        }
        #endregion

        public TheTweetyHouseGameLogic()
        {
            _gameID = GAMEID.TheTweetyHouse;
            GameName = "TheTweetyHouse";
        }

        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams,int currency, double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams,currency, userBalance, index, counter);
            dicParams["n_reel_set"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set" };
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
