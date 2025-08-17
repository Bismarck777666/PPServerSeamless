using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class AsgardGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25asgard";
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
                return "msi=12&def_s=10,5,9,3,9,3,6,11,11,6,9,8,5,5,10&cfgs=2214&ver=2&reel_set_size=6&def_sb=6,7,7,9,7&def_sa=8,10,4,10,3&scatters=1~0,0,2,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&base_aw=n;tt~nlf;tt~msf;tt~rrf;tt~rwf&cpri=1&sc=10.00,20.00,50.00,100.00,250.00,500.00,1000.00,3000.00,5000.00&defc=100.00&wilds=2~500,100,25,0,0~1,1,1,1,1;13~500,100,25,0,0~1,1,1,1,1;14~500,100,25,0,0~1,1,1,1,1;15~500,100,25,0,0~1,1,1,1,1;16~500,100,25,0,0~1,1,1,1,1;17~500,100,25,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,75,15,0,0;200,60,15,0,0;150,50,15,0,0;100,40,15,0,0;50,20,10,0,0;50,20,10,0,0;50,20,10,0,0;50,20,5,0,0;50,20,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&rtp=95.47&reel_set0=11,10,7,9,4,5,11,10,6,4,8,7,5,3,8,9,2,2,10,6~3,8,6,9,7,4,8,10,1,9,4,11,5,10,6,11,2,2,7,5~4,9,11,5,6,8,1,9,11,5,10,7,4,8,6,3,2,2,7,10~10,3,11,5,9,6,5,10,1,8,3,7,9,4,8,7,2,2,6,11~3,9,6,10,7,4,8,5,11,3,8,4,9,5,11,7,2,2,10,6&reel_set2=5,2,9,8,4,12,12,12,11,6,12,12,12,3,8,12,12,12,10,7~12,12,12,5,4,6,12,12,12,7,2,11,3,5,12,12,12,10,8,9~11,12,12,12,4,5,9,12,12,12,3,10,5,12,12,12,7,6,2,8~8,11,12,12,12,10,9,2,5,10,6,12,12,12,7,3,12,12,12,4~3,2,10,4,12,12,12,7,5,11,9,12,12,12,6,8,12,12,12,5&reel_set1=5,4,13,17,6,4,3,5,4,16,3,5,6,15,14,4,6,5,3,6~6,3,14,13,5,3,4,15,17,6,4,3,6,5,16,13,5,6,4,5~3,4,6,17,4,5,3,15,13,3,6,4,5,3,6,5,14,16,5,6~15,16,5,4,6,5,3,6,14,13,6,4,5,3,6,4,13,17,3,4~3,6,5,4,6,4,5,16,15,4,3,6,4,14,17,3,5,6,13,5&reel_set4=8,4,11,9,5,7,10,3,9,4,7,11,6,8,9,3,6,8,10,5~10,9,6,5,10,8,4,11,3,7,11,3,10,9,4,11,6,8,7,5~9,4,9,3,7,8,5,10,11,6,3,11,9,4,10,5,7,8,6,7~8,3,7,11,5,3,9,4,8,10,3,6,9,4,11,10,6,5,7,11~3,4,8,6,5,7,9,4,9,11,6,10,5,11,4,7,8,3,10,6&reel_set3=8,4,5,9,2,8,10,3,11,7,6,8,4,10,7,6,5,11,2,9~6,4,10,5,2,9,7,10,11,6,8,3,5,7,9,8,5,2,11,4~9,4,5,10,7,4,9,2,3,10,11,6,8,6,8,9,5,11,7,2~8,7,4,10,6,2,11,9,5,4,10,3,7,10,5,9,11,3,8,6~3,7,8,6,9,5,4,10,5,8,3,11,6,5,10,2,11,4,7,9&reel_set5=5,4,2,2,6,4,3,5,4,6,3,5,6,2,2,4,6,5,3,6~6,3,2,6,5,3,4,2,2,6,4,3,6,5,2,2,5,6,4,5~3,4,6,2,4,5,3,2,2,3,6,4,5,3,6,5,2,2,5,6~2,2,5,4,6,5,3,6,2,2,6,4,5,3,6,4,2,3,6,4~3,6,5,4,6,4,5,2,2,4,3,6,4,2,2,3,5,6,4,5&cpri_mask=tbw&awt=rsf";
            }
        }
        #endregion
        public AsgardGameLogic()
        {
            _gameID = GAMEID.Asgard;
            GameName = "Asgard";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams.Add("reel_set", "0");
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul","fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total","reel_set",
                "s", "purtr", "w", "tw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;

                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
    }
}
