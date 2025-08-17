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
    class DwarfAndDragonGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysspltsym";
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
                return "def_s=4,8,11,15,14,13,7,7,8,3,5,21,8,4,17,15,9,3,9,20&cfgs=11873&ver=3&def_sb=8,19,3,10,15&reel_set_size=11&def_sa=3,13,17,4,17&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase:\"94.49\",regular:\"94.53\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"175438596\",max_rnd_win:\"14000\"}}&wl_i=tbm~14000&reel_set10=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1;3~0,0,0,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,100,30,0,0;500,100,30,0,0;200,75,25,0,0;200,75,25,0,0;150,50,20,0,0;150,50,20,0,0;100,40,15,0,0;100,40,15,0,0;80,30,12,0,0;80,30,12,0,0;60,25,10,0,0;60,25,10,0,0;50,20,5,0,0;50,20,5,0,0;30,15,3,0,0;30,15,3,0,0;25,10,2,0,0;25,10,2,0,0&total_bet_max=10,000,000.00&reel_set0=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&reel_set2=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&reel_set1=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&reel_set4=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&purInit=[{bet:2000,type:\"default\"}]&reel_set3=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&reel_set6=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&reel_set5=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&reel_set8=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&reel_set7=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&reel_set9=11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21~11,12,13,14,15,16,17,18,19,1,2,3,4,5,6,7,8,9,20,10,21&total_bet_min=10.00";
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
        public DwarfAndDragonGameLogic()
        {
            _gameID = GAMEID.DwarfAndDragon;
            GameName = "DwarfAndDragon";
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DwarfAndDragonGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in DwarfAndDragonGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
