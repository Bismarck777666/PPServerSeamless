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
    class BaseAmaticSpecAnteBonusGame : BaseAmaticMultiBaseGame
    {
        protected virtual string ExtraString        => "0f10";
        protected virtual string ExtraAntePurString => "";
        protected virtual string ExtraBonusString   => "";
        protected int getAnteFromBetInfo(BaseAmaticSlotBetInfo betInfo)
        {
            if (betInfo.isMoreBet)
                return betInfo.MoreBet + 1;

            return 0;
        }
        
        #region 스핀관련
        protected override OddAndIDData selectRandomOddAndID(int websiteID, BaseAmaticSlotBetInfo betInfo)
        {
            int     anteType        = getAnteFromBetInfo(betInfo);
            double  payoutRate      = getPayoutRate(websiteID);
            double  randomDouble    = Pcg.Default.NextDouble(0.0, 100.0);
            int selectedID = 0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
                selectedID = _startIDs[anteType] + Pcg.Default.Next(0, _emptySpinCounts[anteType]);
            else
                selectedID = _startIDs[anteType] + Pcg.Default.Next(0, _naturalSpinCounts[anteType]);

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(BaseAmaticSlotBetInfo betInfo)
        {
            int anteType            = getAnteFromBetInfo(betInfo);
            int id                  = _startIDs[anteType] + Pcg.Default.Next(0, _emptySpinCounts[anteType]);
            var spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
        {
            int anteType     = getAnteFromBetInfo(betInfo);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, minOdd, maxOdd, anteType, betInfo.PurchaseStep), TimeSpan.FromSeconds(10.0));
            
            if (spinDataDocument == null)
                return null;
            
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<BasePPSlotSpinData> selectRangeFreeSpinData(int websiteID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
        {
            int anteType    = getAnteFromBetInfo(betInfo);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, 1, minOdd, maxOdd, anteType, betInfo.PurchaseStep), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
        #endregion

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
                double sumOdd3 = 0.0;
                double sumOdd4 = 0.0;

                for (int k = 0; k < 3; k++)
                {
                    betInfo.MoreBet = k;
                    for (int i = 0; i < 100000; i++)
                    {
                        BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                        if(k == 0)
                            sumOdd2 += spinData.SpinOdd;
                        if (k == 1)
                            sumOdd3 += spinData.SpinOdd;
                        if (k == 2)
                            sumOdd4 += spinData.SpinOdd;
                    }

                }

                stopWatch.Stop();
                long elapsed2 = stopWatch.ElapsedMilliseconds;
                
                _logger.Info("{0} Performance Test Results:  \r\nPayrate: {7}%, {1}s, {2}%\t{3}s {4}%\t{5}%\t{6}%\t", this.GameName,
                    Math.Round((double)elapsed1 / 1000.0, 3), Math.Round(sumOdd1 / 1000, 3),
                    Math.Round((double)elapsed2 / 1000.0, 3), Math.Round(sumOdd2 / 1000, 3),
                    Math.Round(sumOdd3 / 1000, 3), Math.Round(sumOdd4 / 1000, 3), _config.PayoutRate);


                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticMultiBaseSlotGame::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
        }

        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                int betLineType         = getAnteFromBetInfo(betInfo);
                var spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
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
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BaseAmaticExtra21InitPacket extraInitPacket = new BaseAmaticExtra21InitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum, ExtraString);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                
                BaseAmaticExtra21Packet amaPacket = new BaseAmaticExtra21Packet(spinResult.ResultString, Cols, FreeCols);
                extraInitPacket.extrastr = amaPacket.extrastr;
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLeftHexString(initString, ExtraBonusString + ExtraAntePurString + extraInitPacket.extrastr);
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
    }
}
