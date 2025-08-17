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
   
    class Hot7GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Hot7";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 4, 8, 10, 12, 16, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000, 3000, 4000 };
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
                return "052193332022755666555114144430219333344424225055511016666721933322625550566674440411312193331104444043372255565666219333555114744474335066622000301010101010104271010001131f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c806101010101010";
            }
        }
        #endregion

        public Hot7GameLogic()
        {
            _gameID     = GAMEID.Hot7;
            GameName    = "Hot7";
        }
    }
}
