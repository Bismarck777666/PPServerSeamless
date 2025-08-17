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
    internal class LordOfTheOceanMagicGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ919ÿ1.10.54.0ÿNGI_GS2_Slot_Serverÿ11412ÿ1.0.0ÿLord of the Ocean Magic 95%ÿ1.102.0_2024-02-26_095937ÿ1.88.1";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49696,100.0,ÿM,10,10000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Lord of the Ocean Magic 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,4K1Q4K2AJ1QK3QAJTQK3QAJTAKSQA34JTAK23QAJTAK43QKJ2AK3Q4K1Q4K2AJ1QK3QAJTQK3QA2JTAKSQ4A3JQTAK3QAJTAK3QKJ2AK3Q4K1Q4K2AJ1QK3QAJTQK3QAJTAKSQA3JT4AK3QAJTAK3QKJ2AK3Qÿ:r,0,TQSJKTQJT4JKAQJT4JKAJT4KJAKT4KJAKT3J1KT2QJT2KTQSAJKTQJT4JKAQJT4JKAJT4KJAKT4KJAKT3QJ1KT2QJT2KTQSJKTQJT4JKAQJT4JKAJT4KJAKT4KJQAKT3J1KT2QJT2Kÿ:r,0,JA3TK2ATJQ34TKAT3JQ4KAT2QJSQT4JA2QT4JA2Q1TJ2A1QTJA3TKA2TJQ3TK2AT3JQ4KAT2QJSQT4JA2QT4JA2Q1TJ2A1QTJA3TKAT2JQ3TKAT3JQ4KAT2QJSQ3T4JA2QT4JA2Q1TJ2A1QTÿ:r,0,J3ASTKQA2T31KQ4A2JT1KQA2J1T3QSAKQJT3QAKQ3JT4AKQ3JT4KJASTKQA2T3KQ4A2JT1KQA2J1T3Q4SAKQJT3QAKQ32JT4AKQ3JT4KJASTKQA2T3KQ4AQ2JT1K3QA2J1T3QSAKQJT3QAKQ3JT4AKQ3JT4Kÿ:r,0,1K4SJQ1K3AT4JQ3ATJQ43AKJ2ST4QAKJ2TQAK32TQAK3ATQ1KSJQ1K3AT4JQ3ATJQ413AKJ2S4QAKJ2TQAK32TQAK34ATQ1KSJQ1K3AT4JQ3AT2JQ43AKJT2S4QAKJ2TQAK32TQA4K3ATQÿ:r,1,AKSQ3AKJAK23QTKJA24QTKJ3A24QTKJA2JQ1KJAKSQ3AKJA23QTKJA24QTKJA24QTKJ3A2JQ1KJAKSQ3A4KJA23QTKJA24QTKJA24QTKJA2JQ1KJÿ:r,1,QTSJA2QT1Q4AKJ3TJQAK43TJQAK4ATJQAK4ATJQTSJ2QJT1QAKJ3TJQAK43TJQAK4ATJQAK4ATJQTSJ2QT1QAKJ3TJQAKQ43TJQAK4ATJQAK4ATJÿ:r,1,JASQTJ1KJAQT31KJAQT31KJAQT32KJAQT42KJASQTJ1KJAQT31KJAQT31KJAQT32KJAQT412KJASQTJ1KJAQT31KJAQT231KJAQT32KJAQT42Kÿ:r,1,3QSJA34TKQJA34TKQJA14TKQJKQ14TKQTKQ2T3QS1JA34TKQJA34TKQJA14TKQJK14TKQ3TKQ2T3QSJA34TKQJA34TKQJA14TKQJK14TKQTKQ2Tÿ:r,1,Q1AST4QAKJ2T3QAKJ2T31KJ2T3S14J2T314QAT314QAK3J24QAST41QAKJ2T3QAKJ2T31KJ2T3S14J2T314QAT314QAKJ24QAST4QAKJ2T3QAKJ2T31KJ2T3S14J2T3K14QAT314QAKJ24ÿ:r,2,J4KSAJ1QT2K4AJQ32K4AJSQ32KTAJQAJKT2KQAJK3SAJ1QT2K4AJQ32K4AJSQ32KTAJQAJKT2KQAJKSAJ1QT2K4AJQ32K4AJSQ32KTAJQAJ4KT2KQAÿ:r,2,KASQTKAQ4JQTJA34JQTJSA3K1QTJATKAQ4J2TKASQTKAQ4JQTJA34JQTJSA3K1QTJATKAQ4J2TKASQTKAQ4JQTJA34JQTJSA3K1QTJATKAQ4J2Tÿ:r,2,1TQSAJQK23T1AJTK2Q4AJTSKJQ1A3TKJQ1A23TKJ1QSAJQK23TAJTK2Q4AJTSKJQ1A3TKJQ1A3TKJ1QSAJQK23TAJTK2Q4AJTSKJQ1A3TKJQ1A3TKJÿ:r,2,KTSJ1K4AQTJ1K4AQTJ2QKS43QTJQK43QTAQT43KTSJ1K4AQTJ1K4AQTJ2KS43QTJQK43QTAQT43KTSJ1K4AQTJ1K4AQTJ2KS43QTJQK43QTAQT43ÿ:r,2,12STJQ2AK4TJQ3SAK4TJQ3A12TKSQ3A12TK43J12STJQ2AK4TJQ3SAK4TJQ3A12TKSQ3A12TK43J12STJQ2AK4TJQ3S2AK4TJQ3A12TKSJQ3A12TK43Jÿ:j,S,1,0,1234AKQJTÿ:u,APPPP.PP.PP.PPPPÿ:w,S,3,-2,0ÿ:w,S,4,-20,0ÿ:w,S,5,-200,0ÿ:w,1,2,10,0ÿ:w,1,3,100,0ÿ:w,1,4,1000,0ÿ:w,1,5,5000,0ÿ:w,2,2,5,0ÿ:w,2,3,40,0ÿ:w,2,4,400,0ÿ:w,2,5,2000,0ÿ:w,3,2,5,0ÿ:w,3,3,30,0ÿ:w,3,4,100,0ÿ:w,3,5,750,0ÿ:w,4,2,5,0ÿ:w,4,3,30,0ÿ:w,4,4,100,0ÿ:w,4,5,750,0ÿ:w,A,3,5,0ÿ:w,A,4,40,0ÿ:w,A,5,150,0ÿ:w,K,3,5,0ÿ:w,K,4,40,0ÿ:w,K,5,150,0ÿ:w,Q,3,5,0ÿ:w,Q,4,25,0ÿ:w,Q,5,100,0ÿ:w,J,3,5,0ÿ:w,J,4,25,0ÿ:w,J,5,100,0ÿ:w,T,3,5,0ÿ:w,T,4,25,0ÿ:w,T,5,100,0ÿ:wa,S,3,-2,0,0,0,0:1,0,0ÿ:wa,S,4,-20,0,0,0,0:1,0,0ÿ:wa,S,5,-200,0,0,0,0:1,0,0ÿ:wa,S,3,-2,0,0,0,0:1,1,0ÿ:wa,S,4,-20,0,0,0,0:1,1,0ÿ:wa,S,5,-200,0,0,0,0:1,1,0ÿ:wa,S,3,-2,0,0,0,0:1,2,0ÿ:wa,S,4,-20,0,0,0,0:1,2,0ÿ:wa,S,5,-200,0,0,0,0:1,2,0ÿ:wa,O,2,-10,0,0,0,0:1,1:2,0ÿ:wa,O,3,-100,0,0,0,0:1,1:2,0ÿ:wa,O,4,-1000,0,0,0,0:1,1:2,0ÿ:wa,O,5,-5000,0,0,0,0:1,1:2,0ÿ:wa,P,2,-5,0,0,0,0:1,1:2,0ÿ:wa,P,3,-40,0,0,0,0:1,1:2,0ÿ:wa,P,4,-400,0,0,0,0:1,1:2,0ÿ:wa,P,5,-2000,0,0,0,0:1,1:2,0ÿ:wa,R,2,-5,0,0,0,0:1,1:2,0ÿ:wa,R,3,-30,0,0,0,0:1,1:2,0ÿ:wa,R,4,-100,0,0,0,0:1,1:2,0ÿ:wa,R,5,-750,0,0,0,0:1,1:2,0ÿ:wa,U,2,-5,0,0,0,0:1,1:2,0ÿ:wa,U,3,-30,0,0,0,0:1,1:2,0ÿ:wa,U,4,-100,0,0,0,0:1,1:2,0ÿ:wa,U,5,-750,0,0,0,0:1,1:2,0ÿ:wa,V,3,-5,0,0,0,0:1,1:2,0ÿ:wa,V,4,-40,0,0,0,0:1,1:2,0ÿ:wa,V,5,-150,0,0,0,0:1,1:2,0ÿ:wa,W,3,-5,0,0,0,0:1,1:2,0ÿ:wa,W,4,-40,0,0,0,0:1,1:2,0ÿ:wa,W,5,-150,0,0,0,0:1,1:2,0ÿ:wa,X,3,-5,0,0,0,0:1,1:2,0ÿ:wa,X,4,-25,0,0,0,0:1,1:2,0ÿ:wa,X,5,-100,0,0,0,0:1,1:2,0ÿ:wa,Y,3,-5,0,0,0,0:1,1:2,0ÿ:wa,Y,4,-25,0,0,0,0:1,1:2,0ÿ:wa,Y,5,-100,0,0,0,0:1,1:2,0ÿ:wa,Z,3,-5,0,0,0,0:1,1:2,0ÿ:wa,Z,4,-25,0,0,0,0:1,1:2,0ÿ:wa,Z,5,-100,0,0,0,0:1,1:2,0ÿ:wa,1,2,10,0,0,0,0:1,-1,0ÿ:wa,1,3,100,0,0,0,0:1,-1,0ÿ:wa,1,4,1000,0,0,0,0:1,-1,0ÿ:wa,1,5,5000,0,0,0,0:1,-1,0ÿ:wa,2,2,5,0,0,0,0:1,-1,0ÿ:wa,2,3,40,0,0,0,0:1,-1,0ÿ:wa,2,4,400,0,0,0,0:1,-1,0ÿ:wa,2,5,2000,0,0,0,0:1,-1,0ÿ:wa,3,2,5,0,0,0,0:1,-1,0ÿ:wa,3,3,30,0,0,0,0:1,-1,0ÿ:wa,3,4,100,0,0,0,0:1,-1,0ÿ:wa,3,5,750,0,0,0,0:1,-1,0ÿ:wa,4,2,5,0,0,0,0:1,-1,0ÿ:wa,4,3,30,0,0,0,0:1,-1,0ÿ:wa,4,4,100,0,0,0,0:1,-1,0ÿ:wa,4,5,750,0,0,0,0:1,-1,0ÿ:wa,A,3,5,0,0,0,0:1,-1,0ÿ:wa,A,4,40,0,0,0,0:1,-1,0ÿ:wa,A,5,150,0,0,0,0:1,-1,0ÿ:wa,K,3,5,0,0,0,0:1,-1,0ÿ:wa,K,4,40,0,0,0,0:1,-1,0ÿ:wa,K,5,150,0,0,0,0:1,-1,0ÿ:wa,Q,3,5,0,0,0,0:1,-1,0ÿ:wa,Q,4,25,0,0,0,0:1,-1,0ÿ:wa,Q,5,100,0,0,0,0:1,-1,0ÿ:wa,J,3,5,0,0,0,0:1,-1,0ÿ:wa,J,4,25,0,0,0,0:1,-1,0ÿ:wa,J,5,100,0,0,0,0:1,-1,0ÿ:wa,T,3,5,0,0,0,0:1,-1,0ÿ:wa,T,4,25,0,0,0,0:1,-1,0ÿ:wa,T,5,100,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:b,14,2,3,4,5,8,10,15,20,30,40,50,80,100,150ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,2.0ÿbs,0,1,0ÿ:k,nullÿe,4,0,3,3ÿb,1000,1000,0ÿ:x,10ÿs,1ÿr,1,5,31,23,32,9,36ÿx,3,1,1,4ÿrw,0";
            }
        }
        #endregion

        public LordOfTheOceanMagicGameLogic()
        {
            _gameID = GAMEID.LordOfTheOceanMagic;
            GameName = "LordOfTheOceanMagic";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,2,1500,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:b,14,2,3,4,5,8,10,15,20,30,40,50,80,100,150ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,2.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,5,31,23,32,9,36ÿx,3,1,1,4ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
