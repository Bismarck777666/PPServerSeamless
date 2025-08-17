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
    public class BusyBeesResult : BasePPSlotSpinResult
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
    class BusyBeesGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20bl";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "tw=0.00&def_s=4,3,5,6,3,5,6,2,4,3,4,3,3,4,3&msg_code=0&pbalance=0.00&cfgs=925&reel1=9,4,10,5,9,11,8,7,0,10,8,6,11,7,4,0,10,9,8,6,10,9,5,11,10,8,6,5,7,10,9,3,8,7,9,11,2,0,3,2,8,7,11,6,9,4,7,9,2,7,4,10,2,11,1,5,11,9,0,6,3,8,10,11,9,4,10,5,9,11,8,7,0,10,8,6,11,7,4,0,10,9,8,6,10,9,5,11,10,8,6,5,7,10,9,3,8,7,9,11,2,0,3,2,8,7,11,6,9,4,7,9,2,7,4,10,2,11,1,5,11,9,0,6,3,8,10,11&ver=2&reel0=5,6,2,10,9,7,11,6,3,5,0,10,8,0,1,8,11,10,5,11,6,10,9,11,7,6,3,7,6,4,5,7,4,11,7,9,2,11,5,7,10,8,0,10,8,3,11,9,4,7,11,9,3,6,4,9,2,10,1,9,8,10,0,8,5,6,2,10,9,7,11,6,3,5,0,10,8,0,1,8,11,10,5,11,6,10,9,11,7,6,3,7,6,4,5,7,4,11,7,9,2,11,5,7,10,8,0,10,8,3,11,9,4,7,11,9,3,6,4,9,2,10,1,9,8,10,0,8&reel3=8,7,2,9,11,3,10,9,11,10,9,8,10,1,6,11,10,8,9,5,8,10,6,3,8,9,6,7,11,9,7,3,5,7,6,4,3,7,10,5,8,4,5,9,6,11,2,4,5,2,1,9,7,10,11,7,4,11,9,10,8,11,7,6,8,7,2,9,11,3,10,9,11,10,9,8,10,1,6,11,10,8,9,5,8,10,6,3,8,9,6,7,11,9,7,3,5,7,6,4,3,7,10,5,8,4,5,9,6,11,2,4,5,2,1,9,7,10,11,7,4,11,9,10,8,11,7,6&reel2=10,9,6,11,9,3,11,6,7,4,5,9,7,0,6,7,3,8,9,10,0,7,10,2,11,5,2,10,11,9,3,11,10,9,11,8,2,10,7,8,10,5,6,8,9,4,5,10,1,9,0,11,1,8,11,0,8,4,10,11,6,9,5,4,10,9,6,11,9,3,11,6,7,4,5,9,7,0,6,7,3,8,9,10,0,7,10,2,11,5,2,10,11,9,3,11,10,9,11,8,2,10,7,8,10,5,6,8,9,4,5,10,1,9,0,11,1,8,11,0,8,4,10,11,6,9,5,4&reel4=11,6,5,8,7,11,3,8,10,6,3,9,2,4,5,10,4,6,3,8,5,11,10,9,2,11,4,9,6,8,4,1,2,6,9,11,7,5,9,3,6,10,7,9,10,6,4,10,1,11,10,5,3,8,9,7,8,9,11,7,8,11,7,9,11,6,5,8,7,11,3,8,10,6,3,9,2,4,5,10,4,6,3,8,5,11,10,9,2,11,4,9,6,8,4,1,2,6,9,11,7,5,9,3,6,10,7,9,10,6,4,10,1,11,10,5,3,8,9,7,8,9,11,7,8,11,7,9&scatters=1~0,0,0,0,0~12,12,12,0,0~10,5,3,1,1&gmb=0,0,0&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&pos=10,12,18,76,46&wilds=2~1,1,1,1,1&bonuses=0&a=0&gs=0&paytable=0,0,0,0,0;0,0,0,0,0;5000,2000,200,20,0;1000,200,30,0,0;500,150,25,0,0;400,100,20,0,0;300,80,15,0,0;200,50,10,0,0;200,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0";
            }
        }
	
	
        #endregion
        public BusyBeesGameLogic()
        {
            _gameID = GAMEID.BusyBees;
            GameName = "BusyBees";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                BusyBeesResult spinResult = new BusyBeesResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

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
                _logger.Error("Exception has been occurred in BasePPSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            BusyBeesResult result = new BusyBeesResult();
            result.SerializeFrom(reader);
            return result;
        }
        private bool isAllStatusZero(string strStatus)
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
                int ind     = -1;
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin          = 0.0;
                string strGameLog       = "";
                string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    var result = _dicUserResultInfos[strGlobalUserID] as BusyBeesResult;
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                        if (dicParams.ContainsKey("status"))
                        {
                            if (!isAllStatusZero(dicParams["status"]))
                            {
                                ind = (int)message.Pop();
                                if (result.BonusSelections == null)
                                    result.BonusSelections = new List<int>();

                                if (result.BonusSelections.Contains(ind))
                                {
                                    betInfo.pushFrontResponse(actionResponse);
                                    saveBetResultInfo(strGlobalUserID);
                                    throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                                }
                                int level = int.Parse(dicParams["level"]);
                                List<int> availablePositions = new List<int>();
                                string[]  strMarkers         = dicParams["markers"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                for(int i = 0; i < strMarkers.Length; i++)
                                {
                                    if (strMarkers[i] == level.ToString())
                                        availablePositions.Add(i);
                                }
                                if(!availablePositions.Contains(ind))
                                    throw new Exception("User selected wrong position, Malicious Behavior");

                                result.BonusSelections.Add(ind);
                                if (dicParams.ContainsKey("status"))
                                {
                                    int[] status = new int[21];
                                    string[] strStatusArray = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                                    for (int i = 0; i < 21; i++)
                                    {
                                        status[i] = int.Parse(strStatusArray[i]);
                                        if (status[i] == 1)
                                            status[i] = 2;
                                    }
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        status[result.BonusSelections[i]] = 1;
                                    dicParams["status"] = string.Join(",", status);
                                }
                                if (dicParams.ContainsKey("wins"))
                                {
                                    string[] strWins    = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);                                 
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                    {
                                        int selectedPos = result.BonusSelections[i];
                                        int firstPos    = 0;
                                        for (int j = 0; j < strMarkers.Length; j++)
                                        {
                                            if (strMarkers[j] == (i + 1).ToString())
                                            {
                                                firstPos = j;
                                                break;
                                            }
                                        }
                                        if (selectedPos != firstPos)
                                        {
                                            string strTemp = strWins[selectedPos];
                                            strWins[selectedPos] = strWins[firstPos];
                                            strWins[firstPos] = strTemp;
                                        }
                                    }
                                    dicParams["wins"] = string.Join(",", strWins);
                                }                                
                            }
                        }

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
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
                _logger.Error("Exception has been occurred in BusyBeesGameLogic::onDoBonus {0}", ex);
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

    }
}
