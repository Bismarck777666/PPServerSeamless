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
    internal class DiamondLinkMightyBuffaloGameLogic : BaseGreenLockRespinSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ15901ÿ1.10.54.0ÿGTSKDiamondLinkV2SlotServerÿ15977ÿ1.1.0ÿDiamond Link Mighty Buffalo Linked V2 95%ÿ1.117.0_2025-03-13_091353ÿ1.45.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,47705,100.0,ÿM,10,10000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Diamond Link Mighty Buffalo Linked V2 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:l,^_-_^ÿ:l,^^_^^ÿ:l,__^__ÿ:l,-^_^-ÿ:l,-_^_-ÿ:r,0,RRQHKDSQDAH#AD#KSKAEEEEEEKR#ADKSKHKHQDRASDQ#QHKSADQSQSHARADQHKSDKDKDQSAHQHARK#AMKRRQHKDQKDA#HAD#KSAEEEEEESKRADKSKHKHQRASDQ#QHKADQSQSAKRADQHKSQDKDKDQSAHQKHAR#ADMKRRQHKHDQDA#AD#RRKSAEEEEEEKSRADKSKHKHQRASDQ#QHQKADQSQSARADQHKSDKDHKDQSAHQHAR#AMKÿ:r,0,SAMKEEEE##QHADQHASKRQRKHADRKHQDSKRRSSQKA###DQSRRDADQAHDKRA##ASSQSKHSAMKEEEEQHADQHASKRQRHADRKHQDSKRRQKA###DQSDADQAHDKRA##ASSQSKHSAMKEEEEQHADQHASKRQRHADRKHQDSKRRQKAD###DQSDADQAHDKQRA##DASSQSKHÿ:r,0,SDAQKR###QAK###EEEEKHAD###RSAHRAHKSRQDKHDQRKHSQSQASQSAKDKDDQHQRRASAQSKADAHQRAHKMSQDKRASDAHQKR###QAK###EEEEKHADS###RSAHRAHKSRQDKHDQRKHSQSQSQSAKDKDDQHQARRASQSKADAHRAQHKMSQHDKRASDAQKR###QAK###EEEEKHAD###RSAHRAHKSRQDKHDQRAKHSQSKQSQSAKADKDDQHQRRASQSKRRADAKHRAHKMSQDKRAÿ:r,0,SMHAD###KEEEEEEEEDQKDHDA###QSKHSASRASHQMRQR###SKHDADRQASHQDKQDSARDHKSSKRSQDHKDRADRSSKRDAQHKSMHAD###KEEEEEEEEDQKDHDQA###SRKHSASRASHQMKRQRSKHD###ADRQAHSSQDKQDSARHAKSSKRSQDHKDSRADRKRDAQHKSMHAD###KEEEEEEEEDQKDHDA###SKHSASRASHQMRQRSKHDADRQAHQDKQDSADRHKASSKRSQDHKDRADRKRDAQHKÿ:r,0,D#ADRR#ASKSQASDEEEEEEEEQRK#HMKSAKSHQDSKMAHDKAQRRKSHK#HQHDQRDAD#ARRQ#ASKSDQADEEEEEEEEQRKHMKSASHQDHSKMAH#DKAQRRKSHK#HQMHDQRDAD#ARR#ASKSQMADEEEEEEEEQRAKHMKS#ASHQDSKMAHDKAQRRKSHK#HQHDQRDAÿ:r,1,RRQAAHKDQDAADQQKSAEEEEEEKRASKSQKHKHQRASDQQHQSDQSARADQRKSDKDRRKDQSAHQHARAKRRQHKSDQDAADKSAEEEEEESKRASKSKQHKHQRASDQQHQSQSARADQRKSDSKDKADQSAHQHARAKRRQHKDQDAADKSAEEEEEEKRASKSKHDKHQRASDQQHKQSQSARADQRKSDKSDKDRRQSAHQHASRAKÿ:r,1,SQAKEEEEQHADQHASKRQRHADRKHQDSKRRHQKADKHDADQAQDKRAASSQSKHSAQKEEEEQHADQHASKRQRHADRKDHQDSKRRQKADKHDADQAQDKDRAASSQSKHSAKEEEEQDHADQHASKRQRHADRKHQDSKRRQKADKHDADQAQDKRSSAASSQSKHÿ:r,1,SDAQKRQADEEEEAHQDKSAHRAHQKSRQDKHSDQRKHSKHSRSQSKAHDKDDQHQRRASQSQHDAHRAHKSQDKRASDADQKRQAHDEEEEAHQDKSAHRAHKSRQDKHDQRKHSKSRKSQSAHDKDDQHQRRASQSQHDAQHRAHKSQDKRASDAQSKRQADEEEEAHQDKSAHRAHKSRQDKHDQRKHSKSRSQSAHDKDDQHQRRASQSQHDAHRAHKSQDKRAÿ:r,1,SHKRKEEEEEEEEDKADHDASKHSASRASDHQRQRSKHDADRQAHRDKQHSAQHKSSKHHRSQDHKSDRADRQRDAHHSHKRAKEEEEEEEEDKADHDASKDHSASRASHQRQRSKHDAHDRQAHRDSKQHSAQHKSSKRSQDHKDKRADRAQRDAHHSHKRKEEEEEEEEDHKADHKDASKHSASRASHQRQRSKHDADRQAHRDKAQHSAQHKHSSKRSQDHKDRQADRQRDAHHÿ:r,1,DARRASKSQADEEEEEEEEQSRKHKSASHQDSHKAHDHQRRKSHKHQHDQRDADARRASKHSQADEEEEEEEEQRKSHKSASHQDSKSAHDQRRKSHKHQHDQRDADARRASKSQRRADEEEEEEEEQRKHAKSASHQDSKAHDQRRKSHKHQHDQRDAÿ:r,2,DSRRHRSDRASEHSRDEDADHRSEDSASDADHSHMRSDEHDSDRRHRSDRASEHRDEDADHRSHEDSASDADHRSHMRSDEHDSRRDHRSDRASEHRDEDSADHRSEDSASDADHSHMRSDRREHÿ:r,2,DHRERSARSDRESHASHDSDEADRDHSDASAHESHRMDHRERSARSDRESHASHDSHDEADRDHSDASAHESHRMDHRERSARSDRDESHASHADSDEADRDHSDASAHESHRMÿ:r,2,SDHRAEHRDSDSHSESHASARSRERDSHSDAHDRDEDHDADHSDSEHRARSMHSRSHRAEHRDSDSHSESHASARASRERDSHDAHDARDEDHDADHSDSHRARASMHSRSHRAEHRDSDSDHSESHASARSRAERDSAHDAHDRDEDHDADHSDSHRARSMHSRÿ:r,2,HDRDRSDRSAEDSRASHDRHRAEHARSRDHDHRSHSHRESASDASDHRDSAHEDHSHDMHDARDRSDRSAEDSRSHDRHAEHSARSRDHDHRSHSRESASDSDHRSAHEHDSHDMHDRDRSDRSAEDASRSHDRHAEHARSRDHDHRSHSRESASDSDHRSAHEHSHDMÿ:r,2,MHSDRERASDSRDSAESDHDSRAAHEHDSHERSHARDHMHSDRERASDSRDAESDHDSRAAHEHDSHERSARDHMHSDRERASDSRDAESDHDSRAAHEHDSHERHSARDHÿ:r,3,E0E00EEE00E0E0E00E0EEE0E0ÿ:r,3,EEEÿ:r,3,EEEÿ:r,3,EEEÿ:r,3,E0E0EEE0000EEE0000E00E0E000ÿ:r,4,H0H00HHH00H0H0H00H0HHH0H0ÿ:r,4,HHHÿ:r,4,HHHÿ:r,4,HHHÿ:r,4,H0H0HHH0000HHH0000H00H0H000ÿ:r,5,R0R00RRR00R0R0R00R0RRR0R0ÿ:r,5,RRRÿ:r,5,RRRÿ:r,5,RRRÿ:r,5,R0R0RRR0000RRR0000R00R0R000ÿ:r,6,D0D00DDD00D0D0D00D0DDD0D0ÿ:r,6,DDDÿ:r,6,DDDÿ:r,6,DDDÿ:r,6,D0D0DDD0000DDD0000D00D0D000ÿ:r,7,S0S00SSS00S0S0S00S0SSS0S0ÿ:r,7,SSSÿ:r,7,SSSÿ:r,7,SSSÿ:r,7,S0S0SSS0000SSS0000S00S0S000ÿ:r,8,A0A00AAA00A0A0A00A0AAA0A0ÿ:r,8,AAAÿ:r,8,AAAÿ:r,8,AAAÿ:r,8,A0A0AAA0000AAA0000A00A0A000ÿ:j,M,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,#,101,-200,1:Majorÿ:w,#,102,-1000,1:Grandÿ:w,#,15,-1,0ÿ:w,#,14,-1,0ÿ:w,#,13,-1,0ÿ:w,#,12,-1,0ÿ:w,#,11,-1,0ÿ:w,#,10,-1,0ÿ:w,#,9,-1,0ÿ:w,#,8,-1,0ÿ:w,#,7,-1,0ÿ:w,#,6,-1,0ÿ:w,Z,3,-1,0ÿ:w,Z,2,-1,0ÿ:w,Z,1,-1,0ÿ:w,#,103,-1,0ÿ:w,M,2,10,0ÿ:w,M,3,50,0ÿ:w,M,4,200,0ÿ:w,M,5,750,0ÿ:w,E,2,10,0ÿ:w,E,3,50,0ÿ:w,E,4,200,0ÿ:w,E,5,750,0ÿ:w,H,3,20,0ÿ:w,H,4,60,0ÿ:w,H,5,150,0ÿ:w,R,3,20,0ÿ:w,R,4,60,0ÿ:w,R,5,150,0ÿ:w,D,3,15,0ÿ:w,D,4,30,0ÿ:w,D,5,75,0ÿ:w,S,3,15,0ÿ:w,S,4,30,0ÿ:w,S,5,75,0ÿ:w,A,3,10,0ÿ:w,A,4,25,0ÿ:w,A,5,50,0ÿ:w,K,3,5,0ÿ:w,K,4,15,0ÿ:w,K,5,30,0ÿ:w,Q,3,5,0ÿ:w,Q,4,15,0ÿ:w,Q,5,30,0ÿ:wa,#,101,-200,1:Major,0,0,0:1,0,0ÿ:wa,#,102,-1000,1:Grand,0,0,0:1,0,0ÿ:wa,#,15,-1,0,0,0,0:1,0,0ÿ:wa,#,14,-1,0,0,0,0:1,0,0ÿ:wa,#,13,-1,0,0,0,0:1,0,0ÿ:wa,#,12,-1,0,0,0,0:1,0,0ÿ:wa,#,11,-1,0,0,0,0:1,0,0ÿ:wa,#,10,-1,0,0,0,0:1,0,0ÿ:wa,#,9,-1,0,0,0,0:1,0,0ÿ:wa,#,8,-1,0,0,0,0:1,0,0ÿ:wa,#,7,-1,0,0,0,0:1,0,0ÿ:wa,#,6,-1,0,0,0,0:1,0,0ÿ:wa,Z,3,-1,0,0,0,0:1,0,0ÿ:wa,Z,2,-1,0,0,0,0:1,0,0ÿ:wa,Z,1,-1,0,0,0,0:1,0,0ÿ:wa,#,103,-1,0,0,0,0:1,0,0ÿ:wa,M,2,10,0,0,0,0:1,-1,0ÿ:wa,M,3,50,0,0,0,0:1,-1,0ÿ:wa,M,4,200,0,0,0,0:1,-1,0ÿ:wa,M,5,750,0,0,0,0:1,-1,0ÿ:wa,E,2,10,0,0,0,0:1,-1,0ÿ:wa,E,3,50,0,0,0,0:1,-1,0ÿ:wa,E,4,200,0,0,0,0:1,-1,0ÿ:wa,E,5,750,0,0,0,0:1,-1,0ÿ:wa,H,3,20,0,0,0,0:1,-1,0ÿ:wa,H,4,60,0,0,0,0:1,-1,0ÿ:wa,H,5,150,0,0,0,0:1,-1,0ÿ:wa,R,3,20,0,0,0,0:1,-1,0ÿ:wa,R,4,60,0,0,0,0:1,-1,0ÿ:wa,R,5,150,0,0,0,0:1,-1,0ÿ:wa,D,3,15,0,0,0,0:1,-1,0ÿ:wa,D,4,30,0,0,0,0:1,-1,0ÿ:wa,D,5,75,0,0,0,0:1,-1,0ÿ:wa,S,3,15,0,0,0,0:1,-1,0ÿ:wa,S,4,30,0,0,0,0:1,-1,0ÿ:wa,S,5,75,0,0,0,0:1,-1,0ÿ:wa,A,3,10,0,0,0,0:1,-1,0ÿ:wa,A,4,25,0,0,0,0:1,-1,0ÿ:wa,A,5,50,0,0,0,0:1,-1,0ÿ:wa,K,3,5,0,0,0,0:1,-1,0ÿ:wa,K,4,15,0,0,0,0:1,-1,0ÿ:wa,K,5,30,0,0,0,0:1,-1,0ÿ:wa,Q,3,5,0,0,0,0:1,-1,0ÿ:wa,Q,4,15,0,0,0,0:1,-1,0ÿ:wa,Q,5,30,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,25ÿ:m,25ÿ:b,13,1,2,3,4,5,8,10,15,20,30,40,50,80ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,25.0ÿbs,0,1,0ÿ:k,nullÿe,25,0,0,0ÿb,1000,1000,0ÿ:x,750ÿs,1ÿr,0,5,13,40,69,4,42ÿx,1,dl:13:-1;77:5;46:-1;58:3;58:3;59:5;69:-1;70:-1;71:-1;4:-1;24:1;24:2;42:-1;43:-1;44:-1;3;oÿc,#,11,0,0,0,0,ef:,011011123132,0ÿrw,0";
            }
        }
        #endregion

        public DiamondLinkMightyBuffaloGameLogic()
        {
            _gameID = GAMEID.DiamondLinkMightyBuffalo;
            GameName = "DiamondLinkMightyBuffalo";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,25,2000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,25ÿ:m,25ÿ:b,13,1,2,3,4,5,8,10,15,20,30,40,50,80ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,25.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,13,40,69,4,42ÿx,1,dl:13:-1;77:5;46:-1;58:3;58:3;59:5;69:-1;70:-1;71:-1;4:-1;24:1;24:2;42:-1;43:-1;44:-1;3;oÿc,#,11,0,0,0,0,ef:,011011123132,0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
