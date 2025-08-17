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
   
    class TweetyBirdsGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "TweetyBirds";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 10, 20, 30, 40 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0522b0444466775999993338881144999998888877799222227022239bbbb66a8555a7711777711222a555566422b0499bbbb755536663333888815555a244449999a88822a888833330772222222253364899a7777166699bbbb23804466669999911888885777772999999937777776666bbbb99999977521c044566555993933881199172772221822bbbb661177b70b55bb334421cbbbb66331155a224409a88bbbb7721a88333302222599bb771666bbbb21d04499991188882377776666bbbb550301010101010104271010002145186a0281010101010101028280a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 4;
        #endregion

        public TweetyBirdsGameLogic()
        {
            _gameID     = GAMEID.TweetyBirds;
            GameName    = "TweetyBirds";
        }

        protected override int getLineTypeFromPlayLine(BaseAmaticSlotBetInfo betInfo)
        {
            switch (betInfo.PlayLine)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                case 30:
                    return 2;
                case 40:
                    return 3;
                default:
                    return -1;
            }
        }
    }
}
