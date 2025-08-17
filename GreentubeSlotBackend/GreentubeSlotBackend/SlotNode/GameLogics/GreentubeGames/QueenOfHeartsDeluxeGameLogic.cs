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
    internal class QueenOfHeartsDeluxeGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ54ÿ1.10.54.0ÿGTATSlotServerÿ367ÿ1.0.0ÿQueen of Hearts 95%ÿ1.100.0_2023-12-14_090056ÿ1.101.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Queen of Hearts 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:r,0,CK9O1KOJ1OK9JIQKOJ1#K9OJ1RK9HJKO9JOK1CJK9R1JO9C1HQJOK#JO1CK9O1KOJ1OKJIQKOJ1#K9OJ1RK9HJKO9JOK1CJ9R1JO9C1HQJOK#JO1CK9O1KOJ1OKJIQKOJ1#K9OJ1RK9HJKO9JOK1CJ9R1JO9C1HQJOK#JO1ÿ:r,0,I9KIQC9IK#QC9IJOQCKI9HQIKR9I1CQRKI9CQH9CQR9CKIQC9IJ#QI9CQI9KIQC9IK#QC9IJOQCKI9HQIKR9I1C#QRKI9CQH9CQR9CKIQC9IJ#QI9CQI9KIQC9IK#QC9IJOQCKI9HQIKR9I1CQRKI9CQH9CQR9CKIQC9IJ#QI9CQÿ:r,0,HQJ9R1JK9C1JQ#1J9I1JQCKJ9RQKH1QCJ1OQ9IJQC1JCK1C9JCK1#QJC91HQJ9R1J9C1JQ#1J9I1JQCKJ9RQCKH1QCJ1OQ9IJQC1JCK1C9JCK1#QJC91HQJ9R1J9C1JQ#1J9KI1JQCKJ9RQKH1QCJ1OQ9IJQC1JCK1C9JCK1#QJC91ÿ:r,0,I1QJ#QKH1CJQ1RQK1QC1QOJ1KQCJ1QH91QIK19Q1JCQ1CQ1HJQ1#Q1KQI1QJ#QK1CHJQ1RQK1QJC1Q9OJ1QCJ1QH91QIK1C9Q1CQ1CQ1HJQ1#Q1KQI1QJ#QK1CJQ1RQK1QC#1QOJ1QCJ1QH91QIK19Q1CQ1CQ1HJQ1#Q1KQÿ:r,0,O91JC91QO91KOQ9KHJ1O9KOQ9O1JOK91OJ9O1JO91CJ9O1Q#91KRJ9O1K9IQJO91JC91QO91KOQ9KHJ1O9KOQ9O1JO91OJ9O1JO91CJ9O1Q#9KRJ9O1KIQJO91JC91QO91KOQ9KHJ1O9KOQ9O1JO91OJ9O1JOK91CJ9O1Q#9KRJ9O1KIQJÿ:r,1,AK921KO61OK6IQKOJ1#K9OJ1RK97J1KO9JOK1OJ9R1JO9C1HKJOK#JO1AK921KO61OK6I9QKOJ1#K9OJ1RK697JKO9JO6K1OJ9R1JO9CO1HKJOK#JO1AK9621KO61OK6IQKOJ1#K9OJ1RK97JKO9JOK1OJ9R1JO9C1HKJOK#JO1ÿ:r,1,39K3QC9RIK#QA9I6OQC4I9H5IKR8I1C5RK39CQ79IQC9R5KIQC9IJ#QI9CQ39K35QC9IK#QA9I6OQC34I9H5IKR8I19C5RK39CQ79IQC9RK3IQC9IJ#QI9CQ39K3QC9IK#QA9I6OQC4I9H5QIKR8I1C5RK39CQR79IQC9RKIQ3C9IJ#QI9RCQÿ:r,1,H5J9R7J9C16Q#K1J8I1JQAKJ9RQK81QCJ1OQ9IJQC1JCK1C9JCK1#QJC91H5J9R7J9C16Q#1J8I1JQAKJ9RQK81QCJ1OQ9IJQC1JCK1C9JCK1#QJC91H5J9R7J9C16Q#1J8I1JQAKJ9RQK81QCJ1OQ9IJQC1JCK1C9JCK1#QJC91ÿ:r,1,I1Q6#QK1C6Q1RQK1QA1QOJ1QCJ1Q691QIK19Q1CQ1CQ1HJQ1#Q1KQI1Q6#QK1C6Q1RQK61QA1QOJ1QCJ1Q691QIK19Q1CQ1CQ1HJQ1#Q1KQI1Q6#QK1C6Q1RQK1QA1QOJ1QCJ1Q691QIK19Q1CQ1CQ1HJQ1#JQ1KQÿ:r,1,O91JC915O914OQ9KHJ1O8KOQ9O1JO91OJ9O1JO91CJ9O1Q#9KRJ9O1KIQJO91JC915O914OQ9KHJ1O8KOQ9O1JO91OJ9O1JO91CJ9O1Q#9KRJ9O1KIQJO91JC915O914OQ9KHJ1O8KOQ9O1JO91OJ9O1JO91CJ9O1Q#9KRJ9O1K9IQJÿ:j,H,1,0,ÿ:j,A,1,0,ÿ:j,2,1,0,ÿ:j,3,1,0,ÿ:j,4,1,0,ÿ:j,5,1,0,ÿ:j,6,1,0,ÿ:j,7,1,0,ÿ:j,8,1,0,ÿ:u,APPPP.PP.PP.PPPPÿ:w,#,5,-400,0ÿ:w,#,4,-20,0ÿ:w,#,3,-5,0ÿ:w,#,2,-2,0ÿ:w,H,5,10000,0ÿ:w,H,4,2000,0ÿ:w,H,3,200,0ÿ:w,H,2,10,0ÿ:w,H,1,2,0ÿ:w,R,5,3000,0ÿ:w,R,4,300,0ÿ:w,R,3,50,0ÿ:w,R,2,5,0ÿ:w,C,5,500,0ÿ:w,C,4,100,0ÿ:w,C,3,25,0ÿ:w,C,2,2,0ÿ:w,O,5,500,0ÿ:w,O,4,75,0ÿ:w,O,3,20,0ÿ:w,I,5,500,0ÿ:w,I,4,75,0ÿ:w,I,3,20,0ÿ:w,K,5,250,0ÿ:w,K,4,50,0ÿ:w,K,3,10,0ÿ:w,Q,5,200,0ÿ:w,Q,4,50,0ÿ:w,Q,3,10,0ÿ:w,J,5,200,0ÿ:w,J,4,25,0ÿ:w,J,3,5,0ÿ:w,1,5,100,0ÿ:w,1,4,25,0ÿ:w,1,3,5,0ÿ:w,9,5,100,0ÿ:w,9,4,25,0ÿ:w,9,3,5,0ÿ:wa,#,5,-400,0,0,0,0:1,-1,0ÿ:wa,#,4,-20,0,0,0,0:1,-1,0ÿ:wa,#,3,-5,0,0,0,0:1,-1,0ÿ:wa,#,2,-2,0,0,0,0:1,-1,0ÿ:wa,H,5,10000,0,0,0,0:1,-1,0ÿ:wa,H,4,2000,0,0,0,0:1,-1,0ÿ:wa,H,3,200,0,0,0,0:1,-1,0ÿ:wa,H,2,10,0,0,0,0:1,-1,0ÿ:wa,H,1,2,0,0,0,0:1,-1,0ÿ:wa,R,5,3000,0,0,0,0:1,-1,0ÿ:wa,R,4,300,0,0,0,0:1,-1,0ÿ:wa,R,3,50,0,0,0,0:1,-1,0ÿ:wa,R,2,5,0,0,0,0:1,-1,0ÿ:wa,C,5,500,0,0,0,0:1,-1,0ÿ:wa,C,4,100,0,0,0,0:1,-1,0ÿ:wa,C,3,25,0,0,0,0:1,-1,0ÿ:wa,C,2,2,0,0,0,0:1,-1,0ÿ:wa,O,5,500,0,0,0,0:1,-1,0ÿ:wa,O,4,75,0,0,0,0:1,-1,0ÿ:wa,O,3,20,0,0,0,0:1,-1,0ÿ:wa,I,5,500,0,0,0,0:1,-1,0ÿ:wa,I,4,75,0,0,0,0:1,-1,0ÿ:wa,I,3,20,0,0,0,0:1,-1,0ÿ:wa,K,5,250,0,0,0,0:1,-1,0ÿ:wa,K,4,50,0,0,0,0:1,-1,0ÿ:wa,K,3,10,0,0,0,0:1,-1,0ÿ:wa,Q,5,200,0,0,0,0:1,-1,0ÿ:wa,Q,4,50,0,0,0,0:1,-1,0ÿ:wa,Q,3,10,0,0,0,0:1,-1,0ÿ:wa,J,5,200,0,0,0,0:1,-1,0ÿ:wa,J,4,25,0,0,0,0:1,-1,0ÿ:wa,J,3,5,0,0,0,0:1,-1,0ÿ:wa,1,5,100,0,0,0,0:1,-1,0ÿ:wa,1,4,25,0,0,0,0:1,-1,0ÿ:wa,1,3,5,0,0,0,0:1,-1,0ÿ:wa,9,5,100,0,0,0,0:1,-1,0ÿ:wa,9,4,25,0,0,0,0:1,-1,0ÿ:wa,9,3,5,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,10,0,9,-1ÿb,0,0,0ÿs,1ÿr,0,5,67,11,0,14,59ÿrw,0";
            }
        }
        #endregion

        public QueenOfHeartsDeluxeGameLogic()
        {
            _gameID = GAMEID.QueenOfHeartsDeluxe;
            GameName = "QueenOfHeartsDeluxe";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,3000,2,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10ÿ:b,17,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200,300ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,2.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,0,5,67,11,0,14,59ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
