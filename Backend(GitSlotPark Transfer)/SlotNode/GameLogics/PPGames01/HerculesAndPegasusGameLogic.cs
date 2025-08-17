using System;
using System.Collections.Generic;
using GITProtocol;
using Akka.Actor;
using Akka.Event;

namespace SlotGamesNode.GameLogics
{
    public class HerculesAndPegasusGameLogic : BasePPSlotGame
    {

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20hercpeg";
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
            get
            {
                return 20;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 20;
            }
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
                return "msi=12&def_s=9,6,3,4,5,4,8,6,9,8,5,3,8,8,7&cfgs=3398&ver=2&reel_set_size=8&def_sb=2,7,5,3,5&def_sa=6,2,7,3,8&prg_cfg_m=wm,s,s,wms,s&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&cls_s=-1&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,60.00,70.00,80.00,90.00,100.00,110.00,120.00,130.00,140.00,150.00,160.00,170.00,180.00,190.00,200.00,240.00,300.00,400.00,500.00,700.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00&defc=100.00&prg_cfg=1,16,15,16,15&wilds=2~500,200,40,2,0~1,1,1,1,1;17~500,200,40,2,0~1,1,1,1,1;18~500,200,40,2,0~1,1,1,1,1&bonuses=0;13;14&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,200,40,2,0;300,140,40,0,0;200,100,20,0,0;200,100,20,0,0;100,20,8,0,0;100,20,8,0,0;50,10,4,0,0;50,10,4,0,0;50,10,4,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&rtp=94.50&reel_set0=2,9,10,11,3,3,3,9,10,8,6,10,4,9,8,5,9,0,7,8~5,9,8,4,7,4,9,11,4,7,5,2,10,9,5,11,6,9,10,11,3,3,3,10,6,9,8~11,10,5,7,6,11,4,7,11,3,3,3,9,11,0,9,10,4,8,5,10,2,7,0~11,10,7,6,10,11,3,3,3,8,5,9,7,2,4,9,8,5,9~6,10,9,3,3,3,9,10,2,8,10,5,8,6,11,9,6,11,14,9,13,4,11,6,7,8,11,4,9,8,5,10,7,14,4,11,10,2,8&reel_set2=4,9,10,11,3,3,3,5,7,11,9,5,10,8,11,9,7,10,8,11,10,6,11,7~5,9,8,4,7,11,4,8,10,6,7,11,8,7,9,6,11,3,3,3~7,11,8,6,11,10,5,7,6,11,5,3,3,3,7,4,8,5,10,6,9,8~11,10,7,6,10,6,7,4,11,6,10,11,5,8,3,3,3,3,11,9,5,8,10,4,8,10,9,7,4,11,5,9,8~6,10,9,3,3,3,3,9,10,11,8,10,5,8,6,7,5,10,6,7,4,8&reel_set1=8,12,9,12,12,12,9,12,8,12,12,11,12,8,2,4,5,12,12,12,12,12,7,12,12,12,6,2,8,12,12,10,6,3~12,12,9,12,10,4,12,9,12,12,12,3,12,2,5,10,12,12,12,12,10,12,11,7,12,8,12,10,6,12~12,8,12,12,10,12,8,12,12,7,12,12,12,12,12,12,12,12,6,12,7,2,9,6,2,9,12,12,3,4,4,5,11~5,10,4,7,6,10,5,12,12,9,12,6,12,12,10,12,11,12,5,12,4,8,3,11,2,9,3,8,12,10,12,7,12~6,2,11,12,7,12,12,12,5,12,8,12,9,12,8,12,12,12,12,10,12,7,12,10,12,9,12,9,11,12,4,3&reel_set4=9,10,11,3,3,3,9,10,8,6,10,4,9,8,5,9,10,2,2,2,7~5,9,8,4,7,11,4,8,10,2,7,11,8,2,3,3,3,10,6,9,8,7,9,6,11~7,11,8,7,2,10,7,2,8,9,2,7,4,8,5,10,6,9,8,3,3,3~11,10,7,6,10,11,5,5,8,3,3,3,11,9,5,8,10,2,8,10,9,7,4,11,5,9,8,6,9~6,10,9,7,6,9,11,3,10,6,7,0,10,5,8,3,11,8,5,10,2,0,7,8,9,5,10,4,11,10,8,5,10,3,7,9&reel_set3=9,2,7,10,11,3,3,3,9,10,8,6,10,4,9,8,5,9,10~5,9,8,4,7,11,4,8,10,2,7,11,8,2,3,3,3,10,6,9,8,7,9,6,11~7,11,8,7,2,10,7,2,8,9,2,7,4,8,5,10,6,9,8,3,3,3~11,10,7,6,10,11,5,5,8,3,3,3,11,9,5,8,10,2,8,10,9,7,4,11,5,9,8,6,9~6,10,9,7,6,9,11,3,10,6,7,0,10,5,8,3,11,8,5,10,0,7,8,9,2,5,10,4,11,10,8,5,10,3,7,9&reel_set6=9,10,11,3,3,3,9,10,8,6,10,4,9,8,5,9,10,2,2,2,7~5,9,8,4,7,11,4,8,10,2,7,11,8,2,3,3,3,10,6,9,8,7,9,6,11~7,11,8,7,2,10,7,2,8,9,2,2,2,7,4,8,5,10,6,9,8,3,3,3~11,10,7,6,10,11,5,5,8,3,3,3,11,9,5,8,10,2,8,10,9,7,4,11,5,9,2,2,2,8,6,9~6,10,9,7,6,9,11,3,10,6,7,0,10,5,8,3,11,8,5,10,0,7,8,9,2,5,10,4,11,10,8,5,10,3,7,2,2,2,9&reel_set5=7,9,10,11,3,3,3,9,2,2,2,10,8,6,10,4,9,8,5,9,10~5,9,8,4,7,11,4,8,10,2,7,11,8,2,3,3,3,10,6,9,8,7,9,6,11~7,11,8,7,2,10,7,2,8,9,2,7,4,8,5,10,6,9,8,3,3,3~11,10,7,6,10,11,5,5,8,3,3,3,11,9,5,8,10,2,8,10,9,2,2,2,7,4,11,5,9,8,6,9~6,10,9,7,6,9,11,3,10,6,7,0,10,5,8,3,11,8,5,10,0,2,7,8,9,5,10,4,11,10,8,5,10,3,7,9&reel_set7=9,10,11,3,3,3,9,10,8,6,10,4,9,2,2,2,2,8,5,9,10,2,7~5,9,8,4,7,11,4,8,10,2,7,11,8,2,3,3,3,10,6,9,8,7,9,6,11~7,11,8,7,2,10,7,2,2,2,8,9,2,7,3,3,3,4,8,5,10,6,9,8~11,10,7,6,10,11,5,5,8,3,3,3,11,9,5,8,10,2,8,10,9,7,4,11,5,9,8,6,2,2,2,9~6,10,9,7,6,9,11,3,10,6,7,0,10,5,8,3,11,8,5,10,0,7,8,9,2,5,10,4,11,2,2,2,10,8,5,10,3,7,9";
            }
        }
        #endregion

        public HerculesAndPegasusGameLogic()
        {
            _gameID = GAMEID.HerculesAndPegasus;
            GameName = "HerculesAndPegasus";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
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
                    BasePPSlotSpinResult    result          = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS || (ind < 0 || ind > 2))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if (!dicParams.ContainsKey("wins") || !dicParams.ContainsKey("wins_mask") || !dicParams.ContainsKey("status"))
                            throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));

                        string[] strWins     = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string[] strStatus   = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        //0인 경우 필요없음
                        if (ind != 0)
                        {
                            string strTemp      = strWins[0];
                            strWins[0]          = strWins[ind];
                            strWins[ind]        = strTemp;

                            strTemp             = strWinsMask[0];
                            strWinsMask[0]      = strWinsMask[ind];
                            strWinsMask[ind]    = strTemp;

                            strTemp             = strStatus[0];
                            strStatus[0]        = strStatus[ind];
                            strStatus[ind]      = strTemp;
                        }
                        dicParams["wins"]       = string.Join(",", strWins);
                        dicParams["wins_mask"]  = string.Join(",", strWinsMask);
                        dicParams["status"]     = string.Join(",", strStatus);

                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                        string strResponse      = convertKeyValuesToString(dicParams);

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
                _logger.Error("Exception has been occurred in DiamondStrikeGameLogic::onDoBonus {0}", ex);
            }
        }


        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
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
