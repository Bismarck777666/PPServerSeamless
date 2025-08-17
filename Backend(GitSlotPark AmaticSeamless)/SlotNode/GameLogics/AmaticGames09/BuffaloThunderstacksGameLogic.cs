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
   
    class BuffaloThunderstacksGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BuffaloThunderstacks";
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
                return "05263446666444444000055555533663344554411111551155663333332222299999447755228888844330022441167777708899266622446611115522225555666611222233445566661115544220000444444888885555553333399999553367777700778888099267622662244666666664444441111144333333344330000222225555553366336655117777733665577556888889999900889901126b611111222222555555533333662244444442266666661133554466223355441188888550000446655446777779999900887709977882696666622442222255225533333366334411663344111114444444000055119999955005555557777744665503355688888777788880030101010101010427101000115186a028101010101010100a280a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010100015fffff15fffff15fffff15fffff";
            }
        }
        #endregion

        public BuffaloThunderstacksGameLogic()
        {
            _gameID     = GAMEID.BuffaloThunderstacks;
            GameName    = "BuffaloThunderstacks";
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
            AmaticRespinInitPacket respinInitPacket = new AmaticRespinInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    AmaticRespinPacket amaPacket = new AmaticRespinPacket(spinResult.ResultString, Cols, FreeCols);
                    respinInitPacket.accwin     = amaPacket.accwin;
                    respinInitPacket.respintype = amaPacket.respintype;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, respinInitPacket.accwin);
            initString = encrypt.WriteLeftHexString(initString, respinInitPacket.respintype);

            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            AmaticRespinPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new AmaticRespinPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new AmaticRespinPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    AmaticRespinPacket oldPacket = new AmaticRespinPacket(spinResult.ResultString, Cols, FreeCols);
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

            AmaticRespinPacket respinPacket = null;
            if (packet is AmaticRespinPacket)
                respinPacket = packet as AmaticRespinPacket;
            else
                respinPacket = new AmaticRespinPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, respinPacket.accwin);
            newSpinString = encrypt.WriteLeftHexString(newSpinString, respinPacket.respintype);
            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            AmaticRespinPacket respinPacket = packet as AmaticRespinPacket;

            base.convertWinsByBet(balance, respinPacket, betInfo);
            respinPacket.accwin = convertWinByBet(respinPacket.accwin, betInfo);
        }

        public class AmaticRespinPacket : AmaticPacket
        {
            public long     accwin      { get; set; }
            public string   respintype  { get; set; }

            public AmaticRespinPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                accwin      = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                respintype  = amaConverter.ReadLeftHexString(message, point, out point);
                curpoint    = point;
            }

            public AmaticRespinPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                accwin      = 0;
                respintype  = "0015fffff15fffff15fffff15fffff";
                curpoint    = curpoint + 2 + respintype.Length;
            }
        }

        public class AmaticRespinInitPacket : InitPacket
        {
            public long     accwin      { get; set; }
            public string   respintype  { get; set; }

            public AmaticRespinInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                accwin      = 0;
                respintype  = "0015fffff15fffff15fffff15fffff";
                curpoint    = curpoint + 2 + respintype.Length;
            }
        }
    }
}
