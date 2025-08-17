using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class YouCanPiggyBankOnItGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10piggybank";
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
                return "def_s=4,11,10,8,6,12,3,9,12,3,8,5,11,10,11&cfgs=1&ver=3&def_sb=4,6,11,4,9&reel_set_size=5&def_sa=10,5,8,12,5&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.52\",purchase:\"96.53\",regular:\"96.53\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"532424\",max_rnd_win:\"2100\",max_rnd_win_a:\"1400\",max_rnd_hr_a:\"270233\"}}&wl_i=tbm~2100;tbm_a~1400&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=12,3,8,6,3,7,9,5,7,7,9,5,7,7,11,10,7,7,9,8,5,7,6,1,11,10,7,7,7,7,7,7,4,7,5,4,9,12,6,3,6,1,8,10,8,12,8,11,7,9,4,11,9,6,8,6,12,10,4,7~10,4,3,9,5,7,7,6,1,6,8,4,6,7,8,5,11,10,1,6,11,7,12,10,12,7,7,7,7,7,7,11,3,7,8,6,12,5,7,4,8,7,8,12,7,6,9,6,7,5,10,7,11,9,12,7,9,4~10,7,8,7,3,9,4,8,6,10,6,4,9,5,10,12,7,9,5,12,8,5,8,7,4,7,7,7,7,7,7,11,7,9,7,1,7,4,7,7,6,10,8,11,9,7,12,1,10,6,5,12,7,6,9,6,11,8,11,7~9,12,1,7,9,7,8,5,6,7,12,8,7,5,6,9,7,5,7,7,4,8,6,12,11,6,7,7,7,7,7,7,7,8,6,10,9,11,6,9,7,10,4,10,7,7,11,6,3,6,3,11,5,9,12,8,4,10,8,7~12,8,12,7,8,12,10,11,10,6,9,7,6,8,6,7,7,7,7,7,7,7,6,4,5,7,6,9,7,7,1,9,4,3,10,7,11,9,7,7&reel_set2=8,9,12,7,5,7,12,7,11,10,3,8,7,6,9,6,7,9,11,9,7,6,7,7,7,7,7,7,10,11,7,4,3,9,6,5,6,7,3,10,8,6,7,1,8,12,10,4,5,1,7~5,11,8,6,12,11,7,6,7,6,7,8,4,1,10,4,12,5,8,7,7,7,7,7,7,9,5,7,6,7,11,6,4,7,12,8,10,12,11,9,1,10,7,10,7,7,3,9~8,6,5,11,6,8,7,10,6,7,6,12,3,12,9,7,7,10,3,8,5,11,4,9,7,5,4,7,7,7,7,7,7,8,11,7,1,10,12,9,10,6,8,4,9,12,7,5,8,7,8,7,6,4,7,6,11,9,7,7,1,10~9,6,7,6,11,8,4,7,4,5,7,11,12,1,6,7,12,6,9,6,12,4,8,11,7,7,4,10,7,7,7,7,7,7,7,6,12,5,7,9,3,7,9,6,8,6,7,1,5,10,8,10,7,5,9,7,9,8,12,7,10,3,11~7,5,3,8,7,10,7,5,10,9,12,4,12,11,6,7,4,7,9,6,7,11,9,1,6,7,11,8,7,7,7,7,7,7,7,5,12,7,6,1,10,7,4,8,3,9,12,7,11,8,6,7,12,10,6,8,1,5,10,11,6,7,9,4,6&reel_set1=5,7,10,8,7,7,3,4,6,8,9,7,10,9,6,7,7,7,7,7,7,6,5,3,11,7,8,12,8,4,11,6,1,9,12,7,7,10,6~5,3,8,6,10,8,6,3,5,7,11,6,1,10,8,7,7,12,4,6,7,7,7,7,7,7,6,5,9,12,7,8,7,7,9,12,11,7,12,6,7,11,4,6,9,1,11,8,10~12,11,7,7,9,6,8,4,11,8,11,8,6,10,5,7,7,7,7,7,7,10,7,10,8,6,10,4,9,6,7,1,9,12,7,3~6,7,7,9,4,11,9,6,12,4,8,7,12,1,7,12,4,11,5,7,7,7,7,7,7,7,6,3,9,8,5,11,10,7,8,6,7,8,6,10,9,3,7,5,1,10,6~3,10,7,7,8,7,11,8,10,7,6,1,5,6,7,12,4,7,3,7,11,7,9,6,8,6,5,11,7,7,7,7,7,7,7,12,10,6,12,9,6,10,5,7,11,12,1,4,7,9,6,7,11,6,1,4,9,10,8,4,12,8,9,5&reel_set4=12,8,1,12,7,8,12,8,10,12,4,5,11,9,1,11,10,6,7,7,6,9,8,4,8,10,3,12,10,8~11,8,6,11,10,9,7,8,7,7,12,11,10,7,7,7,3,9,4,9,7,12,9,11,5,7,7,11,3,5~7,3,1,6,10,7,4,7,7,7,9,7,8,5,7,11,6,4,12~10,5,8,5,7,4,7,11,7,6,5,9,7,7,6,3,4,3,12~1,8,7,7,9,3,4,5,7,11,7,7,7,10,8,5,11,4,12,6,9,12,6,7&purInit=[{bet:1000,type:\"default\"}]&reel_set3=11,10,7,9,6,3,9,8,12,6,7,11,10,4,8,6,11,5,8,5,7,7,7,7,7,7,3,4,3,1,7,4,12,7,9,7,8,7,6,5,10,12,7,6,7,7,9,1,6,9,8~10,8,6,4,9,5,4,10,8,11,5,6,12,7,11,7,6,7,11,10,6,7,7,7,7,7,7,1,7,1,7,12,7,11,6,7,4,8,5,7,8,12,3,9,10,1,12,3,7,6~6,8,3,8,12,7,5,8,7,4,5,9,6,7,7,7,7,7,7,11,4,10,11,10,11,10,8,9,12,9,7,7,1,6,7,6~7,9,7,6,8,9,1,9,3,6,7,12,4,8,11,7,7,7,7,7,7,7,1,6,12,5,9,12,11,8,10,7,4,5,10,6,7,6~7,10,5,3,11,8,4,9,6,7,12,7,10,1,6,1,8,12,7,6,4,7,7,7,7,7,7,7,8,10,7,9,5,7,12,11,3,7,12,7,5,6,9,6,11,7,8,7,6,7,4,11,9&total_bet_min=200.00";
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
        
        public YouCanPiggyBankOnItGameLogic()
        {
            _gameID     = GAMEID.YouCanPiggyBankOnIt;
            GameName    = "YouCanPiggyBankOnIt";
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

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);

            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in YouCanPiggyBankOnItGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in YouCanPiggyBankOnItGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in YouCanPiggyBankOnItGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            base.overrideSomeParams(betInfo, dicParams);

            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
    }
}
