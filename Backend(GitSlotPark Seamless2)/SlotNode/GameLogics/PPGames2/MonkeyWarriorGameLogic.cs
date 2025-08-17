using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Akka.Actor;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    public class MonkeyWarriorBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 25.0f;
            }
        }
    }
    class MonkeyWarriorGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243mwarrior";
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
                return 25;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 25;
            }
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
                return "def_s=10,3,6,8,9,9,7,10,3,5,4,5,8,4,10&cfgs=2998&ver=2&mo_s=11&reel_set_size=2&def_sb=5,9,3,10,3&mo_v=25,50,75,100,125,150,175,200,250,350,400,450,500,600,750,1250,2500,5000&def_sa=4,9,7,9,10&mo_jp=750;1250;2500;5000&scatters=1~0,0,2,0,0~8,8,8,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&mo_jp_mask=jp4;jp3;jp2;jp1&sc=10.00,20.00,50.00,100.00,250.00,500.00,1000.00,3000.00,5000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,50,25,0,0;300,40,25,0,0;200,35,20,0,0;150,30,20,0,0;50,20,10,0,0;50,20,10,0,0;50,15,10,0,0;50,15,10,0,0;0,0,0,0,0;0,0,0,0,0&rtp=95.50&reel_set0=6,4,10,9,4,5,8,7,6,9,7,11,10,11,11,9,10,6,9,5,3,10,7,4,10,5,8,6,10,9~2,9,3,7,5,9,6,5,8,9,1,10,8,5,7,4,8,11,11,11,5,9,6,10,8,1,9,7,5,8,6,9,10~2,7,6,10,8,3,7,9,1,10,4,8,6,9,11,11,11,8,6,3,9,5,8,1,10,8,3,7,9,4~2,9,8,3,4,10,8,4,9,7,1,10,1,7,4,9,7,11,11,11,10,9,6,7,4,5,8,1,9,3,10,4,8,7,3,9,7,5,10,9,3,7,4,10,6,4,7,8~2,10,9,5,10,3,9,5,8,3,4,7,5,4,10,8,5,10,4,6,7,3,8,10,11,11,11,8,4,9&t=243&reel_set1=3,5,11,11,5,5,4,4,5,5,6,6,4,4,5,5,6,6,11,11,6,6,4,4,5,5,6,6,6,4~5,6,3,1,5,6,4,5,1,5,5,11,11,6,6,5,5,5,1,6,3,3,4,11,11,11,5,6,6,6,6,5,5,6~6,6,3,1,3,3,4,4,4,3,3,4,4,3,11,11,3,6,6,6,5,3,3,5,11,11,11,3,3,6,6,6,6~3,4,3,4,4,4,4,1,5,3,3,3,11,11,11,4,4,1,3,4,4,3,3,4,4,3,3,6,6,1,6,5~4,3,3,4,4,5,5,3,3,11,11,11,3,3,4,4,5,5,4,4,3,3,4,4,5,5,6,6,11,11,4,4";
            }
        }
        #endregion

        public MonkeyWarriorGameLogic()
        {
            _gameID = GAMEID.MonkeyWarrior;
            GameName = "MonkeyWarrior";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
        }

        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = -1;
                if(message.DataNum > 0)
                    ind = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin = 0.0;
                string strGameLog = "";
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotSpinResult    result          = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if(ind >= 0)
                        {
                            if(!dicParams.ContainsKey("wins") || !dicParams.ContainsKey("wins_mask") || !dicParams.ContainsKey("status") || ind >= 3)
                                throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));

                            string[] strWins     = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strStatus   = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                            //0인 경우 필요없음
                            if(ind != 0)
                            {
                                string strTemp = strWins[0];
                                strWins[0] = strWins[ind];
                                strWins[ind] = strTemp;

                                strTemp = strWinsMask[0];
                                strWinsMask[0] = strWinsMask[ind];
                                strWinsMask[ind] = strTemp;

                                strTemp = strStatus[0];
                                strStatus[0] = strStatus[ind];
                                strStatus[ind] = strTemp;
                            }
                            dicParams["wins"]       = string.Join(",", strWins);
                            dicParams["wins_mask"]  = string.Join(",", strWinsMask);
                            dicParams["status"]     = string.Join(",", strStatus);
                        }
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }
                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID = betInfo.BetTransactionID;
                            resultMsg.RoundID = betInfo.RoundID;
                            resultMsg.TransactionID = createTransactionID();

                        }
                        copyBonusParamsToResult(dicParams, result);
                        result.NextAction = nextAction;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MonkeyWarriorGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set", "gsf_r", "gsf" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("s") && spinParams.ContainsKey("s"))
                resultParams["s"] = spinParams["s"];
            return resultParams;
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                MonkeyWarriorBetInfo betInfo = new MonkeyWarriorBetInfo();
                betInfo.BetPerLine           = (float)message.Pop();
                betInfo.LineCount            = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MonkeyWarriorGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in MonkeyWarriorGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            MonkeyWarriorBetInfo betInfo = new MonkeyWarriorBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
