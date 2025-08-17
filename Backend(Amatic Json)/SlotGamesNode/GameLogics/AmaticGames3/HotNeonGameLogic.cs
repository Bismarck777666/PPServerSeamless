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
   
    class HotNeonGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotNeon";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000, 3000, 4000 };
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
                return "052255755666551141445430333225552244642254228551101666733344242206655511336664464021122755506667444041113335552262333446660442422a56556663311044440433372255660116664442211222857551146644443335066220336660155522114405215766655144303366655524216066733442265566655514621755566674401555226633446217554466631446033722555662185714664435550664462233550301010101010104271010001131f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c80b1010101010101010101010";
            }
        }
        #endregion

        public HotNeonGameLogic()
        {
            _gameID     = GAMEID.HotNeon;
            GameName    = "HotNeon";
        }
    }
}
