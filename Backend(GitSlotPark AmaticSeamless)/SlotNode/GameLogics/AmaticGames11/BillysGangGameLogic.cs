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
    class BillysGangGameLogic : BaseAmaticPurAnteGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BillysGang";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 20 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0524b06040208030205050508040908060300080707070306060800040800030401040608090406080208020806040906080301070802030607080906070108030208060204070802060400030825000060505050007040208060003040006070406080800030601070707040d06000601010404060604000e060f040101060306100300040602050406030208080611010306021204130602040602030006242080305050508040a08030a08070707030004020806000301040801030203010803020a04010802030603060a060103000408080a0606010408000303080a030408062490300050300080501080401050802030501080707070d080105080205060e080500080204040801050302080501060800030501080300080806020201000308080f0805050200080610239060304070707080203040b0802040608060103040206050b030105000405060103020504050601080b06020303040006050b030308030800040030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a264151010101010101010101010101010101010101010102290c130c0c0c100c0c0c0f0c0c0d0c120c0c0c110c140c0e0c0c1a0c0c150c0c160c0c170c180c0c0c192290c130c0c1a0c0c0c100c0c0c0f0c0c190c0c110c140c0c170c0c0d0c120c180c0e0c0c150c0c160c0c2290c0c100c170c0c0c1a0c0d0c120c180c0e0c0c0c0f0c0c190c0c110c0c0c150c0c0c160c0c130c0c142290c0c100c170c180c0c110c140c0c0c1a0c0d0c0c120c0c0c150c0c0c160c0c0e0c0f0c0c0c190c130c2290c0c170c120c0c100c110c160c0c0c180c0c0c0d0c0c150c0c140c0c0e0c0c1a0c0c0f0c0c190c130c2290c0c170c120c0c160c100c0c0c0d0c190c180c0c0c1a0c0f0c0c0c110c0c140c0c150c0c0e0c0c130c2260c0c0a0c0c0c0c0c0c0c0c0c0c0c0c0a0c0c0c0c0c0c0c0c0a0c0c0c0c0c0a0c0c0c0c0c0a0c2260c0c0a0c0c0c0c0c0c0c0a0c0c0c0c0c0c0c0c0a0c0c0c0c0c0c0a0c0c0c0a0c0c0c0c0c0c0c2260c0c0a0c0c0c0c0c0c0c0a0c0c0c0c0c0a0c0c0c0c0c0c0c0a0c0c0c0c0c0c0c0a0c0c0c0c0c2290c0c170c120c0c160c100c0c1a0c0c0c0d0c190c180c0c0f0c0c0c110c0c140c0c150c0c0e0c0c130c2290c0c170c120c1a0c0c0c100c110c160c0c0c180c0c0c0d0c0c150c0c140c0c0e0c0c0f0c0c190c130c2290c0c100c170c180c0c110c140c0c0d0c0c120c0c0c150c0c0c160c1a0c0c0c0e0c0f0c0c0c190c130c2290c130c0c0c100c0c0c0f0c0c0d0c120c1a0c0c0c0c110c140c0e0c0c150c0c160c0c170c180c0c0c192290c0c100c170c0c0d0c120c180c0e0c0c0c0f0c1a0c0c0c190c0c110c0c0c150c0c0c160c0c130c0c142290c130c0c0c100c0c0c0f0c0c190c0c110c140c0c170c0c0d0c1a0c0c120c180c0e0c0c150c0c160c0c10032322642961010101010101010101010101010101010101a1f151f13214121f1f1a112192141121412131521921912131f2191511152142141f1314112191411121314151a1f2142192322642962fa31f41010101010101010101010101010101010101010100000000000";
            }
        }
        protected override string ExtraAntePurString => "2290c130c0c0c100c0c0c0f0c0c0d0c120c0c0c110c140c0e0c0c1a0c0c150c0c160c0c170c180c0c0c192290c130c0c1a0c0c0c100c0c0c0f0c0c190c0c110c140c0c170c0c0d0c120c180c0e0c0c150c0c160c0c2290c0c100c170c0c0c1a0c0d0c120c180c0e0c0c0c0f0c0c190c0c110c0c0c150c0c0c160c0c130c0c142290c0c100c170c180c0c110c140c0c0c1a0c0d0c0c120c0c0c150c0c0c160c0c0e0c0f0c0c0c190c130c2290c0c170c120c0c100c110c160c0c0c180c0c0c0d0c0c150c0c140c0c0e0c0c1a0c0c0f0c0c190c130c2290c0c170c120c0c160c100c0c0c0d0c190c180c0c0c1a0c0f0c0c0c110c0c140c0c150c0c0e0c0c130c2260c0c0a0c0c0c0c0c0c0c0c0c0c0c0c0a0c0c0c0c0c0c0c0c0a0c0c0c0c0c0a0c0c0c0c0c0a0c2260c0c0a0c0c0c0c0c0c0c0a0c0c0c0c0c0c0c0c0a0c0c0c0c0c0c0a0c0c0c0a0c0c0c0c0c0c0c2260c0c0a0c0c0c0c0c0c0c0a0c0c0c0c0c0a0c0c0c0c0c0c0c0a0c0c0c0c0c0c0c0a0c0c0c0c0c2290c0c170c120c0c160c100c0c1a0c0c0c0d0c190c180c0c0f0c0c0c110c0c140c0c150c0c0e0c0c130c2290c0c170c120c1a0c0c0c100c110c160c0c0c180c0c0c0d0c0c150c0c140c0c0e0c0c0f0c0c190c130c2290c0c100c170c180c0c110c140c0c0d0c0c120c0c0c150c0c0c160c1a0c0c0c0e0c0f0c0c0c190c130c2290c130c0c0c100c0c0c0f0c0c0d0c120c1a0c0c0c0c110c140c0e0c0c150c0c160c0c170c180c0c0c192290c0c100c170c0c0d0c120c180c0e0c0c0c0f0c1a0c0c0c190c0c110c0c0c150c0c0c160c0c130c0c142290c130c0c0c100c0c0c0f0c0c190c0c110c140c0c170c0c0d0c1a0c0c120c180c0e0c0c150c0c160c0c10032322642961010101010101010101010101010101010101a1f151f13214121f1f1a112192141121412131521921912131f2191511152142141f1314112191411121314151a1f2142192322642962fa31f41010101010101010101010101010101010101010100000000000";
        protected override bool SupportPurchaseFree => true;
        protected override int ReelSetBitNum => 2;
        protected double[] PurchaseFreeMultiples
        {
            get
            {
                return new double[] { 50, 100, 150 };
            }
        }
        #endregion

        protected const int     PurFreeCount    = 3;
        protected double []                     _totalPurFreeWinRates   = new double[PurFreeCount]; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double []                     _minPurFreeWinRates     = new double[PurFreeCount]; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값


        public BillysGangGameLogic()
        {
            _gameID     = GAMEID.BillysGang;
            GameName    = "BillysGang";
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
                BsonDocument spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequestWithPuri(GameName, 1, getPurchaseMultiple(betInfo) * 0.2, getPurchaseMultiple(betInfo) * 0.5, betInfo.PurchaseStep), TimeSpan.FromSeconds(10.0));
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
