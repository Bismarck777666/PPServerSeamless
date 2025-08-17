using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class GreatRhinoDeluxGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20rhinoluxe";
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
                return "def_s=8,6,9,6,10,10,7,6,9,11,5,11,5,5,4&cfgs=4008&ver=2&reel_set_size=2&def_sb=11,2,10,9,9&def_sa=8,1,1,7,8&bonusInit=[{bgid:0,bgt:26,bg_i:\"375,500\",bg_i_mask:\"psw,psw\"},{bgid:1,bgt:47,bg_i:\"375,500\",bg_i_mask:\"psw,psw\"}]&scatters=1~0,0,2,0,0~0,0,10,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=0.01,0.02,0.03,0.04,0.05,0.10,0.20,0.30,0.40,0.50,0.75,1.00,2.00,3.00,4.00,5.00&defc=0.10&wilds=2~500,150,50,4,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,150,50,2,0;200,50,15,0,0;150,40,10,0,0;100,30,10,0,0;50,25,10,0,0;25,10,5,0,0;20,10,5,0,0;20,10,5,0,0;20,10,5,0,0;0,0,0,0,0&rtp=94.54&reel_set0=10,9,3,3,11,9,7,5,8,10,2,2,11,9,6,8,10,5,7,11,9,4,5,10,9,6,11,10,3,3,11,4,7,9,11,5,10,9,7,6,8,9,2,2,10,11,5,10,9,4,5,10,9,7,3,3,3,11,10~10,11,4,7,8,6,7,11,1,10,9,2,2,10,11,4,5,8,7,6,10,11,1,8,7,4,11,3,3,3,10,11,4,6,8,7,5,11,1,9,11,6,4,11,8,3,3,3,11,6,5,8,11,1~4,7,8,9,6,5,10,8,1,11,6,7,10,2,2,2,8,3,3,3,9,10,1,8,6,5,9,7,4,8,10,6,7,10,8,6,9,10,5,4,9,1,8,10,7,6,8,11,5,4,8,7,6,9,3,3,3,8~2,2,2,8,7,6,9,5,7,8,11,1,9,8,6,7,11,5,4,8,3,3,11,8,1,9,11,7,6,8,10,5,9,11,5,7,9,8,4,5,11,9,8,11,5,6,10,4,7,11,8,3,3,3,9,11~7,8,9,4,6,10,9,5,8,11,7,6,9,8,7,10,11,4,10,9,3,3,3,8,9,7,4,10,8,7,11,9,6,10,7,11,9,2,2,8,11,7,5,9,10,7,11,9,6,7,10,9,4,8,9&accInit=[{id:0,mask:\"cp; tp; s; sp\"}]&reel_set1=10,9,3,3,11,9,7,5,8,10,8,4,11,9,6,8,10,5,2,2,9,4,5,10,9,6,11,10,6,5,11,4,7,9,11,5,10,9,7,6,8,9,4,7,10,11,5,10,9,4,5,10,9,3,3,3,7,11,10~10,11,4,7,8,6,7,11,9,8,9,4,10,7,11,4,5,8,7,6,10,11,6,8,7,4,11,3,3,2,2,2,4,6,8,7,5,11,10,9,11,6,4,11,8,3,3,3,11,6,5,8,11,8~4,7,8,9,6,5,10,8,10,11,6,7,10,2,2,9,8,3,3,3,9,10,7,8,6,5,9,7,4,8,10,6,7,10,11,6,9,10,5,4,9,7,8,10,7,6,8,2,2,5,8,7,6,9,4,3,5,8~2,2,11,8,7,6,9,5,7,8,11,8,9,8,6,7,11,5,4,6,2,2,11,8,5,9,11,7,6,8,10,5,9,11,5,7,9,2,2,2,11,9,8,11,5,6,10,4,7,2,8,3,3,3,9,11~7,8,9,4,6,10,9,5,8,11,7,6,9,8,2,2,11,4,10,9,3,3,5,8,9,7,4,10,8,7,11,9,6,10,7,11,9,2,2,2,11,7,5,9,10,7,11,9,6,7,10,9,4,8,9";
            }
        }

        #endregion
        public GreatRhinoDeluxGameLogic()
        {
            _gameID     = GAMEID.GreatRhinoDelux;
            GameName    = "GreatRhinoDelux";
        }
        
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, int currency, double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams, currency, userBalance, index, counter);
            dicParams["reel_set"] =  "0";
        }
        
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
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
