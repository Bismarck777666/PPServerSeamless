using Akka.Event;
using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class CashBoxGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20cashmachine";
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
                return "def_s=12,12,12,12,12,12,12,12,12,9,12,12,12,12,12&cfgs=8646&ver=3&def_sb=9,9,9,9,12&reel_set_size=6&def_sa=11,11,11,11,12&scatters=1~10,5,1,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"1624695\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=12,1,11,12,5,3,11,12,8,12,12,9,12,12,12,4,12,6,10,11,12,12,12,13,1,7,12,12,12,9,12,12,9,12,12,12~12,12,12,4,7,12,12,1,12,12,12,8,12,12,12,1,12,9,3,9,12,12,12,13,11,12,12,12,5,12,12,11,12,11,12,10,12,6,12,9~12,10,12,12,12,9,12,12,12,1,12,12,11,1,9,12,13,12,11,12,12,12,4,5,12,12,12,9,6,8,11,3,12,12,12,9,12,12,7,11,1~12,12,8,12,12,11,12,12,6,11,12,12,9,12,10,1,12,12,12,11,12,3,12,12,5,12,4,12,12,13,12,1,12,12,7,12,9~12,12,10,11,12,1,12,12,9,12,6,12,12,7,12,13,12,13,12,12,12,3,12,12,4,12,12,8,9,12,1,12,12,11,12,12,9,5,12,12,11&reel_set2=8,12,8,4,5,7,1,3,7,11,3,12,6,11,6,12,1,3,4,5,9,10,4~12,6,3,10,11,4,7,4,5,8,6,5,3,12,3,6,10,8,9,5,7,1,12,11,12,1,4~1,12,10,3,12,10,4,1,5,11,5,9,4,7,8,11,12,6,7,3,8~3,8,5,11,10,6,3,7,11,5,12,10,8,1,9,6,12,4,10,4,6,7,3,12,3,4~10,5,4,6,10,9,5,1,8,1,7,5,4,8,11,4,11,12,10,3,6,7,6,3,12,3,12&reel_set1=12,12,12,1,8,12,11,12,1,9,7,6,9,12,12,10,5,12,9,12,12,12,11,12,12,12,1,12,3,11,12,11,12,12,12,4,9,12,12,12~6,11,12,12,12,11,1,12,12,12,3,7,9,12,12,5,12,12,12,1,12,12,12,9,12,12,11,12,12,12,10,12,11,1,9,8,9,12,12,4,12,12~5,12,12,9,12,10,12,12,9,1,3,1,11,12,12,12,7,12,11,12,11,12,12,12,6,12,4,9,12,1,12,9,12,12,8,12,11,12,12,12~12,4,12,12,9,5,12,3,12,12,11,12,12,6,12,12,12,7,8,12,1,11,12,12,9,12,11,12,12,10,9,12,12,1~12,11,12,12,12,1,9,12,1,12,12,12,10,12,12,12,9,6,12,12,12,7,8,9,3,4,12,11,5,12,11,12,12,12,9,12,12,12,11,12,1,12&reel_set4=12,12,9,1,6,7,11,12,12,12,11,5,4,12,12,9,12,9,12,12,12,11,12,1,12,8,12,12,12,1,10,12,9,11,12,12,12,3,12~12,9,12,12,4,12,12,10,12,12,11,12,9,12,12,1,12,12,12,5,8,12,12,7,12,3,12,12,1,9,12,12,11,12,11,6~8,1,6,7,12,12,12,11,12,1,12,3,12,12,12,9,11,12,9,12,11,12,5,12,12,12,9,12,12,3,9,8,10,12,12,10,1,4,12,12,5,12,12,12,6,12,7,11,12,9,12,1,12,11~12,9,12,12,1,12,12,11,12,12,7,12,12,9,12,12,12,10,12,12,11,12,5,12,4,6,3,12,9,12,12,1,12,8,11,12~3,12,12,12,11,12,11,4,9,11,12,12,12,1,5,12,12,11,12,12,12,6,1,12,12,12,10,12,1,12,12,8,12,7,9,12,9,12,12,12&purInit=[{bet:2000,type:\"default\"}]&reel_set3=1,4,12,1,12,12,1,12,6,5,1,13,3,12,12,1,8,12,12,10,1,12,12,9,1,11,7,9~12,4,1,12,12,1,12,12,6,1,9,12,1,13,1,12,12,7,1,9,12,8,11,10,1,5,12,10,3~12,1,12,8,12,12,1,10,12,1,7,13,1,10,12,9,1,9,7,12,1,11,6,12,12,4,1,12,9,8,10,1,12,1,12,12,1,5,11,3,1,12~8,12,12,11,1,8,12,5,11,12,12,6,1,12,12,1,10,7,10,1,13,4,10,7,12,1,12,12,1,12,12,1,9,1,3,9~1,12,1,12,1,12,1,12,12,1,12,7,1,5,7,8,1,12,1,10,12,3,1,12,1,8,9,12,10,1,13,12,11,12,10,4,12,1,6,12&reel_set5=12,11,12,11,3,12,12,9,6,12,12,12,13,9,8,12,10,12,12,12,9,12,12,12,9,13,12,12,12,11,13,12,12,4,12,13,12,7,12,12,11,5,12,12,12~10,8,12,11,12,5,12,9,12,12,4,7,13,12,6,9,3,12,12,12,11,12,12,12,13,9,12,12,12,11~12,12,9,12,9,12,12,8,12,12,13,5,12,12,4,12,9,12,12,12,10,12,13,12,12,6,12,11,12,7,12,11,12,12,13,12,11,12,12,3~12,12,8,12,3,12,13,12,9,12,12,13,12,12,4,12,12,12,11,12,11,12,6,12,5,12,12,9,12,12,11,12,9,12,12,7,12,12,10~12,13,12,11,12,12,12,6,5,12,11,12,12,12,13,12,12,12,9,12,8,3,7,12,12,12,11,12,12,12,13,4,10,12,12&total_bet_min=10.00";
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
        public CashBoxGameLogic()
        {
            _gameID = GAMEID.CashBox;
            GameName = "CashBox";
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
	
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in CashBoxGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, betInfo.LineCount);
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
                _logger.Error("Exception has been occurred in CashBoxGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
