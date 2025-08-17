using Akka.Actor;
using GITProtocol;
using Microsoft.Extensions.Logging;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class VampiresVSWolvesGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10vampwolf";
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
                return "def_s=8,10,5,8,5,8,10,4,7,1,3,7,4,7,1&cfgs=2448&ver=2&reel_set_size=3&def_sb=4,3,5,7,1&def_sa=1,10,8,3,5&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&wilds=2~1000,150,25,0,0~1,1,1,1,1;19~1000,150,25,0,0~1,1,1,1,1;20~1000,150,25,0,0~1,1,1,1,1;21~1000,150,25,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;800,125,25,0,0;800,125,25,0,0;300,75,15,0,0;300,75,15,0,0;180,50,10,0,0;180,50,10,0,0;100,30,8,0,0;60,20,6,0,0;50,15,5,0,0;30,12,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,150,25,0,0;1000,150,25,0,0;1000,150,25,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=7,9,12,7,5,9,1,1,1,7,2,5,11,7,10,11,5,4,11,6,4,11,8,6,4,1,1,1,10,3,5,4,9,3,4,7,11,12,8,4,6,9,10,11,2,6,11,7,6,10,3,8,11,2,7,11,3,5,12,2,6,12,10,11,1,1,1,12,4,11,12,9,5,12,9,3,8,12,9,10,5,12,10,3,9,6,12,8,10,3,8,6,9,12,6,10,2,7,12,10,7,9,10,2,7,8,11,10,8,12,5,2,12,5,8,4,11,8,3~12,11,10,2,10,8,9,11,4,5,11,12,8,3,6,9,3,8,12,6,2,12,5,8,4,10,7,9,11,5,12,10,4,11,12,5,6,12,3,7,12,2,9,11,7,9,3,6,7,12,10,11,12,6,3,12,2,9,3,8,2,9,8,12,9,8,11,4,7,9,5,12,7,6,10,11,5,10,3,4,7,8,6,7,5,10,4,5~8,9,12,10,9,5,12,8,10,12,7,3,12,2,4,12,8,11,12,6,7,11,12,8,10,4,12,10,9,2,10,7,4,3,6,10,9,6,3,11,10,2,10,8,4,9,7,8,12,2,7,12,10,7,5,10,3,8,9,11,10,3,8,11,2,7,11,3,9,6,5,4,9,6,12,4,11,8,5,2,9,5,7,9,5,11,6,5,11,12,7~11,8,12,4,5,12,4,8,10,6,2,10,12,6,7,2,3,10,9,12,6,2,12,6,7,11,6,10,8,11,4,8,7,4,12,9,10,8,6,11,7,6,11,9,4,10,9,12,4,3,5,7,2,5,10,3,7,11,6,3,9,5,2,9,5,8,7,4,10,3,9,11,10,12,8,4,9,12,11,9,12,6,8,2,11,12,5~8,5,10,12,5,8,9,10,7,5,2,7,5,3,10,11,1,1,1,12,9,2,8,12,7,10,6,2,9,6,7,8,11,4,9,12,11,8,12,9,3,12,6,1,1,1,11,10,3,11,4,10,12,4,10,11,7,10,5,8,10,3,12,7,6,9,7,3,2,4,6,12,7,8,12,7,1,1,1,9,6,7,11,8,9,5,2,9,8,3,4,11,3,5,12,11,6,10,11,6&reel_set2=8,7,11,7,9,12,5,9,11,11,7,10,5,11,7,5,12,7,4,8,5,10,3,6,10,4,3,8,11,11,8,6,9,10,6,2,11,6,12,6,7,10,3,8,11,11,2,7,4,5,12,12,6,5,12,10,11,11,4,9,9,12,3,9,12,9,10,12,5,12,10,9,6,8,9,10,3,8,6~12,11,10,3,6,4,8,12,10,11,8,10,11,4,5,12,4,3,7,10,4,8,12,6,12,2,6,8,12,5,3,10,11,12,4,7,10,11,5,12,10,6,11,5,6,12,3,12,7,2,9,9,11,7,4,6,8,12,12,10,11,7,3,6,11,11,9,12,5,10,7,9,8~8,10,12,11,10,5,12,9,11,12,7,3,4,12,5,8,12,11,6,12,5,11,7,5,12,7,8,9,7,8,11,12,9,11,5,12,11,9,8,5,4,7,11,10,7,3,12,11,10,12,11,10,4,7,2,6,11,9,10,8,9,12,8,11,7,10,6,3,10,3,4,9,9~12,9,8,11,9,11,6,9,11,10,12,6,9,7,11,12,7,8,11,10,12,7,8,9,7,12,10,6,9,8,10,9,7,10,3,12,4,2,11,12,5,11,9,7,12,11,8,7,10,11,10,11,7,8,11,5,8,12,7,3,10,5,10,9,8,11,10,12,11,11~6,9,11,6,10,3,11,9,6,4,7,5,11,4,12,6,9,3,5,9,11,9,6,7,10,9,12,10,12,11,9,3,10,5,12,6,5,8,12,5,10,12,6,11,12,5,2,9,11,8,11,9,5,11,4,6,12,11,9,7,11,9,12,8,10,5,7,4,2,12,12,11,10,12,10,12,10,10,3,9,6,5,12,12,3,8,11,12,6,12,4,12,10&reel_set1=11,6,4,11,8,18,6,4,10,16,17,4,9,16,4,18,11,12,8,6,9,16,10,11,2,6,11,18,6,10,16,8,11,2,18,11,16,17,12,2,6,12,10,11,12,4,12,10,16,9,6,12,8,10,16,8,6,9,12,6,10,2,18,12,10,18,9,10,2,9,18,8,12,17,2,12,17,8,4,11,8,16~12,11,10,4,6,18,10,11,2,10,8,9,11,4,17,11,12,8,16,6,9,18,16,8,12,6,2,12,17,8,11,17,16,10,2,4,10,16,18,9,11,17,12,10,11,12,17,6,12,16,18,12,2,9,11,18,9,16,6,18,12,10,17,11,12,6,16,12,2,6,10,11,9,12,4,10,18,8,2~8,9,12,4,10,9,17,12,8,10,12,18,16,12,2,4,12,8,11,12,17,18,11,4,6,11,4,6,11,18,8,6,18,11,12,8,10,16,4,12,10,9,2,10,18,4,6,10,9,6,16,11,10,2,11,9,16,6,2,17,10,8,17,16,9,18,8,12,2,18,12,10,18,17,10,16,8,9,11,10,8~11,8,12,4,18,10,17,18,8,10,16,9,12,4,17,12,4,8,10,6,2,18,10,12,6,18,2,16,10,9,12,6,2,12,6,18,11,16,6,10,8,11,4,8,18,4,12,9,8,17,9,11,10,2,11,16,10,8,6,11,18,6,17,11,9,4,10,9,12,4,16,17,18,2,17,10,16,18,11,6,9,17,2,9,17,8,18,4,10,16,9,11,10,12~17,11,9,4,17,2,8,17,10,12,17,8,9,10,18,17,2,18,17,16,10,18,11,12,9,2,8,12,18,10,6,2,9,6,18,8,11,16,9,12,11,6,16,11,10,16,11,4,10,8,11,4,12,8,10,12,18,8,12,18,9,6,18,11,8,9,17,2,9,8,16,4,11,16,17,12,11,6,10,11,6,10,2,12,4,11,12,6,9";
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
        public VampiresVSWolvesGameLogic()
        {
            _gameID = GAMEID.VampiresVSWolves;
            GameName = "VampiresVSWolves";
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
                    BasePPSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                        Dictionary<string, string> dicParams = new Dictionary<string, string>();

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
                        if (!startSpinData.IsEvent)
                            sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                        else if (maxWin > selectedWin)
                            addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);

                        dicParams = splitResponseToParams(strSpinResponse);

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
                _logger.Error("Exception has been occurred in VampiresVSWolvesGameLogic::onDoBonus {0}", ex);
            }
        }

    }
}
