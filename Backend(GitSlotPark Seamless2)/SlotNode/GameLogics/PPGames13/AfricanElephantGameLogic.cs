using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class AfricanElephantGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20hotzone";
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
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=10,8,7,4,4,7,7,10,6,9,11,11,9,11,10,10,4,4,7,8&cfgs=8152&ver=3&def_sb=9,10,7,10,5&reel_set_size=4&def_sa=9,11,9,11,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"12300123\",max_rnd_win:\"15000\"}}&wl_i=tbm~15000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~1500,250,50,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;600,100,40,0,0;250,50,25,0,0;150,40,25,0,0;100,25,15,0,0;100,25,10,0,0;80,20,5,0,0;80,20,5,0,0;80,15,5,0,0;80,15,5,0,0&total_bet_max=10,000,000.00&reel_set0=9,11,6,10,11,8,9,10,7,4,8,6,10,9,11,1,10,5,9,8,11,10,4,3,10,3,9,8,11,7,11,8,10,8,5,1,11,9,8,10,5~6,11,8,5,1,9,5,1,11,3,8,6,4,7,5,9,6,3,10,9,4,6,9,8,7,8,6,11,7,4,5,11,8,10,11,3,7,5,3,11,5,9,8,7,4,8,6,8,7,4,6,11,10,8,7,9~10,6,5,11,7,6,8,1,10,9,10,9,3,10,8,4,7,9,6,9,5,10,8,3,7,10,4,7,4,10,11,3,8,7,5,8,5~10,4,10,5,7,8,3,8,6,8,3,11,5,6,11,1,3,9,7,6,8,6,9,4,8,7,6,7,11,4,10,8,9,5,10,1,4,10,7,10,9,5,11,9,11~3,5,7,6,7,10,9,5,9,11,9,7,10,7,4,10,4,10,4,8,6,11,8,7,3,8,3,4,5,10,4,8,9,11,9,6,8,6,5,7,8,6,5,3,4,1,8,7,10,11,9,11&reel_set2=11,3,11,4,8,10,6,11,1,9,1,4,1,10,11,9,10,1,11,8,1,8,9,8,1,10,11,8,1,6,10,1,7,8,9,5,7,9,3,11,1,9~1,11,6,11,5,11,3,7,10,1,9,1,8,3,8,6,10,3,1,5,8,6,1,9,1,5,1,11,7,8,1,9,4,1,6,4,8,4~9,10,1,6,10,9,5,3,4,8,1,7,1,3,4,1,9,11,10,8,1,9,10,8,4,10,5,7,1,6,8,7,6,1,6,9,11,7,1,10,8,1,8,3,5,1,7,8,5~1,6,1,10,1,3,11,10,1,7,6,9,11,8,10,7,8,4,1,9,10,8,1,6,1,4,11,6,1,10,5,7,11,9,5,3,9,1,11,10,11,7~5,1,10,11,9,1,11,8,3,6,8,4,5,4,1,8,6,7,9,4,1,10,1,3,9,7,6,5,1,4,1&reel_set1=4,3,7,5,3,5,8,6,3,10,7,11,6,3,8,3,10,5,9,3,3,3,3,4,3,9,3,10,3,10,3,1,9,3,11,3,3,11,3,11,3,8,3,9~1,3,5,3,7,3,3,9,7,10,3,3,5,1,6,3,8,3,3,9,3,3,3,3,8,6,7,11,4,3,10,11,3,3,6,8,6,3,4,3,3,9,7,3,3,10~4,3,3,8,4,10,11,3,6,3,11,3,10,3,3,8,3,3,6,7,8,3,3,8,3,3,5,3,5,3,3,3,3,3,3,1,3,5,4,8,3,9,3,3,5,3,10,7,3,6,9,7,3,3,4,10,4,8,3,3,7,6,10,5,3,11,9~5,4,9,3,3,5,4,3,3,8,3,9,8,3,3,8,3,5,11,3,3,3,3,1,9,3,3,6,3,3,1,10,3,10,3,6,3,10,4,7,6,7,11~9,3,3,7,3,4,3,3,9,3,7,6,5,7,3,7,8,4,3,8,10,3,5,7,3,3,3,3,8,3,5,10,3,3,8,11,6,1,10,3,3,4,3,3,1,6,4,6,3,5,9,3,11&purInit=[{bet:2000,type:\"default\"}]&reel_set3=5,10,9,3,10,3,5,8,10,11,9,8,11,9,11,8,9,10,9,10,8,4,10,11,8,4,8,9,8,9,10,11,5,6,8,9,11,6,10,3,10,7,6,9,11,6,11,10,9,7,5,11,7,9,10,9~9,6,9,8,10,8,6,11,10,6,5,4,5,11,9,4,8,3,5,7,6,5,8,11,8,11,7,6,10,6,3,7,9,8,7,4,7,3,7,9,11,8~5,4,3,10,8,4,10,9,10,3,11,8,5,3,11,7,10,9,8,5,7,11,10,7,6,4,8,6,5,8,4,8,10,9,10,7,9,7,8,7,5,10,4,7,9,5,10,6,4,6,9,8,6,3,7,9,11~9,11,8,6,8,4,11,5,6,10,11,5,6,9,7,4,10,11,8,10,4,3,7,3,10,5,10,3,9,3,11,5,8,10,6,11,5,6,11,7,10,7,4,9,8,11,10,9,4,8,9~7,5,4,7,3,9,11,7,9,10,11,3,8,6,10,4,7,8,7,11,8,5,10,6,11,10,6,4,8,6,3,9,4,5,8&total_bet_min=10.00";
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
        public AfricanElephantGameLogic()
        {
            _gameID = GAMEID.AfricanElephant;
            GameName = "AfricanElephant";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "2";
	        dicParams["g"] = "{purchase_s:{def_s:\"5,7,3,6,7,4,8,8,4,9,5,10,6,3,7,4,11,1,4,9\",def_sa:\"5,11,8,4,10\",def_sb:\"5,9,7,3,7\",s:\"5,7,3,6,7,4,8,8,4,9,5,10,6,3,7,4,11,1,4,9\",sa:\"5,11,8,4,10\",sb:\"5,9,7,3,7\",sh:\"4\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in AfricanElephantGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in AfricanElephantGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
