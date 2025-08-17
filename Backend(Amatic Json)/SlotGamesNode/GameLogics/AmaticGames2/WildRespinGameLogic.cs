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
    class WildRespinGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "WildRespin";
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
                return "052520000222211113333555544446666777787733662255774455664433227766551111776600005584444255000011112222333344445555999966667777833334444556666776666777711775577771100007777666625d555500001111222233334444777766668222299994444555566774466557755225544552255554444111133330000269000033332222444455551111666677778333399992222444477337733223322003300777777774444333322221111000055556666255111122220000333355556666444477778223366335544445577662277111144445555666677770000111100001010101010104ade8100121437d0281010101111101028280a1100101010101000000000000000002211121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f404232842710331010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010100f15fffff15fffff15fffff15fffff";
            }
        }
        protected override int LineTypeCnt  => 4;
        protected override string ExtraString => "0f15fffff15fffff15fffff15fffff";
        #endregion

        public WildRespinGameLogic()
        {
            _gameID     = GAMEID.WildRespin;
            GameName    = "WildRespin";
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
