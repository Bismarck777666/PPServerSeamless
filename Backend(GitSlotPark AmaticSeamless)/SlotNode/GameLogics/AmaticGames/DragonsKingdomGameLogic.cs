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

namespace SlotGamesNode.GameLogics
{
   
    class DragonsKingdomGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DragonsKingdom";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 20 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "05222666333377722288844444455511199900022822229993333337a66611118894444770005555a8226333357444466a888222250699907a8111176a72211116222974445076a883335a65005a97721d64447773335605111888000999222521b66333222444695111100044487921e2223337a97668911114440005a833321a333444222506a98111176a744421d111122244496a833385a600075a9721b6874443336507511198092221110001010101010104c33c100121437d014101010111110101414141100101010101000000000000000002211121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f404232842710151010101010101010101010101010101010101010108ffffffff";
            }
        }

        protected override int LineTypeCnt
        {
            get
            {
                return 1;
            }
        }
        #endregion

        public DragonsKingdomGameLogic()
        {
            _gameID     = GAMEID.DragonsKingdom;
            GameName    = "DragonsKingdom";
        }
    }
}
