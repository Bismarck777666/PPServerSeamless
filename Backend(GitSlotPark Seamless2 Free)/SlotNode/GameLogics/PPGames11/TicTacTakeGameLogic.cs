using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TicTacTakeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10tictac";
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
                return "def_s=6,8,8,4,7,3,9,6,6,8,4,8,9,5,7&cfgs=5134&ver=2&def_sb=5,9,3,5,8&reel_set_size=9&def_sa=5,10,9,3,8&scatters=&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2163331\",max_rnd_win:\"2200\"}}&wl_i=tbm~2200&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,100,25,0,0;500,75,15,0,0;250,50,10,0,0;50,12,5,0,0;50,12,5,0,0;50,12,5,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=6,8,4,8,3,6,6,4,8,8,8,6,6,8,4,8,8,8,6,8,8,5,8,8,7,6,4,4,8,8,6,6,6,8,8,6,8,3,6,8,6,4,6,8,4,7,8,8,6,5,6,6,8,7,8,8,8,4,4,8,8,4,8,8,8,6,6,8,6,4,3,8,8,6,8,8,8,7,5,4,8,7,6,8,4,8,4,6,6,8,6,8,8,8,7,6~9,5,7,5,5,6,7,9,5,5,7,9,10,5,7,7,4,7,7,7,7,7,5,7,7,10,5,3,5,7,7,7,5,9,7,7,7,5,7,9,9,9,5,7,7,5,7,7,9,6,7,7,7,7,10,5,10,7,5,10,10,10,7,7,3,7,10,8,9,7,7,5,5,10,8,7,7,5,5,10,7,5,5,5,7,7,7,5,7,7,5,5,5,9,5,7,5,7,7,7,8,5,9,5~6,10,6,8,8,10,6,6,10,10,6,10,6,6,6,8,9,8,6,3,8,6,8,6,8,8,6,6,8,9,9,9,6,6,7,6,9,6,6,8,8,6,8,5,6,5,10,10,10,9,8,7,6,4,10,6,6,8,4,8,6,4,4,8,8,8,6,8,8,6,8,6,9,8,9,8,6,3,9,6~5,7,7,5,5,7,8,7,7,9,7,9,7,5,10,7,7,7,7,7,5,9,8,9,9,7,7,5,7,5,7,7,10,7,5,10,9,9,9,7,10,10,7,4,7,7,5,7,7,5,7,5,10,7,9,7,10,10,10,5,7,5,7,7,5,7,7,6,7,5,7,5,5,9,5,7,5,5,5,8,5,6,7,5,7,5,7,7,3,10,7,5,7,3,5,5,6~8,5,8,4,8,8,3,8,8,6,8,6,8,4,8,8,7,6,6,6,8,7,4,4,6,4,8,8,6,8,4,6,8,3,6,8,6,6,8,8,8,6,8,6,8,8,6,6,8,8,4,8,6,7,6,4,5,8,8,7&reel_set2=3,5,4,8,7,5,4,7,6,7,5,5,3,6,7~4,8,9,8,6,10,10,10,5,10,8,4,9,9,9,3,9,10,9,7,10,4~10,9,9,5,10,10,10,3,8,4,6,3,9,9,9,10,9,5,7,4,10~9,7,9,8,10,10,10,3,10,4,10,9,6,9,9,9,4,10,8,5,9,9,10,10~3,5,6,7,5,7,8,4,5,4,6,5,7&reel_set1=7,5,5,7,7,5,5,7,7,5,6,7,5,5,7,7,7,5,7,8,3,7,7,7,7,7,8,4,7,5,4,5,5,7,5,7,3,7,7,7,5,4,7,6,5,5,5,7,5,5,5,7,3,7,7,7,7,7,5,7,7,7,7,7,5,5,5,8,6~3,6,8,8,10,8,8,6,6,8,8,9,6,8,4,9,10,4,6,6,5,6,8,8,10,8,9,9,9,8,6,4,7,8,8,7,6,6,8,6,8,5,8,8,9,6,3,4,4,8,10,8,4,8,10,10,6,10,10,10,3,9,6,8,8,4,8,8,6,8,8,4,9,8,8,4,5,9,7,8,6,10,4,8,9,8,4,8,4~5,5,7,5,4,4,7,9,6,10,6,6,5,5,5,4,10,8,5,8,7,7,6,5,7,5,5,9,6,9,9,9,5,10,7,4,9,6,7,3,9,7,9,7,5,5,10,10,10,7,8,5,3,7,6,7,6,5,5,7,5,5,6,7,7,7,7,5,10,8,10,10,7,7,5,6,7,7,6,9,7~4,8,6,8,7,8,6,9,8,6,8,6,10,8,3,6,8,8,6,9,8,4,8,10,9,9,9,8,9,10,4,5,8,8,6,6,3,8,4,6,6,8,7,4,5,8,6,6,4,5,4,7,10,10,10,8,8,4,8,8,3,8,8,4,4,8,8,9,10,9,8,10,10,8,8,6,8,6,8,8,6,4~7,7,5,7,7,5,6,3,7,5,7,5,7,7,5,7,7,5,5,7,7,7,7,7,6,7,5,7,7,5,7,7,5,7,8,5,7,5,7,5,7,5,7,8,5,5,5,7,7,5,7,7,5,7,8,7,3,7,5,7,5,5,7,5,5,4,7,5&reel_set4=5,6,5,7,5,5,5,6,7,6,7,7,7,7,3,7,6,7,7,5,3,3,3,7,7,5,3,6,3,4~4,8,6,8,8,6,4,6,4,8,8,4,6,8,4,8,8,6,8,8,6,8,8,8,9,8,9,4,8,8,6,9,8,8,6,4,8,8,4,8,4,8,8,4,4,6~7,8,7,5,7,8,7,3,7,7,7,7,8,7,10,8,7,8,5,7,8,7,5,5,5,8,7,8,7,10,5,8,10,7,3,3,3,5,7,3,7,7,3,8,5,7,7,8~8,6,4,6,8,6,4,8,8,4,8,8,6,4,8,4,6,8,8,6,10,4,8,8,6,6,4,9,8,8,4,4,8,9,8,10,8~7,3,6,5,7,6,7,5,7,5,5,5,7,5,6,7,7,6,6,7,6,7,7,7,7,3,7,7,6,7,7,6,5,4,7,3,3,3,7,3,7,7,5,3,5,5,7,6,5&purInit=[{type:\"d\",bet:1000}]&reel_set3=5,7,7,3,7,5,5,5,4,6,7,7,5,7,5,7,7,7,7,6,7,3,6,5,5,7,3,3,3,7,6,7,7,3,6,3,6~8,4,9,8,6,6,4,8,4,8,8,4,8,8,10,8,4,6,8,4,6,6,9,8,9,8,4,8,8,4,8,4,6,8,10,4,4,8,6,8,8,6,6,8~5,8,7,5,3,8,7,7,7,7,3,7,10,5,7,3,7,8,8,7,5,5,5,7,8,7,8,7,7,8,8,7,3,3,3,8,5,10,7,8,7,10,5,8,7,7~6,8,4,8,9,8,4,6,8,6,4,6,6,8,8,8,4,8,8,6,8,8,9,4,8,8,4,8,8,4,8,8~6,7,7,5,5,5,6,7,7,5,5,7,7,7,7,6,7,5,3,6,7,3,3,3,4,7,7,6,3,3,7&reel_set6=8,4,3,3,8,3,3,3,4,4,3,6,8,4,8,4,4,4,5,3,7,3,5,8,3,5,5,5,8,4,6,8,6,5,8,8,8,5,7,6,5,4,8,7,6~5,7,7,3,7,10,5,5,5,8,6,9,7,5,3,9,4,4,4,7,7,8,5,7,10,3,3,3,8,4,10,4,10,9,7,5~6,6,5,8,3,5,5,5,4,6,8,10,6,4,4,4,9,8,6,10,8,3,3,3,6,6,4,4,8,6,6,6,8,8,9,6,4,9,8,8,8,3,8,6,10,9,9,6,10~7,8,3,10,7,5,5,5,8,7,4,7,5,4,4,4,9,5,10,9,3,5,3,3,3,7,8,6,5,9,10,7~8,4,8,8,3,3,3,8,5,7,7,5,5,8,4,4,4,8,4,4,3,6,6,5,5,5,8,5,6,3,4,6,3,8,8,8,5,7,4,3,6,3,8&reel_set5=4,7,4,8,4,7,6,4,4,4,8,6,8,4,6,8,8,3,8,8,8,6,5,8,5,8,6,6,5,5,5,8,8,4,8,8,6,8,8,3,3,3,6,8,6,8,7,8,8,5,6,6,6,7,6,8,8,3,8,5,6,7,7,7,8,6,8,7,6,6,4,8,8~4,3,10,7,10,3,3,8,7,7,5,5,5,10,8,9,7,9,10,8,7,7,9,7,4,4,4,7,5,5,6,10,9,10,7,10,4,7,8,3,3,3,6,10,8,9,5,7,8,9,7,7,9,9,9,7,5,7,10,9,7,8,10,7,9,10,10,10,5,7,3,7,10,10,8,10,7,7,9,7~4,3,6,9,5,5,5,8,10,9,9,10,9,4,4,4,9,6,9,8,6,10,3,3,3,6,6,4,6,10,6,6,6,6,10,8,3,8,8,8,8,5,8,6,10,6,10,9,9,9,10,5,6,7,8,10,10,10,6,5,8,4,6,8,6~10,8,6,9,10,7,7,3,9,8,7,5,5,5,9,10,3,7,7,9,7,7,5,9,3,9,4,4,4,7,5,10,4,7,10,7,7,3,10,10,7,7,3,3,3,10,10,7,7,8,10,9,9,7,5,9,10,9,9,9,8,9,8,7,10,7,7,9,8,7,10,5,10,10,10,4,10,10,7,8,7,7,6,5,9,4,5,7,8~8,6,8,8,7,6,4,4,4,7,8,6,3,6,4,8,8,8,6,8,8,6,5,4,8,5,5,5,7,8,8,6,8,5,8,8,3,3,3,6,8,8,5,8,4,8,6,6,6,4,8,4,4,8,6,8,7,7,7,3,6,5,8,8,6,6,7,7&reel_set8=3,4,5,5,3,3,5,8,4,3,4,3,8,3,3,3,4,3,4,5,7,4,6,8,4,8,3,4,3,7,6~5,5,7,3,4,3,5,5,5,6,8,7,7,4,3,6,8,6,4,4,4,5,6,7,7,5,7,8,7,8,3,3,3,7,4,5,4,7,7,3,3,8,3~5,4,8,4,5,5,5,6,5,3,3,6,6,4,4,4,6,4,8,3,4,3,3,3,6,4,6,4,8,6,6,6,8,6,5,4,8,8,8,4,6,6,8,4,3~3,4,7,3,8,7,4,7,5,5,5,3,3,8,5,3,3,6,4,4,8,4,4,4,5,7,7,4,3,4,3,5,7,3,3,3,5,5,7,5,3,7,7,4,8,4,7~5,3,5,5,8,4,3,4,6,8,4,8,3,8,3,3,3,4,3,8,7,4,3,4,5,3,7,6,4,3,5,3,4&reel_set7=6,8,7,6,7,6,8,7,8,8,8,6,8,6,7,6,7,7,8,8,7,7,7,7,6,7,6,7,7,6,6,7,6,6,6,7,6,6,7,8,7,8,6,7,8,7~6,8,5,8,5,5,5,4,5,6,8,5,8,4,4,4,8,4,6,8,8,4,8,8,8,8,6,8,4,8,8,5~5,7,7,5,7,3,7,7,7,7,7,7,5,7,5,3,7,7,5,3,5,5,5,3,7,5,3,5,7,5,5,4,3,3,3,3,7,7,4,5,7,7,4,7,4,4,4,7,7,3,4,7,3,5,7,7~5,8,8,4,8,8,4,5,5,5,6,6,8,8,5,8,6,8,4,4,4,6,8,8,4,8,5,8,8,8,8,4,8,5,5,6,8,6,4,8~7,6,7,7,8,8,8,6,7,7,6,6,8,6,8,7,7,7,7,6,7,6,8,8,7,7,6,6,6,7,7,6,7,7,6,7,8,6&total_bet_min=20.00";
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
	
	
        #endregion
        public TicTacTakeGameLogic()
        {
            _gameID = GAMEID.TicTacTake;
            GameName = "TicTacTake";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        	dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
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
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in TicTacTakeGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in TicTacTakeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
