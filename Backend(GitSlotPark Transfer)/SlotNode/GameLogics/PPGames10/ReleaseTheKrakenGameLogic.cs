using Akka.Actor;
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
    public class ReleaseTheKrakenResult : BasePPSlotSpinResult
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
    class ReleaseTheKrakenGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20kraken";
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
            get { return 20; }
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
                return "def_s=4,3,11,3,7,10,9,4,11,8,11,8,4,6,4,5,8,9,10,11&cfgs=5222&ver=2&reel_set_size=3&def_sb=8,6,11,8,6&def_sa=9,3,7,10,4&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&fs_aw=t&cls_s=-1&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~500,200,50,0,0~1,1,1,1,1;16~500,200,50,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,200,50,0,0;250,100,40,0,0;200,80,40,0,0;150,60,20,0,0;100,60,20,0,0;80,20,10,0,0;80,20,10,0,0;40,10,5,0,0;40,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=9,11,3,3,3,3,3,3,3,3,10,10,8,0,7,9,4,10,11,11,2,5,8,8,9,6,0,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10~9,11,3,3,3,3,3,3,3,3,10,10,8,7,9,4,10,11,11,2,5,8,8,9,6,3,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10~9,11,3,3,3,3,3,3,3,3,10,10,8,0,7,9,4,10,11,11,2,5,8,8,9,6,0,3,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10~9,11,3,3,3,3,3,3,3,3,10,10,8,7,9,4,10,11,11,2,5,8,8,9,6,3,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10~9,11,3,3,3,3,3,3,3,3,10,10,8,7,9,4,10,13,11,11,2,5,8,8,9,6,14,10,11,7,5,5,8,10,6,6,11,10,14,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10&reel_set2=11,11,11,11,3,3,3,3,3,3,3,3,9,9,9,9,2,10,10,10,10,0,4,4,4,4,7,7,7,7,8,8,8,8,5,5,5,5,11,11,11,11,6,6,6,6~11,11,11,11,3,3,3,3,3,3,3,3,9,9,9,9,2,10,10,10,10,4,4,4,4,3,7,7,7,7,8,8,8,8,5,5,5,5,11,11,11,11,6,6,6,6~11,11,11,11,3,3,3,3,3,3,3,3,9,9,9,9,2,10,10,10,10,0,4,4,4,4,3,7,7,7,7,8,8,8,8,5,5,5,5,11,11,11,11,6,6,6,6~11,11,11,11,3,3,3,3,3,3,3,3,9,9,9,9,2,10,10,10,10,4,4,4,4,3,7,7,7,7,8,8,8,8,5,5,11,11,11,11,6,6,6,6~11,11,11,11,3,3,3,3,3,3,3,3,9,9,9,9,2,10,10,10,10,4,4,4,4,7,7,7,7,13,8,8,8,8,5,5,5,5,14,11,11,11,11,6,6,6,6&reel_set1=9,11,3,3,3,3,3,3,3,3,10,10,8,7,9,4,10,11,11,5,8,8,9,6,3,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10~9,11,3,3,3,3,3,3,3,3,10,10,8,7,9,4,10,11,11,5,8,8,9,6,3,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10~9,11,3,3,3,3,3,3,3,3,10,10,8,7,9,4,10,11,11,5,8,8,9,6,3,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10~9,11,3,3,3,3,3,3,3,3,10,10,8,7,9,4,10,11,11,5,8,8,9,6,3,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10~9,11,3,3,3,3,3,3,3,3,10,10,8,7,9,4,10,11,11,5,8,8,9,6,3,10,11,7,5,5,8,10,6,6,11,10,9,5,8,7,7,8,10,9,9,11,5,4,4,9,10&purInit=[{type:\"bg\",bet:2000,game_id:2}]&total_bet_min=10.00&awt=6rl";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 100; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
	
	
        #endregion
        public ReleaseTheKrakenGameLogic()
        {
            _gameID = GAMEID.ReleaseTheKraken;
            GameName = "ReleaseTheKraken";
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

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in ReleaseTheKrakenGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKrakenGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        private int getStatusLength(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return strParts.Length;
        }
        private bool isStatusAllZero(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strParts.Length; i++)
            {
                if (strParts[i] != "0")
                    return false;
            }
            return true;
        }

        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin      = 0.0;
                string strGameLog   = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    ReleaseTheKrakenResult  result          = _dicUserResultInfos[strGlobalUserID] as ReleaseTheKrakenResult;
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

                        int ind = -1;
                        if (dicParams.ContainsKey("status") && !isStatusAllZero(dicParams["status"]))
                        {
                            ind                 = (int)message.Pop();
                            int statusLength    = getStatusLength(dicParams["status"]);
                            if (statusLength == 10)
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
                                int[] status = new int[statusLength];
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    status[result.BonusSelections[i]] = i + 1;
                                dicParams["status"] = string.Join(",", status);

                                if (dicParams.ContainsKey("wins"))
                                {
                                    string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWins = new string[statusLength];
                                    for (int i = 0; i < statusLength; i++)
                                        strNewWins[i] = "0";
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWins[result.BonusSelections[i]] = strWins[i];
                                    dicParams["wins"] = string.Join(",", strNewWins);
                                }
                                if (dicParams.ContainsKey("wins_mask"))
                                {
                                    string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWinsMask = new string[statusLength];
                                    for (int i = 0; i < statusLength; i++)
                                        strNewWinsMask[i] = "h";
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                                    dicParams["wins_mask"] = string.Join(",", strNewWinsMask);
                                }
                            }
                            else
                            {
                                int[] status = new int[statusLength];
                                status[ind] = 1;
                                dicParams["status"] = string.Join(",", status);
                                if (dicParams.ContainsKey("wins"))
                                {
                                    string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (ind != 0)
                                    {
                                        string strTemp = strWins[ind];
                                        strWins[ind] = strWins[0];
                                        strWins[0] = strTemp;
                                    }
                                    dicParams["wins"] = string.Join(",", strWins);
                                }
                                if (dicParams.ContainsKey("wins_mask"))
                                {
                                    string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (ind != 0)
                                    {
                                        string strTemp      = strWinsMask[ind];
                                        strWinsMask[ind]    = strWinsMask[0];
                                        strWinsMask[0]      = strTemp;
                                    }
                                    dicParams["wins_mask"] = string.Join(",", strWinsMask);
                                }
                            }
                        }
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                        string      strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                        {
                            if (ind >= 0)
                                addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);
                            else
                                addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);
                        }

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;
                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win     = realWin;
                            }

                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
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
                _logger.Error("Exception has been occurred in ReleaseTheKrakenGameLogic::onDoBonus {0}", ex);
            }
        }

        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                ReleaseTheKrakenResult spinResult       = new ReleaseTheKrakenResult();
                Dictionary<string, string> dicParams    = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                spinResult.NextAction   = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                spinResult.ResultString = convertKeyValuesToString(dicParams);

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKrakenGameLogic::calculateResult {0}", ex);
                return null;
            }
        }

        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            ReleaseTheKrakenResult result = new ReleaseTheKrakenResult();
            result.SerializeFrom(reader);
            return result;
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if (!resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
                resultParams["g"] = spinParams["g"];
            return resultParams;
        }
    }
}
