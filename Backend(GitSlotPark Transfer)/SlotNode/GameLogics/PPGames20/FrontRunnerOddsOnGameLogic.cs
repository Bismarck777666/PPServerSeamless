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
    class FrontRunnerOddsOnGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10frontrun";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=13,11,13,10,13,11,9,10,9,10,12,10,9,13,9&cfgs=11780&ver=3&def_sb=10,13,8,11,11&reel_set_size=8&def_sa=10,12,11,12,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"94.51\",purchase:\"94.51\",regular:\"94.51\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"427023\",max_rnd_win:\"3000\",max_rnd_win_a:\"2000\",max_rnd_hr_a:\"259159\"}}&wl_i=tbm~3000;tbm_a~2000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~2000,200,50,5,0~1,1,1,1,1;3~0,0,0,0,0~1,1,1,1,1;4~0,0,0,0,0~1,1,1,1,1;5~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;60,20,10,0,0;60,20,10,0,0;30,10,5,0,0;30,10,5,0,0;30,10,5,0,0&total_bet_max=10,000,000.00&reel_set0=11,12,1,13,2,6,7,8,9,10~13,12,1,13,2,12,11,8,9,10,7,12,6,10,11~11,12,1,13,2,6,7,8,9,10~11,12,1,13,2,6,7,8,9,10~11,12,1,13,2,6,7,8,9,10&accInit=[{id:0,mask:\"h1;h2;h3;wp1;wp2;wp3;t;mul\"}]&reel_set2=11,12,13,6,7,8,9,10~12,11,8,10,12,13,11,1,7,9,6,13~11,12,1,13,6,7,8,9,10~10,12,11,7,6,11,12,13,10,13,10,8,9~11,12,13,6,7,8,9,10&reel_set1=11,12,1,13,6,7,8,9,10~8,9,12,13,9,6,11,12,11,7,12,13,10~7,12,6,11,9,8,10,11,13,9,10,12,13~11,12,1,13,6,7,8,9,10~12,6,7,8,13,9,10,13,9,10,11,12&reel_set4=11,12,1,13,6,7,8,9,10~12,9,13,7,11,8,10,9,13,12,10,12,6,11~11,12,1,13,6,7,8,9,10~8,10,13,7,9,13,9,10,11,12,10,11,6,12~6,11,12,11,13,10,13,10,9,8,13,9,12,7&purInit=[{bet:1000,type:\"default\"}]&reel_set3=6,13,12,7,9,10,13,10,8,9,11,12,11,13~12,6,13,11,9,11,13,7,12,1,8,10,12,10~11,12,13,6,7,8,9,10~11,12,1,13,6,7,8,9,10~11,12,13,6,7,8,9,10&reel_set6=12,13,12,13,11,9,12,10,7,2,9,10,13,11,10,12,8,12,13,6,11,13,10,12,8,13,12,8,12~11,3,8,10,12,10,8,12,13,11,10,13,6,9,13,9,7,12,11,2,13,12,13,12~7,10,12,4,13,2,8,12,11,10,9,6,11,12,13~13,12,9,13,9,11,13,10,12,8,7,13,6,12,2,12,11,10,5~13,9,12,8,12,13,7,12,10,13,12,11,9,11,13,11,2,10,6&reel_set5=11,12,13,2,6,7,8,9,10~11,12,13,2,3,6,7,8,9,10~6,9,13,7,12,8,11,9,10,2,4,12,13,10,12~11,12,13,2,5,6,7,8,9,10~10,2,9,13,12,11,10,11,6,12,8,13,11,9,12,7&reel_set7=2,7,8,9~2,8,9~2,7,8,9~2,7,8,9,10~9,12,13,6,13,8,11,13,9,12,10,12,11,10,2,7&total_bet_min=20.00";
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
        public FrontRunnerOddsOnGameLogic()
        {
            _gameID = GAMEID.FrontRunnerOddsOn;
            GameName = "FrontRunnerOddsOn";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "5";
	        dicParams["bl"]         = "0";
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
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();
		
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FrontRunnerOddsOnGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in FrontRunnerOddsOnGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in FrontRunnerOddsOnGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
