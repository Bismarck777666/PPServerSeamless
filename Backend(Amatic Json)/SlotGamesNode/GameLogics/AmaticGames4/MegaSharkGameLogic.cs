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
   
    class MegaSharkGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "MegaShark";
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
                return "052295755666551141445430333225552244642254504522d55110166673334424220665551133666446402116676622c5550666744404111333555226233344666044246606622f5655666331104444043337225566011666444221124404422857551146644443335066220336660155522114405215766655144303366655524216066733442265566655514621755566674401555226633446217554466631446033722555662185714664435550664462233550301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010";
            }
        }
        #endregion

        public MegaSharkGameLogic()
        {
            _gameID     = GAMEID.MegaShark;
            GameName    = "MegaShark";
        }
    }
}
