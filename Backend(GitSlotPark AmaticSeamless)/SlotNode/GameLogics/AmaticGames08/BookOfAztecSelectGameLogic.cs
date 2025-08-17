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

namespace SlotGamesNode.GameLogics
{
    class BookOfAztecSelectGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BookOfAztecSelect";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 40, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "052326054265265347246574562480468947156357268475618527422d71547850678537856857854735837542783751687694822e48367826425407817628437648376874985146186078152322652843786257086596436815347651876284679850461425422e1607825683516248967145879570452634836425726437522594715741689570543672542857165436471682254583795648524768174376948625860746738224451640680574985718625762753647587408225347807495836798563516704625825638526822595714287951642740782405381625361806380301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010000102030405060708101010";
            }
        }
        #endregion

        public BookOfAztecSelectGameLogic()
        {
            _gameID     = GAMEID.BookOfAztecSelect;
            GameName    = "BookOfAztecSelect";
        }

        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_FSOPTION)
                await onDoSpin(strUserID, websiteID, message, userBalance,currency);
            else
                await base.onProcMessage(strUserID, websiteID, message, userBalance, currency);
        }
        protected override async Task onDoSpin(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_FSOPTION)
                {
                    int selectOption = (int)message.Pop();
                    if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) || selectOption < 0)
                    {
                        _logger.Error("{0} option select error in BookOfAztecSelectGameLogic::onDoSpin", strGlobalUserID);
                    }
                    
                    BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    preprocessResponseList(betInfo.RemainReponses, selectOption);
                }
                await base.onDoSpin(strUserID, websiteID, message, userBalance, currency);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BookOfAztecSelectGameLogic::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected List<BaseAmaticActionToResponse> preprocessResponseList(List<BaseAmaticActionToResponse> actionResponseList, int selectOption)
        {
            for (int i = 0; i < actionResponseList.Count; i++)
            {
                BookOfAztecSelectPacket packet = new BookOfAztecSelectPacket(actionResponseList[i].Response, Cols, FreeCols);
                
                int oldPickIndex = (int)packet.extraymbol;
                
                long buff                               = packet.picksymbols[oldPickIndex];
                packet.picksymbols[(int)oldPickIndex]   = packet.picksymbols[selectOption];
                packet.picksymbols[selectOption]        = buff;

                packet.extraymbol = selectOption;

                AmaticMessageType action = convertRespMsgCodeToAction(packet.messagetype);

                actionResponseList[i] = new BaseAmaticActionToResponse(action, buildSpinString(packet));
            }
            return actionResponseList;
        }
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BookOfAztecSelectInitPacket initPacket = new BookOfAztecSelectInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    BookOfAztecSelectPacket amaPacket = new BookOfAztecSelectPacket(spinResult.ResultString, Cols, FreeCols);

                    initPacket.picksymbols = new List<long>();
                    for(int i = 0; i < amaPacket.picksymbols.Count; i++)
                    {
                        initPacket.picksymbols.Add(amaPacket.picksymbols[i]);
                    }
                    initPacket.extraymbol       = amaPacket.extraymbol;
                    initPacket.extracolcnt      = amaPacket.extrawin;
                    initPacket.extrawin         = amaPacket.extracolcnt;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            for (int i = 0; i < initPacket.picksymbols.Count; i++)
            {
                initString = encrypt.WriteDec2Hex(initString, initPacket.picksymbols[i]);
            }
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extraymbol);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extracolcnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrawin);
            
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BookOfAztecSelectPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BookOfAztecSelectPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BookOfAztecSelectPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BookOfAztecSelectPacket oldPacket = new BookOfAztecSelectPacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.picksymbols = new List<long>();
                    for(int i = 0; i < oldPacket.picksymbols.Count; i++)
                    {
                        packet.picksymbols.Add(oldPacket.picksymbols[i]);
                    }
                    packet.extraymbol       = oldPacket.extraymbol;
                    packet.extracolcnt      = oldPacket.extracolcnt;
                    packet.extrawin         = oldPacket.extrawin;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            BookOfAztecSelectPacket bookOfAztecSelectPacket = packet as BookOfAztecSelectPacket;
            
            if (packet is BookOfAztecSelectPacket)
                bookOfAztecSelectPacket = packet as BookOfAztecSelectPacket;
            else
                bookOfAztecSelectPacket = new BookOfAztecSelectPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();

            for(int i = 0; i < bookOfAztecSelectPacket.picksymbols.Count; i++)
            {
                newSpinString = encrypt.WriteDec2Hex(newSpinString, bookOfAztecSelectPacket.picksymbols[i]);
            }
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfAztecSelectPacket.extraymbol);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfAztecSelectPacket.extracolcnt);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, bookOfAztecSelectPacket.extrawin);
            
            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            BookOfAztecSelectPacket bookOfAztecSelectPacket = packet as BookOfAztecSelectPacket;
            
            base.convertWinsByBet(balance, bookOfAztecSelectPacket, betInfo);
            bookOfAztecSelectPacket.extrawin = convertWinByBet(bookOfAztecSelectPacket.extrawin, betInfo);
        }

        public class BookOfAztecSelectPacket : AmaticPacket
        {
            public List<long>   picksymbols         { get; set; }
            public long         extraymbol          { get; set; }
            public long         extracolcnt         { get; set; }
            public long         extrawin            { get; set; }

            public BookOfAztecSelectPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = curpoint;
                picksymbols = new List<long>();
                for(int i = 0; i < 9; i++)
                {
                    long symbol = amaConverter.Read2BitHexToDec(message, point, out point);
                    picksymbols.Add(symbol);
                }
                extraymbol      = amaConverter.ReadLengthAndDec(message, point, out point);
                extracolcnt     = amaConverter.ReadLengthAndDec(message, point, out point);
                extrawin        = amaConverter.ReadLengthAndDec(message, point, out point);
                curpoint        = point;
            }

            public BookOfAztecSelectPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                picksymbols = new List<long>();
                for (int i = 0; i < 9; i++)
                {
                    picksymbols.Add(i);
                }
                extraymbol      = 0;
                extracolcnt     = 0;
                extrawin        = 0;
                curpoint        = picksymbols.Count * 2 + 2 + 2 + 2 + curpoint;
            }
        }

        public class BookOfAztecSelectInitPacket : InitPacket
        {
            public List<long>   picksymbols     { get; set; }
            public long         extraymbol      { get; set; }
            public long         extracolcnt     { get; set; }
            public long         extrawin        { get; set; }

            public BookOfAztecSelectInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {

                picksymbols = new List<long>();
                for (int i = 0; i < 9; i++)
                {
                    picksymbols.Add(i);
                }
                extraymbol      = 0;
                extracolcnt     = 0;
                extrawin        = 0;
                curpoint        = picksymbols.Count * 2 + 2 + 2 + 2 + curpoint;
            }
        }
    }
}
