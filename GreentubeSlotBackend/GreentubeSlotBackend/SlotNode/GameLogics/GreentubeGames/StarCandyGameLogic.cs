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
    internal class StarCandyGameLogic : BaseGreenSlotGame
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ898ÿ1.10.54.0ÿGTSKSlotServerÿ14582ÿ1.0.0ÿStar Candy 95%ÿ1.120.0_2025-05-09_075230ÿ1.313.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,57800,100.0,ÿM,10,10000,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Star Candy 95%ÿ:v,4ÿ:l,^^^^^ÿ:l,-----ÿ:l,_____ÿ:l,VVVVVÿ:l,^-_-^ÿ:l,-_V_-ÿ:l,_-^-_ÿ:l,V_-_Vÿ:l,^^^^-ÿ:l,----^ÿ:l,____Vÿ:l,VVVV_ÿ:l,^----ÿ:l,----_ÿ:l,____-ÿ:l,V____ÿ:l,-^^^^ÿ:l,-____ÿ:l,_----ÿ:l,_VVVVÿ:l,^^^-_ÿ:l,---_Vÿ:l,___-^ÿ:l,VVV_-ÿ:l,^-___ÿ:l,-_VVVÿ:l,_-^^^ÿ:l,V_---ÿ:l,^^-^^ÿ:l,--^--ÿ:l,__V__ÿ:l,VV_VVÿ:l,^---^ÿ:l,--_--ÿ:l,__-__ÿ:l,V___Vÿ:l,-^^^-ÿ:l,-___-ÿ:l,_---_ÿ:l,_VVV_ÿ:l,-^-^-ÿ:l,_-_-_ÿ:l,V_V_Vÿ:l,^-^-^ÿ:l,-_-_-ÿ:l,_V_V_ÿ:l,-^-_-ÿ:l,_-_V_ÿ:l,-_-^-ÿ:l,_V_-_ÿ:r,0,BBBBPPPPTTTTDDDDJJJJBBBBOOLLLLGGCCCCMOOOOOO#GGGGPPMMMMCCCC77777OOLLLLBBBBPPPPTTTTDDDDJJJJBBBBOOLLLLGGCCCCMOOOOOO#GGGGPPMMMMCCCC77777LLLLBBBBPPPPTTTTDDDDJJJJBBBBOOLLLLGGCCCCMOOOOOO#GGGGPPMMMMCCCC77777LLLLBBBBPPPPTTTTDDDDJJJJBBBBOOLLLLGGCCCCMOOOOOO#GGGGPPMMMMCCCC77777LLLLÿ:r,0,BBBBGGGGJJJJOOOOLLLLGGBBBBCCCCMMMMMLLLLOO7777PPDDDDDTCCCC#PPPP7TTTTBBBBGGGGJJJJOOOOLLLLGGBBBBCCCCMMMMMLLLLOO7777PPDDDDDTCCCC#PPPP7TTTTBBBBGGGGJJJJOOOOLLLLGGBBBBPPCCCCMMMMMLLLLOO7777PPDDDDDTCCCC#PPPP7TTTTBBBBGGGGJJJJOOOOLLLLGGBBBBCCCCMMMMMLLLLOO7777PPDDDDDTCCCC#PPPP7TTTTÿ:r,0,BBBBDDDD7777TTTTTGGPPCCCCOOOO#BBBBLLLLJJMMMMMGGGGOOODDLLLL7PPPP7777CCCCBBBBDDDDTTTTTGGPPCCCCOOOO#BBBBLLLLJJMMMMMGGGGOOODDLLLL7PPPP7777CCCCBBBBDDDDTTTTTGGPPCCCCOOOO#BBBBLLLLJJMMMMMGGGGOOODDLLLL7PPPP7777CCCCBBBBDDDDTTTTTGGPPCCCCOOOO#BBBBLLLLJJMMMMMGGGGOOODDLLLL7PPPP7777CCCCÿ:r,0,CCCCMMMMTBBBBD7777#PPPPGGTTTTJDDDD7OOOCCCCBBBBLLLGGGGJLLLLMPPOOOOCCCCMMMMTBBBBD7777#PPPPGGTTTTJDDDD7OOOCCCCBBBBLLLGGGGJLLLLMPPOOOOCCCCMMMMTBBBBD7777#PPPPGGTTTTJDDDD7OOOCCCCBBBBLLLGGGGJLLLLMPPOOOOCCCCMMMMTBBBBDLLL7777#PPPPGGTTTTJDDDD7OOOCCCCBBBBLLLGGGGJLLLLMPPOOOOÿ:r,0,OOO77LLTMMMM7GGGGJJOOOOMTTCCCC#BBBBCCCC77PPDDDDDTTBBBBPPPPGGLLLLLOOO77LLTMMMM7GGGGJJOOOOMTT77CCCC#BBBBCCCC77PPDDDDDTTBBBBPPPPGGLLLLLOOO77LLTMMMM7PPPPGGGGJJOOOOMTTCCCC#BBBBCCCC77PPDDDDDTTBBBBPPPPGGLLLLLOOO77LLTMMMM7GGGGJJOOOOMTTCCCC#BBBBCCCC77PPDDDDDTTBBBBPPPPGGLLLLLÿ:r,1,LOB#CPP777DCBOLJGMMLOJDTTGGJ#CLBOLBGGMMJCPP#DDOLCBJGCOBLM#77PPOTTBLCBCLOB#CPP777DCBOLJGMMLOJDTTGG#CLBOLBGGMMJCPP#DDOLCBJGCOBLM#77PPOTTBLCBCLOB#CPP777DCBOLJGMMLOJBDTTGG#CLBO77LBGGMMJCPP#DDOLCBJGCOBLM#D77PPOTTBLCBCLOB#CPP777DCBOLJGMMLOJDTTCGG#CLBOLBGGMMJCPP#DDOLCBJGCOBLM#77BPPOTTBLCBCÿ:r,1,TLCBM#MBCLPJGODD77BLCOTTMGGP#OBCLMTTDJBLCOPP#BCL777DDMJGGOB#LCOOLBJCPPGTLCBM#MBCLPJGODD77BLCOTTMGGP#OBCLMTTDJBLCOPP#BCL777DDMBJGGOB#LCOOLBJCPPGTLCBM#MBCLPJGODD77BLCOTTMGGP#OBCLMTTDJBLCOPP#BCL777DDMJGGOB#LCOOLBJCPPGTLCBM#MBCLPJGODD77MBLCOTTMGGP#OBCLMTTDJBLCOPP#BCLM777DDMJGGOB#LCOOLBJCPPGÿ:r,1,JBC77#MMBCOPPBTTBCLODDGG#CLO777MBLCJLDDGG#OLBCTTLJPPOCBBJMML#PPGGOCLJLODDJBC77#MMBCOPPBTTBCLODDGG#CLO777MBLCJDDGG#OLBCTTLJPPOCBBJMM#PPGGOCLJLBODDJBC77#MMBCOPPBTTBCLODDGG#CLO777MBLCJDDGG#OLBCTTLJPPOCBBLJMM#PPGGOCLJLODDJBC77#MMBCOPPBTTBCLODDGG#CLO777MBLCJDDGG#OLBCTTLJPPOCBBJMM#PPGGOCLJLODDÿ:r,1,OBCLOPP#GG777LMMDDJBBBCLOTT#CBLOGJPCLOMDCPLO#CB77DMJPGGLOCBTT#MLBCJODGPOBCLOPP#GG777MMDDJBBBCLOTT#CBLOGJPCLOMDCPLO#CB77DMJPGGLOCBTT#MLBCJODGPOBCLOPP#GG777MMDDJBBBCLOTT#CBLOGJPCLOMDCPLO#CB77DMJPGGLOCBTT#MLBCJODGPOBCLOPP#GG777MMDDJBBBCLOTT#CBLOGJPCLOMDCPLO#CB77DMJPGGLOCBTT#MLBCJODGPÿ:r,1,CBOTTP#77DBCLOPJGGMMCBLOP#TTGCPBOLDDMJBCPGG#LBC777JDDMPOCPG#TTBCJMOLLBCBOTTP#77DBCLOPJGGMMCBLOP#TTGCBOLDDMJBCPGGTT#LBC777JDDMPOCPG#TTBCJMOLLBCBOTTP#77DBCLOPJGGMMCBLOP#TTGCBOLDDMJBCPGG#LBC777JDDMBPOCPG#TTBCJMOLLBCBOTTP#77DBCLOPJGGMMCBLOP#TTBGCBOLDDMJBCPGG#LBC777JDDMPLLOCPG#TTBCJMOLLBÿ:j,J,1,0,7TDMPGLOCBÿ:u,APPPP.PP.PP.PPPPÿ:w,F,100,-1,0ÿ:w,J,3,40,0ÿ:w,J,4,200,0ÿ:w,J,5,2000,0ÿ:w,7,3,20,0ÿ:w,7,4,60,0ÿ:w,7,5,200,0ÿ:w,T,3,20,0ÿ:w,T,4,60,0ÿ:w,T,5,200,0ÿ:w,D,3,10,0ÿ:w,D,4,30,0ÿ:w,D,5,120,0ÿ:w,M,3,10,0ÿ:w,M,4,30,0ÿ:w,M,5,120,0ÿ:w,P,3,4,0ÿ:w,P,4,20,0ÿ:w,P,5,80,0ÿ:w,G,3,4,0ÿ:w,G,4,20,0ÿ:w,G,5,80,0ÿ:w,L,3,2,0ÿ:w,L,4,10,0ÿ:w,L,5,40,0ÿ:w,O,3,2,0ÿ:w,O,4,10,0ÿ:w,O,5,40,0ÿ:w,C,3,2,0ÿ:w,C,4,10,0ÿ:w,C,5,40,0ÿ:w,B,3,2,0ÿ:w,B,4,10,0ÿ:w,B,5,40,0ÿ:wa,F,100,-1,0,0,0,0:1,-1,0ÿ:wa,J,3,40,0,0,0,0:1,-1,0ÿ:wa,J,4,200,0,0,0,0:1,-1,0ÿ:wa,J,5,2000,0,0,0,0:1,-1,0ÿ:wa,7,3,20,0,0,0,0:1,-1,0ÿ:wa,7,4,60,0,0,0,0:1,-1,0ÿ:wa,7,5,200,0,0,0,0:1,-1,0ÿ:wa,T,3,20,0,0,0,0:1,-1,0ÿ:wa,T,4,60,0,0,0,0:1,-1,0ÿ:wa,T,5,200,0,0,0,0:1,-1,0ÿ:wa,D,3,10,0,0,0,0:1,-1,0ÿ:wa,D,4,30,0,0,0,0:1,-1,0ÿ:wa,D,5,120,0,0,0,0:1,-1,0ÿ:wa,M,3,10,0,0,0,0:1,-1,0ÿ:wa,M,4,30,0,0,0,0:1,-1,0ÿ:wa,M,5,120,0,0,0,0:1,-1,0ÿ:wa,P,3,4,0,0,0,0:1,-1,0ÿ:wa,P,4,20,0,0,0,0:1,-1,0ÿ:wa,P,5,80,0,0,0,0:1,-1,0ÿ:wa,G,3,4,0,0,0,0:1,-1,0ÿ:wa,G,4,20,0,0,0,0:1,-1,0ÿ:wa,G,5,80,0,0,0,0:1,-1,0ÿ:wa,L,3,2,0,0,0,0:1,-1,0ÿ:wa,L,4,10,0,0,0,0:1,-1,0ÿ:wa,L,5,40,0,0,0,0:1,-1,0ÿ:wa,O,3,2,0,0,0,0:1,-1,0ÿ:wa,O,4,10,0,0,0,0:1,-1,0ÿ:wa,O,5,40,0,0,0,0:1,-1,0ÿ:wa,C,3,2,0,0,0,0:1,-1,0ÿ:wa,C,4,10,0,0,0,0:1,-1,0ÿ:wa,C,5,40,0,0,0,0:1,-1,0ÿ:wa,B,3,2,0,0,0,0:1,-1,0ÿ:wa,B,4,10,0,0,0,0:1,-1,0ÿ:wa,B,5,40,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,50ÿ:m,25ÿ:b,12,2,3,4,5,8,10,15,20,30,40,50,80ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,50.0ÿbs,0,1,0ÿ:k,nullÿe,25,0,0,0ÿb,1000,1000,0ÿ:x,11544ÿs,1ÿr,1,5,54,45,69,7,39ÿx,6,d:2;0;0;0;0;0:3;0;0;0;0;0:4;0;0;0;8;0:5;0;0;0;0;0:8;0;0;0;0;0:10;0;0;0;0;0:15;0;0;0;0;0:20;0;0;0;0;0:30;0;0;0;0;0:40;0;0;0;0;0:50;0;0;0;0;0:80;0;0;0;0;0,l:false,c:0:0:0:8:0,m:12,f:2886,r:3:48:29:59:64ÿc,F,100,0,0,0,0,0ÿrw,0";
            }
        }
        #endregion

        public StarCandyGameLogic()
        {
            _gameID = GAMEID.StarCandy;
            GameName = "StarCandy";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,50,2000,50,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,50ÿ:m,25ÿ:b,12,2,3,4,5,8,10,15,20,30,40,50,80ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,50.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,5,54,45,69,7,39ÿx,6,d:2;0;0;0;0;0:3;0;0;0;0;0:4;0;0;0;0;0:5;0;0;0;0;0:8;0;0;0;0;0:10;0;0;0;0;0:15;0;0;0;0;0:20;0;0;0;0;0:30;0;0;0;0;0:40;0;0;0;0;0:50;0;0;0;0;0:80;0;0;0;0;0,l:false,c:0:0:0:8:0,m:12,f:2886,r:3:48:29:59:64ÿc,F,100,0,0,0,0,0ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
