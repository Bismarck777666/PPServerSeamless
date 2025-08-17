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
    class HotDice40GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotDice40";
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
                return "0524d6666444444440000555555336633445544111117551155663333332222244557224433224411624e6224466111155222255556666112222334455766661115544220000444444555555333335573362516226622446666666644444411111443333333447330000222225555553366336655113366557115562516111112222225555555333336622444444422666666611335544662233755441155000044665544762516666622442222255225533333376633441166334411111444444400005511555555554466755335560030101010101010427101000115186a02810101010101010282828110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public HotDice40GameLogic()
        {
            _gameID     = GAMEID.HotDice40;
            GameName    = "HotDice40";
        }
    }
}
