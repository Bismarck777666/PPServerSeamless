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
    class BookOfPharaoGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BookOfPharao";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 8, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "052326054265265347246574562470648948156357268475618527422d71547850678537856857854735837542783481587697622e48367826425407817628463768536874974184186078152322652843786257086496436815347651876276859850461425422e1607825683524561961745879870452634836425726437522596851751479470543672542857165436471682254583795648524768174376948625860746738224451680680574974518625762753647587408225347807495836798563516704625825638526822595714287951642740782405381625361806380301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010522596851751479470543672542857165436471682254583795648524768174376948625860746738224451680680574974518625762753647587408225347807495836798563516704625825638526822595714287951642740782405381625361806381010101500000";
            }
        }
        #endregion

        public BookOfPharaoGameLogic()
        {
            _gameID     = GAMEID.BookOfPharao;
            GameName    = "BookOfPharao";
        }

        protected override string buildInitString(string strUserID, double balance, Currencies currency)
        {
            AmaticEncrypt encrypt = new AmaticEncrypt();
            string initString = string.Empty;

            BookOfPharaoInitPacket initPacket = new BookOfPharaoInitPacket(InitString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);
            initPacket.betstepamount    = BettingButton.ToList();
            initPacket.laststep         = 0;
            initPacket.minbet           = BettingButton[0];
            initPacket.maxbet           = BettingButton.ToList().Last() * LINES.ToList().Last();
            initPacket.lastline         = LINES.ToList().Last();

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

                initPacket.laststep = betInfo.PlayBet;
                initPacket.lastline = betInfo.PlayLine;
                initPacket.win      = 0;

                AmaticPacket amaPacket = new AmaticPacket(spinResult.ResultString, Cols, FreeCols);
                initPacket.messagetype      = amaPacket.messagetype;
                if ((AmaticMessageType)amaPacket.messagetype == AmaticMessageType.GamblePick || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.GambleHalf
                    || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastFree || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastRespin
                    || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastPower || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastWheel)
                    initPacket.messagetype = (int)AmaticMessageType.NormalSpin;
                initPacket.reelstops        = amaPacket.reelstops;
                initPacket.freereelstops    = amaPacket.freereelstops;
                initPacket.gamblelogs       = amaPacket.gamblelogs;
                if (betInfo.HasRemainResponse || amaPacket.messagetype == (long)AmaticMessageType.FreeOption)
                {
                    if(amaPacket.messagetype == (long)AmaticMessageType.FreeTrigger || amaPacket.messagetype == (long)AmaticMessageType.FreeSpin || amaPacket.messagetype == (long)AmaticMessageType.ExtendFree)
                        initPacket.messagetype = (long)AmaticMessageType.FreeReopen;

                    if (amaPacket.messagetype == (long)AmaticMessageType.TriggerPower || amaPacket.messagetype == (long)AmaticMessageType.PowerRespin)
                        initPacket.messagetype = (long)AmaticMessageType.FreeSpin;

                    initPacket.totalfreecnt     = amaPacket.totalfreecnt;
                    initPacket.curfreecnt       = amaPacket.curfreecnt;
                    initPacket.curfreewin       = amaPacket.curfreewin;
                    initPacket.freeunparam1     = amaPacket.freeunparam1;
                    initPacket.freeunparam2     = amaPacket.freeunparam2;
                    initPacket.totalfreewin     = amaPacket.totalfreewin;

                    initPacket.linewins         = amaPacket.linewins;
                    initPacket.win              = amaPacket.win;

                    initPacket.reelstops        = amaPacket.freereelstops;
                }
            }

            initString = encrypt.WriteDecHex(initString, initPacket.messageheader);
            initString = encrypt.WriteDecHex(initString, initPacket.reelset.Count);
            for(int i = 0; i < initPacket.reelset.Count; i++)
            {
                if(ReelSetBitNum == 1)
                    initString = encrypt.Write1BitNumArray(initString, initPacket.reelset[i]);
                else if(ReelSetBitNum == 2)
                    initString = encrypt.Write2BitNumArray(initString, initPacket.reelset[i]);
            }
            
            initString = encrypt.WriteDecHex(initString, initPacket.freereelset.Count);
            for (int i = 0; i < initPacket.freereelset.Count; i++)
            {
                if(ReelSetBitNum == 1)
                    initString = encrypt.Write1BitNumArray(initString, initPacket.freereelset[i]);
                else if(ReelSetBitNum == 2)
                    initString = encrypt.Write2BitNumArray(initString, initPacket.freereelset[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.messagetype);
            initString = encrypt.WriteDecHex(initString, initPacket.sessionclose);

            int reelStopCnt = initPacket.reelstops.Count > 5 ? initPacket.reelstops.Count : 5;
            if(initPacket.reelstops.Count >= 5)
            {
                for (int i = 0; i < reelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.reelstops[i]);
            }
            else
            {
                for (int i = 0; i < initPacket.reelstops.Count; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.reelstops[i]);

                for (int i = initPacket.reelstops.Count; i < reelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, 0);
            }

            initString = encrypt.WriteLengthAndDec(initString, initPacket.messageid);

            double pointUnit = getPointUnit(new BaseAmaticSlotBetInfo() { CurrencyInfo = currency });
            long balanceUnit = (long)Math.Round(balance / pointUnit, 0);
            initString = encrypt.WriteLengthAndDec(initString, balanceUnit);        //현재 화페와 단위금액으로 변환된 발란스
            initString = encrypt.WriteLengthAndDec(initString, initPacket.win);     //당첨금(인이트의 경우에는 0)
            initString = encrypt.WriteDec2Hex(initString, initPacket.laststep);     //마지막스핀 스텝
            initString = encrypt.WriteLengthAndDec(initString, initPacket.minbet);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.maxbet);  
            initString = encrypt.WriteDec2Hex(initString, initPacket.lastline);     //마지막스핀 라인

            initString = encrypt.WriteLengthAndDec(initString, initPacket.totalfreecnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.curfreecnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.curfreewin);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.freeunparam1);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.freeunparam2);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.totalfreewin);

            initString = encrypt.WriteLengthAndDec(initString, initPacket.unknownparam1);
            initString = encrypt.WriteDec2Hex(initString, initPacket.minbetline);
            initString = encrypt.WriteDec2Hex(initString, initPacket.maxbetline);
            initString = encrypt.WriteDec2Hex(initString, initPacket.unitbetline);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.unknownparam2);
            initString = encrypt.WriteDec2Hex(initString, initPacket.unknownparam3);

            int freeReelStopCnt = initPacket.freereelstops.Count > 5 ? initPacket.freereelstops.Count : 5;
            if (initPacket.freereelstops.Count >= 5)
            {
                for (int i = 0; i < freeReelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.freereelstops[i]);
            }
            else
            {
                for (int i = 0; i < initPacket.freereelstops.Count; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.freereelstops[i]);

                for (int i = initPacket.freereelstops.Count; i < freeReelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, 0);
            }

            for (int i = 0; i < initPacket.gamblelogs.Count; i++)
            {
                initString = encrypt.WriteDec2Hex(initString, initPacket.gamblelogs[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.betstepamount.Count);
            for(int i = 0; i < initPacket.betstepamount.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.betstepamount[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.linewins.Count);
            for (int i = 0; i < initPacket.linewins.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.linewins[i]);
            }
            
            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strUserID];
                if (betInfo.HasRemainResponse)
                {
                    BookOfPharaoPacket amaPacket = new BookOfPharaoPacket(spinResult.ResultString, Cols, FreeCols);
                    initPacket.extraymbol       = amaPacket.extraymbol;
                    initPacket.extracolcnt      = amaPacket.extracolcnt;
                    initPacket.extrawin         = amaPacket.extrawin;
                    initPacket.extrastring      = amaPacket.extrastring;
                    if (amaPacket.messagetype == (int)AmaticMessageType.FreeTrigger)
                        initPacket.extrastring = "15FFFFF";
                }
            }

            initString = encrypt.WriteLeftHexString(initString, initPacket.freesetstring);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extraymbol);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extracolcnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrawin);
            initString = encrypt.WriteLeftHexString(initString, initPacket.extrastring);

            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BookOfPharaoPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BookOfPharaoPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BookOfPharaoPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BookOfPharaoPacket oldPacket = new BookOfPharaoPacket(spinResult.ResultString, Cols, FreeCols);
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
                    packet.extrastring  = oldPacket.extrastring;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            BookOfPharaoPacket bookOfPharaoPacket = packet as BookOfPharaoPacket;
            
            if (packet is BookOfPharaoPacket)
                bookOfPharaoPacket = packet as BookOfPharaoPacket;
            else
                bookOfPharaoPacket = new BookOfPharaoPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfPharaoPacket.extraymbol);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfPharaoPacket.extracolcnt);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfPharaoPacket.extrawin);
            newSpinString = encrypt.WriteLeftHexString(newSpinString, bookOfPharaoPacket.extrastring);

            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            BookOfPharaoPacket bookOfPharaoPacket = packet as BookOfPharaoPacket;
            
            base.convertWinsByBet(balance, bookOfPharaoPacket, betInfo);
            bookOfPharaoPacket.extrawin = convertWinByBet(bookOfPharaoPacket.extrawin, betInfo);
        }

        public class BookOfPharaoPacket : AmaticPacket
        {
            public long         extraymbol      { get; set; }
            public long         extracolcnt     { get; set; }
            public long         extrawin        { get; set; }
            public string       extrastring     { get; set; }

            public BookOfPharaoPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extraymbol      = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                extracolcnt     = amaConverter.ReadLengthAndDec(message, point, out point);
                extrawin        = amaConverter.ReadLengthAndDec(message, point, out point);
                extrastring     = amaConverter.ReadLeftHexString(message, point, out point);
                curpoint        = point;
            }

            public BookOfPharaoPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extraymbol      = 0;
                extracolcnt     = 0;
                extrawin        = 0;
                extrastring     = "1500000";
                curpoint        = curpoint + 2 + 2 + 2 + extrastring.Length;
            }
        }
        public class BookOfPharaoInitPacket : InitPacket
        {
            public string       freesetstring   { get; set; }
            public long         extraymbol      { get; set; }
            public long         extracolcnt     { get; set; }
            public long         extrawin        { get; set; }
            public string       extrastring     { get; set; }
            public BookOfPharaoInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                freesetstring   = "52259685175147947054367254285716543647168225458379564852476817437694862586074673822445168068057497451862576275364758740822534780749583679856351670462582563852682259571428795164274078240538162536180638";
                extraymbol      = 0;
                extracolcnt     = 0;
                extrawin        = 0;
                extrastring     = "1500000";
                curpoint        = curpoint + freesetstring.Length + 2 + 2 + 2 + extrastring.Length;
            }
        }
    }
}
