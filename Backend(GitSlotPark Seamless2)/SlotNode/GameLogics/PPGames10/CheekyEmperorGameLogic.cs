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
    class CheekyEmperorBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return BetPerLine * 88.0f; }
        }
    }
    public class CheekyEmperorResult : BasePPSlotSpinResult
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
    class CheekyEmperorGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243ckemp";
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
            get { return 88; }
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
                return "def_s=5,10,11,3,13,7,9,7,5,10,4,8,4,6,13&bgid=0&cfgs=6382&ver=2&def_sb=3,9,5,4,11&reel_set_size=3&def_sa=3,13,4,4,12&bonusInit=[{bgid:0,bgt:18,bg_i:\"1000,100,20,10\",bg_i_mask:\"pw,pw,pw,pw\"}]&scatters=&gmb=0,0,0&bg_i=1000,100,20,10&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"446428\",max_rnd_win:\"2500\"}}&wl_i=tbm~2500&bgt=18&sc=3.00,5.00,7.00,10.00,12.00,25.00,35.00,50.00,60.00,90.00,125.00,200.00,300.00,600.00,850.00,1250.00&defc=12.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&bg_i_mask=pw,pw,pw,pw&paytable=0,0,0,0,0;4400,880,440,0,0;0,0,0,0,0;1000,200,100,0,0;500,100,50,0,0;400,80,40,0,0;250,50,25,0,0;100,20,10,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0&reel_set0=10,7,6,6,5,4,5,3,4,4,4,1,7,6,12,3,9,13,4,4,7,7,7,9,11,7,7,6,6,8,4,5,5,5,9,3,12,7,10,1,3,4,6,3,3,3,4,7,11,6,5,6,4,7,5,6,6,6,13,5,5,7,4,5,7,6,5,8~10,5,13,6,5,9,1,7,3,3,3,7,6,5,7,5,4,6,7,4,5,5,5,3,7,5,11,7,6,12,7,5,6,6,6,1,8,6,6,7,3,8,11,3,5,4,4,4,5,6,7,9,8,6,3,3,8,7,7,7,5,7,10,4,12,2,6,13,5,2,6~7,4,3,11,1,5,5,6,6,6,2,4,5,7,7,6,5,9,5,4,4,4,1,8,5,6,8,3,6,7,12,5,5,5,4,7,3,11,13,5,6,12,7,7,7,6,4,5,6,6,2,8,4,13,3,3,3,1,6,8,7,9,7,6,7,10,5~4,10,7,13,5,2,7,6,6,6,2,5,1,4,8,6,6,11,8,7,7,7,6,5,9,1,12,11,5,13,5,5,5,12,11,13,4,4,7,8,13,3,4,4,4,9,7,10,13,9,6,4,8,3,3,3,6,5,9,12,3,3,6,12,7,9~4,13,8,3,9,3,11,7,4,4,4,1,12,11,13,6,9,4,5,11,6,3,3,3,13,6,5,5,7,12,7,13,3,9,7,7,7,8,1,5,6,10,7,10,11,12,7,6,6,6,8,6,9,10,7,12,4,7,4,5,5,5,4,8,7,7,10,8,5,4,3,4,3&reel_set2=6,3,3,4,4,7,5,7,7,7,5,7,6,6,5,6,7,7,4,4,4,6,5,4,6,6,5,4,6,4,5,5,5,3,5,6,6,4,3,6,5,3,6,6,6,4,7,7,5,3,1,6,6,4,3,3,3,1,5,7,6,3,7,6,6,7,6~5,5,6,2,7,5,6,7,7,7,6,2,3,3,5,5,6,5,6,6,6,4,6,5,6,6,4,6,4,4,4,7,6,7,5,5,2,7,6,5,5,5,4,7,4,7,6,7,3,3,3,1,6,4,6,4,7,6,4,7,7~4,4,5,6,7,7,7,6,2,6,7,6,6,6,4,5,6,3,5,5,5,7,1,6,5,2,4,4,4,7,6,6,7,5,3,3,3,2,6,4,3,5,7~7,6,5,5,7,7,7,3,4,4,6,1,6,6,6,3,6,5,3,6,3,3,3,6,4,4,3,3,4,4,4,2,7,5,5,2,6,5,5,5,4,7,7,4,6,5,7~3,6,7,3,4,5,6,3,3,3,5,7,3,3,1,6,5,7,5,6,6,6,5,3,3,6,5,3,6,6,7,7,7,5,6,7,5,7,7,3,5,5,5,3,4,7,6,3,3,7,6,6,5&t=243&reel_set1=4,6,7,3,5,6,7,7,7,3,7,6,3,5,6,5,4,4,4,6,1,6,7,7,6,7,7,5,5,5,3,5,6,1,7,3,4,5,6,6,6,7,7,6,4,6,4,5,3,3,3,6,5,6,4,4,6,6,3,5~2,6,5,4,6,6,7,6,7,7,7,4,6,4,4,6,7,6,1,4,6,6,6,7,6,5,5,7,6,4,7,5,4,4,4,3,3,5,3,6,5,6,7,4,5,5,5,6,5,4,6,6,5,2,5,3,3,3,7,7,4,6,6,7,2,7,7,5,4~6,5,3,3,7,6,6,2,7,7,7,6,4,6,4,6,3,6,6,7,6,6,6,4,7,6,2,6,4,7,5,7,2,5,5,5,4,5,6,7,4,6,7,7,5,1,4,4,4,3,5,7,6,5,6,5,7,7,6,3,3,3,5,4,2,6,6,5,7,6,2,6,5~4,5,5,6,5,7,3,7,7,7,5,3,4,4,6,2,4,2,4,6,6,6,2,1,6,6,7,6,3,4,6,3,3,3,6,4,6,7,6,4,7,7,3,4,4,4,5,4,5,7,7,6,3,3,6,5,5,5,6,4,6,3,5,3,5,4,7,5~3,5,7,3,5,6,6,3,1,3,3,3,6,7,3,5,5,7,3,7,3,5,5,6,6,6,3,7,3,6,6,1,3,6,3,7,7,7,6,7,6,6,7,3,7,5,4,5,5,5,6,7,3,6,7,5,7,4,7,5,4,3";
            }
        }
	
	
        #endregion
        public CheekyEmperorGameLogic()
        {
            _gameID = GAMEID.CheekyEmperor;
            GameName = "CheekyEmperor";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
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

        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                CheekyEmperorResult spinResult = new CheekyEmperorResult();
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
                _logger.Error("Exception has been occurred in CheekyEmperorGameLogic::calculateResult {0}", ex);
                return null;
            }
        }

        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            CheekyEmperorResult result = new CheekyEmperorResult();
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
                    CheekyEmperorResult result  = _dicUserResultInfos[strGlobalUserID] as CheekyEmperorResult;
                    BasePPSlotBetInfo   betInfo = _dicUserBetInfos[strGlobalUserID];
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
                _logger.Error("Exception has been occurred in CheekyEmperorGameLogic::onDoBonus {0}", ex);
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
            CheekyEmperorBetInfo betInfo = new CheekyEmperorBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                CheekyEmperorBetInfo betInfo = new CheekyEmperorBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in CheekyEmperorGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in CheekyEmperorGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
