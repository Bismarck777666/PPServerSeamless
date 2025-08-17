using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using SlotGamesNode.Database;
using Akka.Actor;
using PCGSharp;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using GITProtocol.Utils;

namespace SlotGamesNode.GameLogics
{
    public class BaseSelFreeCQ9SlotGame : BaseCQ9SlotGame
    {
        protected SortedDictionary<double, int>   [] _naturalChildFreeSpinOddProbs  = null;
        protected SortedDictionary<double, int[]> [] _totalChildFreeSpinIDs         = null;
        protected int[]                              _naturalChildFreeSpinCounts    = null;
        protected int[]                              _totalChildFreeSpinCounts      = null;

        protected override bool HasSelectableFreeSpin
        {
            get { return true;  }
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
                bool isSuccess = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.SelFreeSpin), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _naturalSpinOddProbs            = new SortedDictionary<double, int>();
                _naturalChildFreeSpinOddProbs   = new SortedDictionary<double, int>[FreeSpinTypeCount];
                _naturalChildFreeSpinCounts     = new int[FreeSpinTypeCount];
                _totalChildFreeSpinCounts       = new int[FreeSpinTypeCount];
                _totalChildFreeSpinIDs          = new SortedDictionary<double, int[]>[FreeSpinTypeCount];

                _spinDataDefaultBet             = response.DefaultBet;
                _naturalSpinCount               = 0;
                _emptySpinIDs                   = new List<int>();

                Dictionary<double, List<int>>   totalSpinOddIds         = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>   freeSpinOddIds          = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>[] totalChildSpinOddIDs    = new Dictionary<double, List<int>>[FreeSpinTypeCount];

                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinBaseData spinBaseData = response.SpinBaseDatas[i];
                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        if(spinBaseData.SpinType <= 100)
                        {
                            _naturalSpinCount++;
                            if (_naturalSpinOddProbs.ContainsKey(spinBaseData.Odd))
                                _naturalSpinOddProbs[spinBaseData.Odd]++;
                            else
                                _naturalSpinOddProbs[spinBaseData.Odd] = 1;
                        }
                        else if(spinBaseData.SpinType >= 200)
                        {
                            int freeSpinType = spinBaseData.SpinType - 200;
                            if (_naturalChildFreeSpinOddProbs[freeSpinType] == null)
                                _naturalChildFreeSpinOddProbs[freeSpinType] = new SortedDictionary<double, int>();

                            if (_naturalChildFreeSpinOddProbs[freeSpinType].ContainsKey(spinBaseData.Odd))
                                _naturalChildFreeSpinOddProbs[freeSpinType][spinBaseData.Odd]++;
                            else
                                _naturalChildFreeSpinOddProbs[freeSpinType][spinBaseData.Odd] = 1;
                        }
                    }

                    if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                        _emptySpinIDs.Add(spinBaseData.ID);

