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
   
    class DoubleDiamonds50GameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DoubleDiamonds50";
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
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0526f55556665511114443333300000333222555722444422557554444552225555566611111666660000066666557222223333344444555522227b55111666733344442266555336666111110000011115556676661166676666655111663333344222225551111144444000004444455555661166676666626755566674444400000444441133355522663334466644444466633355555444440000044444666666333336666111114466222222645555666331133334444400000444472255661166644422111666755555553333330000033333335511111444442222266666273555711666444433366622200000336611555221144444555666722222255555566444443336666600000666661111122222444555666733333300301010101010104271010001a5186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d033101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 5;
        #endregion

        public DoubleDiamonds50GameLogic()
        {
            _gameID                 = GAMEID.DoubleDiamonds50;
            GameName                = "DoubleDiamonds50";
        }

        protected override int getLineTypeFromPlayLine(int playline)
        {
            switch (playline)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                case 30:
                    return 2;
                case 40:
                    return 3;
                case 50:
                    return 4;
                default:
                    return 0;
            }
        }
    }
}
