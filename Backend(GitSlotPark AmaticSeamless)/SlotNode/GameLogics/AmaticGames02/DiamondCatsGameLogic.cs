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
    class DiamondCatsGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DiamondCats";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 40, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "0522685558a02286667a93338a4449b7c7b9b711aa922633022244db79555da9bcb8118ab79666a90a7d22a1122079448ca3338bb7855b80966a9bddd9666d5552261133c445d88977098a22a8798666779bb5559022a18a033448bc9bd1227d55509a66697a9a9cbabb333522585558a02286667a933384449b7c7b9b711aa922533022244db7955da9bcb8118ab79666a90a7d2271122079448ca338bb7855b80966a9bd9666d5552261133c445d88977098a22a8798666779bb5559022718a033448bc9b12275509a66697a9a9cbabb333030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010150000015000001500000";
            }
        }

        protected override string ExtraString => "150000015000001500000";
        #endregion

        public DiamondCatsGameLogic()
        {
            _gameID     = GAMEID.DiamondCats;
            GameName    = "DiamondCats";
        }
    }
}
