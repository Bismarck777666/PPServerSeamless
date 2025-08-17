using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DingDongChristmasBellsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10ddcbells";
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
                return "def_s=14,17,16,14,15,17,15,14,12,16,16,16,17,15,14&cfgs=9098&ver=3&def_sb=13,13,12,16,15&reel_set_size=2&def_sa=13,13,12,17,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"247075\",max_rnd_win:\"2100\",max_rnd_win_a:\"1400\",max_rnd_hr_a:\"137227\"}}&wl_i=tbm~2100;tbm_a~1400&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~2000,500,100,0,0~1,1,1,1,1;3~2000,500,100,0,0~1,1,1,1,1;4~2000,500,100,0,0~1,1,1,1,1;5~2000,500,100,0,0~1,1,1,1,1;6~2000,500,100,0,0~1,1,1,1,1&bonuses=0&bls=10,15&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,500,100,0,0;500,250,80,0,0;500,250,80,0,0;300,150,50,0,0;300,150,50,0,0;160,80,30,0,0;160,80,30,0,0;160,80,30,0,0;60,30,15,0,0;60,30,15,0,0;40,20,10,0,0&total_bet_max=10,000,000.00&reel_set0=7,8,15,9,17,12,13,15,11,17,15,13,10,14,17,12,16,14,16,17,13~16,13,15,17,10,16,15,16,13,11,17,8,15,7,15,17,15,16,15,14,13,16,9,17,13,15,12~14,17,7,16,8,16,10,15,16,17,14,16,17,11,17,12,16,14,17,9,13,12,14,17,12,16,15,17~15,16,14,12,17,14,17,10,16,14,12,15,12,13,11,14,8,7,16,12,13,15,17,9~15,14,16,13,16,17,7,15,12,15,13,16,11,10,16,13,15,9,12,8,17,15,12,14,15,14,16,17,16&reel_set1=12,17,10,17,7,8,9,16,11,15,13,15,17,15,12,14,17,15,14,16,13~12,16,8,15,10,13,17,7,17,15,17,9,16,15,11,16,17,13,14,17,15~16,12,17,12,7,10,15,7,8,15,8,14,16,17,9,17,14,17,9,13,15,17,13,15,16,14,12,10,16,17,11~8,17,7,14,13,15,13,14,12,16,12,10,11,12,14,16,15,16,17,9,15~12,16,13,15,12,16,13,11,14,10,8,16,7,9,16,15,13,17,14,15,16,17&purInit=[{bet:1000,type:\"fs\"}]&total_bet_min=20.00";
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
        public DingDongChristmasBellsGameLogic()
        {
            _gameID = GAMEID.DingDongChristmasBells;
            GameName = "DingDongChristmasBells";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DingDongChristmasBellsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in DingDongChristmasBellsGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in DingDongChristmasBellsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
