using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class GreatRhinoGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20rhino";
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
                return "def_s=6,9,8,4,7,7,3,4,11,11,10,11,6,5,10&cfgs=2529&ver=2&reel_set_size=2&def_sb=8,4,2,8,7&def_sa=5,7,3,3,3&bonusInit=[{bgid:0,bgt:26,bg_i:\"375,500\",bg_i_mask:\"psw,psw\"}]&scatters=1~0,0,2,0,0~0,0,10,0,0~1,1,1,1,1&gmb=0,0,0&bg_i=375,500&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~500,150,50,4,0~1,1,1,1,1;14~500,150,50,4,0~1,1,1,1,1&bonuses=0&fsbonus=&bg_i_mask=psw,psw&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,150,50,2,0;200,50,15,0,0;150,40,10,0,0;100,30,10,0,0;50,25,10,0,0;25,10,5,0,0;20,10,5,0,0;20,10,5,0,0;20,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&rtp=95.48&reel_set0=5,7,3,3,3,11,11,4,9,2,2,10,7,8,9,4,10,5,9,11,10,7,5,6,8,3,10,10,9~11,8,10,3,3,3,11,9,11,4,7,4,2,2,7,10,10,11,1,8,6,8,11,3,5,6,3~4,10,3,3,3,11,9,10,6,7,11,7,2,2,8,4,8,3,6,9,10,9,1,5,2,2,3,5,9,2,2,8,8~8,7,7,9,9,9,2,2,11,10,1,9,6,5,4,5,8,8,10,4,3,3,3,11,11,6,2,2,11~8,4,2,2,8,7,6,3,3,3,11,10,8,11,4,10,11,9,3,10,5,10,9,6,9,9,7,5,7&reel_set1=9,10,10,11,2,2,9,10,14,14,9,11,8,5,9,5,11,9,7,6,7,14,14,7,10,4~8,8,14,14,11,11,5,4,6,14,14,11,5,7,10,10,6,11,10,4,9,2,2,9~8,2,2,14,14,10,7,9,11,8,10,9,14,14,9,7,4,6,6,5,7,10~2,2,14,14,11,6,7,4,5,10,14,14,6,11,9,8,7,8,9,8,4,6,10~11,14,14,7,9,11,7,5,5,4,9,9,10,6,2,2,14,14,7,11,8,6,8,10,9";
            }
        }

        #endregion
        public GreatRhinoGameLogic()
        {
            _gameID = GAMEID.GreatRhino;
            GameName = "GreatRhino";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
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
