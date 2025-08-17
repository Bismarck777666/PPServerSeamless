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
    class LuckyPiggies2GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyPiggies2";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 4, 8, 12, 16, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 1500, 2000, 3000, 4000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 5 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0528306060105050305050306080802050504070701050504070704040808070303060d0105050606020605050806060305050e060603060f050510070303061107070406060308080707060603120505130806060314060605050506060206000505000102030300050500050501020303030505020505030306060615080808160707071729d070802040103080508060600070308070205030807080804050108080406060308080406060607030808040701080905070a0808070905080804080606090702070308090703080b07050908020808050a08060803080507080802040108070200030705050408010703080500080801080307050804010303030805010603070b050505040806010308050504080604060105030805070707080808092890808070d06020505050800070606010808040305070707080606020807050806070607070e08060708080705080f05060606080208070705051008000707060705080506060111080207120606020706020304040208060008020505020607000207070207070502070802060702071305050208060203070404061407010207150807081606071708101000301010101010104271010001531f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c80610101010101021a0c0d0c0c0c0e0c0c0c0f0c100c110c0c0c0c120c130c140c0c1521a0c130c0c0c100c0c0c0f0c0c0d0c120c0c0c110c140c0e0c0c1521a0c0f0c0c120c0c0c0c0e0c100c0d0c0c0c0c150c130c140c0c1121a0c0c090c0c0c0c0c0a0c0c0c0c090c0c0c0c0c0c0c0c0c0b0c0c21a0c0c0c0a0c0c0c090c0c0c0c0c0b0c0c0c0c0c0c0c0c0c090c0c21a0c0c0c0c0c0a0c0c0c0c0c0c090c0c0c0c0c0c0c0c0c0b0c0c0921a0c0e0c0c0c0d0c0c0c0f0c130c110c0c0c0c150c140c120c0c1021a0c0f0c0c0c140c0c0c120c100c130c0d0c0c0e0c0c0c110c150c21a0c140c0c0c0e0c0c0c120c0d0c110c0c0f0c130c0c0c100c0c15100324b26426410101010101010101010101010101b0a050a010a050a0a0a010a1b0a05020f020102020f0a051b020a0a020a01020a050f0a190102050a0f7c7d7e7f101010101010101010101010101010000000000000000000130000001300000013000000";
            }
        }
        protected string ExtraString => "100324b26426410101010101010101010101010101b010a020a020a0f020a0f0f1b0f020a010a0f0a0f020f0f1b0f0f0a01020f01050a0101190102050a0f7c7d7e7f101010101010101010101010101010000000000000000000130000001300000013000000";
        protected override bool SupportPurchaseFree => true;
        protected override int ReelSetBitNum => 2;
        protected double[] PurchaseFreeMultiples
        {
            get
            {
                return new double[] { 75, 100, 100 };
            }
        }
        protected string ExtraBonusReelSetString => "21a0c0d0c0c0c0e0c0c0c0f0c100c110c0c0c0c120c130c140c0c1521a0c130c0c0c100c0c0c0f0c0c0d0c120c0c0c110c140c0e0c0c1521a0c0f0c0c120c0c0c0c0e0c100c0d0c0c0c0c150c130c140c0c1121a0c0c090c0c0c0c0c0a0c0c0c0c090c0c0c0c0c0c0c0c0c0b0c0c21a0c0c0c0a0c0c0c090c0c0c0c0c0b0c0c0c0c0c0c0c0c0c090c0c21a0c0c0c0c0c0a0c0c0c0c0c0c090c0c0c0c0c0c0c0c0c0b0c0c0921a0c0e0c0c0c0d0c0c0c0f0c130c110c0c0c0c150c140c120c0c1021a0c0f0c0c0c140c0c0c120c100c130c0d0c0c0e0c0c0c110c150c21a0c140c0c0c0e0c0c0c120c0d0c110c0c0f0c130c0c0c100c0c15";
        #endregion

        protected const int     PurFreeCount    = 3;
        protected double []                     _totalPurFreeWinRates   = new double[PurFreeCount]; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double []                     _minPurFreeWinRates     = new double[PurFreeCount]; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값


        public LuckyPiggies2GameLogic()
        {
            _gameID     = GAMEID.LuckyPiggies2;
            GameName    = "LuckyPiggies2";
        }

        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BaseAmaticExtra21InitPacket extraInitPacket = new BaseAmaticExtra21InitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum, ExtraString);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                
                BaseAmaticExtra21Packet     amaPacket   = new BaseAmaticExtra21Packet(spinResult.ResultString, Cols, FreeCols);
                extraInitPacket.extrastr = amaPacket.extrastr;
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLeftHexString(initString, ExtraBonusReelSetString + extraInitPacket.extrastr);
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BaseAmaticExtra21Packet packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BaseAmaticExtra21Packet(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BaseAmaticExtra21Packet(Cols, FreeCols, (int)type, (int)LINES.Last(), ExtraString);

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BaseAmaticExtra21Packet oldPacket = new BaseAmaticExtra21Packet(spinResult.ResultString, Cols, FreeCols);
                    packet.betstep          = oldPacket.betstep;
                    packet.betline          = oldPacket.betline;
                    packet.reelstops        = oldPacket.reelstops;
                    packet.freereelstops    = oldPacket.freereelstops;

                    if (type == AmaticMessageType.HeartBeat)
                    {
                        int cnt = oldPacket.linewins.Count;
                        packet.linewins     = new List<long>();
                        for(int i = 0; i < cnt; i++)
                        {
                            packet.linewins.Add(0);
                        }
                    }
                    else if(type == AmaticMessageType.Collect)
                    {
                        packet.totalfreecnt = oldPacket.totalfreecnt;
                        packet.curfreecnt   = oldPacket.curfreecnt;
                        packet.curfreewin   = oldPacket.curfreewin;
                        packet.freeunparam1 = oldPacket.freeunparam1;
                        packet.freeunparam2 = oldPacket.freeunparam2;
                        packet.totalfreewin = oldPacket.totalfreewin;

                        int cnt = oldPacket.linewins.Count;
                        packet.linewins     = new List<long>();
                        for(int i = 0; i < cnt; i++)
                        {
                            packet.linewins.Add(0);
                        }
                    }

                    packet.extrastr = oldPacket.extrastr;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            BaseAmaticExtra21Packet extraWildPacket = null;
            if (packet is BaseAmaticExtra21Packet)
                extraWildPacket = packet as BaseAmaticExtra21Packet;
            else
                extraWildPacket = new BaseAmaticExtra21Packet(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last(), ExtraString);

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLeftHexString(newSpinString, extraWildPacket.extrastr);
            return newSpinString;
        }

        #region 스핀선택
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
                _logger.Error("Exception has been occurred in LuckyPiggies2GameLogic::selectRandomStartFreeSpinData {0}", ex);
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
                _logger.Error("Exception has been occurred in LuckyPiggies2GameLogic::selectMinStartFreeSpinData {0}", ex);
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
        #endregion
    }
}
