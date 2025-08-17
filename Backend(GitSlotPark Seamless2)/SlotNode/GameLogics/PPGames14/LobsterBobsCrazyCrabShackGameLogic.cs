using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class LobsterBobsCrazyCrabShackGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20lobcrab";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,11,9,7,8,5,10,4,5,10,7,8,8,7,8&cfgs=8094&ver=3&def_sb=7,8,9,5,10&reel_set_size=11&def_sa=6,8,10,5,11&scatters=1~0,0,1,0,0~0,0,0,0,0~1,1,1,1,1;3~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"801667\",max_rnd_win:\"6000\",max_rnd_win_a:\"6000\",max_rnd_hr_a:\"381571\"}}&wl_i=tbm~6000;tbm_a~4000&reel_set10=3,11,7,2,3,9,3,9,11,10,3,2,11,6,3,9,8,3,10,3,9,3,7,11,10,8~3,6,7,11,10,11,3,11,5,9,10,8,7,9,11,8,3,3,3,10,9,11,10,3,11,10,4,2,11,8,10,8,6,9,11~3,8,6,3,6,3,10,11,8,5,3,6,9,7,8,10,3,5,3,9,7,3,7,6,3,11,6,11,3,7~2,9,10,6,11,10,11,10,9,11,3,10,3,3,3,10,2,8,7,11,10,7,11,8,10,3,4,8,11,8~3,4,9,10,9,7,3,9,3,11,9,8,10,11,7,10,3,10,3,10,11,3,11,7,8,3,2,3,7,5,3,9,6,8&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~2500,500,125,0,0~1,1,1,1,1&bonuses=0&bls=20,30&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;750,325,50,0,0;400,125,25,0,0;150,75,20,0,0;65,20,10,0,0;65,20,10,0,0;40,15,5,0,0;20,8,5,0,0;20,8,5,0,0&total_bet_max=10,000,000.00&reel_set0=2,8,9,8,3,9,8,10,5,9,6,3,6,11,3,3,3,10,3,7,9,3,2,3,7,9,2,9,8,10,5,7,3,11~4,7,1,5,11,10,3,10,3,10,11,6,3,3,3,8,7,2,11,6,3,3,10,11,9,8,11,4,11~4,10,2,9,7,10,8,10,3,9,10,7,11,3,3,3,8,10,2,11,3,10,8,3,6,3,11,6,7~2,11,3,3,8,3,10,11,10,11,10,11,10,6,3,3,3,9,11,10,7,10,7,11,10,5,9,10,11,1,3,6,3,3,9~5,11,9,7,3,11,10,11,2,10,4,3,10,2,3,10,3,3,3,10,7,3,6,9,8,11,9,3,9,8,10,2,11,10,9,6&reel_set2=11,3,5,4,5,4,10,11,10,6,11,6,10,3,4~10,9,9,11,7,8,9,10,7,10,11,10,8,11,1~9,6,7,3,10,9,4,6,4,11,3,3,3,8,11,10,3,5,10,2,7,8,3,11,9~5,8,7,5,8,9,10,8,9,7,11,8,4,6,5,1,9,8,7,9,9,4,7,9,9~7,6,9,8,5,8,5,9,6,9,8,10,8,2,7,3,9,4,7,3,9,4,7,8,7,9,5&reel_set1=3,11,10,3,6,9,10,5,7,10,4,7,8,11,9~11,9,10,9,10,7,6,4,1,11,10,11,8,11,7,3,11,1,10,4,7,10,1,11,10,5~7,11,1,10,11,6,10,11,7,10,6,9,5,8,1,9,11,9,5,7,10~5,6,5,11,5,6,9,10,7,10,1,11,9,11,10,11,1,3,11,10,7,9,1,8,7~6,8,11,9,10,11,5,3,5,10,4,9,7,10,2,7,3&reel_set4=10,6,8,5,7,6,7,3,6,10,8,7,6,7,3,10,8,5,10,6,5,7,8,7~6,9,11,9,11,1,4,9,7,6,4,7~7,6,4,8,10,9,8,9,11,9,10,1,10,2,9,11,4,7,5,6,5,8,11,7,6~7,11,10,5,3,3,3,8,3,3,4,9,2,6~5,11,8,11,10,7,6,5,11,5,9,10,9,10,3,8,10,9,2,3,4,10,9,4&purInit=[{bet:2000,type:\"default\"}]&reel_set3=8,9,3,9,8,4,9,8,10,7~9,8,9,5,11,6,3,11,8,11,3,3,3,11,5,9,5,3,6,8,5,8,9,6~9,7,8,9,11,10,8,10,5,8,10,8,9,4,1,11,6~4,5,11,1,11,6,10,5,7,4,11,6,2,8,9,7~7,11,10,5,8,3,9,4,2,6&reel_set6=10,5,8,4,6,3,5,3,8,6,4,11,10,8,6~7,5,7,9,11,3,9,2,9,11,3,5,2~5,7,9,5,11,9,11,1,11,5,7,9,7~8,10,9,6,4,8,11,9,11,6,10,7,8,1,7,4,11,5,7~4,6,10,9,10,8,7,10,3,11,8,4,3,3,3,9,3,6,4,7,3,7,9,7,4,10,9,5,11,6,3,8&reel_set5=3,9,3,11,9,7,4,5,3,3,3,11,9,4,5,7,10,7,11,7,10,4,5~6,8,1,8,10,6,4,8,6,8,4,10,4,10,8,10,4,10~4,7,9,7,1,2,7,11,7,2,11,9,11,4,11,5,7,11,5,4,5~10,8,11,6,5,11,9,8,7,9,5,4,11,7,10,3,7,8,10,6,7,10,6,8,9,6,10,3~7,11,7,11,10,3,5,6,8,10,9,10,6,7,9,8,9,11,3,8,7,9,8,11,7,9,4,8&reel_set8=9,8,9,8,9,10,3,10,3,8,3,9,7,4,8,9~11,6,11,5,11,8,9,3,6,5,3,9,3,8,11,5,3,5~8,5,3,1,9,10,3,10,11,9,9,11,8,9,7,6,11~1,11,10,7,10,3,11,4,7,4,10,3,6,5,11,9~3,3,11,3,3,8,6,4,6,4,3,5,7,11,10,3,10,5,6,3,9,11,7,8,9,5&reel_set7=3,4,3,5,10,6,10,3,11,6,10,3,10,4,11,4~11,9,3,9,10,7,11,7,1,8,7,3,9,8,10,11,10~11,3,11,3,9,10,3,3,10,4,5,3,3,3,9,10,8,9,7,5,3,11,8,7,11,6,4,7,3~10,9,8,5,7,8,7,9,1,3,8,9,8,7,3,6,4,7,4,11,7,9~3,3,10,9,4,8,9,7,3,5,7,9,8,9,7,3,6,8,6,8,3,7,3,6&reel_set9=8,3,8,3,6,7,6,5,3,5,6,10,3,6,8,6,5,3,7,8~11,9,3,1,7,4,6,9,6,3,6,11,7,4,9,7,11,3,9~5,6,1,3,8,10,11,7,9,10,3,7,8,11,4,9,10,5,6,3~9,5,7,3,8,11,3,3,4,9,11,10,5,3,3,3,5,8,10,5,8,10,3,9,11,4,11,8,4,9,3,4~9,3,11,10,5,3,10,11,10,11,8,10,3,9,3,4,3,9,5,9,6,3&total_bet_min=10.00";
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
        public LobsterBobsCrazyCrabShackGameLogic()
        {
            _gameID = GAMEID.LobsterBobsCrazyCrabShack;
            GameName = "LobsterBobsCrazyCrabShack";
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
	
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in LobsterBobsCrazyCrabShackGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in LobsterBobsCrazyCrabShackGameLogic::readBetInfoFromMessage", strGlobalUserID);
                    return;
                }
		
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, betInfo.LineCount);
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
                _logger.Error("Exception has been occurred in LobsterBobsCrazyCrabShackGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
