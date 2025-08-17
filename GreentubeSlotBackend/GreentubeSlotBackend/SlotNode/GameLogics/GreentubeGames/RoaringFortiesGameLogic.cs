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
    internal class RoaringFortiesGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ103ÿ1.0.0ÿRoaring Forties 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,40,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Roaring Forties 95%ÿ:v,4ÿ:l,^^^^^ÿ:l,-----ÿ:l,_____ÿ:l,VVVVVÿ:l,^-_-^ÿ:l,-_V_-ÿ:l,_-^-_ÿ:l,V_-_Vÿ:l,^^^^-ÿ:l,----^ÿ:l,____Vÿ:l,VVVV_ÿ:l,^----ÿ:l,----_ÿ:l,____-ÿ:l,V____ÿ:l,-^^^^ÿ:l,-____ÿ:l,_----ÿ:l,_VVVVÿ:l,^^^-_ÿ:l,---_Vÿ:l,___-^ÿ:l,VVV_-ÿ:l,^-___ÿ:l,-_VVVÿ:l,_-^^^ÿ:l,V_---ÿ:l,^^-^^ÿ:l,--^--ÿ:l,__V__ÿ:l,VV_VVÿ:l,^---^ÿ:l,--_--ÿ:l,__-__ÿ:l,V___Vÿ:l,-^^^-ÿ:l,-___-ÿ:l,_---_ÿ:l,_VVV_ÿ:r,0,77777LLLLPPCCGGGGSOOMMMMBBSPPPPLLOOOOSCCCCOOOBBBBSCCGGGGPPPMMLLLLCCCCOOOOGGMMMMLL77777LLLLOOOPPCCGGGGSOOMMMMCCCCBBSPPPPLLOOOOSCCCCOOOBBBBSCCGGGGPPPMMLLLLCCCCOOOOGGMMMMLL77777LLLLPPCCGGGGSOOMMMMBBCCCCSPPPPLLOOOOSCCCCOOOBBBBSCCGGGGPPPMMLLLLCCCCOOOOGGMMMMLL77777LLLLPPCCGGGGSOOMMMMBBSPPPPLLOOOOSCCCCOOOBBBBSCCGGGGPPPMMLLLLCCCCOOOOGGMMMMLLÿ:r,0,7777CCCCSOOOLLLLMMMMSCCCC77PPLLLLGGGGWWWWCCCCSPPPPBBBBLLLLPPBBLLLLSCCCC7777CCCCSOOOLLLLMMMMSCCCC77PPLLLLGGGGWWWWCCCCSPPPPBBBBLLLLPPBBLLLLSCCCC7777CCCCSOOOLLLLMMMMSCCCC77PPLLLLGGGGWWWWBBCCCCSPPPPBBBBLLLLPPBBLLLLSCCCC7777CCCCSOOOLLLLMMMMSCCCC77PPLLLLGGGGWWWWCCCCSPPPPBBBBLLLLPPBBLLLLSCCCCÿ:r,0,7777OOOOPPPPBBBBSMMMMWWWWWPPPPLLLGGGGSOOOOCCCBBBOOOO777PPOOOO7777OOOOPPPPBBBBSMMMMWWWWWPPPPLLLGGGGSOOOOCCCBBBOOOO777PPOOOO7777OOOOPPPPBBBBSMMMMWWWWWPPPP777LLLGGGGSOOOOCCCBBBOOOO777PPOOOO7777OOOOPPPPBBBBSMMMMWWWWWPPPPLLLGGGGSOOOOCCCBBBOOOO777PPOOOOÿ:r,0,77777BBBBGGGGLLLLLPPPPOOOOO7777MMMMBBBWWWWWBBBBCCCCCMMMMMWWWWWGGGGGSPPPPPBBBB77777BBBBGGGGLLLLLPPPPOOOOO7777MMMMBBBWWWWWCCCCCMMMMMWWWWWGGGGGSPPPPPBBBB77777BBBBGGGGLLLLLBBBPPPPOOOOO7777MMMMBBBWWWWWCCCCCMMMMMWWWWWGGGGGSPPPPPBBBB77777BBBBGGGGLLLLLPPPPOOOOO7777MMMMBBBWWWWWCCCCCMMMMMWWWWWGGGGGSPPPPPBBBBÿ:r,0,77777BBBBGGGGLLLLCCCCSMMMMMLLPPPPP7777OOOOCCBBBBPPPP77777OOBBBBBMMMMPPGGGGG77777BBBBGGGGLLLLCCCCSMMMMMLLPPPPP7777OOOOCCBBBBPPPP77777OOBBBBBMMMMPPGGGGG77777BBBBGGGGLLLLCCCCSMMMMMLLPPPPP7777OOOOCCBBBBPPPP77777OOBBBBBMMMMPPGGGGG77777BBBBGGGGLLLLCCCCSMMMMMLLPPPPP7777OOOOCCBBBBPPPP77777OOBBBBBMMMMPPGGGGGÿ:j,W,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,S,5,-500,0ÿ:w,S,4,-20,0ÿ:w,S,3,-2,0ÿ:w,7,5,1000,0ÿ:w,7,4,200,0ÿ:w,7,3,60,0ÿ:w,7,2,4,0ÿ:w,B,5,300,0ÿ:w,B,4,100,0ÿ:w,B,3,40,0ÿ:w,M,5,200,0ÿ:w,M,4,80,0ÿ:w,M,3,20,0ÿ:w,G,5,200,0ÿ:w,G,4,80,0ÿ:w,G,3,20,0ÿ:w,P,5,100,0ÿ:w,P,4,40,0ÿ:w,P,3,8,0ÿ:w,L,5,100,0ÿ:w,L,4,40,0ÿ:w,L,3,8,0ÿ:w,O,5,100,0ÿ:w,O,4,40,0ÿ:w,O,3,8,0ÿ:w,C,5,100,0ÿ:w,C,4,40,0ÿ:w,C,3,8,0ÿ:wa,S,5,-500,0,0,0,0:1,-1,0ÿ:wa,S,4,-20,0,0,0,0:1,-1,0ÿ:wa,S,3,-2,0,0,0,0:1,-1,0ÿ:wa,7,5,1000,0,0,0,0:1,-1,0ÿ:wa,7,4,200,0,0,0,0:1,-1,0ÿ:wa,7,3,60,0,0,0,0:1,-1,0ÿ:wa,7,2,4,0,0,0,0:1,-1,0ÿ:wa,B,5,300,0,0,0,0:1,-1,0ÿ:wa,B,4,100,0,0,0,0:1,-1,0ÿ:wa,B,3,40,0,0,0,0:1,-1,0ÿ:wa,M,5,200,0,0,0,0:1,-1,0ÿ:wa,M,4,80,0,0,0,0:1,-1,0ÿ:wa,M,3,20,0,0,0,0:1,-1,0ÿ:wa,G,5,200,0,0,0,0:1,-1,0ÿ:wa,G,4,80,0,0,0,0:1,-1,0ÿ:wa,G,3,20,0,0,0,0:1,-1,0ÿ:wa,P,5,100,0,0,0,0:1,-1,0ÿ:wa,P,4,40,0,0,0,0:1,-1,0ÿ:wa,P,3,8,0,0,0,0:1,-1,0ÿ:wa,L,5,100,0,0,0,0:1,-1,0ÿ:wa,L,4,40,0,0,0,0:1,-1,0ÿ:wa,L,3,8,0,0,0,0:1,-1,0ÿ:wa,O,5,100,0,0,0,0:1,-1,0ÿ:wa,O,4,40,0,0,0,0:1,-1,0ÿ:wa,O,3,8,0,0,0,0:1,-1,0ÿ:wa,C,5,100,0,0,0,0:1,-1,0ÿ:wa,C,4,40,0,0,0,0:1,-1,0ÿ:wa,C,3,8,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,40ÿ:m,40ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,40,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,55,37,3,51,21ÿrw,0";
            }
        }
        #endregion

        public RoaringFortiesGameLogic()
        {
            _gameID = GAMEID.RoaringForties;
            GameName = "RoaringForties";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,40,3000,40,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,40ÿ:m,40ÿ:b,12,1,2,3,4,5,8,10,15,20,30,40,50ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,40.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},0,0ÿb,1000,1000,0ÿs,1ÿr,0,5,55,37,3,51,21ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine));
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
