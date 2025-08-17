using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Event;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class SpartaKingBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * LineCount / 2.0f;
            }
        }
    }
    public class SpartaKingGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40spartaking";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return false;
            }
        }
        protected override int ClientReqLineCount
        {
            get {  return 40; }
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
                return "def_s=7,7,8,8,7,5,5,3,3,9,7,7,8,8,7,7,7,9,9,3&cfgs=4337&ver=2&def_sb=5,8,8,8,8&reel_set_size=2&def_sa=8,9,6,10,10&scatters=1~20,5,1,0,0~12,10,8,0,0~1,1,1,1,1,1&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~200,100,20,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;150,75,20,0,0;125,60,20,0,0;100,50,10,0,0;80,40,10,0,0;50,20,5,0,0;40,10,3,0,0;40,10,3,0,0;40,10,2,0,0;40,10,2,0,0&rtp=94.56&reel_set0=11,6,8,9,9,9,9,3,10,6,6,6,4,7,7,7,7,1,8,8,8,5,11,11,11,4,5~7,10,10,10,2,3,4,9,9,9,1,5,5,5,5,6,6,6,11,6,10,8,9,5,9,5,9,6,5,9,11,9,6,10,5,11,10,6,5,4,5,9,6,9,5,8,6,9,10,4,9,5,6,9~3,10,8,8,8,2,10,10,10,5,6,4,6,6,6,8,11,7,11,11,11,9,1,11,10,6,10,2,1,8,9,11,9,8,11,10,11,9,11,10,2,8,1,11,6,11,4,7,10,2,10,11,10,8,1,11,9,6,7,8,10,9,10,7,1~3,9,7,4,6,4,4,4,10,5,11,1,8,2,4,10,9,4,6,2,1,9,11,4,10,11,4,7,4,5,4~11,11,11,11,3,6,6,6,8,4,6,9,9,9,5,10,10,10,10,9,7,2,1,10,1,10,6,5,6,5,6,9,5,9,6,5,9,2,9,6,10,6,5,6,3,6,10,3,5,6,8,10,6,3,9,5,10,6,3,9,6,10,8,9,3,2,5,6,10,8,6,2,6,9,6,9,6,10,9,1,9,1,5,9,5&reel_set1=11,11,11,4,9,1,5,8,3,7,10,11,6,8,3,9,8,10,1,3,4,8,1,8,7,1,4,3,1,10,3,1,5,3,1,3,10,9,3,1,3,1,3,7,9,1,3,8,1,3,1,4,1,7,9,10,3,1,10,5,8,10,7,10,7,9,3,8,1,10,3,1,10,8~9,8,5,5,5,10,11,3,10,10,10,5,6,6,6,1,4,7,6,10,6,5,11,6~11,11,11,4,10,11,7,5,8,3,9,6,1,8,6,5,10,6,3,1,8,3,5,8,5,3,7,8,5~8,9,4,4,4,5,1,7,11,6,3,4,10,1,3,6,7,4,1,5,11,1,4,3,7,3,1,3,10,3,4,11,7~5,1,11,11,11,6,10,6,6,6,9,7,11,8,3,4,10,6,11,6,11,9,11,6,11,3,6,8,11,6,11,6,8,6,11,6,11,4,6,4,7,11,6,11,4,11,6,3,11,6,11,6";
            }
        }        
        #endregion

        public SpartaKingGameLogic()
        {
            _gameID = GAMEID.SpartaKing;
            GameName = "SpartaKing";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                SpartaKingBetInfo betInfo = new SpartaKingBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in SpartaKingGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpartaKingGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams.Add("reel_set", "0");
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            SpartaKingBetInfo betInfo = new SpartaKingBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new SpartaKingBetInfo();
        }

    }
}
