using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class LuckyNewYearGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25newyear";
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
                return 25;
            }
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
                return "def_s=6,7,4,2,8,9,8,5,6,7,8,6,7,3,9&cfgs=3228&ver=2&mo_s=11&reel_set_size=2&def_sb=10,7,5,9,10&mo_v=25,50,75,100,125,150,175,200,250,350,400,450,500,600,750,2500&def_sa=8,7,5,4,7&mo_jp=750;2500;25000&scatters=1~1,1,1,0,0~0,0,5,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&mo_jp_mask=jp3;jp2;jp1&sc=10.00,20.00,40.00,60.00,80.00,100.00,200.00,400.00,800.00,1000.00,2000.00,3000.00,4000.00&defc=100.00&wilds=2~500,250,25,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,250,25,0,0;400,150,20,0,0;300,100,15,0,0;200,50,10,0,0;50,20,10,0,0;50,20,5,0,0;50,20,5,0,0;50,20,5,0,0;0,0,0,0,0;0,0,0,0,0&rtp=96.00&reel_set0=7,4,6,1,10,10,9,2,2,2,5,6,3,6,9,11,11,11,7,8,5,1,10,5,6,8,2,4,10~8,6,9,2,2,2,7,9,10,5,4,9,6,4,5,8,10,11,11,11,3,7,9~9,2,2,2,4,3,5,8,11,11,11,7,4,5,8,10,8,6,2,7,5,2,1,3,10,8~11,11,10,5,7,4,7,7,8,9,2,2,2,7,5,3,2,3,4,10,3,8,4,6,10,6,6,7~3,8,4,7,1,10,3,2,2,2,5,6,1,6,4,6,7,4,10,7,11,11,11,9,3,7,10,6,8,5,9,10,5&reel_set1=4,3,5,10,6,4,6,2,2,2,4,6,7,5,8,3,6,9~5,5,5,10,10,10,3,3,3,1,1,1,4,4,4,7,9,9,9,6,6,6,11,11,11,6,7,7,7,3,4,2,2,2,8,8,8,3~2,2,2,8,8,8,3,5,5,5,4,4,4,7,9,9,9,6,6,6,11,11,11,6,7,7,7,3,4,10,10,10,3,3,3,1,1,1~1,1,1,8,8,8,3,5,5,5,4,4,4,7,9,9,9,6,6,6,11,11,11,3,4,10,10,10,3,3,3,2,2,2,6,7,7,7~5,1,6,9,2,2,2,6,8,10,3,3,4,7,6,5,4";
            }
        }
        #endregion
        public LuckyNewYearGameLogic()
        {
            _gameID = GAMEID.LuckyNewYear;
            GameName = "LuckyNewYear";
        }

        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rwd"))
                dicParams["rwd"] = convertWinByBet(dicParams["rwd"], currentBet);

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
            return resultParams;
        }
    }
}
