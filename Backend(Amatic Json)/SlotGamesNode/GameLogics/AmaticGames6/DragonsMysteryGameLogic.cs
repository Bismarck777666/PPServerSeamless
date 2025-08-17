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
    class DragonsMysteryGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DragonsMystery";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0524900001111222255553333666644447777aaaa88881111888822227777333366669555544442490000555511116666222277773333988884444aaaa11115555222266663333777744448888249000055551111666622227777333388884444aaaa66661111977772222888833334444555524b0000111155552222666693333777794444888811118888aaaa222233337777444466669555524b000011115555222266663333977774444888897777222288883333aaaa6666444495555111152310000555511112222aaaa44446666aaaa3333777798888aaaa231aaaa55550000111122223333aaaa44446666977778888aaaa231aaaa6666000011117777222233334444aaaa555598888aaaa23100001111888892222aaaa333355554444aaaa66667777aaaa23100001111aaaa222288889333377774444aaaa55556666aaaa0301010101010104271010001a5186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d03310101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 5;
        protected override string ExtraString => "10";
        #endregion

        public DragonsMysteryGameLogic()
        {
            _gameID     = GAMEID.DragonsMystery;
            GameName    = "DragonsMystery";
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
                case 50:
                    return 4;
                default:
                    return -1;
            }
        }
    }
}
