﻿using System;
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
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public class BasePPSlotStartSpinData : BasePPSlotSpinData
    {
        public double                           StartOdd        { get; set; }        
        public int                              FreeSpinGroup   { get; set; }
        public List<int>                        PossibleRanges  { get; set; }
        public List<BsonDocument>               FreeSpins       { get; set; }
        public double                           MaxOdd          { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.StartOdd       = reader.ReadDouble();
            this.FreeSpinGroup  = reader.ReadInt32();
            this.PossibleRanges = SerializeUtils.readIntList(reader);
            this.MaxOdd         = reader.ReadDouble();

            this.FreeSpins      = new List<BsonDocument>();
            int count = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                string strJson = reader.ReadString();
                this.FreeSpins.Add(BsonDocument.Parse(strJson));
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
                    writer.Write(this.FreeSpins[i].ToJson());
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
        protected override bool HasSelectableFreeSpin
        {
            get { return true;  }
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
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSelFreeMinStartRequest(GameName, HasPurEnableOption ? 0 : -1), TimeSpan.FromSeconds(10.0));
                
                BasePPSlotStartSpinData spinData =  convertBsonToSpinData(spinDataDocument) as BasePPSlotStartSpinData;
                double minOdd = PurchaseFreeMultiple * 0.2 - spinData.SpinOdd;
                if (minOdd < 0.0)
                    minOdd = 0.0;
                double maxOdd = PurchaseFreeMultiple * 0.5 - spinData.SpinOdd;
                await buildStartFreeSpinData(spinData, StartSpinBuildTypes.IsRangeLimited, minOdd, maxOdd);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, this.HasPurEnableOption ? StartSpinSearchTypes.SPECIFIC : StartSpinSearchTypes.SELFREE), TimeSpan.FromSeconds(10.0));

                BasePPSlotStartSpinData startSpinData = convertBsonToSpinData(spinDataDocument) as BasePPSlotStartSpinData;
                await buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, 0.0, 0.0);
                return startSpinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinData convertBsonToSpinData(BsonDocument document)
        {

            int     spinType    = (int)document["spintype"];
            double  spinOdd     = (double)document["odd"];
            string  strData     = (string)document["data"];

            List<string> spinResponses = new List<string>(strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));

            if (spinType == 100)
            {
                BasePPSlotStartSpinData startSpinData = new BasePPSlotStartSpinData();
                startSpinData.StartOdd      = spinOdd;
                startSpinData.FreeSpinGroup = (int)document["freespintype"];
                startSpinData.SpinStrings   = spinResponses;
                return startSpinData;
            }
            else
            {
                return new BasePPSlotSpinData(spinType, spinOdd, spinResponses);
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, BasePPSlotBetInfo betInfo,  bool isMoreBet)
        {
            BasePPSlotSpinData spinData = await base.selectRandomStop(websiteID, betInfo, isMoreBet);
            if (!(spinData is BasePPSlotStartSpinData))
                return spinData;

            BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
            await buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsNaturalRandom, 0.0, 0.0);
            return startSpinData;
        }

        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo)
        {            
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSelFreeSpinRangeRequest(GameName, minOdd, maxOdd, getRangeID(minOdd, maxOdd)), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            BasePPSlotSpinData spinData = convertBsonToSpinData(spinDataDocument);
            if(spinData is BasePPSlotStartSpinData)
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
            return spinData;
        }
        protected virtual async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            startSpinData.FreeSpins = new List<BsonDocument>();
            int[] freeSpinTypes     = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);
            double maxFreeOdd = 0.0;
            for (int i = 0; i < freeSpinTypes.Length; i++)
            {
                BsonDocument childFreeSpin = null;
                if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                {
                    childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeIDRangeRequest(GameName, freeSpinTypes[i], _normalMaxID), TimeSpan.FromSeconds(10.0));
                }
                else if (buildType == StartSpinBuildTypes.IsTotalRandom)
                {
                    childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeIDRangeRequest(GameName, freeSpinTypes[i], -1), TimeSpan.FromSeconds(10.0));
                }
                else
                {
                    childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequest(GameName, freeSpinTypes[i], minOdd, maxOdd), TimeSpan.FromSeconds(10.0));
                }
                double childOdd = (double) childFreeSpin["odd"];
                if (childOdd > maxFreeOdd)
                    maxFreeOdd = childOdd;
                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd = startSpinData.StartOdd + maxFreeOdd;
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(websiteID, betInfo, baseBet, userBonus);

            //배당구간이벤트만을 처리한다.
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus  rangeOddBonus = userBonus as UserRangeOddEventBonus;
                if (baseBet.LE(rangeOddBonus.MaxBet, _epsilion))
                {
                    BasePPSlotSpinData      spinDataEvent = await selectRangeSpinData(websiteID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                    if(spinDataEvent != null)
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
            BasePPSlotSpinData      spinData    = null;
            BasePPSlotSpinResult    result      = null;

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

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            if (SupportMoreBet && betInfo.MoreBet)
                realBetMoney = totalBet * MoreBetMultiple;

            spinData = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo);
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

            //만일 프리스핀이 선택되였었다면 취소한다.
            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result   = calculateResult(betInfo, spinData.SpinStrings[0], true);
                emptyWin = totalBet * (spinData as BasePPSlotStartSpinData).MaxOdd;

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
                spinData = await selectEmptySpin(websiteID, betInfo);
                result   = calculateResult(betInfo, spinData.SpinStrings[0], true);
                if (spinData is BasePPSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;
            }
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;
        }

        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency)
        {
            if(message.MsgCode == (ushort)CSMSG_CODE.CS_PP_FSOPTION)
                await onFSOption(strUserID, websiteID, message, userBonus, userBalance);

            await base.onProcMessage(strUserID, websiteID, message, userBonus, userBalance, currency);
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
        protected async Task onFSOption(string strUserID, int agentID, GITMessage message, UserBonus userBonus, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
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
                            BasePPSlotSpinData freeSpinData  = convertBsonToSpinData(startSpinData.FreeSpins[ind]);
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
                            if(!startSpinData.IsEvent)
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                            else if (maxWin > selectedWin)
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
        protected virtual void addDoBonusActionHistory(string strGlobalUserID, int ind, string strResponse, int index, int counter)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID) || _dicUserHistory[strGlobalUserID].log.Count == 0)
                return;

            if (_dicUserHistory[strGlobalUserID].bet == 0.0)
                return;

            BasePPHistoryItem item = new BasePPHistoryItem();
            item.cr = string.Format("symbol={0}&repeat=0&action=doBonus&index={1}&counter={2}&ind={3}", SymbolName, index, counter, ind);
            item.sr = strResponse;
            _dicUserHistory[strGlobalUserID].log.Add(item);
        }
    }
}
