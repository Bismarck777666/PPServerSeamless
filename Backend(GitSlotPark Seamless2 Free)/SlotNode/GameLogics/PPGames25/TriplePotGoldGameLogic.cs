using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TriplePotGoldGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysasiatrzn";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
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
                return "def_s=6,12,7,8,12,9,13,13,9,15,6,11,11,5,13&cfgs=1&ver=3&mo_s=14&mo_v=2,3,4,5,6,8,10,12,15,20,25,30,40,50,60,80,100,200,500&def_sb=7,13,7,8,11&reel_set_size=10&def_sa=6,13,14,5,13&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;3~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;4~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.01\",purchase:\"96.02\",regular:\"96.02\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"3649635\",max_rnd_win:\"5000\",max_rnd_win_a:\"4000\",max_rnd_hr_a:\"2418380\"}}&wl_i=tbm~5000;tbm_a~4000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=20,25&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;300,100,50,0,0;200,60,30,0,0;150,50,25,0,0;100,40,20,0,0;75,30,15,0,0;50,20,10,0,0;50,20,10,0,0;20,10,5,0,0;20,10,5,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=8,000,000.00&reel_set0=13,12,13,10,6,4,10,11,13,9,12,9,8,11,3,13,14,6,7,11,9,13,11,5,7,7,1,11,13,11,13,12,13,9,9,12,7,7,11,8,7,11,13,11,7,6,8,11~10,11,6,3,6,12,6,8,5,8,10,13,7,8,12,7,12,11,10,11,12,10,9,6,12,1,10,8,5,13,10,6,2,11,4,8,13,10,12,13,14,8,12,8,11,6,9~14,10,7,6,5,13,11,13,12,10,11,8,8,12,7,11,6,2,12,9,5,13,11,8,13,5,6,11,9,5,8,1,11,13,11,8,12,3,4,9,13,12,6,6~13,3,7,6,11,13,12,8,13,14,7,9,13,10,8,9,9,10,11,7,5,12,10,11,12,7,12,5,10,7,4,11,12,6,2,8,9,8,11,10,6,12,1,9,8~10,5,3,12,13,8,8,5,5,13,10,13,12,15,12,13,6,8,13,8,7,12,7,10,7,7,12,5,7,9,11,10,12,8,9,11,10,9,6,15,12,10,15,4,13,11,5,10,11,1,8,10,15,7,13,11,13&reel_set2=11,11,13,6,7,11,6,14,13,6,13,13,5,7,8,9,4,14,11,14,14,14,13,14,7,13,9,11,12,11,11,8,11,11,10,12,9,8,13,3,13,5,10,13~8,3,14,10,12,10,6,8,5,11,6,10,12,7,10,13,6,10,5,12,7,4,10,10,6,8,14,14,14,7,13,12,2,5,10,6,11,14,8,12,7,9,12,10,14,10,8,10,10,12,13,12,12,11,14,10,12~11,6,12,13,13,12,13,5,12,14,13,14,9,6,12,6,9,8,11,13,12,8,14,14,14,6,9,11,13,13,7,2,13,5,10,4,12,13,7,10,11,13,12,11,14,7,8,3,12,10~7,11,4,9,10,10,13,2,3,7,9,13,12,10,8,12,9,14,14,14,10,12,10,5,13,11,6,12,14,11,8,14,12,10,10,14,7,8,6~8,5,13,8,10,5,6,11,13,6,10,8,11,9,12,7,10,14,3,8,10,11,13,6,13,14,5,12,5,13,14,14,14,10,10,13,5,12,7,10,10,12,9,7,4,12,10,13,12,14,6,9,11,14,8,7,12,10,13,11,13,11,13,10&reel_set1=12,7,11,8,12,5,7,11,13,9,7,8,6,13,10,11,10,12,13,9,11,13,8,13,14,11,7,11,7,11,7,13,12,9,13,6,9,6,5,13,7,9,11,13~7,8,11,14,5,8,6,10,12,6,13,6,6,12,8,8,12,10,5,10,11,12,13,12,6,13,6,10,11,8,12,2,10,7,9,10,12,11,13,8,9,11~7,14,8,12,2,12,8,13,6,13,8,5,7,9,11,13,10,11,12,11,12,10,12,7,6,9,5,12,13,11,5,11,6,13~2,8,10,12,8,13,9,9,7,7,10,8,12,9,12,8,6,7,10,8,7,9,13,10,5,11,9,13,14,12,11,5,11,10,11,13,9,10,11,6,9,6,10,8,10,6,12,9,13,12,7,12,7,10,11~5,5,8,8,7,13,10,13,9,12,10,13,5,15,10,5,6,12,10,8,8,12,11,7,15,13,7,12,9,6,7,10,12,11,8,13,11&reel_set4=5,11,14,9,5,7,11,13,10,11,13,11,11,12,13,1,10,11,6,11,11,13,11,3,6,14,14,14,7,11,12,14,9,8,13,8,13,7,8,11,13,13,12,6,11,13,13,9,13,14,8,7,13,9~7,13,10,11,10,12,11,10,6,8,1,7,13,8,14,10,5,10,12,10,7,14,14,14,10,12,2,12,5,12,6,11,7,12,10,12,10,14,10,13,3,8,14,5,9,8,12,6~7,13,5,8,10,12,13,10,3,11,13,2,6,14,12,14,9,11,14,14,14,5,13,12,6,13,12,7,13,1,6,11,7,11,8,9,12,6,12,13,14~14,12,8,9,7,1,11,6,12,13,10,10,12,10,11,7,14,6,7,12,13,12,10,11,6,11,14,14,14,9,14,10,5,10,5,9,12,11,8,10,7,12,3,10,10,8,10,13,8,12,14,10,6,13,2,9,6,10~5,6,10,12,13,7,8,12,10,9,6,8,7,11,3,11,10,13,12,5,8,13,14,14,14,10,14,6,12,5,13,14,13,11,13,9,8,14,10,12,13,5,10,1,10,10,7,13,11,12&purInit=[{bet:1600,type:\"default\"}]&reel_set3=11,14,13,10,13,13,7,9,4,10,13,8,11,5,12,11,11,8,11,14,14,14,5,6,11,14,12,13,11,13,9,13,1,8,6,11,13,7,11,9,7,13,14~5,12,14,10,10,12,7,4,6,7,11,13,6,5,10,10,6,11,12,14,14,14,12,8,6,12,14,11,9,10,12,1,8,13,10,8,10,12,8,7,14,2,10,10~12,13,13,7,14,6,13,13,14,13,11,12,13,14,11,2,12,8,9,8,10,8,12,6,8,14,14,14,12,1,4,13,6,7,11,13,10,13,12,11,10,5,11,6,11,13,7,6,5,9,6,12,13,12,9~11,5,14,13,9,12,11,6,10,8,10,6,1,12,14,4,7,8,10,14,14,14,5,10,12,7,14,11,9,12,10,8,10,11,7,6,10,12,13,6,10,2,10,6,13~8,13,7,13,12,7,11,10,4,12,10,5,14,14,12,14,13,10,10,11,9,8,6,11,10,5,14,14,14,13,6,10,9,8,5,12,6,13,12,5,1,11,6,13,10,9,5,11,10,13,10,10,11,14,12&reel_set6=13,10,12,13,6,5,11,9,11,13,10,14,7,11,13,6,11,14,14,14,9,3,11,11,13,7,14,13,8,13,9,12,13,5,8,6,11,8,11~12,12,5,10,6,9,10,10,12,11,10,12,10,10,14,5,3,8,10,8,6,13,6,7,12,14,14,14,7,14,6,10,13,8,6,12,7,2,13,10,14,12,11,10,10,14,8,11,12,11,12,5,8,7~13,12,13,11,12,13,5,2,12,9,14,14,7,8,12,11,12,13,7,14,14,14,13,6,13,7,13,6,9,12,11,8,3,12,10,13,5,10,14,11,8,6~6,7,9,10,12,8,5,12,10,8,10,12,13,6,9,12,3,8,6,14,14,14,11,12,10,9,10,7,14,14,13,10,14,2,11,10,6,10,5,12,13,7~14,8,7,11,10,6,9,5,13,10,8,13,11,13,3,12,14,14,14,10,9,8,11,7,10,6,5,12,5,14,12,13,12,7,10,10,13&reel_set5=11,10,6,13,13,14,14,7,13,12,8,13,11,9,11,9,14,14,14,13,7,11,11,8,5,9,8,11,11,14,7,13,11,13,12,6,13,6,4~11,8,11,10,12,5,6,12,10,5,6,14,13,12,14,5,10,8,10,6,12,12,10,12,6,14,14,14,8,7,12,10,10,6,14,11,9,12,10,14,4,13,7,10,13,8,10,11,2,12,10,10,12,7,6~9,11,13,13,7,8,10,9,8,12,5,6,9,13,7,14,13,12,13,12,11,14,14,14,6,14,13,13,12,11,7,12,10,12,13,10,12,2,11,6,7,6,13,11,12,6,4,13,14,5~5,10,12,6,10,7,10,6,8,7,6,12,8,14,11,9,12,6,7,5,10,14,14,14,11,14,13,10,13,2,10,11,10,9,11,10,12,6,12,4,9,7,12,13,10,14,8~13,12,14,10,6,12,5,9,11,14,6,11,6,13,7,14,14,14,10,8,7,8,13,10,5,11,12,13,10,8,13,12,4,12,10,7,14,10,10&reel_set8=11,13,13,8,12,8,9,13,7,14,8,9,12,13,12,6,7,6,11,7,13,14,11,5,14,11,14,14,14,9,11,13,14,11,13,13,11,7,6,8,11,11,13,10,13,5,11,7,9,13,11,7,10,11,6,13,11,13~10,14,7,10,10,6,8,14,12,5,14,6,5,10,12,14,14,14,12,13,11,13,11,10,9,12,10,10,7,8,10,7,8,12,2,11,12~12,6,12,11,7,8,9,12,6,13,6,14,14,7,13,14,14,14,13,5,8,10,13,11,12,6,10,13,12,11,13,2,9,7~7,10,8,10,6,10,8,14,10,11,6,14,12,9,12,13,12,11,14,6,12,7,10,6,10,11,14,14,14,10,7,8,12,2,12,10,7,10,12,13,8,10,9,13,6,9,5,13,11,6,11,6,10,14,5,12,10~12,8,7,11,5,11,6,10,5,9,13,10,12,9,8,5,10,13,7,6,10,14,14,14,13,14,7,12,11,10,14,13,8,5,13,12,13,11,8,7,13,10,6,10,14,6,12&reel_set7=7,6,13,6,14,13,8,11,13,1,13,9,8,11,11,7,11,6,13,13,8,11,7,14,14,14,6,7,11,9,14,13,11,12,9,12,13,5,11,9,5,10,14,11,11,13,12,13,8,14,10,13,11~13,10,10,14,10,10,12,13,8,12,7,14,5,14,14,14,10,7,12,10,10,8,11,5,11,6,12,6,9,1,6,12,2,12~5,12,5,12,6,14,7,13,8,14,8,13,6,12,13,14,7,9,14,14,14,13,12,10,2,12,11,6,12,6,13,6,7,1,11,13,9,11,8,11,13,13~13,9,14,12,10,8,12,10,7,10,13,10,14,11,5,10,10,11,14,14,14,8,12,6,2,12,7,6,9,8,6,9,1,10,7,14,11,10,6,12,6,13,5~14,6,8,11,13,10,10,6,12,11,5,8,10,10,12,11,10,7,10,13,14,14,14,6,13,5,6,10,13,5,13,14,12,1,9,7,12,8,14,5,11,12,8,9&reel_set9=9,13,7,7,11,13,14,11,9,11,10,7,12,6,11,8,13,12,11,13,5,9,13,7,12,9,13,6,11,7,8,13~6,10,11,13,5,11,12,7,10,7,11,10,12,6,8,7,10,11,8,12,10,12,6,13,13,5,9,10,14,10,5,8,8,6,12,6,8,9,6,13,10,11,6,9,12,10,13,6,8,2,8,7,8~6,7,8,6,8,13,11,9,10,13,12,11,12,13,6,11,7,8,9,13,12,2,12,5,12,5,12,10,13,8,14,9,13,11,6,5,11~13,8,10,13,12,10,9,12,8,12,6,7,10,12,7,12,11,7,6,11,14,6,11,8,13,11,10,12,5,7,10,9,7,9,5,6,12,8,9,8,9,5,13,10,7,2,11,10,10~9,15,9,10,13,12,13,11,8,10,7,15,11,13,12,7,7,8,13,6,7,12,5,8,8,12,10,12,6,11,10,5,13,5&total_bet_min=200.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 80; }
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
            get { return 1.25; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        #endregion
        
        public TriplePotGoldGameLogic()
        {
            _gameID = GAMEID.TriplePotGold;
            GameName = "TriplePotGold";
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
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

            if (dicParams.ContainsKey("wlc_v"))
            {
                string strWlcv = dicParams["wlc_v"];
                string[] strParts = strWlcv.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strSubParts.Length > 1)
                        strSubParts[1] = convertWinByBet(strSubParts[1], currentBet);

                    strParts[i] = string.Join("~", strSubParts);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in vswaysasiatrznGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in vswaysasiatrznGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                _logger.Error("Exception has been occurred in vswaysasiatrznGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
