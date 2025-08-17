using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class HotToBurn7DeadlyBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 5.0f;
            }
        }
    }

    class HotToBurn7DeadlyGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10hottb7fs";
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
                return "def_s=3,11,7,4,9,5,8,10,5,10,6,11,9,3,11&cfgs=1&ver=3&def_sb=3,10,11,4,11&reel_set_size=9&def_sa=3,7,4,6,9&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.54\",purchase:\"96.54\",regular:\"96.54\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"464475\",max_rnd_win:\"4000\",max_rnd_sim_a:\"1\",max_rnd_win_a:\"2000\",max_rnd_hr_a:\"498774\"}}&wl_i=tbm~4000;tbm_a~2000&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&purInit_e=1&wilds=2~1000,50,10,0,0~1,1,1,1,1&bonuses=0&bls=5,10&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,50,10,0,0;50,20,8,0,0;50,20,8,0,0;40,10,4,0,0;40,10,4,0,0;40,10,4,0,0;15,5,2,0,0;15,5,2,0,0;15,5,2,0,0&total_bet_max=10,000,000.00&reel_set0=3,2,10,11,4,11,11,11,7,10,4,10,4,5,1,9,9,9,8,5,7,1,7,10,11,7,10,10,10,8,7,8,5,9,10,9,6,6~6,11,10,7,11,11,11,3,6,4,11,9,9,9,8,11,10,9,8,6,10,10,10,11,7,5,8,2,9,11~5,7,4,10,10,6,3,11,11,11,10,9,11,1,8,9,6,9,9,9,2,9,1,9,11,9,7,10,10,10,9,9,8,11,5,10,11,10~5,8,6,9,11,11,11,9,11,11,7,9,9,9,3,4,9,7,11,10,10,10,9,8,9,10,2~8,10,9,4,3,9,11,11,11,4,1,7,9,6,1,10,11,9,9,9,10,11,11,6,5,8,9,10,10,10,9,8,8,5,8,11,2,8,7&accInit=[{id:0,mask:\"cp;tp\"}]&reel_set2=4,8,2,8,9,8,6,2,2,2,7,9,2,7,1,5,9,7,9,9,9,4,9,4,5,7,5,7,6,3,1~6,6,2,2,2,4,8,9,7,6,7,8,8,8,4,9,9,8,3,9,9,9,6,2,8,8,6,8,5~6,7,4,5,2,2,2,3,8,2,7,2,8,8,8,5,9,6,9,1,9,9,9,1,9,9,8,8,9~9,3,9,9,8,9,2,2,2,8,2,9,5,7,8,2,9,9,9,7,6,6,9,7,9,2,4~2,2,9,5,6,1,2,2,2,8,9,9,7,4,6,9,9,9,8,7,8,8,9,1,5,8,3&reel_set1=9,6,10,8,5,9,8,4,5,9,9,9,2,5,7,2,6,7,3,10,7,10,10,10,4,7,7,1,9,10,8,4,9,7,1,10~5,6,4,8,6,2,9,9,9,7,8,10,2,10,9,9,10,10,10,6,8,9,4,8,6,10,3,7~2,9,10,6,9,6,9,3,9,9,9,1,10,7,5,2,8,10,9,10,8,10,10,10,9,9,1,5,8,10,9,10,7,4,10~8,6,8,9,5,2,7,9,9,9,7,9,10,9,2,10,4,9,9,10,10,10,3,7,10,10,8,6,9,9,10,9~4,8,6,9,8,9,5,9,9,9,8,4,5,7,1,10,1,10,10,10,2,6,8,9,8,7,10,9,2,3&reel_set4=5,3,2,2,2,4,6,3,4,4,4,1,5,2,5,5,5,1,4,4,6,6,6,7,5,2,7,7,7,5,6,7,7~3,7,2,2,2,5,4,4,6,4,4,4,3,2,6,5,5,5,6,7,2,6,6,6,5,6,4,7,7,7,5,2,6,4,7~2,7,2,2,2,4,7,5,4,4,4,6,4,5,4,5,5,5,1,2,4,6,6,6,5,3,7,7,7,2,6,7,3,1~2,2,2,3,6,5,4,4,4,7,3,7,6,5,5,5,2,4,6,6,6,4,5,5,7,7,7,6,7,6,7,4~3,5,3,2,2,2,7,6,1,4,4,4,1,6,2,6,5,5,5,2,5,7,7,6,6,6,5,4,5,4,7,7,7,6,4,5,7,2&purInit=[{bet:500,type:\"fs\"}]&reel_set3=5,7,3,2,2,2,7,1,7,7,6,6,6,5,4,7,2,7,7,7,1,5,7,4,6,8,8,8,2,8,6,8,8,4~8,4,2,2,2,3,4,5,6,6,6,7,8,8,6,7,7,7,5,7,8,8,8,6,2,6,6,2~6,8,1,2,2,2,7,7,2,4,6,6,6,2,5,6,5,8,8,8,4,8,3,1,8~8,7,8,2,2,2,8,5,8,7,6,6,6,2,2,8,6,7,7,7,2,7,6,7,8,8,8,4,5,4,3,6~8,1,4,6,2,2,2,4,6,8,7,2,6,6,6,5,8,1,8,3,8,8,8,5,7,2,8,4,8,5&reel_set6=4,4,5,2,2,2,5,1,2,2,3,3,3,4,3,4,4,4,3,5,2,3,5,5,5,1,5,4,5~2,4,3,2,2,2,5,5,3,2,3,3,3,5,4,4,5,4,4,4,5,3,4,2,5,5,5,4,3,4,5,4,4~5,4,2,2,2,3,5,4,3,3,3,2,5,3,4,4,4,2,4,1,5,5,5,4,3,1,5~4,4,2,2,2,3,5,4,5,3,3,3,2,5,4,2,4,4,4,5,3,3,4,5,5,5,3,5,4,5,2~4,4,5,2,2,2,4,1,3,3,3,5,4,4,2,4,4,4,3,4,5,1,5,5,5,3,2,5,5,2,5&reel_set5=4,3,2,2,2,6,6,1,6,3,3,3,5,5,2,1,4,4,4,5,4,4,2,5,5,5,4,5,4,6,6,6,5,2,3,3,6~2,2,2,3,6,3,3,3,5,2,4,4,4,6,5,5,5,6,5,6,6,6,4,4,6,3~3,6,2,2,2,6,5,4,4,3,3,3,5,1,3,4,4,4,1,3,4,5,5,5,6,6,5,6,6,6,4,2,2,4,2~4,2,2,2,6,4,3,3,3,5,2,4,4,4,3,5,5,5,3,5,6,6,6,2~2,3,2,2,2,6,4,5,3,3,3,6,5,5,2,4,4,4,6,6,5,5,5,1,4,1,6,6,6,2,4,5,3,3&reel_set8=7,11,10,10,4,9,1,9,11,11,11,2,11,7,11,9,5,7,7,3,9,9,9,10,9,1,8,4,10,6,8,8,10,10,10,5,6,7,4,7,8,10,1,5,10~11,2,7,8,11,11,11,4,6,9,6,7,9,9,9,11,11,4,10,5,10,10,10,9,8,6,10,8,11,3~11,8,9,4,6,9,11,11,11,10,3,5,11,10,1,9,9,9,1,9,9,8,10,10,10,5,10,6,2,11,7,7,10~7,3,9,9,4,9,11,11,11,6,7,8,10,9,11,9,6,9,9,9,8,11,4,11,10,9,5,10,10,10,2,10,8,9,11,7,11,11~7,11,9,6,9,8,1,4,11,11,11,8,9,11,1,5,8,9,11,8,9,9,9,1,3,10,10,8,11,4,10,8,10,10,10,8,2,7,6,9,5,9,10,11,1&reel_set7=4,2,5,4,2,2,2,5,4,4,5,5,3,3,3,4,3,4,4,4,5,3,1,4,5,5,5,4,5,5,2~2,5,4,4,5,5,2,2,2,4,5,4,5,4,4,5,3,3,3,5,2,5,3,5,4,5,4,4,4,3,5,5,4,5,4,5,5,5,4,4,3,2,4,3,5,5~4,1,3,5,4,2,2,2,4,5,5,4,4,5,5,3,3,3,4,5,5,2,2,3,3,4,4,4,5,4,4,2,5,4,5,5,5,4,5,4,3,4,4,1~2,4,4,5,5,2,2,2,4,2,5,5,3,4,3,3,3,5,3,4,5,5,4,4,4,5,5,2,4,3,4,5,5,5,5,5,5,4,5,3,5,5,4~5,2,2,2,4,4,5,4,3,3,3,5,4,5,5,4,4,4,3,4,4,1,5,5,5,2,5,5,3,4&total_bet_min=40.00";
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
        public HotToBurn7DeadlyGameLogic()
        {
            _gameID = GAMEID.HotToBurn7Deadly;
            GameName = "HotToBurn7Deadly";
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

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                HotToBurn7DeadlyBetInfo betInfo = new HotToBurn7DeadlyBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in HotToBurn7DeadlyGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in HotToBurn7DeadlyGameLogic::readBetInfoFromMessage", strGlobalUserID);
                    return;
                }
		
                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
                    return;
                }
                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
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
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HotToBurn7DeadlyGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
