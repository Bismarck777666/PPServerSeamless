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
    internal class N40SupremeFruitsGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ11619ÿ1.10.54.0ÿEurocoinInteractiveSharedJackpotServerÿ11736ÿ1.3.0ÿ40 Supreme Fruits 95%ÿ1.118.0_2025-03-28_090804ÿ1.68.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49960,100.0,ÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,40 Supreme Fruits 95%ÿ:v,4ÿ:l,-----ÿ:l,_____ÿ:l,^^^^^ÿ:l,VVVVVÿ:l,-_V_-ÿ:l,_-^-_ÿ:l,^^-_Vÿ:l,VV_-^ÿ:l,_VVV_ÿ:l,-^^^-ÿ:l,^-_VVÿ:l,V_-^^ÿ:l,_V_-_ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,V_V_Vÿ:l,-_-^-ÿ:l,_-_V_ÿ:l,^---^ÿ:l,V___Vÿ:l,--_VVÿ:l,__-^^ÿ:l,^-__Vÿ:l,V_--^ÿ:l,-___Vÿ:l,_---^ÿ:l,^^-^^ÿ:l,VV_VVÿ:l,__V__ÿ:l,--^--ÿ:l,^^^-_ÿ:l,VVV_-ÿ:l,_VV_-ÿ:l,-^^-_ÿ:l,^--_Vÿ:l,V__-^ÿ:l,_V_-^ÿ:l,-^-_Vÿ:l,^-_V_ÿ:l,V_-^-ÿ:r,0,GGGGOOOOCCCCMMMMLLLLPPPPCCCCSMMMMOOOOLLLLGGGGOOOOPPPPLLLLGGGGCCCCPPPPMMMMGGGGPPPP7777ÿ:r,0,OOOOSOOOOLLLLCCCCGGGGOOOOMMMMLLLLPPPPCCCCMMMMLLLLPPPPCCCCMMMMPPPPOOOOGGGG7777ÿ:r,0,GGGGCCCCSGGGGLLLLGGGGOOOOSMMMMCCCCOOOOPPPPLLLLGGGGMMMM7777ÿ:r,0,OOOOPPPPOOOOSLLLLPPPPLLLLCCCCMMMMPPPPCCCCMMMMCCCCGGGGOOOOLLLLMMMMOOOOCCCC7777ÿ:r,0,MMMMLLLLPPPPOOOOGGGGOOOOPPPPMMMMLLLLMMMMOOOOGGGGCCCCSGGGGLLLLCCCCPPPPCCCCMMMMOOOO7777ÿ:r,1,GGGGOOOOCCCCPPPPMMMMLLLLPPPPCCCCLLLL7777OOOOSMMMMLLLLOOOOLLLLGGGGSOOOOPPPPLLLLOOOOGGGGCCCCPPPPMMMMLLLLÿ:r,1,OOOOSOOOOLLLLCCCCGGGGOOOOSCCCCMMMMLLLLOOOOPPPPLLLLOOOOCCCCGGGGOOOOMMMM7777LLLLMMMMPPPPCCCCGGGGMMMMSPPPPÿ:r,1,7777MMMMCCCCPPPPCCCCOOOOGGGGPPPPCCCCSGGGGLLLLPPPPCCCCLLLLGGGGOOOOSMMMMOOOOCCCCMMMMLLLLCCCCOOOOPPPPLLLLSPPPPÿ:r,1,OOOOPPPPOOOOSLLLLGGGGPPPPOOOO7777LLLLCCCCMMMMPPPPCCCCGGGGMMMMCCCCGGGGOOOOLLLLCCCCMMMMLLLLMMMMOOOOLLLLSCCCCÿ:r,1,MMMMOOOOLLLL7777OOOOGGGG7777OOOOPPPPSOOOOMMMMLLLLOOOOMMMMOOOOCCCCSOOOOLLLLCCCCPPPPCCCCSCCCCÿ:j,7,1,0,7MGPOLCÿ:u,APPPP.PP.PP.PPPPÿ:w,#,103,-5000,1:Grandÿ:w,#,102,-1250,1:Majorÿ:w,#,101,-200,1:Minorÿ:w,#,100,-50,1:Miniÿ:w,S,5,10000,0ÿ:w,S,4,400,0ÿ:w,S,3,100,0ÿ:w,7,5,500,0ÿ:w,7,4,200,0ÿ:w,7,3,20,0ÿ:w,G,5,200,0ÿ:w,G,4,40,0ÿ:w,G,3,10,0ÿ:w,M,5,100,0ÿ:w,M,4,20,0ÿ:w,M,3,10,0ÿ:w,P,5,100,0ÿ:w,P,4,20,0ÿ:w,P,3,10,0ÿ:w,O,5,50,0ÿ:w,O,4,10,0ÿ:w,O,3,5,0ÿ:w,L,5,50,0ÿ:w,L,4,10,0ÿ:w,L,3,5,0ÿ:w,C,5,50,0ÿ:w,C,4,10,0ÿ:w,C,3,5,0ÿ:wa,#,103,-5000,1:Grand,0,0,0:1,-1,0ÿ:wa,#,102,-1250,1:Major,0,0,0:1,-1,0ÿ:wa,#,101,-200,1:Minor,0,0,0:1,-1,0ÿ:wa,#,100,-50,1:Mini,0,0,0:1,-1,0ÿ:wa,S,5,10000,0,0,0,0:1,-1,0ÿ:wa,S,4,400,0,0,0,0:1,-1,0ÿ:wa,S,3,100,0,0,0,0:1,-1,0ÿ:wa,7,5,500,0,0,0,0:1,-1,0ÿ:wa,7,4,200,0,0,0,0:1,-1,0ÿ:wa,7,3,20,0,0,0,0:1,-1,0ÿ:wa,G,5,200,0,0,0,0:1,-1,0ÿ:wa,G,4,40,0,0,0,0:1,-1,0ÿ:wa,G,3,10,0,0,0,0:1,-1,0ÿ:wa,M,5,100,0,0,0,0:1,-1,0ÿ:wa,M,4,20,0,0,0,0:1,-1,0ÿ:wa,M,3,10,0,0,0,0:1,-1,0ÿ:wa,P,5,100,0,0,0,0:1,-1,0ÿ:wa,P,4,20,0,0,0,0:1,-1,0ÿ:wa,P,3,10,0,0,0,0:1,-1,0ÿ:wa,O,5,50,0,0,0,0:1,-1,0ÿ:wa,O,4,10,0,0,0,0:1,-1,0ÿ:wa,O,3,5,0,0,0,0:1,-1,0ÿ:wa,L,5,50,0,0,0,0:1,-1,0ÿ:wa,L,4,10,0,0,0,0:1,-1,0ÿ:wa,L,3,5,0,0,0,0:1,-1,0ÿ:wa,C,5,50,0,0,0,0:1,-1,0ÿ:wa,C,4,10,0,0,0,0:1,-1,0ÿ:wa,C,3,5,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,40ÿ:m,20ÿ:b,17,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,250ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,20,0,0,0ÿb,1000,1000,0ÿs,1ÿr,1,5,75,61,57,99,49ÿx,1,j=-1ÿrw,0";
            }
        }
        #endregion

        public N40SupremeFruitsGameLogic()
        {
            _gameID = GAMEID.N40SupremeFruits;
            GameName = "40SupremeFruits";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,5000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,40ÿ:m,20ÿ:b,17,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,250ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,5,75,61,57,99,49ÿx,6,jc=2200:331:9:0,jsc=52820.0:7577360.0:4.61785494E9:0.0,j=-1,jhi=9.2510393E9:2.443788288E10:1.898253334E10:0.0,js=805066755:805065715:804946618:0,jhs=804635867:804430271:804228192:0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
