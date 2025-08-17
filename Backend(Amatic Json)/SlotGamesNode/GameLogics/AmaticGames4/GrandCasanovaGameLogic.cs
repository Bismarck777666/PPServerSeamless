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
   
    class GrandCasanovaGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GrandCasanova";
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
                return new long[] { 10, 20 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "052385088852664aac314419990777bbb0aaa99384419990777bbb0aaa999222344799525abb266c880bb1ba9993777aaa2220446255789a3bbbc1678889ab999666555222144538897709aac8886620777bb555099921f1a6344c8859bb277509a9990aaa6bbb52368508582aa6c34619479a79b7babca79a759b7babcab9a70b9ab7ba2354345b25b2b68c86089199737870aaac883870aaa728871870aaac21f404273bb7bc71880a81aa996956659921c1430a4a7ac8868627bb75b09559921a16344c889852775099abaa60bb030101010101010427101000112641410101010101010141401110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 2;
        #endregion

        public GrandCasanovaGameLogic()
        {
            _gameID     = GAMEID.GrandCasanova;
            GameName    = "GrandCasanova";
        }

        protected override int getLineTypeFromPlayLine(BaseAmaticSlotBetInfo betInfo)
        {
            switch (betInfo.PlayLine)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                default:
                    return -1;
            }
        }
    }
}
