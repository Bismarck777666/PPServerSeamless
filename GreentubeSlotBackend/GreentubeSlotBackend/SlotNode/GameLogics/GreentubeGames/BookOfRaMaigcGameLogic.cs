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
    internal class BookOfRaMagicGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol
        {
            get
            {
                return "B";
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ919ÿ1.10.54.0ÿNGI_GS2_Slot_Serverÿ3220ÿ1.0.0ÿBook of Ra Magic 95%ÿ1.102.0_2024-02-26_095937ÿ1.88.1";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Book of Ra Magic 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,TQKJAQTAMBJAQTKASTQKCAJKSQTKMQKSJAKCQKJSQTAJTMAKTJQSATEAQKJTQKJAQTABJAQTKASTQKCAJKSQTKMQKSJAKCQKJSQTAJTQMAKTQSATEAQKJTQKJCAQTABJAQTKASTQKCAJKSQTKMQKSJAKCQKJSQTAJTMAKTQSATEAQKJÿ:r,0,ATEQTBJTAMJKTJCKQTMKAJTKJTKCJTKETQJTCJKAQSJTKQATEQTBJAMJKTJCKQTMKAJTKJTKCJTKETQJTCJKASJTKQATEQTBJAMJKTJCKQTMKAJTKJTKCJTKETQJTCJKASJTKQÿ:r,0,SAQKBQAMCQSAQSKAEJASQJMQTCQJESTQMKATCJQSTACJQTSQKSBQACTQSAQSKAEJASQJMQTCQJETQMKATCJQSTACJEQTSQKBSQACQSAQSKAEJASQJMQTCQJETQMKATCJQSTACJQTÿ:r,0,TQCTACJBTJEAJCTJAETJKASJKBQACQTMKACJQKSTACJTQKMTJQTESATQCTACJBTJEAJCTJQAETJKASJKBQACQTMKATCJQKESTACBJTQKMTJQTSATQCTACJBTJEAJCTJAETJKASTJKBQACQTMQKACJQKSTACJTQKMTJQTSAÿ:r,0,CKJBKQMKSQMACJMQSATBQJMAKSQKCMASTCKQEJKSTJEKACKJBKQMKSQMACJMQSATBQJMAKSQKCASTCKQEJKSTJEKACKJBKQEMKSQMACJMQSATBQJMAKSQKCASTCKQEJKSTJEKAÿ:r,1,TJMKJEKATKJAMTAJMQTBKJMQKACQJSKACMQATJMKCJEKATKJAKMTAJMQCTBKJMQKACQJSKACQATJMKJEKATKJAMCTAJMQTBCKJMQKACQJSKAJCQAÿ:r,1,KQSKTCQJKQAKTJEQAJCQBATKSAJQTKMJAQJTCATKQSKTCQJKQAKTJEQAJCQBATKSAJQTKMJAQJTCATKQSKJTCQJKQAKTJEQAJCQBATKSAJQTKMJAQJTKCATÿ:r,1,QBAQEJQSJCQAETAMQAJKETJABTKJMTQSKJQBAQEJQSJCQAETAMQAJKETJABTKJMTQSKJQBAQEJQSJCQAETAMQAJKSETJABTKJEMTQSKJÿ:r,1,QJBATCKMJEASKQCTKSQTCAJTEJABKTSKQTJCQJBATCKMJEASKQCTKSQTCAJTKEJABKTSKQATJCQJBATCKMJEASKQCTKSQTCAJTEJABKTSJKQTJCÿ:r,1,JATCQTBQCKMQSTEQBKJMASJACEJTSAEKQCTMQSAKCQMTEJTCQTBQCKEMQSTQBKJMASJAEJTSAEKCTMQSKCQMTEJTCQTBQCKMQSTQBKJMASQJAEJTSAEJKCTMQESKCQMTEÿ:j,B,1,0,EMSCAKQJTÿ:u,APPPP.PP.PP.PPPPÿ:w,B,3,-2,0ÿ:w,B,4,-20,0ÿ:w,B,5,-200,0ÿ:w,E,2,10,0ÿ:w,E,3,100,0ÿ:w,E,4,1000,0ÿ:w,E,5,5000,0ÿ:w,M,2,5,0ÿ:w,M,3,40,0ÿ:w,M,4,400,0ÿ:w,M,5,2000,0ÿ:w,S,2,5,0ÿ:w,S,3,30,0ÿ:w,S,4,100,0ÿ:w,S,5,750,0ÿ:w,C,2,5,0ÿ:w,C,3,30,0ÿ:w,C,4,100,0ÿ:w,C,5,750,0ÿ:w,A,3,5,0ÿ:w,A,4,40,0ÿ:w,A,5,150,0ÿ:w,K,3,5,0ÿ:w,K,4,40,0ÿ:w,K,5,150,0ÿ:w,Q,3,5,0ÿ:w,Q,4,25,0ÿ:w,Q,5,100,0ÿ:w,J,3,5,0ÿ:w,J,4,25,0ÿ:w,J,5,100,0ÿ:w,T,3,5,0ÿ:w,T,4,25,0ÿ:w,T,5,100,0ÿ:wa,B,3,-2,0,0,0,0:1,-1,0ÿ:wa,B,4,-20,0,0,0,0:1,-1,0ÿ:wa,B,5,-200,0,0,0,0:1,-1,0ÿ:wa,D,2,-10,0,0,0,0:1,1,0ÿ:wa,D,3,-100,0,0,0,0:1,1,0ÿ:wa,D,4,-1000,0,0,0,0:1,1,0ÿ:wa,D,5,-5000,0,0,0,0:1,1,0ÿ:wa,F,2,-5,0,0,0,0:1,1,0ÿ:wa,F,3,-40,0,0,0,0:1,1,0ÿ:wa,F,4,-400,0,0,0,0:1,1,0ÿ:wa,F,5,-2000,0,0,0,0:1,1,0ÿ:wa,G,2,-5,0,0,0,0:1,1,0ÿ:wa,G,3,-30,0,0,0,0:1,1,0ÿ:wa,G,4,-100,0,0,0,0:1,1,0ÿ:wa,G,5,-750,0,0,0,0:1,1,0ÿ:wa,H,2,-5,0,0,0,0:1,1,0ÿ:wa,H,3,-30,0,0,0,0:1,1,0ÿ:wa,H,4,-100,0,0,0,0:1,1,0ÿ:wa,H,5,-750,0,0,0,0:1,1,0ÿ:wa,I,3,-5,0,0,0,0:1,1,0ÿ:wa,I,4,-40,0,0,0,0:1,1,0ÿ:wa,I,5,-150,0,0,0,0:1,1,0ÿ:wa,L,3,-5,0,0,0,0:1,1,0ÿ:wa,L,4,-40,0,0,0,0:1,1,0ÿ:wa,L,5,-150,0,0,0,0:1,1,0ÿ:wa,O,3,-5,0,0,0,0:1,1,0ÿ:wa,O,4,-25,0,0,0,0:1,1,0ÿ:wa,O,5,-100,0,0,0,0:1,1,0ÿ:wa,P,3,-5,0,0,0,0:1,1,0ÿ:wa,P,4,-25,0,0,0,0:1,1,0ÿ:wa,P,5,-100,0,0,0,0:1,1,0ÿ:wa,R,3,-5,0,0,0,0:1,1,0ÿ:wa,R,4,-25,0,0,0,0:1,1,0ÿ:wa,R,5,-100,0,0,0,0:1,1,0ÿ:wa,E,2,10,0,0,0,0:1,-1,0ÿ:wa,E,3,100,0,0,0,0:1,-1,0ÿ:wa,E,4,1000,0,0,0,0:1,-1,0ÿ:wa,E,5,5000,0,0,0,0:1,-1,0ÿ:wa,M,2,5,0,0,0,0:1,-1,0ÿ:wa,M,3,40,0,0,0,0:1,-1,0ÿ:wa,M,4,400,0,0,0,0:1,-1,0ÿ:wa,M,5,2000,0,0,0,0:1,-1,0ÿ:wa,S,2,5,0,0,0,0:1,-1,0ÿ:wa,S,3,30,0,0,0,0:1,-1,0ÿ:wa,S,4,100,0,0,0,0:1,-1,0ÿ:wa,S,5,750,0,0,0,0:1,-1,0ÿ:wa,C,2,5,0,0,0,0:1,-1,0ÿ:wa,C,3,30,0,0,0,0:1,-1,0ÿ:wa,C,4,100,0,0,0,0:1,-1,0ÿ:wa,C,5,750,0,0,0,0:1,-1,0ÿ:wa,A,3,5,0,0,0,0:1,-1,0ÿ:wa,A,4,40,0,0,0,0:1,-1,0ÿ:wa,A,5,150,0,0,0,0:1,-1,0ÿ:wa,K,3,5,0,0,0,0:1,-1,0ÿ:wa,K,4,40,0,0,0,0:1,-1,0ÿ:wa,K,5,150,0,0,0,0:1,-1,0ÿ:wa,Q,3,5,0,0,0,0:1,-1,0ÿ:wa,Q,4,25,0,0,0,0:1,-1,0ÿ:wa,Q,5,100,0,0,0,0:1,-1,0ÿ:wa,J,3,5,0,0,0,0:1,-1,0ÿ:wa,J,4,25,0,0,0,0:1,-1,0ÿ:wa,J,5,100,0,0,0,0:1,-1,0ÿ:wa,T,3,5,0,0,0,0:1,-1,0ÿ:wa,T,4,25,0,0,0,0:1,-1,0ÿ:wa,T,5,100,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,9,-1ÿb,0,0,0ÿs,1ÿr,0,5,65,24,9,24,38ÿx,3,,,10ÿrw,0";
            }
        }
        #endregion

        public BookOfRaMagicGameLogic()
        {
            _gameID = GAMEID.BookOfRaMagic;
            GameName = "BookOfRaMagic";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,5,5000,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:b,15,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,50.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,65,24,9,24,38ÿx,3,,,10ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
                resMessage.Append("\vFK_SERVERÿÿGAMEIDÿ919ÿCATEGORYÿ1ÿTOURNIDÿÿTOURNENTRYFEEÿ0ÿISTOURNBYINVITATIONONLYÿ0ÿCALLSTARTGAMEÿ1ÿPRIORITYÿ1ÿTOURNDEADLINEHITÿ0ÿGUESTSÿ0ÿCDATEÿ2025-06-28 20:04:22.11ÿDISPLAYORDERÿ1ÿCHATFILTERÿ1ÿRANKINGÿ0ÿPORTÿÿASSIGNMENTINTERVALÿ180ÿGAMEVARIATIONÿ3220ÿTICKETSÿ2ÿMAXPLAYERSÿ100000ÿMUXIPÿ");
                resMessage.Append(GameUniqueString);

                resMessage.Append("%t0ÿq0");
                resMessage.Append($"\aNATIVEJACKPOTCURRENCYFACTORÿ100.0ÿMAXBUYINTOTALÿ0ÿNEEDSSESSIONTIMEOUTÿ0ÿRELIABILITYÿ0ÿNICKNAMEÿ155258EURÿIDÿ155258ÿISDEEPWALLETÿ0ÿCURRENCYFACTORÿ100.0ÿMINBUYINÿ1ÿSESSIONTIMEOUTMINUTESÿ60ÿJACKPOT_PROGRESSIVE_CAPÿ0.0ÿNATIVEJACKPOTCURRENCYÿ{getCurrencyCode(currency)}ÿEXTUSERIDÿDummyDB_ExternalUserId_155258EURÿFREESPINSISCAPREACHEDÿ0ÿCURRENCYÿ{getCurrencySymbol(currency)}ÿTICKETSÿ0ÿJACKPOT_TOTAL_CAPÿ0.0ÿJACKPOTFEEPERCENTAGEÿ0.0ÿISSHOWJACKPOTCONTRIBUTIONÿ0");
                resMessage.Append(buildLineBetString(strGlobalUserID, balance));

                ToUserMessage toUserMessage = new ToUserMessage((int)_gameID, resMessage);

                Sender.Tell(toUserMessage, Self);

                //Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
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
