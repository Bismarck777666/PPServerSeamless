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
   
    class FruitExpressGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "FruitExpress";
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
                return "0525164442225552226664443334445553335554446665556665550006662223331112224440005551116627a6224446662225552226663335553336665554443332221115557000111766611122274441113337444333111444555666555006662223334445551116627f655544466622266633366644422233344422244433355500066675551115552225553336660001116661112220003334447000444111333444222114445556625f6755522266600071112220007222444555333444666444666555444000444111333111444222555000555111555116625a64446111666444666555111333000666333000111333002225553332226663334441115554442225554445556600301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b10101010101010101010100b10101010101010101010100b101010101010101010101010000101010101150000015000001500000150000022e65500552244400022442225511223355544554433344661377723c200442200066220044335544116655001122333666001115553300444722236655533344554455000222004466552244422700441112255112266237266333663344330011133113355566116655444114433000666332222e655005522444000224422255112233555445544333446623421115511331166114443355566333663344337446633000666221377723620044422665522441112255112255533344554455744666550002223b2666441113311335556611333665544411334411330001166330066332223465544665500552233444000224422255112233555445544333662352111551133116611443355533366556633444337446633006662223a23366001115553300444220006622335544116655443336667550011221377723b2556664422554422550066225500022552266551116655533366334442222a05522444004422255112233555333445544666550013777137772352004442266552244111225511225553334455445574466655002223b2666441113311335556611333665544411334411330001166330066332222a0552244400442225511223355533344554466655001377723a23366001115553300444220006622335544116655443336667550011221377723b2330066223300022332266555663311166335544663336633446664442222e655005522444000224422255112233555445544333446623521115553311335533366116611444334433744665566330066622137771377723b25566644225544225500662255000225522665511166555333663344422234255112233555445500022665522663334433446664440022442213777137771377723b26664411133113355566113336655444113344113300011663300663322";
            }
        }
        protected string RespinReelsetString
        {
            get
            {
                return "22e65500552244400022442225511223355544554433344661377723c200442200066220044335544116655001122333666001115553300444722236655533344554455000222004466552244422700441112255112266237266333663344330011133113355566116655444114433000666332222e655005522444000224422255112233555445544333446623421115511331166114443355566333663344337446633000666221377723620044422665522441112255112255533344554455744666550002223b2666441113311335556611333665544411334411330001166330066332223465544665500552233444000224422255112233555445544333662352111551133116611443355533366556633444337446633006662223a23366001115553300444220006622335544116655443336667550011221377723b2556664422554422550066225500022552266551116655533366334442222a05522444004422255112233555333445544666550013777137772352004442266552244111225511225553334455445574466655002223b2666441113311335556611333665544411334411330001166330066332222a0552244400442225511223355533344554466655001377723a23366001115553300444220006622335544116655443336667550011221377723b2330066223300022332266555663311166335544663336633446664442222e655005522444000224422255112233555445544333446623521115553311335533366116611444334433744665566330066622137771377723b25566644225544225500662255000225522665511166555333663344422234255112233555445500022665522663334433446664440022442213777137771377723b26664411133113355566113336655444113344113300011663300663322";
            }
        }
        #endregion

        public FruitExpressGameLogic()
        {
            _gameID     = GAMEID.FruitExpress;
            GameName    = "FruitExpress";
        }

        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            FruitExpressInitPacket respinInitPacket = new FruitExpressInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    FruitExpressPacket amaPacket = new FruitExpressPacket(spinResult.ResultString, Cols, FreeCols);
                    respinInitPacket.linewins1 = new List<long>();
                    for(int i = 0; i < amaPacket.linewins1.Count; i++)
                        respinInitPacket.linewins1.Add(amaPacket.linewins1[i]);
                    respinInitPacket.linewins2 = new List<long>();
                    for (int i = 0; i < amaPacket.linewins2.Count; i++)
                        respinInitPacket.linewins2.Add(amaPacket.linewins2[i]);
                    respinInitPacket.accwin     = amaPacket.accwin;
                    respinInitPacket.respintype = amaPacket.respintype;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteDec2Hex(initString, respinInitPacket.linewins1.Count);
            for (int i = 0; i < respinInitPacket.linewins1.Count; i++)
                initString = encrypt.WriteLengthAndDec(initString, respinInitPacket.linewins1[i]);
            initString = encrypt.WriteDec2Hex(initString, respinInitPacket.linewins2.Count);
            for (int i = 0; i < respinInitPacket.linewins2.Count; i++)
                initString = encrypt.WriteLengthAndDec(initString, respinInitPacket.linewins2[i]);
            
            initString = encrypt.WriteLengthAndDec(initString, respinInitPacket.accwin);
            initString = encrypt.WriteLeftHexString(initString, respinInitPacket.respintype);
            initString = encrypt.WriteLeftHexString(initString, RespinReelsetString);
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            FruitExpressPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new FruitExpressPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new FruitExpressPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    FruitExpressPacket oldPacket = new FruitExpressPacket(spinResult.ResultString, Cols, FreeCols);
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

                    int cnt1 = oldPacket.linewins1.Count;
                    packet.linewins1 = new List<long>();
                    for (int i = 0; i < cnt1; i++)
                        packet.linewins1.Add(oldPacket.linewins1[i]);
                    int cnt2 = oldPacket.linewins2.Count;
                    packet.linewins2 = new List<long>();
                    for (int i = 0; i < cnt2; i++)
                        packet.linewins2.Add(oldPacket.linewins2[i]);

                    packet.accwin       = oldPacket.accwin;
                    packet.respintype   = oldPacket.respintype;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            FruitExpressPacket respinPacket = null;
            if (packet is FruitExpressPacket)
                respinPacket = packet as FruitExpressPacket;
            else
                respinPacket = new FruitExpressPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteDec2Hex(newSpinString, respinPacket.linewins1.Count);
            for(int i = 0; i < respinPacket.linewins1.Count; i++)
                newSpinString = encrypt.WriteLengthAndDec(newSpinString, respinPacket.linewins1[i]);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, respinPacket.linewins2.Count);
            for (int i = 0; i < respinPacket.linewins2.Count; i++)
                newSpinString = encrypt.WriteLengthAndDec(newSpinString, respinPacket.linewins2[i]);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, respinPacket.accwin);
            newSpinString = encrypt.WriteLeftHexString(newSpinString, respinPacket.respintype);
            return newSpinString;
        }
        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            FruitExpressPacket respinPacket = packet as FruitExpressPacket;

            base.convertWinsByBet(balance, respinPacket, betInfo);
            respinPacket.accwin = convertWinByBet(respinPacket.accwin, betInfo);
        }

        public class FruitExpressPacket : AmaticPacket
        {
            public List<long>   linewins1   { get; set; }
            public List<long>   linewins2   { get; set; }
            public long         accwin      { get; set; }
            public string       respintype  { get; set; }

            public FruitExpressPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = curpoint;
                linewins1 = new List<long>();
                long lineCnt1 = amaConverter.Read2BitHexToDec(message, point, out point);
                for (int i = 0; i < lineCnt1; i++)
                    linewins1.Add(amaConverter.ReadLengthAndDec(message, point, out point));

                linewins2 = new List<long>();
                long lineCnt2 = amaConverter.Read2BitHexToDec(message, point, out point);
                for (int i = 0; i < lineCnt2; i++)
                    linewins2.Add(amaConverter.ReadLengthAndDec(message, point, out point));

                accwin      = amaConverter.ReadLengthAndDec(message, point, out point);
                respintype  = amaConverter.ReadLeftHexString(message, point, out point);
                curpoint    = point;
            }

            public FruitExpressPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                linewins1 = new List<long>();
                for (int i = 0; i < lineCnt + 1; i++)
                    linewins1.Add(0);
                linewins2 = new List<long>();
                for (int i = 0; i < lineCnt + 1; i++)
                    linewins2.Add(0);

                curpoint = curpoint + 2 + linewins1.Count * 2 + 2 + linewins2.Count * 2;

                accwin      = 0;
                respintype  = "0001010101011500000150000015000001500000";
                curpoint    = curpoint + 2 + respintype.Length;
            }
        }

        public class FruitExpressInitPacket : InitPacket
        {
            public List<long>   linewins1       { get; set; }
            public List<long>   linewins2       { get; set; }
            public long         accwin          { get; set; }
            public string       respintype      { get; set; }
            public string       respinreelset   { get; set; }

            public FruitExpressInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                linewins1 = new List<long>();
                for (int i = 0; i < linewins.Count; i++)
                    linewins1.Add(0);
                linewins2 = new List<long>();
                for (int i = 0; i < linewins.Count; i++)
                    linewins2.Add(0);

                accwin          = 0;
                respintype      = "0001010101011500000150000015000001500000";
                respinreelset = "";
                curpoint        = curpoint + (linewins.Count + 1) * 2 * 2 + 2 + respintype.Length + respinreelset.Length;
            }
        }
    }
}
