using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class BiggerBassBonanzaGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs12bbb";
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
                return 12;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 12;
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
                return "def_s=5,6,9,6,6,8,11,8,9,9,6,9,12,6,6,6,9,12,6,6&cfgs=5218&ver=2&mo_s=7&def_sb=10,7,7,8,11&mo_v=24,60,120,180,240,300,600,48000&reel_set_size=4&def_sa=11,10,7,10,4&prg_cfg_m=s&scatters=1~0,0,0,0,0~20,15,10,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"1021346\",max_rnd_win:\"4000\"}}&wl_i=tbm~4000&prg_cfg=2&sc=20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00,6000.00,7000.00,8000.00&defc=200.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2400,240,60,6,0;1200,180,36,0,0;600,120,24,0,0;600,120,24,0,0;0,60,12,0,0;120,30,6,0,0;120,30,6,0,0;120,30,6,0,0;120,30,6,0,0;120,30,6,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&rtp=94.62&reel_set0=6,11,5,4,7,7,5,6,12,3,9,8,11,7,9,1,7,8,6,7,8,8,10,5,8,7,7,7,7,11,11,4,9,5,7,6,5,4,7,3,11,7,11,7,9,3,12,8,1,9,9,11,6,6,10,9,4,9~3,1,7,12,7,5,8,10,7,6,5,10,11,5,12,10,6,8,7,8,4,7,7,7,8,12,6,10,3,9,7,12,6,7,11,8,12,9,10,11,4,4,7,5,3,6,7,5~4,5,11,4,4,12,5,7,7,8,11,6,7,6,10,10,3,5,10,8,8,7,12,7,7,7,6,7,10,7,9,10,7,1,9,4,9,12,9,11,5,7,3,8,6,3,12,11,9,3,6~9,9,8,7,6,1,7,8,7,3,3,7,5,6,8,11,8,10,6,7,7,7,7,12,5,4,12,11,6,12,4,5,7,11,9,10,10,9,6,7,4,6,9,5~8,11,8,4,12,5,10,7,10,7,7,7,4,5,7,12,9,11,1,6,3,7,6,9&accInit=[{id:0,mask:\"cp\"},{id:2,mask:\"cp; mp\"},{id:3,mask:\"cp; mp\"}]&reel_set2=12,10,7,4,7,10,7,10,6,7,4,3,7,12,12,10,6,4,7,7,7,10,7,4,6,6,10,9,7,7,3,6,3,6,7,6,12,12,7,9,12,6~11,10,1,7,4,7,7,7,7,11,8,1,5,7,5~6,8,5,11,6,6,9,3,3,12,8,11,5,1,1,12,9~7,12,10,6,6,11,6,9,6,7,4,9,6,6,7,7,6,11,10,12,7,7,7,4,7,5,12,12,8,3,9,4,7,7,5,7,8,8,3,9,8,8,5,9,10,11~6,8,11,5,7,10,9,12,9,7,4,7,7,7,3,9,4,10,6,7,8,6,5,7,11,12,6&reel_set1=7,1,7,4,4,8,4,5,11,1,7,5,8,10,7,1,8,7,7,7,7,7,10,7,8,4,8,7,11,5,1,7,7,11,1,11,11,7,8,11,11,1,5~9,10,9,6,10,12,4,6,4,7,6,7,10,12,10,6,7,7,7,7,6,7,7,10,10,12,6,3,7,12,7,4,7,6,3,4,12,7~12,8,9,9,12,9,9,6,12,3,11,6,6,8,8,11,6,6,5,3,9,12,8,6,11,5,3,6,5,3,9,6,11,5,8,11~5,9,10,6,4,8,10,8,11,1,5,11,9,12,7,7,7,1,8,7,12,6,7,7,12,4,6,3,7,1,1,6,7,9~11,8,7,5,10,7,9,6,9,6,3,7,11,10,7,7,7,11,6,4,10,6,9,7,6,5,7,12,8,12,8,12,4&reel_set3=10,12,12,8,10,7,12,10,7,8,11,8,8,11,12,7,7,7,7,4,3,6,12,6,10,4,10,10,8,9,12,7,8,5,12,10,8,9~11,7,3,4,11,10,5,6,11,8,9,9,7,9,12~7,7,12,6,6,11,8,10,5,5,4,4,9,3~4,5,7,4,5,3,7,7,6,3,6,6,8,5,10,11,9,12~6,4,11,7,10,5,10,7,5,8,7,12,7,12,3,9,11,9,7,4,8,6";
            }
        }
        #endregion

        public BiggerBassBonanzaGameLogic()
        {
            _gameID = GAMEID.BiggerBassBonanza;
            GameName = "BiggerBassBonanza";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
        }

    }
}
