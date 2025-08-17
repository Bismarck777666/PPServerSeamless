using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class ElementalGemsMegaGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswayselements";
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
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 8;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,8,7,6,8,7,6,8,7,6,8,7,9,8,7,9,9,7,9,9,9,9,9,9&cfgs=5092&reel1=7,8,8,4,8,8,8,7,8,3,4,5,7,7,7,8,7,8,5,8,3,3,3,7,8,6,7,7,6,6,6,8,8,3,2,8,4,4,4,8,7,8,3,8,5,5,5,6,8,4,8,6,8&ver=2&reel0=6,8,7,6,6,6,6,3,6,6,6,6,6,8,8,6,8,8,7,6,6,7,2,6,6,8,8,8,8,8,6,8,8,6,8,8,3,6,6,8,3,3,3,6,8,4,6,8,6,6,6,5,6,5,5,5,5,4,3,6,6,8,6,6,4,6,8,8,8,4,4,4,8,8,8,8,6,6,6,4,6,8,5,6,6&def_sb=4,7,8&def_sa=5,3,7&reel2=7,7,7,7,2,5,5,5,5,4,4,4,6,4,3,3,3,8,8,8,8,5,7,6,6,6,5,3,7&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"968992\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~50,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;30,0,0;20,0,0;12,0,0;5,0,0;4,0,0;2,0,0;0,0,0&t=243";
            }
        }
	
	
        #endregion
        public ElementalGemsMegaGameLogic()
        {
            _gameID = GAMEID.ElementalGemsMega;
            GameName = "ElementalGemsMega";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
	
    }
}
