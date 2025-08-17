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
    class FieryFruitsGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "FieryFruits";
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
                return "0524b11116663335550000555511144422224444004441116666555333300033366622255544422224b33335553334440000444466622200222266661115555111155500055533344411166622244424b00004443335553333444422200666111166661112222555500055544433366611155522244424b44400005551111444333366662220066633344441115555222200055544433366611155522224b3333444333555000044442220066611116666111555522225550004443336661115552224440030101010101010427101000115186a02810101010101010282828110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d02910101010101010101010101010101010101010101010101010101010101010101010101010101010100114ffff14ffff14ffff14ffff14ffff";
            }
        }

        protected override string ExtraString => "0114ffff14ffff14ffff14ffff14ffff";
        #endregion

        public FieryFruitsGameLogic()
        {
            _gameID     = GAMEID.FieryFruits;
            GameName    = "FieryFruits";
        }
    }
}
