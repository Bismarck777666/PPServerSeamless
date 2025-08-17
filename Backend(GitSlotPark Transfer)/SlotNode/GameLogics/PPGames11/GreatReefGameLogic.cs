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
    public class GreatReefResult : BasePPSlotSpinResult
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
    class GreatReefGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25sea";
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
                return "tw=0.00&def_s=8,4,8,12,8,5,7,7,2,5,10,12,5,11,9&msg_code=0&pbalance=0.00&cfgs=1071&reel1=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,8,5,10,9,6,11,9,7,6,5,8,9,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,8,5,10,9,6,11,9,7,6,5,8,9,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,8,5,10,9,6,11,9,7,6,5,8,9,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,8,5,10,9,6,11,9,7,6,5,8,9,7,8,3,3&ver=2&reel0=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,11,9,7,6,5,8,9,2,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,11,9,7,6,5,8,9,2,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,11,9,7,6,5,8,9,2,7,8,3,3,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,11,9,7,6,5,8,9,2,7,8,3,3&reel3=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,10,9,6,12,9,7,6,5,8,9,7,8&reel2=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,12,9,6,11,9,7,6,5,8,9,7,8,3,3,8,7,5,6,9,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,12,9,6,11,9,7,6,5,8,9,7,8,3,3,8,7,5,6,9,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,12,9,6,11,9,7,6,5,8,9,7,8,3,3,8,7,5,6,9,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,12,9,6,11,9,7,6,5,8,9,7,8,3,3,8,7,5,6,9&reel4=12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,9,6,12,9,7,6,5,8,9,7,8,12,7,8,9,3,3,3,7,8,6,9,2,11,6,9,4,10,8,1,6,10,4,7,12,2,11,8,5,9,6,12,9,7,6,5,8,9,7,8&gmb=0,0,0&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&pos=0,0,0,0,0&wilds=2~1,1,1,1,1&bonuses=0&fsbonus=1~100,20,5,2,0~8~2~3~15~33~6&a=0&gs=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,75,20,5,0;250,50,15,0,0;200,30,15,0,0;125,25,10,0,0;125,25,10,0,0;100,20,5,0,0;100,20,5,0,0;75,15,5,0,0;75,15,5,0,0;75,15,5,0,0;0,0,0,0,0";
            }
        }
	
	
        #endregion
        public GreatReefGameLogic()
        {
            _gameID = GAMEID.GreatReef;
            GameName = "GreatReef";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override void onDoFSBonus(int agentID, string strUserID, GITMessage message, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOFSBONUS);
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
                    GreatReefResult         result          = _dicUserResultInfos[strGlobalUserID] as GreatReefResult;
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

                        if (result.BonusSelections == null)
                            result.BonusSelections = new List<int>();

                        string[] strStatuses = dicParams["fsb_status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        bool     isAllZero   = true;
                        for(int i = 0; i < strStatuses.Length; i++)
                        {
                            if (int.Parse(strStatuses[i]) != 0)
                                isAllZero = false;
                        }
                        if(!isAllZero)
                        {
                            int ind = (int) message.Pop();
                            if (result.BonusSelections.Contains(ind))
                            {
                                betInfo.pushFrontResponse(actionResponse);
                                saveBetResultInfo(strGlobalUserID);
                                throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                            }
                            result.BonusSelections.Add(ind);
                            if (dicParams.ContainsKey("fsb_status"))
                            {
                                string[] strParts    = dicParams["fsb_status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                int[]    statusArray = new int[strParts.Length];
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    statusArray[result.BonusSelections[i]] = (i + 1);
                                dicParams["fsb_status"] = string.Join(",", statusArray);
                            }
                            if (dicParams.ContainsKey("fsb_wins"))
                            {
                                string[] strWins    = dicParams["fsb_wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                string[] strNewWins = new string[strWins.Length];
                                for (int i = 0; i < strWins.Length; i++)
                                    strNewWins[i] = "";
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    strNewWins[result.BonusSelections[i]] = strWins[i];

                                int winIndex = result.BonusSelections.Count;
                                for(int i = 0; i < strWins.Length; i++)
                                {
                                    if (strNewWins[i] == "")
                                    {
                                        strNewWins[i] = strWins[winIndex];
                                        winIndex++;
                                    }
                                }
                                dicParams["fsb_wins"] = string.Join(",", strNewWins);
                            }
                            if (dicParams.ContainsKey("fsb_mm"))
                            {
                                string[] strWins = dicParams["fsb_mm"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                string[] strNewWins = new string[strWins.Length];
                                for (int i = 0; i < strWins.Length; i++)
                                    strNewWins[i] = "";
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    strNewWins[result.BonusSelections[i]] = strWins[i];

                                int winIndex = result.BonusSelections.Count;
                                for (int i = 0; i < strWins.Length; i++)
                                {
                                    if (strNewWins[i] == "")
                                    {
                                        strNewWins[i] = strWins[winIndex];
                                        winIndex++;
                                    }
                                }
                                dicParams["fsb_mm"] = string.Join(",", strNewWins);
                            }                           
                        }
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                        string      strResponse = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doFSBonus", strResponse, index, counter);
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
                _logger.Error("Exception has been occurred in GreatReefGameLogic::onDoFSBonus {0}", ex);
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
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            GreatReefResult result = new GreatReefResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                GreatReefResult spinResult = new GreatReefResult();
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
                _logger.Error("Exception has been occurred in GreatReefGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
    }
}
