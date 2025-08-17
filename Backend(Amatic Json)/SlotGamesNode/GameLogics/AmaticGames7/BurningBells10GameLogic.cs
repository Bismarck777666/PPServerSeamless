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
    class BurningBells10GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BurningBells10";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 6, 7, 8 ,9 ,10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "05244244466555877755533222118444226664477333855222660006667775551118773332392228777855566633399977700077700446664411166687778444116662310004455500055544466633385557771118222999444111555231666444555000111999777222111000222999333844411133323677782226644400055577111444000111333000551112224433366600301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010";
            }
        }
        #endregion

        public BurningBells10GameLogic()
        {
            _gameID     = GAMEID.BurningBells10;
            GameName    = "BurningBells10";
        }
    }
}
