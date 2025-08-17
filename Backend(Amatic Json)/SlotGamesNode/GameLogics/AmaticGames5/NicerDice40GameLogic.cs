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
   
    class NicerDice40GameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "NicerDice40";
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
                return new long[] { 40 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "05251111166633355500005555111666644422224444004441111666655533330003336662222555444222251333355533344400004444666222002222666644441111555511115550005553333444111666222444255000044433355533334444222006661111666660070066661112222555500055544433366611155522244424c444000055511114443333666622200666333444411155552222000555444433366611155522224c33334443335550000444422200666111116666111555522225550004443336661115552226660030101010101010427101000115186a02810101010101010282828110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010001200";
            }
        }

        protected override string ExtraString => "001200";
        #endregion

        public NicerDice40GameLogic()
        {
            _gameID     = GAMEID.NicerDice40;
            GameName    = "NicerDice40";
        }
    }
}
