using Akka.Actor;
using Akka.Event;
using GITProtocol;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TheWildMachineBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 20.0f; }
        }
    }
    class TheWildMachineGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40madwheel";
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
            get { return 40; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 7;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=12,4,12,7,11,13,13,8,10,9,6,5,13,13,11,11,8,8,4,13,13,6,9,12,7,3,13,13,5,6,12,12,3,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13,13&cfgs=2801&nas=13&ver=2&reel_set_size=4&def_sb=5,10,11,8,1,7,13,13&def_sa=8,3,4,3,11,3,13,13&bonusInit=[{bgid:0,bgt:39,sps_wins:\"1,75,150,125,250,100,1,125,100,75,150,250,1,100,75,250,150,100,1,125,250,100,150,75,1,250,125,200,100,75\",sps_wins_mask:\"pbf,w,w,w,w,w,pbf,w,w,w,w,w,pbf,w,w,w,w,w,pbf,w,w,w,w,w,pbf,w,w,w,w,w\"}]&scatters=1~0,0,0,0,0,0,0~0,0,0,0,0,0,0~1,1,1,1,1,1,1&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~3500,0,500,200,25,0,0~1,1,1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0,0,0;0,0,0,0,0,0,0;0,0,0,0,0,0,0;3500,0,500,100,25,0,0;1000,0,250,75,10,0,0;750,0,125,50,10,0,0;600,0,100,25,5,0,0;450,0,85,18,5,0,0;375,0,75,12,5,0,0;250,0,50,8,2,0,0;250,0,50,8,2,0,0;250,0,50,8,2,0,0;250,0,50,8,2,0,0;0,0,0,0,0,0,0&reel_set0=7,7,9,9,2,11,11,8,8,4,4,10,10,7,7,6,6,12,12,3,3,9,9,7,7,10,10,5,5,11,11,7,7,8,8,10,10,4,4,12,12,7,7,9,9,6,6,8,8,11,11,7,7,10,10,2,12,12,8,8,5,5,9,9,10,10,7,7,11,11,4,4,10,10,12,12,6,6,11,11,2,12,12,5,5,9,9,11,11,6,6,10,10,3,3,12,12,8,8,9,9~4,4,10,10,6,6,1,12,12,7,7,11,11,4,4,9,9,7,7,1,11,11,5,5,12,12,8,8,2,10,10,6,6,1,11,11,4,4,9,9,8,8,3,3,12,9,6,6,1,10,10,8,8,12,12,4,4,11,11,7,7,2,9,9,6,6,11,11,1,10,10,5,5,12,12,8,8,1,9,9,6,6,10,10,4,4,11,11,7,7,1,12,12,6,6,10,10,5,5,11,11,1,6,6,9,9,2,8,8,12,12,3,3,11,11,7,7,10~3,3,6,6,12,6,7,7,9,9,1,10,10,8,8,9,9,5,5,2,11,11,8,8,12,12,6,6,9,9,1,8,10,10,4,4,9,9,5,5,10,10,7,7,11,11,8,9,12,12,1,9,9,8,8,2,10,10,5,5,12,12,7,7,11,11,1,8,8,9,9,5,5,1,12,12,4,4,10,10,8,8,9,9,2,11,11,6,6,12,12,1,7,7,10,10,3,3,9,9,8,8,11,11,5,5,12,12,1,10,10,8,8,9,9,6~7,7,12,12,7,4,1,9,9,5,5,8,8,10,10,3,3,5,6,12,12,4,4,9,9,7,7,11,11,2,10,10,5,5,12,12,1,6,6,11,11,7,7,3,3,9,9,8,8,5,5,10,10,6,6,1,9,9,5,5,12,12,7,5,3,9,9,8,8,11,11,5,5,12,12,1,7,7,10,10,4,4,7,7,2,9,9,5,5,6,6,11,11,8,8,3,3,12,12,7,7,10,10,5,5,9,9,6,6,1,12,12,7,7,4,4,9,9,7,7,2,12,12,6,6,3,3,11,11,7,8~8,8,11,11,4,4,6,6,9,9,5,5,2,10,10,7,7,4,4,9,9,6,6,11,11,3,3,7,7,10,10,4,7,6,6,11,11,2,9,9,8,8,5,5,11,11,4,7,10,10,7,7,8,8,12,12,5,5,6,6,10,10,7,7,8,8,9,9,4,4,11,11,8,8,10,10,6,6,12,12,3,3,9,9,6,6,8,8,12,12,6,6,10,10,2,11,11,7,7,12,12,4,4,10,10,6,6,11,11,5,5,9,9,8,8,6,6,10,10,4,4,7,7,11,6,12~13,13,13,13,13,13,13~13,13,13,13,13,13,13&reel_set2=8,8,10,10,5,5,12,12,2,9,9,4,4,11,11,7,7,10,10,12,12,5,5,11,11,8,8,12,12,6,6,10,10,7,7,9,9,11,11,3,3,12,12,6,6,11,11,8,8,2,10,10,4,4,9,9,5,5,12,12,7,7,10,10,6,6,9,9,8,8,11,11,4,4,9,9~8,8,11,11,6,6,9,9,7,7,12,12,4,4,9,9,5,5,10,10,8,8,11,11,2,12,12,6,6,10,10,3,3,9,9,5,5,11,11,7,7,10,10,4,4,12,12,6,6,11,11,7,7,9,9,12,12,3,3,10,7,6,6,11,11,2,9,9,5,5,12,12,8,11,10,10,7,7,9,9~8,8,10,10,6,6,11,11,8,8,9,9,4,4,12,12,7,7,11,11,2,10,10,5,5,12,12,7,7,10,10,3,3,8,10,9,9,6,6,10,10,4,4,11,11,7,7,12,12,5,5,11,11,8,8,9,9,6,6,5,10,2,12,12,7,7,11,11,4,8,3,3,10,10,5,5,12,12,7,7,9,9~8,8,11,11,6,5,10,10,5,5,12,12,8,8,11,11,3,4,9,9,7,7,12,12,8,8,10,10,4,4,9,9,6,6,11,11,7,7,12,12,8,8,11,11,2,9,9,5,5,10,10,7,7,12,12,3,3,10,7,6,6,9,9,8,8,11,10,7,7,12,12,5,5,9,9~6,6,12,12,7,7,11,11,4,4,9,9,8,8,10,10,5,5,12,12,6,6,11,11,3,3,10,10,7,7,9,9,2,12,12,7,8,11,11,5,5,8,10,6,6,12,12,8,8,9,9,4,4,10,10,6,9,11,11,7,7,9,9,3,3,10,12,5,5,11,11,7,7,9,9~13,13,13,13,13,13,13~13,13,13,13,13,13,13&reel_set1=2,2,2,2,2,2,2~3,3,9,9,10,10,8,8,12,12,7,7,9,9,5,5,10,10,8,8,9,9,6,6,11,11,4,4,10,10,8,8,9,9,7,7,12,12,6,6,11,11,8,8,9,9,10,10,7,7,11,11,9,9,3,3,12,12,8,8,10,10,7,7,9,9,10,10,5,5,11,11,6,6,10,10,2,12,12,8,8,9,9,11,11,7,7,12,12,10,10,4,4,11,11,8,8,9,9,12,12,5,5,11,11,6,6~3,3,9,7,11,11,5,5,10,10,6,6,12,12,7,7,9,9,11,11,4,4,12,12,10,10,6,6,11,11,7,7,12,12,5,5,9,9,11,11,8,6,10,10,2,12,12,7,7,11,11,3,3,12,12,8,8,9,9,4,4,7,11,6,6,4,10,8,8,12,12,7,7,11,11,12,12,2,9,9,10,10,6,6,11,11,9,9,7,7,12,12,8,8,10,10,4,4,9,9,7,7,11,11,8,8,12,12,5~2,2,2,2,2,2,2~3,3,9,9,4,4,8,8,12,12,3,3,6,6,9,9,5,5,12,4,4,5,5,10,10,3,3,8,8,9,9,5,5,4,4,12,12,5,5,7,7,9,9,2,11,11,3,3,10,10,5,5,7,7,9,9,6,6,4,4,11,11,5,5,9,9,6,6,12,12,5,5,10,10,7,7,2,11,11,5,5,8,8,12,12,7,7,4,4,10,10,6,6,9,9,7,7,3,3,11,11,8,8,7,7,9,9,6,6,10,10,7,7,12,12,8,8,11,11,12,12~4,4,5,5,9,9,7,7,11,11,8,8,10,10,4,4,9,9,3,3,12,12,6,6,11,11,7,7,5,5,11,11,8,4,10,10,2,5,5,12,12,6,6,7,7,11,11,4,4,5,5,10,10,6,7,8,8,12,12,7,7,9,9,8,8,6,6,10,5,4,4,5,5,11,11,6,6,7,7,9,9,8,8,4,4,10,10,6,6,8,8,12,12,2,11,11,6,6,8,8,10,10,3,3,9,9,6,6,7,7,10,10,5,11,11,4,10,8,8,11,11,6,6,12,12~2,2,2,2,2,2,2&reel_set3=2,2,2,2,2,2,2~3,3,11,11,7,7,10,10,6,6,9,9,8,8,12,12,4,4,11,11,4,8,10,10,7,7,4,9,12,12,5,5,10,10,9,9,8,8,10,10,6,6,9,9,11,11,7,7,12,12,9,9,5,5,11,11,8,8,10,10,9,9,6,6,10,10,8,8,12,12,2,7,7,10,10,8,8,11,11,9,9,3,3,12,12,5,5,11,11,9,9,6~3,3,11,5,6,6,12,12,10,10,2,11,11,4,4,12,12,9,9,7,7,11,11,5,5,12,12,8,8,10,10,2,12,12,6,6,11,11,7,7,9,9,8,8,2,10,10,4,4,12,12,6,6,11,11,8,8,9,9,2,8,10,7,7,12,12,5,5,11,11,7,7,2,9,9,8,8,10,10,6,6,12,12,4,4,11,11,2,12,12,3,11,11,5,5,12,12,7,7,2,10,10,8,8,9,9,7,7~2,2,2,2,2,2,2~3,3,12,12,6,6,7,7,10,10,4,4,8,8,9,9,5,5,7,7,12,12,6,6,11,11,7,7,9,9,5,5,12,12,4,4,10,10,7,7,9,9,3,3,8,8,12,12,7,7,4,4,11,11,6,6,7,7,9,9,3,3,5,5,12,12,7,7,10,10,6,6,8,8,9,9,5,5,7,7,11,11,2,12,12,5,5,8,8,10,10,4,5,5,6,6,12,12,4,4,11,11,5,5,10,10,8,8,9,9,3,3,6,6,11,11,4,4,12,12,5,5,9,9,4,4~3,3,9,9,8,8,10,10,5,5,6,6,11,11,8,8,7,7,10,10,4,4,8,8,11,11,5,5,4,6,9,9,4,4,7,7,10,10,2,6,6,11,4,4,12,12,7,7,6,7,9,9,8,8,4,4,11,11,5,5,10,4,6,6,7,7,8,12,4,4,8,11,6,6,10,10,5,5,8,8,12,12,3,8,8,9,9,4,4,10,10,6,6,11,11,5,5,12,12,8,8,9,9,6,6,10,10,4,4,11,11,7,7,12,12,8,8,10,10~2,2,2,2,2,2,2";
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
        public TheWildMachineGameLogic()
        {
            _gameID = GAMEID.TheWildMachine;
            GameName = "TheWildMachine";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            TheWildMachineBetInfo betInfo = new TheWildMachineBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new TheWildMachineBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                TheWildMachineBetInfo betInfo = new TheWildMachineBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in TheWildMachineGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheWildMachineGameLogic::readBetInfoFromMessage {0}", ex);
            }
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
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin = 0.0;
                string strGameLog = "";
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo    betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                    if ((result.NextAction != ActionTypes.DOBONUS))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams  = new Dictionary<string, string>();
                        do
                        {
                            if (result.HasBonusResult)
                            {
                                dicParams = splitResponseToParams(result.BonusResultString);
                                if (dicParams.ContainsKey("status") && dicParams["status"] == "0,0")
                                {
                                    int ind = (int)message.Pop();
                                    BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
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
                            }
                            BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                            dicParams = splitResponseToParams(actionResponse.Response);
                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        } while (false);
                        
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction   = convertStringToActionType(dicParams["na"]);
                        string       strResponse = convertKeyValuesToString(dicParams);

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
                _logger.Error("Exception has been occurred in TheWildMachineGameLogic::onDoBonus {0}", ex);
            }
        }

    }
}
