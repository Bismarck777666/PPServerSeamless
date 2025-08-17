using Akka.Util;
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
    internal class DiamondLinkCopsNRobbersGameLogic : BaseGreenLockRespinSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ15901ÿ1.10.54.0ÿGTSKDiamondLinkV2SlotServerÿ16462ÿ1.0.0ÿDiamond Link Cops and Robbers Linked V2 95%ÿ1.117.0_2025-03-13_091353ÿ1.45.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Diamond Link Cops and Robbers Linked V2 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,^^-^^ÿ:l,__-__ÿ:l,-___-ÿ:l,-^^^-ÿ:l,^___^ÿ:l,_^^^_ÿ:l,^---^ÿ:l,_---_ÿ:l,-_-_-ÿ:l,-^-^-ÿ:l,^_-_^ÿ:l,_^-^_ÿ:l,--_--ÿ:l,--^--ÿ:l,^-^-^ÿ:r,0,CQSQBKGGGAKBKWQACMQ###KBAGKSMSQ#ABKCSWQAMKCSACQSQBAGGGKQBQWKACMS###QBKGACMCA#QBQMSWKAMKCSAÿ:r,0,BACSMAGGGKASQWAMCBA###ASKGKQBQM#CMKQBWKQMKQBKBAMSCKGGGQASAWQMBCS###CSAGKQBQC#MMACSWQAMKQBKÿ:r,0,AKCQBKGGGACBKWQACBK###KSAGAKSCQ#AMQSMWKAMQSMBAKCQBQGGGKCBQWKACBS###MSAGQKSCA#QMKBSWQAMQSBMÿ:r,0,MQCASAGGGQMBQWKBCMK###ABQGKQSMA#KSACKWACSACKBMQCASQGGGKMBKWQMCBS###QBQGQQSMK#ASKBSWKCSAKCBÿ:r,0,SQCSQAGGGASMKWAQCBA###MSKGQMKQB#AMKCBWQBMKCBASQCSQKGGGKSMAWKQCBS###ASAGKMKQA#BMQBSWKBMKCBAÿ:j,W,1,0,GCSBMAKQÿ:u,APPPP.PP.PP.PPPPÿ:w,#,101,-1000,1:Grandÿ:w,#,102,-200,1:Majorÿ:w,#,100,-1,0ÿ:w,G,2,5,0ÿ:w,G,3,20,0ÿ:w,G,4,100,0ÿ:w,G,5,1000,0ÿ:w,C,3,15,0ÿ:w,C,4,30,0ÿ:w,C,5,100,0ÿ:w,S,3,15,0ÿ:w,S,4,30,0ÿ:w,S,5,100,0ÿ:w,B,3,10,0ÿ:w,B,4,15,0ÿ:w,B,5,75,0ÿ:w,M,3,10,0ÿ:w,M,4,15,0ÿ:w,M,5,75,0ÿ:w,A,3,5,0ÿ:w,A,4,10,0ÿ:w,A,5,30,0ÿ:w,K,3,5,0ÿ:w,K,4,10,0ÿ:w,K,5,30,0ÿ:w,Q,3,5,0ÿ:w,Q,4,10,0ÿ:w,Q,5,30,0ÿ:wa,#,101,-1000,1:Grand,0,0,0:1,-1,0ÿ:wa,#,102,-200,1:Major,0,0,0:1,-1,0ÿ:wa,#,100,-1,0,0,0,0:1,-1,0ÿ:wa,G,2,5,0,0,0,0:1,-1,0ÿ:wa,G,3,20,0,0,0,0:1,-1,0ÿ:wa,G,4,100,0,0,0,0:1,-1,0ÿ:wa,G,5,1000,0,0,0,0:1,-1,0ÿ:wa,C,3,15,0,0,0,0:1,-1,0ÿ:wa,C,4,30,0,0,0,0:1,-1,0ÿ:wa,C,5,100,0,0,0,0:1,-1,0ÿ:wa,S,3,15,0,0,0,0:1,-1,0ÿ:wa,S,4,30,0,0,0,0:1,-1,0ÿ:wa,S,5,100,0,0,0,0:1,-1,0ÿ:wa,B,3,10,0,0,0,0:1,-1,0ÿ:wa,B,4,15,0,0,0,0:1,-1,0ÿ:wa,B,5,75,0,0,0,0:1,-1,0ÿ:wa,M,3,10,0,0,0,0:1,-1,0ÿ:wa,M,4,15,0,0,0,0:1,-1,0ÿ:wa,M,5,75,0,0,0,0:1,-1,0ÿ:wa,A,3,5,0,0,0,0:1,-1,0ÿ:wa,A,4,10,0,0,0,0:1,-1,0ÿ:wa,A,5,30,0,0,0,0:1,-1,0ÿ:wa,K,3,5,0,0,0,0:1,-1,0ÿ:wa,K,4,10,0,0,0,0:1,-1,0ÿ:wa,K,5,30,0,0,0,0:1,-1,0ÿ:wa,Q,3,5,0,0,0,0:1,-1,0ÿ:wa,Q,4,10,0,0,0,0:1,-1,0ÿ:wa,Q,5,30,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,20ÿ:m,20ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,20,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,85,17,81,28,5ÿx,2,dv:0.0;0.0;0.0;0.0;0.0;2.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0,[Z:S:R:85;17;81;28;5;0;*C:0.0;0.0;0.0;0.0;0.0;2.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;*P:dv:0.0;0.0;0.0;0.0;0.0;2.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0x*]ÿrw,0";
            }
        }
        #endregion

        public DiamondLinkCopsNRobbersGameLogic()
        {
            _gameID = GAMEID.DiamondLinkCopsNRobbers;
            GameName = "DiamondLinkCopsNRobbers";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,2000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,20ÿ:m,20ÿ:b,14,1,2,3,4,5,8,10,15,20,30,40,50,80,100ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,85,17,81,28,5ÿx,2,dv:0.0;0.0;0.0;0.0;0.0;2.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0,[Z:S:R:85;17;81;28;5;0;*C:0.0;0.0;0.0;0.0;0.0;2.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;*P:dv:0.0;0.0;0.0;0.0;0.0;2.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0x*]ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
                resMessage.Append("\vFK_SERVERÿÿGAMEIDÿ15901ÿCATEGORYÿ1ÿTOURNIDÿÿISTOURNBYINVITATIONONLYÿ0ÿTOURNENTRYFEEÿ0ÿCALLSTARTGAMEÿ1ÿPRIORITYÿ1ÿTOURNDEADLINEHITÿ0ÿLINEWINSGGTÿ5.0ÿGUESTSÿ0ÿMULTIPLIERÿ25ÿCDATEÿ2025-06-28 00:06:22.283ÿDISPLAYORDERÿ1ÿCHATFILTERÿ1ÿLINESÿ25ÿJACKPOTWINSÿ[{\"b\":200,\"v\":{\"0\":true},\"t\":0,\"s\":true,\"p\":1,\"n\":\"Major\"},{\"b\":1000,\"v\":{\"0\":true},\"t\":0,\"s\":true,\"p\":1,\"n\":\"Grand\"}]ÿRANKINGÿ0ÿPORTÿÿASSIGNMENTINTERVALÿ180ÿGAMEVARIATIONÿ15977ÿTICKETSÿ2ÿMAXPLAYERSÿ100000ÿMUXIPÿ");
                resMessage.Append("$25ÿ2000ÿgrandÿ189.26820625ÿmajorÿ29.44111875");
                resMessage.Append(GameUniqueString);
                resMessage.Append("$25ÿ2000ÿgrandÿ189.26820625ÿmajorÿ29.44111875");
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
