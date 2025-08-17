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
    class LuckyPiggiesGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyPiggies";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 80, 100, 120, 140, 160, 180, 200, 240, 280, 320, 360, 400, 600, 800, 1000, 1200, 1400, 1600, 1800, 2000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 5 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0528306060105050305050306080802050504070701050504070704040808070303060b0105050606020605050806060305050c060603060d05050e070303060f07070406060308080707060603100505110806060312060605050506060206000505000102030300050500050501020303030505020505030306060613080808140707071529d070802040103080508060600070308070205030807080804050108080406060308080406060607030808040701080905070908080709050808040806060907020703080907030809070509080208080509080608030805070808020401080702000307050504080107030805000808010803070508040103030308050106030709050505040806000308050504080604060105030805070707080808092890808070b06020505050800070606010808040305070707080606020807050806070607070c08060708080705080d05060606080208070705050e0800070706070508050606010f080207100606020706020304040206060008020505020607000207070207070502070802060702071105050208060203070402061207000207130807081406071508101000301010101010104271010001131f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c80610101010101021a0a0b0a0a0a0c0a0a0a0d0a0e0a0f0a0a0a0a100a110a120a0a1321a0a110a0a0a0e0a0a0a0d0a0a0b0a100a0a0a0f0a120a0c0a0a1321a0a0d0a0a100a0a0a0a0c0a0e0a0b0a0a0a0a130a110a120a0a0f21a0a0a090a0a0a0a0a090a0a0a0a090a0a0a0a0a0a0a0a0a090a0a21a0a0a0a090a0a0a090a0a0a0a0a090a0a0a0a0a0a0a0a0a090a0a21a0a0a0a0a0a090a0a0a0a0a0a090a0a0a0a0a0a0a0a0a090a0a0921a0a0c0a0a0a0b0a0a0a0d0a110a0f0a0a0a0a130a120a100a0a0e21a0a0d0a0a0a120a0a0a100a0e0a110a0b0a0a0c0a0a0a0f0a130a21a0a120a0a0a0c0a0a0a100a0b0a0f0a0a0d0a110a0a0a0e0a0a13101010101010101010101010101b020a0a0a020101050a0a021b0a0a0a05020f05010201051b0f050f0a0a0a020f0f050f190102050a0f7c7d7e7f101010101010101010101010101010130000001300000013000000";
            }
        }
        protected string ExtraString => "101010101010101010101010101b020a0a0a020101050a0a021b0a0a0a05020f05010201051b0f050f0a0a0a020f0f050f190102050a0f7c7d7e7f101010101010101010101010101010130000001300000013000000";
        protected override int ReelSetBitNum => 2;
        protected string ExtraReelsetString => "21a0a0b0a0a0a0c0a0a0a0d0a0e0a0f0a0a0a0a100a110a120a0a1321a0a110a0a0a0e0a0a0a0d0a0a0b0a100a0a0a0f0a120a0c0a0a1321a0a0d0a0a100a0a0a0a0c0a0e0a0b0a0a0a0a130a110a120a0a0f21a0a0a090a0a0a0a0a090a0a0a0a090a0a0a0a0a0a0a0a0a090a0a21a0a0a0a090a0a0a090a0a0a0a0a090a0a0a0a0a0a0a0a0a090a0a21a0a0a0a0a0a090a0a0a0a0a0a090a0a0a0a0a0a0a0a0a090a0a0921a0a0c0a0a0a0b0a0a0a0d0a110a0f0a0a0a0a130a120a100a0a0e21a0a0d0a0a0a120a0a0a100a0e0a110a0b0a0a0c0a0a0a0f0a130a21a0a120a0a0a0c0a0a0a100a0b0a0f0a0a0d0a110a0a0a0e0a0a13";
        #endregion

        public LuckyPiggiesGameLogic()
        {
            _gameID     = GAMEID.LuckyPiggies;
            GameName    = "LuckyPiggies";
        }
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BaseAmaticExtra21InitPacket extraInitPacket = new BaseAmaticExtra21InitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum, ExtraString);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                
                BaseAmaticExtra21Packet amaPacket = new BaseAmaticExtra21Packet(spinResult.ResultString, Cols, FreeCols);
                extraInitPacket.extrastr = amaPacket.extrastr;
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLeftHexString(initString, ExtraReelsetString + extraInitPacket.extrastr);
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BaseAmaticExtra21Packet packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BaseAmaticExtra21Packet(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BaseAmaticExtra21Packet(Cols, FreeCols, (int)type, (int)LINES.Last(), ExtraString);

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BaseAmaticExtra21Packet oldPacket = new BaseAmaticExtra21Packet(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extrastr = oldPacket.extrastr;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            BaseAmaticExtra21Packet extraWildPacket = null;
            if (packet is BaseAmaticExtra21Packet)
                extraWildPacket = packet as BaseAmaticExtra21Packet;
            else
                extraWildPacket = new BaseAmaticExtra21Packet(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last(), ExtraString);

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLeftHexString(newSpinString, extraWildPacket.extrastr);
            return newSpinString;
        }
    }
}
