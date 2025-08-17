using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WildSpellsGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25wildspells";
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
                return "def_s=9,6,3,4,5,4,8,6,9,8,5,3,8,8,7&cfgs=2017&ver=2&reel_set_size=4&def_sb=11,13,3,3,3&def_sa=7,4,4,4,11&scatters=&gmb=0,0,0&bg_i=50,3,200,4,1250,5&rt=d&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1;15~200,100,30,3,0~1,1,1,1,1;16~125,60,20,3,0~1,1,1,1,1;17~100,50,15,3,0~1,1,1,1,1&bonuses=0&fsbonus=&bg_i_mask=pw,ic,pw,ic,pw,ic&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,100,30,3,0;125,60,20,3,0;100,50,15,3,0;60,30,10,0,0;60,30,10,0,0;60,30,10,0,0;40,15,5,0,0;40,15,5,0,0;30,10,5,0,0;30,10,5,0,0;30,10,5,0,0;30,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=4,4,4,5,5,5,14,3,3,3,9,12,10,3,3,3,10,8,9,6,12,13,9,10,11,8,6,1,4,4,4,5,5,5,3,3,3,12,11,6,13,9,7,11,7,13,5,5,5,14,10,1,1,8,4,4,4,7,11~9,4,4,4,13,10,7,10,9,5,5,5,11,2,2,2,14,3,3,3,3,4,4,4,8,12,12,7,14,5,5,5,8,2,2,2,13,11,6,3,3,3,2,2,2,7,11,13,10,3,3,3,6,13,5,5,5,8,9,12,6,4,4,4,11,12~9,9,4,4,4,8,2,2,2,7,14,12,3,3,3,5,5,5,10,6,1,4,4,4,1,5,5,5,8,12,14,2,2,2,11,3,3,3,13,5,5,5,10,11,2,2,2,1,7,14,3,3,3,13,12,1,10,13,12,5,5,5,5,4,4,4,6,7,8~4,4,4,7,12,5,5,5,9,12,2,2,2,2,14,10,11,8,2,2,2,12,5,5,5,8,7,4,4,4,3,3,3,5,5,5,8,10,11,14,3,3,3,13,6,12,13,11,5,5,5,12,4,4,4,13,2,2,2,7,3,3,3,6,8,9,10~1,5,5,5,5,10,11,1,1,6,8,4,4,4,12,6,13,7,3,3,3,2,2,2,13,6,11,6,5,5,5,9,13,9,7,10,1,13,8,2,2,2,14,3,3,3,4,4,4,5,5,5,12,10,11,11,3,3,3,14,4,4,4,6,2,2,2,4&reel_set2=11,9,9,4,4,4,13,8,1,6,8,11,10,1,5,5,5,12,9,11,4,4,4,3,3,3,1,13,7,13,6,12,6,5,5,5,13,4,4,4,8,9,8,7,7,3,3,3,6,11,7,14,4,10,12,5,5,5,14,3,3,3,13,10,1~12,14,3,3,3,3,10,2,2,2,16,16,16,14,11,8,8,8,14,12,3,3,3,13,16,16,16,11,17,17,17,13,11,9,7,12,7,2,2,2,9,7,16,16,16,2,2,2,17,17,17,6,13,6,10,9,3,3,3,13,17,17,17,12,11,9,10~14,17,17,17,3,3,3,3,8,1,2,2,2,6,6,11,12,13,11,8,7,2,2,2,17,17,17,12,9,16,16,16,16,13,14,1,3,3,3,2,2,2,16,16,16,7,12,14,7,10,17,17,10,14,10,13,8,3,3,3,12,8,16,16,16,9,1~9,14,7,14,12,12,12,11,2,2,2,10,3,3,3,9,16,16,16,6,3,3,3,6,17,17,17,17,12,16,16,16,14,13,12,7,13,7,13,8,2,2,2,17,17,17,11,16,16,16,3,3,3,8,11,9,17,17,17,8,11,7,2,2,2,10~1,6,14,10,6,8,7,13,3,3,3,14,16,16,16,11,17,17,17,2,2,2,11,9,12,2,2,2,17,17,17,16,16,16,12,3,3,3,8,11,9,6,11,9,13,17,17,17,12,1,10,3,3,3,10,16,16,16,2,2,2,1,6,1&reel_set1=10,3,3,3,6,9,14,5,5,5,7,7,12,10,9,4,4,4,11,6,1,6,13,13,8,10,3,3,3,8,11,4,4,4,11,5,5,5,12,13,1,7,10,14,6,9,4,4,4,14,3,3,3,11,8,9,5,5,5,13,1~8,13,6,9,17,17,17,14,12,13,14,2,2,2,3,3,3,4,4,4,13,6,8,8,7,9,10,9,3,3,3,11,8,14,11,11,2,2,2,3,3,3,7,12,4,4,4,2,2,2,17,17,17,10,12,10,17,17,17,12,13,13,10,11~8,13,14,13,2,2,2,17,17,17,17,1,6,3,3,3,7,1,9,17,17,17,13,4,4,4,12,9,8,2,2,2,3,3,3,9,4,4,4,6,17,17,17,10,1,6,1,9,10,12,8,3,3,3,11,2,2,2,14,13,4,4,4,10,7,11,7~3,3,3,11,11,10,8,13,2,2,2,3,3,3,12,8,13,9,4,4,4,7,9,11,13,12,14,2,2,2,17,17,17,9,4,4,4,12,12,7,6,6,13,4,4,4,3,3,3,17,17,17,6,8,13,14,7,7,2,2,2,8,17,17,17,9~2,2,2,11,8,3,3,3,17,17,17,9,10,6,11,11,1,4,4,4,4,14,13,17,17,17,13,14,2,2,2,4,4,4,7,3,3,3,6,10,13,6,7,4,4,4,2,2,2,3,3,3,9,13,11,8,6,12,17,17,17,1,12,1,1,10&reel_set3=11,4,4,4,7,5,5,5,6,13,11,8,6,9,8,7,3,3,3,8,5,5,5,9,13,14,3,3,3,4,4,4,7,6,12,6,7,1,11,10,10,4,4,4,11,12,13,9,5,5,5,12,14,3,3,3,10,1,1,9,11,8,1~13,13,8,2,2,2,17,17,17,2,2,2,7,11,11,8,16,16,16,17,17,17,13,12,9,10,9,10,15,15,15,17,17,17,6,16,16,16,10,2,6,15,15,15,11,12,9,14,8,16,16,16,12,15,15,15,13,14,6,11,12,10~6,16,16,16,2,2,2,13,8,8,10,15,15,15,17,17,17,2,15,15,15,14,13,9,2,2,2,9,12,13,1,7,1,16,16,16,17,17,17,6,7,10,11,14,8,1,15,15,15,2,2,2,11,7,14,14,16,16,16,12,17,17,17~7,2,2,2,7,12,17,17,17,15,15,15,8,11,6,16,16,16,13,10,6,7,2,2,2,12,17,17,17,9,8,16,16,16,10,8,15,15,15,8,12,12,14,10,13,7,17,17,17,11,11,14,9,16,16,16,11,13,13,12,6,2,2,2~12,15,15,15,14,2,2,2,6,16,16,16,6,14,10,17,17,17,11,15,15,15,11,11,14,6,9,16,16,16,2,2,2,12,11,10,1,10,11,1,17,17,17,7,1,15,15,15,13,9,13,8,1,2,2,2,7,16,16,16,6,9,17,17,7";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 3; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202};
        }

        #endregion
        public WildSpellsGameLogic()
        {
            _gameID = GAMEID.WildSpells;
            GameName = "WildSpells";
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
