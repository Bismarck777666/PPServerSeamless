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
   
    class MerryFruitsGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "MerryFruits";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "052506622334411331133557446622333333555555555574444444444444400011111222222226666666625333711223311223344444444445555555555555511111122222244550006666666666112276655333333253443344553311220002211555555555566666666666666711111166711333333223322222266444444442533344223322331111155337555555555556666666666611222222755664444444444400011224466333324d667221155663344116655442222233111117555555555554444444444400033333226666666660030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010";
            }
        }

        protected override int FreeCols => 0;
        #endregion

        public MerryFruitsGameLogic()
        {
            _gameID     = GAMEID.MerryFruits;
            GameName    = "MerryFruits";
        }
    }
}
