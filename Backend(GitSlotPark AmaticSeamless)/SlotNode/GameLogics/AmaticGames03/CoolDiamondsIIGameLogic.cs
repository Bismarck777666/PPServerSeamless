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
   
    class CoolDiamondsIIGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "CoolDiamondsII";
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
                return new long[] { 10 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0522b0006662226665556663337111444666333222666333228666744411155522233355500044455544455511122c44455522266655571116663336660007666555333666225333000666755511144422255544455544422222572220006664446663336661116665556663335225000666222555666333711144466633322233322666674441115552223335550004445554447555226444555222666555711166633366600075553332263330006667555111444222555444555744422222272220006664446663336661116665553330301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010";
            }
        }
        #endregion

        public CoolDiamondsIIGameLogic()
        {
            _gameID     = GAMEID.CoolDiamondsII;
            GameName    = "CoolDiamondsII";
        }
    }
}
