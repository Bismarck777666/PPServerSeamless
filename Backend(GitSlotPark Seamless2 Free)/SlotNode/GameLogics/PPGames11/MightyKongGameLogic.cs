using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MightyKongGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs50kingkong";
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
                return "def_s=10,4,6,11,6,10,5,12,6,4,3,11,12,1,5,9,11,3,12,7&cfgs=2230&reel1=12,12,12,8,8,8,10,10,10,6,4,8,10,9,7,5,1,12,11,11,11,3,11,12,9,7&ver=2&reel0=4,9,10,10,10,9,6,12,12,12,12,10,7,11,11,11,11,8,8,8,11,3,1,7,3,9,5,8,8,10&def_sb=6,11,11,11,4&def_sa=4,9,10,10,10&reel3=8,8,8,3,5,10,10,10,10,9,10,11,11,11,11,10,5,7,12,12,12,8,3,11,7,1,9,4,6,12,7&reel2=5,8,8,8,12,6,12,5,6,7,4,10,12,11,1,9,3,1,2,10,10,6,8,4,9,10,6,11,4,7,8,9,3,12,11,7&reel4=6,11,11,11,4,10,10,10,3,12,12,12,9,8,8,8,8,9,1,11,4,12,7,11,7,3,6,4,5,10&scatters=&gmb=0,0,0&rt=d&sc=4.00,8.00,12.00,16.00,20.00,40.00,60.00,80.00,100.00,150.00,200.00,300.00,500.00,1000.00,1500.00,2000.00&defc=20.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,300,100,0,0;500,150,90,0,0;250,125,75,0,0;150,100,70,0,0;100,80,60,0,0;80,60,30,0,0;80,60,25,0,0;75,50,20,0,0;75,50,20,0,0;75,50,20,0,0";
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
        public MightyKongGameLogic()
        {
            _gameID = GAMEID.MightyKong;
            GameName = "MightyKong";
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
