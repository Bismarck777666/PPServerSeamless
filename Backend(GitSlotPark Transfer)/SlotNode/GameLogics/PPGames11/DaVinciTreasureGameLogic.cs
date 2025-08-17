using Akka.Actor;
using Akka.Event;
using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DaVinciTreasureGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25davinci";
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
                return "def_s=3,8,10,6,3,3,1,12,5,3,3,10,4,9,3&cfgs=1773&ver=2&reel_set_size=2&def_sb=10,11,8,1,7&def_sa=8,3,2,3,13&prg_cfg_m=wm&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&prg_cfg=0&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;800,200,50,10,0;500,150,30,5,0;500,150,30,5,0;300,100,20,0,0;300,100,20,0,0;150,15,5,0,0;150,15,5,0,0;150,15,5,0,0;100,10,5,0,0;100,10,5,0,0;100,10,5,0,0&reel_set0=4,13,13,10,11,8,10,13,6,6,6,11,8,7,11,12,8,12,6,5,11,13,10,4,12,9,3,3,3~11,4,7,10,10,2,12,6,8,12,11,13,8,5,9,1,10,3,3,3~13,13,11,13,11,9,5,10,7,1,9,1,11,8,9,13,6,9,10,2,9,12,4,11,10,10,4,9,1,11,2,6,3,3,3~9,12,9,5,8,12,7,13,10,11,1,12,8,9,2,10,12,8,7,6,1,4,5,9,3,3,3~5,5,13,10,11,8,9,11,12,6,13,2,4,7,7,4,13,3,3,3&reel_set1=6,11,7,5,4,4,8,10,13,9,6,12,3,3,3,3,3~9,8,8,5,12,2,12,1,7,11,2,7,10,10,13,4,6,5,1,3,3,3,3,3~4,2,11,13,11,10,5,13,9,5,8,13,5,2,1,6,13,11,9,9,11,7,12,3,3,3,3,3~13,12,11,1,11,5,12,10,5,8,7,13,10,9,5,10,11,6,4,1,2,3,3,3,3,3~9,7,7,6,13,13,9,13,8,7,4,5,10,10,12,2,4,5,11,4,3,3,3,3,3";
            }
        }
	
	
        #endregion
        public DaVinciTreasureGameLogic()
        {
            _gameID = GAMEID.DaVinciTreasure;
            GameName = "DaVinciTreasure";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("g_ra"))
                dicParams["g_ra"] = convertWinByBet(dicParams["g_ra"], currentBet);            
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

            if(resultParams.ContainsKey("g_o") || resultParams.ContainsKey("g_l"))
            {
                resultParams["go_i_mask"] = "m,m,m,m";
                resultParams["go_i"]      = "2,3,4,5";
                resultParams["g_t"]       = "multiplier";
            }

            return resultParams;
        }

        protected override void onDoCollectBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();

                GITMessage  responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECTBONUS);
                string      strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo    betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult result  = _dicUserResultInfos[strGlobalUserID];
                    if(result.NextAction != ActionTypes.DOCOLLECTBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        if (betInfo.HasRemainResponse)
                            betInfo.pullRemainResponse();
                        Dictionary<string, string> dicLastParams = splitResponseToParams(result.BonusResultString);

                        Dictionary<string, string> dicParams = new Dictionary<string, string>();
                        dicParams["coef"] = Math.Round(betInfo.TotalBet, 2).ToString();
                        dicParams["na"]   = "s";
                        if (dicLastParams.ContainsKey("g_w"))
                            dicParams["rw"] = dicLastParams["g_w"];
                        else
                            dicParams["rw"] = dicLastParams["g_ra"];
                        dicParams["wp"] = "0";
                        addDefaultParams(dicParams, userBalance, index, counter);
                        responseMessage.Append(convertKeyValuesToString(dicParams));
                        result.NextAction = convertStringToActionType(dicParams["na"]);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                        {
                            addActionHistory(strGlobalUserID, "doCollectBonus", convertKeyValuesToString(dicParams), index, counter);
                            saveHistory(agentID, strUserID, index, counter, userBalance, currency);
                        }

                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strGlobalUserID);
                    }
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoBonus {0}", ex);
            }
        }
        protected override void onDoGambleOption(int agentID, string strUserID, GITMessage message, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
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
                    var result                              = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo betInfo               = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse   = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOGAMBLEOPTION)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        int gOption = (int) message.Pop();
                        if(gOption >= 0)
                        {
                            dicParams["g_o"] = gOption.ToString();
                            dicParams["na"]  = "g";
                        }                      
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse    = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addIndActionHistory(strGlobalUserID, "doGambleOption", strResponse, index, counter, gOption);

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
                    betInfo.pullRemainResponse();
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
                _logger.Error("Exception has been occurred in DaVinciTreasure::onDoGambleOption {0}", ex);
            }
        }

        protected override async Task onDoGamble(int agentID, string strUserID, GITMessage message, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOGAMBLE);
                double realWin = 0.0;
                string strGameLog = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    var result                              = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo           betInfo     = _dicUserBetInfos[strGlobalUserID];
                    Dictionary<string, string>  dicParams   = splitResponseToParams(result.BonusResultString);
                    if (dicParams["na"] != "g")
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        int gambleIndex     = (int)message.Pop();
                        int boxCount        = int.Parse(dicParams["g_o"]) + 2;
                        double payoutRate   = getPayoutRate(agentID);
                        if (gambleIndex == -1)
                        {
                            dicParams["na"] = "c";
                        }
                        else
                        {
                            double gambleAmount = double.Parse(dicParams["g_ra"]);
                            double totalWin     = double.Parse(dicParams["tw"]);
                            int randomIndex     = Pcg.Default.Next(0, boxCount);
                            if (randomIndex == gambleIndex && Pcg.Default.NextDouble(0, 100) <= payoutRate && 
                                (await checkWebsitePayoutRate(agentID, gambleAmount, boxCount * gambleAmount, 1)))
                            {
                                dicParams["g_l"]   = "1";
                                dicParams["g_wi"]  = gambleIndex.ToString();
                                dicParams["g_r"]   = "1";
                                dicParams["g_w"]   = Math.Round(gambleAmount * boxCount, 2).ToString();
                                dicParams["g_si"]  = gambleIndex.ToString();
                                dicParams["g_mul"] = boxCount.ToString();
                                dicParams["g_end"] = "1";
                                dicParams["tw"]    = Math.Round(totalWin - gambleAmount + gambleAmount * boxCount, 2).ToString();
                                dicParams["na"]    = "cb";
                            }
                            else
                            {
                                if(randomIndex == gambleIndex)
                                {
                                    List<int> candidates = new List<int>();
                                    for(int i = 0; i < boxCount; i++)
                                    {
                                        if (i != gambleIndex)
                                            candidates.Add(i);
                                    }
                                    randomIndex = candidates[Pcg.Default.Next(0, candidates.Count)];
                                }
                                dicParams["g_l"]   = "1";
                                dicParams["g_wi"]  = randomIndex.ToString();
                                dicParams["g_r"]   = "0";
                                dicParams["g_si"]  = gambleIndex.ToString();
                                dicParams["g_w"]   = "0.0";
                                dicParams["g_mul"] = boxCount.ToString();
                                dicParams["g_end"] = "1";
                                dicParams["tw"]    = Math.Round(totalWin - gambleAmount, 2).ToString();
                                dicParams["na"]    = "c";
                                    
                                sumUpWebsiteBetWin(agentID, gambleAmount, 0.0, 1);
                            }
                        }

                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                        string strResponse      = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doGamble", strResponse, index, counter);

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
                _logger.Error("Exception has been occurred in DaVinciTreasure::onDoGamble {0}", ex);
            }
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = -1;
                if (message.DataNum >= 1)
                    ind = (int)message.Pop();

                string      strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage  responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
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
                    var result                              = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo betInfo               = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse   = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if(dicParams.ContainsKey("bgt") && dicParams.ContainsKey("status") && dicParams["bgt"] == "9")
                        {
                            string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if(strParts.Length == 3 && !(strParts[0] == "0" && strParts[1] == "0" && strParts[2] == "0"))
                            {
                                string strTemp = "";
                                if(ind != 0)
                                {
                                    strTemp       = strParts[0];
                                    strParts[0]   = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["status"] = string.Join(",", strParts);
                                }
                                strParts = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if(ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["wins"] = string.Join(",", strParts);
                                }
                                strParts = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["wins_mask"] = string.Join(",", strParts);
                                }
                            }
                        }                       
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction   = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);
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
                _logger.Error("Exception has been occurred in DaVinciTreasure::onDoBonus {0}", ex);
            }
        }
    }
}
