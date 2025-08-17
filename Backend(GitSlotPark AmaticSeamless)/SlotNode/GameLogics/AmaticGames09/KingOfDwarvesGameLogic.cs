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
   
    class KingOfDwarvesGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "KingOfDwarves";
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
                return "0522861425607881456798827365348887707668886662271534516078a0345678a4569784242554a54427722214956087a571462845367280883866538822344156a7081562278a453707389888aa177722d4715506708346978aaa451683882281144227797773775223614256078814567988273653488770766882261534516078a0345a678a45697842254a527aaa22414956087a5714628a453672803665388aaa822644156a7081562278a453707389888aa1777aaa22b4715506708346978aaa4516838822811442277977720301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b101010101010101010101014ffff15fffff15fffff15fffff";
            }
        }
        protected override string ExtraString => "14ffff15fffff15fffff15fffff";
        #endregion

        public KingOfDwarvesGameLogic()
        {
            _gameID     = GAMEID.KingOfDwarves;
            GameName    = "KingOfDwarves";
        }
    }
}
