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
    internal class DiamondMysteryFruitiliciousDeluxeGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ11619ÿ1.10.54.0ÿEurocoinInteractiveSharedJackpotServerÿ16392ÿ1.0.0ÿFruitilicious deluxe 95%ÿ1.118.0_2025-03-28_090804ÿ1.68.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Fruitilicious deluxe 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:r,0,LLLCCCCGG7PPGPPPMGOO7OOOM7MMPPPCCLLCCCCLOMLLLGG7PPLGPPPGOO7PPOOOM7MMCCLLCCCCLGGOMLLLGG7PPGMMPPPGOO7OOOM7MMCCLLCCCCLOMÿ:r,0,MMLLPMPMMPLL7LLLGG7GCCC7OOOCPPPOCLOMMPMPMMLL7LLLGG7GCCC7OOOCMPPPOCLOMMPMPMMLL7LLLGG7GCCC7MOOOCPPPOCMLOÿ:r,0,7OPGGOGOOOMMCMLLL7LCCCOPPPGLLLMPCLO7PGGOGOOOMMCMLLL7LCCCOPPPGMPCLO7PGGOGOOOMMLLLCMLLL7LCCCOPPPGMPCLOÿ:r,0,OOOCMMLLLCL7CCCOOGG7PPP7POLPGCLOOOOCMMLLLCLCCCPPPOOGG7PPP7LPOLPGCLOOOOCMMLLLCPPPLCCCOOGG7LPPP7POLPLLLGCLOÿ:r,0,CPOOOL7CCCMM7PPPOOOLLLGPC7PPPLLGCLOCPOOOL7CCCMM7OOOLLLGPCPPPLLGCLOCPOOOL7CCCOOOMM7OOOLLLGPCPPPLLGCLOÿ:u,APPPP.PP.PP.PPPPÿ:w,#,103,-5000,1:Grandÿ:w,#,102,-1250,1:Majorÿ:w,#,101,-200,1:Minorÿ:w,#,100,-50,1:Miniÿ:w,7,5,5000,0ÿ:w,7,4,500,0ÿ:w,7,3,100,0ÿ:w,M,5,500,0ÿ:w,M,4,100,0ÿ:w,M,3,25,0ÿ:w,G,5,500,0ÿ:w,G,4,100,0ÿ:w,G,3,25,0ÿ:w,P,5,125,0ÿ:w,P,4,30,0ÿ:w,P,3,10,0ÿ:w,O,5,125,0ÿ:w,O,4,30,0ÿ:w,O,3,10,0ÿ:w,L,5,125,0ÿ:w,L,4,30,0ÿ:w,L,3,10,0ÿ:w,C,5,125,0ÿ:w,C,4,30,0ÿ:w,C,3,10,0ÿ:wa,#,103,-5000,1:Grand,0,0,0:1,-1,0ÿ:wa,#,102,-1250,1:Major,0,0,0:1,-1,0ÿ:wa,#,101,-200,1:Minor,0,0,0:1,-1,0ÿ:wa,#,100,-50,1:Mini,0,0,0:1,-1,0ÿ:wa,7,5,5000,0,0,0,0:1,-1,0ÿ:wa,7,4,500,0,0,0,0:1,-1,0ÿ:wa,7,3,100,0,0,0,0:1,-1,0ÿ:wa,7,4,500,0,0,1,0:1,-1,0ÿ:wa,7,3,100,0,0,2,0:1,-1,0ÿ:wa,M,5,500,0,0,0,0:1,-1,0ÿ:wa,M,4,100,0,0,0,0:1,-1,0ÿ:wa,M,3,25,0,0,0,0:1,-1,0ÿ:wa,M,4,100,0,0,1,0:1,-1,0ÿ:wa,M,3,25,0,0,2,0:1,-1,0ÿ:wa,G,5,500,0,0,0,0:1,-1,0ÿ:wa,G,4,100,0,0,0,0:1,-1,0ÿ:wa,G,3,25,0,0,0,0:1,-1,0ÿ:wa,G,4,100,0,0,1,0:1,-1,0ÿ:wa,G,3,25,0,0,2,0:1,-1,0ÿ:wa,P,5,125,0,0,0,0:1,-1,0ÿ:wa,P,4,30,0,0,0,0:1,-1,0ÿ:wa,P,3,10,0,0,0,0:1,-1,0ÿ:wa,P,4,30,0,0,1,0:1,-1,0ÿ:wa,P,3,10,0,0,2,0:1,-1,0ÿ:wa,O,5,125,0,0,0,0:1,-1,0ÿ:wa,O,4,30,0,0,0,0:1,-1,0ÿ:wa,O,3,10,0,0,0,0:1,-1,0ÿ:wa,O,4,30,0,0,1,0:1,-1,0ÿ:wa,O,3,10,0,0,2,0:1,-1,0ÿ:wa,L,5,125,0,0,0,0:1,-1,0ÿ:wa,L,4,30,0,0,0,0:1,-1,0ÿ:wa,L,3,10,0,0,0,0:1,-1,0ÿ:wa,L,4,30,0,0,1,0:1,-1,0ÿ:wa,L,3,10,0,0,2,0:1,-1,0ÿ:wa,C,5,125,0,0,0,0:1,-1,0ÿ:wa,C,4,30,0,0,0,0:1,-1,0ÿ:wa,C,3,10,0,0,0,0:1,-1,0ÿ:wa,C,4,30,0,0,1,0:1,-1,0ÿ:wa,C,3,10,0,0,2,0:1,-1,0ÿ:s,0ÿ:i,5ÿ:m,5ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,5,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,3,6,6,18,19ÿx,1,j=-1ÿrw,0";
            }
        }
        #endregion

        public DiamondMysteryFruitiliciousDeluxeGameLogic()
        {
            _gameID = GAMEID.DiamondMysteryFruitiliciousDeluxe;
            GameName = "DiamondMysteryFruitiliciousDeluxe";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,5000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,5ÿ:m,5ÿ:b,18,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500,800,1000ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,3,6,6,18,19ÿx,6,jc=12205:1771:68:5,jsc=4.70772E7:5.4106272E8:3.4265028E8:7.71486852E9,j=-1,jhi=9.06639334E9:3.48695021E10:7.469872472E10:5.970062452E10,js=804305751:804307139:804282110:804287062,jhs=804116664:804266327:804123692:804144087ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
