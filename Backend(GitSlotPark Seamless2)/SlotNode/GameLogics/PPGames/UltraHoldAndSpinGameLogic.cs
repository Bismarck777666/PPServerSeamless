using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class UltraHoldAndSpinGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5ultra";
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
                return 5;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 5;
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
                return "def_s=9,3,11,6,6,11,5,9,11&cfgs=4119&ver=2&reel_set_size=2&def_sb=3,4,7&def_sa=8,7,5&bonusInit=[{bgid:0,bgt:48,mo_s:\"13,13,13,13,13,13,13,13,13,14,14,14,14,14,14,14,15,15,15,15,15\",mo_v:\"5,10,15,20,25,30,35,40,45,50,55,60,70,80,90,100,500,1000,1500,2000,2500\"}]&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&sc=40.00,80.00,120.00,200.00,300.00,400.00,1000.00,2000.00,4000.00,6000.00,8000.00,10000.00,20000.00&defc=400.00&wilds=2~250,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;250,0,0;150,0,0;100,0,0;80,0,0;80,0,0;20,0,0;20,0,0;20,0,0;20,0,0;0,0,0;0,0,0;0,0,0;0,0,0&rtp=95.65&reel_set0=8,9,5,11,11,11,2,7,3,8,8,8,10,11,10,10,10,6,4,9,9,9,5,9,11,9,10,9,3,2,11,10,6,9,10,7,10,6,9,11,2~3,6,0,0,0,11,11,8,2,6,6,6,4,9,9,9,9,7,11,11,11,11,5,6,8,8,8,3,3,6,0,0,0,11,11,10,10,10,10,2,10,2,7,3,4,8,3,4,7,3,9,3,3,6,0,0,0,11,11,10,8,3,11,6,9,10,6,7,3,9,8,10,9,6,8,6,2,6,8,10,7,3,10,8,7,3,10,7,4,7,8,3,7,10,3,4,10,7,4,9,6,8,6,7,9,8,10,8,7,4,9,2,7,8,9,8,9,8,10,9,7,11,4,8,9,8,10,11,4,8,9,4,9,5,10,8,9,10,2,9,11,5,9,4,11,3,6,0,0,0,11,11,6,7,8,2,10,2,9,2,10,9,8,6,10,6,7,4,6,7,2,9,10,2,4,9,7~11,11,11,9,7,3,4,5,10,8,11,9,9,9,6,2,8,8,8,10,10,10,8,4,8,9,2,4&reel_set1=11,8,8,8,7,5,9,9,9,6,10,11,11,11,2,10,10,10,3,4,9,8,10,8,10,4,8,6,4,8,10,3,8,4,10,3,7,3,9,7,3,6,8,6,8,6,9,3,5,3,9,6,3,10,6,4,3,9,10,6,4,8,9,8,9,2,8,2,3,7,8,5,8,7,8,6,7,8,6,5,8,10,4,10,8,6,8,10,7,3,9,3,6,7~11,9,5,8,8,8,7,6,9,9,9,4,11,11,11,2,3,10,10,10,10,8,6,6,3,6,0,0,0,11,11,2,6,8,8,10,6,10,6,10,6,6,3,2,4,3,6,10,6,8,4,6,8,6,9,3,6,3,6,0,0,0,11,11,8,10,8,6,2,6,4,6,8,10,6,8,3,9,6,10,8,8,6,6,10,2,9,6,8,3,4,6,10,4,8,10,8,10,8,5,8,6,10,10,9,8,9,6,3~8,8,8,6,7,11,2,4,9,9,9,9,8,11,11,11,3,10,10,10,5,10,2,9,2,3,9,3,9,11,10,9,6,11,9,3,9,11,9,3,10,2,9,3,4,10,9,10,9,11,9,2,9,4,10,9,4,9,6,5,9,4,10,9,10,11,6,9,2,11,10,11,9,11,2,3,6,11,2,10,2,10,9,3,4,10,9,11,6,2,4,11,9,3,9,2,11,10,3,11,9,11,9,10,9,10,11,9,7,10,9,4,9,11,9,10,6,9,10,6,10,11,2,10,11,9,10,2,6,3,9,11,5,9,10,9,10,11,2,3,10,7,10,11,2,11,10,9,10,2,4,9,11,2,9,4,5,10,6,10,6,11,2,11,3,10,2,11,3,11,3,10,9,10,6,9,7,11,6,7,10,11,9,2,9,11,9,6,10,9,4,6,4,6,11,7,3,9,3,9,6,3";
            }
        }
        #endregion
        public UltraHoldAndSpinGameLogic()
        {
            _gameID = GAMEID.UltraHoldAndSpin;
            GameName = "UltraHoldAndSpin";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "reel_set" };
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
