using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
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
        protected override bool HasSelectableFreeSpin   => true;
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
            return 1;
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSelFreeMinStartRequest(GameName, HasPurEnableOption ? 0 : -1), TimeSpan.FromSeconds(10.0));

                BasePPSlotStartSpinData spinData = convertBsonToSpinData(spinDataDocument) as BasePPSlotStartSpinData;
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
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
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
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, BaseAmaticSlotBetInfo betInfo)
        {
            BasePPSlotSpinData spinData = await base.selectRandomStop(websiteID, betInfo);
            if (!(spinData is BasePPSlotStartSpinData))
                return spinData;

            BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
            await buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsNaturalRandom, 0.0, 0.0);
            return startSpinData;
        }
        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
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
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, double baseBet, BaseAmaticSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (this.SupportPurchaseFree && betInfo.isPurchase)
                return await selectPurchaseFreeSpin(websiteID, betInfo, baseBet);

            return await selectRandomStop(websiteID, betInfo);
        }
        protected override async Task<BaseAmaticSlotSpinResult> generateSpinResult(BaseAmaticSlotBetInfo betInfo, string strUserID, int websiteID, double userBalance, double betMoney, bool usePayLimit)
        {
            BasePPSlotSpinData          spinData    = null;
            BaseAmaticSlotSpinResult    result      = null;

            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BaseAmaticActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, strUserID, nextResponse.Response, false, userBalance, betMoney);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            double pointUnit    = getPointUnit(betInfo);
            double totalBet     = betInfo.RelativeTotalBet * BettingButton[betInfo.PlayBet] * pointUnit;
            double realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.isPurchase)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            if (SupportMoreBet && betInfo.isMoreBet)
                realBetMoney = totalBet * MoreBetMultiple;

            spinData = await selectRandomStop(websiteID, totalBet, betInfo);
            double totalWin = 0.0;
            if (spinData is BasePPSlotStartSpinData)
                totalWin = totalBet * (spinData as BasePPSlotStartSpinData).MaxOdd;
            else
                totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {                
                do
                {
                    if (spinData is BasePPSlotStartSpinData)
                        betInfo.SpinData = spinData;
                    else
                        betInfo.SpinData = null;
                    result = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                } while (false);
            }

            //만일 프리스핀이 선택되였었다면 취소한다.
            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.isPurchase)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result   = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
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
                spinData = await selectEmptySpin(betInfo);
                result   = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                if (spinData is BasePPSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;
            }
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;
        }
        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message, double userBalance,Currencies currency)
        {
            if(message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_FSOPTION)
            {
                onFreeSpinOptionSelectRequest(strUserID, websiteID, message, userBalance);
            }
            else
            {
                await base.onProcMessage(strUserID, websiteID, message, userBalance, currency);
            }
        }
        protected virtual void onFreeSpinOptionSelectRequest(string strUserID, int websiteID, GITMessage message, double userBalance)
        {
            try
            {
                int selectOption            = (int)message.Pop();
                GITMessage responseMessage  = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_FSOPTION);

                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
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
                        BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[selectOption]);
                        betInfo.SpinData = freeSpinData;

                        List<string> freeSpinStrings = new List<string>();
                        for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                            freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData));

                        string strSpinResponse = freeSpinStrings[0];
                        if (freeSpinStrings.Count > 1)
                            betInfo.RemainReponses = buildResponseList(freeSpinStrings);

                        double selectedWin  = (startSpinData.StartOdd + freeSpinData.SpinOdd) * (betInfo.RelativeTotalBet * BettingButton[betInfo.PlayBet]);
                        double maxWin       = startSpinData.MaxOdd * (betInfo.RelativeTotalBet * BettingButton[betInfo.PlayBet]);
                        //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                        sumUpWebsiteBetWin(websiteID, 0.0, selectedWin - maxWin);

                        AmaticPacket packet = new AmaticPacket(strSpinResponse, Cols, FreeCols);
                        convertWinsByBet(userBalance, packet, betInfo);
                        BaseAmaticSlotSpinResult spinResult = new BaseAmaticSlotSpinResult();
                        spinResult.ResultString = buildSpinString(packet);
                        responseMessage.Append(spinResult.ResultString);

                        _dicUserResultInfos[strGlobalUserID]            = spinResult;
                        sendFreeOptionPickResult(spinResult, strGlobalUserID, 0.0, 0.0, "FreeOption", userBalance);
                        _dicUserLastBackupResultInfos[strGlobalUserID]  = spinResult;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
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
        protected virtual string addStartWinToResponse(string strResponse, BasePPSlotStartSpinData startSpinData)
        {
            AmaticPacket firstPacket    = new AmaticPacket(startSpinData.SpinStrings[0], Cols, FreeCols);
            AmaticPacket packet         = new AmaticPacket(strResponse, Cols, FreeCols);
            packet.win = firstPacket.win + packet.totalfreewin;

            return buildSpinString(packet);
        }
        
        protected override async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                BaseAmaticSlotBetInfo betInfo = new BaseAmaticSlotBetInfo();
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //자연빵 1만개스핀 선택
                double sumOdd1 = 0.0;
                for (int i = 0; i < 100000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                    if (spinData is BasePPSlotStartSpinData)
                    {
                        sumOdd1 += (spinData as BasePPSlotStartSpinData).StartOdd;
                        int random = Pcg.Default.Next(0, (spinData as BasePPSlotStartSpinData).FreeSpins.Count);
                        BasePPSlotSpinData freeSpinData = convertBsonToSpinData((spinData as BasePPSlotStartSpinData).FreeSpins[random]);
                        sumOdd1 += freeSpinData.SpinOdd;
                    }
                    else
                    {
                        sumOdd1 += spinData.SpinOdd;
                    }
                }
                stopWatch.Stop();
                long elapsed1 = stopWatch.ElapsedMilliseconds;

                stopWatch.Start();

                //MoreBet 1만개
                double sumOdd2 = 0.0;
                if (SupportMoreBet)
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                        if (spinData is BasePPSlotStartSpinData)
                        {
                            sumOdd2 += (spinData as BasePPSlotStartSpinData).StartOdd;
                            int random = Pcg.Default.Next(0, (spinData as BasePPSlotStartSpinData).FreeSpins.Count);
                            BasePPSlotSpinData freeSpinData = convertBsonToSpinData((spinData as BasePPSlotStartSpinData).FreeSpins[random]);
                            sumOdd2 += freeSpinData.SpinOdd;
                        }
                        else
                        {
                            sumOdd2 += spinData.SpinOdd;
                        }
                    }
                }

                stopWatch.Stop();
                long elapsed2 = stopWatch.ElapsedMilliseconds;

                stopWatch.Start();

                //Min Start1만개
                double sumOdd3 = 0.0;
                if (SupportPurchaseFree)
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        BasePPSlotStartSpinData spinData = (await selectPurchaseFreeSpin(0, betInfo, 0.0)) as BasePPSlotStartSpinData;
                        sumOdd3 += (spinData as BasePPSlotStartSpinData).StartOdd;
                        int random = Pcg.Default.Next(0, (spinData as BasePPSlotStartSpinData).FreeSpins.Count);
                        BasePPSlotSpinData freeSpinData = convertBsonToSpinData((spinData as BasePPSlotStartSpinData).FreeSpins[random]);
                        sumOdd3 += freeSpinData.SpinOdd;
                    }
                }

                stopWatch.Stop();
                long elapsed3 = stopWatch.ElapsedMilliseconds;
                long elapsed4 = stopWatch.ElapsedMilliseconds;

                _logger.Info("{0} Performance Test Results:  \r\nPayrate: {8}%, {1}s, {2}%\t{3}s {4}%\t{5}s {6}%\t{7}s", this.GameName,
                                Math.Round((double)elapsed1 / 1000.0, 3), Math.Round(sumOdd1 / 1000, 3),
                                Math.Round((double)elapsed2 / 1000.0, 3), Math.Round(sumOdd2 / 1000, 3),
                                Math.Round((double)elapsed3 / 1000.0, 3), Math.Round(sumOdd3 / 1000, 3),
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
}
