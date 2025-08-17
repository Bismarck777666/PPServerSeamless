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
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
    class LuckyZodiacGameLogic : BaseSelFreeAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyZodiac";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 40, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "0522973462038b2acd49c5607ab023468a0bc2a6a12069229a16b3894c7d1425307c859b1345789bc15937b37b2291056289abd2105c762849a140568ac159b234067a23612634579a0bc8d1293745c618ab1234056789abc203456789abc08236236b517048a92cd145683709abc1234056789abc012345d6789abc521a234c516d78ab951234b67089ac21ba0238456179bcd12348679acb5d21b4123567089dab1c23c56d789a4b21a312456789abcd152367849cab021a31427b56089cad3192468a57cb0301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010101010";
            }
        }

        protected override int FreeSpinTypeCount
        {
            get { return 12; }
        }
        
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211 };
        }
        #endregion

        public LuckyZodiacGameLogic()
        {
            _gameID     = GAMEID.LuckyZodiac;
            GameName    = "LuckyZodiac";
        }

        protected override void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            LuckyZodiacPacket respinPacket = packet as LuckyZodiacPacket;

            base.convertWinsByBet(balance, respinPacket, betInfo);
            respinPacket.extrawin = convertWinByBet(respinPacket.extrawin, betInfo);
        }
        protected override string addStartWinToResponse(string strResponse, BasePPSlotStartSpinData startSpinData)
        {
            LuckyZodiacPacket firstPacket = new LuckyZodiacPacket(startSpinData.SpinStrings[0], Cols, FreeCols);
            LuckyZodiacPacket packet = new LuckyZodiacPacket(strResponse, Cols, FreeCols);
            packet.win = firstPacket.win + packet.totalfreewin;

            return buildSpinString(packet);
        }
        protected override void onFreeSpinOptionSelectRequest(string strUserID, int websiteID, GITMessage message, double userBalance)
        {
            try
            {
                int selectOption    = (int)message.Pop();
                selectOption        = selectOption - 1;
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_FSOPTION);

                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) || selectOption < 0)
                {
                    _logger.Error("{0} option select error in LuckyZodiacGameLogic::onFreeSpinOptionSelectRequest", strGlobalUserID);
                }
                else
                {
                    BaseAmaticSlotBetInfo   betInfo = _dicUserBetInfos[strGlobalUserID];
                    BaseAmaticSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];

                    BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                    if (selectOption >= startSpinData.FreeSpins.Count)
                    {
                        _logger.Error("{0} option select error in LuckyZodiacGameLogic::onFreeSpinOptionSelectRequest", strGlobalUserID);
                    }
                    else
                    {
                        BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[selectOption]);
                        betInfo.SpinData = freeSpinData;

                        List<string> freeSpinStrings = new List<string>();
                        for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                            freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData));

                        string strSpinResponse = freeSpinStrings[0];
                        if (freeSpinStrings.Count > 1)
                            betInfo.RemainReponses = buildResponseList(freeSpinStrings);

                        double selectedWin  = (startSpinData.StartOdd + freeSpinData.SpinOdd) * (betInfo.RelativeTotalBet * BettingButton[betInfo.PlayBet]);
                        double maxWin       = startSpinData.MaxOdd * (betInfo.RelativeTotalBet * BettingButton[betInfo.PlayBet]);
                        sumUpWebsiteBetWin(websiteID, 0.0, selectedWin - maxWin);

                        LuckyZodiacPacket packet = new LuckyZodiacPacket(strSpinResponse, Cols, FreeCols);
                        convertWinsByBet(userBalance, packet, betInfo);
                        BaseAmaticSlotSpinResult spinResult = new BaseAmaticSlotSpinResult();
                        spinResult.ResultString = buildSpinString(packet);
                        responseMessage.Append(spinResult.ResultString);

                        _dicUserResultInfos[strGlobalUserID]            = spinResult;
                        sendFreeOptionPickResult(spinResult, strGlobalUserID, 0.0, 0.0, "FreeOption", userBalance);
                        _dicUserLastBackupResultInfos[strGlobalUserID]  = spinResult;
                    }
                    
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in LuckyZodiacGameLogic::onFreeSpinOptionSelectRequest {0}", ex);
            }
        }
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            LuckyZodiacInitPacket initPacket = new LuckyZodiacInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    LuckyZodiacPacket amaPacket = new LuckyZodiacPacket(spinResult.ResultString, Cols, FreeCols);
                    initPacket.extrasymbol  = amaPacket.extrasymbol;
                    initPacket.extracolcnt  = amaPacket.extracolcnt;
                    initPacket.extrawin     = amaPacket.extrawin;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrasymbol);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extracolcnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.extrawin);
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            LuckyZodiacPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new LuckyZodiacPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new LuckyZodiacPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    LuckyZodiacPacket oldPacket = new LuckyZodiacPacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extrasymbol      = oldPacket.extrasymbol;
                    packet.extracolcnt      = oldPacket.extracolcnt;
                    packet.extrawin         = oldPacket.extrawin;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            LuckyZodiacPacket luckyZodiacPacket = null;
            if (packet is LuckyZodiacPacket)
                luckyZodiacPacket = packet as LuckyZodiacPacket;
            else
                luckyZodiacPacket = new LuckyZodiacPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, luckyZodiacPacket.extrasymbol);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, luckyZodiacPacket.extracolcnt);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, luckyZodiacPacket.extrawin);
            return newSpinString;
        }

        public class LuckyZodiacPacket : AmaticPacket
        {
            public long     extrasymbol     { get; set; }
            public long     extracolcnt     { get; set; }
            public long     extrawin        { get; set; }

            public LuckyZodiacPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point = 0;
                extrasymbol = amaConverter.ReadLengthAndDec(message, curpoint, out point);
                extracolcnt = amaConverter.ReadLengthAndDec(message, point, out point);
                extrawin    = amaConverter.ReadLengthAndDec(message, point, out point);
                curpoint    = point;
            }

            public LuckyZodiacPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extrasymbol = 0;
                extracolcnt = 0;
                extrawin    = 0;
                curpoint    = curpoint + 2 + 2 + 2;
            }
        }

        public class LuckyZodiacInitPacket : InitPacket
        {
            public long     extrasymbol     { get; set; }
            public long     extracolcnt     { get; set; }
            public long     extrawin        { get; set; }

            public LuckyZodiacInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                extrasymbol = 0;
                extracolcnt = 0;
                extrawin    = 0;
                curpoint    = curpoint + 2 + 2 + 2;
            }
        }
    }
}
