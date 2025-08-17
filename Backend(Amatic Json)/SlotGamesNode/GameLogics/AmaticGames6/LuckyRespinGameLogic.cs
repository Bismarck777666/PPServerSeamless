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
   
    class LuckyRespinGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyRespin";
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
                return "0523355566551141445430332255522446422545524454441560001523d5511066777333442220666555336664466440211335133366612345666000240555066677744404111333555226623334466604424446665551234566666666623d5665566633110444404333777225566601166644422666223456662000333239555114664444333506622033666015552211444055666233344455666523655566551141445430332255522446422545524454441560001577723d5511066777333442220666555336664466440211335133366612345666000240555066677744404111333555226623334466604424446665551234566666666623d566556663311044440433377722556660116664442266622345666200033323c5551146644443335066220336660155522114440556662333444556667770301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b101010101010101010101010001500000";
            }
        }
        #endregion

        public LuckyRespinGameLogic()
        {
            _gameID     = GAMEID.LuckyRespin;
            GameName    = "LuckyRespin";
        }

        protected override string buildInitString(string strUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strUserID, balance, currency);
            AmaticRespinInitPacket respinInitPacket = new AmaticRespinInitPacket(initString, Cols, FreeCols, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];
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

        protected override string buildResMsgString(string strUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
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

                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

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
                respintype  = "001500000";
                curpoint    = curpoint + 2 + respintype.Length;
            }
        }

        public class AmaticRespinInitPacket : InitPacket
        {
            public long     accwin      { get; set; }
            public string   respintype  { get; set; }

            public AmaticRespinInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetBitCnt)
            {
                accwin      = 0;
                respintype  = "001500000";
                curpoint    = curpoint + 2 + respintype.Length;
            }
        }
    }
}
