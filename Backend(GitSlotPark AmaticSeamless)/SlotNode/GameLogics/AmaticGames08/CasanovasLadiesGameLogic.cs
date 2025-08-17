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
    class CasanovasLadiesGameLogic : BaseSelFreeAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "CasanovasLadies";
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
                return new long[] { 25 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0523904000606060402050508080a0301050502070707000404040809090900080808050707080306050601080600040404090909000808080606062300305010409080602060809090204040a060600090901090807070708030505050808080909080707070109080707070822b0005050502040404060708030909090a010504060606020809070707060505090904040601080907070706222010505040306060704000708080a06060605050200040404090909030400070707042200108050305050a06060408090902040404000608030707070008080805090909523e04000606060402050508080a03060b0c0d05050207070700040404080909090008080805070708030605060b0c0d0806000404040909090008080806060623603050e0f100409080602060809090204040a06060009090e0f10090807070708030505050808080909080707070e0f100908070707082310005050502040404060708030909090a09070e0f10050406060602080907070706050509090404060e0f100809070707062240e0f100505040306060704000708080a06060605050200040404090909030400070707042220e0f1008050305050a0606040809090204040400060803070707000808080509090903010101010101042710100021931f419101010101010101919191100101010101000000000000000000811121314151a1f2141a101010101010101010101010101010101010101010101010101052450400060606040b0c0d050508080a03060b0c0d0909090b0c0d050507070700040404080909090008080805070708030605060b0c0d0806000404040909090008080806060623a03050e0f10040908061112130608090904040a06060009090e0f10090807070708030505050808080909080707070e0f1011121309080707070823500050505040404060708030909090a09070e0f100504060606111213080907070706050509090404060e0f101112130809070707062260e0f101112130505040306060704000708080a060606050500040404090909030400070707042240e0f1011121308050305050a060604080909040404000608030707070008080805090909524d0400060606040b0c0d050508080a06060b0c0d0808080b0c0d0909090b0c0d0505070707000404040809090900080808050707080b0c0d0605060b0c0d0806000404040909090008080806060623f05050e0f10040908061112130608090904040a06060009090e0f100908070707081415160505050808080909080707070e0f10111213141516090807070708237000505050404040607080909090a09070e0f100504060606111213080907070706050509090404060e0f1011121314151608090707070622a0e0f1011121314151605050406060704000708080a0606060505000404040909091415160400070707042280e0f10111213141516080505050a060604080909040404000608141516070707000808080509090900";
            }
        }
        protected override int ReelSetBitNum => 2;

        protected string ExtraReelsetString = "52450400060606040b0c0d050508080a03060b0c0d0909090b0c0d050507070700040404080909090008080805070708030605060b0c0d0806000404040909090008080806060623a03050e0f10040908061112130608090904040a06060009090e0f10090807070708030505050808080909080707070e0f1011121309080707070823500050505040404060708030909090a09070e0f100504060606111213080907070706050509090404060e0f101112130809070707062260e0f101112130505040306060704000708080a060606050500040404090909030400070707042240e0f1011121308050305050a060604080909040404000608030707070008080805090909524d0400060606040b0c0d050508080a06060b0c0d0808080b0c0d0909090b0c0d0505070707000404040809090900080808050707080b0c0d0605060b0c0d0806000404040909090008080806060623f05050e0f10040908061112130608090904040a06060009090e0f100908070707081415160505050808080909080707070e0f10111213141516090807070708237000505050404040607080909090a09070e0f100504060606111213080907070706050509090404060e0f1011121314151608090707070622a0e0f1011121314151605050406060704000708080a0606060505000404040909091415160400070707042280e0f10111213141516080505050a0606040809090404040006081415160707070008080805090909";
        
        protected override int FreeSpinTypeCount
        {
            get { return 3; }
        }
        
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202 };
        }
        #endregion

        public CasanovasLadiesGameLogic()
        {
            _gameID     = GAMEID.CasanovasLadies;
            GameName    = "CasanovasLadies";
        }

        protected override string addStartWinToResponse(string strResponse, BasePPSlotStartSpinData startSpinData)
        {
            CasanovasLadiesPacket firstPacket = new CasanovasLadiesPacket(startSpinData.SpinStrings[0], Cols, FreeCols);
            CasanovasLadiesPacket packet = new CasanovasLadiesPacket(strResponse, Cols, FreeCols);
            packet.win = firstPacket.win + packet.totalfreewin;

            return buildSpinString(packet);
        }
        protected override void onFreeSpinOptionSelectRequest(string strUserID, int websiteID, GITMessage message, double userBalance)
        {
            try
            {
                int selectOption    = (int)message.Pop();
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_FSOPTION);

                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) || selectOption < 0)
                {
                    _logger.Error("{0} option select error in CasanovasLadiesGameLogic::onFreeSpinOptionSelectRequest", strGlobalUserID);
                }
                else
                {
                    BaseAmaticSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    BaseAmaticSlotSpinResult    result  = _dicUserResultInfos[strGlobalUserID];

                    BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                    if (selectOption >= startSpinData.FreeSpins.Count)
                    {
                        _logger.Error("{0} option select error in CasanovasLadiesGameLogic::onFreeSpinOptionSelectRequest", strGlobalUserID);
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

                        CasanovasLadiesPacket packet = new CasanovasLadiesPacket(strSpinResponse, Cols, FreeCols);
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
                _logger.Error("Exception has been occurred in CasanovasLadiesGameLogic::onFreeSpinOptionSelectRequest {0}", ex);
            }
        }
        protected override string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = base.buildInitString(strGlobalUserID, balance, currency);
            CasanovasLadiesInitPacket initPacket = new CasanovasLadiesInitPacket(initString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    CasanovasLadiesPacket amaPacket = new CasanovasLadiesPacket(spinResult.ResultString, Cols, FreeCols);
                    initPacket.freeoptionsymbol  = amaPacket.freeoptionsymbol;
                }
            }

            AmaticEncrypt encrypt = new AmaticEncrypt();
            initString = encrypt.WriteLeftHexString(initString, ExtraReelsetString);
            initString = encrypt.WriteDec2Hex(initString, initPacket.freeoptionsymbol);
            return initString;
        }
        protected override string buildResMsgString(string strGlobalUserID, double balance, double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            CasanovasLadiesPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new CasanovasLadiesPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new CasanovasLadiesPacket(Cols, FreeCols, (int)type, (int)LINES.Last());

                packet.balance = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep = 0;
                packet.betline = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                    CasanovasLadiesPacket oldPacket = new CasanovasLadiesPacket(spinResult.ResultString, Cols, FreeCols);
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

            CasanovasLadiesPacket hotChoiceDicePacket = null;
            if (packet is CasanovasLadiesPacket)
                hotChoiceDicePacket = packet as CasanovasLadiesPacket;
            else
                hotChoiceDicePacket = new CasanovasLadiesPacket(Cols, FreeCols, (int)packet.messagetype, (int)LINES.Last());

            AmaticEncrypt encrypt = new AmaticEncrypt();
            newSpinString = encrypt.WriteDec2Hex(newSpinString, hotChoiceDicePacket.freeoptionsymbol);
            return newSpinString;
        }

        public class CasanovasLadiesPacket : AmaticPacket
        {
            public long     freeoptionsymbol     { get; set; }
            public CasanovasLadiesPacket(string message, int reelCnt, int freeReelCnt) : base(message, reelCnt, freeReelCnt)
            {
                AmaticDecrypt amaConverter = new AmaticDecrypt();

                int point           = 0;
                freeoptionsymbol    = amaConverter.Read2BitHexToDec(message, curpoint, out point);
                curpoint            = point;
            }

            public CasanovasLadiesPacket(int reelCnt, int freeReelCnt, int msgType, int lineCnt) : base(reelCnt, freeReelCnt, msgType, lineCnt)
            {
                freeoptionsymbol    = 0;
                curpoint            = curpoint + 2;
            }
        }

        public class CasanovasLadiesInitPacket : InitPacket
        {
            public string   extrareelsetstr     { get; set; }
            public long     freeoptionsymbol    { get; set; }

            public CasanovasLadiesInitPacket(string message, int reelCnt, int freeReelCnt, int reelsetColBitCnt, int reelsetBitCnt) : base(message, reelCnt, freeReelCnt, reelsetColBitCnt, reelsetBitCnt)
            {
                freeoptionsymbol    = 0;
                curpoint            = curpoint + 2;
            }
        }
    }
}
