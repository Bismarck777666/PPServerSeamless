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
   
    class DiamondMonkeyGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DiamondMonkey";
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
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0522d40000111922233384444666697777888836555566444623a444471111800002222733331555566665777718885553337234567456722d11100028943335555677778666684444696888822224823a666610000444478888511117555513333577773333222257716788556722d4777735555166667888849333354444522290007111185222139062456781293840567456781579830222001728345680123745684756012301234222215349065786512973568401823749867022218234567801234567845267850673120342211234056978123459067318402758612300301010101010104271010001a5186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0331010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010321010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 5;
        #endregion

        public DiamondMonkeyGameLogic()
        {
            _gameID     = GAMEID.DiamondMonkey;
            GameName    = "DiamondMonkey";
        }

        protected override int getLineTypeFromPlayLine(BaseAmaticSlotBetInfo betInfo)
        {
            switch (betInfo.PlayLine)
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
                default:
                    return -1;
            }
        }
        protected override string buildInitString(string strUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strUserID, balance, currency);
            DiamondMonkeyInitPacket grandInitPacket = new DiamondMonkeyInitPacket(initString, Cols, FreeCols, ReelSetBitNum, (int)LINES.Last());

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strUserID];
                if (betInfo.HasRemainResponse)
                {
                    DiamondMonkeyPacket amaPacket = new DiamondMonkeyPacket(spinResult.ResultString, Cols, FreeCols);
                    grandInitPacket.extrawildsymbol = amaPacket.extrawildsymbol;
                    grandInitPacket.extrawin        = amaPacket.extrawin;
                    grandInitPacket.extralinewins   = amaPacket.extralinewins;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, grandInitPacket.extrawildsymbol);
            initString = encrypt.WriteLengthAndDec(initString, grandInitPacket.extrawin);
            initString = encrypt.WriteDec2Hex(initString, grandInitPacket.extralinewins.Count);
            for (int i = 0; i < grandInitPacket.extralinewins.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, grandInitPacket.extralinewins[i]);
            }

            return initString;
        }
        protected override string buildResMsgString(string strUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            DiamondMonkeyPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new DiamondMonkeyPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new DiamondMonkeyPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

                    DiamondMonkeyPacket oldPacket = new DiamondMonkeyPacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extrawildsymbol  = oldPacket.extrawildsymbol;
                    packet.extrawin         = oldPacket.extrawin;
                    int extracnt = oldPacket.extralinewins.Count;
                    packet.extralinewins    = new List<long>();
                    for (int i = 0; i < extracnt; i++)
                    {
                        packet.extralinewins.Add(0);
                    }
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            DiamondMonkeyPacket diamondMoneyPacket = packet as DiamondMonkeyPacket;
            
            if (packet is DiamondMonkeyPacket)
                diamondMoneyPacket = packet as DiamondMonkeyPacket;
            else
                diamondMoneyPacket = new DiamondMonkeyPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, diamondMoneyPacket.extrawildsymbol);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, diamondMoneyPacket.extrawin);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, diamondMoneyPacket.extralinewins.Count);
            for (int i = 0; i < diamondMoneyPacket.extralinewins.Count; i++)
            {
                newSpinString = encrypt.WriteLengthAndDec(newSpinString, diamondMoneyPacket.extralinewins[i]);
            }

            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            DiamondMonkeyPacket grandPacket = packet as DiamondMonkeyPacket;
            
            base.convertWinsByBet(balance, grandPacket, betInfo);
            grandPacket.extrawin = convertWinByBet(grandPacket.extrawin, betInfo);
        }

        public class DiamondMonkeyPacket : AmaticPacket
        {
            public long         extrawildsymbol     { get; set; }
            public long         extrawin            { get; set; }
            public List<long>   extralinewins       { get; set; }

            public DiamondMonkeyPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extrawildsymbol = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                extrawin        = amaConverter.ReadLengthAndDec(message, point, out point);
                
                long extraLinewinCnt = amaConverter.Read2BitHexToDec(message, point, out point);
                extralinewins = new List<long>();
                for (int i = 0; i < extraLinewinCnt; i++)
                {
                    extralinewins.Add(amaConverter.ReadLengthAndDec(message, point, out point));
                }
                curpoint        = point;
            }

            public DiamondMonkeyPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extrawildsymbol = 0;
                extrawin        = 0;
                extralinewins = new List<long>();
                for (int i = 0; i < lineCnt; i++)
                    extralinewins.Add(0);
            }
        }
        public class DiamondMonkeyInitPacket : InitPacket
        {
            public long         extrawildsymbol { get; set; }
            public long         extrawin        { get; set; }
            public List<long>   extralinewins   { get; set; }

            public DiamondMonkeyInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetBitCnt, int lines) : base(message, reelCnt, freeReelCnt, reelsetBitCnt)
            {
                extrawin        = 0;
                extrawildsymbol = 0;
                
                extralinewins   = new List<long>();
                for (int i = 0; i < lines; i++)
                {
                    extralinewins.Add(0);
                }
                curpoint        = 2 + 2 + 2 * lines;
            }
        }
    }
}
