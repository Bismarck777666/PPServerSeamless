using Akka.Event;
using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BiggerBassBlizzardGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs12bbbxmas";
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
                return 12;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 12; }
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
                return "def_s=5,10,10,5,11,3,8,3,6,9,4,12,11,4,11,5,9,3,6,8&cfgs=6730&ver=3&mo_s=7&mo_v=24,60,120,180,240,300,600,48000&def_sb=6,8,6,4,8&reel_set_size=6&def_sa=5,9,4,5,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"963113\",max_rnd_win:\"4000\",max_rnd_win_a:\"2667\",max_rnd_hr_a:\"560601\"}}&wl_i=tbm~4000;tbm_a~2667&sc=15.00,30.00,45.00,60.00,75.00,150.00,225.00,300.00,375.00,625.00,875.00,1250.00,2000.00,4000.00,6250.00,9000.00&defc=75.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=12,18&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2400,240,60,6,0;1200,180,36,0,0;600,120,24,0,0;600,120,24,0,0;0,60,12,0,0;120,30,6,0,0;120,30,6,0,0;120,30,6,0,0;120,30,6,0,0;120,30,6,0,0&total_bet_max=10,800,000.00&reel_set0=12,7,6,8,7,11,6,8,10,7,11,4,11,7,9,5,9,12,8,9,8,7,3,11,7,7,7,7,7,9,12,6,7,1,8,7,9,7,7,9,7,1,5,6,11,4,12,11,6,10,7,4,10,9,5,9,5~6,11,12,9,8,4,10,11,8,5,7,7,10,7,4,7,7,6,5,10,12,9,7,7,7,7,7,8,10,7,7,10,12,4,12,8,1,9,11,5,8,3,12,6,10,3,5,7,12,9,12,7,6~7,7,4,9,7,12,9,11,7,8,6,11,8,4,10,7,9,7,10,7,5,12,3,11,5,7,7,7,7,10,5,3,10,12,7,6,4,6,9,7,1,11,7,10,8,6,8,5,3,10,4,5,12,3,8,11,9,6~9,6,12,5,6,5,10,6,5,10,12,11,8,7,8,5,7,7,7,7,4,9,7,3,10,7,4,7,3,6,8,7,11,1,9,7,4,11,12,9~6,3,7,12,6,8,1,4,8,7,11,7,7,5,12,5,7,10,9,4,8,1,6,9,6,7,7,7,7,12,11,5,8,7,9,7,12,10,6,3,7,4,10,9,8,11,4,10,9,7,5,9,6,11,7,5&accInit=[{id:0,mask:\"cp;lvl;tp\"}]&reel_set2=4,6,7,6,3,10,7,6,12,6,10,7,9,6,7,4,7,7,7,7,10,12,3,4,12,6,9,6,12,7,7,4,7,6,10,12,3,7,10,7,12~7,8,7,7,4,11,7,7,11,1,8,7,7,7,7,7,11,4,5,10,1,10,5,1,4,11,1,5,7,8~1,3,8,6,1,8,9,5,3,9,5,12,5,6,1,11,6,11,8,6,11,1,12,11,6,12,9,3,9,12,8,3,1,6,5~10,6,5,8,5,12,7,5,6,10,7,9,12,11,4,6,7,11,6,9,12,8,4,6,7,7,7,7,9,7,9,5,7,12,6,12,7,7,10,4,11,10,7,4,6,3,8,3,8,6,7~10,11,8,10,6,7,6,10,5,10,11,7,12,10,9,4,12,8,3,9,6,12,7,7,7,7,4,6,11,8,12,7,3,4,5,7,7,11,9,6,9,6,7,5,12,7,5,8,6,7,4&reel_set1=4,5,7,11,10,1,4,11,1,7,8,1,11,7,7,7,7,7,10,4,7,5,7,7,8,1,8,11,8,7,5,7,7~12,7,4,12,6,10,3,10,7,10,7,7,3,6,3,7,10,6,12,7,7,7,7,4,6,10,6,10,6,7,12,7,6,7,4,10,9,6,7,7,9,12,4,12,7~12,3,11,9,5,6,9,8,5,8,9,3,6,11,6,9,5,8,12,6,9,11,3,8,12~9,5,3,9,7,5,11,1,6,1,7,8,11,8,4,6,8,1,7,7,12,7,7,7,7,1,12,4,9,10,7,8,6,9,4,1,3,6,11,6,7,10,7,8,12,6,9,10,7,1~9,8,5,6,5,7,8,10,6,7,3,7,10,8,4,7,5,9,6,10,9,12,7,9,8,7,7,7,7,12,4,11,12,6,4,10,7,6,7,10,7,11,12,6,11,6,7,4,6,11,7,9,11,3,5,9,12&reel_set4=5,11,8,7,8,7,8,7,7,5,4,7,11,7,7,7,7,7,11,8,11,7,7,5,4,7,10,4,11,8,7,7,10,4,5~4,10,12,7,12,7,10,4,6,7,4,6,7,6,7,7,7,7,6,7,7,4,9,6,3,10,7,9,10,7,6,12,6,3~5,12,3,9,5,6,11,9,3,12,8,9,8,11,5,12,8,3,6,9,11,6,3,6,9~7,7,12,11,12,10,7,6,4,9,7,5,9,7,7,7,7,9,10,6,8,7,7,8,4,9,11,8,6,3,6,4,5,8~9,7,8,9,8,11,4,8,6,7,7,12,6,5,10,3,12,7,7,7,7,10,8,12,10,5,7,7,4,10,7,6,7,12,5,11,6,11,4,6,9,6&purInit=[{bet:1200,type:\"default\"}]&reel_set3=5,8,12,6,12,8,9,10,4,10,11,7,9,12,7,7,7,10,12,8,10,12,7,11,6,8,12,3,10,4,8~9,4,6,5,7,11,8,7,11,10,12,9,11,3,12,8,7,5,9,3,10,9,11~9,5,12,4,10,4,9,5,4,6,10,5,8,3,5,8,6,7,4,7,7,11,6,7,11,12,6~5,3,4,6,7,7,9,8,3,12,10,5,6,5,11,7,4~4,9,6,5,7,7,12,8,9,10,7,8,7,6,4,5,10,12,11,3,11&reel_set5=8,5,3,4,12,7,5,6,9,10,7,4,7,9,11,7,7,7,7,7,6,9,7,7,9,8,11,8,6,10,11,5,7,12,8,9,6,11~7,5,12,5,10,6,12,10,11,8,6,7,8,7,7,7,7,7,6,7,11,8,12,9,3,10,6,10,6,4,3,12,7,7,9,4,12,5,7~9,6,10,9,7,7,12,3,5,11,10,5,11,12,8,5,7,7,7,7,3,6,3,4,9,11,8,11,4,10,8,6,7,8,9,10,7,12,5,4,7~5,9,7,7,8,5,7,12,8,12,4,11,9,8,7,12,3,6,7,7,7,7,11,9,10,5,11,4,10,4,3,5,6,7,9,10,7,9,6,7,6,8~11,3,5,7,8,9,12,9,10,4,11,7,12,5,6,7,7,7,7,4,9,7,6,12,6,3,8,4,6,11,7,10,5,9,10,7,8&total_bet_min=15.00";
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
        protected override double MoreBetMultiple
        {
            get { return 1.5; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        #endregion
        public BiggerBassBlizzardGameLogic()
        {
            _gameID = GAMEID.BiggerBassBlizzard;
            GameName = "BiggerBassBlizzard";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "5";
            dicParams["bl"] = "0";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BiggerBassBlizzardGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in BiggerBassBlizzardGameLogic::readBetInfoFromMessage", strUserID);
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
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BiggerBassBlizzardGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
        }
    }
}
