using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BountyGoldGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25btygold";
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
                return 12;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,10,2,3,10,6,7,8,4,7,4,10,4,5,11,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13&cfgs=4712&accm=cp~tp~lvl~sc&ver=2&mo_s=11;12&acci=0&mo_v=25,50,75,100,125,150,175,200,250,375,500,625,1250,1875,2500,6250;250,375,500,625,1250,2500&reel_set_size=5&accv=0~6~0~0&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"871840\",jp1:\"25\",max_rnd_win:\"5000\",jp3:\"500\",jp2:\"50\",jp4:\"5000\"}}&wl_i=tbm~5000&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,50,10,0,0;150,30,5,0,0;125,25,5,0,0;75,25,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=7,8,6,8,5,6,4,8,10,9,7,10,11,10,3,6,8,9,11,9,4,9,4,7,3,10,6,7,5,6,11,6,7,10,9,11,10,4,5,7,5,4,7,4,10,9,10,6,8,5,10,9,10,5,8,6~10,4,8,4,6,10,3,5,4,10,11,7,9,7,6,2,2,2,2,2,6,8,4,9,2,8,5,2,11,3,9,3,4,5,10,9,5,10,11,11,11,2,2,9,3,7,9,7,8,3,4,7,2,8,11,6,9,8,2,11~8,7,3,8,9,6,5,8,2,5,4,3,10,8,2,2,2,6,8,7,3,9,2,6,9,4,7,10,2,4,4,11,11,11,6,7,11,11,6,9,5,8,2,10,3,2,9,8,5~11,2,5,9,10,8,5,8,6,2,8,7,5,7,11,6,10,8,2,2,2,2,2,7,9,11,7,8,7,2,7,8,7,11,6,7,10,3,4,8,9,11,11,11,6,5,4,5,10,9,7,3,7,9,4,6,3,9,11,4,3,9,4,3~5,11,10,2,6,7,3,9,7,5,8,9,2,3,2,2,2,2,11,4,9,10,8,4,10,6,9,3,5,3,6,10,7,10,11,11,11,7,6,2,3,4,8,6,5,2,7,4,10,5,8,4,6&accInit=[{id:0,mask:\"cp;tp;lvl;sc;cl\"}]&reel_set2=11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13&reel_set1=11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13&reel_set4=11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13&reel_set3=11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13~11,13,11,13,13,13,11,11,13,11,13,13";
            }
        }
	
	
        #endregion
        public BountyGoldGameLogic()
        {
            _gameID = GAMEID.BountyGold;
            GameName = "BountyGold";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["g"] = "{base:{def_s:\"3,10,2,3,10,6,7,8,4,7,4,10,4,5,11\",def_sa:\"4,7,5,2,11\",def_sb:\"6,7,2,4,7\",reel_set:\"0\",s:\"3,10,2,3,10,6,7,8,4,7,4,10,4,5,11\",sa:\"4,7,5,2,11\",sb:\"6,7,2,4,7\",sh:\"3\",st:\"rect\",sw:\"5\"},matrix_2:{def_s:\"13,13,13,13,13,13,13,13,13,13,13,13,13,13,13\",def_sa:\"13,13,13,13,13\",def_sb:\"13,13,13,13,13\",reel_set:\"2\",s:\"13,13,13,13,13,13,13,13,13,13,13,13,13,13,13\",sa:\"13,13,13,13,13\",sb:\"13,13,13,13,13\",sh:\"3\",st:\"rect\",sw:\"5\"},matrix_3:{def_s:\"13,13,13,13,13,13,13,13,13,13,13,13,13,13,13\",def_sa:\"13,13,13,13,13\",def_sb:\"13,13,13,13,13\",reel_set:\"3\",s:\"13,13,13,13,13,13,13,13,13,13,13,13,13,13,13\",sa:\"13,13,13,13,13\",sb:\"13,13,13,13,13\",sh:\"3\",st:\"rect\",sw:\"5\"},matrix_4:{def_s:\"13,13,13,13,13,13,13,13,13,13,13,13,13,13,13\",def_sa:\"13,13,13,13,13\",def_sb:\"13,13,13,13,13\",reel_set:\"4\",s:\"13,13,13,13,13,13,13,13,13,13,13,13,13,13,13\",sa:\"13,13,13,13,13\",sb:\"13,13,13,13,13\",sh:\"3\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa"))
            {
                string[] strParts = dicParams["apwa"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);
                dicParams["apwa"] = string.Join(",", strParts);
            }

            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

        }

    }
}
