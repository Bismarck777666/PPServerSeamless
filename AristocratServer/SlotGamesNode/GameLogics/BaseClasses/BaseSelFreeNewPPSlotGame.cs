using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlotGamesNode.Database;
using Akka.Actor;
using GITProtocol;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
    public class BaseSelFreeNewPPSlotGame : BaseSelFreePPSlotGame
    {
        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(this.GameName)), "spinDatabase");
                bool isSuccess = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = null;
                if(!HasPurEnableOption)
                    response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadFreeOptSpinInfoRequest(), TimeSpan.FromSeconds(30.0));
                else
                    response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadFreeOptPurSpinInfoRequest(), TimeSpan.FromSeconds(30.0));

                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _normalMaxID                    = response.NormalMaxID;
                _naturalSpinOddProbs            = new SortedDictionary<double, int>();
                _naturalChildFreeSpinOddProbs   = new SortedDictionary<double, int>[FreeSpinTypeCount];
                _naturalChildFreeSpinCounts     = new int[FreeSpinTypeCount];
                _totalChildFreeSpinCounts       = new int[FreeSpinTypeCount];
                _totalChildFreeSpinIDs          = new SortedDictionary<double, int[]>[FreeSpinTypeCount];

                _spinDataDefaultBet = response.DefaultBet;
                _naturalSpinCount = 0;
                _emptySpinIDs = new List<int>();

                Dictionary<double, List<int>> totalSpinOddIds = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>> freeSpinOddIds = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>[] totalChildSpinOddIDs = new Dictionary<double, List<int>>[FreeSpinTypeCount];

                double freeSpinTotalOdd = 0.0;
                double minFreeSpinTotalOdd = 0.0;
                int freeSpinTotalCount = 0;
                int minFreeSpinTotalCount = 0;

                _minStartFreeSpinIDs = new List<int>();
                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinBaseData spinBaseData = response.SpinBaseDatas[i];
                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        if (spinBaseData.SpinType <= 100)
                        {
                            _naturalSpinCount++;
                            if (_naturalSpinOddProbs.ContainsKey(spinBaseData.Odd))
                                _naturalSpinOddProbs[spinBaseData.Odd]++;
                            else
                                _naturalSpinOddProbs[spinBaseData.Odd] = 1;
                        }
                        else if (spinBaseData.SpinType >= 200)
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

                    if (SupportPurchaseFree && spinBaseData.SpinType == 100)
                    {
                        if (HasPurEnableOption && !(spinBaseData as PurEnabledStartSpinData).PurEnabled)
                            continue;

                        freeSpinTotalCount++;
                        freeSpinTotalOdd += (spinBaseData as Start2ndSpinBaseData).AllFreeRate;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if ((spinBaseData as Start2ndSpinBaseData).MinFreeRate > 0.0)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += (spinBaseData as Start2ndSpinBaseData).MinFreeRate;
                            _minStartFreeSpinIDs.Add(spinBaseData.ID);
                        }
                    }
                }
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                for (int i = 0; i < FreeSpinTypeCount; i++)
                {
                    if (totalChildSpinOddIDs[i] == null)
                        continue;

                    _totalChildFreeSpinIDs[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in totalChildSpinOddIDs[i])
                        _totalChildFreeSpinIDs[i].Add(pair.Key, pair.Value.ToArray());
                }

                if (SupportPurchaseFree)
                {
                    _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIds)
                        _totalFreeSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                    _freeSpinTotalCount     = freeSpinTotalCount;
                    _minFreeSpinTotalCount  = minFreeSpinTotalCount;
                    _totalFreeSpinWinRate   = freeSpinTotalOdd / freeSpinTotalCount;
                    _minFreeSpinWinRate     = minFreeSpinTotalOdd / minFreeSpinTotalCount;

                    if (_totalFreeSpinWinRate <= _minFreeSpinWinRate || _minFreeSpinTotalCount == 0)
                        _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                }

                if (this.SupportMoreBet)
                {
                    int naturalEmptyCount = _naturalSpinOddProbs[0.0];
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);

                    double moreBetWinRate = 0.0;
                    foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    {
                        moreBetWinRate += (pair.Key * pair.Value / (_naturalSpinCount - _anteBetMinusZeroCount));
                    }
                    if (_anteBetMinusZeroCount > naturalEmptyCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", this.GameName);
                }

                if (this.SupportPurchaseFree && this.PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);

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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int companyID, BasePPSlotBetInfo betInfo, double baseBet)
        {
            double payoutRate = _config.PayoutRate;
            if (_companyPayoutRates.ContainsKey(companyID))
                payoutRate = _companyPayoutRates[companyID];

            double targetC = PurchaseFreeMultiple * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRate)
                targetC = _totalFreeSpinWinRate;

            if (targetC < _minFreeSpinWinRate)
                targetC = _minFreeSpinWinRate;

            double x = (_totalFreeSpinWinRate - targetC) / (_totalFreeSpinWinRate - _minFreeSpinWinRate);
            double y = 1.0 - x;

            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                return await selectMinStartFreeSpinData(betInfo);

            OddAndIDData startSpinOddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIds, _freeSpinTotalCount);
            BasePPSlotSpinData selectedSpinData = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(startSpinOddAndID.ID), TimeSpan.FromSeconds(10.0));
            if ((selectedSpinData == null) || !(selectedSpinData is BasePPSlotStartSpinData))
                return null;

            BasePPSlotStartSpinData startSpinData = selectedSpinData as BasePPSlotStartSpinData;
            buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, 0.0, 0.0);
            return startSpinData;
        }


    }
}
