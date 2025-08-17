﻿using System;
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
using GITProtocol.Utils;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public class BaseSelFreeGroupedPPSlotGame : BaseSelFreePPSlotGame
    {        
        protected SortedDictionary<int, int>                             _groupNaturalCounts      = new SortedDictionary<int, int>();
        protected SortedDictionary<int, double>                          _groupTotalMinOdds       = new SortedDictionary<int, double>();
        protected SortedDictionary<int, double>                          _groupTotalMeanOdds      = new SortedDictionary<int, double>();
        protected SortedDictionary<int, int>                             _groupEmptyCounts        = new SortedDictionary<int, int>();
        protected SortedDictionary<int, int>                             _groupStartIds           = new SortedDictionary<int, int>();
        protected Dictionary<string, int>                                _groupSequences          = new Dictionary<string, int>();
        protected int                                                    _groupSequenceCount      = 0;

        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet     = (double)  infoDocument["defaultbet"];
                _normalMaxID            = (int)     infoDocument["normalmaxid"];

                BsonArray groupIds      = infoDocument["groupids"] as BsonArray;
                List<int> groupIdList   = new List<int>();
                for(int i = 0; i < groupIds.Count; i++)
                    groupIdList.Add((int)groupIds[i]);

                BsonArray groupStartIds     = infoDocument["groupstartids"] as BsonArray;
                BsonArray groupEmptyCounts  = infoDocument["groupemptycounts"] as BsonArray;
                BsonArray groupSpinCouns    = infoDocument["groupspincounts"] as BsonArray;
                BsonArray groupMinOdds      = infoDocument["groupminodds"] as BsonArray;
                BsonArray groupMeanOdds     = infoDocument["groupmeanodds"] as BsonArray;

                for(int i = 0; i < groupIdList.Count; i++)
                {
                    _groupStartIds.Add(groupIdList[i],      (int) groupStartIds[i]);
                    _groupEmptyCounts.Add(groupIdList[i],   (int) groupEmptyCounts[i]);
                    _groupNaturalCounts.Add(groupIdList[i], (int) groupSpinCouns[i]);
                    _groupTotalMinOdds.Add(groupIdList[i],  (double) groupMinOdds[i]);
                    _groupTotalMeanOdds.Add(groupIdList[i], (double) groupMeanOdds[i]);
                }

                List<BsonDocument> seqDocuments = await Context.System.ActorSelection("/user/spinDBReaders").Ask<List<BsonDocument>>(new SelectAllSequenceRequest(this.GameName), TimeSpan.FromSeconds(10.0));

                _groupSequenceCount = 0;
                for (int i = 0; i < seqDocuments.Count; i++)
                {
                    string  strSequence          = (string) seqDocuments[i]["sequence"];
                    int     count                = (int)seqDocuments[i]["count"];
                    _groupSequences[strSequence] = count;
                    _groupSequenceCount         += count;

                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }


        protected string selectStringFromProbs(Dictionary<string, int> stringProbs, int totalCount)
        {
            int random = Pcg.Default.Next(0, totalCount);
            int sum = 0;
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
        protected async Task<GroupSequenceSample> pickSequenceSample(int websiteID, BasePPSlotBetInfo baseBetInfo)
        {
            BasePPGroupedSlotBetInfo betInfo = baseBetInfo as BasePPGroupedSlotBetInfo;
            double totalBet = Math.Round(betInfo.TotalBet, 2);
            if (betInfo.SeqPerBet.ContainsKey(totalBet) && !betInfo.SeqPerBet[totalBet].IsEnded)
                return betInfo.SeqPerBet[totalBet].nextSample();

            if(betInfo.SeqPerBet.ContainsKey(totalBet) && betInfo.SeqPerBet[totalBet].IsEnded && betInfo.SeqPerBet[totalBet].CreditMoney > 0.0)
                sumUpWebsiteBetWin(websiteID, 0, -betInfo.SeqPerBet[totalBet].CreditMoney);

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

                if (await checkWebsitePayoutRate(websiteID, 0.0, totalBet * minOdd))
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
            seqInfo.CreditMoney = creditedMoney;
            betInfo.SeqPerBet[totalBet] = seqInfo;
            return seqInfo.nextSample();
        }

        protected override async Task<OddAndIDData> selectRandomOddAndID(int websiteID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            double payoutRate   = getPayoutRate(websiteID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
            GroupSequenceSample grpSeqSample = await pickSequenceSample(websiteID, betInfo);
            int selectedID      = 0;
            int groupID         = grpSeqSample.Group;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                if(_groupEmptyCounts[groupID] > 0)
                    selectedID = _groupStartIds[groupID] + Pcg.Default.Next(0, _groupEmptyCounts[groupID]);
            }            
            else
            {
                if (_groupNaturalCounts.ContainsKey(groupID) && _groupNaturalCounts[groupID] > 0)
                    selectedID = _groupStartIds[groupID] + Pcg.Default.Next(0, _groupNaturalCounts[groupID]);
            }
            if (selectedID == 0)
            {
                (betInfo as BasePPGroupedSlotBetInfo).undoSequence();
                return null;
            }
            GroupOddAndIDData selectedOddAndID  = new GroupOddAndIDData(selectedID, 0.0, grpSeqSample.Index, grpSeqSample.Group, grpSeqSample.CumulatedGroupSum);
            return selectedOddAndID;
        }      
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            GroupOddAndIDData   selectedOddAndID = await selectRandomOddAndID(websiteID, betInfo, isMoreBet) as GroupOddAndIDData;
            if (selectedOddAndID == null)
                return null;

            BsonDocument        document        = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(this.GameName, selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
            BasePPSlotSpinData  slotSpinData    = convertBsonToSpinData(document);
            processSlotSpinData(slotSpinData, selectedOddAndID.Index, selectedOddAndID.Group, selectedOddAndID.CumGroupSum);

            if (!(slotSpinData is BasePPSlotStartSpinData))
                return slotSpinData;

            BasePPSlotStartSpinData startSpinData = slotSpinData as BasePPSlotStartSpinData;
            await buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsNaturalRandom, 0.0, 0.0);         
            return slotSpinData;
        }

        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo)
        {
            GroupSequenceSample grpSample   = await pickSequenceSample(websiteID, betInfo);
            int                 groupID     = grpSample.Group;
            BsonDocument document = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSelFreeGroupSpinRangeRequest(this.GameName,
                minOdd, maxOdd, getRangeID(minOdd, maxOdd), groupID), TimeSpan.FromSeconds(10.0));

            if (document == null)
            {
                (betInfo as BasePPGroupedSlotBetInfo).undoSequence();
                return null;
            }
            BasePPSlotSpinData spinData = convertBsonToSpinData(document);

            if (spinData is BasePPSlotStartSpinData)
            {
                BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
                double minFreeOdd = minOdd - startSpinData.StartOdd;
                if (minFreeOdd < 0.0)
                    minFreeOdd = 0.0;
                double maxFreeOdd = maxOdd - startSpinData.StartOdd;
                if (maxFreeOdd < 0.0)
                    maxFreeOdd = 0.0;
                await buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsRangeLimited, minFreeOdd, maxFreeOdd);
            }
            processSlotSpinData(spinData, grpSample.Index, grpSample.Group, grpSample.CumulatedGroupSum);
            return spinData;
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo)
        {
            //배당구간이벤트만을 처리한다.
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                if (baseBet.LE(rangeOddBonus.MaxBet, _epsilion))
                {
                BasePPSlotSpinData     spinDataEvent = await selectRangeSpinData(0, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                if (spinDataEvent != null)
                {
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }
            }
            
            if (SupportMoreBet && betInfo.MoreBet)
                return await selectRandomStop(websiteID, betInfo, true);
            else
                return await selectRandomStop(websiteID, betInfo, false);
        }
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int websiteID, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData spinData = null;
            BasePPSlotSpinResult result = null;

            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float totalBet      = betInfo.TotalBet;
            double realBetMoney = totalBet;

            spinData            = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo);
            if(spinData != null)
            {
                double totalWin = 0.0;
                if (spinData is BasePPSlotStartSpinData)
                    totalWin = totalBet * (spinData as BasePPSlotStartSpinData).MaxOdd;
                else
                    totalWin = totalBet * spinData.SpinOdd;

                if (!usePayLimit || spinData.IsEvent || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
                {                    
                    do
                    {
                        if (spinData.IsEvent)
                        {
                            bool checkRet = await subtractEventMoney(websiteID, strUserID, totalWin);
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
                    } while (false);
                }
                (betInfo as BasePPGroupedSlotBetInfo).undoSequence();
            }

            //만일 프리스핀이 선택되였었다면 취소한다.
            spinData        = await selectEmptySpin(websiteID, betInfo);
            result          = calculateResult(betInfo, spinData.SpinStrings[0], true);
            if (spinData is BasePPSlotStartSpinData)
                betInfo.SpinData = spinData;
            else
                betInfo.SpinData = null;
            sumUpWebsiteBetWin(websiteID, realBetMoney, 0.0);
            return result;
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(int websiteID, BasePPSlotBetInfo betInfo)
        {
            GroupSequenceSample grpSample = await pickSequenceSample(websiteID, betInfo);
            int groupID                   = grpSample.Group;
            int                 id        = _groupStartIds[groupID] + Pcg.Default.Next(0, _groupEmptyCounts[groupID]);
            BsonDocument        document  = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(this.GameName, id), TimeSpan.FromSeconds(10.0));
            BasePPSlotSpinData  spinData  = convertBsonToSpinData(document);
            if (spinData.SpinOdd > 0.0)
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
                betInfo.BetPerLine               = (float)message.Pop();
                betInfo.LineCount                = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BaseSelFreeGroupedPPSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
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
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BasePPGroupedSlotBetInfo betInfo = new BasePPGroupedSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
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
            this.Index          = index;
            this.Group          = group;
            this.CumGroupSum    = cumGroupSum;
        }
    }

}
