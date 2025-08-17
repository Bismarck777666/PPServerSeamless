using GITProtocol;
using SlotGamesNode.GameLogics.BaseClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class MegaJokerGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
            get
            {
                return "";
            }
        }

        protected override int MinCountToFreespin
        {
            get
            {
                return 0;
            }
        }
        protected override string VersionCheckString
        {
            get
            {
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ498ÿ1.0.0ÿMega Joker 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Mega Joker 95%ÿ:v,4ÿ:l,^^^^^ÿ:l,-----ÿ:l,_____ÿ:l,VVVVVÿ:l,^-_-^ÿ:l,-_V_-ÿ:l,_-^-_ÿ:l,V_-_Vÿ:l,^^^^-ÿ:l,----^ÿ:l,____Vÿ:l,VVVV_ÿ:l,^----ÿ:l,----_ÿ:l,____-ÿ:l,V____ÿ:l,-^^^^ÿ:l,-____ÿ:l,_----ÿ:l,_VVVVÿ:l,^^^-_ÿ:l,---_Vÿ:l,___-^ÿ:l,VVV_-ÿ:l,^-___ÿ:l,-_VVVÿ:l,_-^^^ÿ:l,V_---ÿ:l,^^-^^ÿ:l,--^--ÿ:l,__V__ÿ:l,VV_VVÿ:l,^---^ÿ:l,--_--ÿ:l,__-__ÿ:l,V___Vÿ:l,-^^^-ÿ:l,-___-ÿ:l,_---_ÿ:l,_VVV_ÿ:r,0,JJJJOOMEECCCRLLYYG#OCCCCSSLL#PPGGEEOOGGMMRRLLSSMMCCLLPPCCLLCCELLSPMLJJJJOOEECCCRLLYYG#OCCCCSSLL#PPGGEEOOGGMMRRLLSSMMCCLLPPCCLLCCELLSPMLJJJJOOEECCCRLLYYG#OCCCCSSLL#PPGGEEOOGGMMRRLLSSMMCCLLPPCCLLCCELLSPMLJJJJOOEECCCRLLYYG#LOCCCCSSLL#PPGGEEOOGGMMRRLLSSMMCCGLLPPCCLLCCELLSPMLÿ:r,0,SOOOCCRPPYJJJJJCCMMPPSSPPRRGGMMEECCLL#MMGGCCOOYLLMMSGGYCCGGEOOOSSELLSOOORPPYJJJJJCCPPSSPPEERRGGMMEECCLL#MMCCOOYLLMMSGGYGGEOOOSSELLSOOORPPYSSJJJJJCCPPSSPPRRGGMMEECCLL#MMCCOOYLLMMRSGGYGGEOOOSSELLSOOORPPYJJJJJGGCCPPSSPPRRGGMMEECCLL#MMCCOOYLLMMSGGYGGEOOOSSELLÿ:r,0,EEMMSSGGYYLCRRPPEEGG#OOEECSSORRPYYOORRPERRPSJJJJOEEPPYYOEEMMSSGGYYLCRRPPEEGG#OOEECSSORRPYYOORRPECRRPSJJJJOEEPPYYOEEMMSSGGYYLCRRPPEEGG#OOEECSSORRPYYOORRPERRPSJJJJOEEPPYYOEEMMSSGGYYLCRRPPEEGG#OORREECSSORRPYYOORRPERRPSJJJJOEEPPYYOÿ:r,0,YYLLEEOORRGJJJJJPPPYCSSMMYYOSSL#CCYYLLEEOORRGJJJJJPPPYCSSMMYYOSSL#CCYYLLEEOORRGJJJJJPPPYCSSMMYYOSSL#CCYYLLEEOORRGJJJJJPPPYCSSMMYYOSSL#CCÿ:r,0,GG#GYYOEEMMPPRROOSSGJJJJCCYLL#MMEGG#GYYOEEMMPPRROOSSGJJJJCCYLL#MMEGG#GYYOEEMMPPRROOSSGJJJJCCYOLL#MMEGG#GYYOEEMMPPRROOSSGJJJJCCYLL#MMEÿ:j,J,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,#,3,-4,0ÿ:w,#,4,-20,0ÿ:w,#,5,-400,0ÿ:w,J,3,40,0ÿ:w,J,4,200,0ÿ:w,J,5,2000,0ÿ:w,R,3,20,0ÿ:w,R,4,80,0ÿ:w,R,5,400,0ÿ:w,Y,3,20,0ÿ:w,Y,4,80,0ÿ:w,Y,5,400,0ÿ:w,E,3,20,0ÿ:w,E,4,60,0ÿ:w,E,5,240,0ÿ:w,S,3,20,0ÿ:w,S,4,60,0ÿ:w,S,5,240,0ÿ:w,G,3,8,0ÿ:w,G,4,40,0ÿ:w,G,5,160,0ÿ:w,M,3,8,0ÿ:w,M,4,40,0ÿ:w,M,5,160,0ÿ:w,O,3,4,0ÿ:w,O,4,20,0ÿ:w,O,5,100,0ÿ:w,P,3,4,0ÿ:w,P,4,20,0ÿ:w,P,5,100,0ÿ:w,L,3,4,0ÿ:w,L,4,20,0ÿ:w,L,5,100,0ÿ:w,C,3,4,0ÿ:w,C,4,20,0ÿ:w,C,5,100,0ÿ:wa,#,3,-4,0,0,0,0:1,-1,0ÿ:wa,#,4,-20,0,0,0,0:1,-1,0ÿ:wa,#,5,-400,0,0,0,0:1,-1,0ÿ:wa,J,3,40,0,0,0,0:1,-1,0ÿ:wa,J,4,200,0,0,0,0:1,-1,0ÿ:wa,J,5,2000,0,0,0,0:1,-1,0ÿ:wa,R,3,20,0,0,0,0:1,-1,0ÿ:wa,R,4,80,0,0,0,0:1,-1,0ÿ:wa,R,5,400,0,0,0,0:1,-1,0ÿ:wa,Y,3,20,0,0,0,0:1,-1,0ÿ:wa,Y,4,80,0,0,0,0:1,-1,0ÿ:wa,Y,5,400,0,0,0,0:1,-1,0ÿ:wa,E,3,20,0,0,0,0:1,-1,0ÿ:wa,E,4,60,0,0,0,0:1,-1,0ÿ:wa,E,5,240,0,0,0,0:1,-1,0ÿ:wa,S,3,20,0,0,0,0:1,-1,0ÿ:wa,S,4,60,0,0,0,0:1,-1,0ÿ:wa,S,5,240,0,0,0,0:1,-1,0ÿ:wa,G,3,8,0,0,0,0:1,-1,0ÿ:wa,G,4,40,0,0,0,0:1,-1,0ÿ:wa,G,5,160,0,0,0,0:1,-1,0ÿ:wa,M,3,8,0,0,0,0:1,-1,0ÿ:wa,M,4,40,0,0,0,0:1,-1,0ÿ:wa,M,5,160,0,0,0,0:1,-1,0ÿ:wa,O,3,4,0,0,0,0:1,-1,0ÿ:wa,O,4,20,0,0,0,0:1,-1,0ÿ:wa,O,5,100,0,0,0,0:1,-1,0ÿ:wa,P,3,4,0,0,0,0:1,-1,0ÿ:wa,P,4,20,0,0,0,0:1,-1,0ÿ:wa,P,5,100,0,0,0,0:1,-1,0ÿ:wa,L,3,4,0,0,0,0:1,-1,0ÿ:wa,L,4,20,0,0,0,0:1,-1,0ÿ:wa,L,5,100,0,0,0,0:1,-1,0ÿ:wa,C,3,4,0,0,0,0:1,-1,0ÿ:wa,C,4,20,0,0,0,0:1,-1,0ÿ:wa,C,5,100,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,11ÿ:i,12ÿ:i,13ÿ:i,14ÿ:i,15ÿ:i,16ÿ:i,17ÿ:i,18ÿ:i,19ÿ:i,20ÿ:i,21ÿ:i,22ÿ:i,23ÿ:i,24ÿ:i,25ÿ:i,26ÿ:i,27ÿ:i,28ÿ:i,29ÿ:i,30ÿ:i,31ÿ:i,32ÿ:i,33ÿ:i,34ÿ:i,35ÿ:i,36ÿ:i,37ÿ:i,38ÿ:i,39ÿ:i,40ÿ:m,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,40,0,39,-1ÿb,0,0,0ÿs,1ÿr,0,5,14,46,38,9,16ÿrw,0";
            }
        }
        #endregion

        public MegaJokerGameLogic()
        {
            _gameID = GAMEID.MegaJoker;
            GameName = "MegaJoker";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,2000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,11ÿ:i,12ÿ:i,13ÿ:i,14ÿ:i,15ÿ:i,16ÿ:i,17ÿ:i,18ÿ:i,19ÿ:i,20ÿ:i,21ÿ:i,22ÿ:i,23ÿ:i,24ÿ:i,25ÿ:i,26ÿ:i,27ÿ:i,28ÿ:i,29ÿ:i,30ÿ:i,31ÿ:i,32ÿ:i,33ÿ:i,34ÿ:i,35ÿ:i,36ÿ:i,37ÿ:i,38ÿ:i,39ÿ:i,40ÿ:m,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40ÿ:b,12,1,2,3,4,5,8,10,15,20,30,40,50ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,14,46,38,9,16ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
        }

        protected override void onDoInit(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {
            try
            {
                GITMessage resMessage = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_DOINIT);
                resMessage.Append("%t0ÿq0");
                resMessage.Append(VersionCheckString);
                foreach (string currencyName in SupportCurrencyList)
                {
                    resMessage.Append(currencyName);
                }
                resMessage.Append(buildInitString(strGlobalUserID, balance, currency));
                resMessage.Append("\vGAMEVARIATIONÿ665ÿTICKETSÿ1");
                resMessage.Append(GameUniqueString);

                resMessage.Append("%t0ÿq0");
                resMessage.Append($"\aNATIVEJACKPOTCURRENCYFACTORÿ100.0ÿMAXBUYINTOTALÿ0ÿNEEDSSESSIONTIMEOUTÿ0ÿRELIABILITYÿ0ÿNICKNAMEÿ155258EURÿIDÿ155258ÿISDEEPWALLETÿ0ÿCURRENCYFACTORÿ100.0ÿMINBUYINÿ1ÿSESSIONTIMEOUTMINUTESÿ60ÿJACKPOT_PROGRESSIVE_CAPÿ0.0ÿNATIVEJACKPOTCURRENCYÿ{getCurrencyCode(currency)}ÿEXTUSERIDÿDummyDB_ExternalUserId_155258EURÿFREESPINSISCAPREACHEDÿ0ÿCURRENCYÿ{getCurrencySymbol(currency)}ÿTICKETSÿ0ÿJACKPOT_TOTAL_CAPÿ0.0ÿJACKPOTFEEPERCENTAGEÿ0.0ÿISSHOWJACKPOTCONTRIBUTIONÿ0");
                resMessage.Append(buildLineBetString(strGlobalUserID, balance));

                ToUserMessage toUserMessage = new ToUserMessage((int)_gameID, resMessage);

                Sender.Tell(toUserMessage, Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected override void onBalanceConfirm(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {
            try
            {
                GITMessage resMessage = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_BALANCECONFIRM);
                resMessage.Append("%t0ÿq0");
                resMessage.Append($"\aNATIVEJACKPOTCURRENCYFACTORÿ100.0ÿMAXBUYINTOTALÿ0ÿNEEDSSESSIONTIMEOUTÿ0ÿRELIABILITYÿ0ÿNICKNAMEÿ155258EURÿIDÿ155258ÿISDEEPWALLETÿ0ÿCURRENCYFACTORÿ100.0ÿMINBUYINÿ1ÿSESSIONTIMEOUTMINUTESÿ60ÿJACKPOT_PROGRESSIVE_CAPÿ0.0ÿNATIVEJACKPOTCURRENCYÿ{getCurrencyCode(currency)}ÿEXTUSERIDÿDummyDB_ExternalUserId_155258EURÿFREESPINSISCAPREACHEDÿ0ÿCURRENCYÿ{getCurrencySymbol(currency)}ÿTICKETSÿ0ÿJACKPOT_TOTAL_CAPÿ0.0ÿJACKPOTFEEPERCENTAGEÿ0.0ÿISSHOWJACKPOTCONTRIBUTIONÿ0");
                resMessage.Append(buildLineBetString(strGlobalUserID, balance));
                ToUserMessage toUserMessage = new ToUserMessage((int)_gameID, resMessage);

                Sender.Tell(toUserMessage, Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
    }
}
