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
    class WildVolcanoGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "WildVolcano";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8 ,9 ,10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 2000 };
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
                return "0523b070400090a0b060606060c0d0e040404040f100103060406070205010101010802020202030303030207050505050707070701020304050601030524300090a0b050505050c0d0e070707070f10020508040601070303030303010101010202020204040706060606020202060606050207070707010203040506070604030124400090a0b050505050c0d0e070707070f1001020305060407080606060603030303010101010304040404020202020405050504040407070701020302050607070406050123c00090a0b070707070c0d0e040404040f100104060203050701010101030303030805050505020202020606060605050502020207070706060604040423400090a0b060606060c0d0e070707070f1001020308040607030303030501010101050505050404040402020202030303060606060030101010101010427101000115186a02810101010101010282828110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0291010101010101010101010101010101010101010101010101010101010101010101010101010101010140f0f0f0f140f0f0f0f140f0f0f0f140f0f0f0f140f0f0f0f";
            }
        }

        protected override int ReelSetBitNum => 2;
        protected override string ExtraString => "140f0f0f0f140f0f0f0f140f0f0f0f140f0f0f0f140f0f0f0f";
        #endregion

        public WildVolcanoGameLogic()
        {
            _gameID     = GAMEID.WildVolcano;
            GameName    = "WildVolcano";
        }
    }
}
