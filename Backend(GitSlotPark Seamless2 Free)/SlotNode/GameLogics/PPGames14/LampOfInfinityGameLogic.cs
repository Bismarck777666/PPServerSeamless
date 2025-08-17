using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class LampOfInfinityGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20lampinf";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,11,5,3,7,4,9,8,4,8,5,11,11,6,11&cfgs=7642&ver=3&def_sb=6,8,11,3,8&reel_set_size=3&def_sa=5,9,5,4,9&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"1000000\",max_rnd_win:\"5000\",max_rnd_win_a:\"3334\",max_rnd_hr_a:\"1000000\"}}&wl_i=tbm~5000;tbm_a~3334&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=20,30&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,1000,400,200,0;1000,400,200,100,0;400,200,100,40,0;200,100,40,20,0;100,40,20,10,0;100,40,20,10,0;40,20,10,0,0;40,20,10,0,0;40,20,10,0,0&total_bet_max=10,000,000.00&reel_set0=4,8,7,11,6,11,10,11,7,10,10,10,5,9,7,10,5,10,10,6,11,9,11,11,11,9,10,9,8,3,4,1,11,7,8~10,5,10,9,4,9,8,11,6,11,11,10,10,5,3,9,6,9,4,7,1,11,8,7,8,7,9,7,11,10,8,5~10,8,10,11,4,10,11,6,11,7,1,3,7,5,4,7,8,9,4,8,9,11,6,10,9,9~9,10,5,10,8,9,10,10,10,9,11,9,5,4,9,10,9,9,9,7,8,6,8,11,7,11,11,11,7,3,10,1,10,11,11,4~7,1,11,10,8,9,8,9,11,7,10,10,10,9,5,3,8,4,6,11,10,9,10,11,6,4&reel_set2=11,11,9,9,6,9,11,10,11,7,11,9,7,11,8,5,6,7,11,10,10,9,8,10,9,9,11,7,9,10,11,3,10,9,4,8,10,8,10,10,9,11,10~7,9,11,11,8,10,5,10,10,9,9,9,10,4,10,11,11,8,11,11,9,10,10,10,9,8,9,7,11,10,9,3,10,9,6,7~2,2,2,2,2~2,2,2,2,2~2,2,2,2,2&reel_set1=11,4,7,9,8,4,11,7,5,10,10,10,6,8,6,7,9,10,1,11,9,5,11,11,11,10,11,9,9,8,10,3,8,7,10,10~4,10,6,3,1,10,9,10,10,8,9,7,11,11,5,9,7,11,8~11,4,6,7,5,11,10,8,10,1,9,5,11,7,9,6,7,8,9,8,6,8,4,7,11,3,10,5,10,9,10,11,9,4~10,11,1,6,8,10,10,10,9,11,11,10,6,5,9,9,9,3,4,11,1,4,9,11,11,11,5,7,10,9,10,8,9~3,8,9,7,11,6,4,10,11,8,7,9,11,10,10,9,10,10,10,11,5,8,6,9,7,1,7,11,9,10,9,6,4,1,8,11&purInit=[{bet:2000,type:\"default\"}]&total_bet_min=10.00";
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
        public LampOfInfinityGameLogic()
        {
            _gameID = GAMEID.LampOfInfinity;
            GameName = "LampOfInfinity";
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in LampOfInfinityGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in LampOfInfinityGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                _logger.Error("Exception has been occurred in LampOfInfinityGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
