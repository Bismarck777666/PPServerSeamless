using Akka.Actor;
using Akka.Event;
using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DiamondsOfEgyptBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 88;  }
        }
    }
    public class DiamondsOfEgyptResult : BasePPSlotSpinResult
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
    class DiamondsOfEgyptGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswayseternity";
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
            get { return 20; }
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
                return "def_s=5,12,13,7,10,6,14,6,6,12,7,13,8,5,13&cfgs=7624&ver=3&def_sb=5,12,4,6,10&reel_set_size=3&def_sa=6,14,12,5,11&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"3315650\",jp1:\"1000\",max_rnd_win:\"2500\",jp3:\"20\",jp2:\"100\",jp4:\"10\"}}&wl_i=tbm~2500&sc=3.00,5.00,7.00,10.00,12.00,25.00,35.00,50.00,60.00,90.00,125.00,200.00,300.00,600.00,850.00,1250.00&defc=12.00&wilds=2~0,0,0,0,0~1,1,1,1,1;3~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;680,128,68,0,0;380,88,38,0,0;280,68,28,0,0;180,38,18,0,0;128,28,8,0,0;15,8,5,0,0;15,8,5,0,0;15,8,5,0,0;15,8,5,0,0;15,8,5,0,0;15,8,5,0,0;4400,880,440,0,0&reel_set0=7,6,7,10,14,15,5,5,5,10,5,11,7,8,5,8,8,8,15,11,8,5,14,4,7,6,6,6,13,4,9,7,12,6,8,4,4,4,5,7,6,5,7,4,8,7,7,7,9,8,12,6,5,8,5,6~7,7,6,6,8,7,6,4,4,4,13,2,12,8,5,13,8,7,5,6,6,6,15,2,4,4,7,4,4,13,7,7,7,2,5,6,8,14,7,8,4,11,5,5,5,9,8,6,11,6,10,9,6,8,8,8,9,7,7,10,15,10,8,12~4,9,10,8,7,14,7,6,5,5,6,8,8,15,9,5,6,6,2,14,7,15,8,8,6,15,11,7,7,7,4,9,12,8,13,15,5,12,7,9,8,15,7,7,8,11,8,8,6,7,2,15,6,5,5,7,8,8,8,7,6,12,14,4,4,7,6,8,13,7,7,10,6,13,3,13,3,2,7,4,6,7,6,8,8,5,12,6,6,6,11,8,9,7,6,7,10,7,8,6,8,7,9,7,14,6,13,6,2,5,8,6,11,6,15,4,6,8,10~12,11,4,13,10,9,7,7,7,10,14,2,10,14,7,5,8,8,8,13,9,8,10,4,8,12,5,6,6,6,11,12,14,5,6,14,9,4,14,5,5,5,7,12,6,8,6,7,13,4,4,4,8,5,5,15,8,15,6,6,2,9~7,7,9,8,11,5,8,4,13,5,5,5,6,4,7,4,10,15,7,7,12,4,4,4,12,14,15,11,6,12,13,12,11,4,8,8,8,9,4,5,6,14,12,13,5,9,7,7,7,15,8,4,9,11,6,7,6,10,6,6,6,14,10,12,13,10,5,11,9,13,8,9,10&reel_set2=14,5,5,6,12,5,5,5,7,5,11,7,8,5,8,8,8,9,5,8,7,4,6,8,6,6,6,5,6,4,6,12,5,4,4,4,8,10,14,8,10,7,8,7,7,7,13,6,7,8,7,9,6,7~7,8,11,6,4,4,4,7,4,8,13,9,10,8,6,6,6,4,6,6,8,9,6,8,7,7,7,4,7,13,7,10,5,5,5,12,8,6,6,8,5,2,8,8,8,7,11,14,2,7,8,2,12~8,9,11,5,8,7,8,6,13,4,7,8,6,7,7,6,7,7,14,6,2,6,7,7,2,12,7,4,14,5,7,8,4,14,6,6,7,4,8,7,7,7,6,8,2,9,2,12,8,7,6,9,4,5,11,2,14,5,8,10,6,7,9,5,8,8,2,6,7,2,11,5,2,8,6,14,8,7,6,9,7,8,6,8,8,8,4,8,4,8,3,2,7,12,13,5,7,8,7,10,6,8,7,2,8,7,6,13,2,7,8,6,7,6,4,12,2,8,6,8,7,6,13,5,6,7,6,6,6,11,9,11,8,6,8,7,6,13,5,8,2,8,7,10,8,6,9,3,5,13,2,5,8,7,6,2,10,8,9,2,5,12,6,10,6,8,4,6~8,12,14,7,8,6,6,11,2,7,7,7,4,6,9,5,12,6,7,4,12,2,8,8,8,6,13,10,7,14,8,5,12,8,6,6,6,12,2,7,11,5,13,8,9,13,5,5,5,8,10,9,5,5,10,7,4,7,5,4,4,4,14,2,9,11,14,4,13,9,4,14~9,10,7,6,10,5,13,5,5,5,6,13,11,12,6,12,6,4,4,4,9,11,4,14,7,7,6,8,13,8,8,8,6,14,4,5,4,8,4,9,7,7,7,8,11,7,12,5,10,4,11,6,6,6,9,14,7,11,9,13,10,8,12&reel_set1=5,7,4,8,6,8,7,15,7,7,7,5,7,5,7,7,8,4,4,7,5,5,5,7,8,6,7,8,8,7,5,8,7,6,6,6,5,6,6,7,8,15,6,8,5,4,4,4,7,6,4,6,4,4,8,8,5,6~4,6,6,7,7,7,5,2,8,7,8,8,8,6,15,7,4,8,4,5,5,5,7,8,5,8,8,6,6,6,5,5,6,7,8,4,4,4,8,4,6,6,8,5,4~7,8,4,7,6,8,15,2,6,3,7,5,8,7,6,8,6,8,8,8,7,4,2,7,4,7,6,3,4,5,5,15,8,7,7,6,3,4,6,6,6,15,8,8,7,6,3,8,7,5,7,3,6,5,8,8,6,5,4,5,5,8~5,8,7,5,7,7,7,6,8,8,4,2,8,8,8,7,5,4,6,6,5,4,4,4,7,5,7,8,8,5,5,5,4,7,6,8,8,6,6,6,4,8,6,7,7,15,6~6,6,7,8,4,7,4,4,4,8,4,15,8,4,8,15,6,8,8,8,5,6,5,4,8,7,5,7,7,7,5,7,6,5,7,4,6,8,6,6,6,8,4,7,7,8,6,6,5,5";
            }
        }
	
	
        #endregion
        public DiamondsOfEgyptGameLogic()
        {
            _gameID = GAMEID.DiamondsOfEgypt;
            GameName = "DiamondsOfEgypt";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["bg_0"] != null && gParam["bg_0"]["rw"] != null)
                {
                    gParam["bg_0"]["rw"] = convertWinByBet((string) gParam["bg_0"]["rw"], currentBet);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
            }
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
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                DiamondsOfEgyptBetInfo betInfo  = new DiamondsOfEgyptBetInfo();
                betInfo.BetPerLine              = (float)message.Pop();
                betInfo.LineCount               = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DiamondsOfEgyptGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    oldBetInfo.LineCount  = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DiamondsOfEgyptGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            DiamondsOfEgyptBetInfo betInfo = new DiamondsOfEgyptBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new DiamondsOfEgyptBetInfo();
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                DiamondsOfEgyptResult spinResult     = new DiamondsOfEgyptResult();
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
                _logger.Error("Exception has been occurred in DiamondsOfEgyptGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            DiamondsOfEgyptResult result = new DiamondsOfEgyptResult();
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
                    DiamondsOfEgyptResult   result  = _dicUserResultInfos[strGlobalUserID] as DiamondsOfEgyptResult;
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        betInfo.pushFrontResponse(actionResponse);
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
                        var gParam = JToken.Parse(dicParams["g"]);
                        List<string> chHParams = new List<string>();
                        for (int i = 0; i < result.BonusSelections.Count; i++)
                            chHParams.Add(string.Format("0~{0}", result.BonusSelections[i]));

                        gParam["bg_0"]["ch_h"] = string.Join(",", chHParams.ToArray());

                        int[] status = new int[12];
                        for (int i = 0; i < result.BonusSelections.Count; i++)
                            status[result.BonusSelections[i]] = i + 1;
                        gParam["bg_0"]["status"] = string.Join(",", status);

                        string[] strWins = gParam["bg_0"]["wins"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string[] strNewWins = new string[12];
                        for (int i = 0; i < 12; i++)
                            strNewWins[i] = "0";
                        for (int i = 0; i < result.BonusSelections.Count; i++)
                            strNewWins[result.BonusSelections[i]] = strWins[i];
                        gParam["bg_0"]["wins"] = string.Join(",", strNewWins);

                        string[] strWinsMask = gParam["bg_0"]["wins_mask"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string[] strNewWinsMask = new string[12];
                        for (int i = 0; i < 12; i++)
                            strNewWinsMask[i] = "h";
                        for (int i = 0; i < result.BonusSelections.Count; i++)
                            strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                        gParam["bg_0"]["wins_mask"] = string.Join(",", strNewWinsMask);
                        gParam["bg_0"]["wi"]        = ind.ToString();
                        dicParams["g"]              = serializeJsonSpecial(gParam);
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);

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
                _logger.Error("Exception has been occurred in DiamondsOfEgyptGameLogic::onDoBonus {0}", ex);
            }
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "s", "is", "stf" };
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
