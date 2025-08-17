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
    internal class ChilliElToroGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ2578ÿ1.10.54.0ÿRedlineSlotServerÿ12710ÿ1.0.1ÿChilli El Toro 95%ÿ1.119.0_2025-04-14_123855ÿ1.165.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,49800,100.0,ÿM,10,10000,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Chilli El Toro 95%ÿ:v,8ÿ:l,-----ÿ:r,0,QAAAJ3AAJ444AASKQ333TQ111QQTT31AAAJ333QQNNTJ4NNTTT2QTAAJJTTTJJ11TQQQA33QQNN33TTQQ222AAKKKTTKKQSANNÿ:r,0,AAJKTTQN4TT444NNN333TKWWJJJS4QQQWWNN444JNNNSNJK44JJK222KJ44NNJQKKKJ44AAAQ3JJ222AKKK22TTN22QQN22NNJ33NNJSJJK11NKJKÿ:r,0,NKKK2JJJNNWWKNNN44KKJJ111TNNWWNKKKQ33KJJJSTTN333JKTTT1JJJKN11AAKK444QAAJ4KQQQ3JKNNNSTTJJN44QKJ222AAKN444KQJK4Jÿ:r,0,QQT33TTQ4TQQAKKK3QQQ3NN222TQQQ444TN222TTTAAATT22QJ333TTAA22NN3SQQTWWJJTT22QQN44QJTTKKAAASTT444JJAA11KQQA4Qÿ:r,0,NJT222NNNJT2AKTT33AAA111QQTTTSNQQQ1KKKA444NTQQ4AAAJJJN3KKKTSNJJJ11KNNN22QQQJJ44AAJ333KKQQ33JTTT44N11ÿ:r,1,QAAAJ3AAJ444AASKQ333TQ111QQTT31AAAJ333QQNNTJ4NNTTT2QTAAJJTTTJJ11TQQQA33QQNN33STTQQ222AAKKKTTKKQSAÿ:r,1,AAJKTTQN4TT444NNN333TKWWJJJS4QQQWWNN444JNNNSNJK44WWK222KJ44NNJQKKKJ44AAAQ3JJ222AKKK22TTT22QQN22NNJ33NNJSJJK11NKJSKÿ:r,1,NKKK2JJJNNWWJNNN44KKJJ111TNNWWNKKKQ33KJJJSTTN333JKTTT1JJJKN11AAKK444QAAQ4KQQQ3WWNNNSTTJJN44QKJ222AAKN444KQJK4JSÿ:r,1,QQT33TTQ4TQQAKKK3QQQ3NN222TQQQ444TN222TTTAAAWW22JJ333TTAA22NN3SQQTWWJJTT22QQN44QQTTWWAAASTT444JJAA11KQQA4Qÿ:r,1,NJT222NNNJT2AKTT33AAA111QQTTTSNQQQ1KKKA444NTQQ4AAAJJJN3KKKTSNJJJ11KNNN22QQQJJ44AAJ333KKQQ33JTTT44Nÿ:j,W,1,0,1234AKQJTNÿ:u,APPPP.PP.PP.PPPPÿ:w,1,3,15,0ÿ:w,1,4,30,0ÿ:w,1,5,100,0ÿ:w,2,3,10,0ÿ:w,2,4,20,0ÿ:w,2,5,80,0ÿ:w,3,3,8,0ÿ:w,3,4,15,0ÿ:w,3,5,40,0ÿ:w,4,3,8,0ÿ:w,4,4,15,0ÿ:w,4,5,40,0ÿ:w,A,3,5,0ÿ:w,A,4,10,0ÿ:w,A,5,20,0ÿ:w,K,3,5,0ÿ:w,K,4,10,0ÿ:w,K,5,20,0ÿ:w,Q,3,3,0ÿ:w,Q,4,8,0ÿ:w,Q,5,15,0ÿ:w,J,3,3,0ÿ:w,J,4,8,0ÿ:w,J,5,15,0ÿ:w,T,3,2,0ÿ:w,T,4,8,0ÿ:w,T,5,15,0ÿ:w,N,3,2,0ÿ:w,N,4,8,0ÿ:w,N,5,15,0ÿ:w,S,5,-10,0ÿ:w,S,4,-5,0ÿ:w,S,3,-2,0ÿ:wa,1,3,15,0,0,0,0:1,0:1,0ÿ:wa,1,4,30,0,0,0,0:1,0:1,0ÿ:wa,1,5,100,0,0,0,0:1,0:1,0ÿ:wa,2,3,10,0,0,0,0:1,0:1,0ÿ:wa,2,4,20,0,0,0,0:1,0:1,0ÿ:wa,2,5,80,0,0,0,0:1,0:1,0ÿ:wa,3,3,8,0,0,0,0:1,0:1,0ÿ:wa,3,4,15,0,0,0,0:1,0:1,0ÿ:wa,3,5,40,0,0,0,0:1,0:1,0ÿ:wa,4,3,8,0,0,0,0:1,0:1,0ÿ:wa,4,4,15,0,0,0,0:1,0:1,0ÿ:wa,4,5,40,0,0,0,0:1,0:1,0ÿ:wa,A,3,5,0,0,0,0:1,0:1,0ÿ:wa,A,4,10,0,0,0,0:1,0:1,0ÿ:wa,A,5,20,0,0,0,0:1,0:1,0ÿ:wa,K,3,5,0,0,0,0:1,0:1,0ÿ:wa,K,4,10,0,0,0,0:1,0:1,0ÿ:wa,K,5,20,0,0,0,0:1,0:1,0ÿ:wa,Q,3,3,0,0,0,0:1,0:1,0ÿ:wa,Q,4,8,0,0,0,0:1,0:1,0ÿ:wa,Q,5,15,0,0,0,0:1,0:1,0ÿ:wa,J,3,3,0,0,0,0:1,0:1,0ÿ:wa,J,4,8,0,0,0,0:1,0:1,0ÿ:wa,J,5,15,0,0,0,0:1,0:1,0ÿ:wa,T,3,2,0,0,0,0:1,0:1,0ÿ:wa,T,4,8,0,0,0,0:1,0:1,0ÿ:wa,T,5,15,0,0,0,0:1,0:1,0ÿ:wa,N,3,2,0,0,0,0:1,0:1,0ÿ:wa,N,4,8,0,0,0,0:1,0:1,0ÿ:wa,N,5,15,0,0,0,0:1,0:1,0ÿ:wa,S,5,-10,0,0,0,0:1,0,0ÿ:wa,S,4,-5,0,0,0,0:1,0,0ÿ:wa,S,3,-2,0,0,0,0:1,0,0ÿ:wa,S,3,-2,0,0,0,0:1,1,0ÿ:wa,S,4,-5,0,0,0,0:1,1,0ÿ:wa,S,5,-10,0,0,0,0:1,1,0ÿ:s,0ÿ:i,1ÿ:m,50ÿ:b,8,1,2,3,4,5,8,10,15ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,50.0ÿbs,0,1,0ÿ:k,nullÿe,50,0,0,0ÿb,1000,1000,0ÿs,1ÿr,0,5,25,55,73,8,90ÿx,4,2:4:8:6:4,2:5:3,13J,1ÿrw,0";
            }
        }
        #endregion

        public ChilliElToroGameLogic()
        {
            _gameID = GAMEID.ChilliElToro;
            GameName = "ChilliElToro";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,50,750,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:m,50ÿ:b,8,1,2,3,4,5,8,10,15ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,50.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,25,55,73,8,90ÿx,4,2:4:8:6:4,2:5:3,13J,1ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
