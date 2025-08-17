using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class HotFiestaGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25hotfiesta";
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
                return 25;
            }
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
                return "def_s=2,13,8,3,8,1,11,4,6,10,5,13,13,4,9&cfgs=5320&ver=2&def_sb=2,9,8,5,11&reel_set_size=4&def_sa=6,13,10,6,13&scatters=1~0,0,3,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"1652826\",max_rnd_win:\"5000\",max_rnd_win_a:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,40.00,60.00,80.00,100.00,200.00,400.00,800.00,1000.00,2000.00,3000.00,4000.00&defc=100.00&purInit_e=1&wilds=2~1000,250,50,0,0~1,1,1,1,1,1;14~1000,250,50,0,0~1,1,1,1,1,1;15~1000,250,50,0,0~1,1,1,1,1,1;16~1000,250,50,0,0~1,1,1,1,1,1&bonuses=0&fsbonus=&bls=25,35&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;750,150,50,0,0;500,100,35,0,0;300,60,25,0,0;200,40,20,0,0;150,25,12,0,0;100,20,8,0,0;50,10,5,0,0;50,10,5,0,0;25,5,2,0,0;25,5,2,0,0;25,5,2,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=12,7,13,8,11,10,8,11,3,9,7,13,10,6,1,10,12,9,8,4,12,8,7,13,11,5,9,13,10,5,12,13,6,12,6,11,6,10,9,8,13,12,1,7,9,2,1,6,5,12,3,11,13,11,4,7,9,7,10~7,13,10,13,5,10,11,2,10,7,12,6,13,11,8,9,11,3,11,7,12,11,7,5,6,5,8,10,8,10,13,11,4,9,4,3,4,13,10,2,10,12,13,8,9,8,9,12,9,7,9,12,10,5,4,9,8,6,9,7,8,12,11,13~11,10,9,7,12,9,4,11,7,13,12,1,13,8,6,12,6,5,13,9,5,11,7,11,6,11,1,13,1,5,12,3,6,8,13,10,9,13,7,13,6,10,6,11,1,3,4,1,2,10,12,8,10,2,8,9,12,8,10,4,8,5,7,12,9,7,3,8,2,11~13,10,3,12,7,8,12,13,2,9,8,5,13,6,10,4,11,7,6,4,5,8,10,5,11,6,13,12,3,12,10,9,6,10,12,7,11,9,11,10,9,11,13,6~7,9,13,6,11,1,7,11,8,5,12,9,10,13,8,12,9,13,12,5,3,11,4,11,13,12,7,5,3,9,6,12,7,8,10,6,10,8,9,6,11,6,7,11,7,8,4,5,12,11,2,9,10,12,8,10,12,8,7,13,9,8,5,13,5,11,6,4,1,6,9,13,3,7,1,11,5,10,1&reel_set2=8,9,3,8,11,13,11,13,13,9,11,5,9,5,13,3,13,13,9,6,13,6~4,12,10,12,10,12,7,10,7,12,7,4,10,7,10,7,12,7,12,10~9,5,13,11,8,3,11,5,6,13,9,5,8,9,13,9,13,3,13,6,13,11,13,9,13,11~12,10,7,12,7,4,7,10,12,10,7,10,7,12,10,7,10,12,10,12,7,12,4~11,9,13,13,11,9,6,13,3,5,13,11,9,8,13,8,13,13,9,6,3,5,9,11,13,5,11,3,13&reel_set1=12,3,4,11,8,10,13,10,5,9,12,13,6,8,12,6,13,5,11,8,7,3,12,8,9,6,5,10,3,3,3,11,9,8,13,10,13,10,4,10,12,9,7,8,7,12,8,12,9,4,5,12,13,11,6,11,4,11,7~12,7,12,10,9,6,9,3,7,11,10,6,10,11,13,12,6,8,11,6,4,8,11,7,3,4,12,5,7,13,7,4,9,11,4,10,11,6,8,11,12,13,5,10,8,5,10,8,5,3,11,13,9,13,7,13,9,6,13,8,12,7,9,10~12,6,13,10,7,9,10,9,5,11,6,12,10,7,10,7,5,10,8,13,6,10,13,6,4,11,12,8,12,5,13,8,11,8,13,7,8,6,7,6,11,9,3,10,8,3,5,9,4,11,12,7,3,5,11,4,7,4,13,6,13,12,8,13,11,13~10,5,12,4,12,6,10,11,13,12,8,9,8,12,7,4,5,6,11,9,11,3,4,13,9,10,6,10,5,7,6,7,13,9,8,9,3,11,8,6,5,10,13,12,7,11,7,8~11,9,10,9,8,13,3,8,12,11,10,13,7,11,13,4,10,9,8,7,3,3,3,12,5,6,9,8,7,10,3,8,9,4,6,13,11,7,5,12,6,11,12,6&purInit=[{type:\"fsbl\",bet:2500,bet_level:0}]&reel_set3=13,7,6,10,9,6,13,11,7,10,9,11,8,13,8,11,12,13,11,7,11,13,12,5,2,9,8,13,1,9,13,1,12,6,13,6,12,4,8,5,10,9,3,7,3,10,9,1,6,12,4,7,10,11,8,5,10,8,9,4,10,1,11,12,1,12~4,13,12,10,11,10,13,3,6,9,8,2,11,8,12,9,11,9,4,13,3,5,9,12,4,13,3,6,12,8,9,10,8,11,5,9,7,13,11,7,12,13,7,8,13,5,10,9,2,11,10,4,6,2,12,5,10,13,12,13,11~10,5,6,9,7,10,1,2,11,6,7,4,8,12,3,1,12,9,6,13,7,12,13,12,7,9,8,9,13,10,1,9,6,10,7,2,10,8,10,8,11,1,4,11,5,9,13,6,2,8,13,11,13,11,8,12,5,11,12,6,10,5,4,13,7,10,1,3,12,11,9,6,7,11,3,4~9,11,9,8,10,12,6,9,11,10,9,2,12,7,11,6,8,5,10,8,13,5,13,12,7,13,4,10,4,9,11,12,13,11,5,3,12,13,7,6,10,11,12,13,10,3,9~10,11,9,13,11,9,7,1,9,3,12,5,9,10,6,13,1,8,4,7,10,5,7,9,13,8,10,5,8,1,11,10,6,4,6,1,12,4,9,4,9,5,1,13,11,13,12,13,3,12,10,7,2,8,10,12,5,3,7,12,11,6&total_bet_min=8.00";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true;}
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }
        protected override bool SupportMoreBet
        {
            get { return true;}
        }
        protected override double MoreBetMultiple
        {
            get { return 1.4; }
        }
        #endregion

        public HotFiestaGameLogic()
        {
            _gameID = GAMEID.HotFiesta;
            GameName = "HotFiesta";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"]   = "0";
            dicParams["bl"]         = "0";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in HotFiestaGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in HotFiestaGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                    //만일 유저에게 남은 응답이 존재하는 경우
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
                _logger.Error("Exception has been occurred in HotFiestaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

    }
}
