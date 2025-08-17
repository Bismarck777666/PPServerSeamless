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

namespace SlotGamesNode.GameLogics
{
   
    class BaseAmaticExtra3Game : BaseAmaticSlotGame
    {
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BaseAmaticExtra3InitPacket initPacket = new BaseAmaticExtra3InitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    BaseAmaticExtra3Packet amaPacket = new BaseAmaticExtra3Packet(spinResult.ResultString, Cols, FreeCols);
                    initPacket.extrawildsymbol  = amaPacket.extraymbol;
                    initPacket.extracolcnt      = amaPacket.extrawin;
                    initPacket.extrawin         = amaPacket.extracolcnt;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrawildsymbol);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extracolcnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrawin);
            
            return initString;
        }

        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BaseAmaticExtra3Packet packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BaseAmaticExtra3Packet(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BaseAmaticExtra3Packet(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BaseAmaticExtra3Packet oldPacket = new BaseAmaticExtra3Packet(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extraymbol   = oldPacket.extraymbol;
                    packet.extracolcnt  = oldPacket.extracolcnt;
                    packet.extrawin     = oldPacket.extrawin;
                }
            }

            return buildSpinString(packet);
        }

        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            BaseAmaticExtra3Packet bookOfQueenPacket = packet as BaseAmaticExtra3Packet;
            
            if (packet is BaseAmaticExtra3Packet)
                bookOfQueenPacket = packet as BaseAmaticExtra3Packet;
            else
                bookOfQueenPacket = new BaseAmaticExtra3Packet(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfQueenPacket.extraymbol);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfQueenPacket.extracolcnt);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfQueenPacket.extrawin);
            
            return newSpinString;
        }

        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            BaseAmaticExtra3Packet bookOfFortunePacket = packet as BaseAmaticExtra3Packet;
            
            base.convertWinsByBet(balance, bookOfFortunePacket, betInfo);
            bookOfFortunePacket.extrawin = convertWinByBet(bookOfFortunePacket.extrawin, betInfo);
        }

        public class BaseAmaticExtra3Packet : AmaticPacket
        {
            public long         extraymbol          { get; set; }
            public long         extracolcnt         { get; set; }
            public long         extrawin            { get; set; }

            public BaseAmaticExtra3Packet(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extraymbol      = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                extracolcnt     = amaConverter.ReadLengthAndDec(message, point, out point);
                extrawin        = amaConverter.ReadLengthAndDec(message, point, out point);
                curpoint        = point;
            }

            public BaseAmaticExtra3Packet(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extraymbol = 0;
                extracolcnt     = 0;
                extrawin        = 0;
                curpoint        = 2 + 2 + 2 + curpoint;
            }
        }

        public class BaseAmaticExtra3InitPacket : InitPacket
        {
            public long         extrawildsymbol { get; set; }
            public long         extracolcnt     { get; set; }
            public long         extrawin        { get; set; }

            public BaseAmaticExtra3InitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                extrawildsymbol = 0;
                extracolcnt     = 0;
                extrawin        = 0;
                curpoint        = 2 + 2 + 2 + curpoint;
            }
        }
    }
}
