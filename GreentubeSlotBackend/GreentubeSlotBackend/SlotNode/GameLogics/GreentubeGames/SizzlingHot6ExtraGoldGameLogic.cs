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
    internal class SizzlingHot6ExtraGoldGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ919ÿ1.10.54.0ÿNGI_GS2_Slot_Serverÿ3252ÿ1.0.0ÿSizzling Hot 6 extra gold 95%ÿ1.102.0_2024-02-26_095937ÿ1.88.1";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Sizzling Hot 6 extra gold 95%ÿ:v,3ÿ:l,------ÿ:l,^^^^^^ÿ:l,______ÿ:l,^-_-^^ÿ:l,_-^-__ÿ:l,^-__-^ÿ:l,_-^^-_ÿ:r,0,7MPPPG7GGSLLLCCCLLMMOMOOOP7PPPGOOO7GGSGGLLLCCCLLMMOMOOOP7PPPG7MMGGSLLLCCCLLMMOMOOOPÿ:r,0,7MLLLMM7MCCCCSPPPPOOOGOGGL7LLLMM7MCCCCSPPPPOOOGOMMGGL7LLLMM7MCCCCSPPPPOOOGOGGLÿ:r,0,7LCCCSOOO7OMMLLLPMPPPGGCGLLL7LCCCSOOOL7OMMPMPPPGGCGLLL7LCCCSOOO7OMMPMPPPGGCGLLLÿ:r,0,7OOOO7OPPPSCCCGGLLLCLCCCPPMM7OOOOL7OPPPSGGLLLCLCCCPPMM7OOOO7OPPPSGGLLLCLOCCCPPMMÿ:r,0,7PPLLLLMMOSOOOSOPPPL7CCCCCGG7PPLLLLMMOSOOOSOPPPL7CCCCCGG7PPLLLLMMOS7OOOSOPPPL7CCCCCGGÿ:r,0,1555531330666888466224244445155553144443306668886622334244445155553133036668886622426644445ÿ:r,1,9PPPG9GGSMLLLLCCCLLMMOMOOOOP9PPPG9GGSLLLLPCCCLLMMOMOOOOP9PPPG9GGOOOOSLLLLCCCLLMMOMPOOOOPÿ:r,1,9LLLMM9MCCCCCSPPPPOOOOGOGGL9LLLMM9MGGCCCCCSPPPPOOOOGOGGL9LLLMM9MCCCCC9SPPPPOOOOGOGGLÿ:r,1,9LCCCCCPPPSOOO9OMMPMLLLPPPGGCGLLL9LCCCCCSOOOL9OMMPMPPPGGCGLLL9LCCCCCSOOO9OMMPMPPPGGCGLLLÿ:r,1,9OOOO9OPPPSOOOOGGLLLCLCCCCCPPMM9OOOO9LOPPPSGGLLLCLCCCCCPPMM9OOOO9OPPPSGGLLLCLCCCCCPPMMÿ:r,1,9PPLLLLMMOSOOOSOPPPPL9CCCGG9PPLLLLMMOSOOOSOPPPPLO9CCCGG9PPLLLLMMOSOOOSOPPPPL9CCCGGÿ:r,1,9LLOOOOG9GGSLLLCCCLLMMPMPPPPO9OOOOLLG9GGSLLLCCCLL9MMPMPPPPO9OOOOG9GGSLLLCCCLLMMPMPPPPOÿ:u,APPPP.PP.PP.PPPPÿ:w,S,3,10,0ÿ:w,S,4,50,0ÿ:w,S,5,250,0ÿ:w,S,6,1000,0ÿ:w,7,3,100,0ÿ:w,7,4,1000,0ÿ:w,7,5,5000,0ÿ:w,7,6,15000,0ÿ:w,M,3,50,0ÿ:w,M,4,200,0ÿ:w,M,5,500,0ÿ:w,M,6,2000,0ÿ:w,G,3,50,0ÿ:w,G,4,200,0ÿ:w,G,5,500,0ÿ:w,G,6,2000,0ÿ:w,P,3,20,0ÿ:w,P,4,50,0ÿ:w,P,5,200,0ÿ:w,P,6,500,0ÿ:w,O,3,20,0ÿ:w,O,4,50,0ÿ:w,O,5,200,0ÿ:w,O,6,500,0ÿ:w,L,3,20,0ÿ:w,L,4,50,0ÿ:w,L,5,200,0ÿ:w,L,6,500,0ÿ:w,C,2,5,0ÿ:w,C,3,20,0ÿ:w,C,4,50,0ÿ:w,C,5,200,0ÿ:w,C,6,500,0ÿ:wa,S,3,10,0,0,0,0:1,0,0ÿ:wa,S,4,50,0,0,0,0:1,0,0ÿ:wa,S,5,250,0,0,0,0:1,0,0ÿ:wa,S,6,1000,0,0,0,0:1,0,0ÿ:wa,S,3,10,0,0,0,0:1,1,0ÿ:wa,S,4,50,0,0,0,0:1,1,0ÿ:wa,S,5,250,0,0,0,0:1,1,0ÿ:wa,S,6,1000,0,0,0,0:1,1,0ÿ:wa,7,3,100,0,0,0,0:1,0,0ÿ:wa,7,4,1000,0,0,0,0:1,0,0ÿ:wa,7,5,5000,0,0,0,0:1,0,0ÿ:wa,7,6,15000,0,0,0,0:1,0,0ÿ:wa,9,3,100,0,0,0,0:1,1,0ÿ:wa,9,4,1000,0,0,0,0:1,1,0ÿ:wa,9,5,5000,0,0,0,0:1,1,0ÿ:wa,9,6,15000,0,0,0,0:1,1,0ÿ:wa,9,3,100,0,0,1,0:1,1,0ÿ:wa,9,4,1000,0,0,1,0:1,1,0ÿ:wa,9,5,5000,0,0,1,0:1,1,0ÿ:wa,9,3,100,0,0,2,0:1,1,0ÿ:wa,9,4,1000,0,0,2,0:1,1,0ÿ:wa,9,3,100,0,0,3,0:1,1,0ÿ:wa,M,3,50,0,0,0,0:1,-1,0ÿ:wa,M,4,200,0,0,0,0:1,-1,0ÿ:wa,M,5,500,0,0,0,0:1,-1,0ÿ:wa,M,6,2000,0,0,0,0:1,-1,0ÿ:wa,M,5,500,0,0,1,0:1,-1,0ÿ:wa,M,4,200,0,0,2,0:1,-1,0ÿ:wa,M,3,50,0,0,3,0:1,-1,0ÿ:wa,G,3,50,0,0,0,0:1,-1,0ÿ:wa,G,4,200,0,0,0,0:1,-1,0ÿ:wa,G,5,500,0,0,0,0:1,-1,0ÿ:wa,G,6,2000,0,0,0,0:1,-1,0ÿ:wa,G,5,500,0,0,1,0:1,-1,0ÿ:wa,G,4,200,0,0,2,0:1,-1,0ÿ:wa,G,3,50,0,0,3,0:1,-1,0ÿ:wa,P,3,20,0,0,0,0:1,-1,0ÿ:wa,P,4,50,0,0,0,0:1,-1,0ÿ:wa,P,5,200,0,0,0,0:1,-1,0ÿ:wa,P,6,500,0,0,0,0:1,-1,0ÿ:wa,P,5,200,0,0,1,0:1,-1,0ÿ:wa,P,4,50,0,0,2,0:1,-1,0ÿ:wa,P,3,20,0,0,3,0:1,-1,0ÿ:wa,O,3,20,0,0,0,0:1,-1,0ÿ:wa,O,4,50,0,0,0,0:1,-1,0ÿ:wa,O,5,200,0,0,0,0:1,-1,0ÿ:wa,O,6,500,0,0,0,0:1,-1,0ÿ:wa,O,5,200,0,0,1,0:1,-1,0ÿ:wa,O,4,50,0,0,2,0:1,-1,0ÿ:wa,O,3,20,0,0,3,0:1,-1,0ÿ:wa,L,3,20,0,0,0,0:1,-1,0ÿ:wa,L,4,50,0,0,0,0:1,-1,0ÿ:wa,L,5,200,0,0,0,0:1,-1,0ÿ:wa,L,6,500,0,0,0,0:1,-1,0ÿ:wa,L,5,200,0,0,1,0:1,-1,0ÿ:wa,L,4,50,0,0,2,0:1,-1,0ÿ:wa,L,3,20,0,0,3,0:1,-1,0ÿ:wa,C,2,5,0,0,0,0:1,-1,0ÿ:wa,C,3,20,0,0,0,0:1,-1,0ÿ:wa,C,4,50,0,0,0,0:1,-1,0ÿ:wa,C,5,200,0,0,0,0:1,-1,0ÿ:wa,C,6,500,0,0,0,0:1,-1,0ÿ:wa,C,5,200,0,0,1,0:1,-1,0ÿ:wa,C,4,50,0,0,2,0:1,-1,0ÿ:wa,C,3,20,0,0,3,0:1,-1,0ÿ:wa,C,2,5,0,0,4,0:1,-1,0ÿ:s,0ÿ:i,5ÿ:i,7ÿ:m,5,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,1,-1ÿb,0,0,0ÿs,1ÿr,1,6,3,4,29,42,3,14ÿrw,0";
            }
        }
        #endregion

        public SizzlingHot6ExtraGoldGameLogic()
        {
            _gameID = GAMEID.SizzlingHot6ExtraGold;
            GameName = "SizzlingHot6ExtraGold";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,3000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,5ÿ:i,7ÿ:m,5,10ÿ:b,18,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,6,3,4,29,42,3,14ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
        }
        protected override void onDoInit(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {
            try
            {
                GITMessage resMessage = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_DOINIT);
                resMessage.Append("%t0ÿq0");
                resMessage.Append(VersionCheckString);
                foreach(string currencyName in SupportCurrencyList)
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
