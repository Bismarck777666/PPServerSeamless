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
   
    class BellsOnFireGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BellsOnFire";
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
                return new long[] { 40 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052510000066664477333385522226666777755551111877333344446655558777755533222211844422662470000777770044666664411666333399997777785556666877784444111166662222877723d6663333855550000445555000555444411115555577711182222999994444249000000111100022222999997777722221111333338444411199999333366666444445555524b00005555771111444400000551111222266444440000011111333333666677778222224433300001010101010104b96e100121437d028101010111110102828281100101010101000000000000000002211121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f404232842710291010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }

        protected override int LineTypeCnt
        {
            get
            {
                return 1;
            }
        }
        #endregion

        public BellsOnFireGameLogic()
        {
            _gameID                 = GAMEID.BellsOnFire;
            GameName                = "BellsOnFire";
        }
    }
}
