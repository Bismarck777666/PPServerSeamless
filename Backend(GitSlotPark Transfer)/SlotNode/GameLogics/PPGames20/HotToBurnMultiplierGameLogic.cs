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
    class HotToBurnMultiplierGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5hotbmult";
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
                return "def_s=4,8,4,4,9,5,6,6,3,7,4,9,9,4,8&cfgs=10539&ver=3&def_sb=5,9,6,3,7&reel_set_size=1&def_sa=5,8,3,5,9&scatters=1~50,10,2,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"102246\",max_rnd_win:\"3000\",max_rnd_win_a:\"2000\",max_rnd_hr_a:\"94320\"}}&wl_i=tbm~3000;tbm_a~2000&sc=10.00,20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&wilds=&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;6000,1000,100,0,0;500,200,50,0,0;500,200,50,0,0;200,50,20,0,0;200,50,20,0,0;200,50,20,0,0;200,50,20,10,0&reel_set0=7,6,8,4,6,6,6,6,9,5,1,9,4,8,8,8,5,8,7,5,6,8,9,9,9,5,6,6,7,7,7,7,3,9,9,8,6,3,8~6,3,7,9,8,8,8,4,7,4,6,5,9,9,9,9,7,8,7,1,5,3,6,6,6,9,8,6,8,8,7,7,7,9,4,8,7,9,8,5~4,9,9,9,5,6,1,4,7,7,7,6,7,6,6,6,7,9,5,9,8,8,8,5,8,8,3,8~7,9,8,7,7,7,7,8,5,4,7,9,6,6,6,9,7,1,3,6,8,8,8,3,7,6,6,4,9,9,9,8,5,9,5,6,8~4,9,6,8,8,8,1,8,7,7,7,8,4,3,9,6,6,6,5,9,3,9,9,9,5,7,6,9,8,7";
            }
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
        public HotToBurnMultiplierGameLogic()
        {
            _gameID     = GAMEID.HotToBurnMultiplier;
            GameName    = "HotToBurnMultiplier";
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
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();
		
                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;
		
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in HotToBurnMultiplierGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in HotToBurnMultiplierGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
