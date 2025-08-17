using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class LadyGodivaGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20godiva";
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
            get { return 20; }
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
                return "def_s=3,4,5,6,7,3,4,5,6,7,3,4,5,6,7&cfgs=2196&reel1=6,6,6,6,6,6,6,6,6,6,6,6,8,8,8,8,8,3,3,3,3,3,3,3,3,3,8,2,2,2,2,4,4,4,4,4,4,4,4,4,4,10,5,5,5,5,5,5,5,5,5,5,5,7,7,7,7,7,5,3,4,5,10,10,10,10,6,9,9,9,9,9,3,1,9&ver=2&reel0=7,7,7,7,7,5,5,5,5,5,5,5,5,5,2,2,2,2,7,3,3,3,3,3,3,3,3,3,4,4,4,4,4,4,3,9,9,9,9,4,5,4,5,6,6,6,6,6,6,6,6,10,9,8,8,8,8,5,6,1,4,6,10,10,10,10,2&def_sb=4,4,4,4,4&def_sa=7,7,7,7,7&reel3=1,6,6,6,6,6,6,6,6,6,6,6,5,5,5,5,5,5,3,3,3,3,3,5,4,4,4,4,4,4,6,5,3,4,2,2,2,2,6,4,10,3,8,8,8,8,8,7,7,7,7,7,9,9,9,9,9,10,10,10,10,7,4,9&reel2=1,5,5,5,5,5,5,5,5,5,6,6,6,6,6,6,6,6,6,6,8,8,8,8,8,6,2,2,2,5,3,3,3,3,3,3,3,3,3,6,5,3,4,4,4,4,4,4,4,4,4,9,9,9,9,9,4,9,7,7,7,7,7,10,10,10,10,7,4,1,8,4,5&reel4=4,4,4,4,4,4,1,3,3,3,3,3,3,3,3,3,6,6,6,6,6,6,6,6,6,10,4,3,9,9,9,9,10,10,10,10,5,5,5,5,5,8,8,8,8,8,4,7,7,7,7,7,5,2,2,2,2,4,7,8,5,6,5,3,9&scatters=&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~400,100,30,10,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,40,10,3,0;50,25,8,2,0;30,20,7,2,0;25,15,5,1,0;15,10,3,0,0;15,10,3,0,0;13,8,2,0,0;13,8,2,0,0";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 4; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203 };
        }
        #endregion
        public LadyGodivaGameLogic()
        {
            _gameID = GAMEID.LadyGodiva;
            GameName = "LadyGodiva";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
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
