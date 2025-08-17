using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class PixieWingsGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs50pixie";
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
            get { return 50; }
        }
        protected override int ServerResLineCount
        {
            get { return 50; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "msi_p=2,2,2,2,2&msi=12&def_s=5,8,1,4,7,5,6,11,4,11,5,8,4,7,7,5,6,5,6,5&cfgs=2587&ver=2&reel_set_size=6&def_sb=7,12,12,12,12&def_sa=10,12,12,12,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=4.00,8.00,12.00,16.00,20.00,40.00,60.00,80.00,100.00,150.00,200.00,300.00,500.00,1000.00,1500.00,2000.00&defc=20.00&wilds=2~500,150,40,10,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;75,30,10,4,0;45,20,8,3,0;30,18,7,2,0;25,15,6,1,0;20,12,5,0,0;15,10,4,0,0;14,9,3,0,0;13,8,2,0,0;12,7,1,0,0;0,0,0,0,0&reel_set0=11,7,9,3,11,10,1,5,4,5,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,6,3,11,7,5,11,1,11,3,11,9,11,8,9,3,5,9,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,11,3,11,1,5,11,9,11,5,2,7,9,5~8,6,8,3,8,11,1,6,5,8,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,7,4,8,6,4,8,1,6,10,4,6,8,6,4,9,10,8,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,6,4,8,1,6,4,8,6,8,2,6,8,6,10,8,10~11,7,9,3,11,10,1,11,4,5,12,12,12,12,12,12,12,12,12,12,6,3,5,7,11,7,1,7,3,7,11,9,8,11,3,11,9,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,9,3,11,1,7,5,9,7,11,2,9,7,5,7,9,5,1,7,9,5,9~10,6,10,3,10,11,1,6,5,8,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,7,6,8,6,4,10,1,8,4,6,4,6,4,10,9,8,4,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,4,6,10,1,4,10,8,10,8,2,8,10,4,8,6,8,1,8,10,6~11,9,5,3,5,10,1,11,4,9,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,6,3,11,9,11,7,1,11,3,9,11,7,8,11,3,11,4,12,12,12,12,12,12,12,12,12,12,12,12,12,12,7,3,7,1,7,11,7,5,7,2,7,9,11,7,9,7,9,11,7,9&reel_set2=10,9,5,4,4,4,4,4,4,4,4,4,8,8,2,4,3,7,4,4,4,4,4,4,4,3,11,6,6,6,4,5,7,1,8,4,4,4,4,4,10,9,7,11~1,9,2,4,4,4,4,4,4,4,4,6,11,10,4,4,4,4,4,4,3,11,4,4,4,4,5,6,6,4,10,5,7,6,8,7,9,4,4,4,4,4,4,4,4,3,10~4,4,4,4,4,4,4,4,9,8,9,1,11,5,4,9,7,4,4,4,4,4,4,4,11,7,8,3,7,4,10,5,8,4,4,4,4,4,4,4,2,6,8,3~4,4,4,4,4,4,4,4,4,4,4,4,9,8,11,6,4,4,4,4,4,4,4,3,10,4,7,5,10,11,1,8,4,8,6,5,2,7,7,10,4,4,4,4,4,11~10,7,5,4,4,4,4,4,4,4,4,9,9,1,4,4,4,4,4,4,4,7,5,4,10,8,11,2,5,11,6,3,5,3,10,3,6,8,4,4,4,4,4,4&reel_set1=10,9,3,3,3,3,3,3,3,3,3,3,9,9,3,10,7,6,3,6,6,4,4,3,3,3,3,3,3,5,1,2,5,3,3,3,3,3,3,3,3,3,7,11,11,8,5,10,8~8,3,3,3,3,3,3,3,3,7,11,11,1,6,3,3,3,3,3,3,7,6,2,8,3,3,3,3,3,3,3,6,6,7,5,10,4,4,3,9,3,3,3,3,3,3,3,10,8,6,3~3,3,3,3,3,3,3,1,2,10,6,9,7,8,2,3,5,8,11,7,9,8,3,11,4,7,7,3,3,3,3,3,3,3,4,11,9,3,5,10,3,3,3,3,3,3,3,3~3,3,3,3,3,3,3,3,3,11,4,5,10,3,3,3,3,3,3,3,11,9,10,3,2,7,8,7,10,6,11,4,5,8,6,1,3,3,3,3,3,3,3,3,3~3,3,3,3,3,3,3,3,3,3,3,1,7,9,7,3,9,11,8,5,6,6,10,3,3,3,3,3,3,3,3,11,10,5,5,4,5,3,3,3,3,3,3,9,2,4,10,10,7&reel_set4=10,10,9,6,6,6,6,6,6,6,6,6,6,6,6,6,1,10,11,11,5,4,6,6,6,6,6,6,6,3,8,2,11,8,7,7,9,5,6,6,6,6,6,6,6,3,7,9,4~6,6,6,6,6,6,6,6,6,6,6,6,6,11,5,7,3,11,10,6,6,6,6,6,6,6,10,7,4,9,5,1,8,8,7,11,4,9,6,6,6,6,6,6,6,6,10,3,8,2,3~8,9,6,6,6,6,6,6,6,6,6,6,3,9,4,10,5,11,6,6,6,6,6,6,6,4,8,11,7,11,8,9,7,6,6,6,6,6,6,9,7,3,8,3,10,2,1,5,4,10~3,4,5,6,6,6,6,6,6,6,6,6,6,6,5,8,9,11,4,1,4,11,5,11,6,6,6,6,6,6,7,9,7,10,7,3,10,6,6,6,6,6,6,8,2~6,6,6,6,6,6,6,6,6,3,9,3,5,10,7,7,4,7,3,10,6,6,6,6,6,6,6,6,6,8,11,10,8,5,9,6,6,6,6,6,6,1,5,2&reel_set3=6,5,5,5,5,5,5,5,5,7,3,7,11,3,9,1,7,11,5,5,5,5,5,5,5,9,4,6,9,10,8,10,6,3,5,5,5,5,5,5,5,5,5,2,8,4~11,5,5,5,5,5,5,5,5,9,7,6,9,10,5,5,5,5,5,5,5,5,6,4,11,5,5,5,5,5,1,11,2,5,5,5,5,5,5,5,5,5,6,8,10,3,3,8~7,5,5,5,5,5,5,5,5,9,10,11,8,3,4,4,5,3,7,8,5,5,5,5,5,5,5,5,8,2,6,9,9,7,5,5,5,5,5,5,5,5,11,6,1,3,8~4,9,5,5,5,5,5,5,4,10,8,5,5,5,5,5,5,7,3,2,5,11,1,7,5,3,4,6,5,5,5,5,5,5,5,5,5,11,7,10,10,9,11,6~5,5,5,5,5,5,5,5,1,7,6,11,4,9,5,5,5,5,5,5,4,5,3,9,8,5,10,8,3,2,5,5,5,5,5,5,11,10,10,3,10,7&reel_set5=1,4,7,2,2,2,2,2,2,2,2,2,6,10,10,9,10,9,6,11,7,2,2,2,2,2,2,11,4,5,5,8,8,5,2,2,2,2,3,4,6~5,2,2,2,2,2,2,2,2,2,2,10,9,7,5,7,7,6,10,8,2,2,2,2,2,2,3,6,9,7,8,6,11,11,2,2,2,2,2,2,2,2,8,3,4,3,10,6,1~9,2,2,2,2,2,2,2,11,3,8,9,2,2,2,2,2,2,8,7,9,9,11,8,4,8,5,10,4,3,1,2,2,2,2,8,6,7,10,5~10,8,6,3,2,2,2,2,2,2,3,11,8,11,10,1,5,2,2,2,2,2,8,11,8,10,6,9,11,6,2,2,2,2,2,2,7,9,7,4,9,4~4,11,2,2,2,2,2,2,2,2,2,8,7,10,3,7,2,2,2,2,2,9,10,4,3,10,6,7,2,2,2,2,2,2,3,5,1,9,2,2,2,2,3,5,8";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 5; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204 };
        }

        #endregion
        public PixieWingsGameLogic()
        {
            _gameID = GAMEID.PixieWings;
            GameName = "PixieWings";
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
        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                await base.buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, minOdd, maxOdd);
            else
                await base.buildStartFreeSpinData(startSpinData, buildType, minOdd, maxOdd);
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if (!resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
                resultParams["g"] = spinParams["g"];
            return resultParams;
        }

        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (dicParams["na"] == "s")
            {
                dicParams.Remove("fs_opt");
                dicParams.Remove("fs_opt_mask");
            }
        }
    }
}
