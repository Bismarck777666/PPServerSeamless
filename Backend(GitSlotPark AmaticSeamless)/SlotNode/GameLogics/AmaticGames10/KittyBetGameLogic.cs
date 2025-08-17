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
    class KittyBetGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "KittyBet";
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
                return new long[] { 10, 20 };
            }
        }
        protected override int LineTypeCnt => 2;
        protected override string InitString
        {
            get
            {
                return "0522f0a275a8b953196546847389ba93641a62571672643756742462984a5839174035673829a1469485c672489c753a19485a6348c6950732a068c76357325a56a317cb8834806a38254b267c9a083a7c82975b1346a589423b71648a29c8594761b54693ac873574a65a908b2431a605ca857a47947583924681c9a2859742a60a9436047283a41a35862c791639562457085b392462872b69173a856a41a4681b982538a596428571a3583970a9437046942a52418169472300062571473895a89a3196486275a937641a629a2589a59867567489a24c58724a9c2ac6950758749564a5782a036829a19c763485a49a39167038c783a146948316432524d594361756a507a583a629c873574693a27916a38254267c9a08a3178465a4083498591648a297258604760a942563a85678a346093a432857a475862c79163926381c92738539561a41a3845974287947942847524da856a41a35718468573946285397465872694379a2638a596819a49642a73583957a460003a47030101010101010427101000112641410101010101010141401110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a264151010101010101010101010101010101010101010101fdefedfdfefededf1300013000130001300013000150000019000000000";
            }
        }
        protected override string ExtraString => "1300013000130001300013000150000019000000000";
        protected string ExtraKittyString => "1fdefedfdfefededf";
        #endregion

        public KittyBetGameLogic()
        {
            _gameID     = GAMEID.KittyBet;
            GameName    = "KittyBet";
        }

        protected override int getLineTypeFromPlayLine(int playline)
        {
            switch (playline)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                default:
                    return 0;
            }
        }
        protected override string buildInitString(string strUserID, double balance, Currencies currency)
        {
            AmaticEncrypt encrypt = new AmaticEncrypt();
            string initString = string.Empty;

            BaseAmaticExtraWildInitPacket initPacket = new BaseAmaticExtraWildInitPacket(InitString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum, ExtraString);
            initPacket.betstepamount    = BettingButton.ToList();
            initPacket.laststep         = 0;
            initPacket.minbet           = BettingButton[0];
            initPacket.maxbet           = BettingButton.ToList().Last() * LINES.ToList().Last();
            initPacket.lastline         = LINES.ToList().Last();

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

                initPacket.laststep = betInfo.PlayBet;
                initPacket.lastline = betInfo.PlayLine;
                initPacket.win      = 0;

                AmaticPacket amaPacket = new AmaticPacket(spinResult.ResultString, Cols, FreeCols);
                initPacket.messagetype      = amaPacket.messagetype;
                if ((AmaticMessageType)amaPacket.messagetype == AmaticMessageType.GamblePick || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.GambleHalf
                    || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastFree || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastPower)
                    initPacket.messagetype = (int)AmaticMessageType.NormalSpin;
                initPacket.reelstops        = amaPacket.reelstops;
                initPacket.freereelstops    = amaPacket.freereelstops;
                initPacket.gamblelogs       = amaPacket.gamblelogs;
                if (betInfo.HasRemainResponse || amaPacket.messagetype == (long)AmaticMessageType.FreeOption)
                {
                    if(amaPacket.messagetype == (long)AmaticMessageType.FreeTrigger || amaPacket.messagetype == (long)AmaticMessageType.FreeSpin || amaPacket.messagetype == (long)AmaticMessageType.ExtendFree)
                        initPacket.messagetype = (long)AmaticMessageType.FreeReopen;

                    if (amaPacket.messagetype == (long)AmaticMessageType.TriggerPower || amaPacket.messagetype == (long)AmaticMessageType.PowerRespin)
                        initPacket.messagetype = (long)AmaticMessageType.FreeSpin;

                    initPacket.totalfreecnt     = amaPacket.totalfreecnt;
                    initPacket.curfreecnt       = amaPacket.curfreecnt;
                    initPacket.curfreewin       = amaPacket.curfreewin;
                    initPacket.freeunparam1     = amaPacket.freeunparam1;
                    initPacket.freeunparam2     = amaPacket.freeunparam2;
                    initPacket.totalfreewin     = amaPacket.totalfreewin;

                    initPacket.linewins         = amaPacket.linewins;
                    initPacket.win              = amaPacket.win;

                    initPacket.reelstops        = amaPacket.freereelstops;
                }
            }

            initString = encrypt.WriteDecHex(initString, initPacket.messageheader);
            initString = encrypt.WriteDecHex(initString, initPacket.reelset.Count);
            for(int i = 0; i < initPacket.reelset.Count; i++)
            {
                if(ReelSetBitNum == 1)
                    initString = encrypt.Write1BitNumArray(initString, initPacket.reelset[i]);
                else if(ReelSetBitNum == 2)
                    initString = encrypt.Write2BitNumArray(initString, initPacket.reelset[i]);
            }
            
            initString = encrypt.WriteDecHex(initString, initPacket.freereelset.Count);
            for (int i = 0; i < initPacket.freereelset.Count; i++)
            {
                if(ReelSetBitNum == 1)
                    initString = encrypt.Write1BitNumArray(initString, initPacket.freereelset[i]);
                else if(ReelSetBitNum == 2)
                    initString = encrypt.Write2BitNumArray(initString, initPacket.freereelset[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.messagetype);
            initString = encrypt.WriteDecHex(initString, initPacket.sessionclose);

            int reelStopCnt = initPacket.reelstops.Count > 5 ? initPacket.reelstops.Count : 5;
            if(initPacket.reelstops.Count >= 5)
            {
                for (int i = 0; i < reelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.reelstops[i]);
            }
            else
            {
                for (int i = 0; i < initPacket.reelstops.Count; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.reelstops[i]);

                for (int i = initPacket.reelstops.Count; i < reelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, 0);
            }

            initString = encrypt.WriteLengthAndDec(initString, initPacket.messageid);

            double pointUnit = getPointUnit(new BaseAmaticSlotBetInfo() { CurrencyInfo = currency });
            long balanceUnit = (long)Math.Round(balance / pointUnit, 0);
            initString = encrypt.WriteLengthAndDec(initString, balanceUnit);        //현재 화페와 단위금액으로 변환된 발란스
            initString = encrypt.WriteLengthAndDec(initString, initPacket.win);     //당첨금(인이트의 경우에는 0)
            initString = encrypt.WriteDec2Hex(initString, initPacket.laststep);     //마지막스핀 스텝
            initString = encrypt.WriteLengthAndDec(initString, initPacket.minbet);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.maxbet);  
            initString = encrypt.WriteDec2Hex(initString, initPacket.lastline);     //마지막스핀 라인

            initString = encrypt.WriteLengthAndDec(initString, initPacket.totalfreecnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.curfreecnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.curfreewin);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.freeunparam1);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.freeunparam2);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.totalfreewin);

            initString = encrypt.WriteLengthAndDec(initString, initPacket.unknownparam1);
            initString = encrypt.WriteDec2Hex(initString, initPacket.minbetline);
            initString = encrypt.WriteDec2Hex(initString, initPacket.maxbetline);
            initString = encrypt.WriteDec2Hex(initString, initPacket.unitbetline);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.unknownparam2);
            initString = encrypt.WriteDec2Hex(initString, initPacket.unknownparam3);

            int freeReelStopCnt = initPacket.freereelstops.Count > 5 ? initPacket.freereelstops.Count : 5;
            if (initPacket.freereelstops.Count >= 5)
            {
                for (int i = 0; i < freeReelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.freereelstops[i]);
            }
            else
            {
                for (int i = 0; i < initPacket.freereelstops.Count; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.freereelstops[i]);

                for (int i = initPacket.freereelstops.Count; i < freeReelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, 0);
            }

            for (int i = 0; i < initPacket.gamblelogs.Count; i++)
            {
                initString = encrypt.WriteDec2Hex(initString, initPacket.gamblelogs[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.betstepamount.Count);
            for(int i = 0; i < initPacket.betstepamount.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.betstepamount[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.linewins.Count);
            for (int i = 0; i < initPacket.linewins.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.linewins[i]);
            }
            
            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo       betInfo     = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult    spinResult  = _dicUserResultInfos[strUserID];
                if (betInfo.HasRemainResponse)
                {
                    BaseAmaticExtraWildPacket amaPacket = new BaseAmaticExtraWildPacket(spinResult.ResultString, Cols, FreeCols);
                    initPacket.extrastr      = amaPacket.extrastr;
                }
            }

            initString = encrypt.WriteLeftHexString(initString, ExtraKittyString);
            initString = encrypt.WriteLeftHexString(initString, initPacket.extrastr);

            return initString;
        }
    }
}
