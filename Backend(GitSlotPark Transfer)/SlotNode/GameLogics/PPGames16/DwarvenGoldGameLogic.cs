using Akka.Actor;
using Akka.Event;
using GITProtocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    public class DwarvenGoldResult : BasePPSlotSpinResult
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
    class DwarvenGoldGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25dwarves";
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
            get { return 25; }
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
                return "tw=0.00&def_s=8,4,8,12,8,5,7,7,2,5,10,12,5,11,9&msg_code=0&pbalance=0.00&cfgs=1075&reel1=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,8,5,10,9,6,11,9,7,6,5,8,9,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,8,5,10,9,6,11,9,7,6,5,8,9,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,8,5,10,9,6,11,9,7,6,5,8,9,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,8,5,10,9,6,11,9,7,6,5,8,9,7,8,3,3&ver=2&reel0=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,11,9,7,6,5,8,9,2,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,11,9,7,6,5,8,9,2,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,11,9,7,6,5,8,9,2,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,11,9,7,6,5,8,9,2,7,8,3,3&reel3=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,12,9,7,6,5,8,9,7,8&reel2=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,12,9,6,11,9,7,6,5,8,9,7,8,3,3,8,7,5,6,9,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,12,9,6,11,9,7,6,5,8,9,7,8,3,3,8,7,5,6,9,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,12,9,6,11,9,7,6,5,8,9,7,8,3,3,8,7,5,6,9,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,12,9,6,11,9,7,6,5,8,9,7,8,3,3,8,7,5,6,9&reel4=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,9,6,12,9,7,6,5,8,9,7,8&gmb=0,0,0&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&pos=0,0,0,0,0&wilds=2~1,1,1,1,1&bonuses=0&fsbonus=1~100,20,5,2,0~8~2~3~15~33~6&a=0&gs=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,75,20,5,0;250,50,15,0,0;200,30,15,0,0;125,25,10,0,0;125,25,10,0,0;100,20,5,0,0;100,20,5,0,0;75,15,5,0,0;75,15,5,0,0;75,15,5,0,0;0,0,0,0,0";
            }
        }
	
	
        #endregion
        public DwarvenGoldGameLogic()
        {
            _gameID = GAMEID.DwarvenGold;
            GameName = "DwarvenGold";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                DwarvenGoldResult spinResult = new DwarvenGoldResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                spinResult.ResultString = convertKeyValuesToString(dicParams);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            DwarvenGoldResult result = new DwarvenGoldResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override void onDoFSBonus(int agentID, string strUserID, GITMessage message, double userBalance)
        {

            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = -1;
                if(message.DataNum > 0)
                    ind = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOFSBONUS);
                double realWin      = 0.0;
                string strGameLog   = "";
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    DwarvenGoldResult       result          = _dicUserResultInfos[strGlobalUserID] as DwarvenGoldResult;
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOFSBONUS)
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
                            if (result.BonusSelections == null)
                                result.BonusSelections = new List<int>();

                            if (result.BonusSelections.Contains(ind))
                            {
                                betInfo.pushFrontResponse(actionResponse);
                                saveBetResultInfo(strGlobalUserID);
                                throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                            }
                            result.BonusSelections.Add(ind);
                            if (dicParams.ContainsKey("fsb_status"))
                            {
                                int[] status = new int[5];
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    status[result.BonusSelections[i]] = i + 1;
                                dicParams["fsb_status"] = string.Join(",", status);
                            }
                            if (dicParams.ContainsKey("fsb_wins"))
                            {
                                string[] strWins    = dicParams["fsb_wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                for(int i = 0; i < result.BonusSelections.Count; i++)
                                {
                                    string strTemp                      = strWins[i + 1];
                                    strWins[i + 1]                      = strWins[result.BonusSelections[i]];
                                    strWins[result.BonusSelections[i]]  = strTemp;
                                }
                                dicParams["fsb_wins"] = string.Join(",", strWins);
                            }
                            if (dicParams.ContainsKey("fsb_mm"))
                            {
                                string[] strWins = dicParams["fsb_mm"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                {
                                    string strTemp = strWins[i + 1];
                                    strWins[i + 1] = strWins[result.BonusSelections[i]];
                                    strWins[result.BonusSelections[i]] = strTemp;
                                }
                                dicParams["fsb_mm"] = string.Join(",", strWins);
                            }
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
                _logger.Error("Exception has been occurred in DwarvenGoldGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set", "s" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
    }
}
