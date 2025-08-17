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
   
    class HotFruits20GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotFruits20";
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
                return new long[] { 20 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0524d6666444444440005555555336633445544111117551155663333332222244557224433224411624d6224466111552222555566661112222334455766661115544220004444445555553333355733625062266224466666666444444111114433333334473300022222555555336633665511336655711556250611111222222555555533333662244444442266666661133554466223355441155000446655744762506666622442222255225533333376633441166334411111444444400055115555555544667553355600001010101010104b96e100121437d014101010111110101414141100101010101000000000000000001611121314151a1c1e21021221421e22823223c2502642c8312c319031f433e815101010101010101010101010101010101010101010";
            }
        }

        protected override int LineTypeCnt
        {
            get
            {
                return 1;
            }
        }
        #endregion

        public HotFruits20GameLogic()
        {
            _gameID     = GAMEID.HotFruits20;
            GameName    = "HotFruits20";
        }
    }
}
