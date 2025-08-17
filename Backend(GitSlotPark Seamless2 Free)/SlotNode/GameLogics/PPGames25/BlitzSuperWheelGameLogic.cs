using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BlitzSuperWheelGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20lightblitz";
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
                return "def_s=6,12,3,4,8,5,11,11,7,11,3,10,10,3,8&cfgs=1&ver=3&def_sb=4,8,12,7,10&reel_set_size=7&def_sa=7,10,12,5,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase:\"96.52\",regular:\"96.52\"},props:{pp:\"50,100,500,2500,10000\",max_rnd_sim:\"1\",max_rnd_hr:\"12820500\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~100,40,20,0,0~1,1,1,1,1;13~100,40,20,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,40,20,0,0;50,20,14,0,0;40,20,14,0,0;30,14,10,0,0;30,14,10,0,0;20,10,6,0,0;20,10,6,0,0;10,6,4,0,0;10,6,4,0,0;10,6,4,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=8,11,5,11,9,12,4,8,12,4,10,11,2,11,6,8,9,10,3,5,10,8,7,11,12,8,4,10,5,12,9,7,12,9,5,9,4,6,8,10,7,10,9,8,5,8,11,9,6,9,7,9,6,11,5,8,5,10,7,3,10,9,10,5,10,11,6,12,9,12~12,9,8,12,8,10,11,8,4,10,11,8,10,11,7,8,9,2,9,4,11,9,12,10,3,9,11,4,7,12,6,5,8,7,3,9,6,10,8,5,3,5~4,11,5,11,6,8,4,8,9,10,12,10,5,2,9,10,7,8,12,7,9,8,11,10,12,6,9,11,5,9,5,11,12,9,3,10,9,6,9,8,10,7,11,10,8,7,9,10,12,8,3,8,12,10,9,6~8,3,5,9,8,10,9,12,10,3,12,10,9,2,11,4,10,8,9,3,9,7,5,11,3,9,10,7,9,8,6,10,12,8,9,11,6,4,8,9,7,8,4,11,4,10,11,6,11,9,8,12,9,10,7,12,5,10,12,7,4,10,5,9,6,10,5,11~8,7,6,11,9,12,11,9,11,5,10,8,2,4,5,9,8,4,10,9,10,5,9,10,11,12,8,3,8,12,5,6,12,10,7,8,11,7,11,9,4,10,9,10,9,5,7,6,10,12,6,3,9&reel_set2=8,5,1,5,9,10,11,9,3,11,8,10,5,9,10,7,11,5,6,3,1,6,4,7,4,12,4,6,7,8,3,12~8,10,12,9,8,3,5,4,12,5,4,7,6,9,8,7,11,3,6,11,6,11,4,3,6~1,2,3,7,12,8,5,11,6,1,8,7,10,12,7,9,10,7,3,11,2,4,8,5,9,11,2,3,6,12,6,7,10,4,5,8~5,12,10,7,4,8,4,7,4,8,12,3,10,6,3,2,9,3,6,12,6,9,2,5,4,9,3,8,11,2,11,5,2,9,11,5,4,11,8,6,7,10,8,3~5,9,4,1,3,6,7,9,12,10,4,1,8,11,5,7,10,8,3,2,7,5,2,11,10,2,12,3,6,2,11,9,10,1,5,8,2,5,6,12,6,9,11,10,9,8&reel_set1=11,7,10,8,12,6,11,10,8,11,3,9,4,10,12,6,8,10,4,9,8,4,9,5,7,9,10,3,5,6,12,5,8,11,5~4,12,8,11,7,8,12,8,11,5,6,9,4,8,12,3,10,6,11,8,10,5,11,10,8,10,9,5,10,12,11,10,3,9,10,7,4,3,7,9,12,9,6,5,11,9~12,5,11,5,6,9,6,11,12,8,7,4,6,3,8,10,4,10,8,12,5,8,3,10,5,10,9,12,8,12,11,9,11,10,7,9,8,5,9,6,11,10,7~12,6,7,8,9,10,3,9,4,10,8,12,4,6,10,8,11,8,10,11,12,5,8,5,9,3,7,11,10,3,4,12,6,11,7,5,11,9~5,10,12,9,11,10,5,8,6,8,9,11,5,9,10,6,3,12,10,8,9,4,8,5,12,4,7,11&reel_set4=12,8,12,11,3,11,5,3,4,6,5,3,8,6,4,10,4,10,4,7,2,9,3,12,10,8,3,10,6,4,2,12,9,10,7,10,11,3,11,10,9,8,9,5,12,6,2,12,10,2,7,9,7,5,3,8,5,7,2,3,2,5,7,10,5,4,6,10,6~3,5,4,10,11,10,5,8,2,6,11,10,8,5,3,11,9,6,9,7,6,9,2,9,7,3,4,6,11,4,8,2,12,9,3,7,5,10,9,6,3,9,3,2,7,10,5,10,8,6,11,10,6,12,6,10,7,2,4,9,5,2,12,3,9~10,5,7,10,12,3,11,2,7,10,9,6,5,4,2,4,8,11,6,8,6,10,9,6,3,4,7,9,5,3,8,9,12,5,2,10,5,4,9,10,12,11,8,10,5,8,2,3,6,5,7,12,5,12,10~3,10,2,5,4,9,3,10,9,2,8,7,11,3,2,6,10,12,5,10,9,2,4,12,6,12,10,5,3,11,8,7,8,6,5,3,2,7,6,9,3,10,8,3,11,4,9,7,9,5,11,9,7,11,9,5,6,4,3~8,9,5,2,3,8,12,3,12,10,3,4,7,6,2,6,10,2,8,3,9,4,12,10,5,10,5,9,4,6,3,10,7,5,3,12,5,10,9,4,8,4,2,6,2,5,8,7,11,12,2,7,9,5,4,10,11,9,7,6,4,5,3,7,9,2,5,10,11&purInit=[{bet:2000,type:\"default\"}]&reel_set3=11,3,10,3,5,12,4,9,5,10,3,4,10,9,3,8,10,1,10,3,4,8,12,5,10,5,11,12,6,5,7,4,11,7,1,4,8,3,10,3,8,6,3,9,10,5,9,4,7,1,6,2,9,11,5,6,12,7,5,8,12,6,5,1,7,9,3,5,9,10,10~10,11,6,11,7,6,8,7,6,7,4,8,9,8,10,3,12,6,3,10,6,5,3,6,4,5,7,6,11,7,9,5,3,9,5,3,9,6,9,10,11,9,10,4,10,5,9,10,4,11,3,4,12,10,9,8,11,3,9,4,10,3,8,12,2,7,5,3,7,5,12,9,10~5,1,9,8,5,7,9,2,10,8,11,9,7,6,4,10,12,5,2,9,5,3,12,6,4,2,3,5,1,12,11,10,3,12,3,10,1,5,6,8,3,1,8,5,7,11,12,4,8,7,10,12,4,2,5,8,2,3,5,9,6,9,4,10,10,3,11~3,5,2,11,10,3,10,7,10,2,8,5,10,3,2,11,10,8,11,9,5,9,12,9,11,3,4,11,3,6,3,5,4,10,6,7,5,4,5,4,11,9,2,9,4,12,7,9,3,6,7,4,3,7,12,2,8,7,6,2,6,9,8~5,8,12,11,2,6,2,10,5,12,4,6,2,12,3,4,10,9,5,1,4,9,7,9,6,3,10,9,6,3,7,11,3,10,4,3,10,11,8,2,12,10,5,9,1,4,8,2,6,4,8&reel_set6=9,11,9,11,5,7,5,4,7~12,3,6,8,10~11,8,2,12,4,7,11,9,5,8,3,6,10,12,9,8,12,2,9,10,2,7,12,11,3,6,5,2,5,10,9,4,3,8,7,10,11,6,3,8,2,10,5,3,12,9,10,4,10,5,4,7,6,3,10,9,5,2,4~9,3,7,10,3,4,10,5,9,2,9,11,6,3,9,5,8,11,5,4,9,10,11,6,5,3,5,11,9,10,12,6,7,6,12,5,2,8,7,3,2,8,4,10,3,9,10,4,2,7,8,12,3,11,4,7,2,6~12,2,5,7,3,12,4,7,4,9,5,7,8,5,10,4,8,2,6,4,11,2,7,10,5,3,11,2,8,10,3,5,9,4,10,2,3,10,4,5,8,11,9,12,11,5,9,3,10,7,5,3,7,4,12,9,6,10,6,10,2,6&reel_set5=10,6,2,9,7,10,12,10,8,5,8,5,11,7,6,8,5,3,12,3,10,9,2,9,3,5,11,2,7,12,4,2,5,12,7,10,2,4,6,10,2,12,11,8,9,10,9,3,6~10,6,11,6,2,5,12,6,2,3,11,4,5,9,10,4,12,10,4,9,11,8,9,2,3,5,2,10,9,5,11,10,9,7,3,5,9,6,10,8,6,3,8,6,2,8,3,4,7,12,8,7,6,7,10,12,11,9,8,2,5,2,10,3,9~9,10,12,10,4,3,5,4,6,10,2,12,8,3,11,6,3,5,3,10,2,5,2,8,11,10,5,12,6,12,2,4,9,4,9,2,7,5,9,5,7,6,10,7,6,5,11,9,2,8,10,7,2,8~6,2,10,6,7,4,9,3,5,9,5,9,6,9,2,6,12,11,4,2,11,7,3,8,4,5,7,2,8,10,7,2,11,9,3,10,5,8,10,11~6,5,11,2,10,5,10,5,11,10,4,2,3,4,2,8,4,5,10,7,2,8,9,12,5,4,12,2,4,9,5,10,3,2,7,9,2,7,3,9,6,12,6,3,8,5,11,10,7,10,9,12,4,8,5,2,7,9&total_bet_min=200.00";
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
        
        public BlitzSuperWheelGameLogic()
        {
            _gameID = GAMEID.BlitzSuperWheel;
            GameName = "BlitzSuperWheel";
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
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BlitzSuperWheelGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in BlitzSuperWheelGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
