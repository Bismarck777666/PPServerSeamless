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
    class HotSoccerGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotSoccer";
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
                return new long[] { 40 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0524d666644444444089a555555336633445544111117551155663333332222244557224433224411624e622446611115522225555666611222233445576666111554422089a44444455555533333557336251622662244666666664444441111144333333344733089a222225555553366336655113366557115562516111112222225555555333336622444444422666666611335544662233755441155089a446655447625166666224422222552255333333766334411663344111114444444089a5511555555554466755335560030101010101010427101000115186a02810101010101010282828110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public HotSoccerGameLogic()
        {
            _gameID     = GAMEID.HotSoccer;
            GameName    = "HotSoccer";
        }
    }
}
