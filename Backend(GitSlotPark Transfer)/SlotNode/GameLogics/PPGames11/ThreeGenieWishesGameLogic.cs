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
    class ThreeGenieWishesGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs50aladdin";
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
                return "def_s=4,11,5,2,7,8,4,1,11,3,5,7,11,5,11,4,9,5,9,11&cfgs=2224&ver=2&reel_set_size=2&def_sb=6,2,2,2,2&def_sa=11,10,9,9,7&scatters=1~1,1,1,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=4.00,8.00,12.00,16.00,20.00,40.00,60.00,80.00,100.00,150.00,200.00,300.00,500.00,1000.00,1500.00,2000.00&defc=20.00&wilds=2~400,100,20,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;300,80,15,0,0;200,70,15,0,0;150,60,10,0,0;125,50,10,0,0;90,40,10,0,0;80,30,10,0,0;70,25,5,0,0;60,20,5,0,0;50,12,5,0,0&reel_set0=11,7,8,4,11,7,9,5,5,1,11,6,1,2,2,2,2,10,6,7,3,7,9,9,4~9,3,10,10,5,4,2,2,2,2,8,11,6,8,7,8,10,6,8~11,11,8,10,3,3,7,8,6,11,10,1,9,4,8,7,8,2,2,2,2,2,5~8,10,11,6,5,7,2,2,2,2,10,4,5,3,10,9,11,9,11,8,3,6~7,10,7,11,9,3,4,5,8,6,5,7,9,5,1,7,10,2,2,2,2,11,8,11&reel_set1=6,10,3,9,6,11,10,9,10,11,8,4,5,7,5,4,10~10,6,11,7,10,9,4,8,11,10,11,9,6,7,3,8,5~9,3,10,11,6,10,11,5,7,4,6,5,8,11,8~10,4,8,9,9,3,8,11,7,9,11,10,5,6,3,4,11,7,10~5,7,11,10,5,3,7,8,6,3,6,4,4,8,9,11";
            }
        }
	
	
        #endregion
        public ThreeGenieWishesGameLogic()
        {
            _gameID = GAMEID.ThreeGenieWishes;
            GameName = "ThreeGenieWishes";
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
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();
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

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if (dicParams.ContainsKey("status"))
                        {
                            var strStatuses = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if(ind != 0)
                            {
                                string strTemp   = strStatuses[0];
                                strStatuses[0]   = strStatuses[ind];
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
                        if (dicParams.ContainsKey("wins_mask"))
                        {
                            var strStatuses = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (ind != 0)
                            {
                                string strTemp = strStatuses[0];
                                strStatuses[0] = strStatuses[ind];
                                strStatuses[ind] = strTemp;
                            }
                            dicParams["wins_mask"] = string.Join(",", strStatuses);
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
                _logger.Error("Exception has been occurred in ThreeGenieWishesGameLogic::onDoBonus {0}", ex);
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
