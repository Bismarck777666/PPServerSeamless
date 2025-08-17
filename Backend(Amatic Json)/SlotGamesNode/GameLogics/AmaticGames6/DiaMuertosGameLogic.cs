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
    public class DiaMuertosBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet    => 1;
    }

    class DiaMuertosGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DiaMuertos";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 11 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "05237122223811112455555a444489933337866666a97778a9879aa0000024b11123471400b2458b666577357aa878899899aa9ab22999993366b4477777556888885aaaaa24d111788888222b000b3333419578977a9999978948a8a99b4444a68a77777652aaaaa6666b555525aa3211111b2225445676a7878666665389999933aaa888777555559a444440000022222333337777788888aaaaa25aa45411111939524676a687587938aaaaa9988776655444443322222000009999988888666667777755555333335237122223811112455555a444489933337866666a97778a9879aa0000024b11123471400b2458b666577357aa878899899aa9ab22999993366b4477777556888885aaaaa24d111788888222b000b3333419578977a9999978948a8a99b4444a68a77777652aaaaa6666b555525aa3211111b2225445676a7878666665389999933aaa888777555559a444440000022222333337777788888aaaaa25aa45411111939524676a687587938aaaaa9988776655444443322222000009999988888666667777755555333330301010101010104271010001a33e80b101010101010100b0b0b110010101010100000000000000000131a21421e2282322642962c82fa312c315e319031c231f4325832bc3320338433e80b10101010101010101010101000";
            }
        }
        #endregion

        public DiaMuertosGameLogic()
        {
            _gameID     = GAMEID.DiaMuertos;
            GameName    = "DiaMuertos";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                DiaMuertosBetInfo betInfo   = new DiaMuertosBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in DiaMuertosGameLogic::readBetInfoFromMessage", strUserID);
                    return;
                }

                BaseAmaticSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.PlayLine     = betInfo.PlayLine;
                    oldBetInfo.PlayBet      = betInfo.PlayBet;
                    oldBetInfo.PurchaseStep = betInfo.PurchaseStep;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.CurrencyInfo = betInfo.CurrencyInfo;
                    oldBetInfo.GambleType   = betInfo.GambleType;
                    oldBetInfo.GambleHalf   = betInfo.GambleHalf;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DiaMuertosGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            DiaMuertosBetInfo betInfo = new DiaMuertosBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }

        protected override string buildInitString(string strUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strUserID, balance, currency);
            DiaMuertosInitPacket respinInitPacket = new DiaMuertosInitPacket(initString, Cols, FreeCols, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];
                if (betInfo.HasRemainResponse)
                {
                    DiaMuertosPacket amaPacket = new DiaMuertosPacket(spinResult.ResultString, Cols, FreeCols);
                    respinInitPacket.accwin     = amaPacket.accwin;
                    respinInitPacket.respintype = amaPacket.respintype;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, respinInitPacket.accwin);
            initString = encrypt.WriteLeftHexString(initString, respinInitPacket.respintype);

            return initString;
        }

        protected override string buildResMsgString(string strUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            DiaMuertosPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new DiaMuertosPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new DiaMuertosPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

                    DiaMuertosPacket oldPacket = new DiaMuertosPacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.accwin       = oldPacket.accwin;
                    packet.respintype   = oldPacket.respintype;
                }
            }

            return buildSpinString(packet);
        }

        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            DiaMuertosPacket respinPacket = null;
            if (packet is DiaMuertosPacket)
                respinPacket = packet as DiaMuertosPacket;
            else
                respinPacket = new DiaMuertosPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, respinPacket.accwin);
            newSpinString = encrypt.WriteLeftHexString(newSpinString, respinPacket.respintype);
            return newSpinString;
        }

        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            DiaMuertosPacket respinPacket = packet as DiaMuertosPacket;

            base.convertWinsByBet(balance, respinPacket, betInfo);
            respinPacket.accwin = convertWinByBet(respinPacket.accwin, betInfo);
        }

        public class DiaMuertosPacket : AmaticPacket
        {
            public long     accwin      { get; set; }
            public string   respintype  { get; set; }

            public DiaMuertosPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                accwin      = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                respintype  = amaConverter.ReadLeftHexString(message, point, out point);
                curpoint    = point;
            }

            public DiaMuertosPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                accwin      = 0;
                respintype  = "00";
                curpoint    = curpoint + 2 + respintype.Length;
            }
        }

        public class DiaMuertosInitPacket : InitPacket
        {
            public long     accwin      { get; set; }
            public string   respintype  { get; set; }

            public DiaMuertosInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetBitCnt)
            {
                accwin      = 0;
                respintype  = "00";
                curpoint    = curpoint + 2 + respintype.Length;
            }
        }
    }
}
