using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Akka.Actor;
using Akka.Event;

namespace SlotGamesNode.GameLogics
{
    class PeKingLuckGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25peking";
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
                return 25;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 25;
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
                return "def_s=6,9,8,4,7,7,3,4,11,11,10,11,6,5,10&bgid=0&sps_levels=nff,m&sps_wins=8,12,15,18,28,38,2,3,5,8,10,18&sps_wins_mask=nff,nff,nff,nff,nff,nff,m,m,m,m,m,m&cfgs=2329&ver=2&reel_set_size=2&def_sb=13,12,8,10,6&def_sa=6,3,1,8,12&scatters=1~250,10,3,1,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&bgt=28&sc=10.00,20.00,40.00,60.00,80.00,100.00,200.00,400.00,800.00,1000.00,2000.00,3000.00,4000.00&defc=100.00&wilds=2~10000,4000,500,20,0~2,2,2,2,2&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;750,125,30,2,0;500,100,25,2,0;400,80,20,0,0;300,75,15,0,0;250,60,15,0,0;200,50,10,0,0;150,40,10,0,0;125,30,5,0,0;100,30,5,0,0;100,25,5,0,0;100,25,5,2,0&rtp=95.50&reel_set0=6,9,9,2,2,2,11,9,12,5,9,11,6,11,5,9,11,4,12,10,5,9,8,7,6,6,3,11,11,8,13,10,7,12,4,13,3,11,1,8~3,2,2,2,9,6,13,7,10,10,11,9,13,10,11,10,6,5,1,12,10,12,7,10,5,11,4,12,7,13,3,12,8,11,6,12,13,10,8,10,4,5,13,13,7,10~1,12,13,10,7,7,4,13,12,5,7,9,12,8,13,12,12,10,8,4,5,6,10,13,7,9,11,3,12,5,2,2,2,9,11,13,3,8,6,12,6,4,9,12,11,6,13,8,1~8,4,5,5,12,2,2,2,6,11,12,8,13,8,10,7,13,7,13,6,9,8,7,10,12,1,11,8,5,8,3,5,3,12,3,13,11,4,11,10,11,10,8,1,9~12,9,10,10,4,6,13,1,6,11,13,10,10,7,9,6,13,9,11,3,12,7,11,11,5,8,10,2,2,2,3,3,8,13,9,9,4,5,12,13,9,6,1,8,13,13,6,10,10,12&reel_set1=6,7,2,2,2,4,11,13,10,11,11,12,1,12,6,5,8,9,7,7,11,5,10,9,4,13,9,8,13,12,3,9,11,3,10,8~4,12,12,10,3,13,10,4,9,13,10,12,8,7,5,8,2,2,2,13,7,1,12,7,11,6,13,10,6,9,13,10,13,10,5,11,3,7,6,13~11,2,2,2,12,3,13,5,8,6,9,13,13,8,12,11,7,11,13,7,4,7,1,4,12,13,10,8,13,10,11,6,12,9,6,8,4,8,3,12,6,5,9,5,10~5,9,4,3,5,7,11,7,8,5,12,12,2,2,2,3,9,3,5,12,11,1,6,6,8,8,13,8,8,11,13,4,10,1,12,13,10,11,8,11,8,10,7,8,10~9,10,8,1,12,5,11,12,10,10,4,13,11,5,5,3,8,6,4,13,1,3,10,7,2,2,2,9,7,3,12,11,11,9,13,13,12,9,10,8,3,6,13,6,9,12,9,6,10,6,10,13,10";
            }
        }
        #endregion

        public PeKingLuckGameLogic()
        {
            _gameID = GAMEID.PeKingLuck;
            GameName = "PeKingLuck";
        }

        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin = 0.0;
                string strGameLog = "";
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo betInfo   = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string strTemp  = strParts[0];
                        strParts[0]     = strParts[ind];
                        strParts[ind]   = strTemp;
                        dicParams["status"] = string.Join(",", strParts);

                        strParts = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        strTemp = strParts[0];
                        strParts[0] = strParts[ind];
                        strParts[ind] = strTemp;
                        dicParams["wins"] = string.Join(",", strParts);

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
                                _dicUserHistory[strGlobalUserID].baseBet    = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win        = realWin;
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
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoBonus {0}", ex);
            }
        }

        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set" };
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
