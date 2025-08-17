using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class DancePartyBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return BetPerLine * 20.0f; }
        }
    }
    public class DancePartyGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243dancingpar";
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
            get
            {
                return 243;
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=5,3,4,3,7,6,2,4,9,2,5,3,4,3,6&cfgs=3660&ver=2&reel_set_size=2&def_sb=6,5,9,7,10&def_sa=10,4,7,10,9&scatters=1~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~0,0,0,0,0,0,0,0,15,15,15,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;150,60,30,0,0;120,60,20,0,0;100,40,16,0,0;80,30,10,0,0;50,10,6,0,0;50,10,6,0,0;40,8,4,0,0;40,8,4,0,0&rtp=94.50&reel_set0=4,4,4,9,9,9,7,7,7,6,4,1,10,10,10,5,5,5,9,1,8,8,8,10,6,5,5,7,9,8,8,3,3,3,9,10,10,6,6,6~6,6,6,5,5,5,8,8,8,2,2,2,9,9,9,4,4,4,7,7,7,8,9,3,3,3,8,10,10,10,7,1,1,4,7,10,6,10,9~1,8,8,8,7,7,7,6,6,6,4,4,4,10,10,10,3,3,3,2,2,2,7,8,9,9,9,3,1,5,2,9,4,5,5,5~8,8,8,10,10,10,10,2,2,2,4,4,4,7,7,7,9,9,9,7,3,3,3,5,5,5,6,6,6,1,9,8,3,9,6,4~5,5,5,9,9,9,10,10,10,7,6,6,6,10,3,3,3,3,1,1,1,1,2,2,2,5,8,8,8,7,7,4,4,4,4,6,3,7,7,7&t=243&reel_set1=6,6,6,9,9,9,1,5,5,5,8,8,8,3,3,3,8,8,1,10,10,10,7,7,7,10,5,4,4,4,7,4,9~4,4,4,3,3,3,1,6,6,6,2,8,8,10,10,10,9,9,9,4,10,1,3,7,7,7,5,5,5,6,4,6,5,8,8,8~7,7,7,3,5,1,1,5,9,9,9,8,8,8,6,8,2,9,8,6,7,3,4,7,10,10,10,10,4~9,9,9,1,4,4,4,10,10,10,3,3,3,6,6,6,7,7,7,5,5,5,10,2,5,7,8,8,8,6,5,8~1,7,7,7,5,5,5,3,3,3,6,6,6,9,9,9,4,4,4,8,8,8,10,10,10,9,3,7,6,6,1,2";
            }
        }
        #endregion
        public DancePartyGameLogic()
        {
            _gameID = GAMEID.DanceParty;
            GameName = "DanceParty";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams.Add("reel_set", "0");
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if(dicParams.ContainsKey("apwa"))
            {
                string[] strParts = dicParams["apwa"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);
                dicParams["apwa"] = string.Join(",", strParts);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                DancePartyBetInfo betInfo = new DancePartyBetInfo();
                betInfo.BetPerLine        = (float)message.Pop();
                betInfo.LineCount         = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DancePartyGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DancePartyGameLogic::readBetInfoFromMessage {0}", ex);
            }

        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            DancePartyBetInfo betInfo = new DancePartyBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
