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
   
    class Hot7DeluxeGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Hot7Deluxe";
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
                return "0521975556665511414443033320222195110166673333444242250555219555056667444041131333226221956566633110444404333722552195551147444743335066622033003010101010101042710100a1131f4051010101010101a0505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c806101010101010";
            }
        }
        #endregion

        public Hot7DeluxeGameLogic()
        {
            _gameID     = GAMEID.Hot7Deluxe;
            GameName    = "Hot7Deluxe";
        }
    }
}
