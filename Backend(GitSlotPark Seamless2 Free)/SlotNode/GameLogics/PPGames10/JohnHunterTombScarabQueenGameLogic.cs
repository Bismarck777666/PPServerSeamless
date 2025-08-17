using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class JohnHunterTombScarabQueenGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25scarabqueen";
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
            get { return 25; }
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
                return "msi=12&def_s=6,7,4,2,8,9,8,5,6,7,8,6,7,3,9&msr=17&cfgs=5166&ver=2&mo_s=11&reel_set_size=3&def_sb=10,11,9,6,8&mo_v=25,50,75,125,200,250,300,375,450,500,625,750,875&def_sa=7,5,4,4,3&scatters=1~0,0,1,0,0~0,0,8,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;800,50,10,0,0;175,30,5,0,0;150,25,5,0,0;125,25,5,0,0;100,20,5,0,0;100,10,5,0,0;100,10,5,0,0;100,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=5,7,6,4,10,9,4,5,7,6,9,7,11,11,11,11,10,8,4,7,6,9,5,10,6,8,3,4,10,5,7,9,6,10,4,9,5,8,9,6,10,3,7,6,10,8,9,10,6,9,5,3,10~7,2,2,2,2,2,4,8,5,9,1,10,6,9,4,7,8,11,11,11,11,10,6,9,4,8,3,9,5,10,4,9,1,7,5,9,3,8,5,4,9,8,3,7,4,8,5,9,6,10,1,8~3,2,2,2,2,2,8,5,9,1,10,4,8,7,3,8,6,5,8,4,7,1,8,5,9,6,8,9,2,2,2,8,10,6,8,5,7,1,8,6,9,11,11,11,11,8,6,3,9,5,8,10~10,2,2,2,2,5,6,4,7,10,3,7,11,11,11,11,9,5,7,3,10,1,7,6,9,4,7,8,9,3,7,4,10,6,7,1,8,7,3,9,5,7,8,6,7,9,6,7,4,5,8,3,10,4,9,1,7,3,5,8,6,9,7,4,10,3,9,1~7,2,2,6,4,3,9,5,8,6,4,7,6,10,7,6,12,9,4,5,7,6,10,5,7,4,10,6,7,4,9,6,8,3,6,9,5,3,10,6,7,5,4,8,9,6,10,4,7,6,8,4,9,12,10,6,7,4,10,9,5,6,10,8,4,7,8,10,4,8,5,9,6,10,9,4,7,8&reel_set2=18,18,18,18~18,18,18,18~18,18,18,18~18,18,18,18~7,2,2,2,4,8,6,10,3,9,5,12,3,4,7,6,10,9,5,10,7,6,9,4,3,7,6,5,10,7,12,10,6,7,4,9,6,8,7,6,9,5,3,10,6,7,5,3,8,9,12,10,4,7,3,9,4,8,9,4,10,6,5,3,10,9,5,6,9&reel_set1=5,7,6,3,10,9,4,5,7,6,9,7,11,11,11,11,11,9,5,10,6,8,3,4,10,5,7,9,6,10,4,9,5,8,9,6,10,3,7,6,8,10,9,6,11,11,11,11,11,7,4,10~7,2,2,2,2,2,2,2,2,2,3,8,5,9,1,10,6,9,4,11,11,11,11,11,5,10,6,9,4,8,3,9,5,10,4,9,1,7,5,9,11,11,11,11,11,8,3,7,4,8,5,9,6,10,1,8~8,2,2,2,2,2,2,2,2,2,2,2,5,9,1,10,4,8,7,9,8,6,5,8,4,7,1,8,5,9,11,11,11,11,11,4,10,8,6,5,8,7,1,8,6,9,11,11,11,11,11,6,8,9,5,8,3~10,2,2,2,2,2,2,2,2,2,2,2,2,2,2,10,3,7,11,11,11,11,11,5,7,3,10,1,7,6,9,4,7,8,9,3,7,4,10,6,7,1,8,7,3,9,5,7,8,6,11,11,11,11,11,11,5,8,10,4,9,1,7,3,5,8,6,9,7,4,10,3,9,1~7,2,2,8,6,10,3,9,5,12,3,4,7,6,10,7,6,12,9,4,8,7,6,12,5,7,4,10,6,7,12,9,6,8,3,6,9,5,4,10,6,7,12,4,8,9,6,10,12,7,6,8,4,9,12,10,6,7,3,10,9,12,6,10,4,8,7,12,10,4,8,5,9,6,12,10,4,7,8";
            }
        }
	
	
        #endregion
        public JohnHunterTombScarabQueenGameLogic()
        {
            _gameID = GAMEID.JohnHunterTombScarabQueen;
            GameName = "JohnHunterTombScarabQueen";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            double temp = 0.0;
            if (dicParams.ContainsKey("mo_tw") && double.TryParse(dicParams["mo_tw"], out temp))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

            if (dicParams.ContainsKey("prg"))
            {
                string[] strParts = dicParams["prg"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length == 2)
                {
                    strParts[1] = convertWinByBet(strParts[1], currentBet);
                    dicParams["prg"] = string.Join(",", strParts);
                }
            }
        }
	
    }
}
