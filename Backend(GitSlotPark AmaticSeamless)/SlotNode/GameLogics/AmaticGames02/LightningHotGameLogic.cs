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
    class LightningHotGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LightningHot";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 40, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "052265755666551141445430333225552274464225422955110166673334424220665551133666446470211228555066674440411133355522623334466760442422d5655666331104444043337225566011667644402211202295755114664444333506622033666015552211440000301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b10101010101010101010101300013000130001300013000";
            }
        }

        protected override string ExtraString => "10101010101300013000130001300013000";
        #endregion

        public LightningHotGameLogic()
        {
            _gameID     = GAMEID.LightningHot;
            GameName    = "LightningHot";
        }
    }
}
