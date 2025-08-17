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
    internal class XtraHotGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ85ÿ1.0.0ÿXtra Hot 95.6%ÿ1.113.0_2024-11-19_131508ÿ1.107.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Xtra Hot 95.6%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:r,0,7XPPPPXLLLLXCCCCXSXB7OOOO7XPPPPXLLLLXCCCCXSXB7OOOO7XPPPPXLLLLXCCCCXSXB7XOOOOÿ:r,0,7BOOOOXCCCCXSBXPPPPPXLLLL7BOOOOXCCCCXSBXPPPPPXLLLL7BOOOOXCCCCXSBXPPPPPXLLLLÿ:r,0,7CCCCXOOOOOXBXPPPPSLLLLLX7CCCCXOOOOOXBXPPPPSLLLLLX7CCCCXOOOOOXBXPPPPSLLLLLXÿ:r,0,7XBOOOOB7PPPPSBLLLLBCCCCXB7XBOOOOB7PPPPSBLLLLCCCCXB7XBOOOOB7PPPPSBXLLLLCCCCXBÿ:r,0,7BXLLLBXSBOOOBSPPPB7CCCBX7BXLLLBXSBOOOBSPPPB7CCCBX7BXLLLBXSBOOO7BSPPPB7SCCCBXÿ:u,APPPP.PP.PP.PPPPÿ:w,S,5,-50,0ÿ:w,S,4,-10,0ÿ:w,S,3,-2,0ÿ:w,7,5,5000,0ÿ:w,7,4,1000,0ÿ:w,7,3,100,0ÿ:w,B,5,1000,0ÿ:w,B,4,400,0ÿ:w,B,3,50,0ÿ:w,P,5,200,0ÿ:w,P,4,50,0ÿ:w,P,3,20,0ÿ:w,O,5,200,0ÿ:w,O,4,50,0ÿ:w,O,3,20,0ÿ:w,L,5,200,0ÿ:w,L,4,50,0ÿ:w,L,3,20,0ÿ:w,C,5,200,0ÿ:w,C,4,50,0ÿ:w,C,3,20,0ÿ:w,C,2,5,0ÿ:w,X,5,100,0ÿ:w,X,4,20,0ÿ:w,X,3,5,0ÿ:wa,S,5,-50,0,0,0,0:1,-1,0ÿ:wa,S,4,-10,0,0,0,0:1,-1,0ÿ:wa,S,3,-2,0,0,0,0:1,-1,0ÿ:wa,7,5,5000,0,0,0,0:1,-1,0ÿ:wa,7,4,1000,0,0,0,0:1,-1,0ÿ:wa,7,3,100,0,0,0,0:1,-1,0ÿ:wa,B,5,1000,0,0,0,0:1,-1,0ÿ:wa,B,4,400,0,0,0,0:1,-1,0ÿ:wa,B,3,50,0,0,0,0:1,-1,0ÿ:wa,P,5,200,0,0,0,0:1,-1,0ÿ:wa,P,4,50,0,0,0,0:1,-1,0ÿ:wa,P,3,20,0,0,0,0:1,-1,0ÿ:wa,O,5,200,0,0,0,0:1,-1,0ÿ:wa,O,4,50,0,0,0,0:1,-1,0ÿ:wa,O,3,20,0,0,0,0:1,-1,0ÿ:wa,L,5,200,0,0,0,0:1,-1,0ÿ:wa,L,4,50,0,0,0,0:1,-1,0ÿ:wa,L,3,20,0,0,0,0:1,-1,0ÿ:wa,C,5,200,0,0,0,0:1,-1,0ÿ:wa,C,4,50,0,0,0,0:1,-1,0ÿ:wa,C,3,20,0,0,0,0:1,-1,0ÿ:wa,C,2,5,0,0,0,0:1,-1,0ÿ:wa,X,5,100,0,0,0,0:1,-1,0ÿ:wa,X,4,20,0,0,0,0:1,-1,0ÿ:wa,X,3,5,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,5ÿ:m,5ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,5,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,16,19,17,16,23ÿrw,0ÿ:wee,0";
            }
        }
        #endregion

        public XtraHotGameLogic()
        {
            _gameID = GAMEID.XtraHot;
            GameName = "XtraHot";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,5,5000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,5ÿ:m,5ÿ:b,21,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500,800,1000ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,16,19,17,16,23ÿrw,0ÿ:wee,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
