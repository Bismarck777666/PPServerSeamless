using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class CosmicCashGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40cosmiccash";
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
            get { return 40; }
        }
        protected override int ServerResLineCount
        {
            get { return 40; }
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
                return "def_s=17,17,17,17,17,17,17,17,17,17,2,13,8,5,11,16,9,11,7,9,2,14,13,6,5,16,10,11,2,8&cfgs=5844&ver=3&mo_s=0&mo_v=15,20,25,30,40,50,80,100,150,200,400,800,1000,1600,2000,4000&def_sb=6,9,1,7,5&reel_set_size=3&def_sa=4,14,5,2,12&scatters=1~100,10,2,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{mystery:\"15\",max_rnd_sim:\"1\",max_rnd_hr:\"3273322\",jp1:\"2500\",max_rnd_win:\"2650\",jp3:\"50\",jp2:\"250\",jp4:\"20\"}}&wl_i=tbm~2650&sc=5.00,10.00,15.00,20.00,25.00,50.00,75.00,100.00,125.00,190.00,250.00,375.00,625.00,1250.00,1875.00,2500.00&defc=25.00&purInit_e=1&wilds=2~500,200,50,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,100,25,0,0;150,75,10,0,0;100,50,10,0,0;100,50,10,0,0;75,20,5,0,0;75,20,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=5,2,8,14,9,16,10,5,3,8,6,2,15,6,15,14,15,8,3,13,3,10,14,15,12,16,15,15,15,15,15,15,4,5,13,9,10,4,10,14,7,10,4,12,8,1,15,8,7,15,14,12,8,16,1,9,3,3,9,16,16,16,9,13,4,2,5,9,6,4,3,9,3,4,9,14,15,8,11,2,6,15,14,14,5,11,8,16,3,3,3,3,5,15,16,15,5,11,16,3,1,12,1,3,11,7,3,6,16,8,6,8,2,4,9,14,6,7,14~9,9,11,2,2,8,15,16,10,6,15,11,8,5,2,9,9,13,5,8,9,10,14,4,15,1,7,13,16,10,8,7,2,7,10,9,10,16,16,16,9,16,16,8,11,12,13,14,7,14,10,2,13,7,5,6,7,15,2,9,7,7,5,4,3,2,1,12,6,9,9,13,7,8,2,7,15,13,16,7,15,15,15,15,15,15,12,7,8,8,4,16,2,15,15,10,13,4,7,8,3,14,9,3,7,12,5,15,5,7,11,13,5,6,10,7,2,7,10,3,5,15,8,8,13~12,11,9,6,12,11,16,10,12,15,2,15,11,8,5,14,8,15,11,7,6,8,9,16,15,15,5,12,2,12,5,12,13,15,5,3,9,2,13,7,3,8,11,14,2,15,3,15,7,6,16,9,11,8,3,14,15,15,15,15,15,15,15,12,10,13,3,4,12,14,11,14,1,4,6,8,9,5,16,2,3,6,9,15,9,3,10,5,14,15,6,3,3,11,14,8,11,15,15,8,14,9,4,5,7,12,13,12,3,1,16,9,8,16,9,13,10,15,8,8,4~4,4,14,4,16,7,12,3,14,11,8,11,4,12,15,10,16,5,4,13,4,5,2,13,7,9,14,9,5,12,13,14,9,14,2,16,16,16,4,7,15,15,2,13,2,10,15,14,5,2,4,7,5,15,7,16,6,15,11,14,12,13,4,1,11,12,10,7,6,11,13,14,16,7,15,15,15,15,15,15,15,14,10,11,4,4,7,7,13,7,5,3,8,3,15,4,13,16,6,12,3,5,7,16,15,9,6,15,15,7,8,12,15,4,12,15,8,7,1~15,9,2,11,1,12,8,5,7,12,6,12,15,14,7,14,9,6,15,7,15,15,15,15,15,15,15,6,5,12,13,6,5,3,3,13,15,3,15,14,14,11,8,10,16,14,10,10,12,7,3,3,3,3,14,7,8,8,14,3,14,12,3,13,16,15,13,3,6,13,3,11,7,15,3,16,12,16,16,16,10,10,12,4,13,14,8,2,8,15,13,2,2,10,15,10,4,16,12,3,4,13,16,13&accInit=[{id:0,mask:\"prizes\"}]&reel_set2=12,4,5,11,8,7,16,6,14,9,10,3,5,9,16,9,3,9,16,12,16,4,6,5,16,10,6,16,16,16,4,10,13,10,7,13,11,3,16,11,10,14,3,12,4,10,7,13,9,4,8,11,10,3,4,6,5,12&reel_set1=10,8,7,11,5,13,15,2,4,8,2,8,9,6,15,6,13,7,3,4,14,15,13,9,15,10,9,3,12,11,3,13,5,11,9,15,15,15,15,15,15,15,4,12,5,3,9,1,5,6,12,9,15,3,11,12,15,8,5,3,13,7,9,1,8,9,2,15,9,6,15,6,15,5,10,13,3,10,3,3,3,3,4,3,6,8,1,11,8,2,4,7,3,10,11,8,9,3,13,4,3,3,1,8,12,9,8,15,12,4,5,7,10,8,10,6,11,2,14,15~14,10,6,4,6,5,2,15,9,7,2,4,9,14,8,2,2,3,2,14,5,8,7,2,7,2,13,14,15,2,7,7,10,2,8,2,2,9,14,2,15,15,15,15,15,15,15,2,7,15,12,2,15,9,2,8,8,15,2,2,15,2,8,9,8,7,11,15,5,10,2,5,9,5,10,11,2,10,7,2,14,15,6,7,9,9,7,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,10,2,7,2,9,12,8,14,3,11,8,12,2,2,15,2,15,5,13,2,7,15,2,2,7,3,1,7,14,14,5,14,2,14,2,9,2,8,4,2,9,8~8,5,11,3,13,2,2,14,2,11,14,5,13,3,2,15,5,8,8,15,9,8,15,15,9,14,2,12,10,2,13,8,9,3,9,2,6,9,11,2,3,2,12,8,14,2,8,5,14,6,15,15,15,15,15,15,15,3,6,14,5,2,2,10,3,3,5,2,7,6,10,9,12,6,11,2,2,12,2,8,13,2,15,15,5,2,11,7,9,12,2,15,8,8,3,9,2,2,6,2,12,6,2,9,15,11,7,4,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,4,2,8,2,2,4,2,8,2,15,14,9,4,2,11,14,11,9,2,2,11,12,11,2,12,11,15,6,2,14,2,1,15,2,3,10,2,3,5,15,15,14,13,2,8,12,14,7,1,2,2,12~15,4,7,14,2,15,4,4,2,2,5,7,14,4,3,14,4,15,13,13,2,3,10,14,5,13,11,12,11,12,15,15,2,2,9,7,3,13,4,2,10,7,12,2,2,4,4,15,15,15,15,15,15,15,7,4,2,14,11,2,2,15,4,8,11,6,11,2,4,2,2,10,5,2,7,2,5,2,15,2,4,2,2,14,2,15,7,14,13,9,2,8,2,8,5,14,2,13,15,7,3,12,2,6,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,11,12,7,2,4,2,14,7,2,3,7,6,9,1,15,13,2,15,2,9,2,4,2,14,7,12,15,7,1,2,7,6,2,12,4,4,15,12,5,2,7,2,12,7,14,11,13,2,8,11,5~15,3,9,15,7,14,10,12,14,15,14,15,13,8,10,13,6,12,2,11,3,13,6,3,3,8,12,7,3,7,5,10,7,4,15,14,15,11,15,15,15,15,15,15,15,8,10,12,8,2,4,3,14,3,15,4,2,5,15,6,2,7,8,5,10,12,10,1,10,11,10,6,11,3,12,13,4,9,15,8,7,14,7,12,14,3,3,3,3,8,14,6,9,6,12,1,15,14,3,15,13,15,14,10,11,7,2,14,7,8,7,6,15,12,3,10,3,5,13,7,12,9,6,6,13,12,8,6,6&purInit=[{bet:4000,type:\"default\"}]&total_bet_min=5.00";
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
        public CosmicCashGameLogic()
        {
            _gameID = GAMEID.CosmicCash;
            GameName = "CosmicCash";
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
            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

            if (dicParams.ContainsKey("apwa"))
            {
                string strApwa = dicParams["apwa"];
                string[] strParts = strApwa.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);

                dicParams["apwa"] = string.Join(",", strParts);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in CosmicCashGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in CosmicCashGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
