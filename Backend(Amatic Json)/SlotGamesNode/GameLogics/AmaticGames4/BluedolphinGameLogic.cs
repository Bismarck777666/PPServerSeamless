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
   
    class BluedolphinGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Bluedolphin";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "05221693b57819ca082b4a95679ab4673a8281221564b80718bca927a395689a4b63789b2a222193a4756ca0b928b56789ab467893ab8672223947562b5a1b089ca7b489368a2b0785642234b17395acb62790a589b4860a29380618075220b365719ca0982b4a59678b4693a728a1220a564b0718bc92a395679a48ba36789282231973a64b5a0b9285b67c89ab469378a58672233947562b5a1b089ca6789ca4b73689a5b282244ba173956acb290a6586789ab46892b371870301010101010104271010001131f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c80b1010101010101010101010";
            }
        }
        #endregion

        public BluedolphinGameLogic()
        {
            _gameID     = GAMEID.Bluedolphin;
            GameName    = "Bluedolphin";
        }
    }
}
