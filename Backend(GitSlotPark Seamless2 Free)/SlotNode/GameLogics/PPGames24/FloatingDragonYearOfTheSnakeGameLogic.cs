using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FloatingDragonYearOfTheSnakeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10fdsnake";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "def_s=4,8,5,3,12,5,10,8,5,8,6,8,12,3,12&cfgs=1&ver=3&def_sb=6,10,3,6,10&reel_set_size=15&def_sa=4,12,5,3,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.71\",purchase:\"96.71\",regular:\"96.71\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"3086876\",max_rnd_win:\"5000\",max_rnd_win_a:\"3334\",max_rnd_hr_a:\"1748252\"}}&wl_i=tbm~5000;tbm_a~3334&reel_set10=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&sc=10.00,20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=12,6,7,8,10,2,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,2,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,2,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,2,12,9,10,4,3,11,5,2,9,8,7,7,7,7,7~6,12,2,3,6,7,11,3,5,7,10,4,3,5,2,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,2,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,2,12,5,2,8,7,6,4,11,3,9,7,7,10,6,3,5,2,7,7,7,7,7~5,4,6,7,11,2,4,8,2,7,5,3,12,7,7,10,2,6,9,7,7,7,7,7&reel_set12=12,6,7,8,10,2,5,6,12,7,11,6,5,2,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,2,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,2,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,2,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,2,12,9,10,4,3,11,5,2,9,8,7,7,7,7,7~6,12,2,3,6,7,11,3,5,7,2,10,4,3,5,2,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,2,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,2,12,5,2,8,7,6,4,11,3,9,7,7,10,6,3,5,2,7,7,7,7,7~5,4,6,7,11,2,4,8,2,7,5,3,12,7,7,10,2,6,9,7,7,7,7,7&purInit_e=1&reel_set13=12,6,7,8,10,2,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,2,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,2,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,2,12,9,10,4,3,11,5,2,9,8,7,7,7,7,7,7,7~6,12,2,3,6,7,11,3,5,7,10,4,3,5,2,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,2,10,3,6,4,3,8,7,7,7,7,7,7,7~4,5,7,6,2,12,5,2,8,7,6,4,11,3,9,7,7,10,6,3,5,2,7,7,7,7,7,7,7~5,4,6,7,11,2,4,8,2,7,5,3,12,7,7,10,2,6,9,7,7,7,7,7,7,7&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&reel_set14=12,6,7,8,10,2,5,6,12,7,11,6,5,2,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,2,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,2,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,2,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,2,12,9,10,4,3,11,5,2,9,8,7,7,7,7,7,7,7~6,12,2,3,6,7,11,3,5,7,2,10,4,3,5,2,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,2,10,3,6,4,3,8,7,7,7,7,7,7,7~4,5,7,6,2,12,5,2,8,7,6,4,11,3,9,7,7,10,6,3,5,2,7,7,7,7,7,7,7~5,4,6,7,11,2,4,8,2,7,5,3,12,7,7,10,2,6,9,7,7,7,7,7,7,7&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,2,0,0;100,25,2,0,0;50,10,2,0,0;50,10,2,0,0;50,10,2,0,0&total_bet_max=10,000,000.00&reel_set0=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&accInit=[{id:0,mask:\"cp;tp;lvl;sc;cl\"}]&reel_set2=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set1=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set4=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&purInit=[{bet:1000,type:\"default\"}]&reel_set3=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set6=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set5=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set8=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set7=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set9=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&total_bet_min=100.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 100; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
	
	
        protected override double MoreBetMultiple
        {
            get { return 1.5; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
	
        #endregion
        public FloatingDragonYearOfTheSnakeGameLogic()
        {
            _gameID     = GAMEID.FloatingDragonYearOfTheSnake;
            GameName    = "FloatingDragonYearOfTheSnake";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in spinParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "fs", "fswin", "fsres", "fsmax", "fsmul", "trail","reel_set", "g" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!bonusParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = bonusParams[toCopyParams[i]];
            }

            string[] toRemoveParams = new string[] { "mo", "mo_t" };
            for (int i = 0; i < toRemoveParams.Length; i++)
            {
                if (resultParams.ContainsKey(toRemoveParams[i]))
                    resultParams.Remove(toRemoveParams[i]);
            }

            return resultParams;
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FloatingDragonYearOfTheSnakeGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in FloatingDragonYearOfTheSnakeGameLogic::readBetInfoFromMessage", strGlobalUserID);
                    return;
                }
		
                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
                    return;
                }
                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FloatingDragonYearOfTheSnakeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
