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
   
    class GrandXGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GrandX";
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
                return "0524577248481595596363217171824242339669559537668787284141464272353530181822e50768242885566554778783380617872465667171553682446729881717668171735363272298848451492656068753535344323729814141456522c2727343455608681356560787812155067647087886522964869525621716973785795380546567879837213516012345160123451601234516012345160123450001010101010104c328100121437d00a101010111110100a0a0a11001010101010000000000000000021121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f4042328427100b10101010101010101010101010";
            }
        }

        protected override int LineTypeCnt
        {
            get
            {
                return 1;
            }
        }
        #endregion

        public GrandXGameLogic()
        {
            _gameID     = GAMEID.GrandX;
            GameName    = "GrandX";
        }

        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            GrandXInitPacket grandInitPacket = new GrandXInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    GrandXPacket amaPacket = new GrandXPacket(spinResult.ResultString, Cols, FreeCols);
                    grandInitPacket.extrawin        = amaPacket.extrawin;
                    grandInitPacket.extramultiple   = amaPacket.extramultiple;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, grandInitPacket.extrawin);
            initString = encrypt.WriteLengthAndDec(initString, grandInitPacket.extramultiple);

            return initString;
        }

        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            GrandXPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new GrandXPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new GrandXPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    GrandXPacket oldPacket = new GrandXPacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extrawin         = oldPacket.extrawin;
                    packet.extramultiple    = oldPacket.extramultiple;
                }
            }

            return buildSpinString(packet);
        }

        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);
            
            GrandXPacket grandXPacket = null;
            if (packet is GrandXPacket)
                grandXPacket = packet as GrandXPacket;
            else
                grandXPacket = new GrandXPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, grandXPacket.extrawin);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, grandXPacket.extramultiple);
            return newSpinString;
        }

        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            GrandXPacket grandPacket = packet as GrandXPacket;
            
            base.convertWinsByBet(balance, grandPacket, betInfo);
            grandPacket.extrawin = convertWinByBet(grandPacket.extrawin, betInfo);
        }

        public class GrandXPacket : AmaticPacket
        {
            public long extrawin        { get; set; }
            public long extramultiple   { get; set; }

            public GrandXPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extrawin        = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                extramultiple   = amaConverter.ReadLengthAndDec(message, point, out point);
                curpoint        = point;
            }

            public GrandXPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extrawin        = 0;
                extramultiple   = 0;
                curpoint        = curpoint + 2 + 2;
            }
        }

        public class GrandXInitPacket : InitPacket
        {
            public long extrawin        { get; set; }
            public long extramultiple   { get; set; }

            public GrandXInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                extrawin        = 0;
                extramultiple   = 0;
                curpoint        = curpoint + 2 + 2;
            }
        }
    }
}
