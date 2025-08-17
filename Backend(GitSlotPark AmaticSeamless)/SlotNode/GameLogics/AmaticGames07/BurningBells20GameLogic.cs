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
    class BurningBells20GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BurningBells20";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8 ,9 ,10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "05244244466555877755533222118444226664477333855222660006667775551118773332392228777855566633399977700077700446664411166687778444116662310004455500055544466633385557771118222999444111555231666444555000111999777222111000222999333844411133323677782226644400055577111444000111333000551112224433366600301010101010104271010002142641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public BurningBells20GameLogic()
        {
            _gameID     = GAMEID.BurningBells20;
            GameName    = "BurningBells20";
        }
    }
}
