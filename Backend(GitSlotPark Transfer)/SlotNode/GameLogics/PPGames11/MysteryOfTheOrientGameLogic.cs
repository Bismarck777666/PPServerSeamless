using Akka.Event;
using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MysteryOfTheOrientGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysmorient";
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
                return 6;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,3,14,11,8,13,8,10,6,5,10,4,17,13,8,11&cfgs=7150&ver=3&def_sb=4,14,17,6,13&reel_set_size=5&def_sa=9,11,13,3,17&scatters=1~50,30,25,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"19267822\",max_rnd_win:\"8035\"}}&wl_i=tbm~8035&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&purInit_e=1&wilds=2~100,0,0,0,0~1,1,1,1,1;19~0,0,0,0,0~1,1,1,1,1;20~0,0,0,0,0~1,1,1,1,1;21~0,0,0,0,0~1,1,1,1,1;22~0,0,0,0,0~1,1,1,1,1;23~0,0,0,0,0~1,1,1,1,1;24~0,0,0,0,0~1,1,1,1,1;25~0,0,0,0,0~1,1,1,1,1;26~0,0,0,0,0~1,1,1,1,1;27~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,50,25,0,0;75,40,20,0,0;60,30,15,0,0;50,25,12,0,0;50,20,10,0,0;40,15,8,0,0;30,12,5,0,0;30,10,5,0,0;25,5,2,0,0;20,5,2,0,0;20,5,2,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=8,3,8,9,5,12,11,4,17,11,11,4,11,10,17,10,10,7,10,11,10,11,5,4,6,11,5,5,8,5,12,10,10,12,7,8,11,11,1,4,11,7,10,8,3,11,10,12,5,10,11,8,10,12,12,8,11,10,12,4,9,8,3,8,5,13,10,6,1,11,10,4,7,3,7,10,11,5,10,8,5,8,11,13,8,9,12,4,7~5,4,6,5,8,8,3,5,13,6,11,7,4,1,14,2,9,9,4,5,9,9,6,9,13,2,10,11,1,10,8,8,14,8,9,13,7,8,14,13,8,9,7,12,13,5,13,9,9,13,6,7,13,12,17,9,8,10,9,13,2,7,9~12,6,14,11,9,3,5,8,12,13,9,6,13,5,8,9,13,12,9,12,13,8,5,13,9,5,8,12,1,9,6,5,8,4,9,6,13,6,9,2,12,6,2,8,9,1,9,10,12,6,9,11,3,5,12,8,5,13,14,4,3,14,11,13,17,12,9,13,10,13,8,4,9,5,13,7,5,3,6,12,9,13,7,6,12,8,9,8~10,6,12,11,10,11,6,3,6,12,14,5,7,3,9,13,10,2,11,12,5,6,9,7,2,4,9,14,12,14,9,8,6,10,9,9,7,13,8,1,6,9,2,10,4,7,8,12,3,12,1,12,9,6,10,12,5,9,17~13,9,9,13,6,9,13,17,3,7,4,1,5,11,12,11,1,5,9,6,5,6,11,6,10,13,12,10,8,10,12,11,8,9,13,9,5,10,8,4,8,4,11,6,17,4,6,13,13,4,5,1,13,8,3,7,4,13,11,9,6,4,11,8,10,9,10,11,9,6,13,9&reel_set2=18,11,10,12,10,12,7,8,18,8,18,4,3,10,5,13,11,11,12,7,10,11,8,17,7,3,8,11,16,12,9,1,4,3,11,6,5,8,13,10,9,11,18,18,7,4,11,7,5,11,11,6,10,3,10,8~10,7,9,13,4,8,9,5,9,3,10,18,9,8,6,13,7,9,13,9,13,8,4,6,13,13,8,7,8,3,12,18,13,9,6,9,8,18,14,5,13,13,8,18,14,11,1,3,5,9,4,16,14,2,10,14,2,12,3,8,14,10,7,11~9,6,18,10,13,8,13,6,9,12,2,5,3,10,13,7,18,8,13,3,11,9,6,2,8,6,9,18,9,16,14,6,8,7,5,12,9,12,9,12,5,18,8,14,18,11,6,12,9,5,3,14,1,10,13,8,12,4,12,9,8,12,10,9,18,14,6,8,14,13,3,8,9,18,4,9,13,14,11,12,13,4,12,14,8~6,10,2,4,6,10,12,3,8,9,12,3,10,9,7,2,9,9,2,12,14,6,12,9,7,12,5,6,10,9,11,16,11,8,4,18,12,14,6,14,1,10,8,18,11,9,11,10,12,7,10,13,14,9,9,12,9,12,14,2,5,6,13,6,12,11,9~10,18,9,4,8,4,11,16,6,7,11,12,9,6,18,9,13,3,18,9,11,13,9,13,5,6,11,6,1,11,8,4,6,4,5,8,9,11,6,11,6,3,17,9,13,5,12,10,8,4,13,11,8,6,7,6,9,13,11,10,4,12,10,12,7,6,9,9,13,4,9,10,5,8,10,18,9,13,10,4,7,10,9,11,8,10,11,9,13&reel_set1=4,8,4,8,10,13,10,1,7,11,12,11,18,11,10,4,10,5,11,1,10,7,16,8,18,10,4,5,11,11,3,11,8,11,8,18,3,8,4,18,3,7,13,6,11,8,7,10,3,10,11,3,17,10,12,11,9,11,7,11,16,10,11,18,5,3,11,7,3,8,18,18,12,18,6,10,9,8,12,3,7,18,11,10~2,8,5,1,8,13,9,7,6,3,6,7,9,9,11,18,5,9,13,6,14,5,18,14,12,13,18,9,9,13,9,13,9,18,18,14,3,13,9,7,4,2,9,12,10,11,8,2,6,13,14,8,14,8,4,16,7,10,8,9,3,13,3,10,8,4,9,13,14,13,9,1,7,2,8~14,5,13,6,10,6,9,10,8,13,14,9,14,3,14,9,13,9,18,8,2,6,12,9,13,5,9,5,2,3,8,9,4,7,8,12,4,14,8,18,6,12,1,13,18,18,8,12,12,13,7,3,9,11,12,12,13,1,8,6,16,5~5,2,6,9,12,13,9,9,2,14,10,11,6,1,7,12,14,12,11,6,12,8,9,6,3,9,10,9,10,9,11,12,10,6,14,7,3,6,7,10,12,5,2,11,4,16,9,6,9,8,1,14,12,4,9,3,12,9,12,14,7~6,10,7,9,11,9,18,9,10,11,8,9,5,16,6,11,4,11,4,1,3,13,4,11,17,13,6,9,7,6,8,6,10,8,13,9,9,13,9,5,10,7,9,11,8,6,13,3,12,4,9,10,6,18,1,9,13,18,12,6,10,5,13,11,4,16&reel_set4=8,4,10,11,12,5,18,7,10,18,13,11,11,3,18,11,10,10,12,18,8,10,13,8,11,9,8,3,4,7,11,11,8,10,18,12,9,8,6,10,8,11,10,10,8,18,3,5,4,6,11,10,17,12,7,1,7,5,11~8,18,9,11,5,13,2,8,5,13,8,4,13,8,5,6,9,8,8,3,6,13,8,7,9,13,11,14,7,10,9,18,8,10,9,8,18,8,5,18,9,3,9,6,8,6,11,18,18,12,9,9,4,7,8,10,7,14,7,13,18,9,8,9,13,4,18,18,8,9,18,14,5,9,12,9,8,4,8,13,1~9,13,12,18,7,4,8,6,2,10,5,3,13,12,8,13,3,3,8,9,7,12,8,3,9,5,13,2,5,12,4,6,9,12,12,8,9,14,11,6,10,12,9,13,11,18,5,9,18,7,14,6,1,8,9,13,18,8,8,13,18~11,12,3,10,8,10,6,12,9,8,12,10,18,6,10,11,18,14,10,9,13,10,9,7,4,11,9,7,9,10,5,6,11,5,12,6,9,5,9,1,7,2,7,3,12,10,6,3,12,6,12,2,9,4,6,10,6,13,12,3,8,12,9,7,11,3,4,13,9,12,9,14,6,8,12,6,10,9,11,12,10,3,5,7,14,13~9,4,18,13,12,13,13,6,12,8,13,5,4,8,4,13,6,18,11,10,7,8,13,11,8,7,11,18,13,4,10,9,9,13,9,10,13,11,8,9,9,12,10,6,9,18,10,6,5,4,9,11,3,12,1,8,3,11,13,17,8,9,11,6,11,5,10,8,9,11,4,6,10,9,13,13,3,4,9,6,9,6,7,13,11,4,7&purInit=[{bet:2500,type:\"default\"}]&reel_set3=4,12,18,13,8,9,5,8,10,8,10,10,11,1,18,11,12,7,8,13,9,12,3,7,18,11,4,10,10,11,10,11,18,12,5,10,10,4,16,8,11,4,7,12,10,8,17,11,10,8,3,11,10,3,18,6,5,8,7,10,18,4,3,8,18,11,9,12,11,3,11,11,8,7,12,11,8,4,11,10,3,8,11,8,5,3,6,18,10,18,5,7~7,9,3,8,13,9,12,8,18,5,18,2,18,9,13,8,8,10,7,9,9,4,14,9,10,13,18,14,12,8,8,14,9,18,13,5,6,8,6,13,8,5,9,4,7,10,11,8,2,16,3,1,5,11,3,7,4,9,6,8,11~5,14,18,8,12,2,9,11,3,18,6,12,12,16,8,8,12,13,12,8,8,13,7,9,4,9,12,18,2,13,3,6,10,3,5,12,14,13,9,13,4,5,8,8,6,12,10,18,18,18,13,13,5,12,7,9,6,18,11,6,3,6,4,11,14,12,8,9,18,18,9,13,1,8,13,9,8,13,9,3,10,12,7,13,9,18,18,5,9,3,18,6,8,13,8,9,8~10,13,3,9,10,6,8,5,7,4,5,9,8,14,10,9,10,12,9,11,12,11,6,18,3,12,13,9,6,9,9,11,12,10,11,7,6,5,4,2,7,9,14,6,9,5,12,11,6,12,10,9,8,12,18,9,7,18,10,12,9,7,10,9,3,13,3,10,6,10,11,5,14,12,9,10,4,12,2,1,7,12,10,2,3,10,9,18,9,6,11,16,3~5,8,13,9,3,9,9,11,13,7,8,4,8,13,5,8,6,4,11,10,13,11,12,13,6,1,4,10,3,12,11,6,9,18,10,4,6,4,13,11,10,6,4,9,18,11,4,12,9,10,13,17,6,9,11,13,7,11,18,4,9,7,10,9,9,6,5,10,13,6,3,8,10,9,5,13,9,13,18,16,9,8,9,8,6,7,11,6,10&total_bet_min=8.00";
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
        public MysteryOfTheOrientGameLogic()
        {
            _gameID = GAMEID.MysteryOfTheOrient;
            GameName = "MysteryOfTheOrient";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{fs_h3:{reel_set:\"1\"},fs_h4:{reel_set:\"2\"},fs_h5:{reel_set:\"3\"},fs_h6:{reel_set:\"4\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
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
	
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MysteryOfTheOrientGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in MysteryOfTheOrientGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
