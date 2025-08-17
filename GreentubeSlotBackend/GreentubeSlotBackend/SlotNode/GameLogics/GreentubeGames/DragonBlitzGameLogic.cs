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
    internal class DragonBlitzGameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol
        {
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
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ2578ÿ1.10.54.0ÿRedlineSlotServerÿ12158ÿ1.0.2ÿDragon Blitz 95%ÿ1.119.0_2025-04-14_123855ÿ1.165.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿM,10,10000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Dragon Blitz 95%ÿ:v,3ÿ:l,-----ÿ:l,^^^^^ÿ:l,_____ÿ:l,^-_-^ÿ:l,_-^-_ÿ:l,-___-ÿ:l,-^^^-ÿ:l,__-^^ÿ:l,^^-__ÿ:l,_---^ÿ:l,^---_ÿ:l,-_-^-ÿ:l,-^-_-ÿ:l,^-^-^ÿ:l,_-_-_ÿ:l,--^--ÿ:l,--_--ÿ:l,^_^_^ÿ:l,_^_^_ÿ:l,_^-^_ÿ:r,0,ATSJQ2K3JA1KJ2NK2TASJK3TJ3A1KJSTN0KJ0NKSTJ1QJ2ÿ:r,0,NK3TQ1AT2QA3TJ2KT0NJ0AK3QA3NQWAT1NQ2ATWJQ2NAWQÿ:r,0,Q3N1AJ2QK3TQWN2KJ2TQSJN3A1KTWNJ2AKWNASJN1K0AN3JT1K0NQ3KASJT0KQ1N0JT0AKSTNWQ3NTWJA1QJ0Nÿ:r,0,NT2KJ3TQ1JN1TQ0KJ3NT2ANKWJN3QJWNT0AJ1NT3Qÿ:r,0,N3T0KN1T3AJ2NQSJT1A3Q0JN1QJ1T3AQ3K2NTSJQ2KJSNA2ÿ:r,1,J1T3KJ3AK0JN1AJSKT0JK2AJSNT3JQSKT2JN2TQSKJ1AK2ÿ:r,1,QN2ATWJQ1KA1QK3TJ3AQ3NT2AQ3NAWQT2NA0TK0NJ2TQWAÿ:r,1,A1J3KQ2NJ0Q1TJWNK2T3JNSAQ0NTSAK0NT1JWKQ3ANWÿ:r,1,AJ0TNWQK2TN0JK3TQ1JT2NJT3QN1JQ3NJWTA1NK3Nÿ:r,1,N1J0NA3J1TK3QJ3N2AQ1J3QT0ANSTQ2K1J2TKSNJ2T3NASQÿ:r,2,ATSJQ2K3JA1KJ2NK2TASJK3TJ3A1KJSTN0KJ0NKSTJ1QJ2ÿ:r,2,NK3TQ1AT2QA3TJ2KT0NJ0AK3QA3NQWAT1NQ2ATWJQ2NAWQÿ:r,2,Q3N1AJ2QK3TQWN2KJ2TQSJN3A1KTWNJ2AKWNASJN1K0AN3JT1K0NQ3KASJT0KQ1N0JT0AKSTNWQ3NTWJA1QJ0Nÿ:r,2,NT2KJ3TQ1JN1TQ0KJ3NT2ANKWJN3QJWNT0AJ1NT3Qÿ:r,2,N3T0KN1T3AJ2NQSJT1A3Q0JN1QJ1T3AQ3K2NTSJQ2KJSNA2ÿ:r,3,J1T3KJ3AK0JN1AJSKT0JK2AJSNT3JQSKT2JN2TQSKJ1AK2ÿ:r,3,QN2ATWJQ1KA1QK3TJ3AQ3NT2AQ3NAWQT2NA0TK0NJ2TQWAÿ:r,3,A1J3KQ2NJ0Q1TJWNK2T3JNSAQ0NTSAK0NT1JWKQ3ANWÿ:r,3,AJ0TNWQK2TN0JK3TQ1JT2NJT3QN1JQ3NJWTA1NK3Nÿ:r,3,N1J0NA3J1TK3QJ3N2AQ1J3QT0ANSTQ2K1J2TKSNJ2T3NASQÿ:r,4,ATSJQ2K7JA1KJ2NK2TASJK7TJ7A1KJSTN0KJ0NKSTJ1QJ2ÿ:r,4,NK7TQ1AT2QA7TJ2KT0NJ0AK7QA7NQWAT1NQ2ATWJQ2NAWQÿ:r,4,Q7N1AJ2QK7TQWN2KJ2TQSJN7A1KTWNJ2AKWNASJN1K0AN7JT1K0NQ7KASJT0KQ1N0JT0AKSTNWQ7NTWJA1QJ0Nÿ:r,4,NT2KJ7TQ1JN1TQ0KJ7NT2ANKWJN7QJWNT0AJ1NT7Qÿ:r,4,N7T0KN1T7AJ2NQSJT1A7Q0JN1QJ1T7AQ7K2NTSJQ2KJSNA2ÿ:r,5,J1T7KJ7AK0JN1AJSKT0JK2AJSNT7JQSKT2JN2TQSKJ1AK2ÿ:r,5,QN2ATWJQ1KA1QK7TJ7AQ7NT2AQ7NAWQT2NA0TK0NJ2TQWAÿ:r,5,A1J7KQ2NJ0Q1TJWNK2T7JNSAQ0NTSAK0NT1JWKQ7ANWÿ:r,5,AJ0TNWQK2TN0JK7TQ1JT2NJT7QN1JQ7NJWTA1NK7Nÿ:r,5,N1J0NA7J1TK7QJ7N2AQ1J7QT0ANSTQ2K1J2TKSNJ2T7NASQÿ:r,6,ATSJQ6K7JA1KJ6NK6TASJK7TJ7A1KJSTN0KJ0NKSTJ1QJ6ÿ:r,6,NK7TQ1AT6QA7TJ6KT0NJ0AK7QA7NQWAT1NQ6ATWJQ6NAWQÿ:r,6,Q7N1AJ6QK7TQWN6KJ6TQSJN7A1KTWNJ6AKWNASJN1K0AN7JT1K0NQ7KASJT0KQ1N0JT0AKSTNWQ7NTWJA1QJ0Nÿ:r,6,NT6KJ7TQ1JN1TQ0KJ7NT6ANKWJN7QJWNT0AJ1NT7Qÿ:r,6,N7T0KN1T7AJ6NQSJT1A7Q0JN1QJ1T7AQ7K6NTSJQ6KJSNA6ÿ:r,7,J1T7KJ7AK0JN1AJSKT0JK6AJSNT7JQSKT6JN6TQSKJ1AK6ÿ:r,7,QN6ATWJQ1KA1QK7TJ7AQ7NT6AQ7NAWQT6NA0TK0NJ6TQWAÿ:r,7,A1J7KQ6NJ0Q1TJWNK6T7JNSAQ0NTSAK0NT1JWKQ7ANWÿ:r,7,AJ0TNWQK6TN0JK7TQ1JT6NJT7QN1JQ7NJWTA1NK7Nÿ:r,7,N1J0NA7J1TK7QJ7N6AQ1J7QT0ANSTQ6K1J6TKSNJ6T7NASQÿ:r,8,ATSJQ6K7JA5KJ6NK6TASJK7TJ7A5KJSTN0KJ0NKSTJ5QJ6ÿ:r,8,NK7TQ5AT6QA7TJ6KT0NJ0AK7QA7NQWAT5NQ6ATWJQ6NAWQÿ:r,8,Q7N5AJ6QK7TQWN6KJ6TQSJN7A5KTWNJ6AKWNASJN5K0AN7JT5K0NQ7KASJT0KQ5N0JT0AKSTNWQ7NTWJA5QJ0Nÿ:r,8,NT6KJ7TQ5JN5TQ0KJ7NT6ANKWJN7QJWNT0AJ5NT7Qÿ:r,8,N7T0KN5T7AJ6NQSJT5A7Q0JN5QJ5T7AQ7K6NTSJQ6KJSNA6ÿ:r,9,J5T7KJ7AK0JN5AJSKT0JK6AJSNT7JQSKT6JN6TQSKJ5AK6ÿ:r,9,QN6ATWJQ5KA5QK7TJ7AQ7NT6AQ7NAWQT6NA0TK0NJ6TQWAÿ:r,9,A5J7KQ6NJ0Q5TJWNK6T7JNSAQ0NTSAK0NT5JWKQ7ANWÿ:r,9,AJ0TNWQK6TN0JK7TQ5JT6NJT7QN5JQ7NJWTA5NK7Nÿ:r,9,N5J0NA7J5TK7QJ7N6AQ5J7QT0ANSTQ6K5J6TKSNJ6T7NASQÿ:r,10,ATSJQ6K7JA5KJ6NK6TASJK7TJ7A5KJSTN4KJ4NKSTJ5QJ6ÿ:r,10,NK7TQ5AT6QA7TJ6KT4NJ4AK7QA7NQWAT5NQ6ATWJQ6NAWQÿ:r,10,Q7N5AJ6QK7TQWN6KJ6TQSJN7A5KTWNJ6AKWNASJN5K4AN7JT5K4NQ7KASJT4KQ5N4JT4AKSTNWQ7NTWJA5QJ4Nÿ:r,10,NT6KJ7TQ5JN5TQ4KJ7NT6ANKWJN7QJWNT4AJ5NT7Qÿ:r,10,N7T4KN5T7AJ6NQSJT5A7Q4JN5QJ5T7AQ7K6NTSJQ6KJSNA6ÿ:r,11,J5T7KJ7AK4JN5AJSKT4JK6AJSNT7JQSKT6JN6TQSKJ5AK6ÿ:r,11,QN6ATWJQ5KA5QK7TJ7AQ7NT6AQ7NAWQT6NA4TK4NJ6TQWAÿ:r,11,A5J7KQ6NJ4Q5TJWNK6T7JNSAQ4NTSAK4NT5JWKQ7ANWÿ:r,11,AJ4TNWQK6TN4JK7TQ5JT6NJT7QN5JQ7NJWTA5NK7Nÿ:r,11,N5J4NA7J5TK7QJ7N6AQ5J7QT4ANSTQ6K5J6TKSNJ6T7NASQÿ:j,W,1,0,01234567AKQJNTÿ:j,4,1,0,01234567AKQJNTWÿ:j,5,1,0,01234567AKQJNTWÿ:j,6,1,0,01234567AKQJNTWÿ:j,7,1,0,01234567AKQJNTWÿ:u,APPPP.PP.PP.PPPPÿ:w,S,3,-2,0ÿ:w,W,3,50,0ÿ:w,W,4,200,0ÿ:w,W,5,1000,0ÿ:w,0,3,50,0ÿ:w,0,4,200,0ÿ:w,0,5,1000,0ÿ:w,1,3,30,0ÿ:w,1,4,100,0ÿ:w,1,5,500,0ÿ:w,2,3,20,0ÿ:w,2,4,60,0ÿ:w,2,5,200,0ÿ:w,3,3,20,0ÿ:w,3,4,60,0ÿ:w,3,5,200,0ÿ:w,A,3,10,0ÿ:w,A,4,30,0ÿ:w,A,5,120,0ÿ:w,K,3,10,0ÿ:w,K,4,30,0ÿ:w,K,5,120,0ÿ:w,Q,3,10,0ÿ:w,Q,4,30,0ÿ:w,Q,5,120,0ÿ:w,J,3,5,0ÿ:w,J,4,20,0ÿ:w,J,5,100,0ÿ:w,T,3,5,0ÿ:w,T,4,20,0ÿ:w,T,5,100,0ÿ:w,N,3,5,0ÿ:w,N,4,20,0ÿ:w,N,5,100,0ÿ:wa,S,3,-2,0,0,0,0:1,0:2:4:6:8:10,0ÿ:wa,S,3,-2,0,0,0,0:1,1:3:5:7:9:11,0ÿ:wa,W,3,50,0,0,0,0:1,-1,0ÿ:wa,W,4,200,0,0,0,0:1,-1,0ÿ:wa,W,5,1000,0,0,0,0:1,-1,0ÿ:wa,0,3,50,0,0,0,0:1,-1,0ÿ:wa,0,4,200,0,0,0,0:1,-1,0ÿ:wa,0,5,1000,0,0,0,0:1,-1,0ÿ:wa,1,3,30,0,0,0,0:1,-1,0ÿ:wa,1,4,100,0,0,0,0:1,-1,0ÿ:wa,1,5,500,0,0,0,0:1,-1,0ÿ:wa,2,3,20,0,0,0,0:1,-1,0ÿ:wa,2,4,60,0,0,0,0:1,-1,0ÿ:wa,2,5,200,0,0,0,0:1,-1,0ÿ:wa,3,3,20,0,0,0,0:1,-1,0ÿ:wa,3,4,60,0,0,0,0:1,-1,0ÿ:wa,3,5,200,0,0,0,0:1,-1,0ÿ:wa,A,3,10,0,0,0,0:1,-1,0ÿ:wa,A,4,30,0,0,0,0:1,-1,0ÿ:wa,A,5,120,0,0,0,0:1,-1,0ÿ:wa,K,3,10,0,0,0,0:1,-1,0ÿ:wa,K,4,30,0,0,0,0:1,-1,0ÿ:wa,K,5,120,0,0,0,0:1,-1,0ÿ:wa,Q,3,10,0,0,0,0:1,-1,0ÿ:wa,Q,4,30,0,0,0,0:1,-1,0ÿ:wa,Q,5,120,0,0,0,0:1,-1,0ÿ:wa,J,3,5,0,0,0,0:1,-1,0ÿ:wa,J,4,20,0,0,0,0:1,-1,0ÿ:wa,J,5,100,0,0,0,0:1,-1,0ÿ:wa,T,3,5,0,0,0,0:1,-1,0ÿ:wa,T,4,20,0,0,0,0:1,-1,0ÿ:wa,T,5,100,0,0,0,0:1,-1,0ÿ:wa,N,3,5,0,0,0,0:1,-1,0ÿ:wa,N,4,20,0,0,0,0:1,-1,0ÿ:wa,N,5,100,0,0,0,0:1,-1,0ÿ:s,0ÿ:i,20ÿ:m,20ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,0.0ÿ:fs,0,0,0,0,0,0ÿbs,0,1,0ÿ:k,nullÿe,20,0,0,-1ÿb,0,0,0ÿs,1ÿr,1,5,37,13,3,29,34ÿx,3,0:0,-:-,-ÿrw,0";
            }
        }
        #endregion

        public DragonBlitzGameLogic()
        {
            _gameID = GAMEID.DragonBlitz;
            GameName = "DragonBlitz";
        }
        
        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,20,3000,20,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,20ÿ:m,20ÿ:b,15,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,20.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,5,37,13,3,29,34ÿx,3,0:0,-:-,-ÿrw,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
