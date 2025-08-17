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
    class HotChoiceDeluxeGameLogic : BaseSelFreeAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotChoiceDeluxe";
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
                return "052255755666551141445430333225552244642254228551101666733344242206655511336664464021122755506667444041113335552262333446660442422a56556663311044440433372255660116664442211222857551146644443335066220336660155522114405215766655144303366655524216066733442265566655514621755566674401555226633446217554466631446033722555662185714664435550664462233550301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b101010101010101010101000";
            }
        }

        protected override int FreeSpinTypeCount
        {
            get { return 3; }
        }
        
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202 };
        }
        #endregion

        public HotChoiceDeluxeGameLogic()
        {
            _gameID     = GAMEID.HotChoiceDeluxe;
            GameName    = "HotChoiceDeluxe";
        }

        protected override string addStartWinToResponse(string strResponse, BasePPSlotStartSpinData startSpinData)
        {
            HotChoiceDeluxePacket firstPacket = new HotChoiceDeluxePacket(startSpinData.SpinStrings[0], Cols, FreeCols);
            HotChoiceDeluxePacket packet = new HotChoiceDeluxePacket(strResponse, Cols, FreeCols);
            packet.win = firstPacket.win + packet.totalfreewin;

            return buildSpinString(packet);
        }
        protected override void onFreeSpinOptionSelectRequest(string strUserID, int websiteID, GITMessage message, double userBalance)
        {
            try
            {
                int selectOption            = (int)message.Pop();
                GITMessage responseMessage  = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_FSOPTION);

                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) || selectOption < 0)
                {
                    _logger.Error("{0} option select error in HotChoiceDeluxeGameLogic::onFreeSpinOptionSelectRequest", strGlobalUserID);
                }
                else
                {
                    BaseAmaticSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    BaseAmaticSlotSpinResult    result  = _dicUserResultInfos[strGlobalUserID];

                    BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                    if (selectOption >= startSpinData.FreeSpins.Count)
                    {
                        _logger.Error("{0} option select error in HotChoiceDeluxeGameLogic::onFreeSpinOptionSelectRequest", strGlobalUserID);
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

                        HotChoiceDeluxePacket packet = new HotChoiceDeluxePacket(strSpinResponse, Cols, FreeCols);
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
                _logger.Error("Exception has been occurred in HotChoiceDeluxeGameLogic::onFreeSpinOptionSelectRequest {0}", ex);
            }
        }
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            HotChoiceDeluxeInitPacket initPacket = new HotChoiceDeluxeInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    HotChoiceDeluxePacket amaPacket = new HotChoiceDeluxePacket(spinResult.ResultString, Cols, FreeCols);
                    initPacket.freeoptionsymbol  = amaPacket.freeoptionsymbol;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteDec2Hex(initString, initPacket.freeoptionsymbol);
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            HotChoiceDeluxePacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new HotChoiceDeluxePacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new HotChoiceDeluxePacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    HotChoiceDeluxePacket oldPacket = new HotChoiceDeluxePacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.freeoptionsymbol = oldPacket.freeoptionsymbol;
                }
            }

            return buildSpinString(packet);
        }
        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            HotChoiceDeluxePacket hotChoicePacket = null;
            if (packet is HotChoiceDeluxePacket)
                hotChoicePacket = packet as HotChoiceDeluxePacket;
            else
                hotChoicePacket = new HotChoiceDeluxePacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteDec2Hex(newSpinString, hotChoicePacket.freeoptionsymbol);
            return newSpinString;
        }

        public class HotChoiceDeluxePacket : AmaticPacket
        {
            public long     freeoptionsymbol     { get; set; }
            public HotChoiceDeluxePacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point           = 0;
                freeoptionsymbol    = amaConverter.Read2BitHexToDec(message, curpoint, out point);
                curpoint            = point;
            }

            public HotChoiceDeluxePacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                freeoptionsymbol    = 0;
                curpoint            = curpoint + 2;
            }
        }

        public class HotChoiceDeluxeInitPacket : InitPacket
        {
            public long     freeoptionsymbol    { get; set; }

            public HotChoiceDeluxeInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                freeoptionsymbol    = 0;
                curpoint            = curpoint + 2;
            }
        }
    }
}
