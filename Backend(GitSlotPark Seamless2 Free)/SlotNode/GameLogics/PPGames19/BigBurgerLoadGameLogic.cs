using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BigBurgerLoadGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10bburger";
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
                return 5;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=17,17,17,17,17,17,17,17,17,17,17,17,3,10,4,7,13,17,6,8,9,6,8,17,4,12,5,4,13,17&cfgs=11487&ver=3&def_sb=7,11,12,7,10,17&reel_set_size=3&def_sa=5,9,12,6,9,17&scatters=1~1000,100,50,20,10,5,2,1,0,0~0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1;14~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"94.02\",purchase:\"94.01\",regular:\"94.07\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"124814\",max_rnd_win:\"3000\",max_rnd_sim_a:\"1\",max_rnd_win_a:\"2000\",max_rnd_hr_a:\"63543\"}}&wl_i=tbm~3000;tbm_a~2000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&bls=10,15&paytable=0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0;5000,1000,500,200,20,0;1000,500,200,100,10,0;500,200,100,50,5,0;200,100,50,20,0,0;200,100,50,20,0,0;100,50,20,10,0,0;100,50,20,10,0,0;100,50,20,10,0,0;50,20,10,5,0,0;50,20,10,5,0,0;50,20,10,5,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=1,7,11,9,10,8,12,11,11,11,3,6,1,10,1,14,7,12,12,12,4,11,9,11,9,13,8,5,8,9,9,9,7,5,6,13,12,13,10,9,11,10,10,10,12,13,12,6,11,13,14,10,8,12~9,7,11,13,10,7,1,1,6,12,12,12,13,4,5,12,13,10,5,12,6,8,8,13,13,13,3,9,12,9,13,8,12,11,11,4,11,10~13,13,10,10,13,11,9,12,11,11,11,6,1,11,8,1,12,1,6,10,12,12,12,13,13,11,11,5,8,9,12,11,13,13,13,8,9,6,4,3,10,7,14,7,10,10,10,12,1,8,14,12,5,9,14,4,7~4,7,12,1,13,13,11,11,11,10,9,9,6,8,9,12,12,12,7,13,10,6,12,12,11,13,13,13,11,1,1,8,3,4,10,10,10,12,7,5,8,5,11,10,11~10,13,11,5,1,14,7,11,11,11,4,6,13,10,12,1,12,12,7,12,12,12,9,8,14,11,1,11,1,8,13,13,13,9,8,9,12,10,7,6,11,3,9,9,9,10,6,14,5,9,8,12,11,13,13~17,17,17,17,17&reel_set2=11,6,9,6,8,11,11,11,11,9,13,5,14,11,10,12,12,12,12,4,12,5,14,3,7,13,13,13,13,11,8,8,11,8,7,7,7,7,10,11,14,5,13,13,10,9,9,9,9,6,7,11,3,8,9,10,10,10,10,4,7,10,10,9,12,9,7~10,3,13,11,4,11,11,11,11,12,8,8,12,11,8,6,12,12,12,12,4,7,6,13,6,9,13,13,13,13,10,9,10,8,9,5,12,8,8,8,8,7,10,5,9,8,6,9,9,9,9,11,7,5,12,7,9,10,10,10,10,12,9,8,10,3,13,12,4~7,7,8,8,16,11,11,11,11,6,4,9,8,8,11,14,10,12,12,12,12,9,8,13,10,6,13,7,13,13,13,13,9,8,4,4,16,15,7,7,7,7,9,12,10,9,16,5,9,8,8,8,8,6,12,12,9,3,13,11,3,9,9,9,9,15,7,8,10,14,5,10,10,10,10,15,13,6,10,7,13,5,6,13~9,10,9,11,11,11,11,12,13,7,3,4,6,5,12,12,12,12,6,11,7,7,8,9,4,13,13,13,13,8,10,6,7,10,8,8,8,8,10,12,5,9,4,10,5,9,9,9,9,13,11,8,11,11,13,8,10,10,10,10,9,6,8,6,3,9,7,12~6,7,8,6,13,9,5,11,11,11,11,6,8,6,11,13,3,10,5,12,12,12,12,14,9,9,14,10,10,12,12,13,13,13,13,8,7,7,11,7,4,10,6,3,8,8,8,8,9,12,11,10,12,10,13,9,9,9,9,8,12,4,7,11,5,8,13,10,10,10,10,8,10,14,8,12,12,10,12,5,9~13,10,9,10,9,11,11,11,11,3,8,13,4,6,12,12,12,12,8,10,10,11,6,7,13,13,13,13,11,8,12,6,8,11,8,8,8,8,7,6,13,9,12,5,9,9,9,9,5,8,4,10,10,9,10,10,10,10,7,13,5,8,7,13,3,13&reel_set1=9,11,13,10,7,5,11,11,11,12,4,3,13,8,1,10,12,12,12,5,6,11,9,7,12,11,9,9,9,12,1,12,13,8,6,1,10,10,10,9,13,14,8,14,10,14,11~1,12,6,11,4,12,10,8,3,4,12,12,12,11,11,1,13,10,5,8,11,9,7,13,13,13,5,8,7,6,10,13,6,1,13,12,9,12,9~9,14,13,5,9,13,7,10,11,11,11,8,1,6,10,3,13,11,12,9,12,12,12,8,10,8,6,7,9,11,1,13,13,13,8,12,13,14,4,11,11,13,12,10,10,10,1,12,14,7,4,11,12,10,6,5~12,8,3,7,13,1,11,11,11,7,5,10,12,1,1,12,12,12,11,9,12,4,13,11,6,13,13,13,7,13,9,13,8,5,4,10,10,10,8,6,10,6,11,10,9,12~12,7,12,1,13,13,11,11,11,5,8,5,11,11,12,10,12,12,12,9,6,4,3,12,14,9,13,13,13,14,8,9,1,11,10,13,9,9,9,13,10,14,1,7,6,8,11~17,17,17,17,17&purInit=[{bet:1000,type:\"fs\"}]&total_bet_min=20.00";
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
        public BigBurgerLoadGameLogic()
        {
            _gameID = GAMEID.BigBurgerLoad;
            GameName = "BigBurgerLoad";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "6";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("wlc_v"))
            {
                string[] strParts = dicParams["wlc_v"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strValues = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    strValues[1] = convertWinByBet(strValues[1], currentBet);
                    strParts[i] = string.Join("~", strValues);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
            }
            if (dicParams.ContainsKey("apwa"))
            {
                string strApwa = dicParams["apwa"];
                string[] strParts = strApwa.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);

                dicParams["apwa"] = string.Join(",", strParts);
            }
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BigBurgerLoadGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in BigBurgerLoadGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                _logger.Error("Exception has been occurred in BigBurgerLoadGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
