using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;

namespace SlotGamesNode.GameLogics
{
    public class QueenieResult : BasePPSlotSpinResult
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
    public class QueenieGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243queenie";
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
            get
            {
                return 25;
            }
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
                return "def_s=4,8,13,7,10,6,13,5,5,9,7,9,13,4,12&cfgs=5768&ver=2&def_sb=3,8,8,4,11&reel_set_size=2&def_sa=5,10,8,3,10&whi=0&scatters=1~0,0,0,0,0~0,0,0,0,0~0,0,0,0,0&whm=fs,jp,mp,ew,cr,mp,jp,fs,fs,cr,ew,mp,mp,ew,cr,ew,mp,cr,ew,cr&gmb=0,0,0&rt=d&whw=50,0,5,4,1000,20,0,20,10,2000,7,3,100,5,2500,4,50,200,3,750&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"13679891\",jp1:\"4000\",max_rnd_win:\"4200\",jp3:\"50\",jp2:\"200\",jp4:\"20\"}}&wl_i=tbm~4200&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,100,25,0,0;100,30,15,0,0;75,20,10,0,0;50,15,5,0,0;50,15,5,0,0;25,10,2,0,0;25,10,2,0,0;25,10,2,0,0;25,10,2,0,0;25,10,2,0,0;25,10,2,0,0&total_bet_max=10,000,000.00&reel_set0=7,6,4,7,3,3,5,6,4,6,5,6,4,7,7,11,6,6,5,7,7,7,6,6,7,3,9,5,4,5,7,7,6,4,11,4,5,11,8,3,4,7,6,6,6,9,8,12,9,10,11,5,7,3,13,10,11,8,13,5,10,5,5,3,7,7,5~5,7,10,8,4,8,6,4,4,4,7,5,8,7,5,13,9,7,7,7,3,4,7,6,5,12,5,5,3,5,5,5,6,11,6,7,4,9,6,3,3,3,2,7,6,4,13,9,12,6,6,6,5,7,5,3,9,3,12,6,13,7~5,5,3,13,7,7,13,8,5,13,6,7,7,7,2,3,5,12,7,4,10,5,6,10,3,6,6,6,10,9,5,11,13,11,6,11,7,6,6,4,4,5,5,5,6,5,3,4,5,8,12,3,6,7,4,4,4,7,2,10,8,10,6,5,6,7,5,6,3,3,3,6,10,4,7,7,8,5,12,4,5,12,4,7,3~10,13,12,9,2,11,7,12,6,13,4,9,7,11,13,10,6,6,11,7,7,7,8,9,5,8,13,12,10,9,3,13,10,3,3,4,9,8,4,6,10,12,3,3,3,5,7,4,3,7,4,8,2,11,10,9,8,10,7,3,5,11,12,6,4,8~12,3,9,6,5,7,11,12,3,4,11,10,6,13,3,4,8,3,11,13,9,12,4,13,9,7,6,13,10,3,8,10,7,12,4,5,11,8,6&t=243&reel_set1=4,11,6,4,9,6,8,3,7,5,4,9,13,7,7,5,11,5,11,6,5,7,7,7,6,5,10,4,10,7,7,4,8,12,6,5,6,10,8,3,7,6,6,4,7,6,3,5,6,6,6,11,5,4,6,4,5,7,9,7,5,6,7,3,5,5,10,7,3,7,11,8,3,7,3,5~11,4,2,3,5,3,3,4,4,4,12,7,5,6,3,6,6,5,8,7,7,7,10,13,7,3,9,13,12,5,5,5,7,4,9,12,9,7,4,6,5,3,3,3,13,5,4,7,5,8,7,6,6,6,7,6,9,7,5,6,8,4,12,4~9,12,8,7,7,6,5,4,12,5,7,7,7,5,11,5,4,10,3,2,5,6,7,12,6,6,6,5,4,8,4,6,3,6,13,7,4,5,5,5,4,7,7,10,12,13,5,7,10,3,13,4,4,4,10,3,4,3,6,10,6,7,6,3,2,3,3,3,8,11,5,6,8,7,4,13,5,5,6,7~12,9,8,9,5,4,8,13,11,8,3,3,11,9,4,6,9,10,7,7,7,6,12,3,12,9,6,6,13,6,13,7,7,2,10,12,7,4,10,11,8,3,3,3,7,9,7,5,13,5,11,10,3,12,8,4,13,11,10,4,13,3,4,3~12,3,8,6,12,10,13,10,6,10,8,6,5,11,5,3,13,7,13,11,6,7,4,3,5,10,6,7,9,6,12,7,8,9,12,3,11,8,9,10,8,4,13,11,7,3,10,9,12,9,11,12,3,4,11,4,9,4,4&purInit=[{type:\"d\",bet:2500}]&total_bet_min=8.00";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }

        #endregion
        public QueenieGameLogic()
        {
            _gameID = GAMEID.Queenie;
            GameName = "Queenie";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount  = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in QueenieGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
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
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in QueenieGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "5";

        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = -1;
                if(message.DataNum > 0)
                    ind = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double              realWin     = 0.0;
                string              strGameLog  = "";
                ToUserResultMessage resultMsg   = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    QueenieResult           result          = _dicUserResultInfos[strGlobalUserID] as QueenieResult;
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
                            if (dicParams.ContainsKey("ch"))
                                dicParams["ch"] = string.Format("ind~{0}", ind);
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

                            if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID) && !_dicUserFreeSpinInfos[strGlobalUserID].Pending)
                            {
                                resultMsg.FreeSpinID = _dicUserFreeSpinInfos[strGlobalUserID].FreeSpinID;
                                addFreeSpinBonusParams(responseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID, realWin);
                            }
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
                _logger.Error("Exception has been occurred in QueenieGameLogic::onDoBonus {0}", ex);
            }
        }

        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);

            if (dicParams.ContainsKey("wlc_v"))
            {
                string strWlc_v = dicParams["wlc_v"];
                string[] strParts = strWlc_v.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strSubParts.Length >= 2)
                        strSubParts[1] = convertWinByBet(strSubParts[1], currentBet);

                    strParts[i] = string.Join("~", strSubParts);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
            }
        }

        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                QueenieResult spinResult = new QueenieResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (freeSpinInfo != null)
                {
                    
                    dicParams["fra"] = Math.Round(freeSpinInfo.TotalWin, 2).ToString();
                    dicParams["frn"] = freeSpinInfo.RemainCount.ToString();
                    dicParams["frt"] = "N";
                }
                spinResult.ResultString = convertKeyValuesToString(dicParams);

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in QueenieGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            QueenieResult result = new QueenieResult();
            result.SerializeFrom(reader);
            return result;
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
