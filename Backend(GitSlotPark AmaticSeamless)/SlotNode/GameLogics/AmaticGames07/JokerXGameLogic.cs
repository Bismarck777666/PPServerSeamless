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
    class JokerXGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "JokerX";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8 ,9 ,10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 2000 };
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
                return "052511111666333555000055551116666444222244440044411116666555333300033366622225554442222513333555333444000044446662220022226666444411115555111155500055533334441116662224442530000444333555333344442220066611116666607066661112222555500055544433366611155522244424c444000055511114443333666622200666333444411155552222000555444433366611155522224c333344433355500004444222006661111166661115555222255500044433366611155522266600301010101010104271010002142641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a2641510101010101010101010101010101010101010101000";
            }
        }

        protected override string ExtraString => "00";
        #endregion

        public JokerXGameLogic()
        {
            _gameID     = GAMEID.JokerX;
            GameName    = "JokerX";
        }
    }
}
