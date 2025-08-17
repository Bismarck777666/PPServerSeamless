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
    internal class SuperCherry2000GameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ898ÿ1.10.54.0ÿGTSKSlotServerÿ10938ÿ1.0.1ÿSuper Cherry 2000 94%ÿ1.120.0_2025-05-09_075230ÿ1.313.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49920,100.0,ÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Super Cherry 2000 94%ÿ:v,3ÿ:l,---ÿ:l,-1--ÿ:r,0,MLOGCDSPODC1LGMPGSLDBDGOÿ:r,0,GLCPLSMDGODBGCDLMSOC1GODÿ:r,0,DGLCOSGLMG1DOMCGBPSDLDOPÿ:r,1,MLOGCDSPODC1LGMPGSLDBDGOÿ:r,1,GLCPLSMDGODBGCDLMSOC1GODÿ:r,1,DGLCOSGLMG1DOMCGBPSDLDOPÿ:r,2,MLOGCDSPODC1LGMPGSLDBDGOÿ:r,2,GLCPLSMDGODBGCDLMSOC1GODÿ:r,2,DGLCOSGLMG1DOMCGBPSDLDOPÿ:r,3,MLOGCDSPODC1LGMPGSLDBDGOÿ:r,3,GLCPLSMDGODBGCDLMSOC1GODÿ:r,3,DGLCOSGLMG1DOMCGBPSDLDOPÿ:r,4,MLOGCDSPODC1LGMPGSLDBDGOÿ:r,4,GLCPLSMDGODBGCDLMSOC1GODÿ:r,4,DGLCOSGLMG1DOMCGBPSDLDOPÿ:r,5,MLOGCDSPODC1LGMPGSLDBDGOÿ:r,5,GLCPLSMDGODBGCDLMSOC1GODÿ:r,5,DGLCOSGLMG1DOMCGBPSDLDOPÿ:u,APPPP.PP.PP.PPPPÿ:w,X,1,-1,0ÿ:w,#,111,-1,0ÿ:w,C,105,-2000,0ÿ:w,C,104,-125,0ÿ:w,C,103,-25,0ÿ:w,C,102,-15,0ÿ:w,C,101,-5,0ÿ:w,S,3,100,0ÿ:w,1,3,500,0ÿ:w,B,3,50,0ÿ:w,C,3,20,0ÿ:w,M,3,10,0ÿ:w,P,3,10,0ÿ:w,O,3,5,0ÿ:w,L,3,5,0ÿ:w,D,3,2,0ÿ:w,G,3,2,0ÿ:w,C,1,2,0ÿ:wa,X,1,-1,0,0,0,0:1,0,0ÿ:wa,X,1,-1,0,0,0,0:1,1,0ÿ:wa,X,1,-1,0,0,0,0:1,2,0ÿ:wa,X,1,-1,0,0,0,0:1,3,0ÿ:wa,X,1,-1,0,0,0,0:1,4,0ÿ:wa,X,1,-1,0,0,0,0:1,5,0ÿ:wa,X,1,-1,0,0,0,0:1,6,0ÿ:wa,#,111,-1,0,0,0,0:1,0:1:2,0ÿ:wa,#,111,-1,0,0,0,0:1,3,0ÿ:wa,#,111,-1,0,0,0,0:1,4,0ÿ:wa,#,111,-1,0,0,0,0:1,5,0ÿ:wa,C,105,-2000,0,0,0,0:1,-1,0ÿ:wa,C,104,-125,0,0,0,0:1,-1,0ÿ:wa,C,103,-25,0,0,0,0:1,-1,0ÿ:wa,C,102,-15,0,0,0,0:1,-1,0ÿ:wa,C,101,-5,0,0,0,0:1,-1,0ÿ:wa,Z,2,-4,0,0,0,0:1,1,0ÿ:wa,S,3,100,0,0,0,0:1,0:1:2:3:4,0ÿ:wa,S,3,100,0,0,0,0:1,5,0ÿ:wa,1,3,500,0,0,0,0:1,0:1:2,0ÿ:wa,1,3,500,0,0,0,0:1,3,0ÿ:wa,1,3,500,0,0,0,0:1,4,0ÿ:wa,1,3,500,0,0,0,0:1,5,0ÿ:wa,B,3,50,0,0,0,0:1,0:1:2,0ÿ:wa,B,3,50,0,0,0,0:1,3,0ÿ:wa,B,3,50,0,0,0,0:1,4,0ÿ:wa,B,3,50,0,0,0,0:1,5,0ÿ:wa,C,3,20,0,0,0,0:1,0,0ÿ:wa,C,3,20,0,0,0,0:1,2,0ÿ:wa,C,3,20,0,0,0,0:1,3,0ÿ:wa,C,3,20,0,0,0,0:1,4,0ÿ:wa,M,3,10,0,0,0,0:1,0:1:2,0ÿ:wa,M,3,10,0,0,0,0:1,3,0ÿ:wa,M,3,10,0,0,0,0:1,4,0ÿ:wa,M,3,10,0,0,0,0:1,5,0ÿ:wa,P,3,10,0,0,0,0:1,0:1:2,0ÿ:wa,P,3,10,0,0,0,0:1,3,0ÿ:wa,P,3,10,0,0,0,0:1,4,0ÿ:wa,P,3,10,0,0,0,0:1,5,0ÿ:wa,O,3,5,0,0,0,0:1,0:1:2,0ÿ:wa,O,3,5,0,0,0,0:1,3,0ÿ:wa,O,3,5,0,0,0,0:1,4,0ÿ:wa,O,3,5,0,0,0,0:1,5,0ÿ:wa,L,3,5,0,0,0,0:1,0:1:2,0ÿ:wa,L,3,5,0,0,0,0:1,3,0ÿ:wa,L,3,5,0,0,0,0:1,4,0ÿ:wa,L,3,5,0,0,0,0:1,5,0ÿ:wa,D,3,2,0,0,0,0:1,0:1:2,0ÿ:wa,D,3,2,0,0,0,0:1,3,0ÿ:wa,D,3,2,0,0,0,0:1,4,0ÿ:wa,D,3,2,0,0,0,0:1,5,0ÿ:wa,G,3,2,0,0,0,0:1,0:1:2,0ÿ:wa,G,3,2,0,0,0,0:1,3,0ÿ:wa,G,3,2,0,0,0,0:1,4,0ÿ:wa,G,3,2,0,0,0,0:1,5,0ÿ:wa,C,2,4,0,0,0,0:1,1,0ÿ:wa,C,2,4,0,0,1,0:1,1,0ÿ:wa,C,1,2,0,0,0,0:1,0,0ÿ:wa,C,1,2,0,0,2,0:1,0,0ÿ:s,0ÿ:i,1ÿ:m,1ÿ:b,13,20,30,40,50,80,100,150,200,300,400,500,800,1000ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,1,0,0,0ÿb,1000,1000,0ÿs,1ÿr,0,3,23,4,4ÿx,1,H:0ÿrw,0";
            }
        }
        #endregion

        public SuperCherry2000GameLogic()
        {
            _gameID = GAMEID.SuperCherry2000;
            GameName = "SuperCherry2000";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,1000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:m,1ÿ:b,13,20,30,40,50,80,100,150,200,300,400,500,800,1000ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,3,23,4,4ÿx,1,H:0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
