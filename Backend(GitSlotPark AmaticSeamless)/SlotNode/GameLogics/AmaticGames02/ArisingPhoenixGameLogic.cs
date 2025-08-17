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
   
    class ArisingPhoenixGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "ArisingPhoenix";
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
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "05254000022221111333355554444666677778888773366225577445566443322776655111177660000554444258000011112222333344445555999966667777333344445566667766667777117755777711000077778888666626055550000111122223333444477776666222299994444555566774466557755225544552255558888444411113333000026c000033332222444455551111666677773333999922224444773377332233220033007777777744443333222211110000555588886666258111122220000333355556666444477772233663355444455776622771111444455556666888877770000111100301010101010104271010001a5186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d0331010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010100f15fffff15fffff15fffff15fffff";
            }
        }
        protected override int LineTypeCnt  => 5;
        protected override string ExtraString => "0f15fffff15fffff15fffff15fffff";
        #endregion

        public ArisingPhoenixGameLogic()
        {
            _gameID     = GAMEID.ArisingPhoenix;
            GameName    = "ArisingPhoenix";
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
