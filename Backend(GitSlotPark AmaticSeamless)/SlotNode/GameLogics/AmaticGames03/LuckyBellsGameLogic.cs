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
   
    class LuckyBellsGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyBells";
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
                return "052217776665554442136660355583777944432213765483777666555444031564732a32102231037775551302166644492351261a712482223201735721777444555666324a618201706321f10920777134235124443016668325550030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a2641510101010101010101010101010101010101010101001";
            }
        }

        protected override string ExtraString => "01";
        #endregion

        public LuckyBellsGameLogic()
        {
            _gameID     = GAMEID.LuckyBells;
            GameName    = "LuckyBells";
        }
    }
}
