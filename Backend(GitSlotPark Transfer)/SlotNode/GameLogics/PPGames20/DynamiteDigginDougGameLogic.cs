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
    class DynamiteDigginDougGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10dyndigd";
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
                return "def_s=3,13,4,8,14,6,14,11,3,12,7,10,10,9,11&cfgs=10957&ver=3&def_sb=9,11,12,8,11&reel_set_size=3&def_sa=3,14,6,6,13&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;14~1000,500,200,100,50,25,10,5,0,0~0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1;15~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;16~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"1234872\",max_rnd_win:\"3000\",max_rnd_win_a:\"2000\",max_rnd_hr_a:\"413393\"}}&wl_i=tbm~3000;tbm_a~2000&sc=10.00,20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,500,200,20,0;500,200,100,10,0;200,100,50,5,0;100,50,20,0,0;100,50,20,0,0;50,20,10,0,0;50,20,10,0,0;50,20,10,0,0;20,10,5,0,0;20,10,5,0,0;20,10,5,0,0;0,0,0,0,0,0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=10,4,15,6,11,11,11,9,13,13,12,5,12,12,12,3,6,8,13,11,9,9,9,11,9,8,14,11,10,10,10,14,12,12,7,7~6,12,11,14,13,4,8,12,12,12,10,6,7,11,12,11,9,14,13,13,13,7,13,5,13,10,9,12,8,3~13,14,10,13,15,11,11,11,7,5,12,14,13,12,12,4,12,12,12,3,9,9,13,16,15,11,10,13,13,13,11,11,7,4,15,14,6,8,10,10,10,6,8,12,6,9,5,10,7,8~5,12,13,11,7,11,11,11,14,4,12,10,7,6,11,12,12,12,9,6,12,10,14,13,14,13,13,13,8,13,9,10,8,12,9,10,10,10,11,7,13,6,3,8,4,11~11,8,10,13,15,11,11,11,8,12,9,6,7,14,12,12,12,14,13,9,5,12,5,14,13,13,13,11,3,12,4,12,6,9,9,9,11,7,10,10,9,15,13,8&reel_set2=18,17,17,17,17~17,18,17,17,17~17,17,18,17,17~17,17,17,18,17~17,17,17,17,18&reel_set1=6,5,14,11,8,11,11,11,14,6,5,15,7,15,12,12,12,14,10,8,11,7,8,10,9,9,9,3,12,9,13,9,12,13,10,10,10,4,12,12,9,11,13,13~14,7,9,10,5,13,8,6,11,9,12,12,12,8,12,13,12,5,11,11,10,12,14,7,6,13,13,13,10,9,3,6,12,4,7,14,8,13,13,11,4~14,11,7,10,15,11,11,11,14,11,5,10,11,15,16,12,12,12,4,5,8,13,9,12,13,13,13,3,15,9,14,12,10,4,10,10,10,6,9,7,8,12,13,8,6~9,8,8,11,9,10,13,11,11,11,4,13,6,7,6,12,3,12,12,12,10,6,11,11,5,7,14,13,13,13,14,12,10,13,11,13,11,10,10,10,12,7,10,12,9,14,9,8,8,5~11,12,4,9,11,11,11,7,10,6,13,14,15,13,12,12,12,11,10,8,11,14,12,12,13,13,13,14,7,13,5,15,3,12,9,9,9,10,8,9,5,13,6,8&purInit=[{bet:1000}]&total_bet_min=100.00";
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
        
        public DynamiteDigginDougGameLogic()
        {
            _gameID     = GAMEID.DynamiteDigginDoug;
            GameName    = "DynamiteDigginDoug";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{after:{def_s:\"17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17\",def_sa:\"17,17,17,17,17\",def_sb:\"17,17,17,17,17\",s:\"17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17\",sa:\"17,17,17,17,17\",sb:\"17,17,17,17,17\",sh:\"4\",st:\"rect\",sw:\"5\"},before:{def_s:\"17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17\",def_sa:\"17,17,17,17,17\",def_sb:\"17,17,17,17,17\",s:\"17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17\",sa:\"17,17,17,17,17\",sb:\"17,17,17,17,17\",sh:\"4\",st:\"rect\",sw:\"5\"}}";
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DynamiteDigginDougGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in DynamiteDigginDougGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in DynamiteDigginDougGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
