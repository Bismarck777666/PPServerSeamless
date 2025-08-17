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
using SlotGamesNode.Database;
using PCGSharp;
using MongoDB.Bson;
using GITProtocol.Utils;
using System.Diagnostics;

namespace SlotGamesNode.GameLogics
{
    public class AllwaysCandyBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet    => 1;
    }

    class AllwaysCandyGameLogic : BaseAmaticSpecAnteGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "AllwaysCandy";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 9 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0522b36587938145a6784367a3423746047567158675034522a12304526781923405348a483412312042013461207228528692157082567823a47867a856758a25675678229045867849712358372348673567812563763456782281238465981463145872832458971234678102873522c36587136145a6784367a84237460475671586750346522a12304586781623408348a48341231204201348612022c57865215708256782364786785a8567568275b756786229045865719712356172345678567812563783456782271235467815231456728324569562346781021340301010101010104271010001131f409101010101010100909091100101010101000000000000000001011151a21421e2282322642962c82fa312c315e319031c231f40a10101010101010101010522b36587938145a6784367a3423746047567158675034522a12304526781923405348a483412312042013461207228528692157082567823a47867a856758a2567567822904586784971235837234867356781256376345678228123846598146314587283245897123467810287310032412662be2780f10";
            }
        }
        protected override string ExtraAntePurString => "522b36587938145a6784367a3423746047567158675034522a12304526781923405348a483412312042013461207228528692157082567823a47867a856758a2567567822904586784971235837234867356781256376345678228123846598146314587283245897123467810287310032412662be278";
        protected override bool SupportMoreBet      => true;
        protected override double MoreBetMultiple   => 1.2;
        protected override bool SupportPurchaseFree => true;
        protected double[] PurchaseFreeMultiples
        {
            get
            {
                return new double[] { 65, 102, 190 };
            }
        }
        #endregion

        protected const int     PurFreeCount    = 3;
        protected double []                     _totalPurFreeWinRates   = new double[PurFreeCount]; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double []                     _minPurFreeWinRates     = new double[PurFreeCount]; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값


        public AllwaysCandyGameLogic()
        {
            _gameID     = GAMEID.AllwaysCandy;
            GameName    = "AllwaysCandy";
        }

        protected override async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                var stopWatch = new Stopwatch();

                stopWatch.Start();
                //자연빵 1만개스핀 선택
                double sumOdd1 = 0.0;
                BaseAmaticSlotBetInfo betInfo = new BaseAmaticSlotBetInfo();
                betInfo.MoreBet         = -1;
                betInfo.PurchaseStep    = -1;
                betInfo.PlayLine        = (int)LINES[0];
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
                betInfo.MoreBet = 0;
                for (int i = 0; i < 100000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                    sumOdd2 += spinData.SpinOdd;
                }

                stopWatch.Stop();
                long elapsed2 = stopWatch.ElapsedMilliseconds;
                    
                stopWatch.Reset();
                stopWatch.Start();

                double[] sumOdd3 = new double[3] { 0.0, 0.0 ,0.0 };
                betInfo.MoreBet = -1;

                for(int k = 0; k < PurFreeCount; k++)
                {
                    betInfo.PurchaseStep = k;
                    for (int i = 0; i < 100000; i++)
                    {
                        BasePPSlotSpinData spinData = await selectPurchaseFreeSpin(0, betInfo, 0.0);
                        sumOdd3[k] += spinData.SpinOdd;
                    }

                }
                stopWatch.Stop();
                long elapsed3 = stopWatch.ElapsedMilliseconds;
                long elapsed4 = stopWatch.ElapsedMilliseconds;

                _logger.Info("{0} Performance Test Results:  \r\nPayrate: {10}%, {1}s, {2}%\t{3}s {4}%\t{5}s {6}%\t{7}%\t{8}%\t{9}s", this.GameName,
                    Math.Round((double)elapsed1 / 1000.0, 3), Math.Round(sumOdd1 / 1000, 3),
                    Math.Round((double)elapsed2 / 1000.0, 3), Math.Round(sumOdd2 / (1000 * MoreBetMultiple), 3),
                    Math.Round((double)elapsed3 / 1000.0, 3), Math.Round(sumOdd3[0] / 1000, 3),
                    Math.Round(sumOdd3[1] / 1000, 3), Math.Round(sumOdd3[2] / 1000, 3),
                    Math.Round((double)elapsed4 / 1000.0, 3), _config.PayoutRate);

                
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AllwaysCandyGameLogic::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet             = (double)infoDocument["defaultbet"];
                _normalMaxIDs                   = new int[2];
                _naturalSpinCounts              = new int[2];
                _emptySpinCounts                = new int[2];
                _startIDs                       = new int[2];
                
                var defaultBetsArray            = infoDocument["defaultbet"] as BsonArray;
                var normalMaxIDArray            = infoDocument["normalmaxid"] as BsonArray;
                var emptyCountArray             = infoDocument["emptycount"] as BsonArray;
                var normalSelectCountArray      = infoDocument["normalselectcount"] as BsonArray;
                var startIDArray                = infoDocument["startid"] as BsonArray;

                for (int i = 0; i < 2; i++)
                {
                    _normalMaxIDs[i]            = (int)normalMaxIDArray[i];
                    _emptySpinCounts[i]         = (int)emptyCountArray[i];
                    _naturalSpinCounts[i]       = (int)normalSelectCountArray[i];
                    _startIDs[i]                = (int)startIDArray[i];
                }

                var purchaseOdds = infoDocument["purchaseodds"] as BsonArray;
                for (int i = 0; i < PurFreeCount; i++)
                {
                    _totalPurFreeWinRates[i]    = (double)purchaseOdds[2 * i + 1];
                    _minPurFreeWinRates[i]      = (double)purchaseOdds[2 * i];

                    if (PurchaseFreeMultiples[i] > _totalPurFreeWinRates[i])
                        _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                AllwaysCandyBetInfo betInfo   = new AllwaysCandyBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in AllwaysCandyGameLogic::readBetInfoFromMessage", strUserID);
                    return;
                }

                BaseAmaticSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.PlayLine     = betInfo.PlayLine;
                    oldBetInfo.PlayBet      = betInfo.PlayBet;
                    oldBetInfo.PurchaseStep = betInfo.PurchaseStep;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.CurrencyInfo = betInfo.CurrencyInfo;
                    oldBetInfo.GambleType   = betInfo.GambleType;
                    oldBetInfo.GambleHalf   = betInfo.GambleHalf;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AllwaysCandyGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            AllwaysCandyBetInfo betInfo = new AllwaysCandyBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override double getPurchaseMultiple(BaseAmaticSlotBetInfo betInfo)
        {
            return this.PurchaseFreeMultiples[betInfo.PurchaseStep];
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectPurchaseSpinRequest(this.GameName, StartSpinSearchTypes.MULTISPECIFIC, betInfo.PurchaseStep), TimeSpan.FromSeconds(10.0));
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
                int anteType = getAnteFromBetInfo(betInfo);
                BsonDocument spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, getPurchaseMultiple(betInfo) * 0.2, getPurchaseMultiple(betInfo) * 0.5, anteType, betInfo.PurchaseStep), TimeSpan.FromSeconds(10.0));
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
            double payoutRate = getPayoutRate(websiteID);

            int purIndex    = betInfo.PurchaseStep;
            double targetC  = getPurchaseMultiple(betInfo) * payoutRate / 100.0;
            if (targetC >= _totalPurFreeWinRates[purIndex])
                targetC = _totalPurFreeWinRates[purIndex];

            if (targetC < _minPurFreeWinRates[purIndex])
                targetC = _minPurFreeWinRates[purIndex];

            double x = (_totalPurFreeWinRates[purIndex] - targetC) / (_totalPurFreeWinRates[purIndex] - _minPurFreeWinRates[purIndex]);

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }
    }
}
