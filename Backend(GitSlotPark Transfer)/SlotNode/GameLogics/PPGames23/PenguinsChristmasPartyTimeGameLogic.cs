using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class PenguinsChristmasPartyTimeBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 5.0f;
            }
        }
    }
    class PenguinsChristmasPartyTimeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25xmasparty";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 25; }
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
                return "def_s=4,9,4,4,8,5,10,10,5,7,6,9,6,3,8&cfgs=1&ver=3&def_sb=3,9,10,4,8&reel_set_size=5&def_sa=4,10,6,5,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.52\",purchase:\"96.52\",regular:\"96.54\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"15619\",max_rnd_win:\"2000\",max_rnd_win_a:\"1000\",max_rnd_hr_a:\"6310\"}}&wl_i=tbm~2000;tbm_a~1000&sc=20.00,40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&purInit_e=1&wilds=2~5000,100,20,0,0~1,1,1,1,1&bonuses=0&bls=5,10&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,100,20,0,0;50,20,10,0,0;20,10,4,0,0;10,4,2,0,0;10,4,2,0,0;4,2,1,0,0;4,2,1,0,0;4,2,1,0,0&total_bet_max=10,000,000.00&reel_set0=10,4,1,7,8,8,8,3,8,5,7,1,9,9,9,6,9,6,10,5,10,10,10,4,7,6,8,10,9,9~5,3,9,9,10,7,8,8,8,10,9,4,9,2,10,9,9,9,6,5,7,8,8,7,9,10,10,10,6,6,4,6,10,7,5,10,8~6,10,5,4,8,8,8,9,10,2,7,9,9,8,9,9,9,5,8,1,10,6,10,7,10,10,10,6,3,9,5,7,8,4,1~7,5,6,9,7,8,8,8,4,8,6,10,4,5,9,9,9,8,9,2,6,10,10,10,3,5,10,8,10,7,8~10,5,9,10,4,8,8,8,9,8,5,4,6,9,9,9,1,8,9,1,10,8,10,10,10,3,7,6,9,10,6,7,7&reel_set2=9,8,4,9,8,4,6,8,8,8,9,8,6,7,10,5,1,9,9,9,6,10,7,5,7,1,6,7,10,10,10,8,10,10,3,10,1,8,5,9~4,10,9,7,9,8,8,8,7,6,6,5,10,8,9,9,9,2,3,8,6,6,7,10,10,10,9,7,5,9,4,8,10~8,6,10,1,10,4,9,8,8,8,1,4,6,3,6,2,9,9,9,10,5,6,9,9,7,10,1,10,10,10,7,7,10,8,8,9,8,7,5~8,5,9,10,4,8,8,8,9,10,10,8,7,6,9,9,9,5,2,7,4,3,10,10,10,8,7,6,10,8,6,5,9~10,4,6,6,8,8,8,5,1,9,10,8,8,9,9,9,1,10,6,5,7,10,10,10,7,4,9,7,8,3,9&reel_set1=4,6,5,9,8,8,8,1,9,6,1,9,9,9,7,5,9,3,10,8,10,10,10,6,10,7,10,8,7,4~10,9,4,7,7,6,8,8,8,5,7,2,10,9,6,8,9,9,9,7,8,9,5,10,6,8,10,10,10,8,4,10,5,10,6,3,9,8~5,1,5,8,10,8,8,8,10,8,1,9,10,9,9,9,6,6,4,7,9,10,10,10,7,10,2,3,8,6,7,8~10,10,7,4,8,8,8,10,7,10,4,6,5,6,9,9,9,5,8,7,9,8,10,10,10,6,8,10,7,9,2,9,6,3~8,9,5,6,10,8,8,8,5,6,1,10,8,7,6,8,9,9,9,3,4,7,9,1,10,6,10,10,10,7,8,4,9,7,10,10,9,5&reel_set4=4,10,6,9,8,8,8,7,7,6,8,6,7,9,9,9,10,10,8,4,3,9,10,10,10,5,9,8,10,5,9,8~3,8,10,8,10,8,8,8,7,10,5,6,7,5,9,9,9,10,4,9,5,9,6,10,10,10,8,7,9,9,6,8,4,2~8,5,7,6,8,5,8,8,8,2,10,5,9,10,4,7,9,9,9,8,4,7,10,6,8,10,10,10,3,9,10,9,7,6,10,9,6~7,5,9,8,8,8,7,6,5,8,10,6,9,9,9,8,7,10,10,6,9,10,10,10,9,2,4,8,9,4,3,10~8,5,6,10,6,8,8,8,3,8,4,5,10,5,9,9,9,4,9,7,7,8,8,10,10,10,9,6,7,9,10,9,10&purInit=[{bet:500,type:\"default\"}]&reel_set3=9,8,7,6,5,2,7,8,6,4,9,3,6,7,10,5,10~4,9,5,6,9,8,7,4,10,6,3,10,7,8,6,5,2,7~9,4,10,7,6,6,4,5,9,8,3,8,5,6,2,7,7,10~10,6,2,6,9,3,5,7,9,5,4,7,8,10,8,6,4,7~6,6,8,2,9,5,7,4,7,5,6,3,8,10,9,8,7,9,5,10&total_bet_min=100.00";
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
        
        public PenguinsChristmasPartyTimeGameLogic()
        {
            _gameID     = GAMEID.PenguinsChristmasPartyTime;
            GameName    = "PenguinsChristmasPartyTime";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if (!resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
                resultParams["g"] = spinParams["g"];

            return resultParams;
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                PenguinsChristmasPartyTimeBetInfo betInfo = new PenguinsChristmasPartyTimeBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in PenguinsChristmasPartyTimeGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in PenguinsChristmasPartyTimeGameLogic::readBetInfoFromMessage", strUserID);
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

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PenguinsChristmasPartyTimeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new PenguinsChristmasPartyTimeBetInfo();
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            PenguinsChristmasPartyTimeBetInfo betInfo = new PenguinsChristmasPartyTimeBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
