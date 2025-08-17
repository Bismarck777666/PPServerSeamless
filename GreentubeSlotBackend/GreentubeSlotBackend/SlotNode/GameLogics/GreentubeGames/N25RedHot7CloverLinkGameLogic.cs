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
    internal class N25RedHot7CloverLinkGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ13002ÿ1.10.54.0ÿApexClover2LinkServerÿ12568ÿ1.0.2ÿ25 Red Hot 7 Clover Link 94.6%ÿ1.81.0_2022-08-23_104940ÿ1.7.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49990,100.0,ÿM,10,10000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,25 Red Hot 7 Clover Link 94.6%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:l,^_-_^ÿ:l,^^_^^ÿ:l,__^__ÿ:l,-^_^-ÿ:l,-_^_-ÿ:l,-_-_-ÿ:l,-^-^-ÿ:l,^---^ÿ:l,_---_ÿ:l,^^-^^ÿ:l,__-__ÿ:l,^___^ÿ:l,_^^^_ÿ:l,----_ÿ:l,^-_-_ÿ:l,_-^-^ÿ:l,^^^^-ÿ:l,____-ÿ:l,^-^-_ÿ:l,_-_-^ÿ:l,--^^^ÿ:l,--___ÿ:l,-^^-_ÿ:l,-__-^ÿ:l,-^-__ÿ:l,-_-^^ÿ:l,_-^^-ÿ:l,^-__-ÿ:l,^^-_-ÿ:l,__-^-ÿ:r,0,SSSOOOPPPCCCMMMLLLOOOGGGCCCXLLLMMMOOOPPPCCCGGGOOOXLLLGGGCCCPPPOOOLLLPPPLLLOOOLLLPPPCCCXSSSOOOPPPCCCMMMLLLGGGCCCXLLLMMMOOOPPPCCCLLLGGGOOOXLLLGGGCCCPPPOOOLLLPPPLLLOOOLLLPPPCCCXSSSOOOPPPCCCMMMLLLGGGCCCXLLLMMMOOOPPPCCCGGGOOOXLLLGGGCCCPPPOOOLLLPPPLLLOOOMMMLLLPPPCCCXÿ:r,0,SSSOOOLLLMMMCCCGGGOOOXCCCPPPLLLGGGOOOXCCCGGGLLLMMMOOOLLLCCCOOOMMMPPPOOOMMMOOOLLLOOOLLLCCCXSSSLLLMMMCCCGGGOOOXCCCPPPLLLGGGOOOXCCCGGGLLLMMMOOOLLLCCCMMMPPPOOOMMMOOOLLLOOOLLLCCCXSSSLLLMMMCCCGGGOOOXCCCPPPLLLGGGOOOXCCCGGGLLLMMMOOOLLLCCCMMMPPPOOOCCCMMMOOOLLLOOOLLLCCCXÿ:r,0,SSSMMMCCCOOOGGGLLLXCCCPPPLLLGGGOOOXCCCMMMLLLGGGOOOPPPCCCMMMLLLPPPCCCMMMOOOCCCPPPOOOSSSMMMCCCOOOGGGLLLXCCCPPPLLLGGGOOOXCCCMMMLLLGGGOOOPPPCCCMMMLLLPPPCCCMMMOOOCCCPPPXOOOSSSMMMCCCOOOGGGLLLXCCCPPPLLLGGGOOOXCCCMMMLLLGGGOOOPPPCCCMMMLLLPPPCCCMMMOOOCCCPPPOOOÿ:r,0,SSSLLLMMMCCCGGGPPPOOOLLLXPPPCCCMMMLLLGGGOOOCCCLLLOOOGGGCCCPPPMMMOOOLLLCCCMMMPPPOOOCCCPPPXSSSLLLMMMCCCGGGPPPOOOXPPPCCCMMMLLLGGGOOOCCCLLLOOOPPPGGGCCCPPPMMMOOOLLLCCCMMMPPPOOOPPPXSSSLLLMMMCCCGGGPPPOOOXPPPCCCMMMLLLGGGOOOCCCLLLOOOGGGCCCPPPMMMOOOLLLCCCMMMPPPOOOPPPXÿ:r,0,SSSCCCMMMLLLPPPCCCXOOOPPPMMMGGGOOOPPPLLLMMMOOOGGGCCCMMMOOOLLLCCCPPPOOOCCCPPPLLLMMMXSSSCCCMMMLLLPPPCCCXOOOPPPMMMGGGOOOPPPLLLMMMOOOGGGCCCMMMOOOXLLLCCCPPPOOOCCCPPPLLLMMMXSSSCCCMMMLLLPPPCCCXOOOPPPMMMGGGOOOPPPLLLMMMOOOGGGPPPCCCMMMOOOLLLCCCPPPOOOCCCPPPLLLMMMXÿ:j,S,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,1,10,-100,0ÿ:w,1,9,-50,0ÿ:w,1,8,-20,0ÿ:w,1,7,-15,0ÿ:w,1,6,-10,0ÿ:w,1,5,-5,0ÿ:w,1,4,-4,0ÿ:w,1,3,-3,0ÿ:w,1,2,-2,0ÿ:w,1,1,-1,0ÿ:w,6,10,-100,0ÿ:w,6,9,-50,0ÿ:w,6,8,-20,0ÿ:w,6,7,-15,0ÿ:w,6,6,-10,0ÿ:w,6,5,-5,0ÿ:w,6,4,-4,0ÿ:w,6,3,-3,0ÿ:w,6,2,-2,0ÿ:w,6,1,-1,0ÿ:w,2,1,-20,0ÿ:w,3,1,-100,0ÿ:w,5,1,-20000,1:Grandÿ:w,4,1,-1000,1:Majorÿ:w,X,3,-5,0ÿ:w,X,4,-20,0ÿ:w,X,5,-500,0ÿ:w,S,3,50,0ÿ:w,S,4,500,0ÿ:w,S,5,1000,0ÿ:w,G,3,10,0ÿ:w,G,4,40,0ÿ:w,G,5,400,0ÿ:w,M,3,10,0ÿ:w,M,4,40,0ÿ:w,M,5,200,0ÿ:w,P,3,10,0ÿ:w,P,4,40,0ÿ:w,P,5,200,0ÿ:w,L,3,5,0ÿ:w,L,4,20,0ÿ:w,L,5,50,0ÿ:w,O,3,5,0ÿ:w,O,4,20,0ÿ:w,O,5,50,0ÿ:w,C,3,5,0ÿ:w,C,4,20,0ÿ:w,C,5,50,0ÿ:wa,1,10,-100,0,0,0,0:1,0,0ÿ:wa,1,9,-50,0,0,0,0:1,0,0ÿ:wa,1,8,-20,0,0,0,0:1,0,0ÿ:wa,1,7,-15,0,0,0,0:1,0,0ÿ:wa,1,6,-10,0,0,0,0:1,0,0ÿ:wa,1,5,-5,0,0,0,0:1,0,0ÿ:wa,1,4,-4,0,0,0,0:1,0,0ÿ:wa,1,3,-3,0,0,0,0:1,0,0ÿ:wa,1,2,-2,0,0,0,0:1,0,0ÿ:wa,1,1,-1,0,0,0,0:1,0,0ÿ:wa,6,10,-100,0,0,0,0:1,0,0ÿ:wa,6,9,-50,0,0,0,0:1,0,0ÿ:wa,6,8,-20,0,0,0,0:1,0,0ÿ:wa,6,7,-15,0,0,0,0:1,0,0ÿ:wa,6,6,-10,0,0,0,0:1,0,0ÿ:wa,6,5,-5,0,0,0,0:1,0,0ÿ:wa,6,4,-4,0,0,0,0:1,0,0ÿ:wa,6,3,-3,0,0,0,0:1,0,0ÿ:wa,6,2,-2,0,0,0,0:1,0,0ÿ:wa,6,1,-1,0,0,0,0:1,0,0ÿ:wa,2,1,-20,0,0,0,0:1,-1,0ÿ:wa,3,1,-100,0,0,0,0:1,-1,0ÿ:wa,5,1,-20000,1:Grand,0,0,0:1,0,0ÿ:wa,4,1,-1000,1:Major,0,0,0:1,0,0ÿ:wa,X,3,-5,0,0,0,0:1,-1,0ÿ:wa,X,4,-20,0,0,0,0:1,-1,0ÿ:wa,X,5,-500,0,0,0,0:1,-1,0ÿ:wa,S,3,50,0,0,0,0:1,-1,0ÿ:wa,S,4,500,0,0,0,0:1,-1,0ÿ:wa,S,5,1000,0,0,0,0:1,-1,0ÿ:wa,G,3,10,0,0,0,0:1,-1,0ÿ:wa,G,4,40,0,0,0,0:1,-1,0ÿ:wa,G,5,400,0,0,0,0:1,-1,0ÿ:wa,M,3,10,0,0,0,0:1,-1,0ÿ:wa,M,4,40,0,0,0,0:1,-1,0ÿ:wa,M,5,200,0,0,0,0:1,-1,0ÿ:wa,P,3,10,0,0,0,0:1,-1,0ÿ:wa,P,4,40,0,0,0,0:1,-1,0ÿ:wa,P,5,200,0,0,0,0:1,-1,0ÿ:wa,L,3,5,0,0,0,0:1,-1,0ÿ:wa,L,4,20,0,0,0,0:1,-1,0ÿ:wa,L,5,50,0,0,0,0:1,-1,0ÿ:wa,O,3,5,0,0,0,0:1,-1,0ÿ:wa,O,4,20,0,0,0,0:1,-1,0ÿ:wa,O,5,50,0,0,0,0:1,-1,0ÿ:wa,C,3,5,0,0,0,0:1,-1,0ÿ:wa,C,4,20,0,0,0,0:1,-1,0ÿ:wa,C,5,50,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,25ÿ:m,25ÿ:b,11,1,2,3,4,5,8,10,15,20,30,40ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿbs,0,1,0ÿ:k,nullÿe,25,0,0,0ÿb,1000,1000,0ÿ:x,1225ÿs,1ÿr,0,5,51,21,65,57,2ÿx,13,,-----0--00--00-,5:1:6:250|8:1:6:250|9:1:4:100|12:1:7:375|13:1:6:250|,3|6:65:35:64:30;2|0:39:20:58:11;1|64:83:0:70:18;,3,0,false,1,25,3,0,1:6:25:5:;1:6:25:8:;1:4:25:9:;1:7:25:12:;1:6:25:13:;,ÿc,0,5,0,0,0,0,,1222304041,0ÿc,1,6,0,0,0,0,0ÿc,1,6,0,0,0,0,0ÿc,1,4,0,0,0,0,0ÿc,1,7,0,0,0,0,0ÿc,1,6,0,0,0,0,0ÿrw,0";
            }
        }
        #endregion

        public N25RedHot7CloverLinkGameLogic()
        {
            _gameID = GAMEID.N25RedHot7CloverLink;
            GameName = "25RedHot7CloverLink";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,25,1000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,25ÿ:m,25ÿ:b,11,1,2,3,4,5,8,10,15,20,30,40ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,51,21,65,57,2ÿx,13,,-----0--00--00-,5:1:6:250|8:1:6:250|9:1:4:100|12:1:7:375|13:1:6:250|,3|6:65:35:64:30;2|0:39:20:58:11;1|64:83:0:70:18;,3,0,false,1,25,3,0,1:6:25:5:;1:6:25:8:;1:4:25:9:;1:7:25:12:;1:6:25:13:;,ÿc,0,5,0,0,0,0,,1222304041,0ÿc,1,6,0,0,0,0,0ÿc,1,6,0,0,0,0,0ÿc,1,4,0,0,0,0,0ÿc,1,7,0,0,0,0,0ÿc,1,6,0,0,0,0,0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
