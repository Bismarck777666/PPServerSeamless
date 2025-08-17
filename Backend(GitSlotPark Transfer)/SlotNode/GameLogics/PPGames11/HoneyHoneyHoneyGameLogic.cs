using Akka.Actor;
using Akka.Event;
using GITProtocol;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class HoneyHoneyHoneyGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20honey";
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
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
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
                return "def_s=10,9,11,10,10,11,8,8,7,9,3,4,1,6,3&cfgs=2541&ver=2&reel_set_size=4&def_sb=3,7,11,10,3&def_sa=9,5,7,11,6&bonusInit=[{bgid:2,bgt:40,av_symb:\"3,4,5,6,7,8,9,10,11\"},{bgid:3,bgt:40,av_symb:\"3,4,5,6,7,8,9,10,11\"}]&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&cls_s=16&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~500,200,50,2,0~1,1,1,1,1;12~500,200,50,2,0~1,1,1,1,1;13~500,200,50,2,0~1,1,1,1,1;14~500,200,50,2,0~1,1,1,1,1;15~500,200,50,2,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,200,40,2,0;300,100,30,0,0;200,80,20,0,0;200,80,20,0,0;150,40,8,0,0;150,40,8,0,0;100,10,4,0,0;100,10,4,0,0;100,10,4,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=4,8,7,3,3,3,3,10,6,2,11,5,9,1,8,5,7,10,6,11,4,9,2,8,10,4,7,6,1,5,9,8,5,7~4,8,7,3,3,3,3,10,6,2,11,5,9,8,5,7,10,6,11,4,9,8,2,10,4,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,2,11,5,9,1,8,5,7,10,6,11,4,9,2,8,10,4,7,6,1,5,9,8,5,7~4,8,7,3,3,3,3,10,6,2,11,5,9,8,5,7,10,6,11,4,9,8,2,10,4,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,2,11,5,9,1,8,5,7,10,6,11,4,9,2,8,10,4,7,6,1,5,9,8,5,7&reel_set2=4,8,7,3,3,3,3,10,6,11,5,9,2,8,5,7,10,6,11,4,9,8,10,4,2,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,11,5,9,2,8,5,7,10,6,11,4,9,8,10,4,2,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,11,5,9,2,8,5,7,10,6,11,4,9,8,10,4,2,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,11,5,9,2,8,5,7,10,6,11,4,9,8,10,4,2,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,11,5,9,2,8,5,7,10,6,11,4,9,8,10,4,2,7,6,11,5,9,8,5,7&reel_set1=4,8,7,3,3,3,3,10,6,11,5,9,8,5,7,10,6,11,4,9,8,10,4,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,11,5,9,8,5,7,10,6,11,4,9,8,10,4,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,11,5,9,8,5,7,10,6,11,4,9,8,10,4,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,11,5,9,8,5,7,10,6,11,4,9,8,10,4,7,6,11,5,9,8,5,7~4,8,7,3,3,3,3,10,6,11,5,9,8,5,7,10,6,11,4,9,8,10,4,7,6,11,5,9,8,5,7&reel_set3=11,9,5,8,10,3,11,4,8,9,7,5,10,6~3,11,4,8,9,7,5,10,6,11,9,5,8,10~6,11,9,5,8,10,3,11,4,8,9,7,5,10~8,9,7,5,10,6,11,9,5,8,10,3,11,4~5,8,10,3,11,4,8,9,7,5,10,6,11,9";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 2; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201 };
        }
        #endregion
        public HoneyHoneyHoneyGameLogic()
        {
            _gameID = GAMEID.HoneyHoneyHoney;
            GameName = "HoneyHoneyHoney";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "3";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }

        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                await base.buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, minOdd, maxOdd);
            else
                await base.buildStartFreeSpinData(startSpinData, buildType, minOdd, maxOdd);
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
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
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index = (int)message.Pop();
                int counter = (int)message.Pop();

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin          = 0.0;
                string strGameLog       = "";
                string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo    betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult result  = _dicUserResultInfos[strGlobalUserID];
                    if (result.NextAction != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                        Dictionary<string, string> dicParams = new Dictionary<string, string>();

                        do
                        {
                            if (!betInfo.HasRemainResponse)
                            {
                                int ind = (int)message.Pop();
                                if (ind >= startSpinData.FreeSpins.Count)
                                    throw new Exception("FreeSpin index is not valid");

                                BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[ind]);
                                preprocessSelectedFreeSpin(freeSpinData, betInfo);

                                betInfo.SpinData = freeSpinData;
                                List<string> freeSpinStrings = new List<string>();
                                for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                    freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                                string strSpinResponse = freeSpinStrings[0];
                                if (freeSpinStrings.Count > 1)
                                    betInfo.RemainReponses = buildResponseList(freeSpinStrings);

                                double selectedWin = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                                double maxWin = startSpinData.MaxOdd * betInfo.TotalBet;

                                //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);

                                dicParams = splitResponseToParams(strSpinResponse);

                                convertWinsByBet(dicParams, betInfo.TotalBet);
                                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                                break;
                            }

                            BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                            dicParams                             = splitResponseToParams(actionResponse.Response);
                            if(dicParams.ContainsKey("status") && dicParams["status"] == "1,0,0")
                            {
                                int      ind        = (int)message.Pop();
                                if(ind != 0)
                                {
                                    string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string strTemp  = strParts[0];
                                    strParts[0]     = strParts[ind];
                                    strParts[ind]   = strTemp;
                                    dicParams["status"] = string.Join(",", strParts);

                                    strParts        = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    strTemp         = strParts[0];
                                    strParts[0]     = strParts[ind];
                                    strParts[ind]   = strTemp;
                                    dicParams["wins"] = string.Join(",", strParts);

                                    strParts        = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    strTemp         = strParts[0];
                                    strParts[0]     = strParts[ind];
                                    strParts[ind]   = strTemp;
                                    dicParams["wins_mask"] = string.Join(",", strParts);
                                }
                            }
                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        } while (false);

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

                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strGlobalUserID);
                    }
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HoneyHoneyHoneyGameLogic::onDoBonus {0}", ex);
            }
        }

    }
}
