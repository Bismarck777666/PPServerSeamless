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
    class LuckySharkGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyShark";
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
                return new long[] { 25 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0523c0208050000000805060305070a0b0c0507040706080701070503070809070806070d0e060708030507040208070605070f0507030805090805040807231060503080a030807050600000005060407060907060405080705040b0c0d0504080507090507020306080e0f06080706012280605020605080a0506030108030b0c06050104030608000000020401080609080206070d0e0f07062310706030a06070203070502070501040b0c0d0203060501020304050609050803010402080503040e0f0607010607000000231080905040a030708040b0c0d080703040203040108060209060801050602050603040000000205080e0f0805010407060352480108050000000706050708050307060305040105080205030700000006030402000000070502060302040702050802070603050802070800000004080207080305070207080105062470a0204010207040b0807090602040708060c08030607050408060203050d08040307080006090503070006040805010702040307020509060703060e04000803060f050107040310102520502040605080304050103080703040207040106070205070106070304060105070803050208060304070206080304020705030600000008070106080203050703040801060403020804030508020506030803010101010101042710100021931f419101010101010101919191100101010101000000000000000000811121314151a1f2141a101010101010101010101010101010101010101010101010101023a0f010401100502060c0507060d0108080e02030117050406140108071101120204130405060b0801071a051601020a040305180719060215010823a0e010401180205060c0506070b0801080a010203100405060f07080111011a020413050406140108070d0501160217030405150607021908120123a0f0104010b080108110506070a0205060e0102030c080401100708010d0102021604060513011a0807150401021203140405180602021904170623a0a050101180801080c0606070d0204030b010206100401081907080113010202120101051101081407150504021a031703060e0607020f04160623b0b0103010d0502080c050607140801080e0102030f040506100708011704020112050413060a010807150504021605040318061a0702190804110123a0f0104010b0801080a0506070d0205060e0102030c08040119070801110112020215040605140108071702050216031304051a0607021804100623a0a0104010e05020615050706140108080b02030112050406100108070f01020411040506130801071a050c010216040317051807060219010d0823a0f0104010b0205061005050612080108180106060a0405061a07080111010d020413050406180115080716050102170304051406070219080c0123a0a0104011908010810050607130205060c0102030f0804010e071108010b01020215040605140108070d01160502170304061205020219041a0623a0a0504010b080108100305060e020103110102060f040108160708010d011202021301170105150108070c050402190303061807050214041a0623a0a010301140502081a0506070d0801080e0102030c040506100708011804120201130504060b0108071505041602110504031706070219080f0123a1a070401110801080c050607170205060e0102060b08040110070801150201020f040605130108071405011202160304040d0418070219040a0623a0e0106010d0502060c05070611010808100203010f0504060a0108070b04120201130405061508011707160504021a040305180702061401190823a0c0104010b020506160506070e080108130106031a0405061007150801120102040d05040615011408070c020502170304051806070219080f0123a0e010501120801080c0605070d0205060a060203170804011a0708010b03110202180406051601150807140504020f0304021306070719041006101010101010101010101010101010101010160102030405061601020304050616010203040506160102030405061601020304050621100000000000000000000000000000000001f000000000000000000000000000000101010101010101010101010101010";
            }
        }
        protected override int ReelSetBitNum => 2;
        protected string ExtraString => "101010101010101010101010101010101010160102030405061601020304050616010203040506160102030405061601020304050621100000000000000000000000000000000001f000000000000000000000000000000101010101010101010101010101010";
        protected string ExtraReelsetString => "23a0f010401100502060c0507060d0108080e02030117050406140108071101120204130405060b0801071a051601020a040305180719060215010823a0e010401180205060c0506070b0801080a010203100405060f07080111011a020413050406140108070d0501160217030405150607021908120123a0f0104010b080108110506070a0205060e0102030c080401100708010d0102021604060513011a0807150401021203140405180602021904170623a0a050101180801080c0606070d0204030b010206100401081907080113010202120101051101081407150504021a031703060e0607020f04160623b0b0103010d0502080c050607140801080e0102030f040506100708011704020112050413060a010807150504021605040318061a0702190804110123a0f0104010b0801080a0506070d0205060e0102030c08040119070801110112020215040605140108071702050216031304051a0607021804100623a0a0104010e05020615050706140108080b02030112050406100108070f01020411040506130801071a050c010216040317051807060219010d0823a0f0104010b0205061005050612080108180106060a0405061a07080111010d020413050406180115080716050102170304051406070219080c0123a0a0104011908010810050607130205060c0102030f0804010e071108010b01020215040605140108070d01160502170304061205020219041a0623a0a0504010b080108100305060e020103110102060f040108160708010d011202021301170105150108070c050402190303061807050214041a0623a0a010301140502081a0506070d0801080e0102030c040506100708011804120201130504060b0108071505041602110504031706070219080f0123a1a070401110801080c050607170205060e0102060b08040110070801150201020f040605130108071405011202160304040d0418070219040a0623a0e0106010d0502060c05070611010808100203010f0504060a0108070b04120201130405061508011707160504021a040305180702061401190823a0c0104010b020506160506070e080108130106031a0405061007150801120102040d05040615011408070c020502170304051806070219080f0123a0e010501120801080c0605070d0205060a060203170804011a0708010b03110202180406051601150807140504020f0304021306070719041006";
        #endregion

        public LuckySharkGameLogic()
        {
            _gameID     = GAMEID.LuckyShark;
            GameName    = "LuckyShark";
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
