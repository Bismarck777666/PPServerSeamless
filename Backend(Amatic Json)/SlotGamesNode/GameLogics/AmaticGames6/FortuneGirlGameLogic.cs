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
   
    class FortuneGirlGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "FortuneGirl";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 3, 4, 5, 6, 8, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "0521e265a08b7a61a578268c98364a79a4922ba63981794b2947b38b28647047acb27957947a189b522ba39826b19a80a27817592649a165b63a9563a8bc8a422ba0835b294b95b74ac4a9165b47a083692816584b93722ba367184b93792b47b176072849a1bc1b2b561971b95521c7695a08b7691659a268c98364981222a47067acb482b59737acb268394a57a182222a436a80a2691759274b358bc8a1b748bc8222a478914ac7583b7a0836826185924ac75b222a57607284b65a3671bc4963859281bc4920301010101010104271010001131f40a101010101010100a0a0a1100101010101000000000000000001311121314151a1f21421921e22322822d23223c24625025a2640b1010101010101010101010";
            }
        }
        #endregion

        public FortuneGirlGameLogic()
        {
            _gameID     = GAMEID.FortuneGirl;
            GameName    = "FortuneGirl";
        }
    }
}
