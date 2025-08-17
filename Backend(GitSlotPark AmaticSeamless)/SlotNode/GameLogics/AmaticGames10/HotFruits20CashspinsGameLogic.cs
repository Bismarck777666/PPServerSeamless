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
    class HotFruits20CashspinsGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotFruits20Cashspins";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 20 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0a24d060606060404040404040404000000050505050505050303060603030404050504040101010101070505010105050606030303030303020202020204040505070202040403030202040401010624d030606020204040606010101050502020202050505050606060601010102020202030304040505070606060601010105050404020200000004040404040405050505050503030303030505070325006020206060202040406060606060606060404040404040101010101040403030303030303040407030300000002020202020505050505050303060603030606050501010303060605050701010505062500601010101010202020202020505050505050503030303030606020204040404040404020206060606060606010103030505040406060202030305050404010105050000000404060605050704040706250060606060602020404020202020205050202050503030303030307060603030404010106060303040401010101010404040404040400000005050101050505050505050504040606070505030305050624d0808080809080808080a080b0808080808080808080c0d080808080808080808080e080f100808080808080808080808110808080808080808080808081208080808080813141508080808080824d0808090808080808080a0808080808080808080b080c0808080808080d080808080e08080808080f1008080808080808080808111213080808080808080808080808080808140808150808080825008080908080808080808080a080b080c0808080808080808080808080d080808080e0808080808080808080808080f10080808111213080808080808080808140808080808080808150808080808080825008080808080808080908080a0808080808080808080b080c0808080808080808080808080d0e08080808080808080f080808100808081108080808080812131408080808080808081508080808080808250080808080808080809080808080a080808080808080b080808080c080808080d080808080808080808080808080e0f080808080808080810080808110812080808080808080813141508080808080808003010101010101010101010104271010002142641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a2641510101010101010101010101010101010101010101022800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a151010101010101010101010101010101010101d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d000000000000000000000000001f000000000000000000000000000000101010101010101010101010101010";
            }
        }
        protected string ExtraString => "1010101010101010101010101010101010101d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d0102030405080a0f14197d7e7f1d000000000000000000000000001f000000000000000000000000000000101010101010101010101010101010";
        protected override int ReelSetBitNum => 2;
        protected override int Cols => 10;
        protected string ExtraBonusReelSetString => "22800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a1522800060901050a02040b03000c04010d02050e06000f050110020411010612050702130c10140e0a15";
        #endregion

        public HotFruits20CashspinsGameLogic()
        {
            _gameID     = GAMEID.HotFruits20Cashspins;
            GameName    = "HotFruits20Cashspins";
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
