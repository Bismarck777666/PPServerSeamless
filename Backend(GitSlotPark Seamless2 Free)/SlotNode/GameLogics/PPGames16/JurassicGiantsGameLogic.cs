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
    class JurassicGiantsBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 50.0f; }
        }
    }

    class JurassicGiantsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs4096jurassic";
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
            get { return 4096; }
        }
        protected override int ServerResLineCount
        {
            get { return 50; }
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
                return "def_s=3,5,4,8,1,10,6,10,5,7,8,9,6,9,8,3,5,4,8,1,10,6,10,5&cfgs=1930&ver=2&reel_set_size=2&def_sb=13,6,1,1,1,1&def_sa=12,13,7,8,12,10&scatters=1~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~250,100,80,50,45,40,35,30,25,20,15,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&fs_aw=m~2;m~3;m~4&gmb=0,0,0&rt=d&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;400,200,60,15,0,0;400,200,60,15,0,0;250,120,50,0,0,0;250,120,50,0,0,0;200,80,40,0,0,0;200,80,40,0,0,0;150,70,30,0,0,0;100,60,25,0,0,0;100,60,25,0,0,0;80,45,15,0,0,0;80,45,15,0,0,0&rtp=95.02&reel_set0=12,13,7,8,12,10,9,1,1,1,1,11,4,4,4,1,9,8,3,3,3,10,13,12,3,6,5,4~13,7,12,6,8,4,4,4,5,7,11,2,3,9,6,10,4,8,5,7,11,10,1,1,1,1,1~7,12,5,2,8,9,5,6,11,11,12,10,12,3,3,3,10,3,4,13,1,1,1,1,7~11,13,6,4,4,4,7,12,8,10,5,9,11,1,1,1,1,13,7,6,4,2,3,9,12,5,12,1~4,4,4,13,3,12,1,1,1,1,5,12,6,6,7,5,8,1,11,7,9,4,10,2,9,11,10~13,6,1,1,1,1,1,4,4,4,1,12,4,10,7,10,3,3,3,11,11,9,8,5,12,3,7,13,8&t=243&reel_set1=1,1,1,1,4,4,4,12,12,10,9,10,7,5,8,3,3,3,3,6,13,13,13,9,8,1,11~6,4,4,4,10,13,9,12,4,3,8,10,8,2,6,11,5,7,7,11,5,8,1,1,1,1,7~12,13,7,1,1,1,1,5,7,11,5,2,11,1,10,4,3,3,3,12,3,6,9,13,8,8~13,3,9,10,2,7,13,8,12,11,4,4,4,1,1,1,1,6,1,7,12,5,4,8,12,11,10,9~12,6,6,11,2,4,4,4,11,5,8,10,8,3,10,1,1,1,1,12,4,9,13,7,1,9,13,7~13,10,4,4,4,11,1,1,1,1,1,10,9,7,5,13,6,1,8,12,7,4,3,3,3,11&awt=rfm";
            }
        }
	
	
        #endregion
        public JurassicGiantsGameLogic()
        {
            _gameID = GAMEID.JurassicGiants;
            GameName = "JurassicGiants";
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
            JurassicGiantsBetInfo betInfo = new JurassicGiantsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new JurassicGiantsBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                JurassicGiantsBetInfo betInfo   = new JurassicGiantsBetInfo();
                betInfo.BetPerLine              = (float)message.Pop();
                betInfo.LineCount               = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in JurassicGiantsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
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
                _logger.Error("Exception has been occurred in JurassicGiantsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

    }
}
