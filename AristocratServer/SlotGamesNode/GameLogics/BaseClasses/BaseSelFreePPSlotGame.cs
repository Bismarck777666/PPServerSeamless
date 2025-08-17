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

namespace SlotGamesNode.GameLogics
{
    public class BasePPSlotStartSpinData : BasePPSlotSpinData
    {
        public double                           StartOdd        { get; set; }        
        public int                              FreeSpinGroup   { get; set; }
        public List<int>                        PossibleRanges  { get; set; }
        public List<OddAndIDData>               FreeSpins       { get; set; }
        public double                           MaxOdd          { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.StartOdd       = reader.ReadDouble();
            this.FreeSpinGroup  = reader.ReadInt32();
            this.PossibleRanges = SerializeUtils.readIntList(reader);
            this.MaxOdd         = reader.ReadDouble();

            this.FreeSpins      = new List<OddAndIDData>();
            int count = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                OddAndIDData oddAndID = new OddAndIDData();
                oddAndID.ID     = reader.ReadInt32();
                oddAndID.Odd    = reader.ReadDouble();
                this.FreeSpins.Add(oddAndID);
            }
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.StartOdd);
            writer.Write(this.FreeSpinGroup);
            SerializeUtils.writeIntList(writer, this.PossibleRanges);
            writer.Write(this.MaxOdd);
            if (this.FreeSpins == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.FreeSpins.Count);
                for (int i = 0; i < this.FreeSpins.Count; i++)
                {
                    writer.Write(this.FreeSpins[i].ID);
                    writer.Write(this.FreeSpins[i].Odd);
                }
            }            
        }

    }

    public enum StartSpinBuildTypes
    {
        IsNaturalRandom = 0,
        IsTotalRandom   = 1,
        IsRangeLimited  = 2,
    }

    public class BaseSelFreePPSlotGame : BasePPSlotGame
    {

        protected SortedDictionary<double, int>   [] _naturalChildFreeSpinOddProbs  = null;
        protected SortedDictionary<double, int[]> [] _totalChildFreeSpinIDs         = null;
        protected int[]                              _naturalChildFreeSpinCounts    = null;
        protected int[]                              _totalChildFreeSpinCounts      = null;

        protected List<int>                         _minStartFreeSpinIDs           = new List<int>();

        protected override bool HasSelectableFreeSpin
        {
            get { return true;  }
        }

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

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.SelFreeSpin), TimeSpan.FromSeconds(30.0));
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

                _spinDataDefaultBet             = response.DefaultBet;
                _naturalSpinCount               = 0;
                _emptySpinIDs                   = new List<int>();

                Dictionary<double, List<int>>   totalSpinOddIds         = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>   freeSpinOddIds          = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>[] totalChildSpinOddIDs    = new Dictionary<double, List<int>>[FreeSpinTypeCount];

                double freeSpinTotalOdd         = 0.0;
                double minFreeSpinTotalOdd      = 0.0;
                int    freeSpinTotalCount       = 0;
                int    minFreeSpinTotalCount    = 0;

                _minStartFreeSpinIDs = new List<int>();
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

                    if (SupportPurchaseFree && spinBaseData.SpinType == 100)
                    {
                        freeSpinTotalCount++;
                        freeSpinTotalOdd += spinBaseData.Odd;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData is StartSpinBaseData)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += (spinBaseData as StartSpinBaseData).MinRate;
                            _minStartFreeSpinIDs.Add(spinBaseData.ID);
                        }
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

                /*
                _logger.Info("Loading completed spin data of game {0} winrate: {1}, freespinwinrate: {2}, antezerominuscount: {3}, empty spin count: {4}, minfreespinrate: {5}",
                    this.GameName, Math.Round(winRate, 2), Math.Round(_totalFreeSpinWinRate, 2), _anteBetMinusZeroCount, _emptySpinIDs.Count, Math.Round(_minFreeSpinWinRate, 2));
                */
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
            return new int[] { 200, 201, 202, 203, 204, 205, 206};
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

            return 1;
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int                 spinDataID              = _minStartFreeSpinIDs[Pcg.Default.Next(0, _minFreeSpinTotalCount)];
                BasePPSlotSpinData  spinData                =  await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(spinDataID), TimeSpan.FromSeconds(10.0));

                if (!(spinData is BasePPSlotStartSpinData))
                    return null;

                BasePPSlotStartSpinData minStartSpinData = spinData as BasePPSlotStartSpinData;

                double minFreeOdd = 0.2 * PurchaseFreeMultiple - minStartSpinData.StartOdd;
                if (minFreeOdd < 0.0)
                    minFreeOdd = 0.0;

                double maxFreeOdd = 0.5 * PurchaseFreeMultiple - minStartSpinData.StartOdd;
                if (maxFreeOdd < 0.0)
                    maxFreeOdd = 0.0;

                minStartSpinData.FreeSpins    = new List<OddAndIDData>();
                int[]   freeSpinTypes         = PossibleFreeSpinTypes(minStartSpinData.FreeSpinGroup);
                double  maxOdd                = 0.0;
                for(int i = 0; i < freeSpinTypes.Length; i++)
                {
                    int          freeSpinType   = freeSpinTypes[i] - 200;
                    OddAndIDData childFreeSpin  = selectOddAndIDFromProbsWithRange(_totalChildFreeSpinIDs[freeSpinType], minFreeOdd, maxFreeOdd);

                    if (childFreeSpin.Odd > maxOdd)
                        maxOdd = childFreeSpin.Odd;
                    minStartSpinData.FreeSpins.Add(childFreeSpin);
                }
                minStartSpinData.MaxOdd = minStartSpinData.StartOdd + maxOdd;
                return minStartSpinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
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
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int companyID, BasePPSlotBetInfo betInfo,  bool isMoreBet)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(companyID, betInfo, isMoreBet);
            do
            {
                BasePPSlotSpinData spinData = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
                if (!(spinData is BasePPSlotStartSpinData))
                    return spinData;

                if (selectedOddAndID.ID > _normalMaxID)
                {
                    int naturalID = pickNaturalIDFromTotalOddIds(selectedOddAndID.Odd);
                    selectedOddAndID.ID = naturalID;
                    continue;
                }

                BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
                buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsNaturalRandom, 0.0, 0.0);
                return startSpinData;
            } while (true);
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
                if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                {
                    double odd = selectOddFromProbs(_naturalChildFreeSpinOddProbs[freeSpinType], _naturalChildFreeSpinCounts[freeSpinType]);
                    int id = _totalChildFreeSpinIDs[freeSpinType][odd][Pcg.Default.Next(0, _totalChildFreeSpinIDs[freeSpinType][odd].Length)];
                    childFreeSpin = new OddAndIDData(id, odd);
                }
                else if (buildType == StartSpinBuildTypes.IsTotalRandom)
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
        public override async Task<BasePPSlotSpinData> selectRandomStop(int companyID, double baseBet, BasePPSlotBetInfo betInfo)
        {
            if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(companyID, betInfo, baseBet);

            if (SupportMoreBet && betInfo.MoreBet)
                return await selectRandomStop(companyID, betInfo, true);
            else
                return await selectRandomStop(companyID, betInfo, false);
        }
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int companyID, UserBonus bonus, bool usePayLimit, PPFreeSpinInfo freeSpinInfo)
        {
            BasePPSlotSpinData      spinData    = null;
            BasePPSlotSpinResult    result      = null;
            _isRewardedCashback = false;
            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false, freeSpinInfo);

                if (!betInfo.HasRemainResponse)
                {
                    betInfo.RemainReponses = null;

                    if (bonus != null && bonus is UserPPRacePrizeBonus)
                    {
                        UserPPRacePrizeBonus ppRaceBonus = bonus as UserPPRacePrizeBonus;

                        string prizeString = string.Empty;
                        if (ppRaceBonus.PrizeType == "A" && betInfo.TotalBet >= ppRaceBonus.MinBetLimit)
                        {
                            prizeString = string.Format("ev={0}~{1},{2},{3}", "MR", ppRaceBonus.RaceID, ppRaceBonus.PrizeType, ppRaceBonus.Amount);
                            _rewardedBonusMoney = ppRaceBonus.Amount;
                            _isRewardedBonus = true;
                        }
                        else if (ppRaceBonus.PrizeType == "BM" && betInfo.TotalBet >= ppRaceBonus.MinBetLimit)
                        {
                            prizeString = string.Format("ev={0}~{1},{2},{3},{4},{5}", "MR", ppRaceBonus.RaceID, ppRaceBonus.PrizeType, ppRaceBonus.BetMultiplier * Convert.ToDouble(betInfo.TotalBet), Convert.ToDouble(betInfo.TotalBet), ppRaceBonus.BetMultiplier);
                            _rewardedBonusMoney = ppRaceBonus.BetMultiplier * Convert.ToDouble(betInfo.TotalBet);
                            _isRewardedBonus = true;
                        }

                        if (_isRewardedBonus)
                        {
                            _dbWriter.Tell(new PPRaceWinnerDBItem(ppRaceBonus.RaceID, ppRaceBonus.PrizeID, ppRaceBonus.AgentID, strUserID, 1,
                                ppRaceBonus.CuntryCode, ppRaceBonus.Currency, Math.Round(betInfo.TotalBet, 2), Math.Round(_rewardedBonusMoney, 2), 1,
                                GameName, ppRaceBonus.PrizeType, ppRaceBonus.IsAgent, DateTime.UtcNow, DateTime.UtcNow));
                        }

                        result.ResultString = string.Format("{0}&{1}", result.ResultString, prizeString);
                        result.TotalWin += _rewardedBonusMoney;
                    }
                    if (bonus != null && bonus is UserPPCashback)
                    {
                        UserPPCashback ppCashback = bonus as UserPPCashback;

                        string cashbackPeriod = "Daily";
                        if (ppCashback.Period == 1)
                        {
                            cashbackPeriod = "Weekly";
                        }
                        else if (ppCashback.Period == 2)
                        {
                            cashbackPeriod = "Monthly";
                        }
                        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes($"{cashbackPeriod} cashback: {this.getCurrencySymbol(ppCashback.Currency)} {ppCashback.Cashback.ToString("N2")}");
                        string prizeString = $"ev=MR~{ppCashback.CashbackID},G,{Convert.ToBase64String(byteArray)}";

                        _rewardedBonusMoney = ppCashback.Cashback;
                        _isRewardedCashback = true;

                        if (_isRewardedCashback)
                        {
                            _dbWriter.Tell(new PPCashbackWinnerDBItem(ppCashback.CashbackID, ppCashback.AgentID, strUserID, ppCashback.CuntryCode,
                                ppCashback.Currency, ppCashback.Cashback, GameName, ppCashback.IsAgent, ppCashback.Period, ppCashback.PeriodKey, DateTime.UtcNow));
                        }

                        result.ResultString = string.Format("{0}&{1}", result.ResultString, prizeString);
                        result.TotalWin += _rewardedBonusMoney;
                        ppCashback.IsRewarded = true;
                    }
                }
                return result;
            }

            float totalBet      = betInfo.TotalBet;
            double realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            if (SupportMoreBet && betInfo.MoreBet)
                realBetMoney = totalBet * MoreBetMultiple;

            spinData = await selectRandomStop(companyID, totalBet, betInfo);
            double totalWin = 0.0;
            if (spinData is BasePPSlotStartSpinData)
                totalWin = totalBet * (spinData as BasePPSlotStartSpinData).MaxOdd;
            else
                totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || checkCompanyPayoutRate(companyID, realBetMoney, totalWin))
            {
                if (spinData is BasePPSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;
                result = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);

                if (!betInfo.HasRemainResponse && bonus != null && bonus is UserPPRacePrizeBonus)
                {
                    UserPPRacePrizeBonus ppRaceBonus = bonus as UserPPRacePrizeBonus;
                    if (result.NextAction == ActionTypes.DOSPIN)
                    {
                        result.NextAction = convertStringToActionType("c");
                        Dictionary<string, string> dicParams = splitResponseToParams(result.ResultString);
                        dicParams["na"] = "c";
                        result.ResultString = convertKeyValuesToString(dicParams);
                    }

                    string prizeString = string.Empty;
                    if (ppRaceBonus.PrizeType == "A" && betInfo.TotalBet >= ppRaceBonus.MinBetLimit)
                    {
                        prizeString = string.Format("ev={0}~{1},{2},{3}", "MR", ppRaceBonus.RaceID, ppRaceBonus.PrizeType, ppRaceBonus.Amount);
                        _rewardedBonusMoney = ppRaceBonus.Amount;
                        _isRewardedBonus = true;
                    }
                    else if (ppRaceBonus.PrizeType == "BM" && betInfo.TotalBet >= ppRaceBonus.MinBetLimit)
                    {
                        prizeString = string.Format("ev={0}~{1},{2},{3},{4},{5}", "MR", ppRaceBonus.RaceID, ppRaceBonus.PrizeType, ppRaceBonus.BetMultiplier * Convert.ToDouble(betInfo.TotalBet), Convert.ToDouble(betInfo.TotalBet), ppRaceBonus.BetMultiplier);
                        _rewardedBonusMoney = ppRaceBonus.BetMultiplier * Convert.ToDouble(betInfo.TotalBet);
                        _isRewardedBonus = true;
                    }

                    if (_isRewardedBonus)
                    {
                        _dbWriter.Tell(new PPRaceWinnerDBItem(ppRaceBonus.RaceID, ppRaceBonus.PrizeID, ppRaceBonus.AgentID, strUserID, 1,
                            ppRaceBonus.CuntryCode, ppRaceBonus.Currency, Math.Round(betInfo.TotalBet, 2), Math.Round(_rewardedBonusMoney, 2), 1,
                            GameName, ppRaceBonus.PrizeType, ppRaceBonus.IsAgent, DateTime.UtcNow, DateTime.UtcNow));
                    }

                    result.ResultString = string.Format("{0}&{1}", result.ResultString, prizeString);
                    result.TotalWin += _rewardedBonusMoney;
                }

                if (!betInfo.HasRemainResponse && bonus != null && bonus is UserPPCashback)
                {
                    if (result.NextAction == ActionTypes.DOSPIN)
                    {
                        result.NextAction = convertStringToActionType("c");
                        Dictionary<string, string> dicParams = splitResponseToParams(result.ResultString);
                        dicParams["na"] = "c";
                        result.ResultString = convertKeyValuesToString(dicParams);
                    }

                    UserPPCashback ppCashback = bonus as UserPPCashback;
                    string cashbackPeriod = "Daily";
                    if (ppCashback.Period == 1)
                    {
                        cashbackPeriod = "Weekly";
                    }
                    else if (ppCashback.Period == 2)
                    {
                        cashbackPeriod = "Monthly";
                    }
                    byte[] byteArray = System.Text.Encoding.UTF8.GetBytes($"{cashbackPeriod} cashback: {this.getCurrencySymbol(ppCashback.Currency)} {ppCashback.Cashback.ToString("N2")}");
                    string prizeString = $"ev=MR~{ppCashback.CashbackID},G,{Convert.ToBase64String(byteArray)}";

                    _rewardedBonusMoney = ppCashback.Cashback;
                    _isRewardedCashback = true;

                    if (_isRewardedCashback)
                    {
                        _dbWriter.Tell(new PPCashbackWinnerDBItem(ppCashback.CashbackID, ppCashback.AgentID, strUserID, ppCashback.CuntryCode,
                            ppCashback.Currency, ppCashback.Cashback, GameName, ppCashback.IsAgent, ppCashback.Period, ppCashback.PeriodKey, DateTime.UtcNow));
                    }

                    result.ResultString = string.Format("{0}&{1}", result.ResultString, prizeString);
                    result.TotalWin += _rewardedBonusMoney;
                    ppCashback.IsRewarded = true;
                }

                return result;
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result  = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
                emptyWin = totalBet * spinData.SpinOdd;

                if (spinData is BasePPSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;

                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(companyID, betInfo);
                result   = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
                if (spinData is BasePPSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;
            }
            sumUpCompanyBetWin(companyID, realBetMoney, emptyWin);

            if (!betInfo.HasRemainResponse && bonus != null && bonus is UserPPRacePrizeBonus)
            {
                UserPPRacePrizeBonus ppRaceBonus = bonus as UserPPRacePrizeBonus;
                if (result.NextAction == ActionTypes.DOSPIN)
                {
                    result.NextAction = convertStringToActionType("c");
                    Dictionary<string, string> dicParams = splitResponseToParams(result.ResultString);
                    dicParams["na"] = "c";
                    result.ResultString = convertKeyValuesToString(dicParams);
                }

                string prizeString = string.Empty;
                if (ppRaceBonus.PrizeType == "A" && betInfo.TotalBet >= ppRaceBonus.MinBetLimit)
                {
                    prizeString = string.Format("ev={0}~{1},{2},{3}", "MR", ppRaceBonus.RaceID, ppRaceBonus.PrizeType, ppRaceBonus.Amount);
                    _rewardedBonusMoney = ppRaceBonus.Amount;
                    _isRewardedBonus = true;
                }
                else if (ppRaceBonus.PrizeType == "BM" && betInfo.TotalBet >= ppRaceBonus.MinBetLimit)
                {
                    prizeString = string.Format("ev={0}~{1},{2},{3},{4},{5}", "MR", ppRaceBonus.RaceID, ppRaceBonus.PrizeType, ppRaceBonus.BetMultiplier * Convert.ToDouble(betInfo.TotalBet), Convert.ToDouble(betInfo.TotalBet), ppRaceBonus.BetMultiplier);
                    _rewardedBonusMoney = ppRaceBonus.BetMultiplier * Convert.ToDouble(betInfo.TotalBet);
                    _isRewardedBonus = true;
                }

                if (_isRewardedBonus)
                {
                    _dbWriter.Tell(new PPRaceWinnerDBItem(ppRaceBonus.RaceID, ppRaceBonus.PrizeID, ppRaceBonus.AgentID, strUserID, 1,
                        ppRaceBonus.CuntryCode, ppRaceBonus.Currency, Math.Round(betInfo.TotalBet, 2), Math.Round(_rewardedBonusMoney, 2), 1,
                        GameName, ppRaceBonus.PrizeType, ppRaceBonus.IsAgent, DateTime.UtcNow, DateTime.UtcNow));
                }

                result.ResultString = string.Format("{0}&{1}", result.ResultString, prizeString);
                result.TotalWin += _rewardedBonusMoney;
            }
            if (bonus != null && bonus is UserPPCashback)
            {
                UserPPCashback ppCashback = bonus as UserPPCashback;

                if (result.NextAction == ActionTypes.DOSPIN)
                {
                    result.NextAction = convertStringToActionType("c");
                    Dictionary<string, string> dicParams = splitResponseToParams(result.ResultString);
                    dicParams["na"] = "c";
                    result.ResultString = convertKeyValuesToString(dicParams);
                }


                string cashbackPeriod = "Daily";
                if (ppCashback.Period == 1)
                {
                    cashbackPeriod = "Weekly";
                }
                else if (ppCashback.Period == 2)
                {
                    cashbackPeriod = "Monthly";
                }
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes($"{cashbackPeriod} cashback: {this.getCurrencySymbol(ppCashback.Currency)} {ppCashback.Cashback.ToString("N2")}");
                string prizeString = $"ev=MR~{ppCashback.CashbackID},G,{Convert.ToBase64String(byteArray)}";

                _rewardedBonusMoney = ppCashback.Cashback;
                _isRewardedCashback = true;

                if (_isRewardedCashback)
                {
                    _dbWriter.Tell(new PPCashbackWinnerDBItem(ppCashback.CashbackID, ppCashback.AgentID, strUserID, ppCashback.CuntryCode,
                        ppCashback.Currency, ppCashback.Cashback, GameName, ppCashback.IsAgent, ppCashback.Period, ppCashback.PeriodKey, DateTime.UtcNow));
                }

                result.ResultString = string.Format("{0}&{1}", result.ResultString, prizeString);
                result.TotalWin += _rewardedBonusMoney;
                ppCashback.IsRewarded = true;
            }

            return result;
        }

        protected override async Task onProcMessage(string strUserID, int companyID, GITMessage message,UserBonus bonus, double userBalance, Currencies currency, int agentMoneyMode)
        {
            if(message.MsgCode == (ushort)CSMSG_CODE.CS_PP_FSOPTION)
            {
                await onFSOption(strUserID, companyID, message, userBalance);
            }
            await base.onProcMessage(strUserID, companyID, message, bonus,userBalance, currency, agentMoneyMode);
        }
        protected virtual string addStartWinToResponse(string strResponse, double startOdd)
        {
            Dictionary<string, string> dicParams = splitResponseToParams(strResponse);

            if(dicParams.ContainsKey("tw"))
            {
                double oldTW = 0.0;                
                if(double.TryParse(dicParams["tw"], out oldTW))
                {
                    double newTW = oldTW + startOdd * _spinDataDefaultBet;
                    dicParams["tw"] = Math.Round(newTW, 2).ToString();
                    return convertKeyValuesToString(dicParams);
                }
                else
                {
                    return strResponse;
                }
            }
            return strResponse;
        }
        protected async Task onFSOption(string strUserID, int companyID, GITMessage message, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strUserID) || !_dicUserBetInfos.ContainsKey(strUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strUserID];
                    BasePPSlotSpinResult    result  = _dicUserResultInfos[strUserID];
                    if ((result.NextAction != ActionTypes.DOFSOPTION) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                        if(ind >= startSpinData.FreeSpins.Count)
                        {
                            responseMessage.Append("unlogged");
                        }
                        else
                        {
                            OddAndIDData selectedFreeSpinInfo = startSpinData.FreeSpins[ind];
                            BasePPSlotSpinData freeSpinData   = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedFreeSpinInfo.ID), TimeSpan.FromSeconds(10.0)) ;

                            preprocessSelectedFreeSpin(freeSpinData, betInfo);

                            betInfo.SpinData = freeSpinData;

                            List<string> freeSpinStrings = new List<string>();
                            for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                            string strSpinResponse      = freeSpinStrings[0];
                            if (freeSpinStrings.Count > 1)
                                betInfo.RemainReponses  = buildResponseList(freeSpinStrings);

                            double selectedWin  = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin       = startSpinData.MaxOdd * betInfo.TotalBet;
                            
                            sumUpCompanyBetWin(companyID, 0.0, selectedWin - maxWin);

                            Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                            if (SupportMoreBet)
                            {
                                if (betInfo.MoreBet)
                                    dicParams["bl"] = "1";
                                else
                                    dicParams["bl"] = "0";
                            }
                            result.BonusResultString    = convertKeyValuesToString(dicParams);
                            addDefaultParams(dicParams, userBalance, index, counter);
                            ActionTypes nextAction      = convertStringToActionType(dicParams["na"]);
                            string strResponse          = convertKeyValuesToString(dicParams);

                            responseMessage.Append(strResponse);

                            if (_dicUserHistory.ContainsKey(strUserID) && _dicUserHistory[strUserID].log.Count > 0)
                                addFSOptionActionHistory(strUserID, ind, strResponse, index, counter);

                            result.NextAction = nextAction;
                        }
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strUserID);
                    }
                }
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreePPSlotGame::onFSOption {0}", ex);
            }
        }        
        protected virtual void preprocessSelectedFreeSpin(BasePPSlotSpinData freeSpinData, BasePPSlotBetInfo betInfo)
        {

        }
        protected virtual void addFSOptionActionHistory(string strUserID, int ind, string strResponse, int index, int counter)
        {
            if (!_dicUserHistory.ContainsKey(strUserID) || _dicUserHistory[strUserID].log.Count == 0)
                return;

            if (_dicUserHistory[strUserID].bet == 0.0)
                return;

            BasePPHistoryItem item = new BasePPHistoryItem();
            item.cr = string.Format("symbol={0}&repeat=0&action=doFSOption&index={1}&counter={2}&ind={3}", SymbolName, index, counter, ind);
            item.sr = strResponse;
            _dicUserHistory[strUserID].log.Add(item);
        }
        protected virtual void addDoBonusActionHistory(string strUserID, int ind, string strResponse, int index, int counter)
        {
            if (!_dicUserHistory.ContainsKey(strUserID) || _dicUserHistory[strUserID].log.Count == 0)
                return;

            if (_dicUserHistory[strUserID].bet == 0.0)
                return;

            BasePPHistoryItem item = new BasePPHistoryItem();
            item.cr = string.Format("symbol={0}&repeat=0&action=doBonus&index={1}&counter={2}&ind={3}", SymbolName, index, counter, ind);
            item.sr = strResponse;
            _dicUserHistory[strUserID].log.Add(item);
        }
    }
}
