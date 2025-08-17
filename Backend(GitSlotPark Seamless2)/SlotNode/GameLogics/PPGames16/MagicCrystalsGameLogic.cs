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
    class MagicCrystalsBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 30.0f; }
        }
    }
    class MagicCrystalsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243crystalcave";
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
            get { return 243; }
        }
        protected override int ServerResLineCount
        {
            get { return 30; }
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
                return "def_s=3,5,4,8,1,10,6,10,5,7,8,9,6,9,8&cfgs=2129&reel1=8,9,3,10,9,10,5,9,2,6,1,4,3,7,8&ver=2&reel0=9,3,7,1,2,5,6,9,8,8,4,5,7,3,10,7,10,4&def_sb=10,9,8,4,7&def_sa=9,8,9,7,3&reel3=5,10,6,8,5,8,8,2,7,9,3,9,4,1,7,10&reel2=7,9,3,10,5,9,5,4,1,8,6,8,6,2&reel4=6,10,3,9,8,5,6,7,4,2,9,8,8,5,3,7,1,4,7&scatters=1~0,0,0,0,0~25,20,15,0,0~3,3,3,1,1&gmb=0,0,0&rt=d&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&wilds=2~750,200,50,0,0~2,2,2,2,2&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,75,30,0,0;200,60,20,0,0;150,50,15,0,0;100,30,10,0,0;30,15,5,0,0;30,15,5,0,0;20,10,3,0,0;20,10,3,0,0&rtp=96.36&t=243";
            }
        }
	
	
        #endregion
        public MagicCrystalsGameLogic()
        {
            _gameID = GAMEID.MagicCrystals;
            GameName = "MagicCrystals";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            MagicCrystalsBetInfo betInfo = new MagicCrystalsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                MagicCrystalsBetInfo betInfo    = new MagicCrystalsBetInfo();
                betInfo.BetPerLine              = (float)message.Pop();
                betInfo.LineCount               = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MagicCrystalsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in MagicCrystalsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
