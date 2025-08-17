using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class UltraBurnGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5ultrab";
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
                return "def_s=10,3,8,7,9,8,7,5,5&cfgs=3849&reel1=10,10,10,9,9,9,5,5,5,3,3,3,7,7,7,4,4,4,7,10,10,6,6,6,8,8,8,5,4,5,9,8&ver=2&reel0=3,3,3,9,9,9,5,5,5,4,4,4,7,7,7,9,6,6,6,10,10,10,8,8,8,6,10,10,7&def_sb=3,6,3&def_sa=3,5,3&reel2=3,3,3,9,9,9,9,9,6,6,6,4,4,4,3,10,8,8,8,5,7,7,7,10,5,5,5,10,6&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&sc=0.01,0.02,0.05,0.08,0.10,0.15,0.25,0.50,0.75,1.00,2.00,3.00,5.00,10.00,15.00,20.00,25.00,30.00,40.00,50.00&defc=0.25&wilds=2~0,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;500,0,0;50,0,0;50,0,0;20,0,0;20,0,0;20,0,0;20,5,0;5,0,0&rtp=95.63";
            }
        }
        #endregion

        public UltraBurnGameLogic()
        {
            _gameID  = GAMEID.UltraBurn;
            GameName = "UltraBurn";
        }
    }
}
