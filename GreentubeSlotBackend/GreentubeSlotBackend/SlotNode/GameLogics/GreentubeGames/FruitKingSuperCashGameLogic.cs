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
    internal class FruitKingSuperCashGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino - Lobbymanager - Serverÿ936ÿ1.10.54.0ÿEuroCoinInteractiveSlotServerÿ17648ÿ1.1.0ÿFruit King Super Cash 95 % ÿ1.114.0_2024 - 12 - 13_114711ÿ1.247.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Fruit King Super Cash 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,CCOOCCCOCXXXLRRRLLLELLTTGTGTBBPMPMPPPOCCOOLLLOCEEECCCRPPPXXXTTGTGTBBPMPMLLLOÿ:r,0,LLLRRGRGGXXXTPTPTTWWWWCCPPPMPCCCBBPBPMOOOMOEEELLLRRGRGGWWWWTPTPTTXXXCCPPPMPCCCBBPBPMOOOMOLLEÿ:r,0,MOMMRRLLLXXXGGGCGCCCBCCBWWWWBEEELRLLLPTPPPTTOOOMOMMELLLWWWWGGGCGCCCBCCBXXXBRRLRLLLPTPPPTTOOOÿ:r,0,GGOGOOOGOXXXBBPPBPPPRRLRLLLELLWWWWCMCCCMTMTTTTGGOGOOOGOPPPBBPPBXXXRRLRLLLEEECMCCCMTMTTTTWWWWÿ:r,0,OTTTTMMTMXXXPPRPRPGRGGGCGCCCBCBBWWWWLLLLOEEEOOOTTTTMMTMXXXPPRPRPGRGGGCGCCCBCBBWWWWLLLLOOOEOOÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,1,XLTPOGBCMERWXLTPOGBCMERWXLTPOGBCMERWÿ:r,2,CCOOCCCOCLRRRLLLELLTTGCTGTBBPTMPMPPPOCCOOLLLOCEEECCCGRPPPTTGTGTBBPMGPMLLLOCCOOCCCOCLGRRRLLLELLTTGTGTBBPMOOPMPPPOCCOOLLLOCEEECCCRPPPMTTGTPGTBBPMPGMLLLOCCOOCCCOCLRRRLLLELLMTTGTGTBBGPMPMPPPOCCOOCCLLLOCEEEPPPCCCRPPPTTOOGTGTBBLLPMPMLLLOÿ:r,2,LLLRRGRGGPTPOTTWWWWCCPPPMPCCCBBPBPMOOOLLLMOEEELLLRRGRCCGGWWWWTPTOOOPTTCCPPPMPTTCCCBBPBPCCMOOOMOLLELLLRRBBGRGGPTPTTMWWWWCCPPPMBPCCCBBPBPMOOOMOEEELLLRRGRGGWWWWTPTPTTGCCPPPMPCCCBBPBPMGOOOMOLLELLLRRGROGGPTPTTWWWWCCBPPPMPCCCBBPBPMOOOMOEEELLLTTRRGRGGWWWWTPTPTTCCGGPPPMPCCCBBPBPMOOOMOLLEÿ:r,2,MOMMRRLLLGGGOCGCCCBCCBWWWWBEEELRGGGLLLPTPPPTTOOOMOMMELLLWWWWGGGCGCCCBCCBBGRRLRLLLPRTPPPTTBBOOOMOMMRRMLLLGGGCTGCCCBCCBWWWWBEEELRLLLPTPPPTTOOOMOMMECCCLLLWWWWGGGOOOCGCCCBCCBBRRLERLLLPTPPPTTOOOMOMMRRLLLTTGGGCGPCCCBCCBWWWWBEEELRLLLPTLPPPTTOOOMOMMELLLGWWWWGGGCGCCCBCCBBRRLRCCLLLPTPPPTTOOOÿ:r,2,GGMOGOOOGOBBPPBPPPRRLRLLLELLWWWWCMCCCMTMTTTTGGOGOOOGOPPPCCCBBPPBRRLRLLLEEECMCCCMTRMTTTTWWWWGGOGOOOGOBBRPPBPPPRRLRLLLELLMWWWWCMCCCMTMTTTTGGOGOOOGOPPPBBPPGBRRLGGRLLLEEECMCCCBBMTMLLLTTTTWWWWGGOGLLLOOOGOBBPPBPPPLLRRLRLLLELLWWWWBBCMCCCMTMTTTTGGOGOOOGOPPPBBPPBPPRRLROLLLEEECMCCCMBBTMTTTTWWWWÿ:r,2,OGGGTTTTMMTMPPRPRPGRGGGBCGCCCBCBBWWWWLLLLBBOEEEOOOTTTTPPMMTMPPRPRPOGRGGGCGCCCBCBBWWWWLLLLOOOEOOOTTTTGMMTMPPRPTTTTRPGRGGGCGCCCBCGGGBBWWWWLLLLOEEEOOOTTTTMMRTMPPRPRPGTRGGGCGCCCBCBBPWWWWLLLLOOOEOOOTTTTMMTMPPRPRPCCCGRGGGCGCCCBCBBWWWWLLLLOEEEGOOOTTTTMMTRMPPRPRPGRGGGCGCCCBCBBWWWWLLLLOOOEOOÿ:j,W,1,0,LTPOGBCMERÿ:u,APPPP.PP.PP.PPPPÿ:w,Z,1,-1,0ÿ:w,E,5,1250,0ÿ:w,R,5,250,0ÿ:w,B,5,250,0ÿ:w,M,5,200,0ÿ:w,G,5,200,0ÿ:w,T,5,100,0ÿ:w,O,5,100,0ÿ:w,P,5,50,0ÿ:w,L,5,50,0ÿ:w,C,5,50,0ÿ:w,E,4,100,0ÿ:w,R,4,50,0ÿ:w,B,4,50,0ÿ:w,M,4,50,0ÿ:w,G,4,50,0ÿ:w,T,4,20,0ÿ:w,O,4,20,0ÿ:w,P,4,10,0ÿ:w,L,4,10,0ÿ:w,C,4,10,0ÿ:w,E,3,20,0ÿ:w,R,3,10,0ÿ:w,B,3,10,0ÿ:w,M,3,10,0ÿ:w,G,3,10,0ÿ:w,T,3,5,0ÿ:w,O,3,5,0ÿ:w,P,3,5,0ÿ:w,L,3,5,0ÿ:w,C,3,5,0ÿ:wa,Z,1,-1,0,0,0,0:1,-1,0ÿ:wa,E,5,1250,0,0,0,0:1,-1,0ÿ:wa,R,5,250,0,0,0,0:1,-1,0ÿ:wa,B,5,250,0,0,0,0:1,-1,0ÿ:wa,M,5,200,0,0,0,0:1,-1,0ÿ:wa,G,5,200,0,0,0,0:1,-1,0ÿ:wa,T,5,100,0,0,0,0:1,-1,0ÿ:wa,O,5,100,0,0,0,0:1,-1,0ÿ:wa,P,5,50,0,0,0,0:1,-1,0ÿ:wa,L,5,50,0,0,0,0:1,-1,0ÿ:wa,C,5,50,0,0,0,0:1,-1,0ÿ:wa,E,4,100,0,0,0,0:1,-1,0ÿ:wa,R,4,50,0,0,0,0:1,-1,0ÿ:wa,B,4,50,0,0,0,0:1,-1,0ÿ:wa,M,4,50,0,0,0,0:1,-1,0ÿ:wa,G,4,50,0,0,0,0:1,-1,0ÿ:wa,T,4,20,0,0,0,0:1,-1,0ÿ:wa,O,4,20,0,0,0,0:1,-1,0ÿ:wa,P,4,10,0,0,0,0:1,-1,0ÿ:wa,L,4,10,0,0,0,0:1,-1,0ÿ:wa,C,4,10,0,0,0,0:1,-1,0ÿ:wa,E,3,20,0,0,0,0:1,-1,0ÿ:wa,R,3,10,0,0,0,0:1,-1,0ÿ:wa,B,3,10,0,0,0,0:1,-1,0ÿ:wa,M,3,10,0,0,0,0:1,-1,0ÿ:wa,G,3,10,0,0,0,0:1,-1,0ÿ:wa,T,3,5,0,0,0,0:1,-1,0ÿ:wa,O,3,5,0,0,0,0:1,-1,0ÿ:wa,P,3,5,0,0,0,0:1,-1,0ÿ:wa,L,3,5,0,0,0,0:1,-1,0ÿ:wa,C,3,5,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,10ÿ:m,10ÿ:a,0,0ÿ:g,999,0,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,24,29,79,36,58ÿx,7,PLAYED_FEATURE_GAMES=0,ISSUPERHOTCASHRUN=f,REMAINING_FEATURE_GAMES=0,COIN_PRIZES=4:2:10;3:1:1;1:2:1;3:1:2;1:1:1,LOCKED_COIN_POSITIONS=0:0:0;0:0:0;0:0:0;0:0:0;0:0:0,FEATURE_GAMES_ACTIVE=f,ISCOLDCASHRUN=tÿrw,0";
            }
        }
        #endregion

        public FruitKingSuperCashGameLogic()
        {
            _gameID = GAMEID.FruitKingSuperCash;
            GameName = "FruitKingSuperCash";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,3000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,10ÿ:m,10ÿ:b,16,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,0,0,0,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,24,29,79,36,58ÿx,7,PLAYED_FEATURE_GAMES=0,ISSUPERHOTCASHRUN=f,REMAINING_FEATURE_GAMES=0,COIN_PRIZES=4:2:10;3:1:1;1:2:1;3:1:2;1:1:1,LOCKED_COIN_POSITIONS=0:0:0;0:0:0;0:0:0;0:0:0;0:0:0,FEATURE_GAMES_ACTIVE=f,ISCOLDCASHRUN=tÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
