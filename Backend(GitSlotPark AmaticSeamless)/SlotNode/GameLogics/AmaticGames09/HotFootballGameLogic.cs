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
   
    class HotFootballGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotFootball";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 5 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "05263533355576661117444222364616660533372225556665552221711222070224443336222333364057566655555666555666264133374445557666521172223466663333666444466664443331112223330704444366637326763334445666555666444666626d1172233374446667555222544455511155507044411122204445551112224440555111222203344411157504444171555444111555666101000301010101010104271010001131f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c80610101010101001";
            }
        }
        protected override string ExtraString => "01";
        #endregion

        public HotFootballGameLogic()
        {
            _gameID     = GAMEID.HotFootball;
            GameName    = "HotFootball";
        }
    }
}
