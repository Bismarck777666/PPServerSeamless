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
   
    class LadyJokerGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LadyJoker";
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
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0524d6600006644773c3c8552b2b6666777755551a1a8773c444466555587757753c3c1a48442b2b662440000777744664461a663c3c99997777785557666677844441a1a66662b2b7877554423c6663c3c85555000055445558544441a557557751a1a52b2b99994444664423900001a1a2b2b999977772b2b71a43c53c48441a3c3c66666444455557243000055771a551a440044551a1a2b2b66444441a3c3c6666777782b443c2b3c2b2b500301010101010104271010001a5186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d03310101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101011";
            }
        }
        protected override int LineTypeCnt
        {
            get
            {
                return 5;
            }
        }
        protected override string ExtraString => "11";
        #endregion

        public LadyJokerGameLogic()
        {
            _gameID     = GAMEID.LadyJoker;
            GameName    = "LadyJoker";
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
