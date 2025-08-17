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
    class RelicRichesGameLogic : BaseAmaticExtra1Game 
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "RelicRiches";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 6, 7, 8 ,9 ,10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "0525b757817c97678b77a2c79a0bd81b3890a1c983c783adbc7789a2b7775838490b81c483abdc7768397798abc9d8392509dc2b677ba79875780bad9389b1a3c777ac189c280b97b2ca777791c2b7776780bda93ba4938d9c824aa18c109b7782a98b2c3bd8b767a291b0893a4ba777ca3c79c7a18db2ad98dc1987b23a5ba7275da0893a0cb1987757c81bda391cb77ba2bc777ab75a91b76a98da2b387a9cb481ba792b387a3b767cb6810849308d77776a1c728a0cdb1c28d938270b58c2b48c208b78c390ad987ba2c1938dc1a792b1a2b777cb0839a77b3adc39c7198dba1c2a1c2a981ba779cda1c2ada19b0d83b767c38c2525617c9e83aebc79a2c783c97678b7775781c9a0b81b3890a1c483abc983c77a2b77989776838490778975838251a791c2b677b2ca9389b1a3c777ac189c280b97ba4938e9c0bea989ec2b7798767875787780ba91a7c246767a18c109b777ca3c79c7a291b0893a5ba7782a98eb2c3be8b798b2987b277a3a4bab267bea2bc77ba792b38776a91cb198ea0893a0cb77757c81ba277a91c7ea3b75a987a39cb6810849398a028e1c28a377cb767cb481253839cea1c2a198eba2b1a779c71938c39a792b767cb08b78c2089ca17777c2b58c2b48c2a987ba981ba00301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b101010101010101010101010";
            }
        }

        protected override string ExtraString => "10";
        #endregion

        public RelicRichesGameLogic()
        {
            _gameID     = GAMEID.RelicRiches;
            GameName    = "RelicRiches";
        }
    }
}
