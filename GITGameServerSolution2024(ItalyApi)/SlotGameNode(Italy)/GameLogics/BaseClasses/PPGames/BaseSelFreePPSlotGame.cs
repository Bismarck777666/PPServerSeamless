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
using GITProtocol.Utils;
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

        //프리스핀구입기능이 있다면
        protected List<int>                         _minStartFreeSpinIDs           = new List<int>();

        protected override bool HasSelectableFreeSpin
        {
            get { return true;  }
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase   = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(_providerName, GameName)), "spinDatabase");
                bool isSuccess  = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.SelFreeSpin), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", GameName);
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

                if (SupportMoreBet)
                {
                    int naturalEmptyCount = _naturalSpinOddProbs[0.0];
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);

                    double moreBetWinRate = 0.0;
                    foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    {
                        moreBetWinRate += (pair.Key * pair.Value / (_naturalSpinCount - _anteBetMinusZeroCount));
                    }
                    if (_anteBetMinusZeroCount > naturalEmptyCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", GameName);
                }

                if (SupportPurchaseFree && PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", GameName);

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
            if (minOdd == 1000.0 && maxOdd == 1500.0)
                return 7;
            if (minOdd == 1500.0 && maxOdd == 3000.0)
                return 8;

            return 1;
        }
        
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int                 spinDataID  = _minStartFreeSpinIDs[Pcg.Default.Next(0, _minFreeSpinTotalCount)];
                BasePPSlotSpinData  spinData    =  await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(spinDataID), TimeSpan.FromSeconds(10.0));

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
        
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                BasePPSlotSpinData spinDataEvent = await selectFreeRangeSpinData(agentID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                if(spinDataEvent != null)
                {
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }
            
            double payoutRate = getPayoutRate(agentID);
            
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
        
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, BasePPSlotBetInfo betInfo,  bool isMoreBet)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(agentID, betInfo, isMoreBet);
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
        
        protected virtual async Task<BasePPSlotSpinData> selectRangeSpinData(int agentID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo)
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
        
        protected virtual async Task<BasePPSlotSpinData> selectFreeRangeSpinData(int agentID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo)
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

            buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsRangeLimited, minFreeOdd, maxFreeOdd);
            return spinData;
        }
        
        protected virtual void buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            startSpinData.FreeSpins = new List<OddAndIDData>();
            int[] freeSpinTypes     = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);
            double maxFreeOdd       = 0.0;
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
        
        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BasePPSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            //배당구간이벤트만을 처리한다.
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus  rangeOddBonus = userBonus as UserRangeOddEventBonus;
                BasePPSlotSpinData      spinDataEvent = await selectRangeSpinData(agentID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                if(spinDataEvent != null)
                {
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }
            
            if (SupportMoreBet && betInfo.MoreBet)
                return await selectRandomStop(agentID, betInfo, true);
            else
                return await selectRandomStop(agentID, betInfo, false);
        }
        
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit, bool isMustLose)
        {
            BasePPSlotSpinData      spinData    = null;
            BasePPSlotSpinResult    result      = null;

            ActionTypes action = ActionTypes.DOSPIN;
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                action = nextResponse.ActionType;

                result = calculateResult(strGlobalUserID, betInfo, nextResponse.Response, false, action);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float totalBet      = betInfo.TotalBet;
            double realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            if (SupportMoreBet && betInfo.MoreBet)
                realBetMoney = totalBet * MoreBetMultiple;

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
                    result = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, action);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                }
                while (false);
            }

            //만일 프리스핀이 선택되였었다면 취소한다.
            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, action);
                emptyWin    = totalBet * spinData.SpinOdd;

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
                spinData = await selectEmptySpin(agentID, betInfo);
                result   = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, action);
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
            if(message.MsgCode == (ushort)CSMSG_CODE.CS_PP_FSOPTION)
            {
                await onFSOption(strUserID, agentID, message, userBonus, userBalance, isMustLose);
            }
            await base.onProcMessage(strUserID, agentID, currency, message, userBonus, userBalance, isMustLose);
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
        
        protected async Task onFSOption(string strUserID, int agentID, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();

                string strGlobalUserID      = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage responseMessage  = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult    result  = _dicUserResultInfos[strGlobalUserID];
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
                            
                            //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                            sumUpCompanyBetWin(agentID, 0.0, selectedWin - maxWin);

                            //이벤트인 경우 남은 머니를 에이전시잔고에 더해준다.
                            if (startSpinData.IsEvent && maxWin > selectedWin)
                                addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);

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

                            //히스토리보관 및 초기화
                            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                                addFSOptionActionHistory(strGlobalUserID, ind, strResponse, index, counter);

                            result.NextAction = nextAction;
                        }
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strGlobalUserID);
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
        
        protected virtual void addFSOptionActionHistory(string strGlobalUserID, int ind, string strResponse, int index, int counter)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID) || _dicUserHistory[strGlobalUserID].log.Count == 0)
                return;

            if (_dicUserHistory[strGlobalUserID].bet == 0.0)
                return;

            BasePPHistoryItem item = new BasePPHistoryItem();
            item.cr = string.Format("symbol={0}&repeat=0&action=doFSOption&index={1}&counter={2}&ind={3}", SymbolName, index, counter, ind);
            item.sr = strResponse;
            _dicUserHistory[strGlobalUserID].log.Add(item);
        }

        protected override async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //자연빵 1만개스핀 선택
                double sumOdd1 = 0.0;
                for (int i = 0; i < 1000000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo, false);
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

                double sumOdd2 = 0.0;
                //MoreBet 1만개
                for (int i = 0; i < 1000000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo, true);
                    if (spinData is BasePPSlotStartSpinData)
                    {
                        sumOdd2 += (spinData as BasePPSlotStartSpinData).StartOdd;
                        int random = Pcg.Default.Next(0, (spinData as BasePPSlotStartSpinData).FreeSpins.Count);
                        sumOdd2 += (spinData as BasePPSlotStartSpinData).FreeSpins[random].Odd;
                    }
                    else
                    {
                        sumOdd2 += spinData.SpinOdd;
                    }
                }
                stopWatch.Stop();
                long elapsed2 = stopWatch.ElapsedMilliseconds;

                stopWatch.Start();

                //Min Start1만개
                double sumOdd3 = 0.0;
                if(SupportPurchaseFree)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        BasePPSlotStartSpinData spinData = (await selectPurchaseFreeSpin(0, betInfo, 0.0, null)) as BasePPSlotStartSpinData;
                        sumOdd3                          += (spinData as BasePPSlotStartSpinData).StartOdd;
                        int random = Pcg.Default.Next(0, (spinData as BasePPSlotStartSpinData).FreeSpins.Count);
                        sumOdd3                          += (spinData as BasePPSlotStartSpinData).FreeSpins[random].Odd;
                    }
                }

                stopWatch.Stop();
                long elapsed3 = stopWatch.ElapsedMilliseconds;

                stopWatch.Start();
                //이벤트각 구간마다 200
                for (int i = 0; i < 6; i++)
                {
                    double[] rangeMins = new double[] { 10, 50, 100, 300, 500, 1000 };
                    double[] rangeMaxs = new double[] { 50, 100, 300, 500, 1000, 3000 };

                    for (int j = 0; j < 200; j++)
                        await selectRangeSpinData(0, rangeMins[i], rangeMaxs[i], betInfo);
                }

                stopWatch.Stop();
                long elapsed4 = stopWatch.ElapsedMilliseconds;

                _logger.Info("{0} Performance Test Results:  \r\nPayrate: {8}%, {1}s, {2}%\t{3}s {4}%\t{5}s {6}%\t{7}s", this.GameName,
                                Math.Round((double)elapsed1 / 1000.0, 3), Math.Round(sumOdd1 / 1000000, 3),
                                Math.Round((double)elapsed2 / 1000.0, 3), Math.Round(sumOdd2 / 1000000, 3),
                                Math.Round((double)elapsed3 / 1000.0, 3), Math.Round(sumOdd3 / 1000000, 3),
                                Math.Round((double)elapsed4 / 1000.0, 3), _config.PayoutRate);

                Sender.Tell(true);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreePPSlotGame::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
        }
    }
}
