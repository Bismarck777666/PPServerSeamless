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
   
    class HotTwentyGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotTwenty";
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
                return "0524d6666444444440005555555336633445544111117551155663333332222244557224433224411624d622446611155222255556666111222233445576666111554422000444444555555333335573362506226622446666666644444411111443333333447330002222255555533663366551133665571155625061111122222255555553333366224444444226666666113355446622335544115500044665574476250666662244222225522553333337663344116633441111144444440005511555555554466755335560030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public HotTwentyGameLogic()
        {
            _gameID     = GAMEID.HotTwenty;
            GameName    = "HotTwenty";
        }
    }
}
