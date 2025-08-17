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
    class BookOfAztecBonusBuyGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BookOfAztecBonusBuy";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
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
                return "052326054265265347246574562480468947156357268475618527422d71547850678537856857854735837542783751687694822e48367826425407817628437648376874985146186078152322652843786257086596436815347651876284679850461425422e1607825683516248967145879570452634836425726437522594715741689570543672542857165436471682254583795648524768174376948625860746738224451640680574985718625762753647587408225347807495836798563516704625825638526822595714287951642740782405381625361806380301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b10101010101010101010101001253101010";
            }
        }

        protected string ExtraAntePurString => "1001253";

        protected override bool SupportPurchaseFree => true;

        protected override double PurchaseFreeMultiple
        {
            get
            {
                return 83.0;
            }
        }
        #endregion

        public BookOfAztecBonusBuyGameLogic()
        {
            _gameID     = GAMEID.BookOfAztecBonusBuy;
            GameName    = "BookOfAztecBonusBuy";
        }

        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BookOfAztecBonusBuyInitPacket initPacket = new BookOfAztecBonusBuyInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum, (int)LINES.Last());

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    BookOfAztecBonusBuyPacket amaPacket = new BookOfAztecBonusBuyPacket(spinResult.ResultString, Cols, FreeCols);
                    initPacket.extrawildsymbol  = amaPacket.extraymbol;
                    initPacket.extracolcnt      = amaPacket.extrawin;
                    initPacket.extrawin         = amaPacket.extracolcnt;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = string.Format("{0}{1}", initString, ExtraAntePurString);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrawildsymbol);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extracolcnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrawin);
            
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BookOfAztecBonusBuyPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BookOfAztecBonusBuyPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BookOfAztecBonusBuyPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BookOfAztecBonusBuyPacket oldPacket = new BookOfAztecBonusBuyPacket(spinResult.ResultString, Cols, FreeCols);
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

            BookOfAztecBonusBuyPacket bookOfQueenPacket = packet as BookOfAztecBonusBuyPacket;
            
            if (packet is BookOfAztecBonusBuyPacket)
                bookOfQueenPacket = packet as BookOfAztecBonusBuyPacket;
            else
                bookOfQueenPacket = new BookOfAztecBonusBuyPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfQueenPacket.extraymbol);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfQueenPacket.extracolcnt);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfQueenPacket.extrawin);
            
            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            BookOfAztecBonusBuyPacket bookOfFortunePacket = packet as BookOfAztecBonusBuyPacket;
            
            base.convertWinsByBet(balance, bookOfFortunePacket, betInfo);
            bookOfFortunePacket.extrawin = convertWinByBet(bookOfFortunePacket.extrawin, betInfo);
        }

        public class BookOfAztecBonusBuyPacket : AmaticPacket
        {
            public long         extraymbol          { get; set; }
            public long         extracolcnt         { get; set; }
            public long         extrawin            { get; set; }

            public BookOfAztecBonusBuyPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extraymbol      = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                extracolcnt     = amaConverter.ReadLengthAndDec(message, point, out point);
                extrawin        = amaConverter.ReadLengthAndDec(message, point, out point);
                curpoint        = point;
            }

            public BookOfAztecBonusBuyPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extraymbol = 0;
                extracolcnt     = 0;
                extrawin        = 0;
                curpoint        = 2 + 2 + 2 + curpoint;
            }
        }

        public class BookOfAztecBonusBuyInitPacket : InitPacket
        {
            public long         extrawildsymbol { get; set; }
            public long         extracolcnt     { get; set; }
            public long         extrawin        { get; set; }

            public BookOfAztecBonusBuyInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt, int lines) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                extrawildsymbol = 0;
                extracolcnt     = 0;
                extrawin        = 0;
                curpoint        = 2 + 2 + 2 + curpoint;
            }
        }
    }
}
