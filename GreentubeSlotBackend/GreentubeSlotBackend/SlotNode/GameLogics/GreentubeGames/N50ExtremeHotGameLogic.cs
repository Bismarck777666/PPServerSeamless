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
    internal class N50ExtremeHotGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol
        {
            get
            {
                return "S";
            }
        }

        protected override int MinCountToFreespin
        {
            get
            {
                return 3;
            }
        }

        protected override string VersionCheckString
        {
            get
            {
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ11619ÿ1.10.54.0ÿEurocoinInteractiveSharedJackpotServerÿ12224ÿ1.5.1ÿ50 Extreme Hot 95%ÿ1.118.0_2025-03-28_090804ÿ1.68.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49950,100.0,ÿM,10,10000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,50 Extreme Hot 95%ÿ:v,4ÿ:l,^^^^^ÿ:l,-----ÿ:l,_____ÿ:l,VVVVVÿ:l,^-_-^ÿ:l,-_V_-ÿ:l,_-^-_ÿ:l,V_-_Vÿ:l,^^^^-ÿ:l,----^ÿ:l,____Vÿ:l,VVVV_ÿ:l,^----ÿ:l,----_ÿ:l,____-ÿ:l,V____ÿ:l,-^^^^ÿ:l,-____ÿ:l,_----ÿ:l,_VVVVÿ:l,^^^-_ÿ:l,---_Vÿ:l,___-^ÿ:l,VVV_-ÿ:l,^-___ÿ:l,-_VVVÿ:l,_-^^^ÿ:l,V_---ÿ:l,^^-^^ÿ:l,--^--ÿ:l,__V__ÿ:l,VV_VVÿ:l,^---^ÿ:l,--_--ÿ:l,__-__ÿ:l,V___Vÿ:l,-^^^-ÿ:l,-___-ÿ:l,_---_ÿ:l,_VVV_ÿ:l,-^-^-ÿ:l,_-_-_ÿ:l,V_V_Vÿ:l,^-^-^ÿ:l,-_-_-ÿ:l,_V_V_ÿ:l,-^-_-ÿ:l,_-_V_ÿ:l,-_-^-ÿ:l,_V_-_ÿ:r,0,CCCCGGGGSAAAABBBBJJJJCCCCPPOOOOMMLLLLWPPPPPPSMMMMGGWWWWLLLL77777OOOOGGGGAAAAPPPPPPMMMMOOOOÿ:r,0,CCCCMMMMJJJJPPPPOOOOMMCCCCLLLLWWWWWOOOOPP7777GGBBBBBALLLLSGGGG7AAAALLLLGGGG7AAAAÿ:r,0,CCCCBBBBAAAAAMMGGLLLLPPPPSCCCCOOOOJJWWWWWMMMMPPPBBOOOO7GGGG7777LLLLBBBBSAAAAAOOOOWWWWWLLLLÿ:r,0,LLLLWWWWACCCCB7777SGGGGMMAAAAJBBBB7PPPLLLLCCCCOOOMMMMJOOOOWGGPPPPLLLLSWWWWAAAABBBBMMMMOOOOÿ:r,0,PPP77OOAWWWW7MMMMJJPPPPWAALLLLSCCCCLLLL77GGBBBBBAACCCCGGGGMMOOOOOMMMMPPPPCCCCSGGGGMMSOOOOOÿ:r,1,CCCCGGGGSAAAABBBBJJJJCCCCPPOOOOMMLLLLWPPPPPPSMMMMGGWWWWLLLL77777OOOOÿ:r,1,CCCCMMMMJJJJPPPPOOOOMMCCCCLLLLWWWWWOOOOPP7777GGBBBBBALLLLSGGGG7AAAAÿ:r,1,CCCCBBBBAAAAAMMGGLLLLPPPPSCCCCOOOOJJWWWWWMMMMPPPBBOOOO7GGGG7777LLLLÿ:r,1,LLLLWWWWACCCCB7777SGGGGMMAAAAJBBBB7PPPLLLLCCCCOOOMMMMJOOOOWGGPPPPÿ:r,1,PPP77OOAWWWW7MMMMJJPPPPWAALLLLSCCCCLLLL77GGBBBBBAACCCCGGGGMMOOOOOÿ:r,2,JOPCLGG777BJLCPOJMWWOPJBAAMMLOCPJOCMMWWJPLGGBBJPOLCJMLPCJOW77GGPAACJOLCLÿ:r,2,JOLCWWJCLOGJMPBB77COLAAAWJMMGPCLOJWAABJCOLPGGJCLO777BBWJMMPCOLJPPOCJLGGMÿ:r,2,JCL77WWJCLOPGGJCAAACLOJPBBMMLOJP777JWCOLBBMMPJOCLAAOJGGPLCCJWWGGMMPLOJOPBBÿ:r,2,JCLOPGGMM777WWBBJCCCLOPJAAALCJOPMGLJOPWBJLGOPJLC77BWJGMMOPLCJAAWOCLJPBMGÿ:r,2,JLCOPAAG77BJCLOPGJMMWWLCJOPGAAMLCJPOBBWJCLGMMPOJCL777BBWJGPLJGMAACLJWPOOCÿ:j,J,1,0,7ABWGMOPLCÿ:u,APPPP.PP.PP.PPPPÿ:w,#,103,-5000,1:Grandÿ:w,#,102,-1250,1:Majorÿ:w,#,101,-200,1:Minorÿ:w,#,100,-50,1:Miniÿ:w,J,3,40,0ÿ:w,J,4,200,0ÿ:w,J,5,4000,0ÿ:w,7,3,20,0ÿ:w,7,4,80,0ÿ:w,7,5,400,0ÿ:w,A,3,20,0ÿ:w,A,4,80,0ÿ:w,A,5,400,0ÿ:w,B,3,20,0ÿ:w,B,4,60,0ÿ:w,B,5,240,0ÿ:w,W,3,20,0ÿ:w,W,4,60,0ÿ:w,W,5,240,0ÿ:w,G,3,8,0ÿ:w,G,4,40,0ÿ:w,G,5,160,0ÿ:w,M,3,8,0ÿ:w,M,4,40,0ÿ:w,M,5,160,0ÿ:w,O,3,4,0ÿ:w,O,4,20,0ÿ:w,O,5,80,0ÿ:w,P,3,4,0ÿ:w,P,4,20,0ÿ:w,P,5,80,0ÿ:w,L,3,4,0ÿ:w,L,4,20,0ÿ:w,L,5,80,0ÿ:w,C,3,4,0ÿ:w,C,4,20,0ÿ:w,C,5,80,0ÿ:wa,#,103,-5000,1:Grand,0,0,0:1,-1,0ÿ:wa,#,102,-1250,1:Major,0,0,0:1,-1,0ÿ:wa,#,101,-200,1:Minor,0,0,0:1,-1,0ÿ:wa,#,100,-50,1:Mini,0,0,0:1,-1,0ÿ:wa,J,3,40,0,0,0,0:1,-1,0ÿ:wa,J,4,200,0,0,0,0:1,-1,0ÿ:wa,J,5,4000,0,0,0,0:1,-1,0ÿ:wa,7,3,20,0,0,0,0:1,-1,0ÿ:wa,7,4,80,0,0,0,0:1,-1,0ÿ:wa,7,5,400,0,0,0,0:1,-1,0ÿ:wa,A,3,20,0,0,0,0:1,-1,0ÿ:wa,A,4,80,0,0,0,0:1,-1,0ÿ:wa,A,5,400,0,0,0,0:1,-1,0ÿ:wa,B,3,20,0,0,0,0:1,-1,0ÿ:wa,B,4,60,0,0,0,0:1,-1,0ÿ:wa,B,5,240,0,0,0,0:1,-1,0ÿ:wa,W,3,20,0,0,0,0:1,-1,0ÿ:wa,W,4,60,0,0,0,0:1,-1,0ÿ:wa,W,5,240,0,0,0,0:1,-1,0ÿ:wa,G,3,8,0,0,0,0:1,-1,0ÿ:wa,G,4,40,0,0,0,0:1,-1,0ÿ:wa,G,5,160,0,0,0,0:1,-1,0ÿ:wa,M,3,8,0,0,0,0:1,-1,0ÿ:wa,M,4,40,0,0,0,0:1,-1,0ÿ:wa,M,5,160,0,0,0,0:1,-1,0ÿ:wa,O,3,4,0,0,0,0:1,-1,0ÿ:wa,O,4,20,0,0,0,0:1,-1,0ÿ:wa,O,5,80,0,0,0,0:1,-1,0ÿ:wa,P,3,4,0,0,0,0:1,-1,0ÿ:wa,P,4,20,0,0,0,0:1,-1,0ÿ:wa,P,5,80,0,0,0,0:1,-1,0ÿ:wa,L,3,4,0,0,0,0:1,-1,0ÿ:wa,L,4,20,0,0,0,0:1,-1,0ÿ:wa,L,5,80,0,0,0,0:1,-1,0ÿ:wa,C,3,4,0,0,0,0:1,-1,0ÿ:wa,C,4,20,0,0,0,0:1,-1,0ÿ:wa,C,5,80,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,50ÿ:m,25ÿ:b,16,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1600,-1,false,0,falseÿ:es,0,0ÿ:mbnr,25.0ÿbs,0,1,0ÿ:k,nullÿe,25,0,0,0ÿb,1000,1000,0ÿs,1ÿr,1,5,4,35,56,38,62ÿx,1,j=-1ÿrw,0";
            }
        }
        #endregion

        public N50ExtremeHotGameLogic()
        {
            _gameID = GAMEID.N50ExtremeHot;
            GameName = "50ExtremeHot";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,25,5000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,50ÿ:m,25ÿ:b,16,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1600,-1,false,0,falseÿ:es,0,0ÿ:mbnr,25.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,5,4,35,56,38,62ÿx,6,jc=2956:476:17:2,jsc=1.33198505E9:2.1679091025E10:1.241340775E9:5.09772698E10,j=-1,jhi=8.678281825E9:2.604701035E10:1.20563543E10:5.09772698E10,js=805318649:805316030:805204788:805317306,jhs=805312121:804963279:804644890:805317306ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
