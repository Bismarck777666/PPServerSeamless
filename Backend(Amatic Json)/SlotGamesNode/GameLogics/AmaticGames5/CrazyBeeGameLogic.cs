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
   
    class CrazyBeeGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "CrazyBee";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 10 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "05214178537240683756948262146271843507463582769821473586437951782604a8621454270863714352896876214734124873687052698655214178537240683756948262146271843507463582769821473586437951782604a8621454270863714352896876214734124873687052698650301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b101010101010101010101010";
            }
        }
        #endregion

        public CrazyBeeGameLogic()
        {
            _gameID     = GAMEID.CrazyBee;
            GameName    = "CrazyBee";
        }

        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            CrazyBeePacket crazyPacket = packet as CrazyBeePacket;
            
            base.convertWinsByBet(balance, packet, betInfo);
            crazyPacket.extrawin = convertWinByBet(crazyPacket.extrawin, betInfo);
        }

        protected override string buildInitString(string strUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strUserID, balance, currency);
            CrazyBeeInitPacket extraInitPacket = new CrazyBeeInitPacket(initString, Cols, FreeCols, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strUserID];
                if (betInfo.HasRemainResponse)
                {
                    CrazyBeePacket amaPacket = new CrazyBeePacket(spinResult.ResultString, Cols, FreeCols);
                    extraInitPacket.extrawin = amaPacket.extrawin;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, extraInitPacket.extrawin);
            return initString;
        }

        protected override string buildResMsgString(string strUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            CrazyBeePacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new CrazyBeePacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new CrazyBeePacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

                    CrazyBeePacket oldPacket = new CrazyBeePacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extrawin = oldPacket.extrawin;
                }
            }

            return buildSpinString(packet);
        }

        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            CrazyBeePacket extraWildPacket = null;
            if (packet is CrazyBeePacket)
                extraWildPacket = packet as CrazyBeePacket;
            else
                extraWildPacket = new CrazyBeePacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, extraWildPacket.extrawin);
            return newSpinString;
        }

        public class CrazyBeePacket : AmaticPacket
        {
            public long extrawin { get; set; }

            public CrazyBeePacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extrawin = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                curpoint = point;
            }

            public CrazyBeePacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extrawin = 0;
                curpoint = curpoint + 2;
            }
        }

        public class CrazyBeeInitPacket : InitPacket
        {
            public long extrawin { get; set; }

            public CrazyBeeInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetBitCnt)
            {
                extrawin = 0;
                curpoint = curpoint + 2;
            }
        }
    }
}
