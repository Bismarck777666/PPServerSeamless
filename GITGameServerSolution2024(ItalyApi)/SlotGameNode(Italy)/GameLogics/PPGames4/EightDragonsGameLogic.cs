using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class EightDragonsGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20eightdragons";
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
                return "msi=14&def_s=8,5,7,4,3,10,1,13,9,10,4,13,12,7,11&msr=3&cfgs=5210&ver=2&prm=2~2&reel_set_size=2&def_sb=10,14,14,14,14&def_sa=8,14,14,14,14&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.10&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,100,20,2,0;130,50,15,0,0;100,50,15,0,0;70,25,10,0,0;70,25,10,0,0;50,20,10,0,0;50,20,10,0,0;50,15,5,0,0;50,15,5,0,0;40,10,5,0,0;40,10,5,0,0;0,0,0,0,0&rtp=96.37,96.39&reel_set0=6,4,14,14,14,14,4,12,10,11,6,14,14,14,14,12,13,9,5,7,5,9,7,3,8,1,3,11~5,11,10,10,3,7,9,14,14,14,14,14,6,12,4,11,14,14,14,14,4,3,2,2,6,12,8,9,8,1,13~14,14,14,14,1,14,14,14,8,7,5,13,11,10,14,14,14,14,9,10,12,5,6,1,2,2,4,3,6,12,9,8~7,12,4,4,1,9,10,2,2,7,14,14,14,14,14,14,14,5,2,2,11,6,6,8,14,14,14,14,14,11,5,3,13~11,9,1,10,6,7,13,14,14,14,14,14,14,6,8,4,3,7,5,14,14,14,14,14,9,12,10,11,4&reel_set1=5,6,3,11,7,13,11,8,8,1,12,5,3,4,9,10,7,12,13,6,14,14,14,14,14,14,14,14,9~5,11,7,3,2,2,1,14,14,14,14,14,14,14,14,14,12,8,2,2,8,10,6,6,13,9,9,12,10,4~11,13,2,2,9,14,14,14,14,14,14,14,1,3,2,2,13,5,4,4,8,10,7,2,2,12,10,12,6,9~5,6,11,4,13,3,6,3,2,2,12,10,14,14,14,14,14,14,9,1,2,2,12,5,4,8,8,7,2,2~3,10,8,5,14,14,14,14,14,6,1,12,7,6,9,9,10,11,11,7,13,4,3";
            }
        }

        protected override int FreeSpinTypeCount
        {
            get { return 7; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204 };
        }
        #endregion

        public EightDragonsGameLogic()
        {
            _gameID = GAMEID.EightDragons;
            GameName = "EightDragons";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, int currency, double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams, currency, userBalance, index, counter);
            dicParams["n_reel_set"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul","fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total","n_reel_set",
                "s", "w", "tw", "reel_set"};
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;

                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("na") || resultParams["na"] != "fso")
            {
                resultParams.Remove("fs_opt");
                resultParams.Remove("fs_opt_mask");
            }
            return resultParams;
        }
    }
}
