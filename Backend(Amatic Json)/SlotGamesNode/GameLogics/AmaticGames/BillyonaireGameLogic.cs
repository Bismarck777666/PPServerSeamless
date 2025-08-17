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
   
    class BillyonaireGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Billyonaire";
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
                return new long[] { 40 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052416066644773223322666767557755119773334444555577778833144492266888823207770044666644116663333aaaa77779555444411122228888238663363395550000445588005555444111155777119228822aaaa44882470001100022222aaaa77a77722221331113944433411aaaaa3336366446644555885588824305575574400440005511221122664404000113311333366676777924222433838885241606766447322332267666755775511977334434554577578873341944822668882320776700446664411666333aaaa577737545944411128228882238665336339550005044558800554441715115577119228822aaaa44882470001100022222aaaa77a72772221331113944433411aaaaa3336366446644558855888524301557574400544000551122012260644430011311333366676377792422243838880001010101010104c33c100121437d028101010111110102828281100101010101000000000000000002211121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f4042328427102910101010101010101010101010101010101010101010101010101010101010101010101010101010108ffffffff";
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

        public BillyonaireGameLogic()
        {
            _gameID     = GAMEID.Billyonaire;
            GameName    = "Billyonaire";
        }
    }
}
