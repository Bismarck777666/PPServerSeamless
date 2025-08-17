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
    internal class RisingJokerGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ11619ÿ1.10.54.0ÿEurocoinInteractiveSharedJackpotServerÿ12971ÿ1.0.0ÿRising Joker 95%ÿ1.118.0_2025-03-28_090804ÿ1.68.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Rising Joker 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,^^-__ÿ:l,__-^^ÿ:l,-^^^-ÿ:l,-___-ÿ:l,_---^ÿ:r,0,7CCLLLMMMCCCWWWOOOGGGSLLLBBBCCCMMMRROOOLLL7MGGGMMMWWWGGGROOOSCC7CCLLLMMMCCCWWWOOOGGGSLLLBBBCCCMMMRROOOLLL7MGGGMMMWWWGGGROOOSCC7CCLLLMMMCCCWWWOOOGGGSLLLBBBCCCMMMRROOOLLL7MGGGMMMWWWGGGROOOSCCÿ:r,0,7COOOSCCCGGGWWWCCCRRRBGGOOOWWWSLLLMOOOXCCC7COOOSCCCGGGWWWCCCRRRBGGOOOWWWSLLLMOOOXCCC7COOOSCCCGGGWWWCCCRRRBGGOOOWWWSLLLMOOOXCCCÿ:r,0,LLLBB7LLWOOOMMCCCXLLRRGBBLLLMMMWGSWOOOGGCCCLLLBB7LLWOOOMMCCCXLLRRGBBLLLMMMWGSWOOOGGCCCLLLBB7LLWOOOMMCCCXLLRRGBBLLLMMMWGSWOOOGGCCCÿ:r,0,7OOORGGGOOO7CCCMMMRRWWG7CCCCSOOOXLLLBBBGGGCCC7OOORGGGOOO7CCCMMMRRWWGCCCCSOOOXLLLBBBGGGCCC7OOORGGGOOO7CCCMMMRRWWGCCCCSOOOXLLLBBBGGGCCCÿ:r,0,7MLLLCCCMMMBBBOOGGGRRRCCCMMMWWWLLLSOOOWWWXGGGCCCCBBBLLLOOO7MLLLWWWCCCMMMBBBOOGGGRRRLLLCCCMMMWWWLLLSOOOWWWXGGGCCCCBBBLLLOOO7MLLLCCCMMMBBBOOGGGRRRCCCMMMWWWLLLSOOOWWWXGGGCCCCBBBLLLOOOÿ:r,1,7CCLLLMMMCCCWWWOOOGGGSLLLBBBCCCMMMRROOOLLL7MGGGMMMWWWGGGROOOSCC7CCLLLMMMCCCWWWOOOGGGSLLLBBBCCCMMMRROOOLLL7MGGGMMMWWWGGGROOOSCC7CCLLLMMMCCCWWWOOOGGGSLLLBBBCCCMMMRROOOLLL7MGGGMMMWWWGGGROOOSCCÿ:r,1,7COOOSCCCGGGWWWCCCRRRBGGOOOWWWSLLLMOOOXCCC7COOOSCCCGGGWWWCCCRRRBGGOOOWWWSLLLMOOOXCCC7COOOSCCCGGGWWWCCCRRRBGGOOOWWWSLLLMOOOXCCCÿ:r,1,LLLBB7LLWOOOLLMMCCCXLLRRGBBLLLMMMWGSLLLBB7LLWOOOMMCCCXLLRRGBBLLLMMMWGSLLLBB7LLWOOOMMCCCXLLRRGBBLLLMMMWGSÿ:r,1,7OOORGGGOOO7CCCMMMRRWWGCCCCSOOOXLLLSBBBGGGCCC7OOORGGGOOOS7CCCMMMRRWWGCCCCSOOOXLLLSBBBGGGCCC7OOORGGGOOO7SCCCMMMRRWWGCCCCSOOOXLLLSBBBGGGCCCÿ:r,1,7MLLLXCCCMMMBBBOOGGGRRRCCCMMMWWWLLLSOOOWWWXGGGCCCCBBBLLLSOOO7MLLLXCCCMMMBBBOOGGGRRRCCCMMMWWWLLLSOOOWWWXGGGCCCCBBBLLLSOOO7MLLLXCCCMMMBBBOOGGGRRRCCCMMMWWWLLLSOOOWWWXGGGCCCCBBBLLLSOOOÿ:j,X,1,0,7RBWMGOLCÿ:u,APPPP.PP.PP.PPPPÿ:w,#,103,-5000,1:Grandÿ:w,#,102,-1250,1:Majorÿ:w,#,101,-200,1:Minorÿ:w,#,100,-50,1:Miniÿ:w,S,3,50,0ÿ:w,S,4,500,0ÿ:w,S,5,2500,0ÿ:w,7,3,100,0ÿ:w,7,4,600,0ÿ:w,7,5,5000,0ÿ:w,R,3,60,0ÿ:w,R,4,400,0ÿ:w,R,5,2000,0ÿ:w,B,3,40,0ÿ:w,B,4,200,0ÿ:w,B,5,800,0ÿ:w,W,3,20,0ÿ:w,W,4,100,0ÿ:w,W,5,400,0ÿ:w,M,3,10,0ÿ:w,M,4,60,0ÿ:w,M,5,240,0ÿ:w,G,3,10,0ÿ:w,G,4,60,0ÿ:w,G,5,240,0ÿ:w,O,3,5,0ÿ:w,O,4,30,0ÿ:w,O,5,120,0ÿ:w,L,3,5,0ÿ:w,L,4,30,0ÿ:w,L,5,120,0ÿ:w,C,3,5,0ÿ:w,C,4,30,0ÿ:w,C,5,120,0ÿ:wa,#,103,-5000,1:Grand,0,0,0:1,-1,0ÿ:wa,#,102,-1250,1:Major,0,0,0:1,-1,0ÿ:wa,#,101,-200,1:Minor,0,0,0:1,-1,0ÿ:wa,#,100,-50,1:Mini,0,0,0:1,-1,0ÿ:wa,S,3,50,0,0,0,0:1,-1,0ÿ:wa,S,4,500,0,0,0,0:1,-1,0ÿ:wa,S,5,2500,0,0,0,0:1,-1,0ÿ:wa,7,3,100,0,0,0,0:1,-1,0ÿ:wa,7,4,600,0,0,0,0:1,-1,0ÿ:wa,7,5,5000,0,0,0,0:1,-1,0ÿ:wa,R,3,60,0,0,0,0:1,-1,0ÿ:wa,R,4,400,0,0,0,0:1,-1,0ÿ:wa,R,5,2000,0,0,0,0:1,-1,0ÿ:wa,B,3,40,0,0,0,0:1,-1,0ÿ:wa,B,4,200,0,0,0,0:1,-1,0ÿ:wa,B,5,800,0,0,0,0:1,-1,0ÿ:wa,W,3,20,0,0,0,0:1,-1,0ÿ:wa,W,4,100,0,0,0,0:1,-1,0ÿ:wa,W,5,400,0,0,0,0:1,-1,0ÿ:wa,M,3,10,0,0,0,0:1,-1,0ÿ:wa,M,4,60,0,0,0,0:1,-1,0ÿ:wa,M,5,240,0,0,0,0:1,-1,0ÿ:wa,G,3,10,0,0,0,0:1,-1,0ÿ:wa,G,4,60,0,0,0,0:1,-1,0ÿ:wa,G,5,240,0,0,0,0:1,-1,0ÿ:wa,O,3,5,0,0,0,0:1,-1,0ÿ:wa,O,4,30,0,0,0,0:1,-1,0ÿ:wa,O,5,120,0,0,0,0:1,-1,0ÿ:wa,L,3,5,0,0,0,0:1,-1,0ÿ:wa,L,4,30,0,0,0,0:1,-1,0ÿ:wa,L,5,120,0,0,0,0:1,-1,0ÿ:wa,C,3,5,0,0,0,0:1,-1,0ÿ:wa,C,4,30,0,0,0,0:1,-1,0ÿ:wa,C,5,120,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,10ÿ:m,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,0,-1ÿb,0,0,0ÿs,1ÿr,0,5,55,34,1,19,12ÿx,1,j=-1ÿrw,0";
            }
        }
        #endregion

        public RisingJokerGameLogic()
        {
            _gameID = GAMEID.RisingJoker;
            GameName = "RisingJoker";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,5000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,10ÿ:m,10ÿ:b,18,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300,400,500ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,55,34,1,19,12ÿx,6,jc=5723:890:40:4,jsc=2854800.0:4.0919726E8:3.1160578E8:3.240431286E10,j=-1,jhi=4.573246024E10:4.135024892E10:2.1295045032E11:3.240431286E10,js=804714775:804714706:804699126:804533220,jhs=804431372:804253091:804602372:804533220ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
