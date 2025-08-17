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
    internal class N10ImperialCrownDeluxeGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ11619ÿ1.10.54.0ÿEurocoinInteractiveSharedJackpotServerÿ15103ÿ1.0.0ÿ10 Imperial Crown 95%ÿ1.118.0_2025-03-28_090804ÿ1.68.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,10 Imperial Crown 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,^^-__ÿ:l,__-^^ÿ:l,-___-ÿ:l,-^^^-ÿ:l,^---^ÿ:r,0,LLLB7MOOOOGSCCCBRMGPPPPBLLLMGCCC7PPPSOOORLLLB7MOOOOGSCCCBRMGPPPPBLLLMGCCC7PPPSOOORLLLB7MOOOOGSCCCBRMGPPPPBLLLMGCCC7PPPSOOORÿ:r,0,LLL7OOOMBMBPPPB7GCCCOOOGMPPPPLLLBRGMCCC7GLLLPPPROOOOCCCLLL7OOOMBMBPPPB7GCCCOOOGMPPPPLLLBRGMCCC7GLLLPPPROOOOMCCCLLL7OOOMBGMBPPPB7GCCCOOOGMPPPPLLLBRGMCCC7GLLLPPPROOOOCCCÿ:r,0,OOOB7CCCCSLLLBGBGSGPPPMRGLLLOOOO7SMPPPPMSBCCCLLLLOOOSPPPCCCOOOB7CCCCSBLLLBGBGSPPPCCCCMRGLLLOOOO7BSMPPPPMSBCCCLLLLOOOSPPPCCCOOOB7CCCCSLLLBGCCCBGSPPPMRBGLLLOOOO7SMPPPPMSBCCCLLLLOOOSPPPCCCÿ:r,0,LLM7GPPPOOOB7GLLLCCCOOO7MBMBPPPLLLOOOCCCPPPBRGCCLLLM7BGPPPOOOB7GLLLCCCOOOB7MBMBPPPLLLOOOCCC7PPPBRGCCLLLM7GPPPOOOB7GLLLCCCOOO7MBMBPPPLLLOOOCCCPPPBRGCCLÿ:r,0,LLLB7GOOOOMSGCCCGRBPPPLLLLOOOOB7MPPPMRCCCSMPP7CCLLLLBPP7GOOOOSGCCCGRBPPPLLLLOOOOB7MPPPMRCCCSMPP7CCLLLLB7CCCGOOOOSGCCCGRBPPPMLLLLOOOOB7CCCMPPPMRCCCSMPP7CCLÿ:r,1,LLLLB7MOOOGSCCCBRMGPPPBLLLMGCCC7PPPSOOORLLLLB7MOOOGSCCCBRMGPPPBLLLMGCCC7PPPSOOORLLLLB7MOOOGSCCCBRMGPPPBLLLMGCCC7PPPSOOORÿ:r,1,LLL7OOOMBXMBPPPB7GCCCOOOGMPPPPLLLLBRGMBCCC7GLLL7OOOMBXMBPPPB7GCCCOOOGMPPPPLLLLBRGMCCCM7GLLL7OOOMBXMBPPPB7GCCCOOOGMPPPPBLLLLBRGBMCCC7Gÿ:r,1,OOOB7CCCCSLLLBGXBGSPPPMRGLLLMOOOO7SMPPPP7MSBCCOOOB7CCCCSLLLBGXBGSPPPMRGLLLOOOO7SMPPPP7GMSBCCOOOB7CCCCSLLLBGXBGSPPPMRGLLLOOOO7SMPPPP7MSLLLBCCÿ:r,1,LLM7GPPPOOOBRGBLLLG7CCCCMBXMBPPPGCCCOOOLLLM7GPPPOOOBRGBLLLG7CCCCMBXMBPPPCCCOOOLLLM7GPPPOOOBRGBLLLGM7CCCCMBXMBPPPCCCOOOLÿ:r,1,LLLB7PPGOOOOSGMCCCGRBPPPLLLLOOOOB7MPPPMRCCC7MPPSCCLLLLB7GOOOOPPSGCCCGRBPPPLLLLOOOOB7MPPPMRCCC7MPPSCCLLLLB7GOOOOSGCCCGRBPPPMLLLLOOOOB7MPPPMRCCC7MPPSCCLÿ:j,X,1,0,7MGBPOLCÿ:u,APPPP.PP.PP.PPPPÿ:w,#,103,-5000,1:Grandÿ:w,#,102,-1250,1:Majorÿ:w,#,101,-200,1:Minorÿ:w,#,100,-50,1:Miniÿ:w,R,3,50,0ÿ:w,R,4,200,0ÿ:w,R,5,1000,0ÿ:w,S,3,200,0ÿ:w,7,2,10,0ÿ:w,7,3,50,0ÿ:w,7,4,250,0ÿ:w,7,5,5000,0ÿ:w,M,3,40,0ÿ:w,M,4,120,0ÿ:w,M,5,700,0ÿ:w,G,3,40,0ÿ:w,G,4,120,0ÿ:w,G,5,700,0ÿ:w,B,3,20,0ÿ:w,B,4,40,0ÿ:w,B,5,200,0ÿ:w,P,3,10,0ÿ:w,P,4,30,0ÿ:w,P,5,150,0ÿ:w,O,3,10,0ÿ:w,O,4,30,0ÿ:w,O,5,150,0ÿ:w,L,3,10,0ÿ:w,L,4,30,0ÿ:w,L,5,150,0ÿ:w,C,3,10,0ÿ:w,C,4,30,0ÿ:w,C,5,150,0ÿ:wa,#,103,-5000,1:Grand,0,0,0:1,-1,0ÿ:wa,#,102,-1250,1:Major,0,0,0:1,-1,0ÿ:wa,#,101,-200,1:Minor,0,0,0:1,-1,0ÿ:wa,#,100,-50,1:Mini,0,0,0:1,-1,0ÿ:wa,R,3,50,0,0,0,0:1,-1,0ÿ:wa,R,4,200,0,0,0,0:1,-1,0ÿ:wa,R,5,1000,0,0,0,0:1,-1,0ÿ:wa,S,3,200,0,0,0,0:1,-1,0ÿ:wa,7,2,10,0,0,0,0:1,-1,0ÿ:wa,7,3,50,0,0,0,0:1,-1,0ÿ:wa,7,4,250,0,0,0,0:1,-1,0ÿ:wa,7,5,5000,0,0,0,0:1,-1,0ÿ:wa,M,3,40,0,0,0,0:1,-1,0ÿ:wa,M,4,120,0,0,0,0:1,-1,0ÿ:wa,M,5,700,0,0,0,0:1,-1,0ÿ:wa,G,3,40,0,0,0,0:1,-1,0ÿ:wa,G,4,120,0,0,0,0:1,-1,0ÿ:wa,G,5,700,0,0,0,0:1,-1,0ÿ:wa,B,3,20,0,0,0,0:1,-1,0ÿ:wa,B,4,40,0,0,0,0:1,-1,0ÿ:wa,B,5,200,0,0,0,0:1,-1,0ÿ:wa,P,3,10,0,0,0,0:1,-1,0ÿ:wa,P,4,30,0,0,0,0:1,-1,0ÿ:wa,P,5,150,0,0,0,0:1,-1,0ÿ:wa,O,3,10,0,0,0,0:1,-1,0ÿ:wa,O,4,30,0,0,0,0:1,-1,0ÿ:wa,O,5,150,0,0,0,0:1,-1,0ÿ:wa,L,3,10,0,0,0,0:1,-1,0ÿ:wa,L,4,30,0,0,0,0:1,-1,0ÿ:wa,L,5,150,0,0,0,0:1,-1,0ÿ:wa,C,3,10,0,0,0,0:1,-1,0ÿ:wa,C,4,30,0,0,0,0:1,-1,0ÿ:wa,C,5,150,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,10ÿ:m,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,0,-1ÿb,0,0,0ÿs,1ÿr,1,5,5,5,5,1,8ÿx,1,j=-1ÿrw,0";
            }
        }
        #endregion

        public N10ImperialCrownDeluxeGameLogic()
        {
            _gameID = GAMEID.N10ImperialCrownDeluxe;
            GameName = "10ImperialCrownDeluxe";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,5000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,10ÿ:m,10ÿ:b,18,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,5,5,5,5,1,8ÿx,6,jc=898:120:6:0,jsc=5192620.0:8.147954E7:1171080.0:0.0,j=-1,jhi=6.5473339E9:2.23537392E10:1.874810816E10:0.0,js=804715826:804704509:804640209:0,jhs=804103391:804381131:804030641:0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
