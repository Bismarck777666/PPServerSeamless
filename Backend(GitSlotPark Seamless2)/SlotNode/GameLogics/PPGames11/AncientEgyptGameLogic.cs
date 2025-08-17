using Akka.Actor;
using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class AncientEgyptGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10egypt";
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
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "wsc=1~bg~50,10,1,0,0~0,0,0,0,0~fs~50,10,1,0,0~10,10,10,0,0&def_s=5,8,7,9,8,8,7,3,4,4,11,6,8,11,10&cfgs=1999&ver=2&reel_set_size=10&def_sb=8,8,7,5,9&def_sa=9,1,8,4,10&scatters=&gmb=0,0,0&rt=d&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,1000,100,10,0;2000,400,40,5,0;500,100,15,2,0;500,100,15,2,0;150,40,5,0,0;150,40,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0&reel_set0=9,1,8,4,10,9,7,6,8,7,5,7,10,11,11,8,6,10,8,3~1,8,10,11,4,6,5,3,7,5,11,8,9,10,7,9~7,9,9,10,11,11,4,8,4,6,1,10,5,9,6,10,3,8~7,8,11,9,4,6,10,8,3,7,1,10,9,4,5,6,10,5~8,8,7,5,9,10,6,4,3,1,11,10,1,4,7,6,10,3,8,9&reel_set2=8,7,9,9,10,4,1,4,6,3,7,10,5,11,11~9,11,1,8,10,6,11,4,3,4,7,10,5~10,7,6,10,6,1,5,3,11,4,7,9,11,8~6,8,7,9,11,5,10,10,3,8,1,4,10,11,6,9,5~1,6,9,7,3,10,6,9,11,11,8,7,8,4,5,10&reel_set1=6,7,5,10,7,9,3,3,8,4,10,11,6,7,1~9,3,5,4,8,11,7,6,8,1,10,10~3,11,6,9,8,7,6,10,4,5,7,9,1~8,1,5,9,3,5,7,8,10,10,11,11,6,10,4~8,11,7,6,5,3,1,11,6,4,9,8,3,7,10,10&reel_set4=4,11,7,10,9,7,6,3,10,8,1,5,11,8,8,6~7,8,3,6,1,11,10,11,5,6,10,4,5,7,9~9,7,4,3,6,8,11,1,8,7,10,6,4,7,10,5~7,4,11,10,10,9,7,6,5,3,11,5,1,8~10,8,6,5,11,7,5,6,9,8,8,7,1,10,4,3,10&reel_set3=11,10,8,5,7,7,11,9,6,4,9,1,3~11,10,1,3,7,5,9,6,8,9,4,5~5,7,4,11,8,10,4,11,1,6,7,5,9,3,10~7,3,11,6,9,10,10,9,8,5,1,5,11,4~5,6,3,5,11,8,10,1,10,9,7,7,8,7,4&reel_set6=10,6,8,8,6,10,9,11,7,3,5,9,4,1~8,3,1,4,7,5,8,8,11,9,6,10,11~10,11,3,9,7,8,10,1,6,8,9,10,4,5,4,7~11,8,9,10,1,5,7,5,4,3,5,10,11,10,7,6~7,8,5,3,5,6,4,10,11,10,11,9,1,9,10,6,7&reel_set5=11,7,5,6,6,9,7,6,4,3,7,8,10,10,1,9~11,7,9,1,10,3,8,6,5,4,9,11,11~10,7,6,9,1,7,9,7,4,10,5,4,3,11,11,8~11,7,6,11,8,5,7,1,3,6,4,9,10,10,5,5~10,9,11,7,1,10,8,10,7,6,4,8,5,6,3,9,11,5&reel_set8=1,11,5,8,6,11,9,3,4,9,6,7,8,10,10,9~5,6,9,11,8,3,7,8,1,11,7,4,5,9,6,9,10~5,6,9,7,3,9,11,4,4,11,1,8,10,9~9,11,8,10,11,9,7,5,5,10,4,3,5,6,1,6,10~8,5,4,11,1,7,10,10,5,9,3,6,7&reel_set7=6,10,8,9,4,8,7,11,9,10,9,1,6,5,3,6~8,11,4,10,8,7,3,5,7,1,10,9,8,6,9~11,10,11,6,9,8,3,1,4,10,6,4,5,7,7,9~9,7,6,3,10,8,1,11,4,9,5,7,11,10,5,10,9,6~5,9,9,10,4,7,8,7,5,11,6,3,1,10,11,8&reel_set9=10,4,3,9,8,8,11,11,10,6,5,11,7,6,1~7,8,7,4,6,11,10,8,1,3,10,8,6,5,9,9,10~3,8,11,6,4,9,5,10,7,4,6,11,1~9,10,6,4,11,9,10,8,11,5,3,11,1,7,5,6~7,5,3,10,10,11,8,8,6,9,6,7,4,1,9,5";
            }
        }
	
	
        #endregion
        public AncientEgyptGameLogic()
        {
            _gameID = GAMEID.AncientEgypt;
            GameName = "AncientEgypt";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override async Task onProcMessage(string strUserID, int agentID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOMYSTERYSCATTER)
                onDoMysteryScatter(agentID, strUserID, message, userBonus, userBalance);
            else
                await base.onProcMessage(strUserID, agentID, message, userBonus, userBalance, currency);
        }
        protected void onDoMysteryScatter(int agentID, string strUserID, GITMessage message, UserBonus userBonus, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOMYSTERYSCATTER);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                    if (result.NextAction != ActionTypes.DOMYSTERY)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                        BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                        if (actionResponse.ActionType != ActionTypes.DOMYSTERY)
                        {
                            responseMessage.Append("unlogged");
                        }
                        else
                        {
                            Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                            result.BonusResultString = convertKeyValuesToString(dicParams);
                            addDefaultParams(dicParams, userBalance, index, counter);
                            responseMessage.Append(convertKeyValuesToString(dicParams));

                            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);

                            //히스토리보관 및 초기화
                            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                                addActionHistory(strGlobalUserID, "doMysteryScatter", convertKeyValuesToString(dicParams), index, counter);

                            result.NextAction = nextAction;
                        }
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strGlobalUserID);
                    }

                }
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AncientEgyptGameLogic::onDoMysteryScatter {0}", ex);
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
                    var result                  = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo betInfo   = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                        if (dicParams.ContainsKey("status"))
                        {
                            int ind = (int)message.Pop();
                            int statusLength = getStatusLength(dicParams["status"]);
                            if (dicParams.ContainsKey("status"))
                            {
                                int[] status = new int[statusLength];
                                status[ind] = 1;
                                dicParams["status"] = string.Join(",", status);
                            }
                            if (dicParams.ContainsKey("wins_mask"))
                            {
                                string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    string strTemp = strWinsMask[ind];
                                    strWinsMask[ind] = strWinsMask[0];
                                    strWinsMask[0] = strTemp;
                                }
                                dicParams["wins_mask"] = string.Join(",", strWinsMask);
                            }
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
                _logger.Error("Exception has been occurred in AncientEgyptGameLogic::onDoBonus {0}", ex);
            }
        }

    }
}
