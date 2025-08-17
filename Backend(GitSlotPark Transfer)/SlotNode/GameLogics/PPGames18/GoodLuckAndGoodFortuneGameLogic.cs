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
    class GoodLuckAndGoodFortuneGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10luckfort";
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
                return "def_s=3,12,6,7,11,7,13,10,6,10,3,10,12,5,12&cfgs=9396&ver=3&def_sb=6,12,8,5,11&reel_set_size=3&def_sa=3,10,7,7,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"3365303\",max_rnd_win:\"2100\",max_rnd_win_a:\"1400\",max_rnd_hr_a:\"2175947\"}}&wl_i=tbm~2100;tbm_a~1400&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~9000,2500,250,10,0~1,1,1,1,1&bonuses=0&bls=10,15&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;750,125,25,2,0;750,125,25,2,0;400,100,20,0,0;250,75,15,0,0;250,75,15,0,0;125,50,10,0,0;125,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,2,0&total_bet_max=10,000,000.00&reel_set0=10,1,6,2,5,8,13,8,5,9,4,9,12,11,12,11,3,6,7,10,1,12,9,7,10~7,12,13,11,13,12,2,1,3,8,7,10,11,9,6,13,9,4,11,10,5,12,11,13,1,9,12,13,9,8,7,5,10,8,3,9~8,6,3,13,1,12,13,3,12,6,10,8,4,11,10,12,2,6,10,4,8,9,11,5,13,1,12,7,5,7~10,11,6,13,12,8,6,5,11,4,7,11,9,6,4,8,13,10,1,3,9,12,7,8,11,3,11,12,10,6,9,7,9,13,5,4,2~8,5,10,13,9,2,11,12,3,13,10,1,3,6,10,9,8,6,7,11,8,10,3,4,13,6,7,9,12,8,5,9,4,3&reel_set2=8,1,12,2,1,9,10,11,6,4,10,5,11,8,12,11,10,3,13,7,12,9,7,6~10,7,11,12,1,7,9,11,1,13,9,11,8,11,8,12,5,9,2,8,10,13,11,8,9,7,13,6,9,10,4,7,6,5,3,8,11~13,8,7,8,7,10,13,12,6,10,5,3,10,8,11,10,12,13,12,5,12,6,5,2,4,3,10,5,6,13,4,3,9,4,1~8,3,1,10,9,7,5,12,11,2,4,9,7,13,6,10,7,11,12,10,4,11,13,11,13,12,9,6,7,4,8,11~13,3,11,10,9,2,9,8,13,9,12,3,9,5,4,9,10,3,13,8,3,5,8,10,7,6,5,11,10,6,11,1,4&reel_set1=5,7,10,8,13,9,12,11,3,6,4~13,8,7,5,10,9,12,11,6~7,8,13,10,9,12,11,3,6~9,12,8,13,12,3,12,11,10,13,10~10,5,7,8,13,9,12,11,3,6&purInit=[{bet:1000,type:\"default\"}]&total_bet_min=20.00";
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
        public GoodLuckAndGoodFortuneGameLogic()
        {
            _gameID = GAMEID.GoodLuckAndGoodFortune;
            GameName = "GoodLuckAndGoodFortune";
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in GoodLuckAndGoodFortuneGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in GoodLuckAndGoodFortuneGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in GoodLuckAndGoodFortuneGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
