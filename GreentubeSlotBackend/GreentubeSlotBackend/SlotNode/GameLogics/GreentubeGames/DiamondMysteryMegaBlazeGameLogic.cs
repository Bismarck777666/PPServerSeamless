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
    internal class DiamondMysteryMegaBlazeGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ11619ÿ1.10.54.0ÿEurocoinInteractiveSharedJackpotServerÿ15573ÿ1.0.1ÿMega Blaze 95%ÿ1.118.0_2025-03-28_090804ÿ1.68.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,5,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Mega Blaze 95%ÿ:v,3ÿ:l,---ÿ:l,^^^ÿ:l,___ÿ:l,_-^ÿ:l,^-_ÿ:r,0,SCCXXCBPPXXLLPPLLBLLBOO7OOCSCCXXCBPPXXPPLLBLLBOO7OOCSCCXXCBPPXXPPLLBLLBOO7OOCÿ:r,0,SLLXXLLBLLOBOPPBCCOO7CCPPSXXPPXSXXLLBPPLLOBOBCCOOXX7CCPPSXXLLPPXSXXLLBLLOBPPOBCCOO7CCPPSXXPPXÿ:r,0,SCCXXCC7PPXXPPLLOOLLBBOOCSCCXXCCOO7PPXXPPLLOOLLBBOOCSCCXXCC7PPXXPPLLOOLLBBXXOOCÿ:r,1,SCCCCXXOOOOXXPPPPBB7LLLCCSCCCCXXOOOOXXPPPPBB7LLLCCSCCCCXXOOOOXXPPPPBB7LLLCCÿ:r,1,SXBXPPPPPBLLLLL7CCCBXXOOOOSXBXPPPPPBLLLLL7CCCXXOOOOSXBXPPPPPBLLLLL7CCCXXOOOOÿ:r,1,SCCCCXX7OOOOXXPPPPBBCCCCLLLLCSCCCCXX7OOOOXXPPPPBBLLLLCSCCCCXX7OOOOXXPPPPBBLLLLCÿ:u,APPPP.PP.PP.PPPPÿ:w,#,103,-5000,1:Grandÿ:w,#,102,-1250,1:Majorÿ:w,#,101,-200,1:Minorÿ:w,#,100,-50,1:Miniÿ:w,7,3,750,0ÿ:w,S,3,200,0ÿ:w,B,3,60,0ÿ:w,P,3,40,0ÿ:w,O,3,40,0ÿ:w,L,3,40,0ÿ:w,C,3,40,0ÿ:w,X,3,5,0ÿ:wa,#,103,-5000,1:Grand,0,0,0:1,-1,0ÿ:wa,#,102,-1250,1:Major,0,0,0:1,-1,0ÿ:wa,#,101,-200,1:Minor,0,0,0:1,-1,0ÿ:wa,#,100,-50,1:Mini,0,0,0:1,-1,0ÿ:wa,7,3,750,0,0,0,0:1,-1,0ÿ:wa,S,3,200,0,0,0,0:1,-1,0ÿ:wa,B,3,60,0,0,0,0:1,-1,0ÿ:wa,P,3,40,0,0,0,0:1,-1,0ÿ:wa,O,3,40,0,0,0,0:1,-1,0ÿ:wa,L,3,40,0,0,0,0:1,-1,0ÿ:wa,C,3,40,0,0,0,0:1,-1,0ÿ:wa,X,3,5,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,5ÿ:m,5ÿ:a,0,0ÿ:g,999,300,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,5,0,0,-1ÿb,0,0,0ÿs,1ÿr,1,3,15,11,14ÿx,1,j=-1ÿrw,0";
            }
        }
        #endregion

        public DiamondMysteryMegaBlazeGameLogic()
        {
            _gameID = GAMEID.DiamondMysteryMegaBlaze;
            GameName = "DiamondMysteryMegaBlaze";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,5,5000,5,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,5ÿ:m,5ÿ:b,21,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500,800,1000ÿ:a,0,0ÿ:g,999,300,-1,false,0,falseÿ:es,0,0ÿ:mbnr,5.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,3,15,11,14ÿx,6,jc=2372:357:19:0,jsc=255.0:1.10282125E8:2.35983215E8:0.0,j=-1,jhi=1.975936585E9:9.795010345E9:4.853394134E10:0.0,js=804978291:804977075:804977646:0,jhs=804953216:804950696:804625039:0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