                    if (spinBaseData.SpinType <= 100)
                    {
                        if (!totalSpinOddIds.ContainsKey(spinBaseData.Odd))
                            totalSpinOddIds.Add(spinBaseData.Odd, new List<int>());

                        totalSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);
                    }
                    else
                    {
                        int freeSpinType = spinBaseData.SpinType - 200;
                        if (totalChildSpinOddIDs[freeSpinType] == null)
                            totalChildSpinOddIDs[freeSpinType] = new Dictionary<double, List<int>>();

                        if (!totalChildSpinOddIDs[freeSpinType].ContainsKey(spinBaseData.Odd))
                            totalChildSpinOddIDs[freeSpinType][spinBaseData.Odd] = new List<int>();

                        totalChildSpinOddIDs[freeSpinType][spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.ID <= response.NormalMaxID)
                            _naturalChildFreeSpinCounts[freeSpinType]++;

                        _totalChildFreeSpinCounts[freeSpinType]++;
                    }
                }
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                for(int i = 0; i < FreeSpinTypeCount; i++)
                {
                    if (totalChildSpinOddIDs[i] == null)
                        continue;

                    _totalChildFreeSpinIDs[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in totalChildSpinOddIDs[i])
                        _totalChildFreeSpinIDs[i].Add(pair.Key, pair.Value.ToArray());
                }

                double winRate = 0.0;
                foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    winRate += (pair.Key * pair.Value / _naturalSpinCount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        protected virtual int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204};
        }
        
        protected virtual int getRangeID(double minOdd, double maxOdd)
        {
            minOdd = Math.Round(minOdd, 2);
            maxOdd = Math.Round(maxOdd, 2);
            if (minOdd == 10.0 && maxOdd == 50.0)
                return 1;
            if (minOdd == 50.0 && maxOdd == 100.0)
                return 2;
            if (minOdd == 100.0 && maxOdd == 300.0)
                return 3;
            if (minOdd == 300.0 && maxOdd == 500.0)
                return 4;
            if (minOdd == 500.0 && maxOdd == 1000.0)
                return 5;
            if (minOdd == 1000.0 && maxOdd == 3000.0)
                return 6;
            if (minOdd == 1000.0 && maxOdd == 1500.0)
                return 7;
            if (minOdd == 1500.0 && maxOdd == 3000.0)
                return 8;

            return 1;
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, int isExtraBet)
        {
            BasePPSlotSpinData spinData = await base.selectRandomStop(agentID, isExtraBet);
            if (!(spinData is BasePPSlotStartSpinData))
                return spinData;

            BasePPSlotStartSpinData startSpinData   = spinData as BasePPSlotStartSpinData;
            buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsNaturalRandom, 0.0, 0.0);
            return startSpinData;
        }
        protected virtual async Task<BasePPSlotSpinData> selectRangeSpinData(double minOdd, double maxOdd)
        {
            BasePPSlotSpinData spinData = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectRangeSpinDataRequest(minOdd, maxOdd, getRangeID(minOdd, maxOdd)), TimeSpan.FromSeconds(10.0));
            if (spinData == null)
                return null;

            if(spinData is BasePPSlotStartSpinData)
            {
                BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
                double minFreeOdd = minOdd - startSpinData.StartOdd;
                if (minFreeOdd < 0.0)
                    minFreeOdd = 0.0;
                double maxFreeOdd = maxOdd - startSpinData.StartOdd;
                if (maxFreeOdd < 0.0)
                    maxFreeOdd = 0.0;

                buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsRangeLimited, minFreeOdd, maxFreeOdd);
            }
            return spinData;
        }
        protected virtual void buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            startSpinData.FreeSpins = new List<OddAndIDData>();
            int[] freeSpinTypes     = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);
            double maxFreeOdd = 0.0;
            for (int i = 0; i < freeSpinTypes.Length; i++)
            {
                int freeSpinType = freeSpinTypes[i] - 200;
                OddAndIDData childFreeSpin = null;
                if(buildType == StartSpinBuildTypes.IsNaturalRandom)
                {
                    double  odd   = selectOddFromProbs(_naturalChildFreeSpinOddProbs[freeSpinType], _naturalChildFreeSpinCounts[freeSpinType]);
                    int     id    = _totalChildFreeSpinIDs[freeSpinType][odd][Pcg.Default.Next(0, _totalChildFreeSpinIDs[freeSpinType][odd].Length)];
                    childFreeSpin = new OddAndIDData(id, odd);
                }
                else if(buildType == StartSpinBuildTypes.IsTotalRandom)
                {
                    childFreeSpin = selectOddAndIDFromProbs(_totalChildFreeSpinIDs[freeSpinType], _totalChildFreeSpinCounts[freeSpinType]);
                }
                else
                {
                    childFreeSpin = selectOddAndIDFromProbsWithRange(_totalChildFreeSpinIDs[freeSpinType], minOdd, maxOdd);
                }

                if (childFreeSpin.Odd > maxFreeOdd)
                    maxFreeOdd = childFreeSpin.Odd;

                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd = startSpinData.StartOdd + maxFreeOdd;
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BaseCQ9SlotBetInfo betInfo)
        {
            //배당구간이벤트만을 처리한다.
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                BasePPSlotSpinData spinDataEvent = await selectRangeSpinData(rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (spinDataEvent != null)
                {
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }
            return await selectRandomStop(agentID, betInfo.IsExtraBet);
        }
        protected override async Task<BaseCQ9SlotSpinResult> generateSpinResult(BaseCQ9SlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit, bool isMustLose)
        {
            BasePPSlotSpinData      spinData    = null;
            BaseCQ9SlotSpinResult    result      = null;

            if (betInfo.HasRemainResponse)
            {
                BaseCQ9ActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            double totalBet     = betInfo.TotalBet;
            double realBetMoney = totalBet;

            spinData = await selectRandomStop(agentID, userBonus, totalBet, false, isMustLose, betInfo);
            double totalWin = 0.0;
            if (spinData is BasePPSlotStartSpinData)
                totalWin = totalBet * (spinData as BasePPSlotStartSpinData).MaxOdd;
            else
                totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || spinData.IsEvent || checkCompanyPayoutRate(agentID, realBetMoney, totalWin))
            {
                do
                {
                    if (spinData.IsEvent)
                    {
                        bool checkRet = await subtractEventMoney(agentID, strUserID, totalWin);
                        if (!checkRet)
                            break;

                        _bonusSendMessage   = null;
                        _rewardedBonusMoney = totalWin;
                        _isRewardedBonus    = true;
                    }

                    if (spinData is BasePPSlotStartSpinData)
                        betInfo.SpinData = spinData;
                    else
                        betInfo.SpinData = null;

                    result = calculateResult(betInfo, spinData.SpinStrings[0], true);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                }
                while (false);
            }

            spinData = await selectEmptySpin(betInfo);
            result   = calculateResult(betInfo, spinData.SpinStrings[0], true);
            if (spinData is BasePPSlotStartSpinData)
                betInfo.SpinData = spinData;
            else
                betInfo.SpinData = null;
            
            sumUpCompanyBetWin(agentID, realBetMoney, 0);
            return result;
        }
        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            switch ((CSMSG_CODE)message.MsgCode)
            {
                case CSMSG_CODE.CS_CQ9_FreeSpinOptSelectRequest:
                    await onFreeSpinOptionSelectRequest(strUserID, agentID, message);
                    break;
            }
            await base.onProcMessage(strUserID, agentID, currency,message, userBonus, userBalance, isMustLose);
        }
        protected virtual async Task onFreeSpinOptionSelectRequest(string strUserID, int agentID, GITMessage message, bool isSendMsg = true)
        {
            try
            {
                int         selectOption    = (int) message.Pop();
                GITMessage  responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_CQ9_FreeSpinOptSelectResponse);
                responseMessage.Append(true);

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) || selectOption < 0)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BaseCQ9SlotBetInfo    betInfo = _dicUserBetInfos[strGlobalUserID];
                    BaseCQ9SlotSpinResult result  = _dicUserResultInfos[strGlobalUserID];

                    betInfo.pullRemainResponse();
                    BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                    if (selectOption >= startSpinData.FreeSpins.Count)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        OddAndIDData selectedFreeSpinInfo = startSpinData.FreeSpins[selectOption];
                        BasePPSlotSpinData freeSpinData   = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedFreeSpinInfo.ID), TimeSpan.FromSeconds(10.0));

                        betInfo.SpinData = freeSpinData;

                        List<string> freeSpinStrings = new List<string>();
                        for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                            freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData));

                        string strSpinResponse = freeSpinStrings[0];
                        if (freeSpinStrings.Count > 1)
                            betInfo.RemainReponses = buildResponseList(freeSpinStrings);

                        double selectedWin = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                        double maxWin      = startSpinData.MaxOdd * betInfo.TotalBet;

                        //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                        sumUpCompanyBetWin(agentID, 0.0, selectedWin - maxWin);

                        //이벤트인 경우 남은 머니를 에이전시잔고에 더해준다.
                        if (startSpinData.IsEvent && maxWin > selectedWin)
                            addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);

                        dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);
                        convertWinsByBet(resultContext, betInfo);
                        addDefaultParams(resultContext, CQ9Actions.FreeSpinOptionResult);
                        string strResponse = JsonConvert.SerializeObject(resultContext);
                        responseMessage.Append(strResponse);

                        if (_dicUserHistory.ContainsKey(strGlobalUserID))
                            _dicUserHistory[strGlobalUserID].Responses.Add(new CQ9ResponseHistory(CQ9Actions.FreeSpinOptionSelect, DateTime.UtcNow, strResponse));
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;
                }
                if(isSendMsg)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreeCQ9SlotGame::onFreeSpinOptionSelectRequest {0}", ex);
            }
        }
        protected virtual string addStartWinToResponse(string strResponse, BasePPSlotStartSpinData startSpinData)
        {
            dynamic firstResultContext  = JsonConvert.DeserializeObject<dynamic>(startSpinData.SpinStrings[0]);
            dynamic secondResultContext = JsonConvert.DeserializeObject<dynamic>(startSpinData.SpinStrings[1]);
            dynamic resultContext       = JsonConvert.DeserializeObject<dynamic>(strResponse);

            double totalWin     = (double)firstResultContext["TotalWin"];
            double scatterWin   = (double)secondResultContext["ScatterPayFromBaseGame"];

            if (!object.ReferenceEquals(resultContext["ScatterPayFromBaseGame"], null))
                resultContext["ScatterPayFromBaseGame"] = scatterWin;

            if (!object.ReferenceEquals(resultContext["TotalWinAmt"], null))
                resultContext["TotalWinAmt"] = (double)resultContext["TotalWinAmt"] + totalWin;

            int id = (int)resultContext["ID"];
            if(id == 142)
            {
                if (!object.ReferenceEquals(resultContext["AccumlateWinAmt"], null))
                    resultContext["AccumlateWinAmt"] = (double) resultContext["AccumlateWinAmt"] + scatterWin;
            }
            return JsonConvert.SerializeObject(resultContext);
        }
        protected override async Task<double> procRemainResponse(string strUserID, int agentID, BaseCQ9SlotBetInfo betInfo, double userBalance)
        {
            double winMoney = 0.0;
            do
            {
                BaseCQ9ActionToResponse actionToResponse = betInfo.pullRemainResponse();
                if(actionToResponse.ActionType == CQ9Actions.FreeSpinOptionSelect)
                {
                    BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                    GITMessage freeOptSelectMsg = new GITMessage((ushort)CSMSG_CODE.CS_CQ9_FreeSpinOptSelectRequest);
                    freeOptSelectMsg.Append(Pcg.Default.Next(0, startSpinData.FreeSpins.Count));

                    await onFreeSpinOptionSelectRequest(strUserID, agentID, freeOptSelectMsg, false);
                    continue;
                }

                BaseCQ9SlotSpinResult result = calculateResult(betInfo, actionToResponse.Response, false);

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (_dicUserHistory.ContainsKey(strGlobalUserID))
                    _dicUserHistory[strGlobalUserID].Responses.Add(new CQ9ResponseHistory(result.Action, DateTime.UtcNow, result.ResultString));

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                {
                    betInfo.RemainReponses = null;
                    if (result.TotalWin > 0.0)
                        winMoney = result.TotalWin;
                    break;
                }
            } while (true);
            return winMoney;
        }
        protected override async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //자연빵 1만개스핀 선택
                double sumOdd1 = 0.0;
                for (int i = 0; i < 100000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, 0);
                    if (spinData is BasePPSlotStartSpinData)
                    {
                        sumOdd1 += (spinData as BasePPSlotStartSpinData).StartOdd;
                        int random = Pcg.Default.Next(0, (spinData as BasePPSlotStartSpinData).FreeSpins.Count);
                        sumOdd1 += (spinData as BasePPSlotStartSpinData).FreeSpins[random].Odd;
                    }
                    else
                    {
                        sumOdd1 += spinData.SpinOdd;
                    }
                }
                stopWatch.Stop();
                long elapsed1 = stopWatch.ElapsedMilliseconds;

                stopWatch.Start();

                stopWatch.Start();
                //이벤트각 구간마다 2천개
                for (int i = 0; i < 6; i++)
                {
                    double[] rangeMins = new double[] { 10, 50, 100, 300, 500, 1000 };
                    double[] rangeMaxs = new double[] { 50, 100, 300, 500, 1000, 3000 };

                    for (int j = 0; j < 1000; j++)
                        await selectRangeSpinData(rangeMins[i], rangeMaxs[i]);
                }

                stopWatch.Stop();
                long elapsed4 = stopWatch.ElapsedMilliseconds;

                _logger.Info("{0} Performance Test Results:  {4}, {1}, {2}, {3}, {4}", this.GameName, elapsed1, sumOdd1 / 100000, elapsed4, _config.PayoutRate);
                Sender.Tell(true);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreeCQ9SlotGame::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
        }
    }

    public class CQ9FreeSpinOptionResponse
    {
        public int              Type                    { get; set; }
        public int              ID                      { get; set; }
        public int              Version                 { get; set; }
        public int              ErrorCode               { get; set; }
        public int              PlayerBet               { get; set; }
        public int              AccumlateWinAmt         { get; set; }
        public double           ScatterPayFromBaseGame  { get; set; }
        public int              MaxRound                { get; set; }
        public int              CurrentRound            { get; set; }
        public CQ9UDCDataSet    udcDataSet              { get; set; }

        public CQ9FreeSpinOptionResponse()
        {
            this.Type               = 3;
            this.ID                 = 144;
            this.Version            = 0;
            this.ErrorCode          = 0;
            this.AccumlateWinAmt    = 0;
            this.MaxRound           = 1;
            this.CurrentRound       = 1;
            this.udcDataSet         = new CQ9UDCDataSet();
        }
    }
    public  class CQ9UDCDataSet
    {
        public List<int> SelExtraData   { get; set; }
        public List<int> SelMultiplier  { get; set; }
        public List<int> SelSpinTimes   { get; set; }
        public List<int> SelWin         { get; set; }
        public List<int> PlayerSelected { get; set; }

        public CQ9UDCDataSet()
        {
            this.SelExtraData   = new List<int>();
            this.SelMultiplier  = new List<int>();
            this.SelSpinTimes   = new List<int>();
            this.SelWin         = new List<int>();
            this.PlayerSelected = new List<int>();
        }
    }
}

