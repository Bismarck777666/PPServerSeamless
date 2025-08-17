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
   
    class BookOfFruitsGameLogic : BaseAmaticExtra3Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BookOfFruits";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 4, 6, 8, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "05237605426526538191384724657456248046894715635726475618527423171547850678537856420648578573583754278375168769482314836782642540781762843764837687498510514618607815235265281284378625708659643681534765187628467985046142542311607825683516248967145879570452632634836425726437522594715741689570543672542857165436471682254583795648524768174376948625860746738224451640680574985718625762753647587408225347807495836798563516704625825638526822595714287951642740782405381625361806380301010101010104271010001131f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c806101010101010101010";
            }
        }
        #endregion

        public BookOfFruitsGameLogic()
        {
            _gameID     = GAMEID.BookOfFruits;
            GameName    = "BookOfFruits";
        }
    }
}
