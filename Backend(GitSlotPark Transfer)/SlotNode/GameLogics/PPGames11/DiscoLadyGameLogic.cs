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
    class DiscoLadyBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 50.0f; }
        }
    }
    class DiscoLadyGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243discolady";
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
            get { return 50; }
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
                return "def_s=3,10,5,4,7,5,8,4,3,10,4,6,9,5,7&cfgs=5480&ver=3&def_sb=5,10,7,4,10&reel_set_size=4&def_sa=5,8,1,3,6&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"892857\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=4.00,8.00,12.00,16.00,20.00,40.00,60.00,80.00,100.00,150.00,200.00,300.00,500.00,1000.00,1500.00,2000.00&defc=20.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,150,75,0,0;300,150,50,0,0;250,100,40,0,0;200,100,30,0,0;125,30,15,0,0;125,30,15,0,0;100,20,10,0,0;100,20,10,0,0&reel_set0=1,9,9,4,7,8,6,8,1,8,9,9,9,7,8,8,9,3,3,10,3,10,6,10,4,4,4,7,6,9,5,6,7,10,1,7,1,3,1,5,5,5,10,3,1,4,1,8,5,7,7,10,6,6,6,4,9,5,1,7,8,10,10,9,1,7,7,7,9,8,10,7,4,6,10,9,5,5,7,6,8,8,8,5,4,4,10,10,5,9,4,4,3,8,3,3,3,7,8,7,6,1,10,4,8,8,6,8,8~5,3,10,6,2,7,9,1,3,9,10,2,2,2,8,2,9,5,2,6,9,8,9,10,9,8,5,5,5,7,8,9,6,5,6,7,5,6,6,8,9,4,4,4,9,2,6,9,9,1,4,8,10,6,4,8,8,8,5,6,4,1,7,8,8,1,6,3,4,6,9,9,9,7,8,9,1,8,8,1,6,5,8,4,6,6,6,8,6,1,10,2,9,1,3,1,8,3,9~7,3,9,4,10,8,5,10,10,5,5,6,9,9,5,5,4,3,5,10,2,8,9,5,1,3,7,9,3,1,5,2,2,2,10,10,4,1,9,10,1,10,10,3,1,9,5,9,9,6,9,7,1,7,4,6,6,9,1,10,8,9,5,4,2,9,7,10,10,7,6~6,5,6,7,7,6,9,6,6,5,4,9,9,9,3,6,7,5,6,6,1,7,6,8,10,3,8,8,8,9,6,1,6,2,8,3,4,2,9,1,4,6,6,6,5,3,7,10,1,8,8,6,4,7,9,4,4,4,6,7,4,2,6,7,9,7,6,8,7,5,5,5,8,2,5,3,1,6,8,6,7,7,1,7,7,7,4,10,7,1,3,2,6,3,1,9,7,7,2,2,2,10,7,3,4,4,10,1,6,2,4,1,3,3,3,4,6,3,4,4,10,8,9,1,7,5,7,1,7~7,2,4,8,5,9,1,1,10,9,4,4,4,5,1,4,1,2,3,10,4,4,7,1,5,5,5,7,1,6,7,5,7,7,6,8,9,3,1,1,1,9,9,5,10,8,3,9,10,1,3,7,3,3,3,8,6,10,4,5,3,7,2,10,3,4,7,7,7,2,1,8,8,9,10,7,8,9,9,1,6,6,6,5,1,1,7,9,10,7,5,9,9,8,8,8,10,9,4,4,7,7,5,1,3,1,5,10,10,10,8,5,7,6,8,6,4,1,5,9,2,2,2,4,10,10,8,5,5,10,10,2,6,8,1,7&reel_set2=9,8,8,1,5,8,1,7,8,5,5,3,3,6,3,3,3,7,10,5,8,10,4,4,8,8,1,9,7,10,7,4,7,3,4,4,4,10,4,1,6,8,6,4,10,4,6,3,9,3,8,1,8,3,5,5,5,3,8,9,7,4,5,9,7,5,10,1,7,3,7,10,7,1~5,6,1,9,8,7,6,1,9,8,8,7,9,8,8,8,2,10,1,3,8,8,9,7,4,1,4,8,5,4,4,4,7,5,4,9,2,5,10,9,3,5,6,8,6,1,3,3,3,9,10,2,6,4,1,9,10,9,1,3,8,8,9,5,5,5,7,8,6,1,8,8,4,3,10,9,6,1,8,9,3~8,10,9,4,9,8,1,10,7,8,9,4,10,9,8,9,10,4,5,6,7,1,9,7,5,8,3,3,3,10,5,1,3,5,6,9,4,1,7,3,5,5,7,10,10,4,3,10,1,6,10,7,9,6,5,4,4,4,3,1,7,1,5,10,3,2,9,5,1,8,9,6,4,2,10,4,4,8,9,8,7,1,9,4,9~5,1,7,1,10,7,6,10,7,7,3,10,5,10,8,1,7,3,3,3,6,7,1,6,7,2,7,1,7,7,6,9,8,9,8,6,3,7,6,6,6,10,9,7,3,6,8,4,5,6,6,9,8,7,4,8,7,9,4,4,4,7,4,6,8,4,6,10,3,8,1,4,1,9,4,4,6,3,9,1,3~8,10,9,1,9,7,5,1,6,5,5,1,5,4,1,10,4,4,4,9,8,4,5,6,7,10,3,2,7,3,3,1,8,1,8,10,4,3,3,3,9,4,8,10,8,9,9,1,5,3,4,10,5,4,10,5,7,7,5,5,5,1,7,3,10,4,7,4,9,10,7,9,4,7,1,5,10,8,4,7,7,7,9,1,10,7,10,3,9,6,9,10,7,3,10,5,10,6,5,1,4&reel_set1=6,4,10,7,5,4,3,8,10,9,5,1,10,9,7,1,10,4,7,3,9,5,10,3,1,8,8,1,8,4,8,5,5,5,7,7,5,8,6,6,9,1,7,5,1,8,4,7,10,3,8,10,1,8,8,3,10,3,9,7,5,8,7,4,6,6,3,9~8,6,3,1,9,8,8,3,8,1,9,7,9,4,5,6,5,6,5,3,7,3,1,8,8,8,7,2,4,1,5,8,10,6,8,8,4,10,6,9,4,6,8,10,2,8,6,8,2,10,4,1,9,5,5,5,1,3,1,7,10,9,8,9,5,7,1,8,9,8,1,9,1,6,9,8,9,8,8,9,5~9,7,8,9,7,9,4,8,6,5,9,1,9,4,7,1,10,9,8,1,5,2,8,1,5,3,6,9,10,9,1,6,9,5,1,3,9,4,10,8,3,5,9,10,10,4,7,10,7,9,1,5,10,9,4,6,1,4,7,10,5,8,2,4,7,6,5,10,1,10~6,7,7,6,3,9,3,9,7,4,8,9,7,3,3,3,7,9,7,8,2,10,7,8,8,1,7,5,6,7,6,6,6,7,8,7,7,10,5,3,1,4,8,6,1,4,6,4,4,4,10,1,3,1,9,7,8,10,1,6,3,6,4,4~1,10,9,4,5,6,7,7,3,4,7,8,3,10,5,10,3,9,4,4,4,7,4,5,1,9,10,1,9,5,4,10,7,1,5,10,5,8,6,10,3,3,3,2,9,10,8,1,9,9,10,5,8,7,3,6,8,9,9,7,7,3,5,5,5,7,10,5,1,7,4,4,3,10,4,4,7,6,10,1,4,9,3,5,7,7,7,9,10,5,1,4,4,5,1,8,5,8,3,10,1,6,5,5,10,9,5,5&reel_set3=3,5,7,8,6,10,7,4,8,6,9,9,9,7,1,5,10,7,7,9,10,3,7,5,10,4,4,4,7,9,8,5,10,8,3,8,9,3,6,5,5,5,8,8,4,5,10,9,1,4,8,9,1,10,6,6,6,8,1,10,5,1,9,7,7,6,5,1,6,7,7,7,4,9,6,1,5,4,7,4,10,1,4,8,8,8,9,9,4,8,7,10,6,1,10,1,6,10,3,3,3,4,8,1,10,8,7,8,3,3,8,3,4~1,5,7,9,10,4,9,1,6,9,5,6,6,8,2,2,2,8,1,7,6,1,2,5,4,4,2,6,4,9,1,5,5,5,6,9,8,3,8,10,5,3,5,9,6,8,6,9,6,4,4,4,2,8,6,9,1,7,6,8,1,5,9,10,6,8,8,8,3,9,5,8,1,10,6,9,7,9,8,8,1,7,1,9,9,9,10,8,3,8,6,4,8,8,10,9,6,3,9,2,6,6,6,9,8,10,8,1,2,9,8,3,5,3,8,9,8,7,9,2~10,9,9,1,10,3,10,9,6,7,8,1,9,6,1,5,10,10,6,5,5,10,7,5,9,1,10,9,6,9,10,9,2,10,9,1,5,5,2,1,4,7,9,10,3,2,8,10,8,9,10,5,1,5,9,9,4,2,2,2,1,7,5,9,10,9,8,2,4,9,3,10,5,4,10,5,1,5,6,4,3,9,6,1,9,5,10,5,1,4,7,2,7,1,5,5,7,7,10,3,8,10,6,4,6,10,9,4,9,9,3,7,1,3,6,5,7,10,4,3~7,3,6,10,9,8,7,1,9,9,9,7,1,6,7,6,1,7,7,6,8,8,8,1,9,6,4,5,4,6,7,3,4,6,6,6,1,6,9,8,10,1,2,6,4,4,4,6,5,7,5,8,6,6,7,4,6,3,5,5,5,2,4,2,7,3,10,4,5,8,1,7,7,7,4,1,3,7,8,7,8,9,3,7,2,2,2,5,7,10,4,6,2,1,2,3,3,3,6,6,4,9,6,4,3,9,7,10,5,7~3,1,2,10,10,6,8,10,8,7,4,4,4,7,9,8,1,6,7,9,5,10,8,1,5,5,5,7,4,2,4,4,8,7,9,9,10,6,4,1,1,1,4,5,7,1,3,7,3,5,8,9,5,3,3,3,1,1,5,1,1,9,7,1,1,5,1,9,7,7,7,5,7,4,6,10,5,7,5,5,2,1,5,6,6,6,1,3,4,9,10,9,8,9,3,4,10,8,8,8,9,6,10,10,2,7,7,3,2,9,7,10,10,10,4,8,6,8,1,10,9,4,10,9,3,5,2,2,2,1,5,10,9,8,1,8,10,10,7,7,3,7";
            }
        }
	
	
        #endregion
        public DiscoLadyGameLogic()
        {
            _gameID = GAMEID.DiscoLady;
            GameName = "DiscoLady";
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
            if (dicParams.ContainsKey("wlc_v"))
            {
                string[] strParts = dicParams["wlc_v"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strValues = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    strValues[1] = convertWinByBet(strValues[1], currentBet);
                    strParts[i] = string.Join("~", strValues);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);

            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            DiscoLadyBetInfo betInfo = new DiscoLadyBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new DiscoLadyBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                DiscoLadyBetInfo betInfo = new DiscoLadyBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DiscoLadyGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in DiscoLadyGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
