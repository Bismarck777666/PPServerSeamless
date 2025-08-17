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
    internal class LuckyLadysCharmDeluxe6GameLogic : BaseGreenSlotGame
    {
        #region
        protected override string ScatterSymbol {
            get
            {
                return "S";
            }
        }

        protected override int MinCountToFreespin { 
            get
            {
                return 3;
            }
        }

        protected override string VersionCheckString
        {
            get
            {
                return "\b0ÿ2.5.16ÿCasino-Lobbymanager-Serverÿ919ÿ1.10.54.0ÿNGI_GS2_Slot_Serverÿ930ÿ1.0.0ÿLucky Lady's Charm Deluxe 6 95%ÿ1.113.0_2024-11-19_131508ÿ1.93.0";
            }
        }

        protected override string GameUniqueString
        {
            get
            {
                return "\u0006S0ÿA1ÿRÿL,3200,100.0,ÿM,10,10000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿ:n,Lucky Lady's Charm Deluxe 6 95%ÿ:v,3ÿ:l,------ÿ:l,^^^^^^ÿ:l,______ÿ:l,^-_-^^ÿ:l,_-^-__ÿ:l,-___--ÿ:l,-^^^--ÿ:l,__-^^^ÿ:l,^^-___ÿ:l,_---^^ÿ:r,0,5JTWQNKTA1T54KQ3AQSJQ2J4TKJT4J3A5TWQNKTA1T5KQ3AQSJQ2J4TKJT4J3A5TWQNKTA1T5KQ3AQSJQ2J4TKJT4J3Aÿ:r,0,4KWAKT1QJN5TA2JQ1AQ4KATSN3J4K52JN3KJ5KJ4N3QA4KWAQKT1QJN5TA2JQ1AQ4KTSN3J4K52JN3KJ5KJ4NK3QA4KWAKT1QJN54TA2JQ1AQ4KTSN3J4K52JN3KJ5KJ4N3QAÿ:r,0,TQWT3A1N53JT2N4TJ1QNASQT2AT4Q53K1J4A2N3Q5AK4A5TQWT3A1N53J2N4TJ1QNSQT2A4Q53K1J4A2N3Q5AK4A5TQWT3A1N53J2N4TJ1KQNSQT2A4Q5A3K1J4A2N3Q5AK4A5ÿ:r,0,KTWQ2AJ3Q1A5J4NAJ5NK4TSK5J1A5N4KTAWQ25N3J4NJ2KTWQ2AJ3Q1A5J4NJQ5NK4T2SK5J1A5N4KTWQA25N3J4NJ2KTWQ2AJ3Q1A5J24NJ5NK4TSK5J1A5N4KTWQ25N3J4NJ2ÿ:r,0,KAWK3Q4AQ5T2AK1Q4JT1NS4J3Q5A1KA1NQ2KJ3N4KN1KAWK3Q4AQ5T2AK1Q4JT1NS4J31Q5A1KA1NQ2KJ3N4KN1KAWK3Q4AQ5T2AK1Q4JT1NS4J3QT5A1KA1NQ2KJ3N4KN1ÿ:r,0,ZVMIGYUHOILXUZVILX#UZVRLXGZVUHOGZVMIGYUHOILXUZVILX#UZVRLXGZVUHOGZVMIGYUHOILXUZVILX#UZVRLXIGZVUHOGÿ:r,1,T4W23T1AJKQ23N41AJK23N15JK2SQN5J4AQN5T4AQN5T4AQK3T4W23T1AJK253N1TAJK23N15JK2SQ3N5J4AQN5T4AQN5T4AQK3T4W23T1AJK23TN1AJK234N15JK2ASQN5J43AQN5T24AQN5T4JAQK3ÿ:r,1,52WNT52J3QNTA2J4QSN1AJ4KN1AT4KN1QWT4KJ3QT5KJ34QT52WNT52J3QNTA2J4QSN1AJ4KN1AT4K5N1QWT4KJ3QT5KJ3QT52WNT52J3QNTA2J4QSN1AJ4KN1AT4KN1QWT4KJ3QAT5K2J3QTÿ:r,1,1QWJN1QT3AJN1KT4AJ52KT4AJS52N4QJ52N4QT32N1QT3AN1QWJN1QT3AJ2N1KT4AJ52KT4AJS52N4QJ52N4QT32N1QT3AN1QWJN14QT3AJN1KQT4AJN52KT4AJS52N4QJ52N4QT32N1QJT3ANÿ:r,1,2KW54QKN3T54QKN31S54QJ3154TJ213NATWJ21NA5TJ2KN3TJ2KW54QKNA3T54QKN31S54QJ3154TJ21NATWJ21NATJ52KN3TJ2KW54QKN3T54QKN231S54QJ3154TJ21NATWJ21NATJ2K5N3TJÿ:r,1,K1W2NK4TAJ2N5QKJ2S35QKJT35QK1AT35WNK1T35NK1TA5NK1W2NK4TAJ23N5QKJ2S35QKJT35QAK1T235WNK1T35NK1TA5NK1W2NK4TAJ2N5QKJ2S35QKJAT35QKJ1T325WNK1T35NK1TA5Nÿ:r,1,14WJN35TKJQ2514JQ2S514AQ2N145TKN14WJN35TKJQ2514JQ2ST514AQ2N145TKN14WJN35TKJQ2514JQ2S514AQ2N145TQKNÿ:r,2,5TWQN4TK1A5QJN3AQSJQ2TA4JQ1KATJ2KT3A5TWQN4TK1A5JN3AQSJQ2A4JQ1KTJ2KT3A5TWQN4TK1A5JN3TAQSJQN2A4JQ1AKTJ2KT3Aÿ:r,2,45KWAKJ1Q3T4K5WAKTSN43JA2T5J35TSN4Q5TQJ4KWAKJ1Q3T4KWAKTSN3JA2T5J35TSN4Q5TQJ4KWAKJ1Q3T4KWAKTSN3JA2T5J35TSN4Q5TQJÿ:r,2,TQWT3AK15TQWT23KNSQK1NJ4N25AN4SQJ42A4JTQWT3AK15TQWT3KN4SQK1NJ4N2K5ANSTQJ42A4JTQWT3AK15TQWT3KNSAQK1NJ4N25ANSQJ42A4Jÿ:r,2,KTWQ2AJ3A1K5J34TSK5NT4AQJ14TSK5Q2NA3KTAWQ2AJ3A1K5J34TSK5NT4AQJ14TSK5Q2NJA3KTWQ2AJ3AT1K5J34TSK53NT4AQJ14TSK5Q2NA3ÿ:r,2,KAWK3Q4NA5TJ2AK1NS4JAKQ5J3Q1NS4J3T52TKAWK3Q4NA5T2AK1NS4JAKQ5J3Q1NS4J3T52TKAWK3Q4NAQ5T2AK1NS34JAKQ5J3Q1NS4J3T52Tÿ:r,2,YRMZIYLGURVZHXL#IVURGHXYLIORG#HVLIORUGHVIYRMZIYLGUVZHXL#IURGHXLIOZRG#HVLXIORGHVIYRMZIYLGUVZHXL#IURGHXLIORG#HVLIOZRGHVIÿ:r,3,1QWT5J24KNT5J24KN1T5SJAKQ3KNA12Q3KNA1QWT5J24KNT5J24KNT5SJAKQ3KNA1Q53KNA1QWT5J24KNT5J24KNT5SJ2AKQ3KN2A1Q35KNAÿ:r,3,TKWJ3TKQA4JSAT5Q2NJAWT35Q2NJA4S3Q1KQA43TKWJ3TKQA4JSAT5Q2NJAWT5Q2NJA4SQ51KQA43TKWJ23TKQA4JKSAT5Q2NKJAWT5Q2NJA4SQ1KQA43ÿ:r,3,JTW2N1JTQK523KJTQK54K3SANJ54K3ANJT4K51JTW21JTQK52KJTQK54K3SATNJ5Q4K3ANJT4K51JTW21JTQKA52KJTQK54K3SANJ54K3ANJT4K51ÿ:r,3,KNW4T3NAJQ4ST3N2AJQ4TW5NA1JQATQS2KQAJQ2KNW4T3NAJQ4ST3NAJQ4TKW5NA1QATQS2KQAJQ2KNW4T3NAJQ24ST3NAJQ4TW5NA1QATQS2KQAJQ2ÿ:r,3,KTW45QTNKJ41QT3KJN1AS23KJNKA25KTNKJ5KTW45QTNKJ41QT3KJN1TAS23KJNKA25KTNKJ5KTW45QTNKJ41QT3KJ4N1AS23KJNKA25KTNKJ5ÿ:r,3,3AW1J3QN541T2QSJ5ANT2Q3JKANST4QJKANT4J3AW1J3QN54K1T2QSJ5A4NT2QJKANST4QJKANT4J3AW1J3QN541T2QSJ5ANT2QJKANST4QJKANT4Jÿ:j,W,2,0,12345AKQJTNÿ:u,APPPP.PP.PP.PPPPÿ:w,#,2,-2,0ÿ:w,#,3,-5,0ÿ:w,#,4,-20,0ÿ:w,#,5,-500,0ÿ:w,#,6,-1500,0ÿ:w,S,2,-2,0ÿ:w,S,3,-5,0ÿ:w,S,4,-20,0ÿ:w,S,5,-500,0ÿ:w,W,2,10,0ÿ:w,W,3,250,0ÿ:w,W,4,2500,0ÿ:w,W,5,9000,0ÿ:w,W,6,20000,0ÿ:w,1,2,2,0ÿ:w,1,3,25,0ÿ:w,1,4,125,0ÿ:w,1,5,750,0ÿ:w,1,6,10000,0ÿ:w,2,2,2,0ÿ:w,2,3,25,0ÿ:w,2,4,125,0ÿ:w,2,5,750,0ÿ:w,2,6,10000,0ÿ:w,3,3,20,0ÿ:w,3,4,100,0ÿ:w,3,5,400,0ÿ:w,3,6,1500,0ÿ:w,4,3,15,0ÿ:w,4,4,75,0ÿ:w,4,5,250,0ÿ:w,4,6,1000,0ÿ:w,5,3,15,0ÿ:w,5,4,75,0ÿ:w,5,5,250,0ÿ:w,5,6,1000,0ÿ:w,A,3,10,0ÿ:w,A,4,50,0ÿ:w,A,5,125,0ÿ:w,A,6,500,0ÿ:w,K,3,10,0ÿ:w,K,4,50,0ÿ:w,K,5,125,0ÿ:w,K,6,500,0ÿ:w,Q,3,5,0ÿ:w,Q,4,25,0ÿ:w,Q,5,100,0ÿ:w,Q,6,300,0ÿ:w,J,3,5,0ÿ:w,J,4,25,0ÿ:w,J,5,100,0ÿ:w,J,6,300,0ÿ:w,T,3,5,0ÿ:w,T,4,25,0ÿ:w,T,5,100,0ÿ:w,T,6,300,0ÿ:w,N,2,2,0ÿ:w,N,3,5,0ÿ:w,N,4,25,0ÿ:w,N,5,100,0ÿ:w,N,6,300,0ÿ:wa,#,2,-2,0,0,0,0:1,0:1:2:3,0ÿ:wa,#,3,-5,0,0,0,0:1,0:1:2:3,0ÿ:wa,#,4,-20,0,0,0,0:1,0:1:2:3,0ÿ:wa,#,5,-500,0,0,0,0:1,0:1:2:3,0ÿ:wa,#,6,-1500,0,0,0,0:1,0:1:2:3,0ÿ:wa,S,2,-2,0,0,0,0:1,0:1,0ÿ:wa,S,2,-6,0,0,0,0:1,2,0ÿ:wa,S,2,-12,0,0,0,0:1,3,0ÿ:wa,S,3,-5,0,0,0,0:1,0,0ÿ:wa,S,4,-20,0,0,0,0:1,0,0ÿ:wa,S,5,-500,0,0,0,0:1,0,0ÿ:wa,S,3,-5,0,0,0,0:1,1,0ÿ:wa,S,4,-20,0,0,0,0:1,1,0ÿ:wa,S,5,-500,0,0,0,0:1,1,0ÿ:wa,S,6,-1500,0,0,0,0:1,1,0ÿ:wa,S,3,-15,0,0,0,0:1,2,0ÿ:wa,S,4,-60,0,0,0,0:1,2,0ÿ:wa,S,5,-1500,0,0,0,0:1,2,0ÿ:wa,S,3,-30,0,0,0,0:1,3,0ÿ:wa,S,4,-120,0,0,0,0:1,3,0ÿ:wa,S,5,-3000,0,0,0,0:1,3,0ÿ:wa,S,6,-9000,0,0,0,0:1,3,0ÿ:wa,W,2,10,0,0,0,0:1,0:1,0ÿ:wa,W,3,250,0,0,0,0:1,0:1,0ÿ:wa,W,4,2500,0,0,0,0:1,0:1,0ÿ:wa,W,5,9000,0,0,0,0:1,0:1,0ÿ:wa,W,6,20000,0,0,0,0:1,0:1,0ÿ:wa,W,2,30,0,0,0,0:1,2,0ÿ:wa,W,3,750,0,0,0,0:1,2,0ÿ:wa,W,4,7500,0,0,0,0:1,2,0ÿ:wa,W,5,27000,0,0,0,0:1,2,0ÿ:wa,W,2,60,0,0,0,0:1,3,0ÿ:wa,W,3,1500,0,0,0,0:1,3,0ÿ:wa,W,4,15000,0,0,0,0:1,3,0ÿ:wa,W,5,54000,0,0,0,0:1,3,0ÿ:wa,W,6,120000,0,0,0,0:1,3,0ÿ:wa,1,2,2,0,0,0,0:1,0:1,0ÿ:wa,1,3,25,0,0,0,0:1,0:1,0ÿ:wa,1,4,125,0,0,0,0:1,0:1,0ÿ:wa,1,5,750,0,0,0,0:1,0:1,0ÿ:wa,1,6,10000,0,0,0,0:1,0:1,0ÿ:wa,1,2,6,0,0,0,0:1,2,0ÿ:wa,1,3,75,0,0,0,0:1,2,0ÿ:wa,1,4,375,0,0,0,0:1,2,0ÿ:wa,1,5,2250,0,0,0,0:1,2,0ÿ:wa,1,2,12,0,0,0,0:1,3,0ÿ:wa,1,3,150,0,0,0,0:1,3,0ÿ:wa,1,4,750,0,0,0,0:1,3,0ÿ:wa,1,5,4500,0,0,0,0:1,3,0ÿ:wa,1,6,60000,0,0,0,0:1,3,0ÿ:wa,2,2,2,0,0,0,0:1,0:1,0ÿ:wa,2,3,25,0,0,0,0:1,0:1,0ÿ:wa,2,4,125,0,0,0,0:1,0:1,0ÿ:wa,2,5,750,0,0,0,0:1,0:1,0ÿ:wa,2,6,10000,0,0,0,0:1,0:1,0ÿ:wa,2,2,6,0,0,0,0:1,2,0ÿ:wa,2,3,75,0,0,0,0:1,2,0ÿ:wa,2,4,375,0,0,0,0:1,2,0ÿ:wa,2,5,2250,0,0,0,0:1,2,0ÿ:wa,2,2,12,0,0,0,0:1,3,0ÿ:wa,2,3,150,0,0,0,0:1,3,0ÿ:wa,2,4,750,0,0,0,0:1,3,0ÿ:wa,2,5,4500,0,0,0,0:1,3,0ÿ:wa,2,6,60000,0,0,0,0:1,3,0ÿ:wa,3,3,20,0,0,0,0:1,0:1,0ÿ:wa,3,4,100,0,0,0,0:1,0:1,0ÿ:wa,3,5,400,0,0,0,0:1,0:1,0ÿ:wa,3,6,1500,0,0,0,0:1,0:1,0ÿ:wa,3,3,60,0,0,0,0:1,2,0ÿ:wa,3,4,300,0,0,0,0:1,2,0ÿ:wa,3,5,1200,0,0,0,0:1,2,0ÿ:wa,3,3,120,0,0,0,0:1,3,0ÿ:wa,3,4,600,0,0,0,0:1,3,0ÿ:wa,3,5,2400,0,0,0,0:1,3,0ÿ:wa,3,6,9000,0,0,0,0:1,3,0ÿ:wa,4,3,15,0,0,0,0:1,0:1,0ÿ:wa,4,4,75,0,0,0,0:1,0:1,0ÿ:wa,4,5,250,0,0,0,0:1,0:1,0ÿ:wa,4,6,1000,0,0,0,0:1,0:1,0ÿ:wa,4,3,45,0,0,0,0:1,2,0ÿ:wa,4,4,225,0,0,0,0:1,2,0ÿ:wa,4,5,750,0,0,0,0:1,2,0ÿ:wa,4,3,90,0,0,0,0:1,3,0ÿ:wa,4,4,450,0,0,0,0:1,3,0ÿ:wa,4,5,1500,0,0,0,0:1,3,0ÿ:wa,4,6,6000,0,0,0,0:1,3,0ÿ:wa,5,3,15,0,0,0,0:1,0:1,0ÿ:wa,5,4,75,0,0,0,0:1,0:1,0ÿ:wa,5,5,250,0,0,0,0:1,0:1,0ÿ:wa,5,6,1000,0,0,0,0:1,0:1,0ÿ:wa,5,3,45,0,0,0,0:1,2,0ÿ:wa,5,4,225,0,0,0,0:1,2,0ÿ:wa,5,5,750,0,0,0,0:1,2,0ÿ:wa,5,3,90,0,0,0,0:1,3,0ÿ:wa,5,4,450,0,0,0,0:1,3,0ÿ:wa,5,5,1500,0,0,0,0:1,3,0ÿ:wa,5,6,6000,0,0,0,0:1,3,0ÿ:wa,A,3,10,0,0,0,0:1,0:1,0ÿ:wa,A,4,50,0,0,0,0:1,0:1,0ÿ:wa,A,5,125,0,0,0,0:1,0:1,0ÿ:wa,A,6,500,0,0,0,0:1,0:1,0ÿ:wa,A,3,30,0,0,0,0:1,2,0ÿ:wa,A,4,150,0,0,0,0:1,2,0ÿ:wa,A,5,375,0,0,0,0:1,2,0ÿ:wa,A,3,60,0,0,0,0:1,3,0ÿ:wa,A,4,300,0,0,0,0:1,3,0ÿ:wa,A,5,750,0,0,0,0:1,3,0ÿ:wa,A,6,3000,0,0,0,0:1,3,0ÿ:wa,K,3,10,0,0,0,0:1,0:1,0ÿ:wa,K,4,50,0,0,0,0:1,0:1,0ÿ:wa,K,5,125,0,0,0,0:1,0:1,0ÿ:wa,K,6,500,0,0,0,0:1,0:1,0ÿ:wa,K,3,30,0,0,0,0:1,2,0ÿ:wa,K,4,150,0,0,0,0:1,2,0ÿ:wa,K,5,375,0,0,0,0:1,2,0ÿ:wa,K,3,60,0,0,0,0:1,3,0ÿ:wa,K,4,300,0,0,0,0:1,3,0ÿ:wa,K,5,750,0,0,0,0:1,3,0ÿ:wa,K,6,3000,0,0,0,0:1,3,0ÿ:wa,Q,3,5,0,0,0,0:1,0:1,0ÿ:wa,Q,4,25,0,0,0,0:1,0:1,0ÿ:wa,Q,5,100,0,0,0,0:1,0:1,0ÿ:wa,Q,6,300,0,0,0,0:1,0:1,0ÿ:wa,Q,3,15,0,0,0,0:1,2,0ÿ:wa,Q,4,75,0,0,0,0:1,2,0ÿ:wa,Q,5,300,0,0,0,0:1,2,0ÿ:wa,Q,3,30,0,0,0,0:1,3,0ÿ:wa,Q,4,150,0,0,0,0:1,3,0ÿ:wa,Q,5,600,0,0,0,0:1,3,0ÿ:wa,Q,6,1800,0,0,0,0:1,3,0ÿ:wa,J,3,5,0,0,0,0:1,0:1,0ÿ:wa,J,4,25,0,0,0,0:1,0:1,0ÿ:wa,J,5,100,0,0,0,0:1,0:1,0ÿ:wa,J,6,300,0,0,0,0:1,0:1,0ÿ:wa,J,3,15,0,0,0,0:1,2,0ÿ:wa,J,4,75,0,0,0,0:1,2,0ÿ:wa,J,5,300,0,0,0,0:1,2,0ÿ:wa,J,3,30,0,0,0,0:1,3,0ÿ:wa,J,4,150,0,0,0,0:1,3,0ÿ:wa,J,5,600,0,0,0,0:1,3,0ÿ:wa,J,6,1800,0,0,0,0:1,3,0ÿ:wa,T,3,5,0,0,0,0:1,0:1,0ÿ:wa,T,4,25,0,0,0,0:1,0:1,0ÿ:wa,T,5,100,0,0,0,0:1,0:1,0ÿ:wa,T,6,300,0,0,0,0:1,0:1,0ÿ:wa,T,3,15,0,0,0,0:1,2,0ÿ:wa,T,4,75,0,0,0,0:1,2,0ÿ:wa,T,5,300,0,0,0,0:1,2,0ÿ:wa,T,3,30,0,0,0,0:1,3,0ÿ:wa,T,4,150,0,0,0,0:1,3,0ÿ:wa,T,5,600,0,0,0,0:1,3,0ÿ:wa,T,6,1800,0,0,0,0:1,3,0ÿ:wa,N,2,2,0,0,0,0:1,0:1,0ÿ:wa,N,3,5,0,0,0,0:1,0:1,0ÿ:wa,N,4,25,0,0,0,0:1,0:1,0ÿ:wa,N,5,100,0,0,0,0:1,0:1,0ÿ:wa,N,6,300,0,0,0,0:1,0:1,0ÿ:wa,N,2,6,0,0,0,0:1,2,0ÿ:wa,N,3,15,0,0,0,0:1,2,0ÿ:wa,N,4,75,0,0,0,0:1,2,0ÿ:wa,N,5,300,0,0,0,0:1,2,0ÿ:wa,N,2,12,0,0,0,0:1,3,0ÿ:wa,N,3,30,0,0,0,0:1,3,0ÿ:wa,N,4,150,0,0,0,0:1,3,0ÿ:wa,N,5,600,0,0,0,0:1,3,0ÿ:wa,N,6,1800,0,0,0,0:1,3,0ÿ:s,0ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10,20ÿ:b,16,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,20,0,10,10ÿb,1000,10000,0ÿ:x,800ÿs,1ÿr,1,6,6,23,17,21,4,7ÿrw,0ÿ:wee,0";
            }
        }
        #endregion

        public LuckyLadysCharmDeluxe6GameLogic()
        {
            _gameID = GAMEID.LuckyLadysCharmDeluxe6;
            GameName = "LuckyLadysCharmDeluxe6";
        }

        protected override string buildLineBetString(string strGlobalUserID, double balance)
        {
            return string.Format("\u0006S{0}ÿA0ÿC,100.0,,1,ÿT,0,0,0ÿRÿM,1,5000,1,-1,0ÿI00ÿH,0,0,0,0,0,0ÿY,7200,10ÿ:i,1ÿ:i,2ÿ:i,3ÿ:i,4ÿ:i,5ÿ:i,6ÿ:i,7ÿ:i,8ÿ:i,9ÿ:i,10ÿ:i,10ÿ:m,1,2,3,4,5,6,7,8,9,10,20ÿ:b,16,1,2,3,4,5,8,10,15,20,30,40,50,80,100,150,200ÿ:a,0,0ÿ:g,999,1000,-1,false,0,falseÿ:es,0,0ÿ:mbnr,1.0ÿbs,0,1,0ÿ:k,nullÿe,{1},{2},{3},-1ÿb,1000,1000,0ÿs,1ÿr,1,6,6,23,17,21,4,7ÿrw,0ÿ:wee,0", balance, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultLine : _dicUserBetInfos[strGlobalUserID].PlayLine, !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? this._defaultBetPerLine : (_dicUserBetInfos[strGlobalUserID].PlayBet / _dicUserBetInfos[strGlobalUserID].PlayLine), !_dicUserBetInfos.ContainsKey(strGlobalUserID) ? Array.IndexOf(_supportLines, this._defaultLine) : Array.IndexOf(_supportLines, _dicUserBetInfos[strGlobalUserID].PlayLine));
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
