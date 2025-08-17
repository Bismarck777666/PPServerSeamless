using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BookOfTutMegawaysGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaystut";
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
                return 7;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=7,6,7,7,9,7,10,6,10,10,9,3,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12&cfgs=8497&ver=3&def_sb=11,7,11,11,10,4&reel_set_size=21&def_sa=10,10,11,11,9,4&scatters=1~500,100,15,3,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2691790\",max_rnd_win:\"10000\",max_rnd_win_a:\"8000\",max_rnd_hr_a:\"2621805\"}}&wl_i=tbm~10000;tbm_a~8000&reel_set10=4,7,6,9,5,6,6,9,11,11,11,11,4,7,11,9,8,3,8,5,5,6,3,3,3,3,6,8,3,10,11,6,4,7,8,4,4,4,4,9,7,10,11,3,11,3,5,9,9,5,5,5,5,4,8,8,1,11,4,6,3,9,11,6,6,6,3,6,8,4,10,9,10,9,11,7,7,7,5,10,11,9,3,5,11,6,9,7,8,8,8,8,5,10,3,5,7,5,10,3,8,9,9,9,9,7,10,11,11,7,7,4,6,4,5,10,10,10,10,3,10,7,7,5,4,10,8,10,8,9~8,4,5,10,11,11,11,11,7,6,7,4,8,4,4,4,5,11,11,8,2,5,5,5,1,2,5,9,9,6,6,6,8,6,11,6,11,7,7,7,7,3,7,11,5,9,8,8,8,10,4,10,9,3,9,9,9,11,9,7,8,10,10,10,3,4,6,7~9,8,4,4,10,11,11,11,1,8,11,6,8,6,4,4,4,7,11,5,2,5,3,5,5,5,9,2,5,7,10,9,6,6,6,6,7,9,8,3,11,10,10,7,7,7,7,2,4,5,11,6,10,8,8,8,8,4,6,2,8,6,10,9,9,9,9,10,8,7,9,1,7,10,10,10,8,6,5,9,3,4,11,11~7,6,9,6,10,11,5,8,9,9,7,4,7,2,7,6,10,11,11,11,11,3,9,9,4,7,7,8,10,7,10,7,11,6,6,4,9,9,10,4,4,4,11,7,7,5,11,8,9,10,4,1,7,8,10,8,6,3,5,8,5,5,5,3,8,5,3,3,8,1,4,7,11,5,11,11,2,10,8,9,1,6,6,6,6,5,11,11,7,3,6,11,7,7,9,11,8,10,8,3,8,9,8,6,7,7,7,6,5,11,10,11,4,1,10,8,6,8,11,5,10,10,4,9,2,8,8,8,8,4,10,6,9,10,11,8,3,5,8,10,10,5,9,5,6,4,7,9,9,9,9,8,8,10,4,5,9,6,4,7,9,3,11,4,10,9,2,11,11,10,10,10,10,9,10,8,9,11,7,6,2,9,4,5,10,9,9,10,6,3,11,9,10~8,7,3,5,4,5,5,7,4,6,2,6,3,9,10,3,6,11,11,11,5,11,11,7,7,8,3,6,9,8,8,10,7,6,4,10,10,9,4,3,3,3,10,7,9,10,5,11,10,2,11,7,5,9,4,9,4,10,6,1,4,4,4,8,5,1,5,3,10,3,3,5,9,8,2,4,8,6,10,9,9,7,5,5,5,2,6,4,11,10,9,7,10,5,10,10,9,6,9,9,10,6,6,9,6,6,6,6,10,7,8,3,8,9,3,2,4,6,4,10,8,10,11,2,8,9,7,7,7,4,10,9,9,3,10,7,3,6,1,10,3,9,9,11,7,9,10,5,8,8,8,9,3,11,11,6,4,5,3,6,2,6,9,8,7,5,5,8,10,9,9,9,9,5,5,9,6,8,10,9,11,10,10,6,8,9,3,3,9,1,4,7,10,10,10,10,7,5,8,4,7,9,10,10,7,8,3,7,10,9,8,2,3,5,4,4~4,10,7,7,9,9,6,6,8,3,8,3,5,10,11,4,11,8,8,6,6,6,9,10,5,3,4,7,10,6,7,3,7,8,8,9,11,9,6,9,1,8,7,7,7,7,11,8,4,4,9,8,9,6,11,7,7,6,6,1,10,6,8,11,4,8,8,8,8,7,11,3,4,5,4,4,9,4,5,9,7,3,4,10,3,8,5,4,9,9,9,9,11,6,7,5,10,7,9,11,9,5,10,8,7,9,4,5,10,11,3,10,10,10,5,10,7,7,3,11,10,11,3,7,8,8,9,5,3,9,5,4,10,4,5,10&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=6,5,11,11,5,10,11,11,11,11,8,1,4,5,8,6,4,7,7,4,3,3,3,3,10,5,3,9,10,8,11,4,4,4,4,8,4,9,5,7,3,6,8,5,5,5,5,11,5,7,8,10,11,9,6,6,6,9,5,9,7,9,11,10,10,11,7,7,7,8,3,3,7,3,9,10,6,8,8,8,8,6,10,7,4,5,8,4,9,9,9,9,6,4,6,7,11,7,9,6,3,10,10,10,10,8,6,9,3,8,3,7,11,10,10~2,11,8,7,6,11,9,11,11,11,11,7,1,8,9,9,6,7,6,5,9,6,4,4,4,2,9,9,4,7,5,5,11,10,10,5,5,5,4,7,2,9,10,11,10,3,9,6,6,6,11,4,9,10,10,6,9,3,11,5,7,7,7,7,8,4,7,3,4,4,11,6,7,11,8,8,8,11,6,8,7,10,5,8,4,5,9,9,9,5,10,10,8,5,8,7,9,3,8,10,10,10,3,10,8,11,10,3,2,11,7,8,6~6,2,3,6,4,10,7,2,11,11,11,10,1,9,9,10,9,7,11,7,11,4,4,4,5,4,6,2,10,8,7,8,4,5,5,5,2,10,11,7,5,8,10,8,9,6,6,6,6,11,7,6,4,8,8,11,6,10,7,7,7,7,3,4,7,10,6,8,4,3,11,8,8,8,8,9,6,5,3,4,7,8,9,9,9,9,8,10,8,6,5,11,11,9,4,10,10,10,3,5,5,10,10,11,2,5,9,9~8,7,10,10,8,10,11,11,11,11,8,1,9,3,6,9,6,4,4,4,5,2,9,10,4,7,4,11,5,5,5,8,9,11,8,2,11,10,6,6,6,6,4,6,5,4,11,3,11,7,7,7,9,6,9,8,5,10,6,8,8,8,8,8,7,7,3,7,2,8,11,9,9,9,9,3,5,4,6,10,9,7,10,10,10,10,9,8,11,2,5,10,10,5,10~8,7,4,3,7,8,10,11,11,11,5,8,5,1,8,7,11,9,4,5,3,3,3,7,4,5,3,8,10,5,9,4,4,4,9,6,2,11,10,3,4,6,6,5,5,5,10,9,3,10,6,10,10,6,11,6,6,6,6,10,9,11,2,6,10,9,8,7,7,7,2,10,7,9,3,9,9,5,11,8,8,8,9,10,7,7,5,10,3,6,9,9,9,9,4,2,3,8,9,8,7,5,8,10,10,10,10,6,11,6,5,9,8,8,7,3,4~9,9,10,7,5,7,7,3,9,3,4,6,4,6,6,6,1,7,9,9,7,3,11,9,6,10,5,11,3,5,6,7,7,7,7,5,11,4,8,10,4,10,8,10,6,5,11,8,5,8,8,8,8,7,8,3,9,4,4,11,7,5,8,9,7,6,7,9,9,9,9,7,4,6,4,9,11,7,10,10,8,3,7,3,6,10,10,10,10,8,8,9,10,3,8,9,8,10,11,5,10,4,4,10,8&reel_set12=10,8,5,6,8,7,10,3,11,10,1,11,7,4,10,7,10,8,7,8,9,9,11,8,7,11,7,5,6,11,4,9,11,4,6,9,9,11,10,9,10,8~10,4,7,11,5,10,3,7,11,9,1,8,10,11,5,4,6,7,9,9,6,10,8,10,9,5,7,11,6,9,9,11,10,6,7,11,11,11,10,7,4,9,7,8,11,5,11,9,11,11,8,6,5,11,6,11,8,9,5,6,9,11,8,11,10,8,4,11,10,8,10,4,10,11,11~9,4,7,10,9,11,8,3,10,4,7,1,10,8,11,10,4,11,8,6,7,11,11,10,8,7,6,11,7,9,10,11,4,11,7,11,8,6,10,10,10,9,10,7,10,9,10,7,10,8,4,9,4,7,10,5,11,11,9,11,8,6,9,8,7,8,6,9,5,8,5,10,10,9,11,11,7,11,11~9,7,5,4,10,7,9,8,3,9,4,9,1,10,9,7,10,7,11,11,8,10,8,11,11,8,11,10,10,10,11,5,7,8,6,11,11,8,5,8,11,6,9,7,11,9,4,10,11,7,10,10,7,7,6,10,11~9,8,6,7,8,11,8,6,11,7,4,7,1,10,9,11,3,10,9,10,11,8,10,9,4,11,7,10,8,9,9,11,4,7,10,6,7,6,11,8,11,9,7,6,7,11,5,6~4,8,7,11,7,11,8,9,4,11,8,7,1,8,10,7,7,9,11,9,10,3,6,11,8,11,10,4,7,11,6,10,7,11,8,10,6,9,11,8,5,6&purInit_e=1&reel_set13=reel_set12&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&bls=20,25&reel_set18=reel_set12&reel_set19=reel_set12&reel_set14=reel_set12&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;40,20,15,10,0,0;20,15,12,8,0,0;15,12,10,6,0,0;12,10,8,5,0,0;10,8,6,4,0,0;10,8,6,4,0,0;8,7,5,2,0,0;8,7,5,2,0,0;8,7,5,2,0,0;0,0,0,0,0,0&reel_set15=reel_set12&reel_set16=reel_set12&reel_set17=reel_set12&total_bet_max=10,000,000.00&reel_set0=5,3,8,8,3,7,4,3,11,11,11,11,10,7,6,8,9,10,7,11,5,11,3,3,3,3,11,10,9,8,9,10,7,9,7,4,4,4,4,10,10,7,3,6,6,10,6,4,4,5,5,5,5,6,8,9,7,9,5,4,11,6,9,6,6,6,5,9,1,5,3,4,7,9,8,7,7,7,6,3,6,9,7,5,11,11,6,7,8,8,8,8,10,9,8,3,6,11,8,3,3,9,9,9,9,10,10,5,3,5,11,9,5,4,10,10,10,10,4,4,5,11,11,4,8,8,5,7,11~7,10,8,11,10,11,5,11,11,11,11,8,8,10,3,9,10,3,4,4,4,1,7,5,10,11,5,9,2,5,5,5,6,8,10,10,5,4,11,5,6,6,6,11,9,8,9,8,4,5,11,7,7,7,7,6,7,3,9,11,8,10,2,8,8,8,10,8,11,6,4,3,9,7,9,9,9,7,4,9,2,7,7,11,6,10,10,10,6,9,6,4,6,5,9,3,11,7~6,4,7,9,8,4,6,11,9,11,11,11,1,5,11,4,8,11,7,8,11,6,4,4,4,9,3,5,10,8,6,2,4,2,4,5,5,5,2,7,11,10,8,6,8,4,11,6,6,6,6,10,5,11,6,10,10,6,5,9,10,11,7,7,7,7,6,8,11,8,10,3,10,10,3,3,8,8,8,8,11,8,8,4,10,6,2,9,4,7,9,9,9,9,10,7,7,9,7,9,5,4,10,10,10,9,7,5,2,8,9,8,2,7,3,9,5~6,6,2,6,5,11,11,11,11,6,2,5,3,9,5,7,4,4,4,8,9,7,9,10,10,6,5,5,5,3,10,9,10,11,10,6,6,6,6,11,7,8,8,9,11,11,7,7,7,8,7,7,5,11,10,4,8,8,8,8,10,4,10,8,3,11,9,9,9,9,8,1,10,7,5,4,10,10,10,10,2,7,1,4,11,9,6,4~6,9,8,10,5,2,10,7,2,11,11,11,8,10,9,6,3,3,10,9,10,6,4,3,3,3,8,9,6,5,8,10,9,8,9,4,4,4,6,4,5,7,5,7,4,4,3,3,10,5,5,5,8,4,11,1,3,3,11,5,8,1,7,6,6,6,6,4,3,11,11,4,5,7,6,5,10,7,7,7,10,8,9,11,7,3,7,8,11,4,11,8,8,8,5,9,2,6,9,8,10,10,5,6,9,9,9,9,7,7,9,7,10,10,2,6,7,6,10,10,10,10,3,10,9,9,10,9,5,6,9,5,8,9~8,9,11,3,10,9,3,8,7,4,4,10,6,6,6,10,9,1,9,8,5,10,7,6,5,11,10,7,7,7,7,11,3,9,7,3,7,8,5,5,11,11,5,11,9,8,8,8,8,6,6,4,4,10,5,7,5,3,4,9,10,8,9,9,9,9,10,9,7,8,11,4,9,3,6,8,7,10,4,10,10,10,10,3,6,7,4,8,6,7,8,7,6,5,4,3,9,10&reel_set2=reel_set1&reel_set1=5,7,11,7,11,3,10,9,7,10,7,9,10,7,7,4,9,10,6,10,6,8,7,9,8,10,8,10,7,9,8,11,9,11,9,11,5,9,1,8,8,4,11,5,8,4,10,4,11,8,10,11,11,10,5,11,11,8,7,9,10,7,7,11,6,10,9,6,8~10,9,4,5,10,3,11,11,7,4,6,7,11,6,4,9,10,7,10,11,7,9,11,7,11,9,11,10,4,6,11,10,5,4,8,10,11,11,11,7,10,9,6,8,11,9,10,11,11,8,11,6,5,5,8,10,11,8,11,10,5,9,6,8,9,6,11,5,8,11,6,9,9,10,7,1~7,9,7,11,7,7,3,8,7,11,7,11,4,5,9,6,9,8,9,10,5,8,11,9,7,7,10,11,8,10,10,10,11,11,10,10,9,10,6,11,9,8,10,11,4,8,6,7,9,10,4,8,1,5,11,10,6,11,10,11,8~1,5,4,7,5,10,3,9,8,9,8,9,8,4,10,11,7,8,7,11,11,9,8,11,10,10,7,9,8,11,10,10,10,6,5,7,6,11,8,10,11,7,11,7,10,6,9,11,7,8,11,6,11,4,6,10,11,9,10,10,11,6,7,9~11,4,11,9,6,10,3,9,9,6,10,5,11,6,7,8,11,11,7,11,7,6,8,7,11,9,9,6,8,4,8,7,6,4,7,7,1,7,11,11,8,9,9,10,8,10,8,10,9,6,7,7,10,9,8,10,1,8,11,11,8~8,7,11,6,10,8,4,6,9,7,11,11,10,5,3,10,7,11,7,1,8,7,10,8,11,9,5,10,11,9,8,11,7,7,8,11,11,7,9,7,6,11,9,8,6,1,6,11,8,11,7,4,11,6,10,7,11,8,6,4,6,7,11&reel_set4=reel_set1&purInit=[{bet:2000,type:\"fs\"}]&reel_set3=reel_set1&reel_set20=reel_set12&reel_set6=reel_set1&reel_set5=reel_set1&reel_set8=reel_set1&reel_set7=reel_set1&reel_set9=reel_set1&total_bet_min=10.00";
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
            get { return 1.25; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
	
        #endregion
        public BookOfTutMegawaysGameLogic()
        {
            _gameID = GAMEID.BookOfTutMegaways;
            GameName = "BookOfTutMegaways";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "6";
	        dicParams["bl"]         = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("wlc_v"))
            {
                string[] strParts = dicParams["wlc_v"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strValues = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    strValues[1] = convertWinByBet(strValues[1], currentBet);
                    strParts[i] = string.Join("~", strValues);
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


                BasePPSlotBetInfo betInfo   = new BasePPSlotBetInfo();
                betInfo.BetPerLine          = (float)message.Pop();
                betInfo.LineCount           = (int)message.Pop();
		
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BookOfTutMegawaysGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in BookOfTutMegawaysGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                _logger.Error("Exception has been occurred in BookOfTutMegawaysGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
