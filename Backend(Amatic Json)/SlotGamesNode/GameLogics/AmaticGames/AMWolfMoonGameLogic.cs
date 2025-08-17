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
   
    class AMWolfMoonGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "WolfMoon";
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
                return new long[] { 10, 20, 30, 40 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "05255854a36738464911119a700005738473896222279611116552222a394a938486ba711111111a55222222222551111658473a48365600007565222285646547936738946a3754a53852222583764a8475376111169b958625e8791111a73894a7800007594783898a1111975a4a5222267386684a7111159378b678ba911111978a111111187222223f571111639473822225680000656ba83a92222839a490000751111a72222222223b751111a7396469384a000094838b7622225493b8a00007b5a1111652222523764939811118a2222a354674a000057364735b8228951111a6000079225111158000084546b937111162222a370000a922d111156382222a4647700005b60000493911115478383a22f745111165222273800006ba36471111835900009480000a23011116473922228ab536489000094500005461111670000870001010101010104c33c100121437d0281010101111101028280a1100101010101000000000000000002211121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f404232842710291010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 4;
        #endregion

        public AMWolfMoonGameLogic()
        {
            _gameID         = GAMEID.AMWolfMoon;
            GameName        = "WolfMoon";
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
