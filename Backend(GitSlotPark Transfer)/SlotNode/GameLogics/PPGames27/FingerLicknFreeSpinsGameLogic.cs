using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class FingerLicknFreeSpinsBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 5.0f;
            }
        }
    }
    class FingerLicknFreeSpinsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10fingerlfs";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
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
                return "def_s=3,8,8,4,7,5,9,9,5,6,4,7,8,3,8&cfgs=1&ver=3&def_sb=3,7,7,3,9&reel_set_size=1&def_sa=5,6,5,4,9&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.55\",purchase:\"96.55\",regular:\"96.55\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"411592\",max_rnd_win:\"6000\",max_rnd_win_a:\"3000\",max_rnd_hr_a:\"168452\"}}&wl_i=tbm_a~3000;tbm~6000&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&purInit_e=1&wilds=2~500,50,10,0,0~1,1,1,1,1&bonuses=0&bls=5,10&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,50,10,0,0;120,12,4,0,0;120,12,4,0,0;120,12,4,0,0;30,6,2,0,0;30,6,2,0,0;30,6,2,0,0&total_bet_max=10,000,000.00&reel_set0=7,9,6,8,6,6,6,6,5,7,3,6,8,8,8,5,4,7,9,6,9,9,9,6,8,5,3,9,7,7,7,7,5,8,9,8,4,6~3,8,8,8,4,3,5,8,9,9,9,9,8,4,6,7,6,6,6,9,7,9,5,7,7,7,8,7,9,7,6~9,7,9,9,9,8,8,9,7,7,7,6,5,9,6,6,6,5,6,5,7,8,8,8,4,4,3,8,3~4,5,3,7,7,7,7,9,7,8,8,6,6,6,7,9,7,4,8,8,8,6,6,9,3,9,9,9,6,7,5,6,5,8~3,9,8,8,8,6,8,5,7,7,7,7,3,6,7,6,6,6,4,7,9,9,9,5,7,4,9,8&purInit=[{bet:500,type:\"default\"}]&total_bet_min=200.00";
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
            get { return 2; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        #endregion
    
        public FingerLicknFreeSpinsGameLogic()
        {
            _gameID     = GAMEID.FingerLicknFreeSpins;
            GameName    = "FingerLicknFreeSpins";
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

            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                FingerLicknFreeSpinsBetInfo betInfo = new FingerLicknFreeSpinsBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FingerLicknFreeSpinsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in FingerLicknFreeSpinsGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in FingerLicknFreeSpinsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            FingerLicknFreeSpinsBetInfo betInfo = new FingerLicknFreeSpinsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new FingerLicknFreeSpinsBetInfo();
        }
    }
}
