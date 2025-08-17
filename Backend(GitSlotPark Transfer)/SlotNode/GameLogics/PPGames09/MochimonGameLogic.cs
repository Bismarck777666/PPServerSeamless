using Akka.Event;
using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MochimonGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20mochimon";
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
            get
            {
                return 20;
            }
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
                return "def_s=5,6,5,3,9,6,9,8,7,9,5,3,6,6,8,4,5,6,7,8,9,5,6,5,3,9,6,9,8,7,9,5,3,6,6,8,4,5,6,7,8,9,7,3,9,6,9,3,5&cfgs=6964&ver=3&def_sb=6,8,1,2,7,1,3&reel_set_size=1&def_sa=8,6,5,7,8,9,1&scatters=1~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2340000\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0~1&bonuses=0&paytable=0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0;3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,1400,700,300,150,100,50,40,35,30,20,0,0,0,0;2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,1200,600,250,120,80,40,30,25,20,15,0,0,0,0;1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,800,400,200,90,60,30,25,20,15,10,0,0,0,0;800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,400,200,100,60,40,25,20,15,10,8,0,0,0,0;600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,300,160,70,50,30,20,15,10,8,6,0,0,0,0;500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,240,120,60,40,25,15,10,8,6,5,0,0,0,0;400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,200,100,50,30,20,10,8,6,5,4,0,0,0,0;0;0&total_bet_max=10,000,000.00&reel_set0=9,9,6,6,5,9,7,7,7,5,8,8,8,7,9,5,5,6,4,9,9,7,8,8,1,3,3,4,3,3,3,9,9,9,6,6,7,9,9,5,5,4,5,5,9,7,8,5,5,5,5,6,4,9,9,4,8,8,6,9,9,1,7,6,6,8,8,9,9,9,9,5,8,4,8,8,3,8,8,6,6,8,8,8,5,5,6,4~4,4,7,7,8,8,8,7,8,8,9,9,5,5,8,8,1,5,5,3,9,9,9,7,3,3,4,4,7,7,9,9,9,8,8,7,8,4,5,3,3,5,5,4,7,7,5,3,7,8,8,8,8,5,5,8,8,1,9,7,9,9,7,8,8,7,3,3,8,8,8,7,8,8,8,8,5,9,9,9,6,8,8,8,6,3,3,6,7,7,4,4,4,4,7~9,9,9,7,7,7,7,4,5,5,7,7,5,5,5,9,9,4,4,6,6,4,4,8,8,1,5,9,5,3,8,7,7,6,6,8,9,9,9,9,6,3,3,3,9,8,8,6,6,8,9,9,8,3,7,7,9,1,7,7,7,8,9,4,7,7,9,5,5,9,8,8,5,6,9,7,7,3,3,9,9,1,1,6,6,4,4,4,9,6,6~7,7,3,3,6,6,6,5,5,5,6,7,3,3,7,4,4,5,5,5,7,7,1,3,3,4,4,4,4,5,8,8,6,6,8,8,8,8,1,7,8,7,7,6,6,7,7,9,9,7,7,4,4,6,6,8,3,4,4,9,9,9,9,9,4,4,7,4,9,6,6,9,9,7,7,7,5,5,5,9,9,9,8,6,7,7,8,7,5,9,9,6,6,5,5~4,6,6,8,3,3,6,9,7,4,5,5,3,3,7,1,9,4,4,7,7,9,4,4,6,4,5,3,8,6,8,8,8,4,4,7,3,3,3,4,4,8,8,3,4,4,8,8,6,6,6,8,8,7,7,5,5,1,7,7,7,6,4,4,4,8,8,4,3,3,9,9,8,8,9,9,9,5,5,3,3,3,9,3,7,7,3,4,4~5,6,6,4,4,6,6,6,9,6,6,6,7,9,9,8,8,9,8,8,7,7,3,3,8,8,8,9,9,9,9,9,8,8,1,9,4,4,6,6,6,7,5,3,3,7,7,5,5,4,4,8,8,7,7,8,8,7,7,5,8,8,8,8,6,6,9,9,9,9,7,7,9,9,5,5,5,1,6,8,8,5,5,5,5,3,3,9,9,3,8,8,4,4,5,3,3,5,7,7~8,8,6,6,6,7,4,4,4,4,9,9,9,9,5,5,5,1,7,4,6,9,9,4,9,9,7,7,4,4,3,3,9,9,7,7,9,7,7,3,9,8,8,9,9,9,6,6,7,7,6,4,4,6,6,4,9,9,9,9,9,9,1,7,4,4,5,5,8,8,6,6,8,8,5,5,4,4,8,8,8,8,8,8,8,8,9,9,4,4,7,4,4&purInit=[{bet:2000,type:\"default\"}]&total_bet_min=10.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
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
        public MochimonGameLogic()
        {
            _gameID = GAMEID.Mochimon;
            GameName = "Mochimon";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "7";

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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MochimonGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MochimonGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
    }
}
