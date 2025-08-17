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
    internal class SizzlingHotDeluxeGameLogic : BaseGreenSlotGame
    {
        #region
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ665ÿ1.0.0ÿSizzling Hot Deluxe 95%ÿ1.91.0_2023-04-14_123242ÿ1.84.0";
            }
        }
        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,993900,100.0,â\u0082¬ÿM,10,10000,1,0,0ÿI00ÿH,0,0,0,0,0,0ÿV25ÿ:n,Sizzling Hot Deluxe 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:r,0,7MPPPG7GGSLLLCCCLLMMOMOOOP7PPPGOOO7GGSGGLLLCCCLLMMOMOOOP7PPPG7MMGGSLLLCCCLLMMOMOOOPÿ:r,0,7MLLLMM7MCCCCSPPPPOOOGOGGL7LLLMM7MCCCCSPPPPOOOGOMMGGL7LLLMM7MCCCCSPPPPOOOGOGGLÿ:r,0,7LCCCSOOO7OMMLLLPMPPPGGCGLLL7LCCCSOOOL7OMMPMPPPGGCGLLL7LCCCSOOO7OMMPMPPPGGCGLLLÿ:r,0,7OOOO7OPPPSCCCGGLLLCLCCCPPMM7OOOOL7OPPPSGGLLLCLCCCPPMM7OOOO7OPPPSGGLLLCLOCCCPPMMÿ:r,0,7PPLLLLMMOSOOOSOPPPL7CCCCCGG7PPLLLLMMOSOOOSOPPPL7CCCCCGG7PPLLLLMMOS7OOOSOPPPL7CCCCCGGÿ:u,APPPP.PP.PP.PPPPÿ:w,S,5,-50,0ÿ:w,S,4,-10,0ÿ:w,S,3,-2,0ÿ:w,7,5,5000,0ÿ:w,7,4,1000,0ÿ:w,7,3,100,0ÿ:w,M,5,500,0ÿ:w,M,4,200,0ÿ:w,M,3,50,0ÿ:w,G,5,500,0ÿ:w,G,4,200,0ÿ:w,G,3,50,0ÿ:w,P,5,200,0ÿ:w,P,4,50,0ÿ:w,P,3,20,0ÿ:w,O,5,200,0ÿ:w,O,4,50,0ÿ:w,O,3,20,0ÿ:w,L,5,200,0ÿ:w,L,4,50,0ÿ:w,L,3,20,0ÿ:w,C,5,200,0ÿ:w,C,4,50,0ÿ:w,C,3,20,0ÿ:w,C,2,5,0ÿ:wa,S,5,-50,0,0,0,0:1,-1,0ÿ:wa,S,4,-10,0,0,0,0:1,-1,0ÿ:wa,S,3,-2,0,0,0,0:1,-1,0ÿ:wa,7,5,5000,0,0,0,0:1,-1,0ÿ:wa,7,4,1000,0,0,0,0:1,-1,0ÿ:wa,7,3,100,0,0,0,0:1,-1,0ÿ:wa,M,5,500,0,0,0,0:1,-1,0ÿ:wa,M,4,200,0,0,0,0:1,-1,0ÿ:wa,M,3,50,0,0,0,0:1,-1,0ÿ:wa,G,5,500,0,0,0,0:1,-1,0ÿ:wa,G,4,200,0,0,0,0:1,-1,0ÿ:wa,G,3,50,0,0,0,0:1,-1,0ÿ:wa,P,5,200,0,0,0,0:1,-1,0ÿ:wa,P,4,50,0,0,0,0:1,-1,0ÿ:wa,P,3,20,0,0,0,0:1,-1,0ÿ:wa,O,5,200,0,0,0,0:1,-1,0ÿ:wa,O,4,50,0,0,0,0:1,-1,0ÿ:wa,O,3,20,0,0,0,0:1,-1,0ÿ:wa,L,5,200,0,0,0,0:1,-1,0ÿ:wa,L,4,50,0,0,0,0:1,-1,0ÿ:wa,L,3,20,0,0,0,0:1,-1,0ÿ:wa,C,5,200,0,0,0,0:1,-1,0ÿ:wa,C,4,50,0,0,0,0:1,-1,0ÿ:wa,C,3,20,0,0,0,0:1,-1,0ÿ:wa,C,2,5,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,5ÿ:m,5ÿ:b,16,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿbs,0,1,0ÿ:k,nullÿe,5,0,0,0ÿb,1000,1000,0ÿ:x,1600ÿs,1ÿh,Cÿr,0,5,4,17,13,11,7ÿrw,0";
            }
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,â\u0082¬,0,EURÿT,0,0,0ÿRÿM,5,10000,1,0,1000000ÿI00ÿH,0,0,0,0,0,0ÿXÿY,10,10ÿV25ÿ:i,5ÿ:m,5ÿ:b,16,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿh,Cÿr,0,5,4,17,13,11,7ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
        }
        #endregion

        public SizzlingHotDeluxeGameLogic()
        {
            _gameID = GAMEID.SizzlingHotDeluxe;
            GameName = "SizzlingHotDeluxe";
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
