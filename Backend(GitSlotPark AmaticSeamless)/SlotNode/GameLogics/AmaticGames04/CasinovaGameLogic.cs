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
   
    class CasinovaGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Casinova";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 20 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052291753724067495684526794516734576456947546522d26714350746375278456456745671364256347147256522f7359643751726048612734612534567456745712534971623c542706371435248261435671234567567456172435637152435760476273233734172493670526854156012345167945071627342596273143521617953724063756846921452146207143507146352784021973596437017926048524164592174257063761435248263567521a73415249734670652689514539030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public CasinovaGameLogic()
        {
            _gameID     = GAMEID.Casinova;
            GameName    = "Casinova";
        }
    }
}
