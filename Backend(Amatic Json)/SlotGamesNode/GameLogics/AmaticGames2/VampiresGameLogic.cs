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
   
    class VampiresGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Vampires";
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
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "052440600060607060403070413141516030306060306070607050307050705050907070304040404050505070507070708030804060304040906080808080f1011120808080823207070607040403060606030406060306040a0a0a0a07070907050503040704040f101112080405131415160808080b0c0d0e23706060303060309050505040405050308080005050505000404040f101112050503070707090813141516080a0a0a0a040408080b0c0d0e24608131415160a0a0a0a07070a070700071314151606030303090404000403040a0a0a0a0a03030306030604040f10111206060304040505050f1011120808050508080b0c0d0e243050b0c0d0e0507050507040400040405050006060304040b0c0d0e040303070f101112060303030606071314151606070709041314151604030308030808080f10111252400600060706060404070303060706030607050503070605070509070703040405030405040503070705070808070304090404081314151606080806080f101112231070007060700040406060306040403060600060a0a0a0a05070707030705040509040304040f101112080808081314151623706060503030605030905050b0c0d0e050404030505080800050403040407050500070905071314151608080a0a0a0a040408080f101112246000513141516040a0a0a0a07070a0707071314151603030f10111203090404040303040a0a0a0a0a0303030603060604060600040405080805050b0c0d0e080805080f10111224205050705040404040b0c0d0e0505060600040304040f101112030303030606060007060307070904131415160403080308080b0c0d0e08050f101112070713141516030101010101010427101000115186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0331010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010108ffffffff";
            }
        }
        protected override int LineTypeCnt => 5;
        protected override int ReelSetBitNum => 2;
        protected override string ExtraString => "8ffffffff";
        #endregion

        public VampiresGameLogic()
        {
            _gameID     = GAMEID.Vampires;
            GameName    = "Vampires";
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
