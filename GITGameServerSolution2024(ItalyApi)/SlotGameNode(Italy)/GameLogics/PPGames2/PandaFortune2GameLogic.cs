using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class PandaFortune2GameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25pandatemple";
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
                return "def_s=6,7,4,3,8,4,3,5,6,7,8,5,7,3,4&cfgs=4894&ver=2&def_sb=4,13,4,7,8&reel_set_size=3&def_sa=10,3,5,3,7&balance_bonus=0.00&scatters=1~100,15,2,0,0~12,12,12,0,0~1,1,1,1,1&gmb=0,0,0&bg_i=3,0,2,1,1,2,2,3,1,4,3,5,2,6,1,7,2,8,1,9,5,10,15,20,25,50,75,100,150,200,250,500,1000,2500,4998,10,5,10,15,20,25,50,75,100,150,200,250,500,1000,2500,4998,11&rt=d&gameInfo={rtps:{regular:\"94.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"857142\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=0.01,0.02,0.03,0.04,0.05,0.08,0.10,0.20,0.30,0.40,0.50,0.75,1.00,2.00,3.00,4.00,5.00&defc=0.08&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&bg_i_mask=pw,ic,pw,ic,pw,ic,pw,ic,pw,ic,pw,ic,pw,ic,pw,ic,pw,ic,pw,ic,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,ic,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,pw,ic&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,50,25,0,0;150,50,10,0,0;100,20,5,0,0;100,20,5,0,0;100,20,5,0,0;50,15,5,0,0;50,15,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0&rtp=94.5&reel_set0=6,12,4,10,8,12,10,12,8,10,8,10,8,10,8,10,12,8,10,8,12,10,12,10,12~7,11,13,5,9,3,11,3,11,13,11,3,11,13,9~8,5,11,13,3,9,7,6,10,12,4,3,12,6,5,6,4,6,10,3,12,5,12,11,13,5~9,13,12,10,6,4,11,8,3,5,7,13,5,13,10,5,6,8,5~4,13,7,5,3,1,8,2,11,6,10,12,9,11,2,7,5,9,10,12,10,12,11,7,11&reel_set2=10,8,3,6,11,9,4,5,13,7,12,1,6,1,12,7,12,7,9,6,4,7,6,1,8,6,4,7,8,7,6,8,9,6~2,2,2,10,13,6,11,2,9,7,1,5,4,3,12,8,1,6,13,6,11,8,13,1,8,6,7,4,7,12,10,6,11,10,1,7,5~13,8,2,2,2,10,7,1,12,2,5,11,3,9,6,4,11,10,9,11,10~2,2,2,2,11,10,12,8,6,3,7,9,4,1,5,13,8,6,9,5,7,13,9,6,11,13,10,13,4,8,1,10,5,13,3,7,8,5,1,6,1,7,5,10,1~8,12,6,2,2,2,9,1,5,2,13,4,7,3,11,10,11,2,6,12,4,6,4,2,9,6,2,6,2,6,10,2,6,4,9,2,4,9,4,9,4,12,2,12,3,12,2,4,12,2,6,12,2&reel_set1=13,8,3,7,4,9,12,5,11,6,10,1,5,10,1,5,10,6,10,12,5,6~6,1,10,5,7,2,2,2,12,13,8,4,3,9,2,11,9,7,5,10,5,7,2,5,1,10,9,2,4,1,9,4,2,11~2,2,2,8,7,11,3,6,1,2,4,10,5,12,9,13,9,12,8~11,1,2,8,2,2,2,5,10,3,13,6,9,7,4,12,2,7,9,2,7,3,7,2,7,8~13,4,3,11,12,5,6,2,8,7,9,10,1,8,5,1,9,7,9,10,12,4,11,1,10,4,8,5,8";
            }
        }
        #endregion
        public PandaFortune2GameLogic()
        {
            _gameID = GAMEID.PandaFortune2;
            GameName = "PandasFortune2";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, int currency, double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams, currency, userBalance, index, counter);
            dicParams.Add("reel_set", "0");
        }
    }
}
