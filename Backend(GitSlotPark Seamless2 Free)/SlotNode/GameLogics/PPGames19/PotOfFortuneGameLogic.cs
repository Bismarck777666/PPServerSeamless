using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class PotOfFortuneGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20stckwldsc";
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
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,8,7,3,13,6,9,13,6,11,7,11,8,3,12,6,13,11,7,11&cfgs=9806&ver=3&def_sb=5,11,9,6,13&reel_set_size=3&def_sa=5,11,3,3,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"72463768\",max_rnd_win:\"5000\",max_rnd_win_a:\"4000\",max_rnd_hr_a:\"38167939\"}}&wl_i=tbm~5000;tbm_a~4000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~2500,100,50,0,0~1,1,1,1,1;3~2500,100,50,0,0~1,1,1,1,1&bonuses=0&bls=20,25&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2500,100,50,0,0;300,60,30,0,0;200,40,20,0,0;160,32,16,0,0;120,24,12,0,0;100,20,10,0,0;60,12,6,0,0;60,12,6,0,0;40,8,4,0,0;40,8,4,0,0&total_bet_max=8,000,000.00&reel_set0=12,13,11,1,8,9,8,3,11,11,11,11,10,11,13,8,4,13,4,10,1,10,13,13,13,11,6,12,6,10,4,7,13,3,8,8,8,8,5,1,6,7,7,11,3,4,10,9,9,9,7,7,5,8,4,9,12,9,1,10,10,10,10,12,2,11,5,8,13,12,12,9,2~3,13,7,5,8,11,11,11,11,2,6,11,5,3,10,13,12,12,12,12,4,5,6,10,1,12,12,13,13,13,13,4,1,13,9,9,12,13,4,4,4,11,4,8,11,6,12,1,9,9,9,9,12,9,9,12,2,7,13,10,10,10,8,10,13,3,7,10,8,11~13,10,13,12,2,1,6,10,12,9,12,12,12,2,10,13,7,1,10,13,13,7,12,11,13,13,13,13,13,9,12,11,9,7,10,1,4,3,3,6,7,7,7,5,10,12,9,4,10,10,11,12,11,12,9,9,9,9,3,5,5,9,1,10,8,8,6,8,8,10,10,10,9,9,8,7,12,4,13,6,10,9,8,4~11,7,8,11,10,11,11,11,13,9,8,7,7,12,2,12,12,12,12,6,10,10,8,7,12,11,13,13,13,13,5,6,6,11,13,8,8,8,13,9,4,12,1,4,13,9,9,9,5,12,6,9,5,11,2,10,10,10,1,7,11,9,8,3,11,9,12~11,13,7,9,13,1,9,11,11,11,11,10,8,9,13,8,13,11,9,12,12,12,8,10,3,10,12,11,13,8,13,13,13,2,10,10,12,11,7,6,6,9,9,9,9,9,10,7,12,4,10,7,12,10,10,10,12,8,11,1,5,7,6,5&reel_set2=7,10,13,11,10,8,11,8,9,6,7,6,12,13,7,11,10,12,13,4,7,11,9,5,8,5,6,12,10,12,10,5,12,6,13,9,12,9,11,9,13,11,6,13,8,7,11,10,12,13,12,9,7~11,9,7,13,12,13,7,10,12,11,12,6,13,10,5,11,13,9,13,11,12,10,11,8,12,8,5,6,10,13,9,11,9,13,10,6,8,10,8,7,11,10,9,12,11,13,12,10,13,11,9,5,12,10,13,9,6,13,6,11,13,12,7,11,4,8~11,7,11,8,6,9,13,8,9,5,11,5,10,9,6,13,4,12,13,10,11,7,5,9,12,9,8,10,6,10,13,6,12,5,12,9,13,10,13,7,11,12,13,12,13,12,7,12,13,10,12,6,11,10~11,12,8,7,9,11,9,10,8,10,6,13,12,13,5,9,5,7,9,11,12,10,5,8,6,13,12,6,7,8,9,13,11,7,4,10,11,12,13,10,13,12,13,10,11~5,13,8,12,11,9,10,12,9,11,6,7,10,6,10,13,7,12,11,8,5,6,13,9,12,6,12,13,6,8,11,7,10,13,11,5,10,4,12,10,12,8,12&reel_set1=13,9,5,10,11,8,10,11,11,11,7,8,9,12,7,1,12,12,10,12,12,12,12,7,6,4,5,10,6,3,8,11,13,13,13,10,13,2,5,4,13,5,6,6,6,9,7,11,8,6,13,11,13,12,8,8,8,5,12,12,13,6,6,3,7,3,10,10,10,10,11,13,12,2,1,13,7,9~7,12,12,1,9,12,7,8,7,12,12,12,12,13,10,10,3,11,11,4,10,10,2,13,13,13,13,12,4,9,5,7,12,13,9,3,12,11,6,6,6,8,2,13,1,13,13,10,5,13,11,8,8,8,6,5,1,12,8,5,8,8,6,9,9,9,6,1,9,9,11,6,3,5,12,2,13,13~12,9,6,7,13,11,11,11,12,6,8,9,3,4,3,12,12,12,8,9,11,1,5,8,2,13,13,13,11,11,4,13,8,1,4,4,4,8,10,13,10,13,11,5,5,5,6,10,10,6,12,13,7,8,8,8,10,7,12,9,12,10,9,9,9,9,11,5,6,5,1,9,9,10,10,10,10,3,5,2,12,4,10,11~5,12,10,2,12,7,7,1,11,11,11,11,8,8,6,13,7,3,12,6,8,1,12,12,12,5,12,13,9,12,7,6,13,9,8,13,13,13,13,13,8,13,5,11,13,13,11,12,10,6,6,6,8,2,9,12,13,13,10,9,4,10,8,8,8,7,11,7,13,5,11,6,10,13,10,10,10,10,10,10,10,10,5,3,11,10,1,11,10,13,5,11,13~7,3,13,1,11,12,11,1,11,11,11,13,7,10,1,13,12,8,12,10,12,12,12,12,5,3,6,10,7,13,12,10,11,13,13,13,12,8,9,9,10,10,5,11,12,9,6,6,6,12,4,10,8,6,8,8,13,6,8,8,8,8,12,2,11,8,13,9,5,11,10,10,10,10,7,8,13,8,7,6,7,10,12,4,10&purInit=[{bet:1600,type:\"default\"}]&total_bet_min=10.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 80; }
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
            get { return 1.25; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
	
        #endregion
        public PotOfFortuneGameLogic()
        {
            _gameID = GAMEID.PotOfFortune;
            GameName = "PotOfFortune";
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in PotOfFortuneGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in PotOfFortuneGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                _logger.Error("Exception has been occurred in PotOfFortuneGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
