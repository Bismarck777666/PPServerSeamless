using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class BeoWulfGameLogic : BasePPSlotGame
    {

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40beowulf";
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
                return 40;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 40;
            }
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
                return "def_s=12,7,11,10,8,9,8,5,6,7,8,6,12,11,9,11,7,6,5,9&cfgs=2499&reel1=10,3,1,4,6,8,5,10,11,12,7,7,9,2,12,6,11,11&ver=2&reel0=1,7,7,7,9,9,9,6,6,10,3,3,3,3,9,11,12,12,12,5,8,12,11,4,12,7,5,3,4,8&def_sb=3,8,10,9,7&def_sa=1,7,7,7,9&reel3=6,5,7,3,12,9,1,10,3,9,11,2,12,4,6,8,8,4,12&reel2=1,5,3,4,5,7,10,10,10,11,11,11,7,6,12,12,12,12,3,9,2,10,8,6&reel4=3,8,10,9,7,12,10,6,12,8,11,1,5,7,12,8,4,11&scatters=1~25,5,1,0,0~20,15,10,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.10&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&ver=5&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2500,500,50,0,0;1500,400,40,0,0;800,300,30,0,0;500,200,25,0,0;400,150,20,0,0;300,100,15,0,0;200,75,10,0,0;100,50,10,0,0;75,25,5,0,0;50,15,5,0,0;0,0,0,0,0&rtp=94.00";
            }
        }
        #endregion

        public BeoWulfGameLogic()
        {
            _gameID = GAMEID.BeoWulf;
            GameName = "BeoWulf";
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total" };
            for(int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
    }
}
