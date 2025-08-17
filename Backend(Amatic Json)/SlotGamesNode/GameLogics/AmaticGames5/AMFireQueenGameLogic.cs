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
   
    class AMFireQueenGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "FireQueen";
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
                return new long[] { 10, 20 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "05225245678a94567918937809465203263187014522518973a4568789109280936425457371bcd62622245268a798089176847592336bcd9302145225345678a4925782098691367345bcd9821674521e2546879a564378908918746576912352242478a9456718a93780946952a03296a3175722118734568891092809364254573716266821e45268a798089a176847a592336bcd921f345678492578209869136734598216722325a46879a564a378908a91807465a769123030101010101010427101000112641410101010101010141401110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010150000015000001500000";
            }
        }
        protected override int LineTypeCnt => 2;
        protected override string ExtraString => "150000015000001500000";
        #endregion

        public AMFireQueenGameLogic()
        {
            _gameID     = GAMEID.AMFireQueen;
            GameName    = "FireQueen";
        }

        protected override int getLineTypeFromPlayLine(BaseAmaticSlotBetInfo betInfo)
        {
            switch (betInfo.PlayLine)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                default:
                    return -1;
            }
        }
    }
}
