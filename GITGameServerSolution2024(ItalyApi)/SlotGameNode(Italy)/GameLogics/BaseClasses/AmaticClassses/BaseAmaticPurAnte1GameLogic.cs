using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Newtonsoft.Json;
using Akka.Configuration;
using SlotGamesNode.Database;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
    class BaseAmaticPurAnte1GameLogic : BaseAmaticPurAnteGame
    {
        protected int[]                             _naturalAnteSpinCountPerExtra       = new int[] { };
        protected SortedDictionary<double, int>[]   _naturalAnteSpinOddProbsPerExtra    = new SortedDictionary<double, int>[] { };
        protected SortedDictionary<double, int[]>[] _totalAnteSpinOddIdsPerExtra        = new SortedDictionary<double, int[]>[] { };
        protected List<int>[]                       _emptyAnteSpinIDsPerExtra           = new List<int>[] { };

        public BaseAmaticPurAnte1GameLogic()
        {
            _naturalAnteSpinCountPerExtra       = new int[LineTypeCnt];
            _naturalAnteSpinOddProbsPerExtra    = new SortedDictionary<double, int>[LineTypeCnt];
            _totalAnteSpinOddIdsPerExtra        = new SortedDictionary<double, int[]>[LineTypeCnt];
            _emptyAnteSpinIDsPerExtra           = new List<int>[LineTypeCnt];
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase   = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
                bool isSuccess  = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoMutiBaseAnteRequest(), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _normalMaxID        = response.NormalMaxID;
                _spinDataDefaultBet = response.DefaultBet;

                Dictionary<double, List<int>>[] totalSpinOddIdsPerExtra     = new Dictionary<double, List<int>>[LineTypeCnt];
                Dictionary<double, List<int>>[] totalAnteSpinOddIdsPerExtra = new Dictionary<double, List<int>>[LineTypeCnt];
                Dictionary<double, List<int>>[] freeSpinOddIdsPerExtra      = new Dictionary<double, List<int>>[LineTypeCnt];
                double[] freeSpinTotalOddPerExtra     = new double[LineTypeCnt];
                double[] minFreeSpinTotalOddPerExtra  = new double[LineTypeCnt];
                int[] freeSpinTotalCountPerExtra      = new int[LineTypeCnt];
                int[] minFreeSpinTotalCountPerExtra   = new int[LineTypeCnt];
                
                for (int i = 0; i < LineTypeCnt; i++)
                {
                    _naturalSpinCountPerExtra[i]        = 0;
                    _naturalSpinOddProbsPerExtra[i]     = new SortedDictionary<double, int>();
                    _emptySpinIDsPerExtra[i]            = new List<int>();
                    _totalSpinOddIdsPerExtra[i]         = new SortedDictionary<double, int[]>();

                    _naturalAnteSpinCountPerExtra[i]    = 0;
                    _naturalAnteSpinOddProbsPerExtra[i] = new SortedDictionary<double, int>();
                    _emptyAnteSpinIDsPerExtra[i]        = new List<int>();
                    _totalAnteSpinOddIdsPerExtra[i]     = new SortedDictionary<double, int[]>();

                    _freeSpinTotalCountPerExtra[i]      = 0;
                    _minFreeSpinTotalCountPerExtra[i]   = 0;
                    _totalFreeSpinWinRatePerExtra[i]    = 0.0;
                    _minFreeSpinWinRatePerExtra[i]      = 0.0;

                    totalSpinOddIdsPerExtra[i]          = new Dictionary<double, List<int>>();
                    totalAnteSpinOddIdsPerExtra[i]      = new Dictionary<double, List<int>>();
                    freeSpinOddIdsPerExtra[i]           = new Dictionary<double, List<int>>();
                    freeSpinTotalCountPerExtra[i]       = 0;
                    freeSpinTotalOddPerExtra[i]         = 0.0;
                    minFreeSpinTotalCountPerExtra[i]    = 0;
                    minFreeSpinTotalOddPerExtra[i]      = 0.0;
                }

                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinExtraAnteData spinBaseData = response.SpinBaseDatas[i] as SpinExtraAnteData;
                    int extra = spinBaseData.Extra;

                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        if(spinBaseData.IsAnte == 0)
                        {
                            _naturalSpinCountPerExtra[extra]++;

                            if (_naturalSpinOddProbsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                                _naturalSpinOddProbsPerExtra[extra][spinBaseData.Odd]++;
                            else
                                _naturalSpinOddProbsPerExtra[extra][spinBaseData.Odd] = 1;
                        }
                        else
                        {
                            _naturalAnteSpinCountPerExtra[extra]++;

                            if (_naturalAnteSpinOddProbsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                                _naturalAnteSpinOddProbsPerExtra[extra][spinBaseData.Odd]++;
                            else
                                _naturalAnteSpinOddProbsPerExtra[extra][spinBaseData.Odd] = 1;
                        }
                    }

                    if(spinBaseData.IsAnte == 0)
                    {
                        if (!totalSpinOddIdsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                            totalSpinOddIdsPerExtra[extra].Add(spinBaseData.Odd, new List<int>());

                        totalSpinOddIdsPerExtra[extra][spinBaseData.Odd].Add(spinBaseData.ID);
                    }
                    else
                    {
                        if (!totalAnteSpinOddIdsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                            totalAnteSpinOddIdsPerExtra[extra].Add(spinBaseData.Odd, new List<int>());

                        totalAnteSpinOddIdsPerExtra[extra][spinBaseData.Odd].Add(spinBaseData.ID);
                    }

                    if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                    {
                        if(spinBaseData.IsAnte == 0)
                            _emptySpinIDsPerExtra[extra].Add(spinBaseData.ID);
                        else
                            _emptyAnteSpinIDsPerExtra[extra].Add(spinBaseData.ID);
                    }

                    if (SupportPurchaseFree && spinBaseData.SpinType > 0 && spinBaseData.IsAnte == 0)
                    {
                        freeSpinTotalCountPerExtra[spinBaseData.Extra]++;
                        freeSpinTotalOddPerExtra[spinBaseData.Extra] += spinBaseData.Odd;
                        if (!freeSpinOddIdsPerExtra[spinBaseData.Extra].ContainsKey(spinBaseData.Odd))
                            freeSpinOddIdsPerExtra[spinBaseData.Extra].Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIdsPerExtra[spinBaseData.Extra][spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.Odd >= PurchaseFreeMultiples[0] * 0.2 && spinBaseData.Odd <= PurchaseFreeMultiples[0] * 0.5)
                        {
                            minFreeSpinTotalCountPerExtra[spinBaseData.Extra]++;
                            minFreeSpinTotalOddPerExtra[spinBaseData.Extra] += spinBaseData.Odd;
                        }
                    }
                }

                for (int i = 0; i < LineTypeCnt; i++)
                {
                    _totalSpinOddIdsPerExtra[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIdsPerExtra[i])
                        _totalSpinOddIdsPerExtra[i].Add(pair.Key, pair.Value.ToArray());

                    _totalAnteSpinOddIdsPerExtra[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in totalAnteSpinOddIdsPerExtra[i])
                        _totalAnteSpinOddIdsPerExtra[i].Add(pair.Key, pair.Value.ToArray());
                }

                for (int i = 0; i < LineTypeCnt; i++)
                {
                    if (SupportPurchaseFree)
                    {
                        _totalFreeSpinOddIdsPerExtra[i] = new SortedDictionary<double, int[]>();
                        foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIdsPerExtra[i])
                            _totalFreeSpinOddIdsPerExtra[i].Add(pair.Key, pair.Value.ToArray());

                        _freeSpinTotalCountPerExtra[i] = freeSpinTotalCountPerExtra[i];
                        _minFreeSpinTotalCountPerExtra[i] = minFreeSpinTotalCountPerExtra[i];
                        _totalFreeSpinWinRatePerExtra[i] = freeSpinTotalOddPerExtra[i] / freeSpinTotalCountPerExtra[i];
                        _minFreeSpinWinRatePerExtra[i] = minFreeSpinTotalOddPerExtra[i] / minFreeSpinTotalCountPerExtra[i];

                        if (_totalFreeSpinWinRatePerExtra[i] <= _minFreeSpinWinRatePerExtra[i] || _minFreeSpinTotalCountPerExtra[i] == 0)
                            _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                    }

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

        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BaseAmaticSlotBetInfo betInfo)
        {
            if (!SupportMoreBet || !betInfo.isMoreBet)
                return await base.selectRandomStop(agentID, userBonus, baseBet, isChangedLineCount, isMustLose, betInfo);

            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalAnteSpinOddIdsPerExtra[getLineTypeFromPlayLine(betInfo)], rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            BasePPSlotSpinData testData  = await selectRandomStop(agentID, betInfo);
            return testData;
        }

        protected override OddAndIDData selectRandomOddAndID(int companyID, BaseAmaticSlotBetInfo betInfo)
        {
            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(companyID))
                payoutRate = _agentPayoutRates[companyID];

            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            int selectedID      = 0;
            double selectedOdd  = 0.0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedOdd = 0.0;
            }
            else if (SupportMoreBet && betInfo.isMoreBet)
            {
                selectedOdd = selectOddFromProbs(_naturalAnteSpinOddProbsPerExtra[getLineTypeFromPlayLine(betInfo)], _naturalAnteSpinCountPerExtra[getLineTypeFromPlayLine(betInfo)]);
                
                if (!_totalAnteSpinOddIdsPerExtra[getLineTypeFromPlayLine(betInfo)].ContainsKey(selectedOdd))
                    return null;
            }
            else
            {
                selectedOdd = selectOddFromProbs(_naturalSpinOddProbsPerExtra[getLineTypeFromPlayLine(betInfo)], _naturalSpinCountPerExtra[getLineTypeFromPlayLine(betInfo)]);
                if (!_totalSpinOddIdsPerExtra[getLineTypeFromPlayLine(betInfo)].ContainsKey(selectedOdd))
                    return null;
            }

            if (SupportMoreBet && betInfo.isMoreBet)
            {
                selectedID = _totalAnteSpinOddIdsPerExtra[getLineTypeFromPlayLine(betInfo)][selectedOdd][Pcg.Default.Next(0, _totalAnteSpinOddIdsPerExtra[getLineTypeFromPlayLine(betInfo)][selectedOdd].Length)];
            }
            else
            {
                selectedID = _totalSpinOddIdsPerExtra[getLineTypeFromPlayLine(betInfo)][selectedOdd][Pcg.Default.Next(0, _totalSpinOddIdsPerExtra[getLineTypeFromPlayLine(betInfo)][selectedOdd].Length)];
            }


            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID     = selectedID;
            selectedOddAndID.Odd    = selectedOdd;
            return selectedOddAndID;
        }

    }
}
