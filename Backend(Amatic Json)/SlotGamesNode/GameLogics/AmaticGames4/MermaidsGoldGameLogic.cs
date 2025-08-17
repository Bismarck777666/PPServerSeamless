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
   
    class MermaidsGoldGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "MermaidsGold";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
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
                return "05220a578368c98264a79a493865a08b7a61823062981795794b3947acb38647067a189b37947529b5c9a89a22ba4a2983648a165b62a9562a80a378175936b1978bc822ba0825b394b95b74ac759165b47a082693816594b92722ba267184b92793b47b176073849a1bc493b561971b955221a08b4a71659b368c982649817a927a365224a47067acb3962a5935acb485a89470679183224a3671b94b256bc8942649a80a37bc8715a80224ac7593617582b637a0826934ac75ba468914224a52a7607381bc4967859384b65a2671bc4930301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010";
            }
        }
        #endregion

        public MermaidsGoldGameLogic()
        {
            _gameID     = GAMEID.MermaidsGold;
            GameName    = "MermaidsGold";
        }
    }
}
