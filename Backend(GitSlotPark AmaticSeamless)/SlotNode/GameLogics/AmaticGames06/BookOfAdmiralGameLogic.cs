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
   
    class BookOfAdmiralGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BookOfAdmiral";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
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
                return "052326054265265347246574562470648948156357268475618527422d71547850678537856857854735837542783481587697622e48367826425407817628463768536874974184186078152322652843786257086496436815347651876276859850461425422e1607825683516248967145879570452634836425726437522596851751479470543672542857165436471682254583795648524768174376948625860746738224451680680574974518625762753647587408225347807495836798563516704625825638526822595714287951642740782405381625361806380301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b101010101010101010101010091010101010101010101900000000019000000000";
            }
        }
        #endregion

        public BookOfAdmiralGameLogic()
        {
            _gameID     = GAMEID.BookOfAdmiral;
            GameName    = "BookOfAdmiral";
        }

        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BookOfAdmiralInitPacket respinInitPacket = new BookOfAdmiralInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    BookOfAdmiralPacket amaPacket = new BookOfAdmiralPacket(spinResult.ResultString, Cols, FreeCols);
                    respinInitPacket.extrawin   = amaPacket.extrawin;
                    respinInitPacket.extrastr   = amaPacket.extrastr;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, respinInitPacket.extrawin);
            initString = encrypt.WriteLeftHexString(initString, respinInitPacket.extrastr);

            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BookOfAdmiralPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BookOfAdmiralPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BookOfAdmiralPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BookOfAdmiralPacket oldPacket = new BookOfAdmiralPacket(spinResult.ResultString, Cols, FreeCols);
                    packet.betstep          = oldPacket.betstep;
                    packet.betline          = oldPacket.betline;
                    packet.reelstops        = oldPacket.reelstops;
                    packet.freereelstops    = oldPacket.freereelstops;

                    if (type == AmaticMessageType.HeartBeat)
                    {
                        int cnt = oldPacket.linewins.Count;
                        packet.linewins = new List<long>();
                        for (int i = 0; i < cnt; i++)
                        {
                            packet.linewins.Add(0);
                        }
                    }
                    else if (type == AmaticMessageType.Collect)
                    {
                        packet.totalfreecnt = oldPacket.totalfreecnt;
                        packet.curfreecnt   = oldPacket.curfreecnt;
                        packet.curfreewin   = oldPacket.curfreewin;
                        packet.freeunparam1 = oldPacket.freeunparam1;
                        packet.freeunparam2 = oldPacket.freeunparam2;
                        packet.totalfreewin = oldPacket.totalfreewin;

                        int cnt = oldPacket.linewins.Count;
                        packet.linewins = new List<long>();
                        for (int i = 0; i < cnt; i++)
                        {
                            packet.linewins.Add(0);
                        }
                    }

                    packet.extrawin       = oldPacket.extrawin;
                    packet.extrastr   = oldPacket.extrastr;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            BookOfAdmiralPacket respinPacket = null;
            if (packet is BookOfAdmiralPacket)
                respinPacket = packet as BookOfAdmiralPacket;
            else
                respinPacket = new BookOfAdmiralPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, respinPacket.extrawin);
            newSpinString = encrypt.WriteLeftHexString(newSpinString, respinPacket.extrastr);
            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            BookOfAdmiralPacket respinPacket = packet as BookOfAdmiralPacket;

            base.convertWinsByBet(balance, respinPacket, betInfo);
            respinPacket.extrawin = convertWinByBet(respinPacket.extrawin, betInfo);
        }

        public class BookOfAdmiralPacket : AmaticPacket
        {
            public long     extrawin    { get; set; }
            public string   extrastr    { get; set; }

            public BookOfAdmiralPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extrawin    = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                extrastr    = amaConverter.ReadLeftHexString(message, point, out point);
                curpoint    = point;
            }

            public BookOfAdmiralPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extrawin    = 0;
                extrastr    = "091010101010101010101900000000019000000000";
                curpoint    = curpoint + 2 + extrastr.Length;
            }
        }

        public class BookOfAdmiralInitPacket : InitPacket
        {
            public long     extrawin    { get; set; }
            public string   extrastr    { get; set; }

            public BookOfAdmiralInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                extrawin    = 0;
                extrastr    = "091010101010101010101900000000019000000000";
                curpoint    = curpoint + 2 + extrastr.Length;
            }
        }
    }
}
