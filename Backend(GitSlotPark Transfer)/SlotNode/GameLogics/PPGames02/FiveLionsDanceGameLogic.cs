using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Akka.Event;

namespace SlotGamesNode.GameLogics
{

    public class FiveLionsDancBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 20.0f; }
        }
    }
    public class FiveLionsDanceGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1024lionsd";
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
                return 1024;
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
                return "def_s=3,5,4,11,6,3,1,8,11,6,11,11,7,11,6,11,3,8,11,10&cfgs=4162&ver=2&def_sb=3,10,6,13,6&reel_set_size=2&def_sa=11,5,4,10,6&scatters=1~0,0,1,0,0~0,0,10,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&base_aw=ap~5;ap~10;ap~15;ap~5;ap~20;ap~10;ap~25;ap~5;ap~10;ap~5;ap~10;ap~20;ap~15;ap~5;ap~25;ap~5;ap~10;ap~5;ap~10;ap~15;ap~20;ap~5;ap~10;ap~100;ap~25;ap~50;ap~5;ap~10;ap~75;ap~100;ap~15;ap~200;ap~20;ap~25;ap~5;ap~150;ap~15;ap~25;ap~50;ap~5;ap~20;ap~15;ap~25;ap~20;ap~75;ap~10;ap~15;ap~5;ap~15;ap~20&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,80,20,0,0;160,60,10,0,0;160,60,10,0,0;80,30,8,0,0;80,30,8,0,0;60,20,4,0,0;60,20,4,0,0;40,8,2,0,0;40,8,2,0,0;40,8,2,0,0;0,0,0,0,0;0,0,0,0,0&rtp=94.50&reel_set0=4,9,11,7,7,7,1,5,7,12,12,12,7,12,10,8,5,8,8,8,6,6,6,3,6,3,7,3,7,12,7,1,7,12,1,12,6,12,7,10,7,1,7,6,11,7,10,8,1,7,6,1,12,8,7,6,1,8,7,1,12,7,1,6,10,7,10,7,8,12,8,7,12,7,5,7,5,9,12,8,12,1,12,8,1,6,10,12,1,10,9,5,7,12,7,12,6,7,9,10,8,6,10,7~7,7,7,2,11,7,5,5,5,10,6,4,4,4,3,12,8,5,6,6,6,9,4,1,3,3,3,12,5,6,5,10,4,3,10,6,4,10,4,9,10,4~7,5,3,9,2,8,10,12,11,1,13,6,4,6,9,2,13,4,3,4,10,5,4,5,6,8~8,12,12,12,6,11,11,11,7,4,5,3,9,9,9,12,10,10,10,2,13,9,11,10,9~13,7,7,7,9,10,10,10,2,11,11,11,11,7,5,9,9,9,10,6,4,3,12,8,6,6,6,8,8,8,7,8,10,8,7,10,6,9,7,2,9,8,6,10,9,7,11,8,7,8,6,8,9,8&t=243&reel_set1=12,12,12,11,8,8,8,4,3,7,6,5,8,12,10,1,9,8,7,10,6,7,5,8,6,8~3,10,10,10,2,11,12,12,12,12,1,10,9,8,4,7,5,6,12,4,9,10,11,5,2,10,1,10,1,12,6,12,11,7,10,4,10,12,4,10,5,4,12,11,9,12,11,12,2,6,10,8,1,5,10,4,5,9~10,10,10,5,12,12,12,11,6,2,3,9,1,8,4,7,12,10,1,9,12,7,1,12,7,9,11,8,9,7,8,12,1,12,1,12,1,7,3,12,11,1,3,7,9,7,11,1,9,7,9~11,8,8,8,4,10,10,10,12,3,10,6,2,9,7,8,5,8,4,10,9,8,2,10,8,5,8,5,10,6,8,2,8,10,9,10,2,5,2,4,10,9,8,2,9,4,8,9,10,3,9~11,11,11,12,2,9,9,9,7,6,10,11,3,9,8,5,4,10,2,7,8,2,8,4,2,3,7,9,7,9,7,3,2,8,9,8,3,7,6,3,9,2,3,9,7,6,8,9,3,9,10,9,7,9,3,7,5,7,3,10,7,4,6,2,9,3,7&awt=6rl";
            }
        }
        #endregion
        public FiveLionsDanceGameLogic()
        {
            _gameID = GAMEID.FiveLionsDance;
            GameName = "FiveLionsDance";
        }

        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa")) //FiveLionsDance 보너스당첨
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);

        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams.Add("reel_set", "0");
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                FiveLionsDancBetInfo betInfo = new FiveLionsDancBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FiveLionsDanceGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in FiveLionsDanceGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            FiveLionsDancBetInfo betInfo = new FiveLionsDancBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }

        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new FiveLionsDancBetInfo();
        }
        protected override void onDoCollect(int websiteID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} result information has not been found in BasePPSlotGame::onDoCollect.", strGlobalUserID);
                    return;
                }

                BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                if (result.NextAction != ActionTypes.DOCOLLECT)
                {
                    _logger.Error("{0} next action is not DOCOLLECT just {1} in BasePPSlotGame::onDoCollect.", strGlobalUserID, result.NextAction);
                    return;
                }

                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                Dictionary<string, string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance", Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_cash", Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_bonus", "0.00");
                responseParams.Add("na", "s");
                responseParams.Add("stime", GameUtils.GetCurrentUnixTimestampMillis().ToString());
                responseParams.Add("sver", "5");
                responseParams.Add("index", index.ToString());
                responseParams.Add("counter", (counter + 1).ToString());

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECT);
                string strCollectResponse = convertKeyValuesToString(responseParams);
                reponseMessage.Append(strCollectResponse);

                bool needDelay = false;
                if(_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    Dictionary<string, string> dicParams = splitResponseToParams(_dicUserResultInfos[strGlobalUserID].ResultString);
                    if (dicParams.ContainsKey("apv"))
                        needDelay = true;
                }
                if (needDelay)
                    Context.System.Scheduler.ScheduleTellOnce(250, Sender, new ToUserMessage((int)_gameID, reponseMessage), Self);
                else
                    Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);

                addActionHistory(strGlobalUserID, "doCollect", strCollectResponse, index, counter);
                saveHistory(websiteID, strUserID, index, counter, userBalance, currency);

                result.NextAction = ActionTypes.DOSPIN;
                saveBetResultInfo(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPFreeSlotGame::onDoCollect {0}", ex);
            }
        }
    }
}
