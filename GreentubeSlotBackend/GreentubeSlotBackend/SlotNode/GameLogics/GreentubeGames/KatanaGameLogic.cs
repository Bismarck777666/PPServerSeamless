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
    internal class KatanaGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol
        {
            get
            {
                return "#";
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ103ÿ1.0.0ÿKatana 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Katana 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:r,0,S1JLJQRKBATQGKA#J1L1RJBJTQGKRABQTKGA1S1JLJQRKBATQGK1A#J1L1RJBJTQGKRABQTKGA1S1JLJQRKBATQGKA#J1L1RJBJTQGKRABQTKGA1ÿ:r,0,S1JLJQRKBAT1GQK#A1L1RJBJTQGKRAB1QTKGA1S1JLJQRKBAT1GQK#A1L1RJBJTQGKRABQTKGA1S1JLJQRKBAT1GQK#A1L1RJBJTQGKRA1BQTKGA1ÿ:r,0,SKQGATKBQRJLJ1#1AGKTQBARKLQ1GJTJQB1R1ASKQGATKBQRJLJ1#1AGKTQBARKLQGJTJB1R1ASKQGATKBQRJLJ1#1AGKTQBARKLQGJTJB1R1Aÿ:r,0,S1LJRJBQTKGA#QLJKRAB1T1GJLJRJQBKTAGARB1S1LJRJBQTKGA#QLKRAB1QT1GJLJRQBKT1AGARB1S1LJRJBQTKGA#QLKRAB1TK1GJLJRQBKTAGARB1ÿ:r,0,SKQGATKBQRJLJ1#1GATKBQRKLQGJTJB1R1LASKQGA1TKBQRJLJ1#1GATKBQRKLQAGJTJB1R1LASKQGATKBQRJLJ1#1GATKBQRKLQGKJTJB1R1LAÿ:r,1,XJ1LARKBQTAGJ1XKQLARJBJTG1#1RBATGKQXJ1LARKBQTAGJ1XKQLARJBJTG1#1RBATGKQXJ1LARKBQJTAGJ1XKQLARJBJTG1#1RBATGKQÿ:r,1,XJ1LKRBKTGA#KLJQRBKTGQRBJ1XAQ#JT1GAQXJ1LKRBKTGA#KLQRBKTGQRBJ1XAQ#JT1GAQXJ1LKRBKTGA#KLQRBKTGQRBJ1XAQ#JT1GAQÿ:r,1,XKAG1TB1RLJGQ1#KTABR1LG1KAXJQTB1RJ#JQXKAG1TB1RLJGQ#KTABR1LGKJAXJQTB1RJ#JQXKAG1TB1RLJGQ#KTABR1LGKAXJQTB1RJ#JQÿ:r,1,XAKLQRJBJT1G1#ALKRQBATKGQJ#JR1B1TQGAKXA1KLQRJKBJT1G1#ALKRQBATKGQJ#JR1B1ATQGAKXAKLQRJBJT1G1#ALKRQBATKGQJ#JR1B1TQGAKÿ:r,1,XQAG1T1BJRJLQ#KGATQBKRAL1#1JGJTKBKRQAXQJAG1T1BJRJLQ#KGATQBKRAL1#1JGJTKBKRJQAXQAG1T1BJRJLQ#KGA1TQBKRAL1#1JGJTKBKRQAÿ:j,S,1,0,ÿ:j,X,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,#,5,-200,0ÿ:w,#,4,-20,0ÿ:w,#,3,-2,0ÿ:w,S,5,1000,0ÿ:w,S,4,500,0ÿ:w,S,3,100,0ÿ:w,L,5,800,0ÿ:w,L,4,250,0ÿ:w,L,3,80,0ÿ:w,R,5,600,0ÿ:w,R,4,200,0ÿ:w,R,3,40,0ÿ:w,B,5,600,0ÿ:w,B,4,200,0ÿ:w,B,3,40,0ÿ:w,T,5,500,0ÿ:w,T,4,150,0ÿ:w,T,3,30,0ÿ:w,G,5,500,0ÿ:w,G,4,150,0ÿ:w,G,3,30,0ÿ:w,A,5,200,0ÿ:w,A,4,50,0ÿ:w,A,3,10,0ÿ:w,K,5,150,0ÿ:w,K,4,30,0ÿ:w,K,3,10,0ÿ:w,Q,5,150,0ÿ:w,Q,4,30,0ÿ:w,Q,3,10,0ÿ:w,J,5,100,0ÿ:w,J,4,20,0ÿ:w,J,3,10,0ÿ:w,1,5,100,0ÿ:w,1,4,20,0ÿ:w,1,3,10,0ÿ:wa,#,5,-200,0,0,0,0:1,-1,0ÿ:wa,#,4,-20,0,0,0,0:1,-1,0ÿ:wa,#,3,-2,0,0,0,0:1,-1,0ÿ:wa,S,5,1000,0,0,0,0:1,-1,0ÿ:wa,S,4,500,0,0,0,0:1,-1,0ÿ:wa,S,3,100,0,0,0,0:1,-1,0ÿ:wa,L,5,800,0,0,0,0:1,-1,0ÿ:wa,L,4,250,0,0,0,0:1,-1,0ÿ:wa,L,3,80,0,0,0,0:1,-1,0ÿ:wa,R,5,600,0,0,0,0:1,-1,0ÿ:wa,R,4,200,0,0,0,0:1,-1,0ÿ:wa,R,3,40,0,0,0,0:1,-1,0ÿ:wa,B,5,600,0,0,0,0:1,-1,0ÿ:wa,B,4,200,0,0,0,0:1,-1,0ÿ:wa,B,3,40,0,0,0,0:1,-1,0ÿ:wa,T,5,500,0,0,0,0:1,-1,0ÿ:wa,T,4,150,0,0,0,0:1,-1,0ÿ:wa,T,3,30,0,0,0,0:1,-1,0ÿ:wa,G,5,500,0,0,0,0:1,-1,0ÿ:wa,G,4,150,0,0,0,0:1,-1,0ÿ:wa,G,3,30,0,0,0,0:1,-1,0ÿ:wa,A,5,200,0,0,0,0:1,-1,0ÿ:wa,A,4,50,0,0,0,0:1,-1,0ÿ:wa,A,3,10,0,0,0,0:1,-1,0ÿ:wa,K,5,150,0,0,0,0:1,-1,0ÿ:wa,K,4,30,0,0,0,0:1,-1,0ÿ:wa,K,3,10,0,0,0,0:1,-1,0ÿ:wa,Q,5,150,0,0,0,0:1,-1,0ÿ:wa,Q,4,30,0,0,0,0:1,-1,0ÿ:wa,Q,3,10,0,0,0,0:1,-1,0ÿ:wa,J,5,100,0,0,0,0:1,-1,0ÿ:wa,J,4,20,0,0,0,0:1,-1,0ÿ:wa,J,3,10,0,0,0,0:1,-1,0ÿ:wa,1,5,100,0,0,0,0:1,-1,0ÿ:wa,1,4,20,0,0,0,0:1,-1,0ÿ:wa,1,3,10,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,11ÿ:i,12ÿ:i,13ÿ:i,14ÿ:i,15ÿ:i,16ÿ:i,17ÿ:i,18ÿ:i,19ÿ:i,20ÿ:m,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,20,0,19,-1ÿb,0,0,0ÿs,1ÿr,0,5,25,16,8,1,8ÿrw,0";
            }
        }
        #endregion

        public KatanaGameLogic()
        {
            _gameID = GAMEID.Katana;
            GameName = "Katana";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,3000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,11ÿ:i,12ÿ:i,13ÿ:i,14ÿ:i,15ÿ:i,16ÿ:i,17ÿ:i,18ÿ:i,19ÿ:i,20ÿ:f,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19ÿ:m,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20ÿ:e,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0ÿ:b,15,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,25,16,8,1,8ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
