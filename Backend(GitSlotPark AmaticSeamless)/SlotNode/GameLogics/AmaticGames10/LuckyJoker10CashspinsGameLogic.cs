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
    class LuckyJoker10CashspinsGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyJoker10Cashspins";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 2000 };
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
                return "0a26d0101070707040404050505030303020606060905050506010002050606060707070004040408030303060606070707000404040806060607070700040404080606060707070404040505050303030206060609050505030303020606060905050507070704040405050501010126806060604040405050506060600020202060606060004040405050506060606070a060707070404040803030307070703010a03010505050002020206060607070703010a030105050500040404080303030007070703010a030104040407070700050505080303032680702050505090202020505050904040401010107020a07020901010107070701010106060604040407020a070203030308060606040404070a04070707010101080606060404040002020208060606050505030303020202000101010002020205050507020a0702268020206020a06020404040707070005050506020a0602010101080606060404040707070101010707070303030404040007070708060606050a060505050303030006060605050506020a06020303030101010806060605050504040400070707000101010002020227f0805050503030303090606060101010404040003030304040405050501010103030300070707050505000202020001010103030300070707080404040707070804040402020200010101000606060005050506020301050606060907070700010101060606020202050505020202040404090606060202020707070303030326d0b0b0b0c0b0b0b0b0d0b0e0b0b0b0b0b0b0b0b0b0b0b0f100b0b0b0b0b0b0b0b0b0b0b0b110b12130b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b140b0b0b0b0b0b0b0b0b0b150b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b1617180b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b2680b0b0c0b0b0b0b0b0b0b0b0b0b0b0d0b0b0b0b0b0b0e0b0f0b0b0b0b0b0b0b0b0b0b0b0b0b100b0b0b0b0b110b0b0b0b0b12130b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b1415160b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b170b0b180b0b0b0b0b2680b0b0b0c0b0b0b0b0b0b0b0b0b0b0b0b0b0d0b0e0b0f0b0b0b0b0b0b0b0b0b0b0b0b0b0b100b0b0b0b0b110b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b12130b0b0b0b0b0b1415160b0b0b0b0b0b0b0b0b0b0b0b170b0b0b0b0b0b0b0b180b0b0b0b0b0b2680b0b0b0b0b0c0b0b0d0b0b0b0b0b0b0b0b0b0b0b0e0b0f0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b10110b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b120b0b0b130b0b0b140b0b0b0b0b0b0b0b0b0b0b0b0b0b0b1516170b0b0b0b0b0b0b0b0b0b0b0b180b0b0b0b0b0b0b0b27f0b0b0b0b0b0b0b0b0b0c0b0b0b0b0d0b0b0b0b0b0b0e0b0b0b0b0f0b0b0b0b100b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b11120b0b0b0b0b0b0b0b0b0b0b0b130b0b0b0b0b0b0b0b0b0b140b150b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b1617180b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b003010101010101010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b101010101010101010101022800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d181010101010101010101010101010101010101d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d000000000000000000000000001f00000000000000000000000000000010101010101010101010101010101001";
            }
        }
        protected string ExtraString => "1010101010101010101010101010101010101d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d000000000000000000000000001f00000000000000000000000000000010101010101010101010101010101001";
        protected override int ReelSetBitNum => 2;
        protected override int Cols => 10;
        protected string ExtraBonusReelSetString => "22800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d1822800070c02060d03050e04000f050210030611070012060213030514020715060803160f1317110d18";
        #endregion

        public LuckyJoker10CashspinsGameLogic()
        {
            _gameID     = GAMEID.LuckyJoker10Cashspins;
            GameName    = "LuckyJoker10Cashspins";
        }
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BaseAmaticExtra21InitPacket extraInitPacket = new BaseAmaticExtra21InitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum, ExtraString);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                
                BaseAmaticExtra21Packet     amaPacket   = new BaseAmaticExtra21Packet(spinResult.ResultString, Cols, FreeCols);
                extraInitPacket.extrastr = amaPacket.extrastr;
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLeftHexString(initString, ExtraBonusReelSetString + extraInitPacket.extrastr);
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
