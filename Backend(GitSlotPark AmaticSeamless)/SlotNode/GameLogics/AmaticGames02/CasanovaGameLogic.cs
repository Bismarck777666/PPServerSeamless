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
   
    class CasanovaGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Casanova";
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
                return new long[] { 20 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052205088852667aac38441999777bbbaaa99222344799525abb266c880bb1ba9993777aaa2220446255789a3bbbc1678889ab999666555221144538897709aac888662777bb555099921e1a6344c8859bb277509a999aaa6bbb521d8508582aa6c34619479a79b7babca2214345b25b2b68c860891909737870aaac821f404273bb7bc71880a8aa99c6956659921c1430a4a7ac8868627bb75b09559921a16344c889852775099abaa60bb0001010101010104ade8100121437d014101010111110101414141100101010101000000000000000002211121314151a1f21421921e22322822d23223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f40423284271015101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public CasanovaGameLogic()
        {
            _gameID                 = GAMEID.Casanova;
            GameName                = "Casanova";
        }
    }
}
