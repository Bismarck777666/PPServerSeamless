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
    class SevenPiggiesResult : BasePPSlotSpinResult
    {
        public List<int>    BonusSelections { get; set; }
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
    class SevenPiggiesGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs7pigs";
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
            get { return 7; }
        }
        protected override int ServerResLineCount
        {
            get { return 7; }
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
                return "def_s=3,4,4,2,5,3,4,4,2,5,3,4,4,2,5&cfgs=2237&ver=2&reel_set_size=2&def_sb=6,6,6,6,6&def_sa=4,4,4,4,4&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=30.00,60.00,90.00,120.00,150.00,300.00,450.00,600.00,750.00,1000.00,1500.00,2000.00,3500.00,7000.00,10500.00,15000.00&defc=150.00&wilds=2~1500,400,50,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;150,50,20,0,0;50,15,5,0,0;50,15,5,0,0;15,5,3,0,0;15,5,3,0,0&reel_set0=7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5,1,7,7,7,7,7,7,7,7,4,4,4,4,4,1,6,6,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5,2,2,2,2,2~7,7,7,7,7,7,7,1,4,4,4,4,4,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,6,6,6,6,4,4,4,4,4,7,7,7,7,7,2,2,2,2,2,4,4,4,4,4,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5~7,7,7,7,7,4,4,4,4,6,6,6,3,3,3,5,5,5,5,5,1,7,7,7,7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,6,6,6,6,4,4,4,4,4,7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,4,4,4,4,4,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5~7,7,7,7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,3,3,3,3,5,5,5,5,5,1,7,7,7,7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,4,4,4,4,7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,4,4,4,4,4,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5~7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5,1,7,7,7,7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,6,6,6,6,4,4,4,4,4,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5,2,2,2,2,2&reel_set1=7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5,1,7,7,7,7,7,7,7,7,4,4,4,4,4,1,6,6,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5,2,2,2,2,2~7,7,7,7,7,7,7,1,4,4,4,4,4,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,6,6,6,6,4,4,4,4,4,7,7,7,7,7,2,2,2,2,2,4,4,4,4,4,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5~7,7,7,7,7,4,4,4,4,6,6,6,3,3,3,5,5,5,5,5,1,7,7,7,7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,6,6,6,6,4,4,4,4,4,7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,4,4,4,4,4,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5~7,7,7,7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,3,3,3,3,5,5,5,5,5,1,7,7,7,7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,4,4,4,4,7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,4,4,4,4,4,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5~7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5,1,7,7,7,7,7,7,7,7,4,4,4,4,4,6,6,6,6,6,6,6,6,5,5,5,5,5,1,7,7,7,7,7,7,7,7,3,3,3,3,5,5,5,5,5,1,6,6,6,6,6,6,6,6,4,4,4,4,4,1,6,6,6,6,6,6,6,6,3,3,3,3,5,5,5,5,5,2,2,2,2,2";
            }
        }
	
	
        #endregion
        public SevenPiggiesGameLogic()
        {
            _gameID = GAMEID.SevenPiggies;
            GameName = "SevenPiggies";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                SevenPiggiesResult spinResult = new SevenPiggiesResult();
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
                _logger.Error("Exception has been occurred in SevenPiggiesGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            SevenPiggiesResult result = new SevenPiggiesResult();
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
                    SevenPiggiesResult result  = _dicUserResultInfos[strGlobalUserID] as SevenPiggiesResult;
                    BasePPSlotBetInfo  betInfo = _dicUserBetInfos[strGlobalUserID];
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

                        Dictionary<string, string> dicLastResultParams = splitResponseToParams(result.ResultString);
                        if (!dicLastResultParams.ContainsKey("i_pos"))
                            throw new Exception("Last Result doesn't have scatter positions");
                        string [] strPositions = dicLastResultParams["i_pos"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        int selectCount = strPositions.Length;
                        int realIndex   = 0;
                        for(int i = 0; i < strPositions.Length; i++)
                        {
                            if (int.Parse(strPositions[i]) == ind)
                            {
                                realIndex = i;
                                break;
                            }
                        }
                        if (result.BonusSelections.Contains(realIndex))
                        {
                            betInfo.pushFrontResponse(actionResponse);
                            saveBetResultInfo(strGlobalUserID);
                            throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                        }

                        result.BonusSelections.Add(realIndex);
                        if (dicParams.ContainsKey("status"))
                        {
                            int[] status = new int[selectCount];
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                status[result.BonusSelections[i]] = i + 1;
                            dicParams["status"] = string.Join(",", status);
                        }
                        if (dicParams.ContainsKey("wins"))
                        {
                            string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strNewWins = new string[selectCount];
                            for (int i = 0; i < selectCount; i++)
                                strNewWins[i] = "0";
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                strNewWins[result.BonusSelections[i]] = strWins[i];
                            dicParams["wins"] = string.Join(",", strNewWins);
                        }
                        if (dicParams.ContainsKey("wins_mask"))
                        {
                            string[] strWins = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strNewWins = new string[selectCount];
                            for (int i = 0; i < selectCount; i++)
                                strNewWins[i] = "m";
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                strNewWins[result.BonusSelections[i]] = strWins[i];
                            dicParams["wins_mask"] = string.Join(",", strNewWins);
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
                _logger.Error("Exception has been occurred in SevenPiggiesGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set", "gsf_r", "gsf", "i_pos" };
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
