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
    internal class CyberWildzGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ898ÿ1.10.54.0ÿGTSKSlotServerÿ15197ÿ1.1.0ÿCyber Wildz 95%ÿ1.120.0_2025-05-09_075230ÿ1.313.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49960,100.0,ÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Cyber Wildz 95%ÿ:v,5ÿ:l,^^^^^^ÿ:l,------ÿ:l,______ÿ:l,VVVVVVÿ:l,******ÿ:l,^-_V_-ÿ:l,*V_-_Vÿ:l,-_V*V_ÿ:l,V_-^-_ÿ:l,-^-^-^ÿ:l,-_-_-_ÿ:l,_-_-_-ÿ:l,_V_V_Vÿ:l,V_V_V_ÿ:l,V*V*V*ÿ:r,0,BHCIDAAAAAIDGGHFWEHGEIADFBCGTCIIFDGWFBWFEGICFÿ:r,0,DIGFEAAAAAFCHHFGWDFHDCACHBEHFHEEIEGWHBWIEGHDGÿ:r,0,FGBIDAAAAAIDGGCIWDHGCFACIBEGFIHHFEHWIBWFEHIEDÿ:r,0,EHEFCAAAAAFDIIGHWEGIEIADHBDHFEIIGCFWHBWGCFHCGÿ:r,0,CHGCHAAAAAHEGGFHWDFGCIAGIBHEFIDDFEGWIBWFEGHDFÿ:r,0,CHGCHAAAAAHEGGFHWDFGCIAGIBHETIDDFEGWIBWFEGHDFÿ:r,1,IHCIDBBBBBIDGGHFWEHGEIADFFHGTCIFFDGWIGWFEGACFÿ:r,1,DIGFEBBBBBFCHHFGWDFHDCAIHHEHFHEHIEGWCIWIEGADGÿ:r,1,FGCIDBBBBBIDGGCIWDHGCFAEIIEGFIHIFEHWFGWFEHAEDÿ:r,1,EHEFCBBBBBFDIIGHWEGIEIADHHDHFEIHGDFWIFWGCFAIGÿ:r,1,CHGCHBBBBBHEGGFHWDFGCIAGIIHEFIDIFEGWIEWFEGADFÿ:r,1,CHGCHBBBBBHEGGFHWDFGCIAGIIHETIDIFEGWIEWFEGADFÿ:r,2,BHDIBCCCCCIBGGHFWEHGEIABFFDGTDIFFBGWDIWFEGAHFÿ:r,2,DIGFECCCCCFDHHFGWBFHBFADHHEHFHEHIEGWHEWIEGABGÿ:r,2,FGBIBCCCCCIBGGHIWBHGDFAEIIEGFIHIFEHWIHWFEHAEDÿ:r,2,EHEFHCCCCCFBIIGHWEGIEIABHHBHFDIHGHDWDIWGIFAIGÿ:r,2,IHGDHCCCCCHEGGFHWBFGDIAGIIHEFIBIFEGWIBWFEGABFÿ:r,2,IHGDHCCCCCHEGGFHWBFGDIAGIIHETIBIFEGWIBWFEGABFÿ:r,3,BHCIEDDDDDIEGGHFWBHGBIAEFFCGTCIFFHGWGBWFBGACFÿ:r,3,EIGFBDDDDDFCHHFGWCFHECACHHBHFHBHIBGWHEWIBGAEGÿ:r,3,FGBIEDDDDDIFHGCIWGHGCFACIIBGFIEIFBHWGCWFBHABEÿ:r,3,EHBFEDDDDDFBIIGHWBGIBIAEHHIHFHIHGCFWHCWGCFACGÿ:r,3,CHGCHDDDDDHBGGFHWGFGCIAEIIHBFIEIFBGWBGWFBGAEFÿ:r,3,CHGCHDDDDDHBGGFHWGFGCIAEIIHBTIEIFBGWBGWFBGAEFÿ:r,4,BHGFHEEEEEFHCGDBWIDGIDAHBFGCTGDFBHCWGCWBICAGFÿ:r,4,DICBIEEEEEBGDHBCWHBHHGAGDHIDFDIHFICWIDWFICAHGÿ:r,4,FGFBHEEEEECHCGGCWHDGGBAGFIICFFDIBIDWICWBIDAIDÿ:r,4,FHIBGEEEEEBHIICDWICIIFAHDHHDFIHHCGBWHDWCGBAGGÿ:r,4,CHCGDEEEEEDICGBDWHBGGDACGIDIFGHIBICWDIWBICAHFÿ:r,4,CHCGDEEEEEDICGBDWHBGGDACGIDITGHIBICWDIWBICAHFÿ:r,5,BHDEHFFFFFEHCGDBWIDGIEAHBFDCTGEFBHCWIGWBICAGEÿ:r,5,DICBIFFFFFBGDHBCWHBHHGAGBHIDFBIHEICWHDWEICAHGÿ:r,5,HGBEHFFFFFEHCGHEWHDGGBAGEIICFEDIBIDWGEWBIDAIDÿ:r,5,EHIBGFFFFFBHEICDWICIIEAHDHHDFIEHCGBWGIWCGBADGÿ:r,5,CHCGDFFFFFDICGBDWHBGGEACEIDIFEHIBICWCEWBICAHGÿ:r,5,CHCGDFFFFFDICGBDWHBGGEACEIDITEHIBICWCEWBICAHGÿ:r,6,BHDEFGGGGGEFCGDBWIDGIEAFBBHCTHEBBFCWIHWBICADFÿ:r,6,DICBIGGGGGCHDHBCWFBHFHAHBBIDFDIDEICWFDWEICAFHÿ:r,6,FHFEFGGGGGEFCGBEWFDGHBAIEEICFEDEBIDWHEWBIDAIDÿ:r,6,EHIBHGGGGGBFEICDWICIIEAFDDFDFIEDCHBWHIWCHBADFÿ:r,6,CHCHDGGGGGDICGBDWFBGHEACEEDIFEFEBICWCEWBICAFFÿ:r,6,CHCHDGGGGGDICGBDWFBGHEACEEDITEFEBICWCEWBICAFFÿ:r,7,BIGEIHHHHHEGCGDBWFDGFEAIBBGCTGEBBICWEIWBFCAGFÿ:r,7,DICBFHHHHHBGDHBCWCBHBGAGDDFCFBFDEFCWBFWEFCAGGÿ:r,7,FGFEIHHHHHEICGGEWBDGGBAGEEFCFEDEBFDWEIWBFDAFDÿ:r,7,EIFBGHHHHHBDEICDWFCIFEAIDDIDFFEDCGBWBGWCGBAGGÿ:r,7,CICGDHHHHHDFCGBDWGBGGEACEEDFFEGEBFCWGDWBFCAIFÿ:r,7,CICGDHHHHHDFCGBDWGBGGEACEEDFTEGEBFCWGDWBFCAIFÿ:r,8,BHGEHIIIIIEHCGDBWFDGFEAHBBGCTGEBBHCWFGWBFCAGFÿ:r,8,DHCBFIIIIIBGDHBCWHBHHGAGDBFDFCFBEFCWHDWEFCAHGÿ:r,8,FGFEHIIIIIEHCGGEWHDGGBAGEEFCFEDEBFDWGEWBFDAFDÿ:r,8,EHFBGIIIIIBHEICDWFCIFEAHDDHDFFEDCGBWGFWCGBAGGÿ:r,8,CHCGDIIIIIDFCGBDWHBGGEACEEDFFEHEBFCWCEWBFCAHFÿ:r,8,CHCGDIIIIIDFCGBDWHBGGEACEEDFTEHEBFCWCEWBFCAHFÿ:r,9,ONRPONNOJQOPPPKLQORRPQPQMOJNOOPLOOOLROJRLPQJQKRNLRLQRKOQJMOPMRPJQÿ:r,9,QRNRNNOROJOQQPKRPLQRPRRRMPJPPNLLPRRPJLRPPJLPRKLOPQLQNKNJMPOPMOQJOÿ:r,9,QOPOOPJNRRKQNQPRORPLRRNMOJJQOPLQNLQNJOPQQJLQOKOQLQQKLORMJOORJQMNNÿ:r,9,QQQNNJNPNRPNQNKQPRQLOMNQPJPQJPLLPRPQLJRPLQQJQQKPNPPLKQOPMJNRMPPJPÿ:r,9,PNPNPNROJQPNRKRNPPLQMQNNPNJNONLRLROJPNONQLJQQQKLNPKLPPPMQJNPQMJQRÿ:r,9,PPPQQRJRPRKRQNPQLRPOOORRMPJRRQLROLOLNNJNLJNOPKQNLOKLONQJMPPNMRQJPÿ:j,2,1,0,ÿ:j,3,1,0,ÿ:j,5,1,0,ÿ:j,W,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,T,2,-1,0ÿ:w,L,1,-20000,0ÿ:w,A,3,40,0ÿ:w,A,4,140,0ÿ:w,A,5,240,0ÿ:w,A,6,1000,0ÿ:w,B,3,20,0ÿ:w,B,4,40,0ÿ:w,B,5,100,0ÿ:w,B,6,250,0ÿ:w,C,3,20,0ÿ:w,C,4,40,0ÿ:w,C,5,100,0ÿ:w,C,6,250,0ÿ:w,D,3,20,0ÿ:w,D,4,40,0ÿ:w,D,5,100,0ÿ:w,D,6,250,0ÿ:w,E,3,3,0ÿ:w,E,4,6,0ÿ:w,E,5,20,0ÿ:w,E,6,100,0ÿ:w,F,3,3,0ÿ:w,F,4,6,0ÿ:w,F,5,20,0ÿ:w,F,6,100,0ÿ:w,G,3,3,0ÿ:w,G,4,6,0ÿ:w,G,5,20,0ÿ:w,G,6,100,0ÿ:w,H,3,3,0ÿ:w,H,4,6,0ÿ:w,H,5,20,0ÿ:w,H,6,100,0ÿ:w,I,3,3,0ÿ:w,I,4,6,0ÿ:w,I,5,20,0ÿ:w,I,6,100,0ÿ:wa,T,2,-1,0,0,0,0:1,-1,0ÿ:wa,L,1,-20000,0,0,0,0:1,-1,0ÿ:wa,A,3,40,0,0,0,0:1,-1,0ÿ:wa,A,4,140,0,0,0,0:1,-1,0ÿ:wa,A,5,240,0,0,0,0:1,-1,0ÿ:wa,A,6,1000,0,0,0,0:1,-1,0ÿ:wa,B,3,20,0,0,0,0:1,-1,0ÿ:wa,B,4,40,0,0,0,0:1,-1,0ÿ:wa,B,5,100,0,0,0,0:1,-1,0ÿ:wa,B,6,250,0,0,0,0:1,-1,0ÿ:wa,C,3,20,0,0,0,0:1,-1,0ÿ:wa,C,4,40,0,0,0,0:1,-1,0ÿ:wa,C,5,100,0,0,0,0:1,-1,0ÿ:wa,C,6,250,0,0,0,0:1,-1,0ÿ:wa,D,3,20,0,0,0,0:1,-1,0ÿ:wa,D,4,40,0,0,0,0:1,-1,0ÿ:wa,D,5,100,0,0,0,0:1,-1,0ÿ:wa,D,6,250,0,0,0,0:1,-1,0ÿ:wa,E,3,3,0,0,0,0:1,-1,0ÿ:wa,E,4,6,0,0,0,0:1,-1,0ÿ:wa,E,5,20,0,0,0,0:1,-1,0ÿ:wa,E,6,100,0,0,0,0:1,-1,0ÿ:wa,F,3,3,0,0,0,0:1,-1,0ÿ:wa,F,4,6,0,0,0,0:1,-1,0ÿ:wa,F,5,20,0,0,0,0:1,-1,0ÿ:wa,F,6,100,0,0,0,0:1,-1,0ÿ:wa,G,3,3,0,0,0,0:1,-1,0ÿ:wa,G,4,6,0,0,0,0:1,-1,0ÿ:wa,G,5,20,0,0,0,0:1,-1,0ÿ:wa,G,6,100,0,0,0,0:1,-1,0ÿ:wa,H,3,3,0,0,0,0:1,-1,0ÿ:wa,H,4,6,0,0,0,0:1,-1,0ÿ:wa,H,5,20,0,0,0,0:1,-1,0ÿ:wa,H,6,100,0,0,0,0:1,-1,0ÿ:wa,I,3,3,0,0,0,0:1,-1,0ÿ:wa,I,4,6,0,0,0,0:1,-1,0ÿ:wa,I,5,20,0,0,0,0:1,-1,0ÿ:wa,I,6,100,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,15ÿ:m,20ÿ:b,12,1,2,3,4,5,8,10,15,20,30,40,50ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,20,0,0,0ÿb,1000,1000,0ÿs,1ÿr,8,6,7,31,17,18,18,16ÿx,1,w:3|2ÿrw,0";
            }
        }
        #endregion

        public CyberWildzGameLogic()
        {
            _gameID = GAMEID.CyberWildz;
            GameName = "CyberWildz";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,1000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,15ÿ:m,20ÿ:b,12,1,2,3,4,5,8,10,15,20,30,40,50ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,8,6,7,31,17,18,18,16ÿx,1,w:3|2ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
