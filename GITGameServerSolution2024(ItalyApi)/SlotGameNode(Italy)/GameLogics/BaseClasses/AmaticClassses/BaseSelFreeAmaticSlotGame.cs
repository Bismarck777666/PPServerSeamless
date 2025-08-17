using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class BaseSelFreeAmaticSlotGame : BaseAmaticSlotGame
    {
        protected SortedDictionary<double, int>[][]     _naturalChildFreeSpinOddProbsPerExtra   = new SortedDictionary<double, int>[][] { };
        protected SortedDictionary<double, int[]>[][]   _totalChildFreeSpinIDsPerExtra          = new SortedDictionary<double, int[]>[][] { };
        protected int[][]                               _naturalChildFreeSpinCountsPerExtra     = null;
        protected int[][]                               _totalChildFreeSpinCountsPerExtra       = null;

        //프리스핀구입기능이 있다면
        protected List<int>[]   _minStartFreeSpinIDsPerExtra    = new List<int>[] { };
        protected override bool HasSelectableFreeSpin           => true;

        public BaseSelFreeAmaticSlotGame()
        {
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

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.SelFreeMultiBase), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _spinDataDefaultBet         = response.DefaultBet;

                _naturalSpinOddProbsPerExtra            = new SortedDictionary<double, int>[LineTypeCnt];
                _naturalChildFreeSpinOddProbsPerExtra   = new SortedDictionary<double, int>[LineTypeCnt][];
                _naturalChildFreeSpinCountsPerExtra     = new int[LineTypeCnt][];
                _totalChildFreeSpinCountsPerExtra       = new int[LineTypeCnt][];
                _totalChildFreeSpinIDsPerExtra          = new SortedDictionary<double, int[]>[LineTypeCnt][];
                _naturalSpinCountPerExtra               = new int[LineTypeCnt];
                _emptySpinIDsPerExtra                   = new List<int>[LineTypeCnt];

                for (int i = 0; i < LineTypeCnt; i++)
                {
                    _naturalSpinOddProbsPerExtra[i]             = new SortedDictionary<double, int>();
                    _naturalChildFreeSpinOddProbsPerExtra[i]    = new SortedDictionary<double, int>[FreeSpinTypeCount];
                    _naturalChildFreeSpinCountsPerExtra[i]      = new int[FreeSpinTypeCount];
                    _totalChildFreeSpinCountsPerExtra[i]        = new int[FreeSpinTypeCount];
                    _totalChildFreeSpinIDsPerExtra[i]           = new SortedDictionary<double, int[]>[FreeSpinTypeCount];
                    _emptySpinIDsPerExtra[i]                    = new List<int>();
                }
                
                Dictionary<double, List<int>>[]   totalSpinOddIdsPerExtra         = new Dictionary<double, List<int>>[LineTypeCnt];
                Dictionary<double, List<int>>[]   freeSpinOddIdsPerExtra          = new Dictionary<double, List<int>>[LineTypeCnt];
                Dictionary<double, List<int>>[][] totalChildSpinOddIDsPerExtra    = new Dictionary<double, List<int>>[LineTypeCnt][];
                for (int i = 0; i < LineTypeCnt; i++)
                {
                    totalSpinOddIdsPerExtra[i]      = new Dictionary<double, List<int>>();
                    freeSpinOddIdsPerExtra[i]       = new Dictionary<double, List<int>>();
                    totalChildSpinOddIDsPerExtra[i] = new Dictionary<double, List<int>>[FreeSpinTypeCount];
                }

                double[]    freeSpinTotalOddPerExtra         = new double[LineTypeCnt];
                double[]    minFreeSpinTotalOddPerExtra      = new double[LineTypeCnt];
                int[]       freeSpinTotalCountPerExtra       = new int[LineTypeCnt];
                int[]       minFreeSpinTotalCountPerExtra    = new int[LineTypeCnt];

                for(int i = 0; i < LineTypeCnt; i++)
                {
                    freeSpinTotalOddPerExtra[i]         = 0.0;
                    minFreeSpinTotalOddPerExtra[i]      = 0.0;
                    freeSpinTotalCountPerExtra[i]       = 0;
                    minFreeSpinTotalCountPerExtra[i]    = 0;
                }

                _minStartFreeSpinIDsPerExtra = new List<int>[LineTypeCnt];
                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinExtraData spinBaseData = response.SpinBaseDatas[i] as SpinExtraData;
                    int extra = spinBaseData.Extra;

                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        if (spinBaseData.SpinType <= 100)
                        {
                            _naturalSpinCountPerExtra[extra]++;
                            if (_naturalSpinOddProbsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                                _naturalSpinOddProbsPerExtra[extra][spinBaseData.Odd]++;
                            else
                                _naturalSpinOddProbsPerExtra[extra][spinBaseData.Odd] = 1;
                        }
                        else if (spinBaseData.SpinType >= 200)
                        {
                            int freeSpinType = spinBaseData.SpinType - 200;
                            if (_naturalChildFreeSpinOddProbsPerExtra[extra][freeSpinType] == null)
                                _naturalChildFreeSpinOddProbsPerExtra[extra][freeSpinType] = new SortedDictionary<double, int>();

                            if (_naturalChildFreeSpinOddProbsPerExtra[extra][freeSpinType].ContainsKey(spinBaseData.Odd))
                                _naturalChildFreeSpinOddProbsPerExtra[extra][freeSpinType][spinBaseData.Odd]++;
                            else
                                _naturalChildFreeSpinOddProbsPerExtra[extra][freeSpinType][spinBaseData.Odd] = 1;
                        }
                    }

                    if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                        _emptySpinIDsPerExtra[extra].Add(spinBaseData.ID);

                    if (spinBaseData.SpinType <= 100)
                    {
                        if (!totalSpinOddIdsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                            totalSpinOddIdsPerExtra[extra].Add(spinBaseData.Odd, new List<int>());

                        totalSpinOddIdsPerExtra[extra][spinBaseData.Odd].Add(spinBaseData.ID);
                    }
                    else
                    {
                        int freeSpinType = spinBaseData.SpinType - 200;
                        if (totalChildSpinOddIDsPerExtra[extra][freeSpinType] == null)
                            totalChildSpinOddIDsPerExtra[extra][freeSpinType] = new Dictionary<double, List<int>>();

                        if (!totalChildSpinOddIDsPerExtra[extra][freeSpinType].ContainsKey(spinBaseData.Odd))
                            totalChildSpinOddIDsPerExtra[extra][freeSpinType][spinBaseData.Odd] = new List<int>();

                        totalChildSpinOddIDsPerExtra[extra][freeSpinType][spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.ID <= response.NormalMaxID)
                            _naturalChildFreeSpinCountsPerExtra[extra][freeSpinType]++;

                        _totalChildFreeSpinCountsPerExtra[extra][freeSpinType]++;
                    }

                    if (SupportPurchaseFree && spinBaseData.SpinType == 100)
                    {
                        freeSpinTotalCountPerExtra[extra]++;
                        freeSpinTotalOddPerExtra[extra] += spinBaseData.Odd;
                        if (!freeSpinOddIdsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                            freeSpinOddIdsPerExtra[extra].Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIdsPerExtra[extra][spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData is StartSpinExtraBaseData)
                        {
                            minFreeSpinTotalCountPerExtra[extra]++;
                            minFreeSpinTotalOddPerExtra[extra] += (spinBaseData as StartSpinExtraBaseData).MinRate;
                            _minStartFreeSpinIDsPerExtra[extra].Add(spinBaseData.ID);
                        }
                    }
                }
                
                _totalSpinOddIdsPerExtra = new SortedDictionary<double, int[]>[LineTypeCnt];
                for(int i = 0; i < LineTypeCnt; i++)
                {
                    _totalSpinOddIdsPerExtra[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIdsPerExtra[i])
                        _totalSpinOddIdsPerExtra[i].Add(pair.Key, pair.Value.ToArray());
                }

                for(int i = 0; i < LineTypeCnt; i++)
                {
                    for (int j = 0; j < FreeSpinTypeCount; j++)
                    {
                        if (totalChildSpinOddIDsPerExtra[i][j] == null)
                            continue;

                        _totalChildFreeSpinIDsPerExtra[i][j] = new SortedDictionary<double, int[]>();
                        foreach (KeyValuePair<double, List<int>> pair in totalChildSpinOddIDsPerExtra[i][j])
                            _totalChildFreeSpinIDsPerExtra[i][j].Add(pair.Key, pair.Value.ToArray());
                    }
                }
                
                if (SupportPurchaseFree)
                {
                    for(int i = 0; i < LineTypeCnt; i++)
                    {
                        _totalFreeSpinOddIdsPerExtra[i] = new SortedDictionary<double, int[]>();
                        foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIdsPerExtra[i])
                            _totalFreeSpinOddIdsPerExtra[i].Add(pair.Key, pair.Value.ToArray());

                        _freeSpinTotalCountPerExtra[i]      = freeSpinTotalCountPerExtra[i];
                        _minFreeSpinTotalCountPerExtra[i]   = minFreeSpinTotalCountPerExtra[i];
                        _totalFreeSpinWinRatePerExtra[i]    = freeSpinTotalOddPerExtra[i] / freeSpinTotalCountPerExtra[i];
                        _minFreeSpinWinRatePerExtra[i]      = minFreeSpinTotalOddPerExtra[i] / minFreeSpinTotalCountPerExtra[i];

                        if (_totalFreeSpinWinRatePerExtra[i] <= _minFreeSpinWinRatePerExtra[i] || _minFreeSpinTotalCountPerExtra[i] == 0)
                            _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                    }
                }

                for(int i = 0; i < LineTypeCnt; i++)
                {
                    if (this.SupportPurchaseFree && this.PurchaseFreeMultiples[0] > _totalFreeSpinWinRatePerExtra[i])
                        _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);

                    double winRate = 0.0;
                    foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbsPerExtra[i])
                        winRate += (pair.Key * pair.Value / _naturalSpinCountPerExtra[i]);
                }

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
            return new int[] { 200, 201, 202, 203, 204 };
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

        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                int                 spinDataID  = _minStartFreeSpinIDsPerExtra[getLineTypeFromPlayLine(betInfo)][Pcg.Default.Next(0, _minFreeSpinTotalCountPerExtra[getLineTypeFromPlayLine(betInfo)])];
                BasePPSlotSpinData  spinData    =  await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(spinDataID), TimeSpan.FromSeconds(10.0));

                if (!(spinData is BasePPSlotStartSpinData))
                    return null;

                BasePPSlotStartSpinData minStartSpinData = spinData as BasePPSlotStartSpinData;

                double minFreeOdd = 0.2 * getPurchaseMultiple(betInfo) - minStartSpinData.StartOdd;
                if (minFreeOdd < 0.0)
                    minFreeOdd = 0.0;

                double maxFreeOdd = 0.5 * getPurchaseMultiple(betInfo) - minStartSpinData.StartOdd;
                if (maxFreeOdd < 0.0)
                    maxFreeOdd = 0.0;

                minStartSpinData.FreeSpins    = new List<OddAndIDData>();
                int[]   freeSpinTypes         = PossibleFreeSpinTypes(minStartSpinData.FreeSpinGroup);
                double  maxOdd                = 0.0;
                for(int i = 0; i < freeSpinTypes.Length; i++)
                {
                    int          freeSpinType   = freeSpinTypes[i] - 200;
                    OddAndIDData childFreeSpin  = selectOddAndIDFromProbsWithRange(_totalChildFreeSpinIDsPerExtra[getLineTypeFromPlayLine(betInfo)][freeSpinType], minFreeOdd, maxFreeOdd);

                    if (childFreeSpin.Odd > maxOdd)
                        maxOdd = childFreeSpin.Odd;
                    minStartSpinData.FreeSpins.Add(childFreeSpin);
                }
                minStartSpinData.MaxOdd = minStartSpinData.StartOdd + maxOdd;
                return minStartSpinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelAmaticSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BaseAmaticSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                BasePPSlotSpinData spinDataEvent = await selectFreeRangeSpinData(agentID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                if (spinDataEvent != null)
                {
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(agentID))
                payoutRate = _agentPayoutRates[agentID];

            double targetC = getPurchaseMultiple(betInfo) * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRatePerExtra[getLineTypeFromPlayLine(betInfo)])
                targetC = _totalFreeSpinWinRatePerExtra[getLineTypeFromPlayLine(betInfo)];

            if (targetC < _minFreeSpinWinRatePerExtra[getLineTypeFromPlayLine(betInfo)])
                targetC = _minFreeSpinWinRatePerExtra[getLineTypeFromPlayLine(betInfo)];

            double x = (_totalFreeSpinWinRatePerExtra[getLineTypeFromPlayLine(betInfo)] - targetC) / (_totalFreeSpinWinRatePerExtra[getLineTypeFromPlayLine(betInfo)] - _minFreeSpinWinRatePerExtra[getLineTypeFromPlayLine(betInfo)]);

            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                return await selectMinStartFreeSpinData(betInfo);
            
            OddAndIDData startSpinOddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIdsPerExtra[getLineTypeFromPlayLine(betInfo)], _freeSpinTotalCountPerExtra[getLineTypeFromPlayLine(betInfo)]);
            BasePPSlotSpinData selectedSpinData = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(startSpinOddAndID.ID), TimeSpan.FromSeconds(10.0));
            if ((selectedSpinData == null) || !(selectedSpinData is BasePPSlotStartSpinData))
                return null;

            BasePPSlotStartSpinData startSpinData = selectedSpinData as BasePPSlotStartSpinData;
            buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, 0.0, 0.0, getLineTypeFromPlayLine(betInfo));
            return startSpinData;
        }

        protected override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, BaseAmaticSlotBetInfo betInfo)
        {
            BasePPSlotSpinData spinData = await base.selectRandomStop(agentID, betInfo);
            if (!(spinData is BasePPSlotStartSpinData))
                return spinData;

            BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
            buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsNaturalRandom, 0.0, 0.0, getLineTypeFromPlayLine(betInfo));
            return startSpinData;
        }

        protected virtual async Task<BasePPSlotSpinData> selectRangeSpinData(double minOdd, double maxOdd, int linetype)
        {
            BasePPSlotSpinData spinData = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectRangeSpinDataRequest(minOdd, maxOdd, getRangeID(minOdd, maxOdd)), TimeSpan.FromSeconds(10.0));
            if (spinData == null)
                return null;

            if (spinData is BasePPSlotStartSpinData)
            {
                BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
                double minFreeOdd = minOdd - startSpinData.StartOdd;
                if (minFreeOdd < 0.0)
                    minFreeOdd = 0.0;
                double maxFreeOdd = maxOdd - startSpinData.StartOdd;
                if (maxFreeOdd < 0.0)
                    maxFreeOdd = 0.0;

                buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsRangeLimited, minFreeOdd, maxFreeOdd, linetype);
            }
            return spinData;
        }

        protected virtual async Task<BasePPSlotSpinData> selectFreeRangeSpinData(int agentID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
        {
            BasePPSlotSpinData spinData = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectFreeRangeSpinDataRequest(minOdd, maxOdd, getRangeID(minOdd, maxOdd)), TimeSpan.FromSeconds(10.0));
            if (spinData == null)
                return null;

            BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
            double minFreeOdd = minOdd - startSpinData.StartOdd;
            if (minFreeOdd < 0.0)
                minFreeOdd = 0.0;
            double maxFreeOdd = maxOdd - startSpinData.StartOdd;
            if (maxFreeOdd < 0.0)
                maxFreeOdd = 0.0;

            buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsRangeLimited, minFreeOdd, maxFreeOdd, getLineTypeFromPlayLine(betInfo));
            return spinData;
        }
        
        protected virtual void buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd, int linetype)
        {
            startSpinData.FreeSpins = new List<OddAndIDData>();
            int[] freeSpinTypes = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);
            double maxFreeOdd = 0.0;
            for (int i = 0; i < freeSpinTypes.Length; i++)
            {
                int freeSpinType = freeSpinTypes[i] - 200;
                OddAndIDData childFreeSpin = null;
                if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                {
                    double odd      = selectOddFromProbs(_naturalChildFreeSpinOddProbsPerExtra[linetype][freeSpinType], _naturalChildFreeSpinCountsPerExtra[linetype][freeSpinType]);
                    int id          = _totalChildFreeSpinIDsPerExtra[linetype][freeSpinType][odd][Pcg.Default.Next(0, _totalChildFreeSpinIDsPerExtra[linetype][freeSpinType][odd].Length)];
                    childFreeSpin   = new OddAndIDData(id, odd);
                }
                else if (buildType == StartSpinBuildTypes.IsTotalRandom)
                {
                    childFreeSpin = selectOddAndIDFromProbs(_totalChildFreeSpinIDsPerExtra[linetype][freeSpinType], _totalChildFreeSpinCountsPerExtra[linetype][freeSpinType]);
                }
                else
                {
                    childFreeSpin = selectOddAndIDFromProbsWithRange(_totalChildFreeSpinIDsPerExtra[linetype][freeSpinType], minOdd, maxOdd);
                }

                if (childFreeSpin.Odd > maxFreeOdd)
                    maxFreeOdd = childFreeSpin.Odd;

                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd = startSpinData.StartOdd + maxFreeOdd;
        }

        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BaseAmaticSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (this.SupportPurchaseFree && betInfo.isPurchase)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            //배당구간이벤트만을 처리한다.
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                BasePPSlotSpinData spinDataEvent = await selectRangeSpinData(rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, getLineTypeFromPlayLine(betInfo));
                if (spinDataEvent != null)
                {
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            return await selectRandomStop(agentID, betInfo);
        }

        protected override async Task<BaseAmaticSlotSpinResult> generateSpinResult(BaseAmaticSlotBetInfo betInfo, string strUserID, int agentID, double userBalance, double betMoney, UserBonus userBonus, bool usePayLimit, bool isMustLose)
        {
            BasePPSlotSpinData          spinData    = null;
            BaseAmaticSlotSpinResult    result      = null;

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BaseAmaticActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, strGlobalUserID, nextResponse.Response, false, userBalance, betMoney);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            double pointUnit        = getPointUnit(betInfo);
            double totalBet         = betInfo.RelativeTotalBet * BettingButton[betInfo.PlayBet] * pointUnit;
            double  realBetMoney    = totalBet;

            if (SupportPurchaseFree && betInfo.isPurchase)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            if (SupportMoreBet && betInfo.isMoreBet)
                realBetMoney = totalBet * getMoreBetMultiple(betInfo);

            spinData = await selectRandomStop(agentID, userBonus, totalBet, false, isMustLose, betInfo);

            double totalWin = 0.0;
            if (spinData is BasePPSlotStartSpinData)
                totalWin = totalBet * (spinData as BasePPSlotStartSpinData).MaxOdd;
            else
                totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || spinData.IsEvent || checkAgentPayoutRate(agentID, realBetMoney, totalWin))
            {
                if (spinData.IsEvent)
                {
                    _bonusSendMessage   = null;
                    _rewardedBonusMoney = totalWin;
                    _isRewardedBonus    = true;
                }

                if (spinData is BasePPSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;

                result = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                return result;
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseStep > 0)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                emptyWin = totalBet * spinData.SpinOdd;

                if (spinData is BasePPSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData    = await selectEmptySpin(betInfo);
                result      = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                if (spinData is BasePPSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;
            }
            
            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            return result;
        }

        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            if(message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_FSOPTION)
            {
                await onFreeSpinOptionSelectRequest(strUserID, agentID, message, userBonus, userBalance, isMustLose);
            }
            else
            {
                await base.onProcMessage(strUserID, agentID, currency, message, userBonus, userBalance, isMustLose);
            }
        }

        protected virtual string addStartWinToResponse(string strResponse, BasePPSlotStartSpinData startSpinData)
        {
            AmaticPacket firstPacket    = new AmaticPacket(startSpinData.SpinStrings[0], Cols, FreeCols);
            AmaticPacket packet         = new AmaticPacket(strResponse, Cols, FreeCols);
            packet.win = firstPacket.win + packet.totalfreewin;

            return buildSpinString(packet);
        }

        protected virtual async Task onFreeSpinOptionSelectRequest(string strUserID, int agentID, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                int selectOption    = (int)message.Pop();
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_FSOPTION);

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) || selectOption < 0)
                {
                    _logger.Error("{0} option select error in BaseSelFreeAmaticSlotGame::onFreeSpinOptionSelectRequest", strGlobalUserID);
                }
                else
                {
                    BaseAmaticSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    BaseAmaticSlotSpinResult    result  = _dicUserResultInfos[strGlobalUserID];

                    BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                    if (selectOption >= startSpinData.FreeSpins.Count)
                    {
                        _logger.Error("{0} option select error in BaseSelFreeAmaticSlotGame::onFreeSpinOptionSelectRequest", strGlobalUserID);
                    }
                    else
                    {
                        OddAndIDData selectedFreeSpinInfo   = startSpinData.FreeSpins[selectOption];
                        BasePPSlotSpinData freeSpinData     = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedFreeSpinInfo.ID), TimeSpan.FromSeconds(10.0));

                        betInfo.SpinData = freeSpinData;

                        List<string> freeSpinStrings = new List<string>();
                        for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                            freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData));

                        string strSpinResponse = freeSpinStrings[0];
                        if (freeSpinStrings.Count > 1)
                            betInfo.RemainReponses = buildResponseList(freeSpinStrings);

                        double selectedWin  = (startSpinData.StartOdd + freeSpinData.SpinOdd) * (betInfo.RelativeTotalBet * BettingButton[getLineTypeFromPlayLine(betInfo)]);
                        double maxWin       = startSpinData.MaxOdd * (betInfo.RelativeTotalBet * BettingButton[getLineTypeFromPlayLine(betInfo)]);

                        sumUpCompanyBetWin(agentID, 0.0, selectedWin - maxWin);

                        AmaticPacket packet = new AmaticPacket(strSpinResponse, Cols, FreeCols);
                        convertWinsByBet(userBalance, packet, betInfo);
                        BaseAmaticSlotSpinResult spinResult = new BaseAmaticSlotSpinResult();
                        spinResult.ResultString = buildSpinString(packet);
                        responseMessage.Append(spinResult.ResultString);

                        _dicUserResultInfos[strGlobalUserID] = spinResult;
                        sendFreeOptionPickResult(spinResult, strGlobalUserID, 0.0, 0.0, "FreeOption", userBalance);
                        _dicUserLastBackupResultInfos[strGlobalUserID] = spinResult;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreeAmaticSlotGame::onFreeSpinOptionSelectRequest {0}", ex);
            }
        }

        protected virtual void sendFreeOptionPickResult(BaseAmaticSlotSpinResult spinResult, string strGlobalUserID, double betMoney, double winMoney, string strGameLog, double userBalance)
        {
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_FSOPTION);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), UserBetTypes.Normal);
            Sender.Tell(toUserResult, Self);
        }
    }
}
