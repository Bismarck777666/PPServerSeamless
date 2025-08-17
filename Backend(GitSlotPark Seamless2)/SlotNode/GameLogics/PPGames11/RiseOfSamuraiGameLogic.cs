using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class RiseOfSamuraiGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25samurai";
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
                return "def_s=3,4,5,6,3,3,4,5,6,3,3,4,5,6,3&cfgs=3423&ver=2&reel_set_size=6&def_sb=5,5,5,5,5&def_sa=5,5,5,5,5&scatters=&gmb=0,0,0&rt=d&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~400,100,30,10,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,40,10,3,0;50,25,8,2,0;30,20,7,2,0;25,15,5,1,0;15,10,3,0,0;15,10,3,0,0;13,8,2,0,0;13,8,2,0,0&reel_set0=3,3,3,8,8,8,8,8,4,4,4,7,7,7,1,5,5,5,5,5,9,9,9,2,2,2,2,1,6,6,6,10,10,10,10~3,3,3,3,8,8,8,1,4,4,4,4,7,7,7,7,5,5,5,5,5,9,9,9,2,2,2,2,1,6,6,6,6,6,10,10,10~3,3,3,3,8,8,8,1,4,4,4,4,4,4,4,4,4,7,7,7,7,7,5,5,5,5,9,9,9,2,2,2,1,6,6,6,10,10,10,10~3,3,3,3,3,8,8,8,1,4,4,4,4,4,4,7,7,7,5,5,5,5,5,9,9,2,2,2,2,6,6,6,6,6,10,10,10~3,3,3,3,8,8,8,8,4,4,4,4,4,4,7,7,7,5,5,5,5,5,5,9,9,9,9,9,2,2,2,2,1,6,6,6,6,6,10,10,10,10,10&reel_set2=3,3,3,3,3,3,3,6,10,1,6,8,4,7,5,9,2,10,4,7,5,9~3,3,3,3,6,10,1,6,8,4,7,5,9,2,10,8,7,5,9~3,3,3,3,3,3,3,6,1,6,8,4,7,5,9,2,10,8,4,7,5,9~3,3,3,3,3,3,3,6,10,1,6,8,4,7,5,9,2,10,8,4,7,5,9~3,3,3,3,6,10,1,6,8,4,7,5,9,2,10,8,4,7,5,9&reel_set1=2,2,2,2,2,2,6,10,1,3,8,4,7,5,9,6,10,3,8,4,7,5,9~2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,6,10,1,3,8,4,7,5,9,6,10,3,8,4,7,5,9~2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,6,10,1,3,8,4,7,5,9,6,10,3,8,4,7,5,9~2,2,2,2,2,2,2,2,2,2,2,2,2,6,10,1,3,8,4,7,5,9,6,10,3,8,4,7,5,9~2,2,2,2,2,2,2,2,2,2,6,10,1,3,8,4,7,5,9,6,10,3,8,4,7,5,9,6&reel_set4=5,5,5,5,6,10,1,3,8,4,7,2,9,6,10,3,8,4,7,9~5,5,5,5,5,5,5,5,5,5,5,5,5,6,10,1,3,8,4,7,2,9,6,10,3,8,4,7,9~5,5,5,5,5,5,5,5,5,5,5,5,5,6,10,1,3,8,4,7,2,9,6,10,3,8,4,7,9~5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,6,10,1,3,8,4,7,2,9,6,10,3,8,4,7,9~5,5,5,5,5,5,5,5,5,5,5,5,5,5,6,10,1,3,8,4,7,2,9,6,10,8,4,7,9&reel_set3=4,4,4,4,4,4,4,4,4,6,10,1,3,8,2,7,5,9,6,10,3,8,7,5,9~4,4,4,4,4,4,4,4,4,4,4,4,6,1,3,8,2,7,5,9,6,10,3,8,7,5,9~4,4,4,4,4,4,4,4,4,4,4,4,6,10,1,3,8,2,7,5,9,6,10,3,8,7,5,9~4,4,4,4,6,10,1,3,8,2,7,5,9,6,10,3,8,7,5,9~4,4,4,4,4,4,4,6,10,1,3,8,2,7,5,9,6,10,3,8,7,5,9&reel_set5=6,6,6,6,6,10,1,3,8,4,7,5,9,2,10,3,8,4,7,5,9~6,6,6,6,6,6,6,6,6,6,6,10,1,3,8,4,7,5,9,2,10,3,8,4,7,5,9~6,6,6,6,6,6,6,6,6,6,6,6,10,1,3,8,4,7,5,9,2,10,3,8,4,7,5,9~6,6,6,6,6,6,6,6,6,10,1,3,8,4,7,5,9,2,10,3,8,4,7,5,9~6,6,6,6,6,6,6,6,6,10,1,3,8,4,7,5,9,2,10,3,8,4,7,5,9,3";
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
        public RiseOfSamuraiGameLogic()
        {
            _gameID = GAMEID.RiseOfSamurai;
            GameName = "RiseOfSamurai";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if(betInfo.SpinData != null && betInfo.SpinData.SpinType >= 200)
                dicParams["fsopt_i"] = (betInfo.SpinData.SpinType - 200).ToString();
            
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

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set" };
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
    }
}
