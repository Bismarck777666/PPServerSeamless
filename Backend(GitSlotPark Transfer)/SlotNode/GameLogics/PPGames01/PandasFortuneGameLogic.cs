using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class PandasFortuneGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25pandagold";
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
                return "msi=14&def_s=6,7,4,3,8,9,3,5,6,7,8,5,7,3,9&cfgs=2525&ver=2&reel_set_size=2&def_sb=10,3,2,2,2&def_sa=6,10,5,8,10&scatters=1~100,15,2,0,0~15,10,8,0,0~1,1,1,1,1&gmb=0,0,0&bg_i=800,0,200,1,25,2&rt=d&sc=10.00,20.00,50.00,100.00,250.00,500.00,1000.00,3000.00,4000.00&defc=100.00&sh=3&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&bg_i_mask=pw,ic,pw,ic,pw,ic&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,50,25,0,0;150,50,10,0,0;100,20,5,0,0;100,20,5,0,0;100,20,5,0,0;50,15,5,0,0;50,15,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;0,0,0,0,0&rtp=96.17&reel_set0=11,6,13,8,8,9,1,4,10,3,10,9,6,12,13,5,11,5,7,12~7,3,7,11,13,10,12,6,2,2,2,4,8,1,9,7,5,2,12,4~6,7,3,11,9,1,9,9,12,4,5,2,2,2,2,2,13,8,6,4,11,10~10,2,2,2,2,4,2,8,13,10,7,6,13,2,8,11,11,13,4,12,5,10,1,3,12,9~10,12,2,2,2,2,2,2,2,2,4,5,10,8,13,8,6,9,1,9,11,11,2,3,1,3,7,12&reel_set1=6,12,8,1,7,8,11,10,10,12,9,11,3,9,5,4,5,13~7,9,13,10,2,2,2,2,2,14,14,14,14,14,6,7,11,12,5,12,3,14,8,1,2,4,14,4,14,2~2,2,2,2,2,2,2,1,2,6,2,14,14,14,4,8,9,14,2,4,11,13,12,11,10,9,2,5,2,3,2,7,2,6,9,14,7~9,13,6,10,10,2,2,2,2,2,2,2,14,14,14,14,14,14,14,14,14,14,14,14,11,14,14,12,13,12,2,14,4,5,3,14,11,2,8,13,4,14,10,1,1,7~8,10,1,3,9,2,2,2,2,2,2,2,14,14,14,14,14,14,14,14,2,2,3,10,5,12,4,1,2,9,8,13,6,12,11,2,7,14,14";
            }
        }
        #endregion
        public PandasFortuneGameLogic()
        {
            _gameID = GAMEID.PandasFortune;
            GameName = "PandasFortune";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams.Add("n_reel_set", "0");
        }
    }
}
