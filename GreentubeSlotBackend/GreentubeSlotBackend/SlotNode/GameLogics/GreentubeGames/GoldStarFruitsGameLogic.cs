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
    internal class GoldStarFruitsGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ936ÿ1.10.54.0ÿEuroCoinInteractiveSlotServerÿ1934ÿ1.1.2ÿGold Star Fruits 95%ÿ1.114.0_2024-12-13_114711ÿ1.247.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49980,100.0,ÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Gold Star Fruits 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,^^-__ÿ:l,__-^^ÿ:l,-^^^-ÿ:l,-___-ÿ:l,_---^ÿ:r,0,SCCMMMCCCTTTGGGLLLSBBBCCCMMM777OOOLLLSMGGGMMMTTTGGGOOOÿ:r,0,SLOOOSCCC777GGGTTTCCCMMMBBBGGGOOOTTTLLL777MMMOOOBBBCCCLLÿ:r,0,SCLLLOOOBBCCCSBTTTMMMLLL777CCCGGGTTTOOOBBBGGGOOOSLLLMMMÿ:r,0,SOGGOOO777LLLSCCCTTTLLLMMMGSCCCMMMBBBSOOLLL777GGGCCCÿ:r,0,SCLLLMMMBBBOOOSGGG777CCCCMMMTTTLLLOOOTTTGGGCCCCBBBLLLOOOÿ:r,1,SCCMMMCCCTTTGGGSLLLBBBCCCMMM777OOOLLLSMGGGMMMTTTGGGOOO77ÿ:r,1,SLOOOSCCC777GGGTTTCCCMMMBBBGGGOOOTTTLLL777MMMOOOBBBCCSLLÿ:r,1,SCLLLOOOBBCCCSBTTTSMMMLLL777CCCGGGTTTOOOBBBGGGOOOLL7MMMGÿ:r,1,SOGGSOOO777LLLCCCTTTLLLMMMSGCCCMMMBBBOOSLLL777GGGCCCGGCCÿ:r,1,SCLLLSMMMBBBOOOGGG777CCCSMMMTTTLLLOOOTTTGGGCCCCBBBLLLOOOÿ:r,2,SCCLLLMMMCCCTTTOOOGGGLLLBBBCCCMMM77OOOLLLSMGGGMMMTTTGGGSOOOCCÿ:r,2,SCOOOCCCGGGTTTCCC777BGGOOOTTTLLLMOOOWCCCÿ:r,2,LLLBBSLLTOOOMMCCCWLL77GBBLLLMMMTGÿ:r,2,SOOOSGGGOOOSCCCMMM7TTSGCCCCOOOWLLLBBBGGGCCCÿ:r,2,SMLLLCCCMMMBBBOOGGG777CCCMMMTTTLLLOOOTTTGGGCCCCBBBLLLOOOÿ:r,3,SCCLLLMMMCCCTTTOOOGGGLLLBBBCCCMMM77OOOLLLSMGGGMMMTTTGGGSOOOCCÿ:r,3,SCOOOCCCGGGTTTCCC777BGGOOOTTTWLLLMOOOWCCCÿ:r,3,LLLBBSLLTOOOMMCCCWLL77GBBLLLMMMTGÿ:r,3,SOOOSGGGOOOWCCCMMM7TTSGCCCCSOOOWLLLBBBGGGCCCÿ:r,3,SMLLLCCCMMMBBBOOGGG777CCCMMMTTTLLLOOOTTTGGGCCCCBBBLLLOOOÿ:j,W,1,0,S7BTMGLOCWÿ:u,APPPP.PP.PP.PPPPÿ:w,X,1,-1,0ÿ:w,S,5,5000,0ÿ:w,S,4,1000,0ÿ:w,S,3,160,0ÿ:w,7,5,2500,0ÿ:w,7,4,800,0ÿ:w,7,3,120,0ÿ:w,B,5,2000,0ÿ:w,B,4,400,0ÿ:w,B,3,80,0ÿ:w,T,5,1000,0ÿ:w,T,4,160,0ÿ:w,T,3,40,0ÿ:w,M,5,200,0ÿ:w,M,4,80,0ÿ:w,M,3,20,0ÿ:w,G,5,200,0ÿ:w,G,4,80,0ÿ:w,G,3,20,0ÿ:w,O,5,160,0ÿ:w,O,4,40,0ÿ:w,O,3,10,0ÿ:w,L,5,160,0ÿ:w,L,4,40,0ÿ:w,L,3,10,0ÿ:w,C,5,160,0ÿ:w,C,4,40,0ÿ:w,C,3,10,0ÿ:w,X,1,1,0ÿ:wa,X,1,-1,0,0,0,0:1,-1,0ÿ:wa,S,5,5000,0,0,0,0:1,-1,0ÿ:wa,S,4,1000,0,0,0,0:1,-1,0ÿ:wa,S,4,1000,0,0,1,0:1,-1,0ÿ:wa,S,3,160,0,0,0,0:1,-1,0ÿ:wa,S,3,160,0,0,2,0:1,-1,0ÿ:wa,7,5,2500,0,0,0,0:1,-1,0ÿ:wa,7,4,800,0,0,0,0:1,-1,0ÿ:wa,7,4,800,0,0,1,0:1,-1,0ÿ:wa,7,3,120,0,0,0,0:1,-1,0ÿ:wa,7,3,120,0,0,2,0:1,-1,0ÿ:wa,B,5,2000,0,0,0,0:1,-1,0ÿ:wa,B,4,400,0,0,0,0:1,-1,0ÿ:wa,B,4,400,0,0,1,0:1,-1,0ÿ:wa,B,3,80,0,0,0,0:1,-1,0ÿ:wa,B,3,80,0,0,2,0:1,-1,0ÿ:wa,T,5,1000,0,0,0,0:1,-1,0ÿ:wa,T,4,160,0,0,0,0:1,-1,0ÿ:wa,T,4,160,0,0,1,0:1,-1,0ÿ:wa,T,3,40,0,0,0,0:1,-1,0ÿ:wa,T,3,40,0,0,2,0:1,-1,0ÿ:wa,M,5,200,0,0,0,0:1,-1,0ÿ:wa,M,4,80,0,0,0,0:1,-1,0ÿ:wa,M,4,80,0,0,1,0:1,-1,0ÿ:wa,M,3,20,0,0,0,0:1,-1,0ÿ:wa,M,3,20,0,0,2,0:1,-1,0ÿ:wa,G,5,200,0,0,0,0:1,-1,0ÿ:wa,G,4,80,0,0,0,0:1,-1,0ÿ:wa,G,4,80,0,0,1,0:1,-1,0ÿ:wa,G,3,20,0,0,0,0:1,-1,0ÿ:wa,G,3,20,0,0,2,0:1,-1,0ÿ:wa,O,5,160,0,0,0,0:1,-1,0ÿ:wa,O,4,40,0,0,0,0:1,-1,0ÿ:wa,O,4,40,0,0,1,0:1,-1,0ÿ:wa,O,3,10,0,0,0,0:1,-1,0ÿ:wa,O,3,10,0,0,2,0:1,-1,0ÿ:wa,L,5,160,0,0,0,0:1,-1,0ÿ:wa,L,4,40,0,0,0,0:1,-1,0ÿ:wa,L,4,40,0,0,1,0:1,-1,0ÿ:wa,L,3,10,0,0,0,0:1,-1,0ÿ:wa,L,3,10,0,0,2,0:1,-1,0ÿ:wa,C,5,160,0,0,0,0:1,-1,0ÿ:wa,C,4,40,0,0,0,0:1,-1,0ÿ:wa,C,4,40,0,0,1,0:1,-1,0ÿ:wa,C,3,10,0,0,0,0:1,-1,0ÿ:wa,C,3,10,0,0,2,0:1,-1,0ÿ:wa,X,1,1,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,10ÿ:m,10ÿ:b,16,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,0,0,0,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,10,0,0,0ÿb,1000,1000,0ÿ:x,320ÿs,1ÿr,1,5,15,5,13,37,7ÿx,7,CURRENT_SPIN=0,SUPER_METER_PREVIOUS=320.0,COLLECT_TO_SUPER_METER=f,SUPER_METER_WIN_PAYOUT=320.0,SUPER_METER=0.0,PREVIOUS_SUPER_BET=0,COLLECT_SUPER_METER=tÿc,X,1,0,0,0,0,0ÿrw,0";
            }
        }
        #endregion

        public GoldStarFruitsGameLogic()
        {
            _gameID = GAMEID.GoldStarFruits;
            GameName = "GoldStarFruits";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,3000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,10ÿ:m,10ÿ:b,16,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,0,0,0,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,5,15,5,13,37,7ÿx,7,CURRENT_SPIN=0,SUPER_METER_PREVIOUS=320.0,COLLECT_TO_SUPER_METER=f,SUPER_METER_WIN_PAYOUT=320.0,SUPER_METER=0.0,PREVIOUS_SUPER_BET=0,COLLECT_SUPER_METER=tÿc,X,1,0,0,0,0,0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
