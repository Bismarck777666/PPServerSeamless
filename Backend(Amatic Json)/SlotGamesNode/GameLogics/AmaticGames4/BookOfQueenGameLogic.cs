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
   
    class BookOfQueenGameLogic : BaseAmaticExtra3Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BookOfQueen";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 10 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052412854750657164254795678468734653046286456784685485645278264528164522c8406485783617568567387657863859435745827457225b683458153752186785608746243286485467548754364064235417513647617697263165038571480758468273924334586450843617486576487548617286076217528617462854697584934507652682306573841836476358276217054275386294816593468524075238846580639235428715245382562136174584178657410483729503272375745987657064062836437647045874365928657408748658645761235846579827063781456374972483628761381786475680185167152345647603527652138754862354758654752380386549389356217235845867408247654614857651437841287264279481571685180730301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010101010";
            }
        }
        #endregion

        public BookOfQueenGameLogic()
        {
            _gameID     = GAMEID.BookOfQueen;
            GameName    = "BookOfQueen";
        }
    }
}
