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
    public class CaishenCashResult : BasePPSlotSpinResult
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

    class CaishenCashBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 25.0f;
            }
        }
    }
    class CaishenCashGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243caishien";
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
            get { return 243; }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
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
                return "def_s=6,11,13,14,8,12,7,10,4,3,9,4,5,4,3&cfgs=2472&ver=2&mo_s=14&reel_set_size=2&def_sb=2,7,5,3,5&mo_v=8,18,28,38,58,68,78,88,118,138,188,238,288,588,888,1888&def_sa=6,2,7,3,8&bonusInit=[{bgid:1,bgt:15,bg_i:\"1000,100,50,25\",bg_i_mask:\"pw,pw,pw,pw\"}]&scatters=&gmb=0,0,0&rt=d&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1;15~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;150,50,30,0,0;100,30,25,0,0;75,30,20,0,0;60,25,15,0,0;50,20,15,0,0;25,15,12,0,0;25,15,12,0,0;20,12,8,0,0;20,12,8,0,0;20,12,8,0,0;20,12,8,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=11,8,8,3,9,5,7,5,3,3,3,4,10,6,3,4,4,8,8,10,6,11,7,11,6,14,14,14,10,6,3,11,7,12,6,13,4,4,4~14,14,14,11,3,2,11,5,10,13,3,3,13,9,3,3,13,9,7,5,13,6,13,2,3,4,4,6,9,2,12,8~3,13,9,7,9,8,14,14,4,13,12,12,5,10,12,13,8,6,2,4,4,4,5,5,12,5,11,5,2,11,11,10,6,3,12,7,8,8~5,11,4,14,14,10,7,13,3,14,14,13,13,8,4,5,5,13,4,9,5,11,5,10,9,4,10,2,12,9,9,6,8,9,2,10~8,6,6,9,4,7,9,9,6,9,8,14,14,9,10,5,13,4,9,3,11,10,5,9,5,10,9,11,5,14,14,3,5,9,4,9,3,11,3,13,4,6,6,14,14,12&t=243&reel_set1=3,7,3,3,3,6,3,5,4,7,7,4,5,5,3,3,5,5,3,6,6,7,3,4,4,4,4,7,6,3,4,6,4,3,5,6,4,4,4,3,7,4,4,7,6,5,5,5,3~5,7,4,6,5,5,3,6,6,3,3,6,6,4,6,3,15,15,4,7,6,6,6,5,5,5,4,6,7,5,6,4,4,3,7,4,4,3,7,3~4,6,5,5,5,7,7,7,3,3,5,4,5,15,15,4,5,7,3,7,4,4,5,5,3,7,4,3,3,7,3,7,7,5,3,7,7,7,6,7,6~6,6,4,4,4,4,7,5,6,4,6,3,15,15,6,6,5,5,5,4,6,4,7,3,7,4,6,4,4,3,4,7,4,7,3,4,5,6,7,6,4,5~5,4,3,3,3,6,6,5,7,6,6,3,4,5,5,5,6,3,7,7,6,6,6,5,6,4,5,4,3,6,5,7,7,7,7,6,6,6,4";
            }
        }
	
	
        #endregion
        public CaishenCashGameLogic()
        {
            _gameID = GAMEID.CaishenCash;
            GameName = "CaishenCash";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);            
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                CaishenCashResult spinResult = new CaishenCashResult();
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
                _logger.Error("Exception has been occurred in CaishenCashGameLogic::calculateResult {0}", ex);
                return null;
            }
        }

        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            CaishenCashResult result = new CaishenCashResult();
            result.SerializeFrom(reader);
            return result;
        }
        private int getStatusLength(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return strParts.Length;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();

                GITMessage responseMessage  = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin              = 0.0;
                string strGameLog           = "";
                string strGlobalUserID      = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    CaishenCashResult       result          = _dicUserResultInfos[strGlobalUserID] as CaishenCashResult;
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                        if(dicParams.ContainsKey("status"))
                        {
                            int ind             = (int)message.Pop();
                            int statusLength    = getStatusLength(dicParams["status"]);    
                            if(statusLength == 12)
                            {
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
                            }
                            else
                            {
                                if (dicParams.ContainsKey("status"))
                                {
                                    string[] strItems = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if(ind != 0)
                                    {
                                        string strTemp = strItems[0];
                                        strItems[0] = strItems[1];
                                        strItems[1] = strTemp;
                                        dicParams["status"] = string.Join(",", strItems);
                                    }
                                }
                                if (dicParams.ContainsKey("wins"))
                                {
                                    string[] strItems = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (ind != 0)
                                    {
                                        string strTemp = strItems[0];
                                        strItems[0] = strItems[1];
                                        strItems[1] = strTemp;
                                        dicParams["wins"] = string.Join(",", strItems);
                                    }
                                }
                                if (dicParams.ContainsKey("wins_mask"))
                                {
                                    string[] strItems = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (ind != 0)
                                    {
                                        string strTemp = strItems[0];
                                        strItems[0] = strItems[1];
                                        strItems[1] = strTemp;
                                        dicParams["wins_mask"] = string.Join(",", strItems);
                                    }
                                }
                            }
                        }

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);                      
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                        string      strResponse = convertKeyValuesToString(dicParams);

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
                            resultMsg                   = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID  = betInfo.BetTransactionID;
                            resultMsg.RoundID           = betInfo.RoundID;
                            resultMsg.TransactionID     = createTransactionID();

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
                _logger.Error("Exception has been occurred in CaishenCashGameLogic::onDoBonus {0}", ex);
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

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            CaishenCashBetInfo betInfo = new CaishenCashBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                CaishenCashBetInfo betInfo = new CaishenCashBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in CaishenCashGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in CaishenCashGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }


}
