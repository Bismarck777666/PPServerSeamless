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
    class BurningBells40GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BurningBells40";
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
                return "05250444665555877775553322221184442266664477333385522226600006666777755551111877333322472228777785556666333399997777000077770044666644111166668777784444116666223c0004455550005555444466633338555577711182222999944441111555502406664444555500001111999977772222111100002222999933338444411133336244777822226644440000555577111144440000111133330005511112222443333666670030101010101010427101000115186a02810101010101010282828110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public BurningBells40GameLogic()
        {
            _gameID     = GAMEID.BurningBells40;
            GameName    = "BurningBells40";
        }
    }
}
