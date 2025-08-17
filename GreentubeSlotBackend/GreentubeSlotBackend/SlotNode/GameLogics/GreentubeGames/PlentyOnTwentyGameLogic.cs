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
    internal class PlentyOnTwentyGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ120ÿ1.0.0ÿPlenty on Twenty 95.6%ÿ1.113.0_2024-11-19_131508ÿ1.107.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,2000,100.0,ÿM,10,10000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Plenty on Twenty 95.6%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:r,0,777OOOPPPCCCMMMLLLBBBLLLCCCSLLLMMMOOOPPPCCCBBBOOOSLLLBBBCCCPPPMMMLLLPPPLLLOOOPPPLLL777OOOPPPCCCMMMLLLBBBCCCSLLLMMMOOOPPPCCCBBBOOOSLLLBBBCCCPPPMMMLLLPPPLLLOOOLLL777OOOPPPCCCMMMLLLBBBCCCSLLLMMMOOOPPPCCCBBBOOOSLLLBBBLLLCCCPPPMMMLLLPPPLLLOOOLLLÿ:r,0,777LLLMMMCCCLLLBBBOOOSCCCPPPLLLBBBOOOSCCCOOOBBBLLLMMMOOOPPPCCCMMMSPPPOOOMMMOOOLLLOOO777LLLMMMCCCBBBPPPOOOSCCCPPPLLLBBBOOOSCCCBBBLLLMMMOOOPPPCCCMMMPPPOOOBBBMMMOOOLLLOOO777LLLMMMCCCBBBOOOSCCCPPPLLLBBBOOOSPPPCCCBBBLLLMMMOOOPPPCCCMMMPPPOOOMMMOOOLLLOOOÿ:r,0,777MMMCCCOOOBBBLLLSCCCPPPLLLBBBOOOSCCCMMMLLLBBBOOOPPPSCCCMMMLLLPPPCCCMMMOOOCCCSPPP777MMMCCCOOOBBBLLLSCCCPPPLLLBBBOOOSCCCMMMLLLBBBOOOPPPCCCMMMLLLPPPCCCMMMOOOCCCPPP777MMMCCCOOOBBBLLLSCCCPPPLLLBBBOOOSCCCMMMLLLBBBOOOPPPCCCMMMLLLPPPCCCMMMOOOCCCPPPÿ:r,0,777LLLMMMCCCBBBPPPOOOSLLLCCCMMMLLLBBBOOOCCCSLLLOOOBBBCCCPPPMMMOOOLLLCCCMMMSPPPOOO777LLLMMMCCCBBBPPPOOOSLLLCCCMMMLLLBBBOOOCCCSLLLOOOBBBCCCPPPMMMOOOLLLCCCMMMPPPOOO777LLLMMMCCCBBBPPPOOOSLLLCCCMMMLLLBBBOOOCCCSLLLOOOBBBCCCPPPMMMOOOLLLCCCMMMPPPOOOÿ:r,0,777CCCMMMLLLBBBCCCSOOOPPPLLLBBBOOOPPPSLLLMMMOOOBBBCCCMMMOOOLLLCCCPPPOOOCCCPPPLLL777CCCMMMLLLPPPBBBCCCSOOOPPPLLLBBBOOOPPPSLLLMMMOOOBBBCCCMMMOOOLLLCCCPPPOOOCCCPPPLLL777CCCMMMLLLBBBCCCSOOOPPPLLLBBBOOOPPPSLLLMMMOOOBBBCCCMMMOOOLLLCCCPPPOOOCCCPPPLLLÿ:j,7,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,S,5,-500,0ÿ:w,S,4,-20,0ÿ:w,S,3,-5,0ÿ:w,7,5,1000,0ÿ:w,7,4,400,0ÿ:w,7,3,40,0ÿ:w,B,5,400,0ÿ:w,B,4,80,0ÿ:w,B,3,20,0ÿ:w,M,5,200,0ÿ:w,M,4,40,0ÿ:w,M,3,20,0ÿ:w,P,5,200,0ÿ:w,P,4,40,0ÿ:w,P,3,20,0ÿ:w,L,5,100,0ÿ:w,L,4,20,0ÿ:w,L,3,10,0ÿ:w,O,5,100,0ÿ:w,O,4,20,0ÿ:w,O,3,10,0ÿ:w,C,5,100,0ÿ:w,C,4,20,0ÿ:w,C,3,10,0ÿ:wa,S,5,-500,0,0,0,0:1,-1,0ÿ:wa,S,4,-20,0,0,0,0:1,-1,0ÿ:wa,S,3,-5,0,0,0,0:1,-1,0ÿ:wa,7,5,1000,0,0,0,0:1,-1,0ÿ:wa,7,4,400,0,0,0,0:1,-1,0ÿ:wa,7,3,40,0,0,0,0:1,-1,0ÿ:wa,B,5,400,0,0,0,0:1,-1,0ÿ:wa,B,4,80,0,0,0,0:1,-1,0ÿ:wa,B,3,20,0,0,0,0:1,-1,0ÿ:wa,M,5,200,0,0,0,0:1,-1,0ÿ:wa,M,4,40,0,0,0,0:1,-1,0ÿ:wa,M,3,20,0,0,0,0:1,-1,0ÿ:wa,P,5,200,0,0,0,0:1,-1,0ÿ:wa,P,4,40,0,0,0,0:1,-1,0ÿ:wa,P,3,20,0,0,0,0:1,-1,0ÿ:wa,L,5,100,0,0,0,0:1,-1,0ÿ:wa,L,4,20,0,0,0,0:1,-1,0ÿ:wa,L,3,10,0,0,0,0:1,-1,0ÿ:wa,O,5,100,0,0,0,0:1,-1,0ÿ:wa,O,4,20,0,0,0,0:1,-1,0ÿ:wa,O,3,10,0,0,0,0:1,-1,0ÿ:wa,C,5,100,0,0,0,0:1,-1,0ÿ:wa,C,4,20,0,0,0,0:1,-1,0ÿ:wa,C,3,10,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,20ÿ:m,20ÿ:b,16,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,20,0,0,0ÿb,1000,10000,0ÿ:x,12000ÿs,1ÿr,0,5,10,25,42,21,34ÿrw,0ÿ:wee,0";
            }
        }
        #endregion

        public PlentyOnTwentyGameLogic()
        {
            _gameID = GAMEID.PlentyOnTwenty;
            GameName = "PlentyOnTwenty";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,5000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,20ÿ:m,20ÿ:b,16,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,10,25,42,21,34ÿrw,0ÿ:wee,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
