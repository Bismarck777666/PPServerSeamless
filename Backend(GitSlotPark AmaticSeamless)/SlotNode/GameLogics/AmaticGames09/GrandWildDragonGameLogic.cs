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
    class GrandWildDragonGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GrandWildDragon";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 8, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override int LineTypeCnt => 5;
        protected override string InitString
        {
            get
            {
                return "0526f555566655111144433333089ab33322255572244442255755444455222555556661111166666089ab66666557222223333344444555522227b5511166673334444226655533666611111089ab11115556676661166676666655111663333344222225551111144444089ab44444555556611666766666267555666744444089ab44444113335552266333446664444446663335555544444089ab444446666663333366661111144662222226455556663311333344444089ab44447225566116664442211166675555555333333089ab333333355111114444422222666662735557116664444333666222089ab3366115552211444445556667222222555555664444433366666089ab666661111122222444555666733333300301010101010104271010001a5186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0331010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101544444";
            }
        }
        protected override string ExtraString => "1544444";
        #endregion

        public GrandWildDragonGameLogic()
        {
            _gameID     = GAMEID.GrandWildDragon;
            GameName    = "GrandWildDragon";
        }

        protected override int getLineTypeFromPlayLine(int playline)
        {
            switch (playline)
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
                    return 0;
            }
        }
    }
}
