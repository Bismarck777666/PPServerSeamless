using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class AztecGemsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5aztecgems";
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
                return 5;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
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
                return "def_s=7,4,9,7,4,9,7,4,9&cfgs=2527&reel1=4,4,4,9,9,9,7,7,7,7,7,7,2,2,2,8,8,8,9,7,7,4,7,7,5,5,5,5,5,6,6,6,9,2,3,3,3,9,9,9,8,5,8,9,5,3,6,4&ver=2&reel0=8,8,8,8,7,7,7,6,6,6,6,6,8,8,4,4,4,4,8,6,5,5,5,7,8,8,6,9,9,9,9,9,8,6,8,4,3,3,3,3,9,8,9,4,3,2,2,2,9,5,2,2,6,6,7&def_sb=4,4,4&def_sa=8,8,8&reel2=4,4,4,4,8,8,8,9,9,9,9,7,7,7,7,5,5,5,3,3,3,7,7,3,8,9,5,3,8,4,9,7,9,5,5,9,9,6,6,6,6,7,2,2,2,9,2&aw=3&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&base_aw=m~1;m~2;m~3;m~5;m~10;m~15&sc=40.00,80.00,120.00,200.00,300.00,400.00,1000.00,2000.00,4000.00,6000.00,8000.00,10000.00,20000.00&defc=400.00&def_aw=3&wilds=2~25,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;20,0,0;15,0,0;12,0,0;10,0,0;8,0,0;5,0,0;2,0,0&rtp=94.52&awt=6rl";
            }
        }
        #endregion
        public AztecGemsGameLogic()
        {
            _gameID = GAMEID.AztecGems;
            GameName = "AztecGems";
        }        
    }
}
