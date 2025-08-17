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
    internal class ColumbusDeluxeGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
            get
            {
                return "X";
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ82ÿ1.0.0ÿColumbus 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Columbus 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,CSAQ1JPXKSXKNSJ1XQJ1QJ1NJ1QXA1QPA1CAKQ1JPXKSXKNSJ1XQJ1QJ1NJ1QA1QPA1CAQ1JPXKSXKNSJ1XQJP1QJ1NJK1QA1QPA1ÿ:r,0,CQKNQSKJ1NJAKP1KJAPJAJK1JSAJKCQKNQSK1NJAKP1KJAPJAJK1JSAJKCQKNQSK1NJAKP1KJAPJAJK1JSAJKÿ:r,0,CKJSQA1KQ1AKJQAN1KJQK1QAJSXQ1PX1CKJSQA1KQ1AKJQAN1KJQK1QAJSXQ1KPX1CKJSQA1KQ1AKJQAN1KJQK1QKAJSXQ1PKX1ÿ:r,0,CKSJCQ1SKCJ1NAPKN1AQCJ1PAQSAJCKSJCQ1SKJ1NAPKN1AQCJ1PAQSAJCKSJCQ1SKJ1NAPKN1AQCJ1PAQSAJÿ:r,0,XKQJ1KXAKSCJQCJ1XNQPKJ1SQ1KNQAN1SQAXKJ1KXAQKSCJQCJ1NQCPKJ1SQ1NQAKN1SQAXKJ1KXAKSCJQCJ1NQPKJ1SQ1NQAN1SQAÿ:r,1,CAQ1JSXKSXJ1XQJ1QJ1NJ1QA1QPJA1CAQ1JSXKSXJ1XQJ1QJ1NJ1QA1QPA1CAQ1JSXKSXJ1XQJ1QJ1NJ1QA1SQPA1ÿ:r,1,CQKNQSKJ1NJAKP1KJAPJAJK1JSAJKCQKNQSK1NJAKP1KJAPJAJK1JSAJKCQKNQSK1NJAKP1KJAPJAJK1JSAJKÿ:r,1,CQKJSQA1KQ1AKQAN1KQK1QASXQ1PX1CKJSQA1KQ1AKQAN1KQK1QASXQ1PX1CKJSQA1KQ1AKQAN1KQK1QASXQ1PX1ÿ:r,1,CK1AJCQ1S1KJ1NAPKNJAQKJ1S1QSAJCK1JCQ1SKJ1NAPKNJAQKJ1S1QSAJCK1JCNQ1SKJ1NASPKNJAQKJ1S1QSAJÿ:r,1,XKJKXAKSCJQCJ1XNQPKNQ1NJACNJSQAXKJCKXAKSCJQACJ1NQCPKNQ1NJANJSQAXKJKXAKSCJQCJA1NQPXKNQ1NJANJSQAÿ:r,2,AC1JPYKAYJNSJAYQJA1JANKAJAC1JPYKAYJANSJAYQJA1JANKAJAC1JPYKAYJNSJAYQJA1JANKAJÿ:r,2,Q1NQSK1QJ1QJ1QJA1PJAS1KQ1CQ1NAQSK1QJ1QJ1QJAPJAS1KQ1CQ1NQSK1QAJ1QJ1QJAPJAS1KQ1Cÿ:r,2,KJSQA1KP1AKJYKN1KJQAYKJCKJSQA1KP1AKJYKN1KJQAYKJCKJSQA1KP1AKJYKN1KJQAYKJCÿ:r,2,KN1CKQN1SPJ1NJQNS1ASKNAJN1CK1CKQ1NSPJ1NJQNS1ASKNAJN1CK1CKQ1SPJ1NJQNS1ASKNAJN1Cÿ:r,2,QS1KY1KS1JQCJ1NQP1NJYSAPQS1KY1KS1JQCJ1NQP1NJYSAPQS1KY1PKS1JQCJ1NQP1NJYSAPÿ:j,C,1,0,ÿ:j,Y,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,C,5,5000,0ÿ:w,C,4,1000,0ÿ:w,C,3,100,0ÿ:w,C,2,10,0ÿ:w,P,5,1000,0ÿ:w,P,4,200,0ÿ:w,P,3,50,0ÿ:w,P,2,5,0ÿ:w,N,5,500,0ÿ:w,N,4,100,0ÿ:w,N,3,25,0ÿ:w,N,2,5,0ÿ:w,S,5,250,0ÿ:w,S,4,75,0ÿ:w,S,3,15,0ÿ:w,S,2,5,0ÿ:w,A,5,150,0ÿ:w,A,4,40,0ÿ:w,A,3,10,0ÿ:w,K,5,150,0ÿ:w,K,4,40,0ÿ:w,K,3,10,0ÿ:w,Q,5,150,0ÿ:w,Q,4,40,0ÿ:w,Q,3,10,0ÿ:w,J,5,100,0ÿ:w,J,4,20,0ÿ:w,J,3,5,0ÿ:w,1,5,100,0ÿ:w,1,4,20,0ÿ:w,1,3,5,0ÿ:wa,C,5,5000,0,0,0,0:1,-1,0ÿ:wa,C,4,1000,0,0,0,0:1,-1,0ÿ:wa,C,3,100,0,0,0,0:1,-1,0ÿ:wa,C,2,10,0,0,0,0:1,-1,0ÿ:wa,P,5,1000,0,0,0,0:1,-1,0ÿ:wa,P,4,200,0,0,0,0:1,-1,0ÿ:wa,P,3,50,0,0,0,0:1,-1,0ÿ:wa,P,2,5,0,0,0,0:1,-1,0ÿ:wa,N,5,500,0,0,0,0:1,-1,0ÿ:wa,N,4,100,0,0,0,0:1,-1,0ÿ:wa,N,3,25,0,0,0,0:1,-1,0ÿ:wa,N,2,5,0,0,0,0:1,-1,0ÿ:wa,S,5,250,0,0,0,0:1,-1,0ÿ:wa,S,4,75,0,0,0,0:1,-1,0ÿ:wa,S,3,15,0,0,0,0:1,-1,0ÿ:wa,S,2,5,0,0,0,0:1,-1,0ÿ:wa,A,5,150,0,0,0,0:1,-1,0ÿ:wa,A,4,40,0,0,0,0:1,-1,0ÿ:wa,A,3,10,0,0,0,0:1,-1,0ÿ:wa,K,5,150,0,0,0,0:1,-1,0ÿ:wa,K,4,40,0,0,0,0:1,-1,0ÿ:wa,K,3,10,0,0,0,0:1,-1,0ÿ:wa,Q,5,150,0,0,0,0:1,-1,0ÿ:wa,Q,4,40,0,0,0,0:1,-1,0ÿ:wa,Q,3,10,0,0,0,0:1,-1,0ÿ:wa,J,5,100,0,0,0,0:1,-1,0ÿ:wa,J,4,20,0,0,0,0:1,-1,0ÿ:wa,J,3,5,0,0,0,0:1,-1,0ÿ:wa,1,5,100,0,0,0,0:1,-1,0ÿ:wa,1,4,20,0,0,0,0:1,-1,0ÿ:wa,1,3,5,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,9,-1ÿb,0,0,0ÿs,1ÿr,0,5,6,8,12,27,28ÿrw,0";
            }
        }
        #endregion

        public ColumbusDeluxeGameLogic()
        {
            _gameID = GAMEID.ColumbusDeluxe;
            GameName = "ColumbusDeluxe";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,3000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:b,17,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,2.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,6,8,12,27,28ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
