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
   
    class GemStarGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GemStar";
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
                return new long[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0523355566551141445430332255522446422545524454441560001523d5511066777333442220666555336664466440211335133366612345666000240555066677744404111333555226623334466604424446665551234566666666623d5665566633110444404333777225566601166644422666223456662000333239555114664444333506622033666015552211444055666233344455666523655566551141445430332255522446422545524454441560001577723d5511066777333442220666555336664466440211335133366612345666000240555066677744404111333555226623334466604424446665551234566666666623d566556663311044440433377722556660116664442266622345666200033323c5551146644443335066220336660155522114440556662333444556667770301010101010104271010001a33e8641010101010101064640a1100101010101000000000000000000a1112131415161718191a65101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010000101010101";
            }
        }
        protected override int LineTypeCnt => 10;
        #endregion

        public GemStarGameLogic()
        {
            _gameID     = GAMEID.GemStar;
            GameName    = "GemStar";
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
                case 50:
                    return 4;
                case 60:
                    return 5;
                case 70:
                    return 6;
                case 80:
                    return 7;
                case 90:
                    return 8;
                case 100:
                    return 9;
                default:
                    return 0;
            }
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            GemStarPacket gemstarPacket = packet as GemStarPacket;
            
            base.convertWinsByBet(balance, packet, betInfo);
            gemstarPacket.extrawin = convertWinByBet(gemstarPacket.extrawin, betInfo);
        }
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            GemStarInitPacket gemstarInitPacket = new GemStarInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    GemStarPacket amaPacket = new GemStarPacket(spinResult.ResultString, Cols, FreeCols);
                    gemstarInitPacket.extrawin = amaPacket.extrawin;
                    gemstarInitPacket.extracnt = amaPacket.extracnt;
                    gemstarInitPacket.extrastr = amaPacket.extrastr;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, gemstarInitPacket.extrawin);
            initString = encrypt.WriteDec2Hex(initString, gemstarInitPacket.extracnt);
            initString = encrypt.WriteLeftHexString(initString, gemstarInitPacket.extrastr);
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            GemStarPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new GemStarPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new GemStarPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    GemStarPacket oldPacket = new GemStarPacket(spinResult.ResultString, Cols, FreeCols);
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
                    packet.extracnt = oldPacket.extracnt;
                    packet.extrastr = oldPacket.extrastr;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            GemStarPacket gemstarPacket = null;
            if (packet is GemStarPacket)
                gemstarPacket = packet as GemStarPacket;
            else
                gemstarPacket = new GemStarPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, gemstarPacket.extrawin);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, gemstarPacket.extracnt);
            newSpinString = encrypt.WriteLeftHexString(newSpinString, gemstarPacket.extrastr);
            return newSpinString;
        }

        public class GemStarPacket : AmaticPacket
        {
            public long     extrawin    { get; set; }
            public long     extracnt    { get; set; }
            public string   extrastr    { get; set; }

            public GemStarPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extrawin = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                extracnt = amaConverter.Read2BitHexToDec(message, point, out point);
                extrastr = amaConverter.ReadLeftHexString(message, point, out point);
                curpoint = point;
            }

            public GemStarPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extrawin = 0;
                extracnt = 0;
                extrastr = "0101010101";
                curpoint = curpoint + 2 + 2 + extrastr.Length;
            }
        }
        public class GemStarInitPacket : InitPacket
        {
            public long     extrawin    { get; set; }
            public long     extracnt    { get; set; }
            public string   extrastr    { get; set; }

            public GemStarInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                extrawin = 0;
                extracnt = 0;
                extrastr = "0101010101";
                curpoint = curpoint + 2 + 2 + extrastr.Length;
            }
        }
    }
}
