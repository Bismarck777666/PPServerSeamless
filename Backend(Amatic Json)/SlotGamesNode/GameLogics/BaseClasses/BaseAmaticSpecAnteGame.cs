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
    class BaseAmaticSpecAnteGame : BaseAmaticMultiBaseGame
    {
        protected virtual string ExtraString        => "0f10";
        protected virtual string ExtraAntePurString => "";

        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet             = (double)infoDocument["defaultbet"];
                _normalMaxIDs                   = new int[2];
                _naturalSpinCounts              = new int[2];
                _emptySpinCounts                = new int[2];
                _startIDs                       = new int[2];
                _totalFreeSpinWinRates          = new double[2];
                _minFreeSpinWinRates            = new double[2];

                var defaultBetsArray            = infoDocument["defaultbet"] as BsonArray;
                var normalMaxIDArray            = infoDocument["normalmaxid"] as BsonArray;
                var emptyCountArray             = infoDocument["emptycount"] as BsonArray;
                var normalSelectCountArray      = infoDocument["normalselectcount"] as BsonArray;
                var startIDArray                = infoDocument["startid"] as BsonArray;

                var purchaseOdds = new BsonArray();
                if (SupportPurchaseFree)
                    purchaseOdds = infoDocument["purchaseodds"] as BsonArray;

                for (int i = 0; i < 2; i++)
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
        protected int getAnteFromBetInfo(BaseAmaticSlotBetInfo betInfo)
        {
            if (betInfo.isMoreBet)
                return 1;

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
    
        protected override string buildInitString(string strUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strUserID, balance, currency);
            BaseAmaticExtra21InitPacket extraInitPacket = new BaseAmaticExtra21InitPacket(initString, Cols, FreeCols, ReelSetBitNum, ExtraString);

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strUserID];
                if (betInfo.HasRemainResponse)
                {
                    BaseAmaticExtra21Packet amaPacket = new BaseAmaticExtra21Packet(spinResult.ResultString, Cols, FreeCols);
                    extraInitPacket.extrastr = amaPacket.extrastr;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLeftHexString(initString, ExtraAntePurString + extraInitPacket.extrastr);
            return initString;
        }
        protected override string buildResMsgString(string strUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
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

                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

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
