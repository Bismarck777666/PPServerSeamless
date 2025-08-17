using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DiamondStaxxGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected string ExtraString => "13fff13fff13fff13fff13fff";
        protected override string SymbolName
        {
            get
            {
                return "DiamondStaxx";
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
                return "0525752798542763576508498347159146a74065a74168573652798542763576508498347159146a74065a741636259589528645289375906748257643975618756738658167589528645289375906748257643975618706837681672729a87387346a78194278146386276296a79378150481428514604615279a87387346a78194278146386276296a793781504814285146046152725a52645614617950963892738760756425862842841743452645614617950963892738760756425862842841743426816248a67364173819a63849157045348358a5716405726415a8716248a67364173819a63849157045348358a5716405726415a87522d285928572543947190764379249064714793762470694231375625860852758638607653754817468376248953956286522e706715698374956246835683945087593789562498452422d63579165825982576279807542678529857926746358922f269437914578174359816958063875263471541981624670301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010100125113fff13fff13fff13fff13fff";
            }
        }
        protected string ExtraAntePurString => "1001251";
        protected override bool SupportPurchaseFree => true;
        protected override double PurchaseFreeMultiple
        {
            get
            {
                return 81.0;
            }
        }

        #endregion

        public DiamondStaxxGameLogic()
        {
            _gameID     = GAMEID.DiamondStaxx;
            GameName    = "DiamondStaxx";
        }

        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BaseAmaticExtraWildInitPacket initPacket = new BaseAmaticExtraWildInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum, ExtraString);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    BaseAmaticExtraWildPacket amaPacket = new BaseAmaticExtraWildPacket(spinResult.ResultString, Cols, FreeCols);
                    initPacket.extrastr = amaPacket.extrastr;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLeftHexString(initString, ExtraAntePurString);
            initString = encrypt.WriteLeftHexString(initString, initPacket.extrastr);
            
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BaseAmaticExtraWildPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BaseAmaticExtraWildPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BaseAmaticExtraWildPacket(Cols, FreeCols, (int)type, (int)LINES.Last(), ExtraString);

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BaseAmaticExtraWildPacket oldPacket = new BaseAmaticExtraWildPacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extrastr     = oldPacket.extrastr;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            BaseAmaticExtraWildPacket extraWildPacket = null;
            if (packet is BaseAmaticExtraWildPacket)
                extraWildPacket = packet as BaseAmaticExtraWildPacket;
            else
                extraWildPacket = new BaseAmaticExtraWildPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last(), ExtraString);

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLeftHexString(newSpinString, extraWildPacket.extrastr);
            return newSpinString;
        }
    }
}
