using Akka.Event;
using GITProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class RiseOfSamurai3BetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 20.0f;
            }
        }
    }
    class RiseOfSamurai3GameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40samurai3";
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
                return 40;
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
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=11,5,7,7,5,1,6,9,9,6,12,11,9,9,11,12,11,5,5,11&apvi=10&cfgs=5439&ver=2&reel_set_size=2&def_sb=8,2,6,6,1&def_sa=11,9,5,3,9&scatters=1~0,0,0,0,0~0,0,8,0,0~1,1,1,1,1;14~0,0,0,0,0~0,0,8,0,0~1,1,1,1,1&cls_s=-1&gmb=0,0,0&mbri=1,2,3&rt=d&gameInfo={}&wl_i=tbm~10000&apti=bet_mul&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,100,30,0,0;250,75,25,0,0;150,40,15,0,0;100,25,10,0,0;75,15,7,0,0;50,10,5,0,0;30,6,3,0,0;30,6,3,0,0;20,5,2,0,0;20,5,2,0,0;20,5,2,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=7,11,11,1,12,12,6,8,4,10,10,5,11,11,9,9,3,13,13,5,8,12,12,1,13,13,6,10,10~7,11,11,2,12,12,6,8,4,9,9,5,13,13,3,11,11,5,8,12,12,2,13,13,6,10,10~9,7,11,11,2,13,13,6,8,4,9,9,5,10,10,1,6,8,3,11,11,5,8,12,12,2,13,13,6~7,10,10,2,12,12,6,8,11,11,4,9,9,5,6,7,3,11,11,5,6,12,12,7,13,13,6~7,10,10,1,12,12,6,8,4,9,9,5,6,7,3,11,11,5,6,13,13,7,13,13,6,10,10&reel_set1=10,5,9,9,7,10,10,8,12,12,6,13,13,8,9,9,4,9,9,5,6,8,3,3,3,3,11,11~7,10,10,2,12,12,6,8,4,9,9,5,6,3,11,11,5,6,12,12,7,13,13,2,10,10,7,4~7,10,10,2,12,12,6,8,4,9,9,5,6,3,11,11,5,6,12,12,8,13,13,2,10,10,7,4~7,10,10,2,12,12,6,8,4,9,9,5,6,7,3,11,11,5,6,12,12,7,13,13,6,10,10,7~10,10,6,12,12,8,4,9,9,5,6,7,3,3,3,3,11,11,6,12,12,7,13,13,6,10,10,7&purInit=[{type:\"fs\",bet:2000,fs_count:8}]&mbr=1,1,1&total_bet_min=10.00";
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
        public RiseOfSamurai3GameLogic()
        {
            _gameID = GAMEID.RiseOfSamurai3;
            GameName = "RiseOfSamurai3";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                RiseOfSamurai3BetInfo betInfo = new RiseOfSamurai3BetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in RiseOfSamurai3GameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in RiseOfSamurai3GameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in RiseOfSamurai3GameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            RiseOfSamurai3BetInfo betInfo = new RiseOfSamurai3BetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new RiseOfSamurai3BetInfo();
        }
    }
}
