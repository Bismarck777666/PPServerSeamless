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
    internal class N50RedHotBurningCloverLinkGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol
        {
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ13002ÿ1.10.54.0ÿApexClover2LinkServerÿ12704ÿ1.0.2ÿ50 Red Hot Burning Clover Link CC 94.5%ÿ1.81.0_2022-08-23_104940ÿ1.7.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49925,100.0,ÿM,10,10000,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,50 Red Hot Burning Clover Link CC 94.5%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:l,^_-_^ÿ:l,^^_^^ÿ:l,__^__ÿ:l,-^_^-ÿ:l,-_^_-ÿ:l,-_-_-ÿ:l,-^-^-ÿ:l,^---^ÿ:l,_---_ÿ:l,^^-^^ÿ:l,__-__ÿ:l,^___^ÿ:l,_^^^_ÿ:l,----_ÿ:l,^-_-_ÿ:l,_-^-^ÿ:l,^^^^-ÿ:l,____-ÿ:l,^-^-_ÿ:l,_-_-^ÿ:l,--^^^ÿ:l,--___ÿ:l,-^^-_ÿ:l,-__-^ÿ:l,-^-__ÿ:l,-_-^^ÿ:l,_-^^-ÿ:l,^-__-ÿ:l,^^-_-ÿ:l,__-^-ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:l,^_-_^ÿ:l,^^_^^ÿ:l,__^__ÿ:l,-^_^-ÿ:l,-_^_-ÿ:l,-_-_-ÿ:l,-^-^-ÿ:l,^---^ÿ:l,_---_ÿ:l,^^-^^ÿ:l,__-__ÿ:l,^___^ÿ:l,_^^^_ÿ:l,----_ÿ:l,^-_-_ÿ:l,_-^-^ÿ:l,^^^^-ÿ:l,____-ÿ:l,^-^-_ÿ:l,_-_-^ÿ:l,--^^^ÿ:l,--___ÿ:l,-^^-_ÿ:l,-__-^ÿ:l,-^-__ÿ:l,-_-^^ÿ:l,_-^^-ÿ:l,^-__-ÿ:l,^^-_-ÿ:l,__-^-ÿ:r,0,LLLBSMOOOOGXCCCBDMGPPPPBLLLMGCCCSPPPXOOODÿ:r,0,LLLSOOOMBWMBPPPBSGCCCOOOGMPPPPLLLBDGMCCCSGLLLPPPDOOOOCCCÿ:r,0,OOOBSCCCCXLLLBGWBGXPPPMDGLLLOOOOSXMPPPPMXBCCCLLLLOOOXPPPCCCÿ:r,0,LLMSGPPPOOOBSGLLLCCCOOOSMBWMBPPPLLLOOOCCCPPPBDGCCLÿ:r,0,LLLBSGOOOOXGCCCGDBPPPLLLLOOOOBSMPPPMDCCCXMPPSCCLÿ:r,1,LLLBSMOOOOGXCCCBDMGPPPPBLLLMGCCCSPPPXOOODÿ:r,1,LLLSOOOMBWMBPPPBSGCCCOOOGMPPPPLLLBDGMCCCSGLLLPPPDOOOOCCCÿ:r,1,OOOBSCCCCXLLLBGWBGXPPPMDGLLLOOOOSXMPPPPMXBCCCLLLLOOOXPPPCCCÿ:r,1,LLMSGPPPOOOBSGLLLCCCOOOSMBWMBPPPLLLOOOCCCPPPBDGCCLÿ:r,1,LLLBSGOOOOXGCCCGDBPPPLLLLOOOOBSMPPPMDCCCXMPPSCCLÿ:j,W,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,1,10,-100,0ÿ:w,1,9,-50,0ÿ:w,1,8,-20,0ÿ:w,1,7,-15,0ÿ:w,1,6,-10,0ÿ:w,1,5,-5,0ÿ:w,1,4,-4,0ÿ:w,1,3,-3,0ÿ:w,1,2,-2,0ÿ:w,1,1,-1,0ÿ:w,6,10,-100,0ÿ:w,6,9,-50,0ÿ:w,6,8,-20,0ÿ:w,6,7,-15,0ÿ:w,6,6,-10,0ÿ:w,6,5,-5,0ÿ:w,6,4,-4,0ÿ:w,6,3,-3,0ÿ:w,6,2,-2,0ÿ:w,6,1,-1,0ÿ:w,2,1,-20,0ÿ:w,3,1,-100,0ÿ:w,5,1,-20000,1:Grandÿ:w,4,1,-1000,1:Majorÿ:w,D,3,-2,0ÿ:w,D,4,-10,0ÿ:w,D,5,-100,0ÿ:w,S,2,5,0ÿ:w,S,3,20,0ÿ:w,S,4,100,0ÿ:w,S,5,1500,0ÿ:w,G,3,15,0ÿ:w,G,4,50,0ÿ:w,G,5,250,0ÿ:w,M,3,15,0ÿ:w,M,4,50,0ÿ:w,M,5,250,0ÿ:w,B,3,10,0ÿ:w,B,4,25,0ÿ:w,B,5,100,0ÿ:w,C,3,5,0ÿ:w,C,4,15,0ÿ:w,C,5,50,0ÿ:w,L,3,5,0ÿ:w,L,4,15,0ÿ:w,L,5,50,0ÿ:w,O,3,5,0ÿ:w,O,4,15,0ÿ:w,O,5,50,0ÿ:w,P,3,5,0ÿ:w,P,4,15,0ÿ:w,P,5,50,0ÿ:wa,1,10,-100,0,0,0,0:1,-1,0ÿ:wa,1,9,-50,0,0,0,0:1,-1,0ÿ:wa,1,8,-20,0,0,0,0:1,-1,0ÿ:wa,1,7,-15,0,0,0,0:1,-1,0ÿ:wa,1,6,-10,0,0,0,0:1,-1,0ÿ:wa,1,5,-5,0,0,0,0:1,-1,0ÿ:wa,1,4,-4,0,0,0,0:1,-1,0ÿ:wa,1,3,-3,0,0,0,0:1,-1,0ÿ:wa,1,2,-2,0,0,0,0:1,-1,0ÿ:wa,1,1,-1,0,0,0,0:1,-1,0ÿ:wa,6,10,-100,0,0,0,0:1,-1,0ÿ:wa,6,9,-50,0,0,0,0:1,-1,0ÿ:wa,6,8,-20,0,0,0,0:1,-1,0ÿ:wa,6,7,-15,0,0,0,0:1,-1,0ÿ:wa,6,6,-10,0,0,0,0:1,-1,0ÿ:wa,6,5,-5,0,0,0,0:1,-1,0ÿ:wa,6,4,-4,0,0,0,0:1,-1,0ÿ:wa,6,3,-3,0,0,0,0:1,-1,0ÿ:wa,6,2,-2,0,0,0,0:1,-1,0ÿ:wa,6,1,-1,0,0,0,0:1,-1,0ÿ:wa,2,1,-20,0,0,0,0:1,-1,0ÿ:wa,3,1,-100,0,0,0,0:1,-1,0ÿ:wa,5,1,-20000,1:Grand,0,0,0:1,0,0ÿ:wa,4,1,-1000,1:Major,0,0,0:1,0,0ÿ:wa,X,3,-25,0,0,0,0:1,1,0ÿ:wa,D,3,-2,0,0,0,0:1,0,0ÿ:wa,D,4,-10,0,0,0,0:1,0,0ÿ:wa,D,5,-100,0,0,0,0:1,0,0ÿ:wa,S,2,5,0,0,0,0:1,0,0ÿ:wa,S,3,20,0,0,0,0:1,0,0ÿ:wa,S,4,100,0,0,0,0:1,0,0ÿ:wa,S,5,1500,0,0,0,0:1,0,0ÿ:wa,G,3,15,0,0,0,0:1,0,0ÿ:wa,G,4,50,0,0,0,0:1,0,0ÿ:wa,G,5,250,0,0,0,0:1,0,0ÿ:wa,M,3,15,0,0,0,0:1,0,0ÿ:wa,M,4,50,0,0,0,0:1,0,0ÿ:wa,M,5,250,0,0,0,0:1,0,0ÿ:wa,B,3,10,0,0,0,0:1,0,0ÿ:wa,B,4,25,0,0,0,0:1,0,0ÿ:wa,B,5,100,0,0,0,0:1,0,0ÿ:wa,C,3,5,0,0,0,0:1,0,0ÿ:wa,C,4,15,0,0,0,0:1,0,0ÿ:wa,C,5,50,0,0,0,0:1,0,0ÿ:wa,L,3,5,0,0,0,0:1,0,0ÿ:wa,L,4,15,0,0,0,0:1,0,0ÿ:wa,L,5,50,0,0,0,0:1,0,0ÿ:wa,O,3,5,0,0,0,0:1,0,0ÿ:wa,O,4,15,0,0,0,0:1,0,0ÿ:wa,O,5,50,0,0,0,0:1,0,0ÿ:wa,P,3,5,0,0,0,0:1,0,0ÿ:wa,P,4,15,0,0,0,0:1,0,0ÿ:wa,P,5,50,0,0,0,0:1,0,0ÿ:s,0ÿ:i,50ÿ:m,50ÿ:b,9,1,2,3,4,5,8,10,15,20ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿbs,0,1,0ÿ:k,nullÿe,50,0,0,0ÿb,1000,1000,0ÿ:x,75ÿs,1ÿr,0,5,20,7,17,16,9ÿx,22,,---------------,,,,0,false,1,50,0,0,,,,,S;G;M;P;L;O;C;B;,,false,0,,,-1ÿl,P,3,2,0,0,0,0,0,PWP--,0ÿl,P,3,3,0,0,0,0,0,PWP--,0ÿl,P,3,5,0,0,0,0,0,PWP--,0ÿl,P,3,14,0,0,0,0,0,PWP--,0ÿl,P,3,16,0,0,0,0,0,PWP--,0ÿl,P,3,18,0,0,0,0,0,PWP--,0ÿl,P,3,21,0,0,0,0,0,PWP--,0ÿl,P,3,23,0,0,0,0,0,PWP--,0ÿl,P,3,31,0,0,0,0,0,PWP--,0ÿl,P,3,34,0,0,0,0,0,PWP--,0ÿl,P,3,37,0,0,0,0,0,PWP--,0ÿl,P,3,39,0,0,0,0,0,PWP--,0ÿl,P,3,41,0,0,0,0,0,PWP--,0ÿl,P,3,43,0,0,0,0,0,PWP--,0ÿl,P,3,47,0,0,0,0,0,PWP--,0ÿrw,0";
            }
        }
        #endregion

        public N50RedHotBurningCloverLinkGameLogic()
        {
            _gameID = GAMEID.N50RedHotBurningCloverLink;
            GameName = "50RedHotBurningCloverLink";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,50,1000,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,50ÿ:m,50ÿ:b,9,1,2,3,4,5,8,10,15,20ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,20,7,17,16,9ÿx,22,,---------------,,,,0,false,1,50,0,0,,,,,S;G;M;P;L;O;C;B;,,false,0,,,-1ÿl,P,3,2,0,0,0,0,0,PWP--,0ÿl,P,3,3,0,0,0,0,0,PWP--,0ÿl,P,3,5,0,0,0,0,0,PWP--,0ÿl,P,3,14,0,0,0,0,0,PWP--,0ÿl,P,3,16,0,0,0,0,0,PWP--,0ÿl,P,3,18,0,0,0,0,0,PWP--,0ÿl,P,3,21,0,0,0,0,0,PWP--,0ÿl,P,3,23,0,0,0,0,0,PWP--,0ÿl,P,3,31,0,0,0,0,0,PWP--,0ÿl,P,3,34,0,0,0,0,0,PWP--,0ÿl,P,3,37,0,0,0,0,0,PWP--,0ÿl,P,3,39,0,0,0,0,0,PWP--,0ÿl,P,3,41,0,0,0,0,0,PWP--,0ÿl,P,3,43,0,0,0,0,0,PWP--,0ÿl,P,3,47,0,0,0,0,0,PWP--,0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
