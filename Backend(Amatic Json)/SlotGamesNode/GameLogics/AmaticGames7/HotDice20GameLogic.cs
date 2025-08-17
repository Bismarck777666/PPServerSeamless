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
    class HotDice20GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotDice20";
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
                return "0524d6666444444440005555555336633445544111117551155663333332222244557224433224411624d6224466111552222555566661112222334455766661115544220004444445555553333355733625062266224466666666444444111114433333334473300022222555555336633665511336655711556250611111222222555555533333662244444442266666661133554466223355441155000446655744762506666622442222255225533333376633441166334411111444444400055115555555544667553355600301010101010104271010002142641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public HotDice20GameLogic()
        {
            _gameID     = GAMEID.HotDice20;
            GameName    = "HotDice20";
        }
    }
}
