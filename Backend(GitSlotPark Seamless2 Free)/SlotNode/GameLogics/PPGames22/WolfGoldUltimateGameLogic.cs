using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WolfGoldUltimateGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25ultwolgol";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 25; }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
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
                return "def_s=6,10,10,3,8,5,7,9,5,3,4,10,6,4,10&cfgs=1&ver=3&mo_s=11;13;14;15&mo_v=10,25,50,75,125,200,250,375,500,1250,2500,25000;0;75,125,200,250,375;2,3,4,5&def_sb=3,10,6,3,9&reel_set_size=3&def_sa=4,10,9,6,7&mo_jp=500;1250;2500;25000&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.54\",purchase:\"96.56\",regular:\"96.57\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"476190476\",max_rnd_win:\"5000\",max_rnd_win_a:\"3125\",max_rnd_hr_a:\"156250000\"}}&wl_i=tbm~5000;tbm_a~3125&mo_jp_mask=jp1;jp2;jp3;jp4&sc=4.00,8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&purInit_e=1&wilds=2~500,250,25,0,0~1,1,1,1,1&bonuses=0&bls=25,40&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,250,25,0,0;400,150,20,0,0;300,100,15,0,0;200,50,10,0,0;50,20,10,0,0;50,20,5,0,0;50,20,5,0,0;50,20,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=8,000,000.00&reel_set0=9,6,11,7,8,2,5,8,9,3,7,9,10,6,11,11,11,2,7,5,8,4,7,5,8,7,5,4,2,9,8,2,2,2,5,10,9,7,3,6,11,8,11,7,8,10,6,5,6,8,8~8,5,7,6,7,6,8,4,3,11,10,11,11,11,5,8,4,9,5,3,10,7,8,7,8,10,2,2,2,6,2,2,9,10,6,10,4,11,7,9,10,9~10,9,7,2,5,9,5,10,7,4,8,11,11,11,4,6,11,6,3,4,10,2,4,3,6,8,2,2,2,4,9,8,11,10,9,4,10,3,10,9,10,9,10~4,10,5,8,5,4,7,6,7,10,5,8,5,7,9,10,3,9,7,3,11,3,8,5,9,7,9,6,8,9,7,4,2,2,2,7,8,7,9,2,6,3,7,6,10,9,4,6,9,11,11,7,6,10,8,4,10,4,6,9,6,7,10,8,3,8,9,6~6,4,10,4,6,9,6,4,2,3,9,10,4,5,7,10,5,9,10,9,6,8,9,7,11,11,11,7,10,7,11,8,11,10,9,10,3,2,8,6,9,10,9,11,5,9,2,3,8,10,6,10,2,2,2,9,4,10,9,10,4,9,7,3,9,11,10,5,10,9,8,10,6,11,7,4,5,3,3&reel_set2=12,12,12,12,12,12,12,12,12,12,12,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,13,12,12,12,12,12,15,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,13,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,15,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12,12,12,12,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,13,12,12,12,12,12,15,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,13,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,15,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12,12,12,12,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,13,12,12,12,12,12,15,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,13,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,15,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12,12,12,12,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,13,12,12,12,12,12,15,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,13,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,15,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12,12,12,12,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,13,12,12,12,12,12,15,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,13,12,12,12,14,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,15,12,12,12,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12&reel_set1=12,12,12,12,12,12,12,12,11,12,11,12,11,11,12,12,12,12,12,11,11,11,12,12,12,12,12,12,12,12,12,12,12,12,11,12,11,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12,11,12,11,12,11,11,12,12,12,12,12,11,11,11,12,12,12,12,12,12,12,12,12,12,12,12,11,12,11,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12,11,12,11,12,11,11,12,12,12,12,12,11,11,11,12,12,12,12,12,12,12,12,12,12,12,12,11,12,11,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12,11,12,11,12,11,11,12,12,12,12,12,11,11,11,12,12,12,12,12,12,12,12,12,12,12,12,11,12,11,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12,11,12,11,12,11,11,12,12,12,12,12,11,11,11,12,12,12,12,12,12,12,12,12,12,12,12,11,12,11,12,12,12,11,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12&purInit=[{bet:2000,type:\"fs\"}]&total_bet_min=100.00";
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
            get { return 1.6; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        #endregion
        
        public WolfGoldUltimateGameLogic()
        {
            _gameID     = GAMEID.WolfGoldUltimate;
            GameName    = "WolfGoldUltimate";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{s1:{def_s:\"12,12,12,12,12,12,12,12,12,12,12,12,12,12,12\",def_sa:\"12,12,12,12,12\",def_sb:\"12,12,12,12,12\",s:\"12,12,12,12,12,12,12,12,12,12,12,12,12,12,12\",sa:\"12,12,12,12,12\",sb:\"12,12,12,12,12\",sh:\"3\",st:\"rect\",sw:\"5\"},s2:{def_s:\"12,12,12,12,12,12,12,12,12,12,12,12,12,12,12\",def_sa:\"12,12,12,12,12\",def_sb:\"12,12,12,12,12\",s:\"12,12,12,12,12,12,12,12,12,12,12,12,12,12,12\",sa:\"12,12,12,12,12\",sb:\"12,12,12,12,12\",sh:\"3\",st:\"rect\",sw:\"5\"},s3:{def_s:\"12,12,12,12,12,12,12,12,12,12,12,12,12,12,12\",def_sa:\"12,12,12,12,12\",def_sb:\"12,12,12,12,12\",s:\"12,12,12,12,12,12,12,12,12,12,12,12,12,12,12\",sa:\"12,12,12,12,12\",sb:\"12,12,12,12,12\",sh:\"3\",st:\"rect\",sw:\"5\"},s4:{def_s:\"12,12,12,12,12\",def_sa:\"12,12,12,12,12\",def_sb:\"12,12,12,12,12\",s:\"12,12,12,12,12\",sa:\"12,12,12,12,12\",sb:\"12,12,12,12,12\",sh:\"1\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in vs25ultwolgolGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in vs25ultwolgolGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                _logger.Error("Exception has been occurred in vs25ultwolgolGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
