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
    internal class AfricanSimbaGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
            get
            {
                return "I";
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ225ÿ1.0.0ÿAfrican Simba 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49757,100.0,ÿM,10,10000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,African Simba 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:l,^_-_^ÿ:l,^^_^^ÿ:l,__^__ÿ:l,-^_^-ÿ:l,-_^_-ÿ:r,0,IJ9KMJ1GJMQFK9JIM9JB9KJM9JBAIJ9MJ9IJ9KMJ1GJMQFK9JIM9JB9KJM9JBAIJ9MJ9IJ9KMJ1GJMQFK9JIM9JB9KJM9JBAIJ9MJ9ÿ:r,0,GA1BA9QF1ABQFLQAB1QAF1QM1QFK1QAB1JF1QFAQGA1BA9QF1ABQFLQAB1QAF1QM1QFK1QAB1JF1QFAQGA1BA9QF1ABQFLQAB1QAF1QM1QFK1QAB1JF1QFAQÿ:r,0,1LGF1JM1KGQMKAIKGQKM9KGQKI1KG9KBGK9MK1GQK1LF1JM1KGQMKAIKGQKM9KGQKI1KG9KBGK9MK1GQK1LF1JM1KGQMKAIKGQKM9KGQKI1KG9KBGK9MK1GQKÿ:r,0,AFMAB9FAK1JKF1QGJK1FAJ1LA9KFAJBAJFAJ1MJAFMAB9FAK1AJKFQGJK1FAJ1LA9KFAJBAJFAJ1MJAFMAB9FAK1JKFQGJK1FAJ1LA9KFAJBAJFAJ1MJÿ:r,0,9QIBA1F9QABQAGQKIQJB9IJABQ9FQ9MJ9QB9QBA1F9QAJBQAGQKIQJB9IJABQ9FQ9MJ9QB9QBA1F9QAB9QAGQKIAQJB9IJABQ9BFQ9MJ9QBÿ:r,1,IJ9KMJ1GJMQFKJ9IM1KIF9KB9FJABQ9IKAMIJ9KMFJ1GJAMQFKJ9IJM1KF9KB9FJABQ9IKAMIJ9KMJ1GJMQ9FKJ9IM1AKF9KJB9FJABQ9IKAMÿ:r,1,GAK1BA9QF1ABQFLQAB1FKQFJ19FQKL1MJKF1QGA1BKA9QF1ABQFLQAB1FKQFJ19FQKL1MJKF1QGA1BA9QF1ABQFLQAB1FKQFJ19FQKL1MJKF1Qÿ:r,1,1LF1JM1QG1MKA1FKQJAIBJKQMJ1GKF1KMQGKI9GJ1LF1JM1QG1MKA1FKQJAIBJKQMJ1GKF1KMQGKI9GJ1LF1JM1QG1MKA1FKQJAI1BJKQMJ1GKF1KMQGKI9GJÿ:r,1,AFMAB9FAK1MKGQMJAMK1BQ9J1LA9FM1KB9JMA9BJAFMAB9FAK1MKGQMJAM1BQJ1LA9M1KB9JMA9BJAFMAB9FAK1MKGQMJAM1BQJ1LA9M1KBA9JMA9BJÿ:r,1,9QBA1F9MQABK91AMQKJIQJM9JKI9FJG1MKB1M9QIBA1F9QABK1AMQKJI1QJM9KI9FJG1MJKB1M9QBJA1F9QAB9K1AMQKJIQJM9KI9FJG1MKB1Mÿ:r,2,KIJG9MJBQAI9QJF1MK9GKIJ9BKM9IK9JBQAJKIJG9MJBQAI9QF1MK9GKIJ9BKM9IK9JBAJKIJGQ9MJBQAIJ9QF1MK9GKIJ9BKM9IK9JBAJÿ:r,2,LAFQA1MKAF1AGQAF1ABQ9F1QB19M1QLBAQF9JQF1LAFQA1MKAF1AGLQAF1AB9F1QB19M1QLBAQFJAQF1LAFQA1MKAFL1AGQAF1AB9F1QB19M1QLBAQFJQF1ÿ:r,2,KB1JMK1JF9GQJIQF1JGK9LAMKJ1IAFKJ9MAKI1KB1JMK1JF9G1QJIQF1JGK9LAMJ1IAFKJ9MAKI1KB1JMK1JF9GQJIQF1JGK9LAQMJ1IAFKJ9MAKI1ÿ:r,2,MA9FA19ABK9JGAJFA9BQM1LJKFQ9BAMJBQJAF19MA9FA19AJBK9JGAJFA9BQ1LJKFQ9BAMJBQJAF19MA9FAJ19ABK9JGAJFA9BQ1LJKFQ9BAMJBQJAFB19ÿ:r,2,Q1MQK1MJKIQ9FKQM9GJIKQM9QFJBQI1FAB9AMQ1MQK1MAJKI91FKQM9GJIKQM9QFJBQI1FMAB9AMQ1MQK1MJKI9FKQM9GJIKQM9QFMJBQI1FAB9AMÿ:r,3,KIJG9MJBQAI9QF1MK9GKIJ9BKF9IK9JBAJKIJG9MJBQAI9QF1MK9GKIJ9BQKF9IK9JBAJKIJG9MJKBQAJI9QF1MK9GKIJ9BKF9IK9JBAJÿ:r,3,LAFQAL1MKAF1AGQAF1AB9F1QBJ9M1QLBAQFJQF1LAFQAL1MKAF1A9GQAF1AB9F1QLBJ9M1QLBAQFJQF1LAFQA9L1MKAF1AGQAF1AB9F1QBJ9M1QLBAQFJQF1ÿ:r,3,KB1GJM1LJF9GQJIQF1JBK9LAMJ1IQAFKJM9KI1KB1GJM1LJF9GQJIQF1JQBK9LAMJ1IAFKJM9KI1KB1GJM1LJF9GQJIQF1JBK91LAMJ1IAFKJM9KI1ÿ:r,3,MA9FAG19ABKFJGKJFK9BQ1LJKFQ9BAM1BQJAF19MA9KFAG9ABKFJGKJFK9BQ1LJKFQ9BAM1BQJAF19MA9FAG9AJBKFJGKJFK9BQ1LJKFQ9BA9M1BQJAF19ÿ:r,3,Q1GQK1MJKI9FKQM9GJIKQM91FJBQI1FAQB9AMQ1GQKB1MJKI9FKBQM9FGJIKQAM91FJBQI1FAB9AMQ1GQK1MJKI9FKQM9GAJIKQM91FJBQI1FAB9AMÿ:j,L,1,0,GBMFAKQJ19ÿ:u,APPPP.PP.PP.PPPPÿ:w,G,5,2500,0ÿ:w,G,4,500,0ÿ:w,G,3,100,0ÿ:w,B,5,750,0ÿ:w,B,4,150,0ÿ:w,B,3,50,0ÿ:w,M,5,250,0ÿ:w,M,4,75,0ÿ:w,M,3,25,0ÿ:w,F,5,250,0ÿ:w,F,4,75,0ÿ:w,F,3,25,0ÿ:w,A,5,125,0ÿ:w,A,4,25,0ÿ:w,A,3,10,0ÿ:w,K,5,125,0ÿ:w,K,4,25,0ÿ:w,K,3,10,0ÿ:w,Q,5,125,0ÿ:w,Q,4,25,0ÿ:w,Q,3,10,0ÿ:w,J,5,100,0ÿ:w,J,4,20,0ÿ:w,J,3,5,0ÿ:w,1,5,100,0ÿ:w,1,4,20,0ÿ:w,1,3,5,0ÿ:w,9,5,100,0ÿ:w,9,4,20,0ÿ:w,9,3,5,0ÿ:wa,G,5,2500,0,0,0,0:1,0:1,0ÿ:wa,G,4,500,0,0,0,0:1,0:1,0ÿ:wa,G,3,100,0,0,0,0:1,0:1,0ÿ:wa,B,5,750,0,0,0,0:1,0:1,0ÿ:wa,B,4,150,0,0,0,0:1,0:1,0ÿ:wa,B,3,50,0,0,0,0:1,0:1,0ÿ:wa,M,5,250,0,0,0,0:1,0:1,0ÿ:wa,M,4,75,0,0,0,0:1,0:1,0ÿ:wa,M,3,25,0,0,0,0:1,0:1,0ÿ:wa,F,5,250,0,0,0,0:1,0:1,0ÿ:wa,F,4,75,0,0,0,0:1,0:1,0ÿ:wa,F,3,25,0,0,0,0:1,0:1,0ÿ:wa,A,5,125,0,0,0,0:1,0:1,0ÿ:wa,A,4,25,0,0,0,0:1,0:1,0ÿ:wa,A,3,10,0,0,0,0:1,0:1,0ÿ:wa,K,5,125,0,0,0,0:1,0:1,0ÿ:wa,K,4,25,0,0,0,0:1,0:1,0ÿ:wa,K,3,10,0,0,0,0:1,0:1,0ÿ:wa,Q,5,125,0,0,0,0:1,0:1,0ÿ:wa,Q,4,25,0,0,0,0:1,0:1,0ÿ:wa,Q,3,10,0,0,0,0:1,0:1,0ÿ:wa,J,5,100,0,0,0,0:1,0:1,0ÿ:wa,J,4,20,0,0,0,0:1,0:1,0ÿ:wa,J,3,5,0,0,0,0:1,0:1,0ÿ:wa,1,5,100,0,0,0,0:1,0:1,0ÿ:wa,1,4,20,0,0,0,0:1,0:1,0ÿ:wa,1,3,5,0,0,0,0:1,0:1,0ÿ:wa,9,5,100,0,0,0,0:1,0:1,0ÿ:wa,9,4,20,0,0,0,0:1,0:1,0ÿ:wa,9,3,5,0,0,0,0:1,0:1,0ÿ:wa,G,5,7500,0,0,0,0:1,2:3,0ÿ:wa,G,4,1500,0,0,0,0:1,2:3,0ÿ:wa,G,3,300,0,0,0,0:1,2:3,0ÿ:wa,B,5,2250,0,0,0,0:1,2:3,0ÿ:wa,B,4,450,0,0,0,0:1,2:3,0ÿ:wa,B,3,150,0,0,0,0:1,2:3,0ÿ:wa,M,5,750,0,0,0,0:1,2:3,0ÿ:wa,M,4,225,0,0,0,0:1,2:3,0ÿ:wa,M,3,75,0,0,0,0:1,2:3,0ÿ:wa,F,5,750,0,0,0,0:1,2:3,0ÿ:wa,F,4,225,0,0,0,0:1,2:3,0ÿ:wa,F,3,75,0,0,0,0:1,2:3,0ÿ:wa,A,5,375,0,0,0,0:1,2:3,0ÿ:wa,A,4,75,0,0,0,0:1,2:3,0ÿ:wa,A,3,30,0,0,0,0:1,2:3,0ÿ:wa,K,5,375,0,0,0,0:1,2:3,0ÿ:wa,K,4,75,0,0,0,0:1,2:3,0ÿ:wa,K,3,30,0,0,0,0:1,2:3,0ÿ:wa,Q,5,375,0,0,0,0:1,2:3,0ÿ:wa,Q,4,75,0,0,0,0:1,2:3,0ÿ:wa,Q,3,30,0,0,0,0:1,2:3,0ÿ:wa,J,5,300,0,0,0,0:1,2:3,0ÿ:wa,J,4,60,0,0,0,0:1,2:3,0ÿ:wa,J,3,15,0,0,0,0:1,2:3,0ÿ:wa,1,5,300,0,0,0,0:1,2:3,0ÿ:wa,1,4,60,0,0,0,0:1,2:3,0ÿ:wa,1,3,15,0,0,0,0:1,2:3,0ÿ:wa,9,5,300,0,0,0,0:1,2:3,0ÿ:wa,9,4,60,0,0,0,0:1,2:3,0ÿ:wa,9,3,15,0,0,0,0:1,2:3,0ÿ:s,0ÿ:i,1ÿ:i,3ÿ:i,7ÿ:i,15ÿ:i,25ÿ:m,1,3,7,15,25ÿ:b,14,1,2,3,4,5,8,10,15,20,30,40,50,80,100ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,3,0,1,1ÿb,1000,1000,0ÿ:x,20ÿs,1ÿr,2,5,6,6,61,27,20ÿrw,0";
            }
        }
        #endregion

        public AfricanSimbaGameLogic()
        {
            _gameID = GAMEID.AfricanSimba;
            GameName = "AfricanSimba";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,3000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,3ÿ:i,7ÿ:i,15ÿ:i,25ÿ:m,1,3,7,15,25ÿ:b,14,1,2,3,4,5,8,10,15,20,30,40,50,80,100ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,2,5,6,6,61,27,20ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
