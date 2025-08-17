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
    internal class SmokingHot7sGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ11619ÿ1.10.54.0ÿEurocoinInteractiveSharedJackpotServerÿ12225ÿ1.3.2ÿSmoking Hot 7s 95%ÿ1.118.0_2025-03-28_090804ÿ1.68.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Smoking Hot 7s 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:r,0,7LLLOOMMGG7PPGOOOSMMCCPPOOMMCPPGGLLPPLCCMMOSOO7LLLOOMMLGG7GOOOSMMCCPPOOMMCGPPGGLLPPLCCMMOSOO7LLLOOMMGGCC7GOOOSMMOOCCPPOOMMCPPGGLLPPLCCMMOGGSOOÿ:r,0,7PPPMM7MOGGPPGSMMCCPPLLLSLCCPPPGOOOGGPGGMMCPPP7PPPMM7MOGGPPGSMMCCPPLLLSLCCGOOOGGPGGCSPPP7PPPMM7MOGGPPGSMMCCPPLLLSLCCGOOOGGPGGCPPPÿ:r,0,7PCCCLLLSOOL7PMMOOLSCCLLSOOOGGCGSPPCPCCLLSCCMM7PCCCLLLSOOGGL7PMMOOLSCCLLSOOOGGCGPPSCPCCLLCCMM7PCCCLLLSOOL7PMMOOLSCCLLSOOOGGCSGPPCPCCLLCCMMÿ:r,0,7OOOO7OMMLLLLGSGGPPPPPCGPCCCCLLSMMP7PPPCCCCLLCCCCMOOOGGLL7OOOO7OMMPPPLLLLGSGGPPPPPCPCCCCLLSMMP7PPPLLGGCCCCMOOOGGLL7OOOO7OMMLLLLGSGGPPPPPCPCCCCLLSMMP7PPPLLCCCCMOOOGGLLÿ:r,0,7MLLPPPPMMOSOOOOLLLP7CCCCGCGG7LLPPPMCCCCCPMMOSOOOOLLLP7PPPPCCCCCGG7LLGPPPPMMOSOOOOLLLPG7CCCCGCGG7OOOOLLPPPMPMMOSOOOOLLLP7CCCCCGG7LLPPPPMMOCCCCSOOOOLLLP7CCCCGCGG7LLPPPMPMMOSOOOOLLLP7CCCCCGGÿ:r,1,7MPPPG7GGSLLLCCCLLMMOMOOOP7PPPGOOO7GGSGGLLLCCCLLMMOMOOOP7PPPG7MMGGSLLLCCCLLMMOMOOOPÿ:r,1,7MLLLMM7MCCCCSPPPPOOOGOGGL7LLLMM7MCCCCSPPPPOOOGOMMGGL7LLLMM7MCCCCSPPPPOOOGOGGLÿ:r,1,7LCCCSOOO7OMMLLLPMPPPGGCGLLL7LCCCSOOOL7OMMPMPPPGGCGLLL7LCCCSOOO7OMMPMPPPGGCGLLLÿ:r,1,7OOOO7OPPPSCCCGGLLLCLCCCPPMM7OOOOL7OPPPSGGLLLCLCCCPPMM7OOOO7OPPPSGGLLLCLOCCCPPMMÿ:r,1,7PPLLLLMMOSOOOSOPPPL7CCCCCGG7PPLLLLMMOSOOOSOPPPL7CCCCCGG7PPLLLLMMOS7OOOSOPPPL7CCCCCGGÿ:u,APPPP.PP.PP.PPPPÿ:w,#,103,-5000,1:Grandÿ:w,#,102,-1250,1:Majorÿ:w,#,101,-200,1:Minorÿ:w,#,100,-50,1:Miniÿ:w,S,3,10,0ÿ:w,S,4,50,0ÿ:w,S,5,250,0ÿ:w,7,3,100,0ÿ:w,7,4,1000,0ÿ:w,7,5,5000,0ÿ:w,M,3,50,0ÿ:w,M,4,200,0ÿ:w,M,5,500,0ÿ:w,G,3,50,0ÿ:w,G,4,200,0ÿ:w,G,5,500,0ÿ:w,P,3,20,0ÿ:w,P,4,50,0ÿ:w,P,5,200,0ÿ:w,O,3,20,0ÿ:w,O,4,50,0ÿ:w,O,5,200,0ÿ:w,L,3,20,0ÿ:w,L,4,50,0ÿ:w,L,5,200,0ÿ:w,C,2,5,0ÿ:w,C,3,20,0ÿ:w,C,4,50,0ÿ:w,C,5,200,0ÿ:wa,#,103,-5000,1:Grand,0,0,0:1,-1,0ÿ:wa,#,102,-1250,1:Major,0,0,0:1,-1,0ÿ:wa,#,101,-200,1:Minor,0,0,0:1,-1,0ÿ:wa,#,100,-50,1:Mini,0,0,0:1,-1,0ÿ:wa,S,3,10,0,0,0,0:1,-1,0ÿ:wa,S,4,50,0,0,0,0:1,-1,0ÿ:wa,S,5,250,0,0,0,0:1,-1,0ÿ:wa,7,3,100,0,0,0,0:1,-1,0ÿ:wa,7,4,1000,0,0,0,0:1,-1,0ÿ:wa,7,5,5000,0,0,0,0:1,-1,0ÿ:wa,M,3,50,0,0,0,0:1,-1,0ÿ:wa,M,4,200,0,0,0,0:1,-1,0ÿ:wa,M,5,500,0,0,0,0:1,-1,0ÿ:wa,G,3,50,0,0,0,0:1,-1,0ÿ:wa,G,4,200,0,0,0,0:1,-1,0ÿ:wa,G,5,500,0,0,0,0:1,-1,0ÿ:wa,P,3,20,0,0,0,0:1,-1,0ÿ:wa,P,4,50,0,0,0,0:1,-1,0ÿ:wa,P,5,200,0,0,0,0:1,-1,0ÿ:wa,O,3,20,0,0,0,0:1,-1,0ÿ:wa,O,4,50,0,0,0,0:1,-1,0ÿ:wa,O,5,200,0,0,0,0:1,-1,0ÿ:wa,L,3,20,0,0,0,0:1,-1,0ÿ:wa,L,4,50,0,0,0,0:1,-1,0ÿ:wa,L,5,200,0,0,0,0:1,-1,0ÿ:wa,C,2,5,0,0,0,0:1,-1,0ÿ:wa,C,3,20,0,0,0,0:1,-1,0ÿ:wa,C,4,50,0,0,0,0:1,-1,0ÿ:wa,C,5,200,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,5ÿ:m,5ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,5,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,3,72,3,82,28ÿx,1,j=-1ÿrw,0";
            }
        }
        #endregion

        public SmokingHot7sGameLogic()
        {
            _gameID = GAMEID.SmokingHot7s;
            GameName = "SmokingHot7s";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,5000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,5ÿ:m,5ÿ:b,18,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500,800,1000ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,3,72,3,82,28ÿx,6,jc=13919:2010:92:9,jsc=2731740.0:1.112211E8:3.307221E9:7.9479284E8,j=-1,jhi=9.09484624E9:3.255293118E10:1.8954007362E11:7.308640018E10,js=804715327:804715045:804714187:804526691,jhs=804060562:804269993:804094779:804068286ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
