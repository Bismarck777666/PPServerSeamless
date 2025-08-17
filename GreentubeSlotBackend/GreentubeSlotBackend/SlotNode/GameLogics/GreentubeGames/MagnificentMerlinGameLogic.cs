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
    internal class MagnificentMerlinGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ2578ÿ1.10.54.0ÿRedlineSlotServerÿ15846ÿ1.0.0ÿMagnificent Merlin 95%ÿ1.119.0_2025-04-14_123855ÿ1.165.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Magnificent Merlin 95%ÿ:v,7ÿ:l,-----ÿ:r,0,A3KTTTJJ3A2QQTTTA3QQ1J3AATTTQ3AATTTJ2AATTTQ1KK3Q1CK3QTTTCJ1AA11QQ3A4KK3J4QQ2K3JJ1A3KK2J3QQ4K1AA4QQJA2AA1J2TT34ÿ:r,0,K4JTTTA2KK4J2Q3KK4Q2J3CC4JTTTQ2JJ3A4K3QQTTTA2Q4JJ3Q4K3QQ4AATJWWJJ4KTTWW2KK4Q3J2AA1QQCCCK3JJ214Q2AK42KK23WJ1A4Tÿ:r,0,4TTTK4JJTTTK2AA4WWTTQQ3J2AA4K3JJ1K4AA241QQ4K3JJJTWWKKTTTJ4KK1A2JJ3A1KK2A4JJ3QKCC1K4QQ1A4KK3A4JJ2ACCCKQQ14WT2A3ÿ:r,0,ATTTJ2K4Q2AA3J4KTTTJ2CC3Q4A3J2KK3Q4A2K4QQTTTA4Q2JJWWQQTTTA3Q2J4AA2Q4A2ATTQQ4CCC4K3AA1Q4K2J3QQ2K3A421KJJW21Q3A4ÿ:r,0,A3Q2JJTTTQ3AATTTQ1KK4J3QQ2K3AA4J3KK4C1JJ3K1AA2J3KK4J1AA2J4QQ2KTTTAA1Q3JJ11QQ2J1CK4J3QQ1K3JJTK4A41K4K4AA1J2TT34ÿ:r,1,CJK14J3T1JTAK2JWATK3JWQ2KAÿ:r,1,CJK14J3T1JTAK2JWATK3JWQ2KAÿ:r,1,CJK14J3T1JTAK2JWATK3JWQ2KAÿ:r,1,CJK14J3T1JTAK2JWATK3JWQ2KAÿ:r,1,CJK14J3T1JTAK2JWATK3JWQ2KAÿ:j,W,1,0,1234AKQJTÿ:u,APPPP.PP.PP.PPPPÿ:w,W,3,15,0ÿ:w,W,4,30,0ÿ:w,W,5,100,0ÿ:w,1,3,25,0ÿ:w,1,4,50,0ÿ:w,1,5,200,0ÿ:w,2,3,15,0ÿ:w,2,4,30,0ÿ:w,2,5,100,0ÿ:w,3,3,10,0ÿ:w,3,4,20,0ÿ:w,3,5,50,0ÿ:w,4,3,10,0ÿ:w,4,4,20,0ÿ:w,4,5,50,0ÿ:w,A,3,5,0ÿ:w,A,4,15,0ÿ:w,A,5,30,0ÿ:w,K,3,5,0ÿ:w,K,4,15,0ÿ:w,K,5,30,0ÿ:w,Q,3,5,0ÿ:w,Q,4,10,0ÿ:w,Q,5,20,0ÿ:w,J,3,5,0ÿ:w,J,4,10,0ÿ:w,J,5,20,0ÿ:w,T,3,5,0ÿ:w,T,4,10,0ÿ:w,T,5,20,0ÿ:w,B,27,-1,0ÿ:w,B,26,-1,0ÿ:w,B,25,-1,0ÿ:w,B,24,-1,0ÿ:w,B,23,-1,0ÿ:w,B,22,-1,0ÿ:w,B,21,-1,0ÿ:w,B,20,-1,0ÿ:w,B,19,-1,0ÿ:w,B,18,-1,0ÿ:w,B,17,-1,0ÿ:w,B,16,-1,0ÿ:w,B,15,-1,0ÿ:w,B,14,-1,0ÿ:w,B,13,-1,0ÿ:w,B,12,-1,0ÿ:w,B,11,-1,0ÿ:w,B,10,-1,0ÿ:w,B,9,-1,0ÿ:w,B,8,-1,0ÿ:w,B,7,-1,0ÿ:w,B,6,-1,0ÿ:w,B,5,-1,0ÿ:w,G,99,-2000,0ÿ:wa,W,3,15,0,0,0,0:1,0,0ÿ:wa,W,4,30,0,0,0,0:1,0,0ÿ:wa,W,5,100,0,0,0,0:1,0,0ÿ:wa,1,3,25,0,0,0,0:1,0,0ÿ:wa,1,4,50,0,0,0,0:1,0,0ÿ:wa,1,5,200,0,0,0,0:1,0,0ÿ:wa,2,3,15,0,0,0,0:1,0,0ÿ:wa,2,4,30,0,0,0,0:1,0,0ÿ:wa,2,5,100,0,0,0,0:1,0,0ÿ:wa,3,3,10,0,0,0,0:1,0,0ÿ:wa,3,4,20,0,0,0,0:1,0,0ÿ:wa,3,5,50,0,0,0,0:1,0,0ÿ:wa,4,3,10,0,0,0,0:1,0,0ÿ:wa,4,4,20,0,0,0,0:1,0,0ÿ:wa,4,5,50,0,0,0,0:1,0,0ÿ:wa,A,3,5,0,0,0,0:1,0,0ÿ:wa,A,4,15,0,0,0,0:1,0,0ÿ:wa,A,5,30,0,0,0,0:1,0,0ÿ:wa,K,3,5,0,0,0,0:1,0,0ÿ:wa,K,4,15,0,0,0,0:1,0,0ÿ:wa,K,5,30,0,0,0,0:1,0,0ÿ:wa,Q,3,5,0,0,0,0:1,0,0ÿ:wa,Q,4,10,0,0,0,0:1,0,0ÿ:wa,Q,5,20,0,0,0,0:1,0,0ÿ:wa,J,3,5,0,0,0,0:1,0,0ÿ:wa,J,4,10,0,0,0,0:1,0,0ÿ:wa,J,5,20,0,0,0,0:1,0,0ÿ:wa,T,3,5,0,0,0,0:1,0,0ÿ:wa,T,4,10,0,0,0,0:1,0,0ÿ:wa,T,5,20,0,0,0,0:1,0,0ÿ:wa,B,27,-1,0,0,0,0:1,-1,0ÿ:wa,B,26,-1,0,0,0,0:1,-1,0ÿ:wa,B,25,-1,0,0,0,0:1,-1,0ÿ:wa,B,24,-1,0,0,0,0:1,-1,0ÿ:wa,B,23,-1,0,0,0,0:1,-1,0ÿ:wa,B,22,-1,0,0,0,0:1,-1,0ÿ:wa,B,21,-1,0,0,0,0:1,-1,0ÿ:wa,B,20,-1,0,0,0,0:1,-1,0ÿ:wa,B,19,-1,0,0,0,0:1,-1,0ÿ:wa,B,18,-1,0,0,0,0:1,-1,0ÿ:wa,B,17,-1,0,0,0,0:1,-1,0ÿ:wa,B,16,-1,0,0,0,0:1,-1,0ÿ:wa,B,15,-1,0,0,0,0:1,-1,0ÿ:wa,B,14,-1,0,0,0,0:1,-1,0ÿ:wa,B,13,-1,0,0,0,0:1,-1,0ÿ:wa,B,12,-1,0,0,0,0:1,-1,0ÿ:wa,B,11,-1,0,0,0,0:1,-1,0ÿ:wa,B,10,-1,0,0,0,0:1,-1,0ÿ:wa,B,9,-1,0,0,0,0:1,-1,0ÿ:wa,B,8,-1,0,0,0,0:1,-1,0ÿ:wa,B,7,-1,0,0,0,0:1,-1,0ÿ:wa,B,6,-1,0,0,0,0:1,-1,0ÿ:wa,B,5,-1,0,0,0,0:1,-1,0ÿ:wa,G,99,-2000,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:m,50ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,50,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,15,73,4,14,74ÿx,2,,ÿrw,0";
            }
        }
        #endregion

        public MagnificentMerlinGameLogic()
        {
            _gameID = GAMEID.MagnificentMerlin;
            GameName = "MagnificentMerlin";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,50,2500,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:m,50ÿ:b,12,1,2,3,4,5,8,10,15,20,30,40,50ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,50.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,15,73,4,14,74ÿx,2,,ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
