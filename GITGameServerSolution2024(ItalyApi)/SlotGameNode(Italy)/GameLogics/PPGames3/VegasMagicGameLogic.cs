using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class VegasMagicGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20vegasmagic";
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
                return "def_s=7,9,1,3,7,8,4,11,1,9,8,11,11,10,9&cfgs=2435&reel1=3,2,10,4,11,8,8,8,5,6,7,8,9,9,9,7,4,5,11,11,11,3,6,7,8,10,10,10,1,9,7,8&ver=2&reel0=4,5,11,11,11,3,6,7,8,10,10,10,1,9,7,8,3,10,4,11,8,8,8,5,6,7,8,9,9,9,7&def_sb=8,1,11,10,9&def_sa=11,9,7,4,11&reel3=10,10,10,1,9,7,8,3,2,10,4,11,8,8,8,5,6,7,8,9,9,9,7,4,5,11,11,11,3,6,7,8&reel2=8,9,9,9,7,4,5,11,11,11,3,6,7,8,10,10,10,1,9,7,8,3,2,10,4,11,8,8,8,5,6,7&reel4=6,7,8,9,9,9,7,4,5,11,11,11,3,6,7,8,10,10,10,1,9,7,8,3,10,4,11,8,8,8,5&scatters=1~500,50,5,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.10&wilds=2~0,0,0,0,0~1,1,1,1,1;12~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;3000,150,25,5,0;250,75,20,0,0;250,75,20,0,0;150,50,10,0,0;150,50,10,0,0;75,25,5,0,0;75,25,5,0,0;50,15,5,0,0;50,15,5,0,0;0,0,0,0,0&rtp=94.07";
            }
        }
        #endregion
        public VegasMagicGameLogic()
        {
            _gameID = GAMEID.VegasMagic;
            GameName = "VegasMagic";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams,int currency,double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams, currency, userBalance, index, counter);
        }
    }
}
