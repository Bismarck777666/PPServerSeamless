using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlotGamesNode.Database;
using Akka.Actor;
using PCGSharp;
using GITProtocol;
using System.IO;
using System.Diagnostics;

namespace SlotGamesNode.GameLogics
{
    public class BaseSelFreeGroupedPPSlotGame : BaseSelFreePPSlotGame
    {        
        protected SortedDictionary<int, int>                             _groupNaturalCounts      = new SortedDictionary<int, int>();
        protected SortedDictionary<int, SortedDictionary<double, int>>   _groupNaturalOddProbs    = new SortedDictionary<int, SortedDictionary<double, int>>();
        protected SortedDictionary<int, SortedDictionary<double, int[]>> _groupTotalOddIDs        = new SortedDictionary<int, SortedDictionary<double, int[]>>();
        protected SortedDictionary<int, double>                          _groupTotalMinOdds       = new SortedDictionary<int, double>();
        protected SortedDictionary<int, double>                          _groupTotalMeanOdds      = new SortedDictionary<int, double>();
        protected SortedDictionary<int, int[]>                           _groupEmptySpinIDs       = new SortedDictionary<int, int[]>();
        protected Dictionary<string, int>                                _groupSequences          = new Dictionary<string, int>();
        protected int                                                    _groupSequenceCount      = 0;

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

