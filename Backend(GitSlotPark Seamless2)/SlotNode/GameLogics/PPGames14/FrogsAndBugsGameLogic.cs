using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FrogsAndBugsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysfrbugs";
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
                return 5;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=14,14,14,14,14,14,14,14,14,14,14,14,4,10,3,9,5,13,5,8,11,13,4,9,6,12,6,9,3,13&cfgs=8032&ver=3&def_sb=3,8,5,9,7,10&reel_set_size=4&def_sa=7,9,8,3,7,9&scatters=1~35,30,25,20,15,10,5,0,0,0~0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"1333333333\",max_rnd_win:\"3000\"}}&wl_i=tbm~3000&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&purInit_e=1&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0;50,30,20,15,0,0;30,25,15,10,0,0;25,20,12,7,0,0;20,15,10,5,0,0;15,10,7,4,0,0;10,7,5,3,0,0;7,5,3,2,0,0;7,5,3,2,0,0;4,3,2,1,0,0;4,3,2,1,0,0;4,3,2,1,0,0;0,0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=12,9,12,13,9,12,10,12,10,4,11,5,9,11,10,5,13,6,13,6,11,13,7,9,10,9,8,4,13,5,8,7,8,3,13,12,10,5,10,9,11,13,7,4,3,11,10,9,12,3,6,8,13,7,4,11,12,10,13,12,11,8,9~13,11,12,10,12,4,13,7,3,10,7,8,11,10,1,8,5,9,13,10,4,9,6,8,12,11,9,3,11,10,11,4,10,1,12,5,13,10,7,13,7,12,5,11,9,11,13,13,10,9,12,5,9,12,1,9,7,12,11,13,3,6,13,12,8,6,3,5,9,4,6,13,12,13,6~8,9,11,4,12,11,13,3,9,6,13,10,11,10,5,13,11,10,12,9,5,10,13,9,11,12,4,13,12,9,7,6,12,3,11,10,8,9,1,7,5,8,11,4,12,9,5,7,10,13,6,7,13,12,10,1,9,3,10,12,13,6,8,4,11~10,1,9,3,11,10,4,3,11,13,5,9,9,9,13,12,7,11,3,12,9,10,13,11,6,8,10,12,11,10,5,10,11,8,11,4,9,8,9,9,10,8,11,13,9,1,13,11,13,6,4,7,13,9,7,12,5,4,3,6,13,12,13,12,6,10,13,12,5,9,4,7,11,12,9,6,12,7,11~9,5,12,1,4,10,12,13,11,8,11,9,13,11,10,5,12,5,4,13,3,10,11,9,7,9,12,11,1,5,10,13,11,1,10,3,5,9,6,12,1,3,7,10,9,13,12,11,6,10,8,9,6,7,4,9,6,7,13,8,13,8,5,4,3,8,7,10,12,10,11,3,13,11,13,7,9,13,12,3,10~10,12,4,11,13,11,4,12,10,9,10,5,12,11,6,8,6,13,10,5,12,7,8,7,13,10,7,9,6,3,9,3,11,13,11,13,3,11,13,9,12,9,13,4,6,8,9,11,10,7,13,4,9,5,8&reel_set2=12,9,11,12,4,9,11,12,4,12,11,10,7,5,12,9,12,9,6,9,3,12,12,12,12,5,10,3,8,12,5,11,8,10,6,7,4,11,12,7,12,12,6,8,11,12,12,10,9~3,13,1,7,11,1,11,9,5,1,11,5,9,1,3,1,13,1,11,9,1,11,13,1,11,13,3,1,13,1,3,9,1,9,11,7,9,11,1,9,1,5,13,11,1,11,11,13,11,9,7,1,9,5~10,1,6,1,8,1,12,12,8,10,12,10,10,1,13,1,8,12,8,10,1,4,12,6,10,1,13,1,12,1,4,10,1,10,4,1,4,1,6,8,10,1,6,12,12,10,8,1~1,4,10,1,11,1,8,1,3,1,3,11,12,11,10,1,4,1,9,4,13,1,5,10,7,13,10,1,5,1,11,1,12,1,8,1,13,1,3,10,9,4,6,7,1,10,11,8,11,13,6,1,12,1,13,6,1,7,1,9,10,1,13,12,1,10,1,10,11,3,12,7,1,12,11,9,9~13,1,8,13,10,1,10,1,5,1,8,9,1,7,1,13,1,4,1,10,5,4,1,3,1,12,7,4,11,1,10,8,1,9,11,10,6,4,7,1,13,12,9,1,11,9,1,7,6,13,1,3,12,1,10,13,12,13,1,8,1,6,12,11,1,10,13,1,11,13,9,3,12,1,9,1~9,12,11,10,4,5,12,8,4,12,8,13,10,9,5,12,7,5,12,7,11,10,8,11,6,3,11,4,7,13,12,13,4,10,11,13,10,10,9,3,9,5,3,13,6,8,7,13,9,7,6,11,13,9,13,6&reel_set1=12,11,7,13,11,5,8,5,13,9,10,11,8,3,13,12,9,6,11,8,11,13,6,13,5,12,4,13,6,10,5,6,7,3,8,4,13,9,12,9,8,13,6,12,11,9,8,12,13,7,4,12,9,13,4,11,7,13,12,9,12,11,10,11,10,6,10,9,11,3,7,3,12,10,12,13,10,13,6~10,11,10,7,10,11,5,11,13,12,9,11,3,11,10,13,6,7,12,4,7,13,12,6,8,13,11,13,8,3,4,10,3,12,9,8,7,8,9,12,13,7,13,11,12,4,3,12,11,9,6,12,9,10,9,6,5,13,10,13,4,7,12,6,5,10,12,9,5,13,11~13,10,12,13,3,8,5,13,12,8,13,12,10,8,11,8,13,6,13,7,9,10,11,6,3,9,11,4,10,9,11,12,9,6,12,4,10,12,5,3,9,8,11,10,7,13,13,11,11,6,12,3,11,11,7,6,10,11,10,4,9,12,7,11,5,13,13,9,10,12,4,8,5,12,7~7,11,13,4,10,6,12,13,11,13,5,12,8,12,5,9,10,11,7,13,10,9,6,11,13,11,13,4,9,10,12,5,13,11,8,10,9,11,6,13,7,13,12,7,4,9,8,13,9,3,13,3,11,11,3,6,12,7,12,10,9,11,12,6,13,11,4,8~8,9,13,11,12,10,11,13,8,6,7,5,10,13,9,13,5,10,7,13,10,12,13,5,9,12,6,11,3,8,7,10,12,6,8,4,9,13,3,4,11,8,10,12,4,3,12,11,12,9,10,7,6,9~13,11,13,7,10,12,13,3,11,12,13,6,9,12,5,6,13,7,3,13,10,11,12,11,12,10,9,5,13,8,10,13,5,12,10,12,10,13,9,11,3,7,8,10,5,8,11,9,13,11,6,11,4,9,11,13,7,12,4,11,9,5,8,10,8,12,6,13,8,4,7,3,8,9,10,9&purInit=[{bet:2500,type:\"default\"}]&reel_set3=11,3,12,13,12,10,13,9,10,11,4,13,10,6,9,7,11,10,4,13,8,9,12,6,8,9,5,8,7,10,3,13,11,12,13,7,3,9,11,13,12,9,12,13,5,11,13,5,11,12,9,8,4,10,6,8,12,10,8,7,6,12~10,11,10,13,12,7,8,3,5,6,11,12,5,9,13,11,13,9,5,3,13,12,7,9,12,9,8,13,8,13,6,8,10,7,12,11,12,9,12,4,10,6,8,13,12,13,4,7,11,7,10,12,11,4,13,6,5,11,9,4,10,11,3~13,12,7,12,11,12,10,11,8,10,8,9,7,6,11,3,13,12,9,13,12,8,11,12,4,9,4,5,9,7,10,5,13,10,6,3,11,12,13,8,6,10,5,13,9,13,9,13,7,6,11,6,4,8,13,5,11,12,9,3,4,11,10,8,12,11,5,12,9,3,9,10,12,10,11,10,13,6,11,12,10,7,11,13,7,13,4,11~10,7,10,12,11,13,11,6,11,9,7,11,12,11,8,7,4,11,10,13,12,8,12,9,11,9,4,11,7,3,13,5,13,4,13,11,6,8,11,9,12,13,8,6,10,5,3,5,12,3,7,13,6,12,9,5,13,6,10,3,13,12,8,10,13,11,9,10,9,13,8,13,13,4~12,11,10,11,10,5,4,12,5,13,6,7,9,8,11,12,6,11,9,7,8,13,12,9,13,12,10,6,13,9,3,10,6,11,10,12,8,3,7,4,8,13,11,13,9,12,5,11,10,5,7,4,12,13,9,12,9,7,5,8,10,4,12,13,10,4,10,13,7,11,8,13,3,7,6,9,12,8,11,13,3,13,9,10,12~11,10,13,5,3,10,11,9,4,5,13,12,13,7,13,8,10,11,10,11,6,8,3,12,7,5,4,5,8,9,12,10,6,12,7,3,9,13,11,12,6,12,8,11,13,4,10,9,13,9,10,13,6,9&total_bet_min=8.00";
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
        public FrogsAndBugsGameLogic()
        {
            _gameID = GAMEID.FrogsAndBugs;
            GameName = "FrogsAndBugs";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "6";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

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
	
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FrogsAndBugsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in FrogsAndBugsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
