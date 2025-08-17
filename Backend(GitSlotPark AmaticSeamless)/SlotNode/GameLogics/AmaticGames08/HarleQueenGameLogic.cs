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
    class HarleQueenGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HarleQueen";
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
                return "0524d000006666447733335522226666777755551111773333444466555577775553322221144422662840000777770044666664411666333388888888777775556666777444411116666222277700007777700446666644116663333777774444111166662222777888888882806663333555500004455550005554444111155555777111222288888888884444666333355550000445555000555444411115555577711122224444888888888829000000011110002222288888888887777722221111333334444111333366666444445555500000011110003333366666222211113333344441117777788888888882222444445555524a000055557711114444000005511112222664444400000111113333336666777722222443330030101010101010427101000115186a02810101010101010282828110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public HarleQueenGameLogic()
        {
            _gameID     = GAMEID.HarleQueen;
            GameName    = "HarleQueen";
        }
    }
}
