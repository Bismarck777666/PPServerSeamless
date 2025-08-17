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
    internal class FortuneFishingGameLogic : BaseGreenSlotGame
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
                return 3;
            }
        }

        protected override string VersionCheckString
        {
            get
            {
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ919ÿ1.10.54.0ÿNGI_GS2_Slot_Serverÿ13131ÿ1.0.3ÿFortune Fishing 95%ÿ1.102.0_2024-02-26_095937ÿ1.88.1";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,10,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Fortune Fishing 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,LC#KQIVRVCJAKBQCPPJAKBCTAKZLTAK#BTAKVGVQTLAJBQPPKIJTQLIJOBQJTIAÿ:r,0,LT#QKLVRVACTJKLAPPCTJQLACTZJBAC#TJLAVYVKQJBQKIPPJQKIBQKAGBTKQIÿ:r,0,LQ#JITVRVKQJITKQPPJABKQJALZKQCJ#BKQCVYVABJTCABPPLTCJALTKOABQTIÿ:r,0,QT#JIQVRVTBAJIQTPPBAJKLTBAZJKLB#CTKLVYVQCJTLQCPPKAJCKAQTGAKLJIÿ:r,0,JK#TBJVRVKICTBJQPPICAJBKQAZTLQA#TBQAVGVJLQATBQPPAKLCTJKLOTKACIÿ:r,1,LC#KQBVRVCJLKQBCPPLAKBCTIKZBTAL#BTAKVGVCTLAJBQPPLIJBQLIJOBQCJIAÿ:r,1,LT#QBLVRVACTJBLAPPCTJBLACTZJBAC#LJBQVYVKTJBLKIPPCQKIBQKLGBQKAIÿ:r,1,LQ#JITVRVKQJITKLPPJABLQJCBZKQBL#BKQCVYVABLTCABPPLJCABLTKOCBATIÿ:r,1,QT#LIQVRVTBALIQTPPBAJCLTBAZJQLB#CJKLVYVBCJKLBCPPKABJKLQTGBAKCIÿ:r,1,JL#TBJVRVKICTBJKPPICTBJLQAZTBQL#TBQAVGVLCQALBQPPAKLCBJKLOCBKAIÿ:r,2,TKJVRVBTIQCJBPPLKQBJALKZQCJAB#KICJVGVALKICJAPPLKITQCLKOBTQACTQIBTQAL#CJTLABWÿ:r,2,KBIVRVTLIQAKCPPLJQTKBLJZQAKCB#JQIKVYVCLJQBKCPPLJTCIAJTGBKATLQATBIQAT#CJBACLWÿ:r,2,AIJVRVAKLBJCKPPLBJAKLTCZJABQT#LJAKVYVQTCJIKQPPTCJBKQTCOLBQTICKQAILBQ#AICTLBWÿ:r,2,CJTVRVCABQKTLPPABQLTCALZQKTCI#BQJIVYVCIBQJTIPPBLJTKLAJGTKCAJIKLACQKB#JQLBKAWÿ:r,2,AILVRVACBJLAKPPCTLAKQCTZLAKBC#TLJKVGVQCTBJKQPPCTBJKQITOBJKQITBJLQAIB#LACJIQWÿ:j,V,1,0,Zÿ:j,P,1,0,Zÿ:j,Y,1,0,Zÿ:j,G,1,0,Zÿ:j,O,1,0,Zÿ:j,R,1,0,Zÿ:j,W,1,0,TJQKABLCIZÿ:u,APPPP.PP.PP.PPPPÿ:w,Z,3,10,0ÿ:w,Z,4,50,0ÿ:w,Z,5,200,0ÿ:w,T,3,5,0ÿ:w,T,4,25,0ÿ:w,T,5,100,0ÿ:w,J,3,5,0ÿ:w,J,4,25,0ÿ:w,J,5,100,0ÿ:w,Q,3,5,0ÿ:w,Q,4,25,0ÿ:w,Q,5,100,0ÿ:w,K,3,5,0ÿ:w,K,4,25,0ÿ:w,K,5,100,0ÿ:w,A,3,5,0ÿ:w,A,4,25,0ÿ:w,A,5,100,0ÿ:w,B,3,20,0ÿ:w,B,4,100,0ÿ:w,B,5,500,0ÿ:w,L,3,20,0ÿ:w,L,4,100,0ÿ:w,L,5,500,0ÿ:w,C,3,30,0ÿ:w,C,4,150,0ÿ:w,C,5,1000,0ÿ:w,I,2,5,0ÿ:w,I,3,50,0ÿ:w,I,4,200,0ÿ:w,I,5,2500,0ÿ:wa,7,1,-2,0,0,0,0:1,2,0ÿ:wa,6,1,-5,0,0,0,0:1,2,0ÿ:wa,5,1,-10,0,0,0,0:1,2,0ÿ:wa,4,1,-15,0,0,0,0:1,2,0ÿ:wa,3,1,-20,0,0,0,0:1,2,0ÿ:wa,2,1,-25,0,0,0,0:1,2,0ÿ:wa,1,1,-50,0,0,0,0:1,2,0ÿ:wa,W,2,5,0,0,0,0:1,2,0ÿ:wa,W,3,50,0,0,0,0:1,2,0ÿ:wa,W,4,200,0,0,0,0:1,2,0ÿ:wa,W,5,2500,0,0,0,0:1,2,0ÿ:wa,Z,3,10,0,0,0,0:1,0:1:2,0ÿ:wa,Z,4,50,0,0,0,0:1,0:1:2,0ÿ:wa,Z,5,200,0,0,0,0:1,0:1:2,0ÿ:wa,T,3,5,0,0,0,0:1,0:1:2,0ÿ:wa,T,4,25,0,0,0,0:1,0:1:2,0ÿ:wa,T,5,100,0,0,0,0:1,0:1:2,0ÿ:wa,J,3,5,0,0,0,0:1,0:1:2,0ÿ:wa,J,4,25,0,0,0,0:1,0:1:2,0ÿ:wa,J,5,100,0,0,0,0:1,0:1:2,0ÿ:wa,Q,3,5,0,0,0,0:1,0:1:2,0ÿ:wa,Q,4,25,0,0,0,0:1,0:1:2,0ÿ:wa,Q,5,100,0,0,0,0:1,0:1:2,0ÿ:wa,K,3,5,0,0,0,0:1,0:1:2,0ÿ:wa,K,4,25,0,0,0,0:1,0:1:2,0ÿ:wa,K,5,100,0,0,0,0:1,0:1:2,0ÿ:wa,A,3,5,0,0,0,0:1,0:1:2,0ÿ:wa,A,4,25,0,0,0,0:1,0:1:2,0ÿ:wa,A,5,100,0,0,0,0:1,0:1:2,0ÿ:wa,B,3,20,0,0,0,0:1,0:1:2,0ÿ:wa,B,4,100,0,0,0,0:1,0:1:2,0ÿ:wa,B,5,500,0,0,0,0:1,0:1:2,0ÿ:wa,L,3,20,0,0,0,0:1,0:1:2,0ÿ:wa,L,4,100,0,0,0,0:1,0:1:2,0ÿ:wa,L,5,500,0,0,0,0:1,0:1:2,0ÿ:wa,C,3,30,0,0,0,0:1,0:1:2,0ÿ:wa,C,4,150,0,0,0,0:1,0:1:2,0ÿ:wa,C,5,1000,0,0,0,0:1,0:1:2,0ÿ:wa,I,2,5,0,0,0,0:1,0:1:2,0ÿ:wa,I,3,50,0,0,0,0:1,0:1:2,0ÿ:wa,I,4,200,0,0,0,0:1,0:1:2,0ÿ:wa,I,5,2500,0,0,0,0:1,0:1:2,0ÿ:s,0ÿ:i,5ÿ:i,10ÿ:m,5,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,1,-1ÿb,0,0,0ÿs,1ÿr,1,5,41,9,15,22,27ÿx,5,0,0,0,0,0ÿrw,0";
            }
        }
        #endregion

        public FortuneFishingGameLogic()
        {
            _gameID = GAMEID.FortuneFishing;
            GameName = "FortuneFishing";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,10,3000,10,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,5ÿ:i,10ÿ:m,5,10ÿ:b,16,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,10.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,5,41,9,15,22,27ÿx,5,0,0,0,0,0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
