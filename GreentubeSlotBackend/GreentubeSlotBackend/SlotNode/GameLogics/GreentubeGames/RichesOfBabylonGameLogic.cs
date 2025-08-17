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
    internal class RichesOfBabylonGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ898ÿ1.10.54.0ÿGTSKSlotServerÿ11152ÿ1.2.0ÿRiches of Babylon 95%ÿ1.120.0_2025-05-09_075230ÿ1.313.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Riches of Babylon 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:l,^_-_^ÿ:l,^^_^^ÿ:l,__^__ÿ:l,-^_^-ÿ:l,-_^_-ÿ:r,0,KVTKBBBBQTJAQLLLLTKQJFTBBBBAKJBBBBKQJTWKJATLLLLQJVTLLLLJAQFAQÿ:r,0,KABBBBTJKWTJAKLLLLKAQJBBBBJQATLLLLKAQJTWQVJTKVTQAFQWJQTÿ:r,0,QBBBBAWQABBKTQLLLLTJALLQJTVKJBBBBAKWJAKLLLLKTFJQVKQFJQBBJTALLTAKÿ:r,0,LLLJQWKJQATBBBBTKVJTQAKJTQKLLLJQFJTAKQAVTQBBBJAFKTAKTJAQKÿ:r,0,KTBBQJKAWJQTJALLLLQAKTBBBQKTVQTKAJLLLJATQFJKBBJQWJTAFKVTQAÿ:r,1,KVQKBBBBBBBBBBBBTKJFAQTJAQKJVQJTKQJTWKJATLLLLLLLLLLLLQATFAQJTÿ:r,1,BBBBBBBBBBBBQKJLLLLLLLLLLLATJQTKWAQKJVQTKVTQAFKWJTAJTAJÿ:r,1,QBBBBBBBBBBBTAQTLKJALLLLLLLLLLLTAKWJAKTAKQJTFJQVKQFJQVKJTAQJTAKTÿ:r,1,LLLLLLLKJQKTBBBBBBJTQAKJTQKVATJQFJTAKQAVTQWJQAJFKTAKTJVAQÿ:r,1,KJBBBBBBBJKTJAQTJWQAKJQATQKTVQTKLLLLLLLAQFJKATJQWJTAFKVTQAÿ:r,2,JTQJWWWQJBBBBBATKJAKTQLLLLLKQAKVTAQJAQTBBBJKTVQJLLLTKAVKÿ:r,2,TQAKTWWWKTBBBBKTJAQLLLLKJATJAKBBBBAJTQVKJAQJLLLLQTKVJQATLLJATQVJAKQBBKAQJÿ:r,2,AKWWWAKJAQLLLLLQJKTJBBQAKTQVKTJALLLTAQJBBBBBQJTKQVKATJQLLKQTJKATBBTAJKVTJLLTAJVQKBBBKQTAKQJÿ:r,2,AQJTKBBBBJQAKTLLLTAQJAQBBBAJTLLLLKTJKBBKJQTALLLQTJKTBBBQTAKJWWWKJQAVKAQTKLLAJTVQAKTAQVJTAQJKVQTÿ:r,2,KQJKTJBBBBQKAVQJLLTQAKJVTABBQKLLLLKQTJKTVAQLLLAQJKTABBBAJQTJKTAWWWTAVKTQKAJBBTKJQTJQLLJÿ:r,3,JTQJLLKQJBBBBBATKJAKTQLLLLLKJAKVTAQJAQTBBBBBTVQJLLLTKAVKÿ:r,3,TQAKTWWWKTBBBBKTJAQLLLLKJATJAKBBBBAJTQVKJAQJLLLLQTKVJQATLLJATQVJAKQBBKAQJÿ:r,3,AKWWWAKJAQLLLLLQJKTJBBQAKTQVKTJALLLTAQJBBBBBQJTKQVKATJQLLKQTJKATBBTAJKVTJLLTAJVQKBBBKQTAKQJÿ:r,3,AQJTKBBBBJQAKTLLLTAQJAQBBBAJTLLLLKTJKBBKJQTALLLQTJKTBBBQTAKJWWWKJQAVKAQTKLLAJTVQAKTAQVJTAQJKVQTÿ:r,3,KQJKTJBBBBQKAVQJLLTQAKJVTABBQAKLLLLKQTJKTVAQLLLLQJKTABBBAJQTJKTABBBTAVKTQKAJTKJQTJQLLJAÿ:r,4,JTQJLLKQJBBBBBATKJAKTQLLLLLKJAKVTAQJAQTBBBBBTVQJLLLTKAVKÿ:r,4,TQAKTWWWKTBBBBKTJAQLLLLKJATJAKBBBBAJTQVKJAQJLLLLQTKVJQATLLJATQVJAKQBBKAQJÿ:r,4,AKJTKAQLLLLLLJKQJBBBAKTQVAKTJALLLTAQJABBBBBQJTKQVKATJQLLKQTJKATBBTAJKVTJLLQTAJVQKBBBKQTAKQJÿ:r,4,AQJTKBBBBJQAKTLLLTAQJAQBBBAJTLLLLKTJKBBKJQTALLLQTJKTBBBQTAKJWWWKJQAVKAQTKLLAJTVQAKTAQVJTAQJKVQTÿ:r,4,KQJKTJBBBBQKAVQJLLTQAKJVTABBQAKLLLLKQTJKTVAQLLLLQJKTABBBAJQTJKTABBBTAVKTQKAJTKJQTJQLLJAÿ:r,5,JTQJWWWQJBBBBBATKJAKTQLLLLLKQAKVTAQJAQTBBBJKTVQJLLLTKAVKÿ:r,5,TQAKTQAJKTBBBBBTJKQLLLLLJATJAKBBBBKJTQVKJAQJLLLLQTAVKQATLLJATQVJAKQBBKAQJÿ:r,5,AKWWWAKJAQLLLLLQJKTJBBQAKTQVKTJALLLTAQJBBBBBQJTKQVKATJQLLKQTJKATBBTAJKVTJLLTAJVQKBBBKQTAKQJÿ:r,5,AQJTKBBBBJQAKTLLLLAQJABBBBAJTLLLLKTJKBBKJQTALLLQTJKTBBBQTAKJTQKJQAVKAQTKLLAJTVQAKTAQVJTAQJKVQTJÿ:r,5,KQJKTJBBBBQKAVQJLLTQAKJVTABBQKLLLLKQTJKTVAQLLLAQJKTABBBAJQTJKTAWWWTAVKTQKAJBBTKJQTJQLLJÿ:j,W,1,0,VLBAKQJTÿ:u,APPPP.PP.PP.PPPPÿ:w,S,1,-1,0ÿ:w,F,5,-100,0ÿ:w,F,4,-10,0ÿ:w,F,3,-2,0ÿ:w,W,3,25,0ÿ:w,W,4,100,0ÿ:w,W,5,500,0ÿ:w,V,3,20,0ÿ:w,V,4,75,0ÿ:w,V,5,250,0ÿ:w,L,3,15,0ÿ:w,L,4,50,0ÿ:w,L,5,100,0ÿ:w,B,3,15,0ÿ:w,B,4,50,0ÿ:w,B,5,100,0ÿ:w,A,3,10,0ÿ:w,A,4,25,0ÿ:w,A,5,75,0ÿ:w,K,3,10,0ÿ:w,K,4,25,0ÿ:w,K,5,75,0ÿ:w,Q,3,5,0ÿ:w,Q,4,15,0ÿ:w,Q,5,50,0ÿ:w,J,3,5,0ÿ:w,J,4,15,0ÿ:w,J,5,50,0ÿ:w,T,3,5,0ÿ:w,T,4,15,0ÿ:w,T,5,50,0ÿ:wa,S,1,-1,0,0,0,0:1,-1,0ÿ:wa,F,5,-100,0,0,0,0:1,0:1,0ÿ:wa,F,4,-10,0,0,0,0:1,0:1,0ÿ:wa,F,3,-2,0,0,0,0:1,0:1,0ÿ:wa,W,3,25,0,0,0,0:1,0:1,0ÿ:wa,W,4,100,0,0,0,0:1,0:1,0ÿ:wa,W,5,500,0,0,0,0:1,0:1,0ÿ:wa,V,3,20,0,0,0,0:1,0:1,0ÿ:wa,V,4,75,0,0,0,0:1,0:1,0ÿ:wa,V,5,250,0,0,0,0:1,0:1,0ÿ:wa,L,3,15,0,0,0,0:1,0:1,0ÿ:wa,L,4,50,0,0,0,0:1,0:1,0ÿ:wa,L,5,100,0,0,0,0:1,0:1,0ÿ:wa,B,3,15,0,0,0,0:1,0:1,0ÿ:wa,B,4,50,0,0,0,0:1,0:1,0ÿ:wa,B,5,100,0,0,0,0:1,0:1,0ÿ:wa,W,2,5,0,0,0,0:1,2:3:4:5,0ÿ:wa,W,3,25,0,0,0,0:1,2:3:4:5,0ÿ:wa,W,4,100,0,0,0,0:1,2:3:4:5,0ÿ:wa,W,5,500,0,0,0,0:1,2:3:4:5,0ÿ:wa,V,2,5,0,0,0,0:1,2:3:4:5,0ÿ:wa,V,3,20,0,0,0,0:1,2:3:4:5,0ÿ:wa,V,4,75,0,0,0,0:1,2:3:4:5,0ÿ:wa,V,5,250,0,0,0,0:1,2:3:4:5,0ÿ:wa,L,2,5,0,0,0,0:1,2:3:4:5,0ÿ:wa,L,3,15,0,0,0,0:1,2:3:4:5,0ÿ:wa,L,4,50,0,0,0,0:1,2:3:4:5,0ÿ:wa,L,5,100,0,0,0,0:1,2:3:4:5,0ÿ:wa,B,2,5,0,0,0,0:1,2:3:4:5,0ÿ:wa,B,3,15,0,0,0,0:1,2:3:4:5,0ÿ:wa,B,4,50,0,0,0,0:1,2:3:4:5,0ÿ:wa,B,5,100,0,0,0,0:1,2:3:4:5,0ÿ:wa,A,3,10,0,0,0,0:1,-1,0ÿ:wa,A,4,25,0,0,0,0:1,-1,0ÿ:wa,A,5,75,0,0,0,0:1,-1,0ÿ:wa,K,3,10,0,0,0,0:1,-1,0ÿ:wa,K,4,25,0,0,0,0:1,-1,0ÿ:wa,K,5,75,0,0,0,0:1,-1,0ÿ:wa,Q,3,5,0,0,0,0:1,-1,0ÿ:wa,Q,4,15,0,0,0,0:1,-1,0ÿ:wa,Q,5,50,0,0,0,0:1,-1,0ÿ:wa,J,3,5,0,0,0,0:1,-1,0ÿ:wa,J,4,15,0,0,0,0:1,-1,0ÿ:wa,J,5,50,0,0,0,0:1,-1,0ÿ:wa,T,3,5,0,0,0,0:1,-1,0ÿ:wa,T,4,15,0,0,0,0:1,-1,0ÿ:wa,T,5,50,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,25ÿ:m,25ÿ:a,0,0ÿ:g,5,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,25,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,9,52,0,30,40ÿrw,0";
            }
        }
        #endregion

        public RichesOfBabylonGameLogic()
        {
            _gameID = GAMEID.RichesOfBabylon;
            GameName = "RichesOfBabylon";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,25,2500,25,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,25ÿ:m,25ÿ:b,14,1,2,3,4,5,8,10,15,20,30,40,50,80,100ÿ:a,0,0ÿ:g,5,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,25.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,9,52,0,30,40ÿx,4,b:33,s:33,d:1;33;82:2;33;82:3;33;82:4;33;82:5;33;82:8;33;82:10;33;82:15;33;82:20;33;82:30;33;82:40;33;82:50;33;82:80;33;82:100;33;82,n:1;33;82:2;33;82:3;33;82:4;33;82:5;33;82:8;33;82:10;33;82:15;33;82:20;33;82:30;33;82:40;33;82:50;33;82:80;33;82:100;33;82ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
