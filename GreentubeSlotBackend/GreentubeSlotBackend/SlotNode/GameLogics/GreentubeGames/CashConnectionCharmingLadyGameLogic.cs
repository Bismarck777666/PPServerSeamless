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
    internal class CashConnectionCharmingLadyGameLogic : BaseGreenSlotGame
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
                return 6;
            }
        }

        protected override string VersionCheckString
        {
            get
            {
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ11289ÿ1.10.54.0ÿGTATCashConnectionServerÿ11278ÿ1.1.0ÿCash Connection - Charming Lady linked 95%ÿ1.118.0_2025-03-28_090804ÿ1.51.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49220,100.0,ÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Cash Connection - Charming Lady linked 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,C1LQK1AB1CKQSAQHHHJQR9P1KBJ1PJS9C1Q9K1A9CKQSAQHHJQRAP9RKJB1PJÿ:r,0,9KLAK1BQJKC1PJKBJQPKJHHH9SJACRJ9SKJCKRAJBK1HH9SJPKC1RJ9SKJCKQP1ÿ:r,0,AQL1SKQBKCJS19BJKQ9HHHQ1PJKR1QSAPQ1BAC91S9HHQ1PJKR19LAPQHHAC9QRJAP9ÿ:r,0,K1LQRAPLJSQBACJL9BJC9LKB1HHHKCJ9AL9PLKQ1CQLKR1PLKCJLAC9PHHQRC9SJPQÿ:r,0,QALKSQP9C1R9PARKQPJ19HHKABJK1RKJS9PK9HHAS1B9HHPJS9CAJK9CJK9SJC9HHASKJB19JP9Jÿ:r,1,C1LQK1AB1CKQSAQHHHJQR9P1KBJ1PJS9C1Q9K1A9CKQSAQHHJQRAP9RKJB1PJÿ:r,1,9KLAK1BQJKC1PJKBJQPKJHHH9SJACRJ9SKJCKRAJBK1HH9SJPKC1RJ9SKJCKQP1ÿ:r,1,AQL1SKQBKCJS19BJKQ9HHHQ1PJKR1QSAPQ1BAC91S9HHQ1PJKR19LAPQHHAC9QRJAP9ÿ:r,1,K1LQRAPLJSQBACJL9BJC9LKB1HHHKCJ9AL9PLKQ1CQLKR1PLKCJLAC9PHHQRC9SJPQÿ:r,1,QALKSQP9C1R9PARKQPJ19HHKABJK1RKJS9PK9HHAS1B9HHPJS9CAJK9CJK9SJC9HHASKJB19JP9Jÿ:r,2,C1LQK1AB1CKQSAQHHHJQR9P1KBJ1PJS9C1Q9K1A9CKQSAQHHJQRAP9RKJB1PJÿ:r,2,9KLAK1BQJKC1PJKBJQPKJHHH9SJACRJ9SKJCKRAJBK1HH9SJPKC1RJ9SKJCKQP1ÿ:r,2,AQL1SKQBKCJS19BJKQ9HHHQ1PJKR1QSAPQ1BAC91S9HHQ1PJKR19LAPQHHAC9QRJAP9ÿ:r,2,K1LQRAPLJSQBACJL9BJC9LKB1HHHKCJ9AL9PLKQ1CQLKR1PLKCJLAC9PHHQRC9SJPQÿ:r,2,QALKSQP9C1R9PARKQPJ19HHKABJK1RKJS9PK9HHAS1B9HHPJS9CAJK9CJK9SJC9HHASKJB19JP9Jÿ:r,3,C1LQK1AB1CKQSAQHHHJQR9P1KBJ1PJS9C1Q9K1A9CKQSAQHHJQRAP9RKJB1PJÿ:r,3,9KLAK1BQJKC1PJKBJQPKJHHH9SJACRJ9SKJCKRAJBK1HH9SJPKC1RJ9SKJCKQP1ÿ:r,3,AQL1SKQBKCJS19BJKQ9HHHQ1PJKR1QSAPQ1BAC91S9HHQ1PJKR19LAPQHHAC9QRJAP9ÿ:r,3,K1LQRAPLJSQBACJL9BJC9LKB1HHHKCJ9AL9PLKQ1CQLKR1PLKCJLAC9PHHQRC9SJPQÿ:r,3,QALKSQP9C1R9PARKQPJ19HHKABJK1RKJS9PK9HHAS1B9HHPJS9CAJK9CJK9SJC9HHASKJB19JP9Jÿ:r,4,H1HQP9HJ1ARJHQKJPAQ9HKC1RAKSQJH1CQJ9RACKJPQS1J9BKQ9ÿ:r,4,HQHAC9HQPJRQHAJP1JBKHAKC9PQA1KHJS9CAB1CK9SA1KQRJP9Aÿ:r,4,H1HKPAHQR1BKHQKJP9SQHK1AB1CKJQHKC9JPKSQR1C9SJP9KRQPÿ:r,4,H1HQBKHJRAS9HK9QC1RJHQ1PQKBAC9HA1CARKPAC1ARJQBKJPARÿ:r,4,HJHAB1HK9SAJH9QJP9APH9QR1PKAPQHJAQ9K1AB1CKJ9AQ1CJ1Kÿ:r,5,H1BQP9CJ1ARJSQKJPAQ9PKC1RAKSQJP1CQJ9RACKJPQS1J9BKQ9ÿ:r,5,HQRAC9SQPJRQCAJP1JBK9AKC9PQA1KPJS9CAB1CK9SA1KQRJP9Aÿ:r,5,H1SKPACQR1BKCQKJP9SQ9K1AB1CKJQSKC9JPKSQR1C9SJP9KRQPÿ:r,5,H1SQBKCJRAS9PK9QC1RJSQ1PQKBAC9RA1CARKPAC1ARJQBKJPARÿ:r,5,HJSAB1CK9SAJR9QJP9APK9QR1PKAPQSJAQ9K1AB1CKJ9AQ1CJ1Kÿ:r,6,A1BQJP9C1ARJQSKJPAQP9KC1RAKSQPJ1CQJ9RACKJPQS1J9BKQ9ÿ:r,6,JQRAC9SQPJRQACJP1JBK9AKC9PQA1KPJS9CAB1CK9SA1KQRJP9Aÿ:r,6,91SKPACQR1BKQCKJP9SQ9K1AB1CKJSKQCKJP9SQR1C9SJP9KRQPÿ:r,6,91SQBKCJRA9SKP9QC1RJQS1PQKBAC9AR1CARKPAC1ARJQBKJPARÿ:r,6,QJSAB1CK9SA9JQRJP9APK9AQR1PKAPQSAQ9K1AB1CKJ9AQ1CJ1Kÿ:j,L,2,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,H,103,-12500,99.0:Major,12500.0ÿ:w,H,102,-75000,99.0:Grand,75000.0ÿ:w,H,14,-1,0ÿ:w,H,13,-1,0ÿ:w,H,12,-1,0ÿ:w,H,11,-1,0ÿ:w,H,10,-1,0ÿ:w,H,9,-1,0ÿ:w,H,8,-1,0ÿ:w,H,7,-1,0ÿ:w,H,6,-1,0ÿ:w,Z,100,-50,0ÿ:w,Z,101,-10,0ÿ:w,L,2,10,0ÿ:w,L,3,250,0ÿ:w,L,4,2500,0ÿ:w,L,5,9000,0ÿ:w,B,2,2,0ÿ:w,B,3,25,0ÿ:w,B,4,125,0ÿ:w,B,5,750,0ÿ:w,R,2,2,0ÿ:w,R,3,25,0ÿ:w,R,4,125,0ÿ:w,R,5,750,0ÿ:w,S,3,20,0ÿ:w,S,4,100,0ÿ:w,S,5,400,0ÿ:w,P,3,15,0ÿ:w,P,4,75,0ÿ:w,P,5,250,0ÿ:w,C,3,15,0ÿ:w,C,4,75,0ÿ:w,C,5,250,0ÿ:w,A,3,10,0ÿ:w,A,4,50,0ÿ:w,A,5,125,0ÿ:w,K,3,10,0ÿ:w,K,4,50,0ÿ:w,K,5,125,0ÿ:w,Q,3,5,0ÿ:w,Q,4,25,0ÿ:w,Q,5,100,0ÿ:w,J,3,5,0ÿ:w,J,4,25,0ÿ:w,J,5,100,0ÿ:w,1,3,5,0ÿ:w,1,4,25,0ÿ:w,1,5,100,0ÿ:w,9,2,2,0ÿ:w,9,3,5,0ÿ:w,9,4,25,0ÿ:w,9,5,100,0ÿ:wa,H,103,-12500,99.0:Major,12500.0,0,0:1,-1,0ÿ:wa,H,102,-75000,99.0:Grand,75000.0,0,0:1,-1,0ÿ:wa,H,14,-1,0,0,0,0:1,-1,0ÿ:wa,H,13,-1,0,0,0,0:1,-1,0ÿ:wa,H,12,-1,0,0,0,0:1,-1,0ÿ:wa,H,11,-1,0,0,0,0:1,-1,0ÿ:wa,H,10,-1,0,0,0,0:1,-1,0ÿ:wa,H,9,-1,0,0,0,0:1,-1,0ÿ:wa,H,8,-1,0,0,0,0:1,-1,0ÿ:wa,H,7,-1,0,0,0,0:1,-1,0ÿ:wa,H,6,-1,0,0,0,0:1,-1,0ÿ:wa,Z,100,-50,0,0,0,0:1,-1,0ÿ:wa,Z,101,-10,0,0,0,0:1,-1,0ÿ:wa,L,2,10,0,0,0,0:1,-1,0ÿ:wa,L,3,250,0,0,0,0:1,-1,0ÿ:wa,L,4,2500,0,0,0,0:1,-1,0ÿ:wa,L,5,9000,0,0,0,0:1,-1,0ÿ:wa,B,2,2,0,0,0,0:1,-1,0ÿ:wa,B,3,25,0,0,0,0:1,-1,0ÿ:wa,B,4,125,0,0,0,0:1,-1,0ÿ:wa,B,5,750,0,0,0,0:1,-1,0ÿ:wa,R,2,2,0,0,0,0:1,-1,0ÿ:wa,R,3,25,0,0,0,0:1,-1,0ÿ:wa,R,4,125,0,0,0,0:1,-1,0ÿ:wa,R,5,750,0,0,0,0:1,-1,0ÿ:wa,S,3,20,0,0,0,0:1,-1,0ÿ:wa,S,4,100,0,0,0,0:1,-1,0ÿ:wa,S,5,400,0,0,0,0:1,-1,0ÿ:wa,P,3,15,0,0,0,0:1,-1,0ÿ:wa,P,4,75,0,0,0,0:1,-1,0ÿ:wa,P,5,250,0,0,0,0:1,-1,0ÿ:wa,C,3,15,0,0,0,0:1,-1,0ÿ:wa,C,4,75,0,0,0,0:1,-1,0ÿ:wa,C,5,250,0,0,0,0:1,-1,0ÿ:wa,A,3,10,0,0,0,0:1,-1,0ÿ:wa,A,4,50,0,0,0,0:1,-1,0ÿ:wa,A,5,125,0,0,0,0:1,-1,0ÿ:wa,K,3,10,0,0,0,0:1,-1,0ÿ:wa,K,4,50,0,0,0,0:1,-1,0ÿ:wa,K,5,125,0,0,0,0:1,-1,0ÿ:wa,Q,3,5,0,0,0,0:1,-1,0ÿ:wa,Q,4,25,0,0,0,0:1,-1,0ÿ:wa,Q,5,100,0,0,0,0:1,-1,0ÿ:wa,J,3,5,0,0,0,0:1,-1,0ÿ:wa,J,4,25,0,0,0,0:1,-1,0ÿ:wa,J,5,100,0,0,0,0:1,-1,0ÿ:wa,1,3,5,0,0,0,0:1,-1,0ÿ:wa,1,4,25,0,0,0,0:1,-1,0ÿ:wa,1,5,100,0,0,0,0:1,-1,0ÿ:wa,9,2,2,0,0,0,0:1,-1,0ÿ:wa,9,3,5,0,0,0,0:1,-1,0ÿ:wa,9,4,25,0,0,0,0:1,-1,0ÿ:wa,9,5,100,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,10ÿ:m,10ÿ:b,18,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500ÿ:a,0,0ÿ:g,0,0,0,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,10,0,0,0ÿb,1000,1000,0ÿ:x,640ÿs,1ÿr,0,5,14,35,41,51,36ÿx,2,1:2:-10:5:1:3,-10#7ÿc,H,6,0,0,0,0,,010221224142,0ÿrw,0";
            }
        }
        #endregion

        public CashConnectionCharmingLadyGameLogic()
        {
            _gameID = GAMEID.CashConnectionCharmingLady;
            GameName = "CashConnectionCharmingLady";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,5000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,10ÿ:m,10ÿ:b,18,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500ÿ:a,0,0ÿ:g,0,0,0,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,14,35,41,51,36ÿx,2,1:2:-10:5:1:3,-10#7ÿc,H,6,0,0,0,0,,010221224142,0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
                resMessage.Append("$20ÿ5000ÿgrandÿ21.9261057ÿmajorÿ4.0185388");
                resMessage.Append(GameUniqueString);

                resMessage.Append("%t0ÿq0");
                resMessage.Append($"\aNATIVEJACKPOTCURRENCYFACTORÿ100.0ÿMAXBUYINTOTALÿ0ÿNEEDSSESSIONTIMEOUTÿ0ÿRELIABILITYÿ0ÿNICKNAMEÿ155258EURÿIDÿ155258ÿISDEEPWALLETÿ0ÿCURRENCYFACTORÿ100.0ÿMINBUYINÿ1ÿSESSIONTIMEOUTMINUTESÿ60ÿJACKPOT_PROGRESSIVE_CAPÿ0.0ÿNATIVEJACKPOTCURRENCYÿ{getCurrencyCode(currency)}ÿEXTUSERIDÿDummyDB_ExternalUserId_155258EURÿFREESPINSISCAPREACHEDÿ0ÿCURRENCYÿ{getCurrencySymbol(currency)}ÿTICKETSÿ0ÿJACKPOT_TOTAL_CAPÿ0.0ÿJACKPOTFEEPERCENTAGEÿ0.0ÿISSHOWJACKPOTCONTRIBUTIONÿ0");
                resMessage.Append(buildLineBetString(strGlobalUserID, balance));
                resMessage.Append("$20ÿ5000ÿgrandÿ21.9261057ÿmajorÿ4.0185388");

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
                _logger.Error("Exception has been occurred in BaseGreenSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
    }
}