                ReadFreeOptGroupedSpinInfoResponse response = await _spinDatabase.Ask<ReadFreeOptGroupedSpinInfoResponse>(new ReadFreeOptGroupedSpinInfoRequest(), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _normalMaxID                    = response.NormalMaxID;
                _naturalChildFreeSpinOddProbs   = new SortedDictionary<double, int>[FreeSpinTypeCount];
                _naturalChildFreeSpinCounts     = new int[FreeSpinTypeCount];
                _totalChildFreeSpinCounts       = new int[FreeSpinTypeCount];
                _totalChildFreeSpinIDs          = new SortedDictionary<double, int[]>[FreeSpinTypeCount];
                _spinDataDefaultBet             = response.DefaultBet;

                Dictionary<int,List<int>>                    groupEmptySpinIDs      = new Dictionary<int, List<int>>();
                Dictionary<int,Dictionary<double,List<int>>> groupTotalSpinOddIds   = new Dictionary<int, Dictionary<double, List<int>>>();
                Dictionary<double, List<int>>                freeSpinOddIds         = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>[]              totalChildSpinOddIDs   = new Dictionary<double, List<int>>[FreeSpinTypeCount];
                Dictionary<int, double>                      groupNormalSpinSumOdds = new Dictionary<int, double>();
                Dictionary<int, int>                         groupNormalSpinCounts  = new Dictionary<int, int>();

                Dictionary<int, SortedDictionary<double, List<int>>> groupNormalSpinIds     = new Dictionary<int, SortedDictionary<double, List<int>>>();

                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    GroupedSpinBaseData spinBaseData = response.SpinBaseDatas[i];
                    int groupID = spinBaseData.Group;
                    if (spinBaseData.IsLast)
                        groupID = 100 + spinBaseData.Group;

                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        if (spinBaseData.SpinType <= 100)
                        {
                            if (!_groupNaturalCounts.ContainsKey(groupID))
                                _groupNaturalCounts.Add(groupID, 0);

                            if (!_groupNaturalOddProbs.ContainsKey(groupID))
                                _groupNaturalOddProbs.Add(groupID, new SortedDictionary<double, int>());

                            _groupNaturalCounts[groupID]++;
                            if (_groupNaturalOddProbs[groupID].ContainsKey(spinBaseData.Odd))
                                _groupNaturalOddProbs[groupID][spinBaseData.Odd]++;
                            else
                                _groupNaturalOddProbs[groupID][spinBaseData.Odd] = 1;
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
                    {
                        if (!groupEmptySpinIDs.ContainsKey(groupID))
                            groupEmptySpinIDs.Add(groupID, new List<int>());
                        groupEmptySpinIDs[groupID].Add(spinBaseData.ID);
                    }

                    if(spinBaseData.SpinType == 0)
                    {
                        if (!_groupTotalMinOdds.ContainsKey(groupID))
                            _groupTotalMinOdds[groupID] = spinBaseData.Odd;
                        else if(_groupTotalMinOdds[groupID] > spinBaseData.Odd)
                            _groupTotalMinOdds[groupID] = spinBaseData.Odd;

                        if(!groupNormalSpinCounts.ContainsKey(groupID))
                        {
                            groupNormalSpinCounts[groupID] = 1;
                            groupNormalSpinSumOdds[groupID] = spinBaseData.Odd;
                        }
                        else
                        {
                            groupNormalSpinCounts[groupID]++;
                            groupNormalSpinSumOdds[groupID] += spinBaseData.Odd;
                        }
                        if (!groupNormalSpinIds.ContainsKey(groupID))
                            groupNormalSpinIds.Add(groupID, new SortedDictionary<double, List<int>>());

                        if (!groupNormalSpinIds[groupID].ContainsKey(spinBaseData.Odd))
                            groupNormalSpinIds[groupID][spinBaseData.Odd] = new List<int>();

                        groupNormalSpinIds[groupID][spinBaseData.Odd].Add(spinBaseData.ID);
                    }

                    if (spinBaseData.SpinType <= 100)
                    {
                        if (!groupTotalSpinOddIds.ContainsKey(groupID))
                            groupTotalSpinOddIds.Add(groupID, new Dictionary<double, List<int>>());

                        if(!groupTotalSpinOddIds[groupID].ContainsKey(spinBaseData.Odd))
                            groupTotalSpinOddIds[groupID][spinBaseData.Odd] = new List<int>();
                        groupTotalSpinOddIds[groupID][spinBaseData.Odd].Add(spinBaseData.ID);
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
                
                _groupTotalOddIDs   = new SortedDictionary<int, SortedDictionary<double, int[]>>();
                foreach (KeyValuePair<int, Dictionary<double, List<int>>> pair in groupTotalSpinOddIds)
                {
                    if (!_groupTotalOddIDs.ContainsKey(pair.Key))
                        _groupTotalOddIDs[pair.Key] = new SortedDictionary<double, int[]>();

                    foreach(KeyValuePair<double, List<int>> pair2 in pair.Value)
                        _groupTotalOddIDs[pair.Key][pair2.Key] = pair2.Value.ToArray();
                }
                foreach(KeyValuePair<int, double> pair in groupNormalSpinSumOdds)
                    _groupTotalMeanOdds[pair.Key] = pair.Value / groupNormalSpinCounts[pair.Key];

                foreach(KeyValuePair<int, double> pair in _groupTotalMinOdds)
                {
                    if (pair.Value == 0.0)
                        continue;

                    groupEmptySpinIDs[pair.Key] = new List<int>();
                    foreach(KeyValuePair<double, List<int>> pair2 in groupNormalSpinIds[pair.Key])
                    {
                        if (pair2.Key >= pair.Value && pair2.Key <= _groupTotalMeanOdds[pair.Key])
                            groupEmptySpinIDs[pair.Key].AddRange(pair2.Value);
                        else
                            break;
                    }
                }
                _groupEmptySpinIDs = new SortedDictionary<int, int[]>();
                foreach (KeyValuePair<int, List<int>> pair in groupEmptySpinIDs)
                    _groupEmptySpinIDs.Add(pair.Key, pair.Value.ToArray());

                for (int i = 0; i < FreeSpinTypeCount; i++)
                {
                    if (totalChildSpinOddIDs[i] == null)
                        continue;

                    _totalChildFreeSpinIDs[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in totalChildSpinOddIDs[i])
                        _totalChildFreeSpinIDs[i].Add(pair.Key, pair.Value.ToArray());
                }

                _groupSequences     = new Dictionary<string, int>();
                _groupSequenceCount = 0;
                foreach (GroupSequence sequence in response.Sequences)
                {
                    _groupSequences[sequence.Sequence]   = sequence.Count;
                    _groupSequenceCount                 += sequence.Count;
                }                
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }
        protected string selectStringFromProbs(Dictionary<string, int> stringProbs, int totalCount)
        {
            int random  = Pcg.Default.Next(0, totalCount);
            int sum     = 0;
            foreach (KeyValuePair<string, int> pair in stringProbs)
            {
                sum += pair.Value;
                if (random < sum)
                    return pair.Key;
            }
            return stringProbs.First().Key;
        }
        protected SelectedGroupSequence getCurrentSequenceGroup(BasePPSlotBetInfo baseBetInfo)
        {
            BasePPGroupedSlotBetInfo betInfo = baseBetInfo as BasePPGroupedSlotBetInfo;
            double totalBet = Math.Round(betInfo.TotalBet, 2);

            if (!betInfo.SeqPerBet.ContainsKey(totalBet))
                return null;

            return betInfo.SeqPerBet[totalBet];
        }
        protected GroupSequenceSample pickSequenceSample(int agentID, BasePPSlotBetInfo baseBetInfo)
        {
            BasePPGroupedSlotBetInfo betInfo = baseBetInfo as BasePPGroupedSlotBetInfo;
            double totalBet = Math.Round(betInfo.TotalBet, 2);
            if (betInfo.SeqPerBet.ContainsKey(totalBet) && !betInfo.SeqPerBet[totalBet].IsEnded)
                return betInfo.SeqPerBet[totalBet].nextSample();

            if(betInfo.SeqPerBet.ContainsKey(totalBet) && betInfo.SeqPerBet[totalBet].IsEnded && betInfo.SeqPerBet[totalBet].CreditMoney > 0.0)
                sumUpCompanyBetWin(agentID, 0, -betInfo.SeqPerBet[totalBet].CreditMoney);

            double creditedMoney = 0.0;
            List<int> groupIDs = new List<int>();
            do
            {
                string   strSequence = selectStringFromProbs(_groupSequences, _groupSequenceCount);
                string[] strParts    = strSequence.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                groupIDs.Clear();
                for (int i = 0; i < strParts.Length; i++)
                {
                    if(i == strParts.Length - 1)
                        groupIDs.Add(int.Parse(strParts[i]) + 100);
                    else
                        groupIDs.Add(int.Parse(strParts[i]));
                }
                double minOdd = 0.0;
                for(int i = 0; i < groupIDs.Count; i++)
                {
                    if (_groupTotalMinOdds[groupIDs[i]] == 0.0)
                        continue;
                    minOdd += _groupTotalMeanOdds[groupIDs[i]];
                }
                if (minOdd == 0.0)
                    break;

                if (checkCompanyPayoutRate(agentID, 0.0, totalBet * minOdd))
                {
                    creditedMoney = totalBet * minOdd;
                    break;
                }
            } while (true);

            List<int> finalGroupIDs = new List<int>();
            for (int i = 0; i < groupIDs.Count - 1; i++)
                finalGroupIDs.Add(groupIDs[i]);

            int lastGroupID      = groupIDs[groupIDs.Count - 1];
            var randomizedGroups = finalGroupIDs.OrderBy(item => Pcg.Default.Next());

            SelectedGroupSequence seqInfo = new SelectedGroupSequence();
            int totalGroupSum             = 0;
            int index                     = 1;
            foreach (int groupID in randomizedGroups)
            {
                totalGroupSum += groupID;
                seqInfo.Samples.Add(new GroupSequenceSample(index, totalGroupSum, groupID));
                index++;
            }
            seqInfo.Samples.Add(new GroupSequenceSample(index, totalGroupSum, lastGroupID));
            seqInfo.CreditMoney         = creditedMoney;
            betInfo.SeqPerBet[totalBet] = seqInfo;
            return seqInfo.nextSample();
        }
        protected override OddAndIDData selectRandomOddAndID(int agentID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(agentID))
                payoutRate = _agentPayoutRates[agentID];

            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
            double selectedOdd  = 0.0;
            GroupSequenceSample grpSeqSample = pickSequenceSample(agentID, betInfo);
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedOdd = 0.0;
            }            
            else
            {
                selectedOdd = selectOddFromProbs(_groupNaturalOddProbs[grpSeqSample.Group], _groupNaturalCounts[grpSeqSample.Group]);
            }

            if (!_groupTotalOddIDs[grpSeqSample.Group].ContainsKey(selectedOdd))
            {
                (betInfo as BasePPGroupedSlotBetInfo).undoSequence();
                return null;
            }
            int selectedID                      = _groupTotalOddIDs[grpSeqSample.Group][selectedOdd][Pcg.Default.Next(0, _groupTotalOddIDs[grpSeqSample.Group][selectedOdd].Length)];
            GroupOddAndIDData selectedOddAndID  = new GroupOddAndIDData(selectedID, selectedOdd, grpSeqSample.Index, grpSeqSample.Group, grpSeqSample.CumulatedGroupSum);
            return selectedOddAndID;
        }      
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            GroupOddAndIDData   selectedOddAndID = selectRandomOddAndID(agentID, betInfo, isMoreBet) as GroupOddAndIDData;
            if (selectedOddAndID == null)
                return null;

