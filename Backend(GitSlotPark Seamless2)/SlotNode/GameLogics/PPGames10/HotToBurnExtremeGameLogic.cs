using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class HotToBurnExtremeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40hotburnx";
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
            get { return 40; }
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
                return "def_s=5,7,7,5,6,3,9,9,4,8,5,7,8,5,7,4,6,4,3,8&cfgs=6355&ver=3&def_sb=4,7,6,5,7&reel_set_size=4&def_sa=3,7,7,3,6&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2000000\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=5.00,10.00,15.00,20.00,25.00,50.00,75.00,100.00,125.00,190.00,250.00,375.00,625.00,1250.00,1875.00,2500.00&defc=25.00&purInit_e=1&wilds=2~5000,1000,100,2,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,1000,100,0,0;280,96,28,0,0;280,96,28,0,0;96,28,8,0,0;96,28,8,0,0;96,28,8,0,0;96,28,8,2,0&total_bet_max=10,000,000.00&reel_set0=9,5,9,4,6,9,6,8,8,6,8,6,6,6,6,6,6,5,8,6,1,5,8,6,8,2,4,6,1,8,8,8,8,8,6,4,5,9,7,7,8,7,7,8,8,6,7,7,7,8,5,6,6,3,6,8,6,8,4,8,8,5~9,1,4,3,7,4,8,8,8,4,7,8,4,6,9,9,9,7,9,7,4,4,7,6,6,6,4,6,7,9,4,4,4,7,9,8,7,7,6,7,7,7,4,5,7,6,4,7,2,5,5~7,4,7,9,6,9,9,9,4,4,6,8,6,5,8,6,6,6,9,5,5,3,9,8,7,7,7,6,8,6,6,7,2,8,8,8,7,8,5,5,8,7,5,5,5,6,9,5,1,5,9,9,7~5,6,5,8,2,6,7,7,7,8,5,4,7,3,6,6,6,4,7,7,9,6,7,1,8,8,8,7,9,9,8,6,8,4,9,9,9,8,9,9,6,4,5,8,6,6~8,6,8,7,8,8,8,5,2,5,6,9,4,8,7,7,7,7,5,6,9,7,8,6,1,6,6,6,4,9,8,6,8,6,3,9,9,9,6,1,7,3,5,5,9,5,5,5,5,7,6,9,7,9,4,7,6&reel_set2=8,5,2,7,3,2,3,8,3,6,3,8,7,6,4,2,4,1,9,2,4~4,6,7,2,9,9,6,2,5,1,9,9,9,2,3,5,6,2,3,8,5,9,7,2~2,3,2,6,1,5,3,3,3,4,9,4,7,2,3,3,6,8~2,6,2,2,7,8,5,4,9,1,5,7,9,2,6,6,2,5,3,8,6,2,5,2,9~7,8,6,5,5,2,3,2,6,6,6,2,3,6,9,7,4,8,2,2,9,1&reel_set1=8,9,3,7,2,4,6,4,9,9,9,5,7,1,5,9,8,9,1,6,4,7,7,7,1,6,4,2,7,2,6,3,4,8,6,6,6,6,5,8,9,9,8,8,3,7,6,7,8~4,1,7,9,2,6,5,7,9,7,9,7,6,9,4,9,9,9,3,9,7,9,5,2,4,5,7,6,5,9,6,5,4,2,4,4,4,4,4,7,8,9,2,4,6,3,8,5,8,8,3,5,9,5,2,8,4~9,5,4,2,8,3,3,3,1,6,4,2,7,5,3,8,8,8,9,6,8,7,7,6,9,9,9,6,3,4,9,4,9,9,7,7,7,2,8,8,7,8,6,6,6,2,3,9,1,8,9,6,6~7,8,6,9,5,9,9,5,5,5,5,8,3,8,5,8,3,6,2,7,3,3,3,6,3,8,1,4,3,2,5,4,7,7,7,9,6,9,2,9,7,6,7,8,6,6,6,5,9,7,8,9,6,5,3,5,8,8,8,5,8,3,5,7,6,7,6,2,5~7,1,8,6,3,8,8,8,2,4,5,9,7,2,8,9,3,7,7,7,2,6,6,4,6,7,9,5,5,5,7,6,9,8,4,3,5,3,8,9&purInit=[{bet:4000,type:\"default\"}]&reel_set3=8,5,8,6,6,6,6,1,8,6,8,6,4,8,8,8,6,4,3,1,8,6~1,7,9,7,4,4,1,9,7,4,9,4,4,9,9~7,1,9,6,8,9,9,9,3,1,9,6,9,7,5,9~9,6,7,4,1,6,6,6,3,7,2,6,8,7,5,5~8,2,5,8,1,7,6,8,8,8,3,9,4,8,4,7,9,1,7&total_bet_min=5.00";
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
            get { return false; }
        }
	
	
        #endregion
        public HotToBurnExtremeGameLogic()
        {
            _gameID = GAMEID.HotToBurnExtreme;
            GameName = "HotToBurnExtreme";
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
	
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in HotToBurnExtremeGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
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
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HotToBurnExtremeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
