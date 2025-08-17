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
    internal class DolphinsPearlClassicGameLogic : BaseGreenSlotGame
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
                return 0;
            }
        }

        protected override string VersionCheckString
        {
            get
            {
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ171ÿ1.0.0ÿDolphins Pearl (classic) 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Dolphins Pearl (classic) 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:r,0,I1DQJ9K1AL1IKQSAQPJQRJF1KJ1FJSAI1DQ9K1AL1IKQSAQPJQRJFA1KJ1FJSAI1DFQ9K1AL1IKQSAQPJQRJF1KJ1FJSAÿ:r,0,FKDAK1FLQJ9I1ARJQLAQFK1P9SJFKIRJ9SKJIKJF9SQAFKDIAK1LQJ9I1ARJQLAQFK1P9SJFKIRJ9SKJIKJF9SQAFKDAK1LQJ9I1ARJQLAQFK1P9SJFKIRJ9SKAJIKJF9SQAÿ:r,0,1QD1SAL9IS1JR9F1JLQI9PQ1RAFQIS9KLJFAR9SQIAKFAI1QD1SAL9ISJR9QF1JLQ9PJQ1RAFQ9ISKLJFAR9SQIAKFAI1QD1SAL9ISJR9F1JLQ9PQ1RAFQISKLJFAR9SQIAKFAIÿ:r,0,K1DQRAJSQLAIJF9JI9KF1PKIJLAI9FK1DQRI9SJF9JRK1DQRAJSQLARIJF9JI9KF1PKIJLAI9FK1DJQRI9SJF9JRK1DQRAJSQLAIJF9JI9KF1PKIQJLAI9FK1DQRI9SJF9JRÿ:r,0,KADKSQFAQI1RAKLQFJ1L9PFJKSQIALKAL9QRKJS9FK9LKAQDKSQFAQI1LRAKLQFJ1L9PFJSQIALKAL9QRKJS9FK9LKADKSQFAQI1RAKLQFJ1L9PLFJSQIALKAL9QRKJS9FK9Lÿ:r,1,I1DQ9F1KLAIJ9SAQPJQRAFJQLK1JRK1SAI1DQ9F1KLAIJ9SAQPJQR1AFJQLK1JRK1SAI1DQ9F1KLAIJ9SAQPJQRAFJQLK1JRK1SAÿ:r,1,FKDAKJLQS1FKDAK1P9SJAR1IJSI1P9KFQI1QJFKDAKJLQS1FKDAK1P9SJAR1IJSI1P9FQI1QJFKDAKJLQS1FKDAK1P9SJAR1IJSI1P9FQI1QJÿ:r,1,1QD1SAK9LI1QD1SK9PQKL9JKF9RIA9PQJFRAFJ1QD1SAKLIR1QD1FSK9PQKL9JF9RIA9PQJF1RAFJ1QD1SAKLI1QD1SK9PQKL9JF9RIDA9PQJFRAFJÿ:r,1,K1DQRAJSALKIJSF1PKI91FAQJLF1PKIQR9ASK1DQRAJSALKIAJSF1PKI91FAQJLF1PKIQR9ASK1DQRAJSALKIJSRF1PAKI91FAQJLF1PKIQR9ASÿ:r,1,KADKSQF9AI1RAKL9PFJAKQIJSQL9PFJS1IR1KADKPSQF9AI1RAKL9PFJAKQIJSQL9PFJS1IR1KADKSQF9AI1RAKL9PFJAKQIJSQLA9PFJS1IR1ÿ:j,D,2,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,P,5,-500,0ÿ:w,P,4,-20,0ÿ:w,P,3,-5,0ÿ:w,P,2,-2,0ÿ:w,D,5,9000,0ÿ:w,D,4,2500,0ÿ:w,D,3,250,0ÿ:w,D,2,10,0ÿ:w,L,5,750,0ÿ:w,L,4,125,0ÿ:w,L,3,25,0ÿ:w,L,2,2,0ÿ:w,R,5,750,0ÿ:w,R,4,125,0ÿ:w,R,3,25,0ÿ:w,R,2,2,0ÿ:w,S,5,400,0ÿ:w,S,4,100,0ÿ:w,S,3,20,0ÿ:w,F,5,250,0ÿ:w,F,4,75,0ÿ:w,F,3,15,0ÿ:w,I,5,250,0ÿ:w,I,4,75,0ÿ:w,I,3,15,0ÿ:w,A,5,125,0ÿ:w,A,4,50,0ÿ:w,A,3,10,0ÿ:w,K,5,125,0ÿ:w,K,4,50,0ÿ:w,K,3,10,0ÿ:w,Q,5,100,0ÿ:w,Q,4,25,0ÿ:w,Q,3,5,0ÿ:w,J,5,100,0ÿ:w,J,4,25,0ÿ:w,J,3,5,0ÿ:w,1,5,100,0ÿ:w,1,4,25,0ÿ:w,1,3,5,0ÿ:w,9,5,100,0ÿ:w,9,4,25,0ÿ:w,9,3,5,0ÿ:w,9,2,2,0ÿ:wa,P,5,-500,0,0,0,0:1,0,0ÿ:wa,P,4,-20,0,0,0,0:1,0,0ÿ:wa,P,3,-5,0,0,0,0:1,0,0ÿ:wa,P,2,-2,0,0,0,0:1,0,0ÿ:wa,P,5,-1500,0,0,0,0:1,1,0ÿ:wa,P,4,-60,0,0,0,0:1,1,0ÿ:wa,P,3,-15,0,0,0,0:1,1,0ÿ:wa,P,2,-6,0,0,0,0:1,1,0ÿ:wa,D,5,9000,0,0,0,0:1,0,0ÿ:wa,D,4,2500,0,0,0,0:1,0,0ÿ:wa,D,3,250,0,0,0,0:1,0,0ÿ:wa,D,2,10,0,0,0,0:1,0,0ÿ:wa,D,5,27000,0,0,0,0:1,1,0ÿ:wa,D,4,7500,0,0,0,0:1,1,0ÿ:wa,D,3,750,0,0,0,0:1,1,0ÿ:wa,D,2,30,0,0,0,0:1,1,0ÿ:wa,L,5,750,0,0,0,0:1,0,0ÿ:wa,L,4,125,0,0,0,0:1,0,0ÿ:wa,L,3,25,0,0,0,0:1,0,0ÿ:wa,L,2,2,0,0,0,0:1,0,0ÿ:wa,L,5,2250,0,0,0,0:1,1,0ÿ:wa,L,4,375,0,0,0,0:1,1,0ÿ:wa,L,3,75,0,0,0,0:1,1,0ÿ:wa,L,2,6,0,0,0,0:1,1,0ÿ:wa,R,5,750,0,0,0,0:1,0,0ÿ:wa,R,4,125,0,0,0,0:1,0,0ÿ:wa,R,3,25,0,0,0,0:1,0,0ÿ:wa,R,2,2,0,0,0,0:1,0,0ÿ:wa,R,5,2250,0,0,0,0:1,1,0ÿ:wa,R,4,375,0,0,0,0:1,1,0ÿ:wa,R,3,75,0,0,0,0:1,1,0ÿ:wa,R,2,6,0,0,0,0:1,1,0ÿ:wa,S,5,400,0,0,0,0:1,0,0ÿ:wa,S,4,100,0,0,0,0:1,0,0ÿ:wa,S,3,20,0,0,0,0:1,0,0ÿ:wa,S,5,1200,0,0,0,0:1,1,0ÿ:wa,S,4,300,0,0,0,0:1,1,0ÿ:wa,S,3,60,0,0,0,0:1,1,0ÿ:wa,F,5,250,0,0,0,0:1,0,0ÿ:wa,F,4,75,0,0,0,0:1,0,0ÿ:wa,F,3,15,0,0,0,0:1,0,0ÿ:wa,F,5,750,0,0,0,0:1,1,0ÿ:wa,F,4,225,0,0,0,0:1,1,0ÿ:wa,F,3,45,0,0,0,0:1,1,0ÿ:wa,I,5,250,0,0,0,0:1,0,0ÿ:wa,I,4,75,0,0,0,0:1,0,0ÿ:wa,I,3,15,0,0,0,0:1,0,0ÿ:wa,I,5,750,0,0,0,0:1,1,0ÿ:wa,I,4,225,0,0,0,0:1,1,0ÿ:wa,I,3,45,0,0,0,0:1,1,0ÿ:wa,A,5,125,0,0,0,0:1,0,0ÿ:wa,A,4,50,0,0,0,0:1,0,0ÿ:wa,A,3,10,0,0,0,0:1,0,0ÿ:wa,A,5,375,0,0,0,0:1,1,0ÿ:wa,A,4,150,0,0,0,0:1,1,0ÿ:wa,A,3,30,0,0,0,0:1,1,0ÿ:wa,K,5,125,0,0,0,0:1,0,0ÿ:wa,K,4,50,0,0,0,0:1,0,0ÿ:wa,K,3,10,0,0,0,0:1,0,0ÿ:wa,K,5,375,0,0,0,0:1,1,0ÿ:wa,K,4,150,0,0,0,0:1,1,0ÿ:wa,K,3,30,0,0,0,0:1,1,0ÿ:wa,Q,5,100,0,0,0,0:1,0,0ÿ:wa,Q,4,25,0,0,0,0:1,0,0ÿ:wa,Q,3,5,0,0,0,0:1,0,0ÿ:wa,Q,5,300,0,0,0,0:1,1,0ÿ:wa,Q,4,75,0,0,0,0:1,1,0ÿ:wa,Q,3,15,0,0,0,0:1,1,0ÿ:wa,J,5,100,0,0,0,0:1,0,0ÿ:wa,J,4,25,0,0,0,0:1,0,0ÿ:wa,J,3,5,0,0,0,0:1,0,0ÿ:wa,J,5,300,0,0,0,0:1,1,0ÿ:wa,J,4,75,0,0,0,0:1,1,0ÿ:wa,J,3,15,0,0,0,0:1,1,0ÿ:wa,1,5,100,0,0,0,0:1,0,0ÿ:wa,1,4,25,0,0,0,0:1,0,0ÿ:wa,1,3,5,0,0,0,0:1,0,0ÿ:wa,1,5,300,0,0,0,0:1,1,0ÿ:wa,1,4,75,0,0,0,0:1,1,0ÿ:wa,1,3,15,0,0,0,0:1,1,0ÿ:wa,9,5,100,0,0,0,0:1,0,0ÿ:wa,9,4,25,0,0,0,0:1,0,0ÿ:wa,9,3,5,0,0,0,0:1,0,0ÿ:wa,9,2,2,0,0,0,0:1,0,0ÿ:wa,9,5,300,0,0,0,0:1,1,0ÿ:wa,9,4,75,0,0,0,0:1,1,0ÿ:wa,9,3,15,0,0,0,0:1,1,0ÿ:wa,9,2,6,0,0,0,0:1,1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:m,1,2,3,4,5,6,7,8,9ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,9,0,8,-1ÿb,0,0,0ÿs,1ÿr,0,5,1,27,45,23,28ÿrw,0";
            }
        }
        #endregion

        public DolphinsPearlClassicGameLogic()
        {
            _gameID = GAMEID.DolphinsPearlClassic;
            GameName = "DolphinsPearlClassic";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,2700,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:m,1,2,3,4,5,6,7,8,9ÿ:b,17,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,2.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,1,27,45,23,28ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
