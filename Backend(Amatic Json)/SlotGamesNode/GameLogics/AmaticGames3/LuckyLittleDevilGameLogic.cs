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
   
    class LuckyLittleDevilGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyLittleDevil";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000, 3000, 4000, 5000 };
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
                return "0f1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a1b0123456789a0030101010101010101010101010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b10101010101010101010100013fff1ffffffffffffffff";
            }
        }

        protected override int Cols => 15;

        protected override int FreeCols => 0;

        protected override string ExtraString => "0013fff1ffffffffffffffff";
        #endregion

        public LuckyLittleDevilGameLogic()
        {
            _gameID     = GAMEID.LuckyLittleDevil;
            GameName    = "LuckyLittleDevil";
        }
    }
}
