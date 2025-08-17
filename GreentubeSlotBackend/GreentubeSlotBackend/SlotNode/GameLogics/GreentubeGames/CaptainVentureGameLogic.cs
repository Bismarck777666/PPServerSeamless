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
    internal class CaptainVentureGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
            get
            {
                return "B";
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ307ÿ1.10.54.0ÿSlotStudiosSlotServerÿ276ÿ1.0.0ÿCaptain Gold 94%ÿ1.97.0_2023-10-02_102620ÿ1.55.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Captain Gold 94%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,A1GJUAE1LQ9K1QBJQPJG1EKLQUAJM1KA1QBJQPJG1EKLQÿ:r,0,K19E1BAMQKJQGK19UJGKEJP9UKPJQMAJG9UQAGKLAJEK1BAMQKLAÿ:r,0,QKGAE1QL1UAMKE1UJP9G1PAGQEJUKMJGAP9UQEAMQ9BQKE1QL1Q9Bÿ:r,0,LQPAJE9K1QPKE9UJG9JG1BKEJMAE9GJUQMAEJG9PK1LQ9JG1BKEJÿ:r,0,KGJUQEAM1ALKUQGAKJU9BKG9MK1MKQGJQE1PAM9QPA1M9KG1ALK9BKG9ÿ:r,1,KA1GJUAE1IQ9K1QBJQPJG1JEKIQUAJM1KA1GJUAE1IQ9K1QBJQPJG1EKIQUAJM1KA1GJUAE1IQ9KA1QBJQPJG1EKIQUAJM1ÿ:r,1,PJQMAJG9UQAJGKAIJEK19E1AMQIUKJQGK1B9UJGKAEJP9UKPJQMAKJG9UJQAGKAIJEK19EP1AMQIKJQGK1B9UJGKEJPQ9UKPJQMAJG19UQAGKAIJEK19E1AMQAIKJQGK1B9UJIGKEJP9UKÿ:r,1,P9G1PAGQEJIQUKMJKGAP9UQEAMQB9GAE1GQI1UAMKE1UJP9G1PAGQEJIQUKMJKGAP9UQEAMQB9GUAE1QI1UAMKE1UJP9GQ1PAGQEJIQUKMJKGAP9UQEAMKQB9GAE1QI1UAQMKE1KUJÿ:r,1,EJMAE9GJUQMAEJG9PK1IQPAJE9K1QPKE9UJG9EJG1BKEJMAE9GJUQMAEJG9PK1IQPAJE9K1QPKEM9UJG9JG1BKEJMAE9GJKUQMAEJG9PK1IQPKAJE9K1QPKE9UJG9JG1BKÿ:r,1,1MKQGKJQE1PAM9QPA1M9BKGJUQEAM1AIKUQGAKJU9KG9MK1MKQGJQE1PAM9QPAQ1M9BKGJUQEAM1AIKUQGAKJU9KG9MK1MKQGJQE1PAM9QPA1M9BKGJUQEAM1AIKUQGAKJU9KG9MKÿ:r,2,IQJBKA1GJUAE1IQJ9K1QBJQPJG1EKIQUAJM1ÿ:r,2,PJQMAJG9UQAGKIAJEK9E1BKIA1B9MQIKJQGK1B9UJGKEJP19UKÿ:r,2,UJP9G1PAGQIKQEJUKMJGAP9BQI1UQEAMQB9GAE1B9QI1UAMKE1ÿ:r,2,EJMAE9GJUQMAEJG9P1BKQPAIJE9BK1IQPKE9UJG9JG1BKÿ:r,2,K1MQGJQE1PAM9BKGQPA1M9BKGJUQEAM1AIKUQGABKJU9KG9Mÿ:r,3,A1GJUAE1LQ9K1QBJQPJG1EKQUAJM1KA1QBJQPJG1EKLQÿ:r,3,K19E1BAMQKJQGK19UJGKEJP9UKPJQMAJG9UQAGKLAJEK1BAMQKLAÿ:r,3,QKGAE1QL1UAMKE1UJP9G1PAGQEJUKMJGAP9UQEAMQ9BQKE1QL1Q9Bÿ:r,3,LQPAJE9K1QPKE9UJG9JG1BKEJMAE9GJUQMAEJG9PK1LQ9JG1BKEJÿ:r,3,KGJUQEAM1ALKUQGAKJU9BKG9MK1MKQGJQE1PAM9QPA1M9KG1ALK9BKG9ÿ:j,L,1,0,ÿ:j,I,2,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,B,5,-125,0ÿ:w,B,4,-20,0ÿ:w,B,3,-5,0ÿ:w,B,2,-1,0ÿ:w,L,5,10000,0ÿ:w,L,4,400,0ÿ:w,L,3,50,0ÿ:w,L,2,10,0ÿ:w,M,5,400,0ÿ:w,M,4,80,0ÿ:w,M,3,20,0ÿ:w,M,2,2,0ÿ:w,P,5,400,0ÿ:w,P,4,80,0ÿ:w,P,3,20,0ÿ:w,P,2,2,0ÿ:w,U,5,300,0ÿ:w,U,4,60,0ÿ:w,U,3,15,0ÿ:w,G,5,200,0ÿ:w,G,4,40,0ÿ:w,G,3,15,0ÿ:w,E,5,200,0ÿ:w,E,4,40,0ÿ:w,E,3,15,0ÿ:w,A,5,100,0ÿ:w,A,4,30,0ÿ:w,A,3,10,0ÿ:w,K,5,100,0ÿ:w,K,4,30,0ÿ:w,K,3,10,0ÿ:w,Q,5,80,0ÿ:w,Q,4,15,0ÿ:w,Q,3,5,0ÿ:w,J,5,80,0ÿ:w,J,4,15,0ÿ:w,J,3,5,0ÿ:w,1,5,80,0ÿ:w,1,4,15,0ÿ:w,1,3,5,0ÿ:w,9,5,80,0ÿ:w,9,4,15,0ÿ:w,9,3,5,0ÿ:w,9,2,2,0ÿ:wa,B,5,-125,0,0,0,0:1,0:3,0ÿ:wa,B,4,-20,0,0,0,0:1,0:3,0ÿ:wa,B,3,-5,0,0,0,0:1,0:3,0ÿ:wa,B,2,-1,0,0,0,0:1,0:3,0ÿ:wa,B,5,-500,0,0,0,0:1,1:2,0ÿ:wa,B,4,-80,0,0,0,0:1,1:2,0ÿ:wa,B,3,-20,0,0,0,0:1,1:2,0ÿ:wa,B,2,-4,0,0,0,0:1,1:2,0ÿ:wa,L,5,10000,0,0,0,0:1,0:3,0ÿ:wa,L,4,400,0,0,0,0:1,0:3,0ÿ:wa,L,3,50,0,0,0,0:1,0:3,0ÿ:wa,L,2,10,0,0,0,0:1,0:3,0ÿ:wa,I,5,80000,0,0,0,0:1,1:2,0ÿ:wa,I,4,3200,0,0,0,0:1,1:2,0ÿ:wa,I,3,400,0,0,0,0:1,1:2,0ÿ:wa,I,2,80,0,0,0,0:1,1:2,0ÿ:wa,M,5,400,0,0,0,0:1,0:3,0ÿ:wa,M,4,80,0,0,0,0:1,0:3,0ÿ:wa,M,3,20,0,0,0,0:1,0:3,0ÿ:wa,M,2,2,0,0,0,0:1,0:3,0ÿ:wa,M,5,1600,0,0,0,0:1,1:2,0ÿ:wa,M,4,320,0,0,0,0:1,1:2,0ÿ:wa,M,3,80,0,0,0,0:1,1:2,0ÿ:wa,M,2,8,0,0,0,0:1,1:2,0ÿ:wa,P,5,400,0,0,0,0:1,0:3,0ÿ:wa,P,4,80,0,0,0,0:1,0:3,0ÿ:wa,P,3,20,0,0,0,0:1,0:3,0ÿ:wa,P,2,2,0,0,0,0:1,0:3,0ÿ:wa,P,5,1600,0,0,0,0:1,1:2,0ÿ:wa,P,4,320,0,0,0,0:1,1:2,0ÿ:wa,P,3,80,0,0,0,0:1,1:2,0ÿ:wa,P,2,8,0,0,0,0:1,1:2,0ÿ:wa,U,5,300,0,0,0,0:1,0:3,0ÿ:wa,U,4,60,0,0,0,0:1,0:3,0ÿ:wa,U,3,15,0,0,0,0:1,0:3,0ÿ:wa,U,5,1200,0,0,0,0:1,1:2,0ÿ:wa,U,4,240,0,0,0,0:1,1:2,0ÿ:wa,U,3,60,0,0,0,0:1,1:2,0ÿ:wa,G,5,200,0,0,0,0:1,0:3,0ÿ:wa,G,4,40,0,0,0,0:1,0:3,0ÿ:wa,G,3,15,0,0,0,0:1,0:3,0ÿ:wa,G,5,800,0,0,0,0:1,1:2,0ÿ:wa,G,4,160,0,0,0,0:1,1:2,0ÿ:wa,G,3,60,0,0,0,0:1,1:2,0ÿ:wa,E,5,200,0,0,0,0:1,0:3,0ÿ:wa,E,4,40,0,0,0,0:1,0:3,0ÿ:wa,E,3,15,0,0,0,0:1,0:3,0ÿ:wa,E,5,800,0,0,0,0:1,1:2,0ÿ:wa,E,4,160,0,0,0,0:1,1:2,0ÿ:wa,E,3,60,0,0,0,0:1,1:2,0ÿ:wa,A,5,100,0,0,0,0:1,0:3,0ÿ:wa,A,4,30,0,0,0,0:1,0:3,0ÿ:wa,A,3,10,0,0,0,0:1,0:3,0ÿ:wa,K,5,100,0,0,0,0:1,0:3,0ÿ:wa,K,4,30,0,0,0,0:1,0:3,0ÿ:wa,K,3,10,0,0,0,0:1,0:3,0ÿ:wa,A,5,400,0,0,0,0:1,1:2,0ÿ:wa,A,4,120,0,0,0,0:1,1:2,0ÿ:wa,A,3,40,0,0,0,0:1,1:2,0ÿ:wa,K,5,400,0,0,0,0:1,1:2,0ÿ:wa,K,4,120,0,0,0,0:1,1:2,0ÿ:wa,K,3,40,0,0,0,0:1,1:2,0ÿ:wa,Q,5,80,0,0,0,0:1,0:3,0ÿ:wa,Q,4,15,0,0,0,0:1,0:3,0ÿ:wa,Q,3,5,0,0,0,0:1,0:3,0ÿ:wa,J,5,80,0,0,0,0:1,0:3,0ÿ:wa,J,4,15,0,0,0,0:1,0:3,0ÿ:wa,J,3,5,0,0,0,0:1,0:3,0ÿ:wa,1,5,80,0,0,0,0:1,0:3,0ÿ:wa,1,4,15,0,0,0,0:1,0:3,0ÿ:wa,1,3,5,0,0,0,0:1,0:3,0ÿ:wa,Q,5,320,0,0,0,0:1,1:2,0ÿ:wa,Q,4,60,0,0,0,0:1,1:2,0ÿ:wa,Q,3,20,0,0,0,0:1,1:2,0ÿ:wa,J,5,320,0,0,0,0:1,1:2,0ÿ:wa,J,4,60,0,0,0,0:1,1:2,0ÿ:wa,J,3,20,0,0,0,0:1,1:2,0ÿ:wa,1,5,320,0,0,0,0:1,1:2,0ÿ:wa,1,4,60,0,0,0,0:1,1:2,0ÿ:wa,1,3,20,0,0,0,0:1,1:2,0ÿ:wa,9,5,80,0,0,0,0:1,0:3,0ÿ:wa,9,4,15,0,0,0,0:1,0:3,0ÿ:wa,9,3,5,0,0,0,0:1,0:3,0ÿ:wa,9,2,2,0,0,0,0:1,0:3,0ÿ:wa,9,5,320,0,0,0,0:1,1:2,0ÿ:wa,9,4,60,0,0,0,0:1,1:2,0ÿ:wa,9,3,20,0,0,0,0:1,1:2,0ÿ:wa,9,2,8,0,0,0,0:1,1:2,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,trueÿ:es,0,0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,9,-1ÿb,0,0,0ÿs,1ÿr,0,5,24,21,0,2,34ÿrw,0";
            }
        }
        #endregion

        public CaptainVentureGameLogic()
        {
            _gameID = GAMEID.CaptainVenture;
            GameName = "CaptainVenture";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,2,2000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:b,15,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1000,-1,false,0,trueÿ:es,0,0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,24,21,0,2,34ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
                _logger.Error("Exception has been occurred in BaseGreenSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
    }
}
