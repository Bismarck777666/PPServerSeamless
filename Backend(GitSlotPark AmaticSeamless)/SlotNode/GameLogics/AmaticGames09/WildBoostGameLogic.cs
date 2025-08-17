using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WildBoostGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected string ExtraString => "214ffffffffffffffffffff";
        protected override string SymbolName
        {
            get
            {
                return "WildBoost";
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
                return new long[] { 10, 20, 30, 40 };
            }
        }
        protected override int LineTypeCnt => 4;
        protected override string InitString
        {
            get
            {
                return "0142782222000011119999777744445555333366668888009911001441223355229966553377448866778809110914102723635420796595336587424868782780000222266661111444499998888333355557777009911001199664422335522335577448866778809110914102723635659534207983658742468782789999000044442222111177778888333355556666009911004411223322559966883355774466778809110914276592361035420795338746524868782786666000022227777111133334444555588889999009911004411228866335522996655337744778809110914103696554207953364248632725878782781111000099992222888833337777444455556666009911004433662233551177445522998866778809110936562010274653541798795342234868783137989992221114446665553337770008887999222111444666555333777000888699922211144466655533377700088859992221114446665553337770008884999222111444666555333777000888399922211144466655533377700088899992221114446665553337770008882999222111444666555333777000888199922211144466655533377700088a099922211144466655533377700088810101010101010101010101010100030101010101010101010101010101010101010101010427101000115186a028101010101010100a280a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d02910101010101010101010101010101010101010101010101010101010101010101010101010101010101000214ffffffffffffffffffff";
            }
        }
        protected override int Cols => 20;
        protected override int ReelSetColBitNum => 2;
        #endregion

        public WildBoostGameLogic()
        {
            _gameID     = GAMEID.WildBoost;
            GameName    = "WildBoost";
        }

        protected override int getLineTypeFromPlayLine(int playline)
        {
            switch (playline)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                case 30:
                    return 2;
                case 40:
                    return 3;
                default:
                    return 0;
            }
        }
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            WildBoostInitPacket initPacket = new WildBoostInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum, ExtraString);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    WildBoostPacket amaPacket = new WildBoostPacket(spinResult.ResultString, Cols, FreeCols);

                    initPacket.extrawin     = amaPacket.extrawin;
                    initPacket.respinnum    = amaPacket.respinnum;
                    initPacket.respinmask   = amaPacket.respinmask;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrawin);
            initString = encrypt.WriteDec2Hex(initString, initPacket.respinnum);
            initString = encrypt.WriteLeftHexString(initString, initPacket.respinmask);
            
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            WildBoostPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new WildBoostPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new WildBoostPacket(Cols, FreeCols, (int)type, (int)LINES.Last(), ExtraString);

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    WildBoostPacket oldPacket = new WildBoostPacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extrawin     = oldPacket.extrawin;
                    packet.respinnum    = oldPacket.respinnum;
                    packet.respinmask   = oldPacket.respinmask;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            WildBoostPacket wildBoostPacket = null;
            if (packet is WildBoostPacket)
                wildBoostPacket = packet as WildBoostPacket;
            else
                wildBoostPacket = new WildBoostPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last(), ExtraString);

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, wildBoostPacket.extrawin);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, wildBoostPacket.respinnum);
            newSpinString = encrypt.WriteLeftHexString(newSpinString, wildBoostPacket.respinmask);
            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            WildBoostPacket wildBoostPacket = packet as WildBoostPacket;

            base.convertWinsByBet(balance, wildBoostPacket, betInfo);
            wildBoostPacket.extrawin = convertWinByBet(wildBoostPacket.extrawin, betInfo);
        }
        public class WildBoostPacket : AmaticPacket
        {
            public long     extrawin    { get; set; }
            public long     respinnum   { get; set; }
            public string   respinmask  { get; set; }

            public WildBoostPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extrawin    = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                respinnum   = amaConverter.Read2BitHexToDec(message, point, out point);
                respinmask  = amaConverter.ReadLeftHexString(message, point, out point);
                curpoint    = point;
            }

            public WildBoostPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt, string maskStr) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extrawin    = 0;
                respinnum   = 0;
                respinmask  = maskStr;
                curpoint    = 2 + 2 + respinmask.Length + curpoint;
            }
        }

        public class WildBoostInitPacket : InitPacket
        {
            public long     extrawin    { get; set; }
            public long     respinnum   { get; set; }
            public string   respinmask  { get; set; }

            public WildBoostInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt, string maskStr) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                extrawin    = 0;
                respinnum   = 0;
                respinmask  = maskStr;
                curpoint    = 2 + 2 + respinmask.Length + curpoint;
            }
        }
    }
}
