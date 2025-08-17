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
   
    class BookOfAztecGameLogic : BaseAmaticExtra3Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BookOfAztec";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 6, 8, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "052326054265265347246574562480468947156357268475618527422d71547850678537856857854735837542783751687694822e48367826425407817628437648376874985146186078152322652843786257086596436815347651876284679850461425422e1607825683516248967145879570452634836425726437522594715741689570543672542857165436471682254583795648524768174376948625860746738224451640680574985718625762753647587408225347807495836798563516704625825638526822595714287951642740782405381625361806380301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public BookOfAztecGameLogic()
        {
            _gameID     = GAMEID.BookOfAztec;
            GameName    = "BookOfAztec";
        }
    }
}
