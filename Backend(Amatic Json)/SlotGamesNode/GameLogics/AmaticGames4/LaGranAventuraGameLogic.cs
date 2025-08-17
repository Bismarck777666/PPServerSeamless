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
    class LaGranAventuraGameLogic : BaseSelFreeAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LaGranAventura";
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
                return new long[] { 20 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052210245678bac9456789a1b893ab78a9b465221189a73c45b68a7089ab928b9ab645475621f452698a0bc7a98b8917a6b47589a3b6221345678bca4957829ab86a91b8609ba74522325468790bac564378ba989ab18794b6a57652218508587a1a664289479a379b7ab6b9b452244641b5b2b6389799778570aa88a56a6467b9227448bb70ab7988a1a4a696956529974ab835b85a22874b0a4a7881867bb5b6953569659a7849ab289a4229148943986525778159abb27a6b0ba6b938a9467a8030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a2641510101010101010101010101010101010101010101000150000015000001500000";
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

        public LaGranAventuraGameLogic()
        {
            _gameID     = GAMEID.LaGranAventura;
            GameName    = "LaGranAventura";
        }

        protected override string addStartWinToResponse(string strResponse, BasePPSlotStartSpinData startSpinData)
        {
            LaGranAventuraPacket firstPacket = new LaGranAventuraPacket(startSpinData.SpinStrings[0], Cols, FreeCols);
            LaGranAventuraPacket packet = new LaGranAventuraPacket(strResponse, Cols, FreeCols);
            packet.win = firstPacket.win + packet.totalfreewin;

            return buildSpinString(packet);
        }

        protected override void onFreeSpinOptionSelectRequest(string strUserID, int companyID, GITMessage message, UserBonus userBonus, double userBalance)
        {
            try
            {
                int selectOption    = (int)message.Pop();
                selectOption        = selectOption - 1;
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_FSOPTION);

                if (!_dicUserResultInfos.ContainsKey(strUserID) || !_dicUserBetInfos.ContainsKey(strUserID) || selectOption < 0)
                {
                    _logger.Error("{0} option select error in LaGranAventuraGameLogic::onFreeSpinOptionSelectRequest", strUserID);
                }
                else
                {
                    BaseAmaticSlotBetInfo       betInfo = _dicUserBetInfos[strUserID];
                    BaseAmaticSlotSpinResult    result  = _dicUserResultInfos[strUserID];

                    BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                    if (selectOption >= startSpinData.FreeSpins.Count)
                    {
                        _logger.Error("{0} option select error in LaGranAventuraGameLogic::onFreeSpinOptionSelectRequest", strUserID);
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

                        if (!startSpinData.IsEvent)
                            sumUpWebsiteBetWin(companyID, 0.0, selectedWin - maxWin);
                        else if (maxWin > selectedWin)
                            addEventLeftMoney(companyID, strUserID, maxWin - selectedWin);

                        LaGranAventuraPacket packet = new LaGranAventuraPacket(strSpinResponse, Cols, FreeCols);
                        convertWinsByBet(userBalance, packet, betInfo);
                        BaseAmaticSlotSpinResult spinResult = new BaseAmaticSlotSpinResult();
                        spinResult.ResultString = buildSpinString(packet);
                        responseMessage.Append(spinResult.ResultString);

                        _dicUserResultInfos[strUserID] = spinResult;
                        sendFreeOptionPickResult(spinResult, strUserID, 0.0, 0.0, "FreeOption", userBalance);
                        _dicUserLastBackupResultInfos[strUserID] = spinResult;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in LaGranAventuraGameLogic::onFreeSpinOptionSelectRequest {0}", ex);
            }
        }

        protected override string buildInitString(string strUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strUserID, balance, currency);
            LaGranAventuraInitPacket initPacket = new LaGranAventuraInitPacket(initString, Cols, FreeCols, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];
                if (betInfo.HasRemainResponse)
                {
                    LaGranAventuraPacket amaPacket = new LaGranAventuraPacket(spinResult.ResultString, Cols, FreeCols);
                    initPacket.extrastr  = amaPacket.extrastr;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLeftHexString(initString, initPacket.extrastr);
            return initString;
        }

        protected override string buildResMsgString(string strUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            LaGranAventuraPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new LaGranAventuraPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new LaGranAventuraPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

                    LaGranAventuraPacket oldPacket = new LaGranAventuraPacket(spinResult.ResultString, Cols, FreeCols);
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

                    packet.extrastr = oldPacket.extrastr;
                }
            }

            return buildSpinString(packet);
        }

        protected override string buildSpinString(AmaticPacket packet)
        {
            string newSpinString = base.buildSpinString(packet);

            LaGranAventuraPacket laGranAventuraPacket = null;
            if (packet is LaGranAventuraPacket)
                laGranAventuraPacket = packet as LaGranAventuraPacket;
            else
                laGranAventuraPacket = new LaGranAventuraPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteLeftHexString(newSpinString, laGranAventuraPacket.extrastr);
            return newSpinString;
        }

        public class LaGranAventuraPacket : AmaticPacket
        {
            public string     extrastr     { get; set; }
            public LaGranAventuraPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point   = 0;
                extrastr    = amaConverter.ReadLeftHexString(message, curpoint, out point);
                curpoint    = point;
            }

            public LaGranAventuraPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                extrastr    = "00150000015000001500000";
                curpoint    = curpoint + extrastr.Length;
            }
        }

        public class LaGranAventuraInitPacket : InitPacket
        {
            public string     extrastr    { get; set; }

            public LaGranAventuraInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetBitCnt)
            {
                extrastr    = "00150000015000001500000";
                curpoint    = curpoint + extrastr.Length;
            }
        }
    }
}
