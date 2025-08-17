using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FangtasticFreespinsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10fangfree";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return false;
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
                return "def_s=6,12,4,6,12,3,9,8,3,8,4,11,12,4,10&cfgs=1&ver=3&mo_s=7&mo_v=20,50,100,150,200,250,500,20000,1,2,3&def_sb=5,12,5,6,8&reel_set_size=5&def_sa=6,12,6,5,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.51\",purchase:\"96.52\",regular:\"96.51\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"1362769\",max_rnd_win:\"2100\",max_rnd_win_a:\"1400\",max_rnd_hr_a:\"707263\"}}&wl_i=tbm~2100;tbm_a~1400&sc=10.00,20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,5,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0&total_bet_max=10,000,000.00&reel_set0=9,7,8,7,10,11,4,8,5,12,7,9,1,8,6,11,7,8,7,3,9,7,12,6,10,8,7,7,7,7,7,7,10,5,9,12,6,4,7,10,9,7,8,1,7,6,11,3,7,11,4,3,7,12,10,9,5,6,4,6,6~11,9,4,7,10,6,12,1,3,8,7,7,6,11,5,6,11,12,9,11,7,6,10,7,6,8,7,7,7,7,7,7,4,5,9,7,7,8,7,8,6,8,10,7,4,12,8,6,1,11,5,10,12,5,7,3,7,4,9,6~9,1,7,5,7,6,10,11,8,9,4,10,9,11,8,10,7,12,3,6,7,7,6,5,11,7,7,7,7,7,7,12,7,7,9,7,10,4,11,5,9,8,12,6,1,10,9,8,12,6,7,10,7,3,11,4,6,7,5,7,8~6,3,7,7,11,7,9,12,5,9,5,7,6,9,1,9,7,1,8,6,11,7,6,4,6,12,8,7,7,7,7,7,7,7,8,9,7,11,5,10,8,10,11,12,6,4,9,4,8,3,6,7,10,12,5,10,12,7,7,8,6,4,7~7,9,7,7,4,9,12,11,5,8,10,11,7,7,7,7,7,7,7,9,12,3,7,6,4,6,12,7,8,6,7,11,6,1,7,10&accInit=[{id:0,mask:\"l;s;b\"}]&reel_set2=8,5,7,6,7,6,4,10,7,3,7,7,5,8,10,9,6,9,7,9,7,7,7,7,7,7,1,4,11,10,8,12,8,5,4,11,6,3,7,7,12,9,6,9,11,8,6,12,3,1,10~7,10,1,7,11,12,9,12,10,6,7,7,9,6,10,7,4,5,7,8,6,12,7,7,7,7,7,7,4,11,10,7,6,5,7,8,6,8,4,7,3,6,3,5,11,1,12,9,6,8~10,7,5,12,8,9,7,7,12,7,1,10,3,9,11,6,9,3,4,11,5,6,11,7,7,7,7,7,7,10,8,1,7,7,6,8,9,7,9,8,7,4,6,10,5,12,8,10,7,6,4,6,11~7,9,7,3,7,12,9,12,4,1,5,8,7,9,10,6,9,7,7,7,7,7,7,7,1,6,11,5,6,4,10,5,6,10,8,6,7,12,8,12,11,8,4,7,11,6~6,3,6,8,7,6,7,5,11,4,9,10,1,6,7,7,4,9,7,12,11,6,9,7,7,7,7,7,7,7,8,7,12,7,5,10,6,12,8,7,7,3,8,10,4,11,9,7,11,6,5,9,12,1&reel_set1=8,7,10,6,1,10,9,8,6,5,12,6,4,7,7,6,3,10,12,6,7,7,7,7,7,7,9,4,9,10,7,8,11,3,6,7,11,7,6,5,1,7,7,8,4,7,11,3~6,8,5,8,4,6,7,1,11,9,6,8,4,10,6,9,7,6,10,7,7,4,6,9,12,11,7,7,7,7,7,7,3,5,11,7,10,7,11,8,5,7,8,1,7,6,11,7,9,7,4,12,7,3,8,6,12,10,12,5,12~7,5,7,8,4,5,10,4,6,7,6,7,9,12,5,10,12,11,5,6,7,9,6,8,11,10,7,7,7,7,7,7,3,7,3,1,10,7,7,4,9,11,7,8,10,4,10,11,8,7,9,8,9,11,1,12,7,6,8,12~7,5,10,12,6,7,11,3,7,9,7,7,8,3,4,6,7,7,9,7,5,7,7,7,7,7,7,7,12,8,4,11,6,8,12,5,8,9,1,4,12,1,7,10,6,10,6,8,9,6,11~6,8,7,12,6,11,7,3,10,12,6,9,6,9,7,12,7,7,8,5,6,4,7,7,7,7,7,7,7,8,4,11,8,10,4,5,12,7,7,9,10,9,7,11,3,7,1,10,7,7,6,5,6&reel_set4=4,3,11,10,8,10,12,8,11,12,8,4,7,9,10,12,6,12,6,9,10,7,8,10,12,10,5,8,12,7,8,10,8,12~3,9,4,10,11,7,5,8,9,6,3,8,11,7,7,7,12,5,9,11,9,11,10,9,12,7,7,11,7,7,9~12,7,11,6,4,9,11,10,6,7,7,7,5,4,8,5,3,8,5,9,12,6,7,7~6,10,5,7,4,9,7,5,11,3,6,4,12,6,7,5,7,8~9,12,8,4,5,7,5,8,10,7,7,7,11,10,7,6,12,9,6,7,3,4,7&purInit=[{bet:1000,type:\"default\"}]&reel_set3=9,7,7,12,7,9,5,11,6,10,4,7,7,6,7,5,4,8,4,8,7,11,7,7,7,7,7,7,8,5,1,12,9,10,11,8,10,9,10,3,6,3,12,6,7,1,8,6,9,3,6~6,7,9,7,4,10,6,1,6,7,5,9,5,10,9,1,6,7,11,12,8,6,7,8,11,9,4,7,7,7,7,7,7,12,7,8,7,11,6,7,12,3,7,4,10,11,10,7,4,10,8,12,8,7,1,11,6,12,6,8,5,7,3~6,7,7,10,8,4,5,7,9,12,8,12,7,11,6,3,4,6,11,9,10,11,7,7,7,7,7,7,9,11,1,7,10,7,12,9,10,3,6,7,4,5,7,8,5,7,1,8,10,8,7,6~3,8,6,7,7,9,11,8,6,1,12,7,12,7,4,6,5,1,3,1,7,7,7,7,7,7,7,11,12,7,9,8,6,9,10,4,9,10,6,7,7,12,8,4,10,5,7,8,5,9~11,4,7,7,6,5,11,10,6,11,7,7,12,6,5,10,9,8,12,1,9,7,7,7,7,7,7,7,10,4,9,12,6,7,8,7,9,11,7,5,3,8,12,4,7,7,6,3,10,8,1&total_bet_min=100.00";
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
        public FangtasticFreespinsGameLogic()
        {
            _gameID     = GAMEID.FangtasticFreespins;
            GameName    = "FangtasticFreespins";
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
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in vs10fangfreeGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in vs10fangfreeGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in vs10fangfreeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
