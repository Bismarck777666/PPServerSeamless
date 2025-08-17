using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Newtonsoft.Json;
using Akka.Configuration;
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
    class AztecSecretGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "AztecSecret";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 40, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 10 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0522a17538728408694526784516873670452968407534622a26718438507463587594561764825347168254176822a73586437517260824961827361542541725438183622a57280637145249681435671873517246735604728722973807526956045168450781628734285862731376522b175387284086945267845168736704529684075346522a26718438507463587594561764825347168254176822a73586437517260824961827361542541725438183622a572806371452496814356718735172467356047287229738075269560451684507816287342858627313760301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b101010101010101010101010";
            }
        }
        #endregion

        public AztecSecretGameLogic()
        {
            _gameID     = GAMEID.AztecSecret;
            GameName    = "AztecSecret";
        }
    }
}
