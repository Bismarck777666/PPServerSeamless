using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
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
    class BaseAmaticMultiBaseGame : BaseAmaticSlotGame
    {
        protected int []    _normalMaxIDs           = null;
        protected int []    _naturalSpinCounts      = null;
        protected int []    _emptySpinCounts        = null;
        protected int []    _startIDs               = null;
        protected double[]  _totalFreeSpinWinRates  = null;
        protected double[]  _minFreeSpinWinRates    = null;

        protected override async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                var stopWatch = new Stopwatch();

                for( int k = 0; k < LineTypeCnt; k++)
                {
                    stopWatch.Start();
                    //자연빵 1만개스핀 선택
                    double sumOdd1 = 0.0;
                    BaseAmaticSlotBetInfo betInfo = new BaseAmaticSlotBetInfo();
                    betInfo.MoreBet         = -1;
                    betInfo.PurchaseStep    = -1;
                    betInfo.PlayLine = (int)LINES[k];
                    for (int i = 0; i < 100000; i++)
                    {
                        BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                        sumOdd1 += spinData.SpinOdd;
                    }
                    stopWatch.Stop();
                    long elapsed1 = stopWatch.ElapsedMilliseconds;

                    stopWatch.Reset();
                    stopWatch.Start();

                    double sumOdd2 = 0.0;
                    //MoreBet 1만개
                    if (SupportMoreBet)
                    {
                        betInfo.MoreBet = 0;
                        for (int i = 0; i < 100000; i++)
                        {
                            BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                            sumOdd2 += spinData.SpinOdd;
                        }
                    }

                    stopWatch.Stop();
                    long elapsed2 = stopWatch.ElapsedMilliseconds;
                    
                    stopWatch.Reset();
                    stopWatch.Start();

                    double sumOdd3 = 0.0;
                    if (SupportPurchaseFree)
                    {
                        betInfo.PurchaseStep = 0;
                        for (int i = 0; i < 100000; i++)
                        {
                            BasePPSlotSpinData spinData = await selectPurchaseFreeSpin(0, betInfo, 0.0);
                            sumOdd3 += spinData.SpinOdd;
                        }
                    }

                    stopWatch.Stop();
                    long elapsed3 = stopWatch.ElapsedMilliseconds;
                    long elapsed4 = stopWatch.ElapsedMilliseconds;

                    _logger.Info("{0} Performance Test Results:  \r\nLineType: {9}, Payrate: {8}%, {1}s, {2}%\t{3}s {4}%\t{5}s {6}%\t{7}s", this.GameName,
                        Math.Round((double)elapsed1 / 1000.0, 3), Math.Round(sumOdd1 / 1000, 3),
                        Math.Round((double)elapsed2 / 1000.0, 3), Math.Round(sumOdd2 / (1000 * MoreBetMultiple), 3),
                        Math.Round((double)elapsed3 / 1000.0, 3), Math.Round(sumOdd3 / 1000, 3),
                        Math.Round((double)elapsed4 / 1000.0, 3), _config.PayoutRate, k);
                }

                
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticMultiBaseSlotGame::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
        }
        
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet             = (double)infoDocument["defaultbet"];
                _normalMaxIDs                   = new int[LineTypeCnt];
                _naturalSpinCounts              = new int[LineTypeCnt];
                _emptySpinCounts                = new int[LineTypeCnt];
                _startIDs                       = new int[LineTypeCnt];
                _totalFreeSpinWinRates          = new double[LineTypeCnt];
                _minFreeSpinWinRates            = new double[LineTypeCnt];

                var defaultBetsArray            = infoDocument["defaultbet"] as BsonArray;
                var normalMaxIDArray            = infoDocument["normalmaxid"] as BsonArray;
                var emptyCountArray             = infoDocument["emptycount"] as BsonArray;
                var normalSelectCountArray      = infoDocument["normalselectcount"] as BsonArray;
                var startIDArray                = infoDocument["startid"] as BsonArray;

                var purchaseOdds = new BsonArray();
                if (SupportPurchaseFree)
                    purchaseOdds = infoDocument["purchaseodds"] as BsonArray;

                for (int i = 0; i < LineTypeCnt; i++)
                {
                    _normalMaxIDs[i]            = (int)normalMaxIDArray[i];
                    _emptySpinCounts[i]         = (int)emptyCountArray[i];
                    _naturalSpinCounts[i]       = (int)normalSelectCountArray[i];
                    _startIDs[i]                = (int)startIDArray[i];

                    if (SupportPurchaseFree)
                    {
                        _totalFreeSpinWinRates[i]   = (double)purchaseOdds[i * 2 + 1];
                        _minFreeSpinWinRates[i]     = (double)purchaseOdds[i * 2];
                    }

                    if (this.SupportPurchaseFree && this.PurchaseFreeMultiple > _totalFreeSpinWinRates[i])
                        _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }

        protected override OddAndIDData selectRandomOddAndID(int websiteID, BaseAmaticSlotBetInfo betInfo)
        {
            int     betLineType     = getLineTypeFromPlayLine(betInfo.PlayLine);
            double  payoutRate      = getPayoutRate(websiteID);
            double  randomDouble    = Pcg.Default.NextDouble(0.0, 100.0);
            int selectedID = 0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
                selectedID = _startIDs[betLineType] + Pcg.Default.Next(0, _emptySpinCounts[betLineType]);
            else
                selectedID = _startIDs[betLineType] + Pcg.Default.Next(0, _naturalSpinCounts[betLineType]);

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(BaseAmaticSlotBetInfo betInfo)
        {
            int betLineType         = getLineTypeFromPlayLine(betInfo.PlayLine);
            int id                  = _startIDs[betLineType] + Pcg.Default.Next(0, _emptySpinCounts[betLineType]);
            var spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                int betLineType = getLineTypeFromPlayLine(betInfo.PlayLine);
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequestWithBetType(GameName, HasPurEnableOption ? StartSpinSearchTypes.SPECIFIC : StartSpinSearchTypes.GENERAL, betLineType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticMultiBaseGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                int betLineType = getLineTypeFromPlayLine(betInfo.PlayLine);
                BsonDocument spinDataDocument = null;
                if (HasPurEnableOption)
                {
                    spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5, betLineType, -1), TimeSpan.FromSeconds(10.0));
                }
                else
                {
                    spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequestWithBetType(GameName, 1, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5, betLineType, -1), TimeSpan.FromSeconds(10.0));
                }
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticMultiBaseGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int websiteID, BaseAmaticSlotBetInfo betInfo, double baseBet)
        {
            double payoutRate = _config.PayoutRate;
            if (_websitePayoutRates.ContainsKey(websiteID))
                payoutRate = _websitePayoutRates[websiteID];

            int betLineType = getLineTypeFromPlayLine(betInfo.PlayLine);
            double targetC = getPurchaseMultiple(betInfo) * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRates[betLineType])
                targetC = _totalFreeSpinWinRates[betLineType];

            if (targetC < _minFreeSpinWinRates[betLineType])
                targetC = _minFreeSpinWinRates[betLineType];

            double x = (_totalFreeSpinWinRates[betLineType] - targetC) / (_totalFreeSpinWinRates[betLineType] - _minFreeSpinWinRates[betLineType]);

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }
        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
        {
            int betLineType     = getLineTypeFromPlayLine(betInfo.PlayLine);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, minOdd, maxOdd, betLineType, -1), TimeSpan.FromSeconds(10.0));
            
            if (spinDataDocument == null)
                return null;
            
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<BasePPSlotSpinData> selectRangeFreeSpinData(int websiteID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
        {
            int betLineType = getLineTypeFromPlayLine(betInfo.PlayLine);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, 1, minOdd, maxOdd, betLineType, -1), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
    }
}
