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
   
    class MagicOwlGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "MagicOwl";
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
                return "0522d111162222666611115444495555000087333377778888230000066663337630000555522227777111188881111444458233111133335981111444466677779888800002222555566796786230000067780000222233335558811114444666677788551111230111155598888111177796664444779660000333355222288522d1111622226666111154444955550000873333777788882300000666633376300005555222277771111888811114444582331111333359811114444666777798888000022225555667967862300000677800002222333355588111144446666777885511112301111555988881111777966644447796600003333552222880001010101010104b96e100121437d0321010101111101032320a1100101010101000000000000000002211121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f40423284271033101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 5;
        #endregion

        public MagicOwlGameLogic()
        {
            _gameID                 = GAMEID.MagicOwl;
            GameName                = "MagicOwl";
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
