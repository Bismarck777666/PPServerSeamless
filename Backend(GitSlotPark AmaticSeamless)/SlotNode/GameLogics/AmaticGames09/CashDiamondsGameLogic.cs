using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class CashDiamondsGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "CashDiamonds";
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
                return "05248543264237770631701738735043715552731574266604324355523605436836777136521248632477725551240566635243613721821426432734517463513540364764361374265865252477746abc460726555243427648647317505412427766645324303416527465128125463863175067123f25401281201607771436665413503513263432762460766671555417357403524615067012015273483476034753246536140472555414523777136660342537532462535242999999999999999999a99999999999999999999bc99999999999999999999def99242999999999999999999a99999999999999999999bc99999999999999999999def9921cabc4607265552434276486473175242999999999999999999a99999999999999999999bc99999999999999999999def99242999999999999999999a99999999999999999999bc99999999999999999999def990301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010111213151112111213151112111213151112111213151112111213151112101010101010101010101010101010";
            }
        }
        #endregion

        public CashDiamondsGameLogic()
        {
            _gameID     = GAMEID.CashDiamonds;
            GameName    = "CashDiamonds";
        }

        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            BonusInitPacket bonusInitPacket = new BonusInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    BounsPacket amaPacket = new BounsPacket(spinResult.ResultString, Cols, FreeCols);
                    bonusInitPacket.bonusmasks = new List<long>();
                    for(int i = 0; i < amaPacket.bonusmasks.Count; i++)
                        bonusInitPacket.bonusmasks.Add(amaPacket.bonusmasks[i]);

                    bonusInitPacket.bonuswins = new List<long>();
                    for (int i = 0; i < amaPacket.bonuswins.Count; i++)
                        bonusInitPacket.bonuswins.Add(amaPacket.bonuswins[i]);
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            for (int i = 0; i < bonusInitPacket.bonusmasks.Count; i++)
                initString = encrypt.WriteLengthAndDec(initString, bonusInitPacket.bonusmasks[i]);
            
            for (int i = 0; i < bonusInitPacket.bonuswins.Count; i++)
                initString = encrypt.WriteLengthAndDec(initString, bonusInitPacket.bonuswins[i]);
            
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            BounsPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new BounsPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new BounsPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    BounsPacket oldPacket = new BounsPacket(spinResult.ResultString, Cols, FreeCols);
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

                    int cnt1 = oldPacket.bonusmasks.Count;
                    packet.bonusmasks = new List<long>();
                    for (int i = 0; i < cnt1; i++)
                        packet.bonusmasks.Add(oldPacket.bonusmasks[i]);
                    
                    int cnt2 = oldPacket.bonuswins.Count;
                    packet.bonuswins = new List<long>();
                    for (int i = 0; i < cnt2; i++)
                        packet.bonuswins.Add(oldPacket.bonuswins[i]);
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            BounsPacket bonusPacket = null;
            if (packet is BounsPacket)
                bonusPacket = packet as BounsPacket;
            else
                bonusPacket = new BounsPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            for(int i = 0; i < bonusPacket.bonusmasks.Count; i++)
                newSpinString = encrypt.WriteLengthAndDec(newSpinString, bonusPacket.bonusmasks[i]);

            for (int i = 0; i < bonusPacket.bonuswins.Count; i++)
                newSpinString = encrypt.WriteLengthAndDec(newSpinString, bonusPacket.bonuswins[i]);
            
            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            BounsPacket bonusPacket = packet as BounsPacket;

            base.convertWinsByBet(balance, bonusPacket, betInfo);

            for(int i = 0; i < bonusPacket.bonuswins.Count; i++)
                bonusPacket.bonuswins[i] = convertWinByBet(bonusPacket.bonuswins[i], betInfo);
        }

        public class BounsPacket : AmaticPacket
        {
            public List<long>   bonusmasks  { get; set; }
            public List<long>   bonuswins   { get; set; }

            public BounsPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = curpoint;
                bonusmasks = new List<long>();
                for (int i = 0; i < 30; i++)
                    bonusmasks.Add(amaConverter.ReadLengthAndDec(message, point, out point));
                
                bonuswins = new List<long>();
                for (int i = 0; i < 15; i++)
                    bonuswins.Add(amaConverter.ReadLengthAndDec(message, point, out point));

                curpoint = point;
            }

            public BounsPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                bonusmasks = new List<long>();
                for (int i = 0; i < 30; i++)
                    bonusmasks.Add(0);

                bonuswins = new List<long>();
                for (int i = 0; i < 15; i++)
                    bonuswins.Add(0);
                
                curpoint = curpoint + (bonusmasks.Count + bonuswins.Count) * 2;
            }
        }

        public class BonusInitPacket : InitPacket
        {
            public List<long>   bonusmasks  { get; set; }
            public List<long>   bonuswins   { get; set; }

            public BonusInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                bonusmasks = new List<long>();
                for (int i = 0; i < 30; i++)
                    bonusmasks.Add(0);

                bonuswins = new List<long>();
                for (int i = 0; i < 15; i++)
                    bonuswins.Add(0);

                curpoint = curpoint + (bonusmasks.Count + bonuswins.Count) * 2;
            }
        }
    }
}
