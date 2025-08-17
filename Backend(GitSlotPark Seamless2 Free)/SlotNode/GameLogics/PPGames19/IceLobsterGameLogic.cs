using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class IceLobsterGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20stickypos";
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
                return "def_s=5,8,9,6,11,4,10,4,4,10,6,8,6,5,12&cfgs=11648&ver=3&mo_s=13&mo_v=10,20,40,60,80,100,160,200,300,400,600,1000,2000,4000,10000,20000&def_sb=3,10,4,3,11&reel_set_size=3&def_sa=4,10,3,7,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase:\"94.54\",regular:\"94.56\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"588235294\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~2000,200,50,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,0,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,15,0,0;100,25,10,0,0;100,25,10,0,0;50,10,2,0,0;50,10,2,0,0;50,10,2,0,0;0,0,0,0,0&total_bet_max=8,000,000.00&reel_set0=7,4,9,2,9,11,3,5,10,9,10,11,1,6,1,2,2,2,12,10,6,10,8,3,7,12,4,8,12,7,11,9,5,2~9,12,4,11,6,10,6,10,9,8,6,11,8,9,6,2,5,12,6,5,9,8,11,2,12,7,10,1,2,2,2,4,7,11,2,1,9,7,9,7,8,12,4,11,7,10,6,6,10,9,11,8,5,3,10,6,12,9,12,6,3,12~4,10,11,12,6,4,8,2,7,5,4,12,5,3,12,9,3,12,11,5,11,11,5,12,6,12,4,10,2,2,2,4,12,7,2,4,2,5,7,10,5,11,8,11,9,11,1,3,9,10,1,10,5,8,5,12,10,12,6,10~9,6,6,11,5,6,11,5,8,7,8,11,2,12,4,8,7,7,11,10,11,1,3,5,12,5,2,6,4,9,12,3,8,6,2,2,2,8,11,8,9,6,6,12,6,10,11,6,11,10,8,3,9,8,9,12,4,8,11,1,7,5,11,10,8,11,3,8,7,4,2,10,7~12,7,5,3,2,10,6,7,12,10,8,9,12,10,10,11,10,12,9,5,10,5,3,3,4,10,11,12,10,8,4,7,3,7,8,4,12,2,2,2,5,4,7,11,10,10,1,9,11,2,11,10,10,2,7,10,3,6,6,11,12,9,10,10,3,7,12,1,3,9,12,7,10,5,11,12,9,7,7,4,9&reel_set2=12,10,11,6,11,5,3,9,7,4,9,4,11,10,7,8,2,11,7,9,10,8,12,11,9,2,3,2,2,2,8,9,4,8,2,6,12,10,6,12,6,7,9,7,2,12,5,4,12,11,2,12,10,5,2,8,7,9,11,2,3~12,2,11,12,5,10,8,6,9,8,12,6,3,6,2,11,8,12,9,10,6,9,6,9,2,2,2,4,12,4,6,7,11,4,7,5,2,8,12,9,10,11,9,10,7,2,7,6,5,6,10,11~12,5,11,9,10,6,11,9,5,11,5,4,12,4,2,5,10,12,2,5,11,11,9,5,10,2,2,2,6,10,6,8,2,4,5,9,3,8,12,8,7,12,7,12,3,12,10,7,10,12,4,2,12,11,4~10,7,8,11,8,11,2,5,2,2,6,2,4,12,7,5,3,11,4,11,8,6,11,2,10,3,5,11,4,4,6,11,8,2,2,2,12,8,6,9,6,6,8,5,12,8,11,12,3,11,7,11,8,7,7,9,11,9,3,8,6,6,10,8,6,11,7,8,12,5,10,8,9,11~9,10,10,5,7,5,12,3,12,2,10,11,4,10,2,7,3,7,12,11,10,4,9,2,2,2,11,3,10,9,10,10,6,10,3,9,7,12,6,8,7,8,2,12,4,12,10,11,7,5,10&reel_set1=7,12,10,9,12,9,4,11,12,2,1,6,3,6,8,2,2,2,9,4,8,10,7,10,7,13,11,13,4,9,8,11,2,13,13,13,11,12,6,13,12,9,2,2,13,12,5,10,13,8,11,7,9,13~10,3,10,6,6,12,13,12,9,1,11,7,11,8,13,7,6,12,11,8,2,7,2,2,2,7,5,13,9,6,2,6,11,13,8,13,9,13,9,2,11,12,9,6,13,10,12,8,9,13,13,13,8,9,6,7,8,7,2,2,6,10,11,10,6,10,13,6,6,2,13,5,6,13,12,4,9,10~9,2,5,7,13,2,12,13,13,4,5,5,4,12,9,13,2,2,2,12,8,11,12,5,4,7,12,10,2,11,3,10,11,10,12,13,11,13,8,13,13,13,5,10,8,6,5,10,1,2,10,6,12,13,11,7,4,9,5,12,7,13,11~6,8,13,12,11,8,1,6,6,11,1,2,11,13,10,6,11,8,2,2,2,7,7,13,5,7,8,7,5,5,8,6,13,4,10,8,2,6,13,12,13,13,13,11,13,11,3,11,2,12,13,11,12,9,10,7,9,10,13,9,2,8,3,9~4,12,5,12,11,12,13,10,7,10,10,9,11,10,6,2,2,2,9,2,8,7,12,3,12,10,7,13,5,13,7,8,1,13,13,13,3,2,2,11,9,13,7,6,2,12,10,9,10,5,10,10,7,11&purInit=[{bet:2000,type:\"default\"}]&total_bet_min=10.00";
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
        public IceLobsterGameLogic()
        {
            _gameID = GAMEID.IceLobster;
            GameName = "IceLobster";
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
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in IceLobsterGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in IceLobsterGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
