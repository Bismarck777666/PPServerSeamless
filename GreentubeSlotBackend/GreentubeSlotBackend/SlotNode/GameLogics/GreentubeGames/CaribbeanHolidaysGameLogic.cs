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
    internal class CaribbeanHolidaysGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
            get
            {
                return "P";
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ513ÿ1.0.0ÿCaribbean Holidays 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Caribbean Holidays 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,^^-^^ÿ:l,__-__ÿ:l,-^-_-ÿ:l,-_-^-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^---^ÿ:r,0,PQJ1QDJI1CKQW9AJG1KA1IJWAC1LQKÿ:r,0,KIQ1P9WJIKCJD9WQAIKLAJCK9WKDJQGAJI9C1AK1GQJÿ:r,0,KIAC1QL1WAGKCAWJD9WQC1JGQI1DAIQC9WKGJIAD9PQÿ:r,0,1LQDKC9WJI9KIJWQGACJI9JDK1LQDAJC9I1PKCJGAC9ÿ:r,0,ALKWQIAG9KQDA1G9PKIJWQCAKJW9KI9GK1GQIJQC1DAG1ÿ:r,1,PQJ1QDJI1CKQW9AJG1KA1IJWAC1LQKÿ:r,1,KIQ1P9WJIKCJD9WQAIKLAJCK9WKDJQGAJI9C1AK1GQJÿ:r,1,KIAC1QL1WAGKCAWJD9WQC1JGQI1DAIQC9WKGJIAD9PQÿ:r,1,1LQDKC9WJI9KIJWQGACJI9JDK1LQDAJC9I1PKCJGAC9ÿ:r,1,ALKWQIAG9KQDA1G9PKIJWQCAKJW9KI9GK1GQIJQC1DAG1ÿ:r,2,QPJ1QDJI1CKQW9AJG1LQJPKA1IJWAC1LQJKÿ:r,2,KIQ1P9WJIKCJD1P9WQAIKLAJCK9WKDJQGAJI9C1KLAK19GQJÿ:r,2,KIAC1P9QL1WAGKCAWJD9PQL1WQC1JGQI1DAIQC9WKGJIAD9PQÿ:r,2,1LQDKC9WJI9KIJWQGACJI9JD1PK1LQDAJC9I1PKCJGAC9ÿ:r,2,ALKWQIAG9KIQDA1G9PKIJWQCAKJW9PKI9GK1GQIJQC1DAG1ÿ:j,L,2,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,P,5,-550,0ÿ:w,P,4,-25,0ÿ:w,P,3,-5,0ÿ:w,P,2,-2,0ÿ:w,L,5,10000,0ÿ:w,L,4,2000,0ÿ:w,L,3,250,0ÿ:w,L,2,10,0ÿ:w,G,5,500,0ÿ:w,G,4,120,0ÿ:w,G,3,25,0ÿ:w,G,2,2,0ÿ:w,D,5,500,0ÿ:w,D,4,120,0ÿ:w,D,3,25,0ÿ:w,D,2,2,0ÿ:w,W,5,400,0ÿ:w,W,4,80,0ÿ:w,W,3,20,0ÿ:w,I,5,250,0ÿ:w,I,4,60,0ÿ:w,I,3,15,0ÿ:w,C,5,250,0ÿ:w,C,4,60,0ÿ:w,C,3,15,0ÿ:w,A,5,120,0ÿ:w,A,4,40,0ÿ:w,A,3,10,0ÿ:w,K,5,120,0ÿ:w,K,4,40,0ÿ:w,K,3,10,0ÿ:w,Q,5,100,0ÿ:w,Q,4,15,0ÿ:w,Q,3,5,0ÿ:w,J,5,100,0ÿ:w,J,4,15,0ÿ:w,J,3,5,0ÿ:w,1,5,100,0ÿ:w,1,4,15,0ÿ:w,1,3,5,0ÿ:w,9,5,100,0ÿ:w,9,4,15,0ÿ:w,9,3,5,0ÿ:w,9,2,2,0ÿ:wa,P,5,-550,0,0,0,0:1,0,0ÿ:wa,P,4,-25,0,0,0,0:1,0,0ÿ:wa,P,3,-5,0,0,0,0:1,0,0ÿ:wa,P,2,-2,0,0,0,0:1,0,0ÿ:wa,P,5,-1100,0,0,0,0:1,1:2,0ÿ:wa,P,4,-50,0,0,0,0:1,1:2,0ÿ:wa,P,3,-10,0,0,0,0:1,1:2,0ÿ:wa,P,2,-4,0,0,0,0:1,1:2,0ÿ:wa,L,5,10000,0,0,0,0:1,0,0ÿ:wa,L,4,2000,0,0,0,0:1,0,0ÿ:wa,L,3,250,0,0,0,0:1,0,0ÿ:wa,L,2,10,0,0,0,0:1,0,0ÿ:wa,L,5,20000,0,0,0,0:1,1:2,0ÿ:wa,L,4,4000,0,0,0,0:1,1:2,0ÿ:wa,L,3,500,0,0,0,0:1,1:2,0ÿ:wa,L,2,20,0,0,0,0:1,1:2,0ÿ:wa,G,5,500,0,0,0,0:1,0,0ÿ:wa,G,4,120,0,0,0,0:1,0,0ÿ:wa,G,3,25,0,0,0,0:1,0,0ÿ:wa,G,2,2,0,0,0,0:1,0,0ÿ:wa,G,5,1000,0,0,0,0:1,1:2,0ÿ:wa,G,4,240,0,0,0,0:1,1:2,0ÿ:wa,G,3,50,0,0,0,0:1,1:2,0ÿ:wa,G,2,4,0,0,0,0:1,1:2,0ÿ:wa,D,5,500,0,0,0,0:1,0,0ÿ:wa,D,4,120,0,0,0,0:1,0,0ÿ:wa,D,3,25,0,0,0,0:1,0,0ÿ:wa,D,2,2,0,0,0,0:1,0,0ÿ:wa,D,5,1000,0,0,0,0:1,1:2,0ÿ:wa,D,4,240,0,0,0,0:1,1:2,0ÿ:wa,D,3,50,0,0,0,0:1,1:2,0ÿ:wa,D,2,4,0,0,0,0:1,1:2,0ÿ:wa,W,5,400,0,0,0,0:1,0,0ÿ:wa,W,4,80,0,0,0,0:1,0,0ÿ:wa,W,3,20,0,0,0,0:1,0,0ÿ:wa,W,5,800,0,0,0,0:1,1:2,0ÿ:wa,W,4,160,0,0,0,0:1,1:2,0ÿ:wa,W,3,40,0,0,0,0:1,1:2,0ÿ:wa,I,5,250,0,0,0,0:1,0,0ÿ:wa,I,4,60,0,0,0,0:1,0,0ÿ:wa,I,3,15,0,0,0,0:1,0,0ÿ:wa,I,5,500,0,0,0,0:1,1:2,0ÿ:wa,I,4,120,0,0,0,0:1,1:2,0ÿ:wa,I,3,30,0,0,0,0:1,1:2,0ÿ:wa,C,5,250,0,0,0,0:1,0,0ÿ:wa,C,4,60,0,0,0,0:1,0,0ÿ:wa,C,3,15,0,0,0,0:1,0,0ÿ:wa,C,5,500,0,0,0,0:1,1:2,0ÿ:wa,C,4,120,0,0,0,0:1,1:2,0ÿ:wa,C,3,30,0,0,0,0:1,1:2,0ÿ:wa,A,5,120,0,0,0,0:1,0,0ÿ:wa,A,4,40,0,0,0,0:1,0,0ÿ:wa,A,3,10,0,0,0,0:1,0,0ÿ:wa,K,5,120,0,0,0,0:1,0,0ÿ:wa,K,4,40,0,0,0,0:1,0,0ÿ:wa,K,3,10,0,0,0,0:1,0,0ÿ:wa,A,5,240,0,0,0,0:1,1:2,0ÿ:wa,A,4,80,0,0,0,0:1,1:2,0ÿ:wa,A,3,20,0,0,0,0:1,1:2,0ÿ:wa,K,5,240,0,0,0,0:1,1:2,0ÿ:wa,K,4,80,0,0,0,0:1,1:2,0ÿ:wa,K,3,20,0,0,0,0:1,1:2,0ÿ:wa,Q,5,100,0,0,0,0:1,0,0ÿ:wa,Q,4,15,0,0,0,0:1,0,0ÿ:wa,Q,3,5,0,0,0,0:1,0,0ÿ:wa,J,5,100,0,0,0,0:1,0,0ÿ:wa,J,4,15,0,0,0,0:1,0,0ÿ:wa,J,3,5,0,0,0,0:1,0,0ÿ:wa,1,5,100,0,0,0,0:1,0,0ÿ:wa,1,4,15,0,0,0,0:1,0,0ÿ:wa,1,3,5,0,0,0,0:1,0,0ÿ:wa,Q,5,200,0,0,0,0:1,1:2,0ÿ:wa,Q,4,30,0,0,0,0:1,1:2,0ÿ:wa,Q,3,10,0,0,0,0:1,1:2,0ÿ:wa,J,5,200,0,0,0,0:1,1:2,0ÿ:wa,J,4,30,0,0,0,0:1,1:2,0ÿ:wa,J,3,10,0,0,0,0:1,1:2,0ÿ:wa,1,5,200,0,0,0,0:1,1:2,0ÿ:wa,1,4,30,0,0,0,0:1,1:2,0ÿ:wa,1,3,10,0,0,0,0:1,1:2,0ÿ:wa,9,5,100,0,0,0,0:1,0,0ÿ:wa,9,4,15,0,0,0,0:1,0,0ÿ:wa,9,3,5,0,0,0,0:1,0,0ÿ:wa,9,2,2,0,0,0,0:1,0,0ÿ:wa,9,5,200,0,0,0,0:1,1:2,0ÿ:wa,9,4,30,0,0,0,0:1,1:2,0ÿ:wa,9,3,10,0,0,0,0:1,1:2,0ÿ:wa,9,2,4,0,0,0,0:1,1:2,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,11ÿ:i,12ÿ:i,13ÿ:i,14ÿ:i,15ÿ:i,16ÿ:i,17ÿ:i,18ÿ:i,19ÿ:i,20ÿ:m,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20ÿ:a,0,0ÿ:g,999,1000,-1,false,0,trueÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,20,0,19,-1ÿb,0,0,0ÿs,1ÿr,0,5,24,10,33,13,5ÿrw,0";
            }
        }
        #endregion

        public CaribbeanHolidaysGameLogic()
        {
            _gameID = GAMEID.CaribbeanHolidays;
            GameName = "CaribbeanHolidays";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,3000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,11ÿ:i,12ÿ:i,13ÿ:i,14ÿ:i,15ÿ:i,16ÿ:i,17ÿ:i,18ÿ:i,19ÿ:i,20ÿ:m,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20ÿ:b,15,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150ÿ:a,0,0ÿ:g,999,1000,-1,false,0,trueÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,24,10,33,13,5ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
