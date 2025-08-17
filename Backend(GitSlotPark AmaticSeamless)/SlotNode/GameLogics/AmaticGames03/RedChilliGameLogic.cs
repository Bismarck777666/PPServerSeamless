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
   
    class RedChilliGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "RedChilli";
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
                return "0522c000666222666555666333711144466633322266363362296667444111555222333555000444555444565511122c44455522266655571116663336660007666555336366228333000666755511144422255544455544422222222622200066644466633366671116665556663336522700606622266655533371114443633222661333122a666744411155522233350004445557444561110022225444555222665457111633300066675336312122b33300606567551114442225554445554447222001112282220060664446663363667111556563633012422030101010101010427101000112641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public RedChilliGameLogic()
        {
            _gameID     = GAMEID.RedChilli;
            GameName    = "RedChilli";
        }
    }
}
