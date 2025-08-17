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
    class AladdinsTreasureGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs50amt";
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
            get { return 50; }
        }
        protected override int ServerResLineCount
        {
            get { return 50; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "tw=0.00&def_s=3,8,4,8,1,10,6,10,5,7,8,9,6,9,8,7,4,5,3,2&msg_code=0&pbalance=0.00&cfgs=1072&reel1=8,4,7,10,0,6,10,5,4,6,7,8,9,4,6,7,10,1,6,5,3,8,9,7,3,5,10,0,9,7,8,4,5,9,4,10,8,6,9,5,3,8,9,10,6,3,8,10,5,7,8,9,1,7,8,9,7,3,9,6,4,7,5,6,10,5,8,10,7,8,10,5,2,2,2,2,9,8,7,10,8,9,7,4,9,6,10&ver=2&reel0=8,7,5,3,10,8,3,6,5,9,4,6,8,7,10,8,3,5,9,10,8,1,7,8,5,9,3,7,6,10,7,5,4,6,10,0,9,6,4,5,10,3,9,6,5,8,9,6,7,4,9,5,6,7,1,9,5,10,4,7,10,8,9,7,4,8,10,9,6,7,0,8,7,9,10,4,8,6,10&reel3=4,6,7,8,4,10,8,7,5,8,7,10,9,4,6,8,4,10,7,9,8,4,7,5,8,7,0,9,8,10,6,5,10,8,9,10,1,5,10,9,5,0,9,10,7,1,5,6,3,10,5,0,9,8,7,9,3,8,7,6,3,5,7,9,6,3,9,7,10,2,2,2,2,6,5,9,4,10,6,9,8,7,3,6,5,8,10&reel2=5,10,4,8,7,6,4,9,5,8,3,7,8,0,5,10,4,7,6,9,8,4,10,7,0,8,10,4,7,9,8,5,6,4,7,8,3,7,6,8,10,9,6,1,10,7,6,3,9,6,7,5,1,9,6,10,7,9,3,10,7,5,9,0,10,6,8,3,7,8,5,2,2,2,2,8,6,3,5,9,4,10,7,4,9,8&reel4=7,4,6,7,9,10,4,5,7,9,8,4,7,6,10,7,8,3,9,8,10,7,6,5,7,10,8,5,3,7,9,5,8,4,10,5,3,9,7,4,5,9,3,8,10,0,6,8,5,6,7,1,9,5,8,10,9,7,6,0,10,6,7,5,6,0,8,10,4,6,9,7,10,9,8,3,5,6,9,4,6,8,4,10&scatters=1~0,0,0,0,0~50,30,15,0,0~2,2,2,1,1&gmb=0,0,0&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&pos=0,0,0,0,0&wilds=2~1,1,1,1,1&bonuses=0&a=0&gs=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,250,100,0,0;500,150,50,0,0;250,100,30,0,0;150,75,25,0,0;80,45,15,0,0;80,45,15,0,0;50,30,10,0,0;50,30,10,0,0";
            }
        }
	
	
        #endregion
        public AladdinsTreasureGameLogic()
        {
            _gameID = GAMEID.AladdinsTreasure;
            GameName = "AladdinsTreasure";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = -1;
                if(message.DataNum > 0)
                    ind = (int) message.Pop();

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin = 0.0;
                string strGameLog = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotSpinResult    result          = _dicUserResultInfos[strGlobalUserID];
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
                            int         level           = int.Parse(dicParams["level"]);
                            string[]    strMarkers      = dicParams["markers"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            List<int>   availPositions  = new List<int>();
                            for(int i = 0; i < strMarkers.Length; i++)
                            {
                                if (strMarkers[i] == level.ToString())
                                    availPositions.Add(i);
                            }
                            if(availPositions.Contains(ind))
                            {
                                var lastBonusParams = splitResponseToParams(result.BonusResultString);
                                var lastWP          = int.Parse(lastBonusParams["wp"]);
                                var currentWP       = int.Parse(dicParams["wp"]);
                                int wp              = currentWP - lastWP;

                                string[]  strWins   = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                List<int> rightInds = new List<int>();
                                for (int i = 0; i < availPositions.Count; i++)
                                {
                                    if (int.Parse(strWins[availPositions[i]]) == wp)
                                        rightInds.Add(availPositions[i]);
                                }
                                if(!rightInds.Contains(ind))
                                {
                                    int randPos      = rightInds[Pcg.Default.Next(0, rightInds.Count)];
                                    string strTemp   = strWins[ind];
                                    strWins[ind]     = strWins[randPos];
                                    strWins[randPos] = strTemp;
                                    dicParams["wins"] = string.Join(",", strWins);

                                    string[] strStatuses = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    int[]    statues     = new int[strStatuses.Length];
                                    if (level == 1)
                                    {
                                        for (int i = 0; i < strStatuses.Length; i++)
                                        {
                                            if (availPositions.Contains(i))
                                                statues[i] = 2;
                                        }
                                        statues[ind] = 1;
                                    }
                                    else if(level == 2)
                                    {
                                        for (int i = 0; i < strStatuses.Length; i++)
                                            statues[i] = 2;
                                        statues[ind] = 1;
                                    }
                                    dicParams["status"] = string.Join(",", statues);
                                }
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

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS ||
                            (nextAction == ActionTypes.DOSPIN && !betInfo.HasRemainResponse))
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }

                            if(nextAction == ActionTypes.DOSPIN)
                            {
                                saveHistory(agentID, strUserID, ind, counter, userBalance, currency);
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
                    Sender.Tell(resultMsg, Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AladdinsTreasureGameLogic::onDoBonus {0}", ex);
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
