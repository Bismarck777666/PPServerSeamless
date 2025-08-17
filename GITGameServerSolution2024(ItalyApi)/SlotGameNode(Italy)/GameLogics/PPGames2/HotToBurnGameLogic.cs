using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class HotToBurnGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5hotburn";
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
                return "def_s=7,7,5,9,3,4,7,5,9,8,4,7,6,8,6&cfgs=3744&reel1=3,8,4,3,4,9,1,6,6,6,7,7,7,5,3,8,8,8,9,9,9,1,7,4,7,5,8&ver=2&reel0=5,1,8,8,8,4,7,7,7,3,6,6,6,3,5,1,9,9,9,4,7,6,5&def_sb=8,6,6,9,6&def_sa=7,5,9,9,9&reel3=3,8,4,3,4,9,1,6,6,6,7,7,7,5,3,8,8,8,9,9,9,1,7,4,7,5,8&reel2=3,8,4,3,4,9,1,6,6,6,7,7,7,5,3,8,8,8,9,9,9,1,7,4,7,5,8&reel4=3,8,4,3,4,9,1,6,6,6,7,7,7,5,3,8,8,8,9,9,9,1,7,4,7,5,8&scatters=1~50,10,2,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=0.01,0.02,0.05,0.08,0.10,0.15,0.25,0.50,0.75,1.00,2.00,3.00,5.00,10.00,15.00,20.00&defc=0.05&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,1000,100,0,0;500,200,50,0,0;500,200,50,0,0;200,50,20,0,0;200,50,20,0,0;200,50,20,0,0;200,50,20,5,0&rtp=95.67";
            }
        }
        #endregion

        public HotToBurnGameLogic()
        {
            _gameID  = GAMEID.HotToBurn;
            GameName = "HotToBurn";
        }
    }
}
