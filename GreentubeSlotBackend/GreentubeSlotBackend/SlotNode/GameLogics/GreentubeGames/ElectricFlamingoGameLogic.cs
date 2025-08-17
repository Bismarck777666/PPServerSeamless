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
    internal class ElectricFlamingoGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol
        {
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ2578ÿ1.10.54.0ÿRedlineSlotServerÿ17106ÿ1.1.0ÿElectric Flamingo 95%ÿ1.119.0_2025-04-14_123855ÿ1.165.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49950,100.0,ÿM,10,10000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Electric Flamingo 95%ÿ:v,3ÿ:l,-----ÿ:r,0,Q1A33JCCCT0KAT2Q44Q3K00AST2K3AQST43K4T1KSQ22J34T11J0KQ00AST2K3JCCCÿ:r,0,TA03TQ4TQ41T2KACCCKA3TQ44K3TQ4AQ11AKWWWJ1T4JWWQTK31QT31JA22AQ33TA34KJWWAQ34AJ00J1QK02Jÿ:r,0,T42T33A1JQCCCJ11TASJ10KTJ14AQ00KA2QT4Q2K03QK24K22TASJ00KQ44KJ34Q20A31TJSKCCCÿ:r,0,TJ44AT13AJ43JK13JKT11KJQ42AT34KWWJCCCAJ0ATJ34AT33AK23JK43JK4JA43TJQ22T4AK00AWWWAJ0AQ12Qÿ:r,0,Q44Q31QST1QA41ASK3QA00TQ22AQCCCJ1KJ14Q33QJT2QA41ASQK11QA00Q4JQCCCJA1ÿ:r,1,Q1AQ33JTQ0KA12JQ44JQ31TAK00AST2K3AQSJ43K4T1KSJ22J34T11J0KJ00ASJ2KQ3JTÿ:r,1,TA13KQ4TQ41J3QA02KQA3TQ44K3TQ4AQ11AKWWWJ1T4JWWQTA31QJ31TA22AQT33TA34KTWWAQ34AJ00JT1QK02Jÿ:r,1,T42T33A1J40QK34QJ11TASJ10KT20J14AQ00KA2TKSQ2TA04Q2K03QK24K22TASJ00K2T44KJA34Q20A31TJSKT4Qÿ:r,1,TJ44AT14AQWWJK13JKT11KJQ32AT34KWWJQ12AJ0ATJ34AT33AK23JK43JK3JA43TJQ22T4AK00JAWWWAJ0AQ12Qÿ:r,1,Q44Q31QST1QA41ASQK13QA00TQ22AQK2TJ1QKJ1KQ33QJT2QA41ASQK11JA00Q4JQK4JQA1ÿ:r,2,CJK14J2S3TA1JTAK2JWAT0K3JWQ2KAÿ:r,2,CJK14J2S3TA1JTAK2JWAT0K3JWQ2KAÿ:r,2,CJK14J2S3TA1JTAK2JWAT0K3JWQ2KAÿ:r,2,CJK14J2S3TA1JTAK2JWAT0K3JWQ2KAÿ:r,2,CJK14J2S3TA1JTAK2JWAT0K3JWQ2KAÿ:j,W,1,0,01234AKQJTÿ:u,APPPP.PP.PP.PPPPÿ:w,0,3,25,0ÿ:w,0,4,75,0ÿ:w,0,5,200,0ÿ:w,1,3,25,0ÿ:w,1,4,50,0ÿ:w,1,5,100,0ÿ:w,2,3,25,0ÿ:w,2,4,50,0ÿ:w,2,5,80,0ÿ:w,3,3,20,0ÿ:w,3,4,40,0ÿ:w,3,5,80,0ÿ:w,4,3,10,0ÿ:w,4,4,30,0ÿ:w,4,5,75,0ÿ:w,A,3,5,0ÿ:w,A,4,15,0ÿ:w,A,5,50,0ÿ:w,K,3,5,0ÿ:w,K,4,15,0ÿ:w,K,5,50,0ÿ:w,Q,3,5,0ÿ:w,Q,4,15,0ÿ:w,Q,5,50,0ÿ:w,J,3,5,0ÿ:w,J,4,15,0ÿ:w,J,5,50,0ÿ:w,T,3,5,0ÿ:w,T,4,15,0ÿ:w,T,5,50,0ÿ:w,B,15,-1,0ÿ:w,B,14,-1,0ÿ:w,B,13,-1,0ÿ:w,B,12,-1,0ÿ:w,B,11,-1,0ÿ:w,B,10,-1,0ÿ:w,B,9,-1,0ÿ:w,B,8,-1,0ÿ:w,B,7,-1,0ÿ:w,B,6,-1,0ÿ:w,G,99,-200,0ÿ:w,U,1,-1,0ÿ:w,S,3,25,0ÿ:wa,0,3,25,0,0,0,0:1,0:1,0ÿ:wa,0,4,75,0,0,0,0:1,0:1,0ÿ:wa,0,5,200,0,0,0,0:1,0:1,0ÿ:wa,1,3,25,0,0,0,0:1,0:1,0ÿ:wa,1,4,50,0,0,0,0:1,0:1,0ÿ:wa,1,5,100,0,0,0,0:1,0:1,0ÿ:wa,2,3,25,0,0,0,0:1,0:1,0ÿ:wa,2,4,50,0,0,0,0:1,0:1,0ÿ:wa,2,5,80,0,0,0,0:1,0:1,0ÿ:wa,3,3,20,0,0,0,0:1,0:1,0ÿ:wa,3,4,40,0,0,0,0:1,0:1,0ÿ:wa,3,5,80,0,0,0,0:1,0:1,0ÿ:wa,4,3,10,0,0,0,0:1,0:1,0ÿ:wa,4,4,30,0,0,0,0:1,0:1,0ÿ:wa,4,5,75,0,0,0,0:1,0:1,0ÿ:wa,A,3,5,0,0,0,0:1,0:1,0ÿ:wa,A,4,15,0,0,0,0:1,0:1,0ÿ:wa,A,5,50,0,0,0,0:1,0:1,0ÿ:wa,K,3,5,0,0,0,0:1,0:1,0ÿ:wa,K,4,15,0,0,0,0:1,0:1,0ÿ:wa,K,5,50,0,0,0,0:1,0:1,0ÿ:wa,Q,3,5,0,0,0,0:1,0:1,0ÿ:wa,Q,4,15,0,0,0,0:1,0:1,0ÿ:wa,Q,5,50,0,0,0,0:1,0:1,0ÿ:wa,J,3,5,0,0,0,0:1,0:1,0ÿ:wa,J,4,15,0,0,0,0:1,0:1,0ÿ:wa,J,5,50,0,0,0,0:1,0:1,0ÿ:wa,T,3,5,0,0,0,0:1,0:1,0ÿ:wa,T,4,15,0,0,0,0:1,0:1,0ÿ:wa,T,5,50,0,0,0,0:1,0:1,0ÿ:wa,B,15,-1,0,0,0,0:1,-1,0ÿ:wa,B,14,-1,0,0,0,0:1,-1,0ÿ:wa,B,13,-1,0,0,0,0:1,-1,0ÿ:wa,B,12,-1,0,0,0,0:1,-1,0ÿ:wa,B,11,-1,0,0,0,0:1,-1,0ÿ:wa,B,10,-1,0,0,0,0:1,-1,0ÿ:wa,B,9,-1,0,0,0,0:1,-1,0ÿ:wa,B,8,-1,0,0,0,0:1,-1,0ÿ:wa,B,7,-1,0,0,0,0:1,-1,0ÿ:wa,B,6,-1,0,0,0,0:1,-1,0ÿ:wa,G,99,-200,0,0,0,0:1,-1,0ÿ:wa,U,1,-1,0,0,0,0:1,-1,0ÿ:wa,S,3,25,0,0,0,0:1,0:1,0ÿ:s,0ÿ:i,1ÿ:m,25ÿ:b,14,1,2,3,4,5,8,10,15,20,30,40,50,80,100ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,25.0ÿbs,0,1,0ÿ:k,nullÿe,25,0,0,0ÿb,1000,1000,0ÿs,1ÿr,0,5,61,51,54,0,10ÿx,3,00,CC,<ÿrw,0";
            }
        }
        #endregion

        public ElectricFlamingoGameLogic()
        {
            _gameID = GAMEID.ElectricFlamingo;
            GameName = "ElectricFlamingo";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,25,3000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:m,25ÿ:b,14,1,2,3,4,5,8,10,15,20,30,40,50,80,100ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,25.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,61,51,54,0,10ÿx,3,00,CC,<ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
