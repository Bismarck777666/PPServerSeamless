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
   
    class WildSharkGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "WildShark";
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
                return "0522b044446677599999333888114499999888887779922222a02223bcde96666a8555a777111777711222a55556622c0499bcde7555536663333888815555a244449999a88822a8888bcde333302222222253364899a77777766699123804466669999911888885777772999999937777776666bcde99999977521f999988887555666604441113333222221c777bcde2688bcde514990bcde3332267774bcde1330a556a6668a8222bcde9999bcde22026666bcde108884377bcde559999bcde21818839924467755566877788000010101010101012100121437d0321010101111101032320a1100101010101000000000000000002211121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f40423284271033101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 5;
        #endregion

        public WildSharkGameLogic()
        {
            _gameID                 = GAMEID.WildShark;
            GameName                = "WildShark";
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
