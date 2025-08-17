using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
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
        protected virtual int getLineTypeFromPlayLine(BaseAmaticSlotBetInfo betInfo)
        {
            return 0;
        }
        protected override OddAndIDData selectRandomOddAndID(int agentID, BaseAmaticSlotBetInfo betInfo)
        {
            int     betLineType     = getLineTypeFromPlayLine(betInfo);
            double  payoutRate      = getPayoutRate(agentID);
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
            int betLineType         = getLineTypeFromPlayLine(betInfo);
            int id                  = _startIDs[betLineType] + Pcg.Default.Next(0, _emptySpinCounts[betLineType]);
            var spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                int betLineType = getLineTypeFromPlayLine(betInfo);
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
                int betLineType = getLineTypeFromPlayLine(betInfo);
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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int websiteID, BaseAmaticSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                if (baseBet.LE(rangeOddBonus.MaxBet, _epsilion))
                {
                    BasePPSlotSpinData spinDataEvent = await selectRangeFreeSpinData(websiteID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                    if (spinDataEvent != null)
                    {
                        spinDataEvent.IsEvent = true;
                        return spinDataEvent;
                    }
                }
            }

            double payoutRate = _config.PayoutRate;
            if (_websitePayoutRates.ContainsKey(websiteID))
                payoutRate = _websitePayoutRates[websiteID];

            int betLineType = getLineTypeFromPlayLine(betInfo);
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
            int betLineType     = getLineTypeFromPlayLine(betInfo);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, minOdd, maxOdd, betLineType, -1), TimeSpan.FromSeconds(10.0));
            
            if (spinDataDocument == null)
                return null;
            
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<BasePPSlotSpinData> selectRangeFreeSpinData(int websiteID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
        {
            int betLineType = getLineTypeFromPlayLine(betInfo);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, 1, minOdd, maxOdd, betLineType, -1), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
    }
}
