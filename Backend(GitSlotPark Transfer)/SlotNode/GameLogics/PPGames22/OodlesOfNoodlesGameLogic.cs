using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class OodlesOfNoodlesGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10noodles";
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
                return "def_s=6,12,7,6,10,5,8,5,4,9,6,10,6,6,10&cfgs=1&ver=3&def_sb=3,8,11,7,9&reel_set_size=6&def_sa=5,12,11,7,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.58\",purchase:\"96.58\",regular:\"96.58\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"531872\",max_rnd_win:\"5100\",max_rnd_win_a:\"3400\",max_rnd_hr_a:\"305159\"}}&wl_i=tbm~5100;tbm_a~3400&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=6,12,1,3,11,12,5,7,8,3,4,9,5,6,8,3,9,10,7,5~6,8,11,3,4,9,3,5,6,10,1,12,10,7,4~10,6,9,7,8,4,11,6,4,1,12,5,3,12,11,12,3,5~4,12,8,9,4,12,3,6,5,11,5,7,10,7,4,12,1,11,7~11,12,1,3,4,5,6,7,8,9,10&reel_set2=12,3,5,6,12,8,5,1,3,9,7,8,5,9,3,4,11,7,4,10,5~11,12,3,4,5,6,7,8,9,10~11,5,12,6,4,6,10,9,10,7,5,12,4,12,3,8,4,3~12,11,5,4,5,10,11,3,12,7,11,7,8,1,5,12,4,9,4,6,7~11,12,3,4,5,6,7,8,9,10&reel_set1=6,5,11,3,7,5,6,12,3,9,4,1,7,5,10,8,9,12,5,3,8~11,12,3,4,5,6,7,8,9,10~10,8,12,3,4,10,5,4,11,9,5,6,1,6,3,7,10,12~11,12,3,4,5,6,7,8,9,10~11,12,3,4,5,6,7,8,9,10&reel_set4=11,12,3,4,5,6,7,8,9,10~11,12,1,3,4,5,6,7,8,9,10~11,12,3,4,5,6,7,8,9,10~11,12,1,3,4,5,6,7,8,9,10~11,12,3,4,5,6,7,8,9,10&purInit=[{bet:1000,type:\"fs\"}]&reel_set3=11,12,3,4,5,6,7,8,9,10~1,10,4,8,12,4,3,7,6,9,5,10,11,3,7,6~11,12,1,3,4,5,6,7,8,9,10~11,12,3,4,5,6,7,8,9,10~11,12,3,4,5,6,7,8,9,10&reel_set5=5,4,10,3,9,3,5,11,4,3,7,12,5,7,8,9,12,8,6~4,8,3,6,4,7,11,3,10,6,10,9,5,7,12~11,10,3,8,10,4,9,3,6,12,5,11,12,4,12,7,5,6~9,4,5,11,7,12,5,8,12,10,6,3,4,11,7,4,7,5~11,12,3,4,5,6,7,8,9,10&total_bet_min=20.00";
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
        
        public OodlesOfNoodlesGameLogic()
        {
            _gameID = GAMEID.OodlesOfNoodles;
            GameName = "OodlesOfNoodles";
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in vs10noodlesGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in vs10noodlesGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in vs10noodlesGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
