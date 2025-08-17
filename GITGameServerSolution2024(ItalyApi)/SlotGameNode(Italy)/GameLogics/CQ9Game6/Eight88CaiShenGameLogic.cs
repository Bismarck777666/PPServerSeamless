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
using Newtonsoft.Json.Linq;
using SlotGamesNode.Database;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
   
    class Eight88CaiShenGameLogic : BaseCQ9SlotGame
    {
        //프리스핀구매기능이 있을떄만 필요하다. 디비안의 모든(구매가능한) 프리스핀들의 오드별 아이디어레이
        protected SortedDictionary<double, int[]>   _totalFreeSpin2OddIds   = new SortedDictionary<double, int[]>();
        protected int                               _freeSpin2TotalCount    = 0;
        protected int                               _minFreeSpin2TotalCount = 0;
        protected double                            _totalFreeSpin2WinRate  = 0.0; //스핀디비안의 모든 스틱키 프리스핀들의 배당평균값
        protected double                            _minFreeSpin2WinRate    = 0.0; //구매금액의 20% - 50%사이에 들어가는 모든 스틱키 프리스핀들의 평균배당값

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "227";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 60;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 10, 20, 30, 60, 120, 250, 500, 1000, 2000, 1, 2, 4, 5 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 2000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "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";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "888 Cai Shen";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"ko\", \"name\": \"대박 재물신\"}," +
                        "{ \"lang\": \"zh-cn\", \"name\": \"发发发财神\"}," +
                        "{ \"lang\": \"en\", \"name\": \"888 Cai Shen\"}," +
                        "{ \"lang\": \"th\", \"name\": \"ฟาชัยเซ็น แจกไม่อั้น\"}]";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple => 3600.0 / 60;

        private double PurchaseFree2Multiple = 7200.0 / 60;
        #endregion
        public Eight88CaiShenGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            List<CQ9ExtendedFeature2> extendFeatureByGame2 = new List<CQ9ExtendedFeature2>()
            {
                new CQ9ExtendedFeature2(){ name = "FeatureMinBet",value = "3600" },
                new CQ9ExtendedFeature2(){ name = "Feature2MinBet",value = "7200" }
            };
            _initData.ExtendFeatureByGame2 = extendFeatureByGame2;

            _gameID                 = GAMEID.Eight88CaiShen;
            GameName                = "Eight88CaiShen";
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Props.Create(() => new SpinDatabase(this._providerName, this.GameName)), "spinDatabase");
                bool isSuccess = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.Normal), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _normalMaxID            = response.NormalMaxID;
                _naturalSpinOddProbs    = new SortedDictionary<double, int>();
                _spinDataDefaultBet     = response.DefaultBet;
                _naturalSpinCount       = 0;
                _emptySpinIDs           = new List<int>();

                Dictionary<double, List<int>> totalSpinOddIds   = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>> totalSpin2OddIds  = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>> freeSpinOddIds    = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>> freeSpin2OddIds   = new Dictionary<double, List<int>>();

                double freeSpinTotalOdd = 0.0, freeSpin2TotalOdd = 0.0;
                double minFreeSpinTotalOdd = 0.0, minFreeSpin2TotalOdd = 0.0;
                int freeSpinTotalCount = 0, freeSpin2TotalCount = 0;
                int minFreeSpinTotalCount = 0, minFreeSpin2TotalCount = 0;

                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinBaseData spinBaseData = response.SpinBaseDatas[i];
                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        _naturalSpinCount++;
                        if (_naturalSpinOddProbs.ContainsKey(spinBaseData.Odd))
                            _naturalSpinOddProbs[spinBaseData.Odd]++;
                        else
                            _naturalSpinOddProbs[spinBaseData.Odd] = 1;
                    }

                    if(spinBaseData.SpinType < 2)
                    {
                        if (!totalSpinOddIds.ContainsKey(spinBaseData.Odd))
                            totalSpinOddIds.Add(spinBaseData.Odd, new List<int>());

                        if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                            _emptySpinIDs.Add(spinBaseData.ID);

                        totalSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);
                    }
                    else
                    {
                        if (!totalSpin2OddIds.ContainsKey(spinBaseData.Odd))
                            totalSpin2OddIds.Add(spinBaseData.Odd, new List<int>());

                        totalSpin2OddIds[spinBaseData.Odd].Add(spinBaseData.ID);
                    }

                    if (spinBaseData.SpinType == 1)
                    {
                        freeSpinTotalCount++;
                        freeSpinTotalOdd += spinBaseData.Odd;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.Odd >= PurchaseFreeMultiple * 0.2 && spinBaseData.Odd <= PurchaseFreeMultiple * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += spinBaseData.Odd;
                        }
                    }
                    else if(spinBaseData.SpinType == 2)
                    {
                        freeSpin2TotalCount++;
                        freeSpin2TotalOdd += spinBaseData.Odd;

                        if (!freeSpin2OddIds.ContainsKey(spinBaseData.Odd))
                            freeSpin2OddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpin2OddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.Odd >= PurchaseFree2Multiple * 0.2 && spinBaseData.Odd <= PurchaseFree2Multiple * 0.5)
                        {
                            minFreeSpin2TotalCount++;
                            minFreeSpin2TotalOdd += spinBaseData.Odd;
                        }
                    }
                }

                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIds)
                    _totalFreeSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                _totalFreeSpin2OddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in freeSpin2OddIds)
                    _totalFreeSpin2OddIds.Add(pair.Key, pair.Value.ToArray());

                _freeSpinTotalCount     = freeSpinTotalCount;
                _minFreeSpinTotalCount  = minFreeSpinTotalCount;
                _totalFreeSpinWinRate   = freeSpinTotalOdd / freeSpinTotalCount;
                _minFreeSpinWinRate     = minFreeSpinTotalOdd / minFreeSpinTotalCount;
                
                _freeSpin2TotalCount    = freeSpin2TotalCount;
                _minFreeSpin2TotalCount = minFreeSpin2TotalCount;
                _totalFreeSpin2WinRate  = freeSpin2TotalOdd / freeSpin2TotalCount;
                _minFreeSpin2WinRate    = minFreeSpin2TotalCount / minFreeSpin2TotalCount;

                if (_totalFreeSpinWinRate <= _minFreeSpinWinRate || _minFreeSpinTotalCount == 0)
                    _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);

                if (_totalFreeSpin2WinRate <= _minFreeSpin2WinRate || _minFreeSpin2TotalCount == 0)
                    _logger.Error("min freespin2 rate doesn't satisfy condition {0}", this.GameName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseCQ9SlotBetInfo betInfo)
        {
            try
            {
                if(betInfo.ReelPay / (betInfo.MiniBet * betInfo.PlayBet) == PurchaseFree2Multiple)
                {
                    OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpin2OddIds, _freeSpin2TotalCount);
                    return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                }
                else
                {
                    OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIds, _freeSpinTotalCount);
                    return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseCQ9SlotBetInfo betInfo)
        {
            try
            {
                if(betInfo.ReelPay / (betInfo.MiniBet * betInfo.PlayBet) == PurchaseFree2Multiple)
                {
                    OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpin2OddIds, _minFreeSpin2TotalCount, PurchaseFree2Multiple * 0.2, PurchaseFree2Multiple * 0.5);
                    return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                }
                else
                {
                    OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, _minFreeSpinTotalCount, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5);
                    return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int companyID, BaseCQ9SlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID;
                if (betInfo.ReelPay / (betInfo.MiniBet * betInfo.PlayBet) == PurchaseFree2Multiple)
                    oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpin2OddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                else
                    oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;

                    convertExtraDataBuyFeature(spinDataEvent, betInfo);
                    
                    return spinDataEvent;
                }
            }

            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(companyID))
                payoutRate = _agentPayoutRates[companyID];

            double x = 0;
            if (betInfo.ReelPay / (betInfo.MiniBet * betInfo.PlayBet) == PurchaseFree2Multiple)
            {
                double targetC = PurchaseFree2Multiple * payoutRate / 100.0;
                if (targetC >= _totalFreeSpin2WinRate)
                    targetC = _totalFreeSpin2WinRate;

                if (targetC < _minFreeSpin2WinRate)
                    targetC = _minFreeSpin2WinRate;

                x = (_totalFreeSpin2WinRate - targetC) / (_totalFreeSpin2WinRate - _minFreeSpin2WinRate);
            }
            else
            {
                double targetC = PurchaseFreeMultiple * payoutRate / 100.0;
                if (targetC >= _totalFreeSpinWinRate)
                    targetC = _totalFreeSpinWinRate;

                if (targetC < _minFreeSpinWinRate)
                    targetC = _minFreeSpinWinRate;

                x = (_totalFreeSpinWinRate - targetC) / (_totalFreeSpinWinRate - _minFreeSpinWinRate);
            }

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);

            convertExtraDataBuyFeature(spinData, betInfo);

            return spinData;
        }

        protected void convertExtraDataBuyFeature(BasePPSlotSpinData spinData,BaseCQ9SlotBetInfo betInfo)
        {
            string strResponse      = spinData.SpinStrings[0];
            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(strResponse);
            if (!object.ReferenceEquals(resultContext["ExtraData"], null))
            {
                if(betInfo.ReelPay / (betInfo.MiniBet * betInfo.PlayBet) == PurchaseFree2Multiple)
                    resultContext["ExtraData"][0] = 2;
                else
                    resultContext["ExtraData"][0] = 1;
            }
            spinData.SpinStrings[0] = JsonConvert.SerializeObject(resultContext);
        }
    }
}
