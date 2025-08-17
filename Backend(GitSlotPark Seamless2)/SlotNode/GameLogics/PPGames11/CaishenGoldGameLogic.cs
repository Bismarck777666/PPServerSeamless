using Akka.Actor;
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
    class CaishenGoldBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 38.0f; }
        }
    }
    public class CaishenGoldResult : BasePPSlotSpinResult
    {
        public List<int> BonusSelections { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            BonusSelections = SerializeUtils.readIntList(reader);
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            SerializeUtils.writeIntList(writer, BonusSelections);
        }
    }
    class CaishenGoldGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243fortune";
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
            get { return 38; }
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
                return "def_s=6,7,4,2,8,9,3,5,6,7,8,5,7,3,9&cfgs=2011&ver=2&reel_set_size=2&def_sb=5,6,1,4,8&def_sa=6,7,5,4,4&bonusInit=[{bgid:0,bgt:15,bg_i:\"1000,100,50,30\",bg_i_mask:\"pw,pw,pw,pw\"}]&scatters=1~1900,380,190,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&bg_i=1000,100,50,30&rt=d&sc=5.00,10.00,15.00,20.00,25.00,50.00,75.00,100.00,125.00,200.00,250.00,375.00,700.00,1250.00,2000.00,2750.00&defc=25.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&bg_i_mask=pw,pw,pw,pw&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,75,25,0,0;150,50,20,0,0;125,30,15,0,0;100,25,10,0,0;75,20,10,0,0;50,10,5,0,0;50,10,5,0,0;25,10,5,0,0;25,10,5,0,0;25,10,5,0,0;25,10,5,0,0&reel_set0=6,7,5,4,4,4,10,4,13,9,1,8,7,12,5,3,3,3,6,5,6,5,10,8,3,9,3,12,8,10,3,9,5,4,11,3,12,3,6,4,13,7,11,4,13~9,13,10,8,13,6,10,3,11,7,3,5,9,8,2,11,6,12,3,5,10,3,1,1,7,8,2,13,5,3,5,11,6,9,4,10,7,2,1,5,4,4,4,4~7,13,3,7,5,12,4,4,4,8,4,5,11,13,9,1,1,7,4,12,3,10,5,1,3,5,2,3,2,7,11,6,8,6,12,4,6,13,5,1,3,8,4,10,9,7,11~7,9,12,10,4,12,13,2,5,11,10,9,11,10,13,3,5,3,7,10,8,8,8,6,3,2,4,4,6,8,4,11,11,8,9,5,12,1,13,9,13,5~5,6,1,4,8,9,10,9,4,11,13,3,5,13,6,9,6,13,8,5,6,1,2,5,9,11,6,8,4,7,3,13,7,10,12,11,9,10,12,2,8,5,9,12,3,4&t=243&reel_set1=3,6,3,6,5,5,5,5,4,5,3,3,1,7,7,7,3,5,5,4,7,6,4,3,7,6,7,6,7,6,3,4,7,4,3,4,4,5,4,4,7,7,5,7,3,1~3,3,3,3,6,7,7,7,1,6,3,5,5,5,3,4,4,4,3,6,7,4,5,7,5,3,7,3,4,5,6,5,7,6,6,6,4,4,7,7,7,3,5,4,2,5,6,4,1,2,4,6~6,6,6,7,7,7,3,3,3,4,4,4,7,4,7,6,1,2,3,1,4,6,5,5,5,4,3,4,3,5,7,3,4,5,5,6,3,7,5,7,3,5,4,3,3,6,7,4,4,7,6~6,5,5,5,2,4,4,4,4,7,4,6,3,3,3,6,6,4,5,4,4,5,6,4,6,6,3,3,3,3,3,1,7,3,3,1,4,7,4,5,7,3,5,7,3,7,2,5,4~4,6,6,6,1,4,4,5,5,5,4,6,5,4,6,6,7,7,7,7,5,5,7,3,3,3,3,4,3,4,5,6,7,5,7,4,6,6,6,7,5,7,4,6,3,5,1,4,3,3,5,3";
            }
        }
	
	
        #endregion
        public CaishenGoldGameLogic()
        {
            _gameID = GAMEID.CaishenGold;
            GameName = "CaishenGold";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            CaishenGoldBetInfo betInfo = new CaishenGoldBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                CaishenGoldBetInfo betInfo = new CaishenGoldBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in CaishenGoldGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in CaishenGoldGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "gsf_r", "gsf" };
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

        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                CaishenGoldResult spinResult = new CaishenGoldResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in CaishenGoldGameLogic::calculateResult {0}", ex);
                return null;
            }
        }

        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            CaishenGoldResult result = new CaishenGoldResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();
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
                    CaishenGoldResult result = _dicUserResultInfos[strGlobalUserID] as CaishenGoldResult;
                    BasePPSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if (result.BonusSelections == null)
                            result.BonusSelections = new List<int>();

                        if (result.BonusSelections.Contains(ind))
                        {
                            betInfo.pushFrontResponse(actionResponse);
                            saveBetResultInfo(strGlobalUserID);
                            throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                        }

                        result.BonusSelections.Add(ind);
                        if (dicParams.ContainsKey("status"))
                        {
                            int[] status = new int[12];
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                status[result.BonusSelections[i]] = i + 1;
                            dicParams["status"] = string.Join(",", status);
                        }
                        if (dicParams.ContainsKey("wins"))
                        {
                            string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strNewWins = new string[12];
                            for (int i = 0; i < 12; i++)
                                strNewWins[i] = "0";
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                strNewWins[result.BonusSelections[i]] = strWins[i];
                            dicParams["wins"] = string.Join(",", strNewWins);
                        }
                        if (dicParams.ContainsKey("wins_mask"))
                        {
                            string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strNewWinsMask = new string[12];
                            for (int i = 0; i < 12; i++)
                                strNewWinsMask[i] = "h";
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                            dicParams["wins_mask"] = string.Join(",", strNewWinsMask);
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
                _logger.Error("Exception has been occurred in CaishenGoldGameLogic::onDoBonus {0}", ex);
            }
        }

    }
}
