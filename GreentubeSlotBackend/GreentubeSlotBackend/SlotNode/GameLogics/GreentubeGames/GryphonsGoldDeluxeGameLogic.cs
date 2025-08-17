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
    internal class GryphonsGoldDeluxeGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
            get
            {
                return "H";
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ614ÿ1.0.0ÿGryphons Gold Deluxe 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Gryphons Gold Deluxe 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,C1LQ9K1AB1CKQSAQHJQRJP1KJ1PJSAC1LQ9K1AB1CKQSAQHJQRJP1KJ1PJSAC1LQ9K1AB1CKQSAQHJQRJP1KQJ1PJSAÿ:r,0,PKLAK1BQJ9C1ARJQBAQPK1H9SJPKCRJA9SKJCKJP9SQAPKLAKJ1BQJ9C1ARJQBAQPK1H9SJPKCQRJ9SKJCKJP9KSQAPKLAK1BQJP9C1ARJQBAQPK1H9SJPKCRJ9SKJACKJP9SQAÿ:r,0,1QL1SAB9CQSJR9P1JBQ9HQ1RAPQCSKBJPAR9SQCAK9PAC1QL1SAB9CSJR9P1JBQ9HQ1RAPQCSKBJPAR9SQCAKPAC1QL1SAB9CSJR9P1JBQ9HQ1RAPQCSKBJPAR9SQCA1KPACÿ:r,0,K1LQRAJSQBACJP9LJC9KRP1HKCJBAC9PK1L9QRC9ASJP9JRK1LQKRAJSQBACJP9JRC9KP1HKCJBAJC9PK1LQRC9SJP9JRK1LQRAJSQBACJP9JC9KP1HKCJBAC9PK1LQRC9SJP9JRÿ:r,0,KJALKSQPAQC1RAKBQPJ1B9HPJSQ9CAB9KAB9QRKJS9QPK9BKAQLKSQPAQC1RAKBQPJ1B9HPJSQCABKAB9QRKJS9PK9BKALKSQPAQC1RAKBQPJ1B9HPJ9SQCABKAB9QRKJQS9PK9Bÿ:r,1,C1LQ9P1KBACJ9SAQHJQRAPJQB9K1JRK1SAC1LQ9P1KBACJ9SAQ1HJQRAPJQBK1JR9K1SAC1LQ9P1KRBACJ9SAQHJQRAPJQBK1JRK1SAÿ:r,1,PKLAKJBQS1PKLAK1H9SJAR1CJSC1H9PQC1QJPKLAKJBQS1PKLAK1H9KSJAR1CQJSC1H9PQC1QJPKLAKJBQS1PKLAJK1H9SJAR1CJSC1H9PQC1QJÿ:r,1,19QL1SAKBC1PQL1SK9HQKAB9JP9SRCA9HQJPRAPJ1QL1SAKBC1QL1SK9HQKB9JP9RCA19HQJPRAPJ1QL1SAKBC1QL1SK9HQKB9JKP9RCA9HQSJPRAPJÿ:r,1,K1LQRAJSABKCJSPA1HKSC91PAQJBP1HKCQR9ASK1LQRAJSABKCJSP1RHKC91PAQJBP1HKCQR9PASK1LQRAJSABKCJSP1HRKC91PAQJBP1HKCQR9ASÿ:r,1,KALKSQP9AC1RAKB9HPJAKQCJSQB9HPJS1CR1KALKSQP9AC1RAKB9HPJAKQCJSQB9HPJS1KCR1KALKSQP9AC1RAQKB9HPJAKQCJSQB9KHPJS1CR1ÿ:j,L,2,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,H,5,-500,0ÿ:w,H,4,-20,0ÿ:w,H,3,-5,0ÿ:w,H,2,-2,0ÿ:w,L,5,9000,0ÿ:w,L,4,2500,0ÿ:w,L,3,250,0ÿ:w,L,2,10,0ÿ:w,B,5,750,0ÿ:w,B,4,125,0ÿ:w,B,3,25,0ÿ:w,B,2,2,0ÿ:w,R,5,750,0ÿ:w,R,4,125,0ÿ:w,R,3,25,0ÿ:w,R,2,2,0ÿ:w,S,5,400,0ÿ:w,S,4,100,0ÿ:w,S,3,20,0ÿ:w,P,5,250,0ÿ:w,P,4,75,0ÿ:w,P,3,15,0ÿ:w,C,5,250,0ÿ:w,C,4,75,0ÿ:w,C,3,15,0ÿ:w,A,5,125,0ÿ:w,A,4,50,0ÿ:w,A,3,10,0ÿ:w,K,5,125,0ÿ:w,K,4,50,0ÿ:w,K,3,10,0ÿ:w,Q,5,100,0ÿ:w,Q,4,25,0ÿ:w,Q,3,5,0ÿ:w,J,5,100,0ÿ:w,J,4,25,0ÿ:w,J,3,5,0ÿ:w,1,5,100,0ÿ:w,1,4,25,0ÿ:w,1,3,5,0ÿ:w,9,5,100,0ÿ:w,9,4,25,0ÿ:w,9,3,5,0ÿ:w,9,2,2,0ÿ:wa,H,5,-500,0,0,0,0:1,0,0ÿ:wa,H,4,-20,0,0,0,0:1,0,0ÿ:wa,H,3,-5,0,0,0,0:1,0,0ÿ:wa,H,2,-2,0,0,0,0:1,0,0ÿ:wa,H,5,-1500,0,0,0,0:1,1,0ÿ:wa,H,4,-60,0,0,0,0:1,1,0ÿ:wa,H,3,-15,0,0,0,0:1,1,0ÿ:wa,H,2,-6,0,0,0,0:1,1,0ÿ:wa,L,5,9000,0,0,0,0:1,0,0ÿ:wa,L,4,2500,0,0,0,0:1,0,0ÿ:wa,L,3,250,0,0,0,0:1,0,0ÿ:wa,L,2,10,0,0,0,0:1,0,0ÿ:wa,L,5,27000,0,0,0,0:1,1,0ÿ:wa,L,4,7500,0,0,0,0:1,1,0ÿ:wa,L,3,750,0,0,0,0:1,1,0ÿ:wa,L,2,30,0,0,0,0:1,1,0ÿ:wa,B,5,750,0,0,0,0:1,0,0ÿ:wa,B,4,125,0,0,0,0:1,0,0ÿ:wa,B,3,25,0,0,0,0:1,0,0ÿ:wa,B,2,2,0,0,0,0:1,0,0ÿ:wa,B,5,2250,0,0,0,0:1,1,0ÿ:wa,B,4,375,0,0,0,0:1,1,0ÿ:wa,B,3,75,0,0,0,0:1,1,0ÿ:wa,B,2,6,0,0,0,0:1,1,0ÿ:wa,R,5,750,0,0,0,0:1,0,0ÿ:wa,R,4,125,0,0,0,0:1,0,0ÿ:wa,R,3,25,0,0,0,0:1,0,0ÿ:wa,R,2,2,0,0,0,0:1,0,0ÿ:wa,R,5,2250,0,0,0,0:1,1,0ÿ:wa,R,4,375,0,0,0,0:1,1,0ÿ:wa,R,3,75,0,0,0,0:1,1,0ÿ:wa,R,2,6,0,0,0,0:1,1,0ÿ:wa,S,5,400,0,0,0,0:1,0,0ÿ:wa,S,4,100,0,0,0,0:1,0,0ÿ:wa,S,3,20,0,0,0,0:1,0,0ÿ:wa,S,5,1200,0,0,0,0:1,1,0ÿ:wa,S,4,300,0,0,0,0:1,1,0ÿ:wa,S,3,60,0,0,0,0:1,1,0ÿ:wa,P,5,250,0,0,0,0:1,0,0ÿ:wa,P,4,75,0,0,0,0:1,0,0ÿ:wa,P,3,15,0,0,0,0:1,0,0ÿ:wa,P,5,750,0,0,0,0:1,1,0ÿ:wa,P,4,225,0,0,0,0:1,1,0ÿ:wa,P,3,45,0,0,0,0:1,1,0ÿ:wa,C,5,250,0,0,0,0:1,0,0ÿ:wa,C,4,75,0,0,0,0:1,0,0ÿ:wa,C,3,15,0,0,0,0:1,0,0ÿ:wa,C,5,750,0,0,0,0:1,1,0ÿ:wa,C,4,225,0,0,0,0:1,1,0ÿ:wa,C,3,45,0,0,0,0:1,1,0ÿ:wa,A,5,125,0,0,0,0:1,0,0ÿ:wa,A,4,50,0,0,0,0:1,0,0ÿ:wa,A,3,10,0,0,0,0:1,0,0ÿ:wa,A,5,375,0,0,0,0:1,1,0ÿ:wa,A,4,150,0,0,0,0:1,1,0ÿ:wa,A,3,30,0,0,0,0:1,1,0ÿ:wa,K,5,125,0,0,0,0:1,0,0ÿ:wa,K,4,50,0,0,0,0:1,0,0ÿ:wa,K,3,10,0,0,0,0:1,0,0ÿ:wa,K,5,375,0,0,0,0:1,1,0ÿ:wa,K,4,150,0,0,0,0:1,1,0ÿ:wa,K,3,30,0,0,0,0:1,1,0ÿ:wa,Q,5,100,0,0,0,0:1,0,0ÿ:wa,Q,4,25,0,0,0,0:1,0,0ÿ:wa,Q,3,5,0,0,0,0:1,0,0ÿ:wa,Q,5,300,0,0,0,0:1,1,0ÿ:wa,Q,4,75,0,0,0,0:1,1,0ÿ:wa,Q,3,15,0,0,0,0:1,1,0ÿ:wa,J,5,100,0,0,0,0:1,0,0ÿ:wa,J,4,25,0,0,0,0:1,0,0ÿ:wa,J,3,5,0,0,0,0:1,0,0ÿ:wa,J,5,300,0,0,0,0:1,1,0ÿ:wa,J,4,75,0,0,0,0:1,1,0ÿ:wa,J,3,15,0,0,0,0:1,1,0ÿ:wa,1,5,100,0,0,0,0:1,0,0ÿ:wa,1,4,25,0,0,0,0:1,0,0ÿ:wa,1,3,5,0,0,0,0:1,0,0ÿ:wa,1,5,300,0,0,0,0:1,1,0ÿ:wa,1,4,75,0,0,0,0:1,1,0ÿ:wa,1,3,15,0,0,0,0:1,1,0ÿ:wa,9,5,100,0,0,0,0:1,0,0ÿ:wa,9,4,25,0,0,0,0:1,0,0ÿ:wa,9,3,5,0,0,0,0:1,0,0ÿ:wa,9,2,2,0,0,0,0:1,0,0ÿ:wa,9,5,300,0,0,0,0:1,1,0ÿ:wa,9,4,75,0,0,0,0:1,1,0ÿ:wa,9,3,15,0,0,0,0:1,1,0ÿ:wa,9,2,6,0,0,0,0:1,1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,9,-1ÿb,0,0,0ÿs,1ÿr,0,5,8,43,84,25,73ÿrw,0";
            }
        }
        #endregion

        public GryphonsGoldDeluxeGameLogic()
        {
            _gameID = GAMEID.GryphonsGoldDeluxe;
            GameName = "GryphonsGoldDeluxe";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,3000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:b,17,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,2.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,8,43,84,25,73ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
