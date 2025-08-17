using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class RainbowReelsBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 20.0f; }
        }
    }
    class RainbowReelsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40rainbowr";
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
            get { return 40; }
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
                return "def_s=3,12,3,13,9,11,4,8,10,13,13,12,11,6,6,7,9,5,13,10&cfgs=8589&ver=3&def_sb=13,7,13,8,3&reel_set_size=4&def_sa=11,11,3,4,5&scatters=1~100,100,100,100,100,100,100,100,10,2,0,0~0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"6477143\",max_rnd_win:\"5000\"},rtps:{purchase:\"94.03\",regular:\"94.00\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~200,60,20,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0;200,60,20,0,0;150,50,16,0,0;125,40,14,0,0;100,30,12,0,0;75,25,10,0,0;60,20,8,0,0;50,15,6,0,0;40,12,5,0,0;35,9,4,0,0;30,7,3,0,0;25,5,2,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=3,11,13,7,13,11,5,12,13,4,13,9,1,13,5,13,8,11,9,10,12,5,13,3,5,11,13,7,3,11,7,9,11,3,11,12,5,9,10,1,8,12,7,8,9,6,8,13,5,7,9~4,11,12,4,12,9,7,7,13,4,9,5,12,8,10,6,10,13,6,9,13,4,12,10,3,10,8,6,13,7,8,11,3,12,11,6,10,8,6,10,9,4,10,9,5,11,10,6,12,8,6,13~3,8,11,5,13,9,7,10,5,9,5,11,7,8,10,1,13,11,5,9,7,3,13,1,7,8,13,7,12,11,5,13,6,3,8,10,13,7,6,13,11,5,9,11,3,11,5,7,9,11,5,12,8,7~5,13,8,6,13,11,5,8,9,4,13,10,6,13,8,7,13,8,6,10,13,7,8,11,6,9,13,3,12,11,7,3,10,6,11,7,4,9,8,7,10,12,5,8,11,6,8,9,7,10,3,11,5,12~3,13,11,3,10,9,7,11,8,5,9,13,6,10,3,10,8,13,7,4,13,5,8,11,7,13,9,3,10,11,6,12,10,6,9,1,7,10,8,6,11,12,5,9,12,4,8,11,6,10,1,7,11,12&reel_set2=3,11,12,1,13,9,6,12,13,4,13,9,1,12,7,13,8,11,9,8,12,5,13,9,7,11,12,6,8,11,11,10,13,3,11,12,5,9,10,7,8,2,2,2~3,11,12,4,12,9,5,7,13,6,9,7,12,8,10,6,10,12,5,9,13,4,12,10,3,10,8,6,13,7,8,11,4,13,11,6,10,8,6,11,9,2,2,2,5,5,11,11,6~4,8,11,1,13,9,7,10,5,9,6,11,7,8,10,1,13,11,5,9,8,3,13,9,7,8,13,7,12,11,5,13,6,4,8,10,12,8,6,12,10,2,2,2,3,3,11~5,13,8,6,9,6,7,8,9,4,13,10,6,8,11,7,13,8,5,7,13,7,8,11,6,9,13,3,10,11,7,8,10,6,7,12,4,9,8,7,10,2,2,2,2,2,6,8,9,7~3,13,11,1,10,9,7,11,8,5,9,13,6,10,13,10,8,13,7,4,13,5,8,11,7,13,9,3,10,11,6,12,10,6,9,1,7,10,8,6,11,2,2,2,2&reel_set1=3,11,12,1,13,9,6,12,13,4,13,9,1,12,7,13,8,11,9,8,12,5,13,9,7,11,12,6,8,11,11,10,13,3,11,12,5,9,10,7,8,2,2,2,10,10,6,8~3,11,12,4,12,9,5,7,13,6,9,7,12,8,10,6,10,12,5,9,13,4,12,10,3,10,8,6,13,7,8,11,4,13,11,6,10,8,6,11,9,2,2,2,5,5,11,11~4,8,11,1,13,9,7,10,5,9,6,11,7,8,10,1,13,11,5,9,8,3,13,9,7,8,13,7,12,11,5,13,6,4,8,10,12,8,6,12,10,2,2,2,3,3,11,10,7~5,13,8,6,9,6,7,8,9,4,13,10,6,8,11,7,13,8,5,7,13,7,8,11,6,9,13,3,10,11,7,8,10,6,7,12,4,9,8,7,10,2,2,2,10,12,6,8,9,7~3,13,11,1,10,9,7,11,8,5,9,13,6,10,13,10,8,13,7,4,13,5,8,11,7,13,9,3,10,11,6,12,10,6,9,1,7,10,8,6,11,2,2,2,2,12,4,8,11,6&purInit=[{bet:2000,type:\"fs\"}]&reel_set3=3,11,13,7,13,11,5,12,13,4,13,9,1,13,5,13,8,11,9,10,12,5,13,3,5,11,13,7,3,11,7,9,11,3,11,12,5,9,10,1,8,12,7,8,9,6,8,13,5,7,9~4,11,12,4,12,9,7,7,13,4,9,5,12,8,10,6,10,13,6,9,13,4,12,10,3,10,8,6,13,7,8,11,3,12,11,6,10,8,6,10,9,4,10,9,5,11,10,6,12,8,6,13~3,8,11,5,13,9,7,10,5,9,5,11,7,8,10,1,13,11,5,9,7,3,13,1,7,8,13,7,12,11,5,13,6,3,8,10,13,7,6,13,11,5,9,11,3,11,5,7,9,11,5,12,8,7~5,13,8,6,13,11,5,8,9,4,13,10,6,13,8,7,13,8,6,10,13,7,8,11,6,9,13,3,12,11,7,3,10,6,11,7,4,9,8,7,10,12,5,8,11,6,8,9,7,10,3,11,5,12~3,13,11,3,10,9,7,11,8,5,9,13,6,10,3,10,8,13,7,4,13,5,8,11,7,13,9,3,10,11,6,12,10,6,9,1,7,10,8,6,11,12,5,9,12,4,8,11,6,10,1,7,11,12&total_bet_min=10.00";
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
	
	
        #endregion
        public RainbowReelsGameLogic()
        {
            _gameID = GAMEID.RainbowReels;
            GameName = "RainbowReels";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            RainbowReelsBetInfo betInfo = new RainbowReelsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new RainbowReelsBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                RainbowReelsBetInfo betInfo     = new RainbowReelsBetInfo();
                betInfo.BetPerLine              = (float)message.Pop();
                betInfo.LineCount               = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in RainbowReelsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in RainbowReelsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
