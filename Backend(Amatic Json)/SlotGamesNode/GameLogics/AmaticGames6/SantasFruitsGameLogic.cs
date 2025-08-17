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
   
    class SantasFruitsGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SantasFruits";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
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
                return "052c1aaaa5515550000aaaa49999666632999923888181866664aaaa3999946666477773a3aaaa7777399994aaa277745555177700000666646466638884999393aaa177772aaa4666188828299991915553aaaaa4a4737394999924555388832888312d255552aaaa25555000099993a3aaa466600000999177726266638883866454555188817772746663777272728889939993aaa3a3a47774745552666161999377773aaa00000888399935553277748883833666313555499990000aaa277735554aaaa46644777438882281888181aaa31388841341777000064388824299938881245554541488818143138881515552484888000064aaa41488828232777199941488831383134188824142c35555232888000007773213288828239323999000006662366347777434929990000999321238881a1aaa42aaa3243aaa1a155523a3aaa0000888232663636632aaa32929990000aaa414329991888323237770000009993213218884317177747432bf9990000066615552aaa277726266660000088884aaa4a4155541477713231666239994214777314888832425552399900000005551999426636238881aaa1555aaa2a42999499466662455513777484888346663737771a1aaa24777313888300301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010";
            }
        }
        #endregion

        public SantasFruitsGameLogic()
        {
            _gameID     = GAMEID.SantasFruits;
            GameName    = "SantasFruits";
        }
    }
}
