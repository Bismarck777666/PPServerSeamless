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
    internal class JustJewelsDeluxeGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ90ÿ1.0.0ÿJust Jewels 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Just Jewels 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,CCCGGGMMMMRRRPPPPPEBBBBBCCCGGGMMMMRRREPPPPPOOOBBBBBCCCGGGMMMMRRREPPPPPBBBBBCCCGGGMMMMRRRPPPPPEBBBBBCCCGGGMMMMRRREPPPPPOOOBBBBBCCCGGGMMMMRRREPPPPPBBBBBCCCGGGMMMMRRRPPPPPEBBBBBCCCGGGMMMMRRREPPPPPOOOBBBBBCCCGGGMMMMRRREPPPPPBBBBBÿ:r,0,CCCBBBOOOOEPPPPERRRRREGGGGEBBBOOOOEPPPPERRRRREMMMGGGGEBBBOOOOEPPPPERRRRREGGGGECCCBBBOOOOEPPPPERRRRREGGGGEBBBOOOOEPPPPERRRRREMMMGGGGEBBBOOOOEPPPPERRRRREGGGGECCCBBBOOOOEPPPPERRRRREGGGGEBBBOOOOEPPPPERRRRREMMMGGGGEBBBOOOOEPPPPERRRRREGGGGEÿ:r,0,CPPPOOOOBBBERRRMMMMGGGGGGPPPOOOOBBBERRRMMMMGGGGGGPPPOOOOBBBERRRMMMMGGGGGGCCCPPPOOOOBBBERRRMMMMGGGGGGPPPOOOOBBBERRRMMMMGGGGGGPPPOOOOBBBERRRMMMMGGGGGGCCCPPPOOOOBBBERRRMMMMGGGGGGPPPOOOOBBBERRRMMMMGGGGGGPPPOOOOBBBERRRMMMMGGGGGGCCÿ:r,0,BBBCCCRRRMMMMGGGCCCCPPPPPPEBBBCCCRRRRMMMMGGGOOOPPPPPPEBBBCCCRRRGGGMMMMPPPPPPEBBBCCCRRRMMMMGGGCCCCPPPPPPEBBBCCCRRRRMMMMGGGOOOPPPPPPEBBBCCCRRRGGGMMMMPPPPPPEBBBCCCRRRMMMMGGGCCCCPPPPPPEBBBCCCRRRRMMMMGGGOOOPPPPPPEBBBCCCRRRGGGMMMMPPPPPPEÿ:r,0,GGGGOOOORRRREBBBBBBMMMPPPPGGGGOOOORRRRBBBBBBEPPPPCCCGGGGOOOORRRREBBBBBBPPPPGGGGOOOORRRREBBBBBBMMMPPPPGGGGOOOORRRRBBBBBBEPPPPCCCGGGGOOOORRRREBBBBBBPPPPGGGGOOOORRRREBBBBBBMMMPPPPGGGGOOOORRRRBBBBBBEPPPPCCCGGGGOOOORRRREBBBBBBPPPPÿ:u,APPPP.PP.PP.PPPPÿ:w,E,5,-50,0ÿ:w,E,4,-10,0ÿ:w,E,3,-2,0ÿ:w,C,5,5000,0ÿ:w,C,4,500,0ÿ:w,C,3,50,0ÿ:w,O,5,500,0ÿ:w,O,4,150,0ÿ:w,O,3,30,0ÿ:w,M,5,500,0ÿ:w,M,4,150,0ÿ:w,M,3,30,0ÿ:w,R,5,200,0ÿ:w,R,4,50,0ÿ:w,R,3,15,0ÿ:w,B,5,200,0ÿ:w,B,4,50,0ÿ:w,B,3,15,0ÿ:w,P,5,150,0ÿ:w,P,4,25,0ÿ:w,P,3,10,0ÿ:w,G,5,150,0ÿ:w,G,4,25,0ÿ:w,G,3,10,0ÿ:wa,E,5,-50,0,0,0,0:1,-1,0ÿ:wa,E,4,-10,0,0,0,0:1,-1,0ÿ:wa,E,3,-2,0,0,0,0:1,-1,0ÿ:wa,C,5,5000,0,0,0,0:1,-1,0ÿ:wa,C,4,500,0,0,0,0:1,-1,0ÿ:wa,C,4,500,0,0,1,0:1,-1,0ÿ:wa,C,3,50,0,0,0,0:1,-1,0ÿ:wa,C,3,50,0,0,1,0:1,-1,0ÿ:wa,C,3,50,0,0,2,0:1,-1,0ÿ:wa,O,5,500,0,0,0,0:1,-1,0ÿ:wa,O,4,150,0,0,0,0:1,-1,0ÿ:wa,O,4,150,0,0,1,0:1,-1,0ÿ:wa,O,3,30,0,0,0,0:1,-1,0ÿ:wa,O,3,30,0,0,1,0:1,-1,0ÿ:wa,O,3,30,0,0,2,0:1,-1,0ÿ:wa,M,5,500,0,0,0,0:1,-1,0ÿ:wa,M,4,150,0,0,0,0:1,-1,0ÿ:wa,M,4,150,0,0,1,0:1,-1,0ÿ:wa,M,3,30,0,0,0,0:1,-1,0ÿ:wa,M,3,30,0,0,1,0:1,-1,0ÿ:wa,M,3,30,0,0,2,0:1,-1,0ÿ:wa,R,5,200,0,0,0,0:1,-1,0ÿ:wa,R,4,50,0,0,0,0:1,-1,0ÿ:wa,R,4,50,0,0,1,0:1,-1,0ÿ:wa,R,3,15,0,0,0,0:1,-1,0ÿ:wa,R,3,15,0,0,1,0:1,-1,0ÿ:wa,R,3,15,0,0,2,0:1,-1,0ÿ:wa,B,5,200,0,0,0,0:1,-1,0ÿ:wa,B,4,50,0,0,0,0:1,-1,0ÿ:wa,B,4,50,0,0,1,0:1,-1,0ÿ:wa,B,3,15,0,0,0,0:1,-1,0ÿ:wa,B,3,15,0,0,1,0:1,-1,0ÿ:wa,B,3,15,0,0,2,0:1,-1,0ÿ:wa,P,5,150,0,0,0,0:1,-1,0ÿ:wa,P,4,25,0,0,0,0:1,-1,0ÿ:wa,P,4,25,0,0,1,0:1,-1,0ÿ:wa,P,3,10,0,0,0,0:1,-1,0ÿ:wa,P,3,10,0,0,1,0:1,-1,0ÿ:wa,P,3,10,0,0,2,0:1,-1,0ÿ:wa,G,5,150,0,0,0,0:1,-1,0ÿ:wa,G,4,25,0,0,0,0:1,-1,0ÿ:wa,G,4,25,0,0,1,0:1,-1,0ÿ:wa,G,3,10,0,0,0,0:1,-1,0ÿ:wa,G,3,10,0,0,1,0:1,-1,0ÿ:wa,G,3,10,0,0,2,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,9,-1ÿb,0,0,0ÿs,1ÿr,0,5,39,8,53,24,3ÿrw,0";
            }
        }
        #endregion

        public JustJewelsDeluxeGameLogic()
        {
            _gameID = GAMEID.JustJewelsDeluxe;
            GameName = "JustJewelsDeluxe";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,2,3000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:f,0,1,2,3,4,5,6,7,8,9ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:e,0,0,0,0,0,0,0,0,0,0ÿ:b,16,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,2.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,39,8,53,24,3ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
                resMessage.Append("\aTICKETSÿÿMINOFFERÿ50ÿINDEPENDENTLINESANDBETSÿ1ÿCCODEÿchÿMAXLIMITÿ50000ÿREALCURRENCYFACTORÿ106.53030787258975178438265686ÿREALMONEYCURRENCYCODEISOÿCHFÿTOURNISSUBSCRIBEDÿ0ÿNATIVEJACKPOTCURRENCYÿCHFÿISSERVERAUTOREBUYÿ0ÿMINBUYINÿ2ÿPAYOUTRATEÿ95.01%ÿALLOWCATEGORYÿ0,1,3,4,5ÿNATIVEJACKPOTCURRENCYFACTORÿ93.87000693699351264382058437ÿUSERIDÿ621884763ÿLANGUAGEIDÿ2ÿJACKPOTFEEPERCENTAGEÿ0.0000ÿMAXBUYINÿ2147483647ÿLIMITÿ50000ÿREALMONEYÿ0ÿERRNUMBERÿ42ÿOFFERÿ50ÿPLAYERBALANCEÿ50000ÿDISALLOWRECONNECTAFTERINACTIVITYÿ0ÿAUTOKICKÿ0ÿCLIENT_TO_WRAPPERÿ1ÿISDEEPWALLETÿ1ÿCURRENCYÿÿSITEIDÿ2337ÿRELIABILITYÿ14ÿWRAPPER_TO_CLIENTÿ1ÿNEEDSSESSIONTIMEOUTÿ0ÿLANGUAGEISOCODEÿENÿEXTUSERIDÿ621884763ÿTICKETTYPEÿ1ÿCURRENCYFACTORÿ100.000000ÿSESSIONTIMEOUTMINUTESÿ20ÿNICKNAMEÿ#1165325621884763ÿREALCURRENCYÿCHFÿMINLIMITÿ50");
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
                resMessage.Append("\aTICKETSÿÿMINOFFERÿ50ÿINDEPENDENTLINESANDBETSÿ1ÿCCODEÿchÿMAXLIMITÿ50000ÿREALCURRENCYFACTORÿ106.53030787258975178438265686ÿREALMONEYCURRENCYCODEISOÿCHFÿTOURNISSUBSCRIBEDÿ0ÿNATIVEJACKPOTCURRENCYÿCHFÿISSERVERAUTOREBUYÿ0ÿMINBUYINÿ2ÿPAYOUTRATEÿ95.01%ÿALLOWCATEGORYÿ0,1,3,4,5ÿNATIVEJACKPOTCURRENCYFACTORÿ93.87000693699351264382058437ÿUSERIDÿ621884763ÿLANGUAGEIDÿ2ÿJACKPOTFEEPERCENTAGEÿ0.0000ÿMAXBUYINÿ2147483647ÿLIMITÿ50000ÿREALMONEYÿ0ÿERRNUMBERÿ42ÿOFFERÿ50ÿPLAYERBALANCEÿ50000ÿDISALLOWRECONNECTAFTERINACTIVITYÿ0ÿAUTOKICKÿ0ÿCLIENT_TO_WRAPPERÿ1ÿISDEEPWALLETÿ1ÿCURRENCYÿÿSITEIDÿ2337ÿRELIABILITYÿ14ÿWRAPPER_TO_CLIENTÿ1ÿNEEDSSESSIONTIMEOUTÿ0ÿLANGUAGEISOCODEÿENÿEXTUSERIDÿ621884763ÿTICKETTYPEÿ1ÿCURRENCYFACTORÿ100.000000ÿSESSIONTIMEOUTMINUTESÿ20ÿNICKNAMEÿ#1165325621884763ÿREALCURRENCYÿCHFÿMINLIMITÿ50");
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
