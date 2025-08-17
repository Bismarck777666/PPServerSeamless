using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class GoldenFishGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string ExtraString => "0000000000";
        protected override string SymbolName
        {
            get
            {
                return "GoldenFish";
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
                return new long[] { 10 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "052355555666551141445430783333225555224464225445544545522223955111666633344242266555113366660782115556666661166666666622f5556664444078411133355522623334466644244466633322e5655666331144440783433222556611666444221126666237555511466444433356622078336661555221144444555666622222200301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b10101010101010101010100000000000";
            }
        }
        #endregion

        public GoldenFishGameLogic()
        {
            _gameID     = GAMEID.GoldenFish;
            GameName    = "GoldenFish";
        }
    }
}