            BasePPSlotSpinData  slotSpinData    = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));

            processSlotSpinData(slotSpinData, selectedOddAndID.Index, selectedOddAndID.Group, selectedOddAndID.CumGroupSum);

            if (!(slotSpinData is BasePPSlotStartSpinData))
                return slotSpinData;

            BasePPSlotStartSpinData startSpinData = slotSpinData as BasePPSlotStartSpinData;
            buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsNaturalRandom, 0.0, 0.0);         
            return slotSpinData;
        }
        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int agentID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo)
        {
            GroupSequenceSample grpSample = pickSequenceSample(agentID, betInfo);
            int     groupID = grpSample.Group;
            bool    isLast  = false;
            if (groupID >= 100)
            {
                groupID -= 100;
                isLast   = true;
            }

            BasePPSlotSpinData spinData = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectGroupRangeSpinDataRequest(minOdd, maxOdd, getRangeID(minOdd, maxOdd), groupID, isLast), TimeSpan.FromSeconds(10.0));
            if (spinData == null)
            {
                (betInfo as BasePPGroupedSlotBetInfo).undoSequence();
                return null;
            }

            if (spinData is BasePPSlotStartSpinData)
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
            processSlotSpinData(spinData, grpSample.Index, grpSample.Group, grpSample.CumulatedGroupSum);
            return spinData;
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BasePPSlotBetInfo betInfo)
        {
            //배당구간이벤트만을 처리한다.
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                BasePPSlotSpinData     spinDataEvent = await selectRangeSpinData(0, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                if (spinDataEvent != null)
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
            BasePPSlotSpinData spinData = null;
            BasePPSlotSpinResult result = null;

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

            spinData            = await selectRandomStop(agentID, userBonus, totalBet, false, isMustLose, betInfo);
            if(spinData != null)
            {
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
                (betInfo as BasePPGroupedSlotBetInfo).undoSequence();
            }

            //만일 프리스핀이 선택되였었다면 취소한다.
            spinData    = await selectEmptySpin(agentID, betInfo);
            result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, action);
            if (spinData is BasePPSlotStartSpinData)
                betInfo.SpinData = spinData;
            else
                betInfo.SpinData = null;
            sumUpCompanyBetWin(agentID, realBetMoney, 0.0);
            return result;
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(int agentID, BasePPSlotBetInfo betInfo)
        {
            GroupSequenceSample grpSample = pickSequenceSample(agentID, betInfo);
            int                 id        = _groupEmptySpinIDs[grpSample.Group][Pcg.Default.Next(0, _groupEmptySpinIDs[grpSample.Group].Length)];
            BasePPSlotSpinData  spinData  = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(id), TimeSpan.FromSeconds(10.0));
            if(spinData.SpinOdd > 0.0)
            {
                SelectedGroupSequence sequenceGroup  = getCurrentSequenceGroup(betInfo);
                sequenceGroup.CreditMoney           -= (spinData.SpinOdd * betInfo.TotalBet);
            }
            processSlotSpinData(spinData, grpSample.Index, grpSample.Group, grpSample.CumulatedGroupSum);
            return spinData;
        }
        protected virtual void processSlotSpinData(BasePPSlotSpinData spinData, int index, int group, int cumBonusSum)
        {

        }
        protected override void preprocessSelectedFreeSpin(BasePPSlotSpinData freeSpinData, BasePPSlotBetInfo baseBetInfo)
        {
            try
            {
                BasePPGroupedSlotBetInfo betInfo = baseBetInfo as BasePPGroupedSlotBetInfo;
                double totalBet = Math.Round(betInfo.TotalBet, 2);

                GroupSequenceSample groupSample = betInfo.SeqPerBet[totalBet].Samples[betInfo.SeqPerBet[totalBet].LastID - 1];
                processSlotSpinData(freeSpinData, groupSample.Index, groupSample.Group, groupSample.CumulatedGroupSum);
            }
            catch(Exception ex)
            {
                _logger.Error("Excetpion has been occurred in BaseSelFreeGroupedPPSlotGame::preprocessSelectedFreeSpin {0}", ex);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {            
            try
            {
                BasePPGroupedSlotBetInfo betInfo = new BasePPGroupedSlotBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BaseSelFreeGroupedPPSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreeGroupedPPSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BasePPGroupedSlotBetInfo betInfo = new BasePPGroupedSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                BasePPGroupedSlotBetInfo betInfo = new BasePPGroupedSlotBetInfo();
                betInfo.BetPerLine = 10;
                betInfo.LineCount  = 25;
                _dicUserBetInfos["test1"] = betInfo;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //자연빵 1만개스핀 선택
                double sumOdd1 = 0.0;
                for (int i = 0; i < 1000000; i++)
                {
                    BasePPSlotSpinResult result = await generateSpinResult(betInfo, "test1", 0, null, true, false);
                    if (betInfo.SpinData is BasePPSlotStartSpinData)
                    {
                        int random = Pcg.Default.Next(0, (betInfo.SpinData as BasePPSlotStartSpinData).FreeSpins.Count);

                        double startOdd     = (betInfo.SpinData as BasePPSlotStartSpinData).StartOdd;
                        double maxOdd       = (betInfo.SpinData as BasePPSlotStartSpinData).MaxOdd;
                        double selectedOdd  = (betInfo.SpinData as BasePPSlotStartSpinData).FreeSpins[random].Odd;

                        sumOdd1 += (startOdd + selectedOdd);
                        sumUpCompanyBetWin(0, 0.0, (selectedOdd + startOdd - maxOdd) * betInfo.TotalBet);
                    }
                    else
                    {
                        sumOdd1 += (result.TotalWin / betInfo.TotalBet);
                    }
                }
                stopWatch.Stop();
                long elapsed1 = stopWatch.ElapsedMilliseconds;

                stopWatch.Start();

                double sumOdd2 = 0.0;                
                stopWatch.Stop();
                long elapsed2 = stopWatch.ElapsedMilliseconds;

                stopWatch.Start();

                //Min Start1만개
                double sumOdd3 = 0.0;
                if (SupportPurchaseFree)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        BasePPSlotStartSpinData spinData = (await selectPurchaseFreeSpin(0, betInfo, 0.0, null)) as BasePPSlotStartSpinData;
                        sumOdd3 += (spinData as BasePPSlotStartSpinData).StartOdd;
                        int random = Pcg.Default.Next(0, (spinData as BasePPSlotStartSpinData).FreeSpins.Count);
                        sumOdd3 += (spinData as BasePPSlotStartSpinData).FreeSpins[random].Odd;
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
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreePPSlotGame::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
        }
    }
    public class GroupOddAndIDData : OddAndIDData
    {
        public int Group        { get; set; }
        public int CumGroupSum  { get; set; }
        public int Index        { get; set; }
        public bool IsEmpty     { get; set; }
        public GroupOddAndIDData(int id, double odd, int index, int group, int cumGroupSum) : base(id, odd)
        {
            Index       = index;
            Group       = group;
            CumGroupSum = cumGroupSum;
        }
    }
}
