using Akka.Actor;
using Akka.Event;
using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class Devils13GameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs13g";
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
            get { return 13; }
        }
        protected override int ServerResLineCount
        {
            get { return 13; }
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
                return "tw=0.00&def_s=4,3,5,6,3,5,6,2,4,3,4,3,3,4,3&msg_code=0&pbalance=0.00&cfgs=181&reel1=7,4,9,7,8,10,6,7,9,0,4,10,7,8,5,9,6,10,7,0,9,8,7,10,6,7,8,9,4,0,10,7,8,6,9,10,4,8,7,9,6,10,5,8,10,2,6,10,5,9,10,8,6,10,7,9,3,8,5,9,10,4,9,3,10,5,2,7,9,5,4,10,8,5,9,8,4,0,6,7,5,9,7,10,8,4,6,9,7,8,3,0,10,9,6,10,9,8,10,3,7,10,5,9,10,6,8,10,5,0,7,9,4,10,8,0,9,8,5,9,8,6,9,7,3,10,6,5&ver=2&reel0=10,1,9,7,8,10,6,7,9,1,8,10,7,8,5,9,6,10,0,5,9,8,7,10,6,7,8,9,4,5,10,7,8,6,9,10,4,8,9,5,7,10,0,8,9,7,2,10,1,0,4,8,6,10,7,1,10,8,7,5,10,4,9,0,6,10,4,7,9,5,3,10,8,4,9,1,4,0,8,1,3,9,7,10,8,4,6,9,1,5,10,8,7,9,6,10,9,8,2,5,9,10,5,1,10,6,8,10,1,5,2,9,4,10,8,1,9,7,3,10,8,6,9,7,3,10,6,0&reel3=8,7,9,0,5,10,6,7,9,0,4,10,7,8,5,9,6,10,4,7,9,8,7,10,5,6,8,9,4,6,10,7,8,6,9,10,4,8,7,9,2,10,5,8,10,7,6,10,8,0,10,8,6,10,7,9,3,8,5,6,10,4,9,3,7,5,4,7,9,0,7,10,8,5,9,8,4,10,6,4,5,9,7,10,8,4,6,9,4,8,5,2,7,9,6,10,9,8,5,0,8,10,4,9,10,6,8,4,5,0,7,9,4,10,8,5,9,7,3,9,4,6,9,7,3,10,6,5&reel2=8,2,9,1,0,10,6,7,1,0,4,10,7,1,5,9,6,10,1,5,9,8,7,10,5,1,8,9,4,1,10,7,8,1,7,10,4,8,7,9,2,10,9,8,10,7,6,10,8,1,10,8,6,10,7,9,0,8,2,7,1,4,9,3,10,5,8,7,9,5,4,10,8,5,9,7,4,10,6,4,5,9,7,10,8,4,6,9,3,8,1,0,7,9,4,10,6,8,9,0,7,10,5,9,3,6,8,10,6,0,7,9,6,10,8,5,9,7,3,9,8,6,9,7,3,10,6,5&reel4=7,4,9,7,8,1,6,7,10,5,4,10,7,8,5,9,6,10,4,5,9,1,7,8,5,6,8,9,4,5,10,7,8,6,9,10,4,8,10,3,7,10,9,8,10,7,6,10,1,5,2,8,6,10,7,9,3,8,10,5,0,2,9,7,0,10,8,7,9,5,0,10,8,1,9,8,4,10,6,5,3,9,7,10,8,1,6,9,0,1,10,8,7,9,6,10,2,8,1,5,7,10,0,9,3,6,8,4,9,1,7,9,4,10,8,1,9,7,6,5,8,6,9,7,3,10,6,9&scatters=1~0,0,0,0,0~0,0,13,0,0~1,1,1,1,1&gmb=0,0,0&sc=15.00,30.00,45.00,60.00,75.00,150.00,225.00,300.00,375.00,600.00,750.00,1200.00,2000.00,4000.00,6000.00,8000.00&defc=75.00&pos=10,12,21,16,20&wilds=2~1,1,1,1,1&bonuses=0&a=0&gs=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;13000,1300,130,13,0;1000,200,50,5,0;500,150,30,4,0;300,100,25,2,0;200,60,20,0,0;150,50,15,0,0;100,40,10,0,0;80,30,8,0,0";
            }
        }
	
	
        #endregion
        public Devils13GameLogic()
        {
            _gameID = GAMEID.Devils13;
            GameName = "Devils13";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int     index           = (int)message.Pop();
                int     counter         = (int)message.Pop();
                string  strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

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
                    var result = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
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

                        string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        bool isAllZero = true;
                        for(int i = 0; i < strParts.Length;i++)
                        {
                            if (int.Parse(strParts[0]) != 0)
                                isAllZero = false;
                        }
                        if(!isAllZero)
                        {
                            int ind = (int)message.Pop();
                            if (dicParams.ContainsKey("status"))
                            {
                                var strStatuses = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    string strTemp = strStatuses[0];
                                    strStatuses[0] = strStatuses[ind];
                                    strStatuses[ind] = strTemp;
                                }
                                dicParams["status"] = string.Join(",", strStatuses);
                            }
                            if (dicParams.ContainsKey("wins"))
                            {
                                var strStatuses = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    string strTemp = strStatuses[0];
                                    strStatuses[0] = strStatuses[ind];
                                    strStatuses[ind] = strTemp;
                                }
                                dicParams["wins"] = string.Join(",", strStatuses);
                            }
                            if (dicParams.ContainsKey("i_wins"))
                            {
                                var strStatuses = dicParams["i_wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    string strTemp = strStatuses[0];
                                    strStatuses[0] = strStatuses[ind];
                                    strStatuses[ind] = strTemp;
                                }
                                dicParams["i_wins"] = string.Join(",", strStatuses);
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
                _logger.Error("Exception has been occurred in Devils13GameLogic::onDoBonus {0}", ex);
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

    }
}
