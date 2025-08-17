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
    internal class RumpelWildspinsGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
            get
            {
                return "S";
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ354ÿ1.0.0ÿRumpel Wildspins 95%ÿ1.113.0_2024-11-19_131508ÿ1.107.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,2000,100.0,ÿM,10,10000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Rumpel Wildspins 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:r,0,WJGQGGGQ1OQ1RAJSK1RQ1OK1SJQN1AQO1JRQKN1SQRK1NQ1RA1WJGRQGGGQ1OQ1RAJSK1RQ1OK1SJQN1QO1JRQKN1SQRK1NQ1RA1WJGQ1GGGQ1OQ1GRAJSK1RQ1OK1SJQN1QO1JRQKN1SQRK1NQ1RA1ÿ:r,0,GAGGGJWAJN1QOKQA1JRQKGOQJR1QNKJRQSAQJ1OAQRJKQ1AQJGAGGGJKWAJN1QOKQA1JRQKOQJR1QNKJQS1AQJOAQRJKQGGG1AQJGAGGGJWAJN1QOKQA1JRQKOQJR1QNKJQSAQJOAQRJKQ1AQJÿ:r,0,WJGGG1GJK1AN1JKS1OJQ1JN1QJARKQAJO1KARJK1OJQKNJR1AKWJAGGG1GJK1AN1JKRS1JQ1JN1QJARKQAJO1KARJK1OJQKNJ1AKWJGGG1GJK1AN1JKS1JQ1JN1QJARKQGGGAJO1KARJK1OJQKNGGGJ1AKÿ:r,0,GQGGGGAQG1WAQSJ1NQK1RJ1AW1QAOKQARQJNA1OQJNAK1RQJA1GQGGGGQG1WA1QSJ1NQK1RJ1AW1GGGGQAOKQARQJNA1OQJNAKRQJA1GQGGGGQG1WAQSJ1NQK1RJ1NAW1QAOKQARQJNA1WOQJNGAKRQJA1ÿ:r,0,WJRGGGGQG1JR1JN1KRJQNA1WQJGGGGR1JOAQSA1NKQRJ1KOJQR1QN1AKWJGGGGQGA1JR1JN1KRGJQNA1WQJR1JOAQSA1NKQRJKOJQR1QNAKWJGGGGQG1JGR1JN1AKRJQNA1WQJR1JOAQSA1NKQRAJKOJQR1QNAKÿ:r,1,1KR1KG1KG1AK1NK1OA1KO1QGRJQN1KJ1KGR1KG1KG1AK1NK1OA1KO1QRJQGN1KJ1KR1KG1KG1AGK1NK1OA1KO1QRJQN1KJÿ:r,1,JAGJAGJANJRAJNKJOK1ARJ1AJQOJAQJAGKJAGJANJRAJNKJOK1ARGJ1AJQOJAQJAGJAGJANJRGAJNKJOK1ARJG1AJQOJAQÿ:r,1,QNAOQK1QJNQKAQN1QOKQRA1JRQJGQR1QANOQK1QJNQKAQN1QOKQRA1JRQJGQR1QAOQK1QJNQKAQN1JQOKQRA1JRQJGQR1ÿ:r,1,NQ1NGANQAN1QNA1GQ1OAQJRA1KQJ1ANQ1NGANQAN1QNA1GQ1OAQJRA1KQJ1ANQ1NGANQAN1QNA1GQ1ONAQJRA1KQJ1Aÿ:r,1,OKJO1ROKANJKORJGQJNRK1ARJQGKJROKJO1ROKANJKORJGQJNRK1ARJQGKJROKJO1ROKANJKORJGQJNRK1ARJQGKJRÿ:r,2,1KR1KG1KG1AK1NK1OA1KO1QGRJQN1KJ1KGR1KG1KG1AK1NK1OA1KO1QRJQGN1KJ1KR1KG1KG1AGK1NK1OA1KO1QRJQN1KJÿ:r,2,JAGJAGJANJRAJNKJOK1ARJ1AJQOJAQJAGKJAGJANJRAJNKJOK1ARGJ1AJQOJAQJAGJAGJANJRGAJNKJOK1ARJG1AJQOJAQÿ:r,2,QNAOQK1QJNQKAQN1QOKQRA1JRQJGQR1QANOQK1QJNQKAQN1QOKQRA1JRQJGQR1QAOQK1QJNQKAQN1JQOKQRA1JRQJGQR1ÿ:r,2,NQ1NGANQAN1QNA1GQ1OAQJRA1KQJ1ANQ1NGANQAN1QNA1GQ1OAQJRA1KQJ1ANQ1NGANQAN1QNA1GQ1ONAQJRA1KQJ1Aÿ:r,2,OKJO1ROKANJKORJGQJNRK1ARJQGKJROKJO1ROKANJKORJGQJNRK1ARJQGKJROKJO1ROKANJKORJGQJNRK1ARJQGKJRÿ:j,W,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,S,5,-500,0ÿ:w,S,4,-20,0ÿ:w,S,3,-2,0ÿ:w,W,3,100,0ÿ:w,W,4,200,0ÿ:w,W,5,1000,0ÿ:w,G,3,100,0ÿ:w,G,4,200,0ÿ:w,G,5,500,0ÿ:w,O,3,100,0ÿ:w,O,4,200,0ÿ:w,O,5,500,0ÿ:w,N,3,40,0ÿ:w,N,4,100,0ÿ:w,N,5,300,0ÿ:w,R,3,40,0ÿ:w,R,4,100,0ÿ:w,R,5,300,0ÿ:w,A,3,20,0ÿ:w,A,4,40,0ÿ:w,A,5,200,0ÿ:w,K,3,20,0ÿ:w,K,4,40,0ÿ:w,K,5,200,0ÿ:w,Q,3,10,0ÿ:w,Q,4,20,0ÿ:w,Q,5,100,0ÿ:w,J,3,10,0ÿ:w,J,4,20,0ÿ:w,J,5,100,0ÿ:w,1,3,10,0ÿ:w,1,4,20,0ÿ:w,1,5,100,0ÿ:wa,S,5,-500,0,0,0,0:1,0,0ÿ:wa,S,4,-20,0,0,0,0:1,0,0ÿ:wa,S,3,-2,0,0,0,0:1,0,0ÿ:wa,W,3,100,0,0,0,0:1,-1,0ÿ:wa,W,4,200,0,0,0,0:1,-1,0ÿ:wa,W,5,1000,0,0,0,0:1,-1,0ÿ:wa,G,3,100,0,0,0,0:1,-1,0ÿ:wa,G,4,200,0,0,0,0:1,-1,0ÿ:wa,G,5,500,0,0,0,0:1,-1,0ÿ:wa,O,3,100,0,0,0,0:1,-1,0ÿ:wa,O,4,200,0,0,0,0:1,-1,0ÿ:wa,O,5,500,0,0,0,0:1,-1,0ÿ:wa,N,3,40,0,0,0,0:1,-1,0ÿ:wa,N,4,100,0,0,0,0:1,-1,0ÿ:wa,N,5,300,0,0,0,0:1,-1,0ÿ:wa,R,3,40,0,0,0,0:1,-1,0ÿ:wa,R,4,100,0,0,0,0:1,-1,0ÿ:wa,R,5,300,0,0,0,0:1,-1,0ÿ:wa,A,3,20,0,0,0,0:1,-1,0ÿ:wa,A,4,40,0,0,0,0:1,-1,0ÿ:wa,A,5,200,0,0,0,0:1,-1,0ÿ:wa,K,3,20,0,0,0,0:1,-1,0ÿ:wa,K,4,40,0,0,0,0:1,-1,0ÿ:wa,K,5,200,0,0,0,0:1,-1,0ÿ:wa,Q,3,10,0,0,0,0:1,-1,0ÿ:wa,Q,4,20,0,0,0,0:1,-1,0ÿ:wa,Q,5,100,0,0,0,0:1,-1,0ÿ:wa,J,3,10,0,0,0,0:1,-1,0ÿ:wa,J,4,20,0,0,0,0:1,-1,0ÿ:wa,J,5,100,0,0,0,0:1,-1,0ÿ:wa,1,3,10,0,0,0,0:1,-1,0ÿ:wa,1,4,20,0,0,0,0:1,-1,0ÿ:wa,1,5,100,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,11ÿ:i,12ÿ:i,13ÿ:i,14ÿ:i,15ÿ:i,16ÿ:i,17ÿ:i,18ÿ:i,19ÿ:i,20ÿ:m,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20ÿ:b,16,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,20,0,19,19ÿb,1000,10000,0ÿ:x,2000ÿs,1ÿr,0,5,38,13,0,39,96ÿl,Q,3,4,0,0,0,0,0,QQW--,0ÿrw,0ÿ:wee,0";
            }
        }
        #endregion

        public RumpelWildspinsGameLogic()
        {
            _gameID = GAMEID.RumpelWildspins;
            GameName = "RumpelWildspins";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,5000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,11ÿ:i,12ÿ:i,13ÿ:i,14ÿ:i,15ÿ:i,16ÿ:i,17ÿ:i,18ÿ:i,19ÿ:i,20ÿ:m,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20ÿ:b,16,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,38,13,0,39,96ÿl,Q,3,4,0,0,0,0,0,QQW--,0ÿrw,0ÿ:wee,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
                resMessage.Append("\aTICKETSÿÿMINOFFERÿ50ÿINDEPENDENTLINESANDBETSÿ1ÿCCODEÿchÿMAXLIMITÿ50000ÿREALCURRENCYFACTORÿ106.53030787258975178438265686ÿREALMONEYCURRENCYCODEISOÿCHFÿTOURNISSUBSCRIBEDÿ0ÿNATIVEJACKPOTCURRENCYÿCHFÿISSERVERAUTOREBUYÿ0ÿMINBUYINÿ2ÿPAYOUTRATEÿ95.01%ÿALLOWCATEGORYÿ0,1,3,4,5ÿNATIVEJACKPOTCURRENCYFACTORÿ93.87000693699351264382058437ÿUSERIDÿ621884763ÿLANGUAGEIDÿ2ÿJACKPOTFEEPERCENTAGEÿ0.0000ÿMAXBUYINÿ2147483647ÿLIMITÿ50000ÿREALMONEYÿ0ÿERRNUMBERÿ42ÿOFFERÿ50ÿPLAYERBALANCEÿ50000ÿDISALLOWRECONNECTAFTERINACTIVITYÿ0ÿAUTOKICKÿ0ÿCLIENT_TO_WRAPPERÿ1ÿISDEEPWALLETÿ1ÿCURRENCYÿÿSITEIDÿ2337ÿRELIABILITYÿ14ÿWRAPPER_TO_CLIENTÿ1ÿNEEDSSESSIONTIMEOUTÿ0ÿLANGUAGEISOCODEÿENÿEXTUSERIDÿ621884763ÿTICKETTYPEÿ1ÿCURRENCYFACTORÿ100.000000ÿSESSIONTIMEOUTMINUTESÿ20ÿNICKNAMEÿ#1165325621884763ÿREALCURRENCYÿCHFÿMINLIMITÿ50");
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
                resMessage.Append("\aTICKETSÿÿMINOFFERÿ50ÿINDEPENDENTLINESANDBETSÿ1ÿCCODEÿchÿMAXLIMITÿ50000ÿREALCURRENCYFACTORÿ106.53030787258975178438265686ÿREALMONEYCURRENCYCODEISOÿCHFÿTOURNISSUBSCRIBEDÿ0ÿNATIVEJACKPOTCURRENCYÿCHFÿISSERVERAUTOREBUYÿ0ÿMINBUYINÿ2ÿPAYOUTRATEÿ95.01%ÿALLOWCATEGORYÿ0,1,3,4,5ÿNATIVEJACKPOTCURRENCYFACTORÿ93.87000693699351264382058437ÿUSERIDÿ621884763ÿLANGUAGEIDÿ2ÿJACKPOTFEEPERCENTAGEÿ0.0000ÿMAXBUYINÿ2147483647ÿLIMITÿ50000ÿREALMONEYÿ0ÿERRNUMBERÿ42ÿOFFERÿ50ÿPLAYERBALANCEÿ50000ÿDISALLOWRECONNECTAFTERINACTIVITYÿ0ÿAUTOKICKÿ0ÿCLIENT_TO_WRAPPERÿ1ÿISDEEPWALLETÿ1ÿCURRENCYÿÿSITEIDÿ2337ÿRELIABILITYÿ14ÿWRAPPER_TO_CLIENTÿ1ÿNEEDSSESSIONTIMEOUTÿ0ÿLANGUAGEISOCODEÿENÿEXTUSERIDÿ621884763ÿTICKETTYPEÿ1ÿCURRENCYFACTORÿ100.000000ÿSESSIONTIMEOUTMINUTESÿ20ÿNICKNAMEÿ#1165325621884763ÿREALCURRENCYÿCHFÿMINLIMITÿ50");
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
