using Akka.Event;
using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DragonTigerBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 20.0f; }
        }
    }
    class DragonTigerGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1024dtiger";
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
            get { return 1024; }
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
                return "def_s=9,12,13,9,9,4,5,7,4,7,11,8,13,10,13,13,5,3,4,7&cfgs=3543&ver=2&def_sb=10,9,3,13,5&reel_set_size=2&def_sa=10,8,6,5,5&scatters=1~20,10,2,0,0~20,15,8,0,0~1,1,1,1,1,1&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;150,50,25,5,0;75,50,25,0,0;75,50,25,0,0;60,40,10,0,0;60,40,10,0,0;50,25,5,0,0;50,25,5,0,0;50,10,3,0,0;50,10,3,0,0;50,5,2,0,0;50,5,2,1,0&reel_set0=10,3,3,3,9,11,3,3,10,3,3,12,10,9,6,11,13,10,4,9,11,5,10,7,10,11,10,9,11,10,9,10,9,6,10,9,11,1,8,10,9,4,11,12,10,7~6,12,5,8,7,13,5,12,2,10,5,12,6,8,5,12,5,11,12,2,4,8,12,5,3,3,3,8,5,9,12,5,13,12,3,3,5,9,5,12,11,5,12,7,8,5,9,1,12,5,11,5~5,13,6,11,7,13,2,8,5,12,6,13,7,13,3,3,11,6,12,7,3,3,3,8,5,12,4,1,3,13,11,6,13,7,8,13,12,2,8,4,13,5,9,13,8,5,13,11,5,8,12,9,6,13,8,1,9,7,13,4,10,5,13,6,8,7,10,7~7,12,6,9,3,3,3,10,3,3,8,3,3,3,10,11,9,5,10,4,11,7,9,6,3,10,2,10,6,9,1,5,10,4,6,10,6,4,13,7,9,7,6,10,7,8,6,11,7,9,4,2,10,13~5,9,7,13,7,5,12,7,9,4,9,4,5,11,7,12,1,10,7,9,4,3,3,3,7,12,5,8,4,10,3,7,9,4,13,5,4,8,6,10,7,9,4,9,4,10,7,9,4,10,7,9,1,10,7,9,4,13&t=243&reel_set1=10,3,3,3,9,11,3,3,10,3,12,10,9,10,9,5,11,10,13,9,10,9,6,10,9,11,1,8,10,9,4,11,12,10,7~6,12,5,8,7,13,5,4,12,2,10,5,12,6,8,9,3,3,12,4,5,13,12,5,9,3,3,3,5,12,11,5,12,7,8,5,9,1,12,5,11,5~5,13,6,11,7,13,3,3,3,4,8,5,12,6,13,7,13,11,6,3,13,7,8,13,12,2,8,13,5,9,8,5,3,3,13,11,5,8,12,2,9,6,13,8,1,9,7,13,4,10,5,13,6,8,7,10,7~7,12,6,9,3,3,3,10,3,3,8,3,3,3,2,10,6,9,1,5,10,3,4,6,10,6,4,13,7,9,7,6,10,7,8,6,11,7,9,4,2,10,13~5,9,7,13,7,5,12,7,9,4,9,4,5,11,4,10,3,7,9,4,13,5,4,8,6,10,7,9,4,9,3,3,3,4,10,7,9,4,10,7,9,1,10,7,9,4,13";
            }
        }
	
	
        #endregion
        public DragonTigerGameLogic()
        {
            _gameID = GAMEID.DragonTiger;
            GameName = "DragonTiger";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {

            DragonTigerBetInfo betInfo = new DragonTigerBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new DragonTigerBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                DragonTigerBetInfo betInfo = new DragonTigerBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DragonTigerGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DragonTigerGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
