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
    class DoubleFruitsGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DoubleFruits";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 48, 60, 80, 100, 120, 160, 200, 240, 300, 360, 400, 500, 600, 700, 800, 900, 1000, 2000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 5 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0524d6660436234446603355516334103323503654411175511556624422265206157241243224463324d62243661112332220525552666113324024157166116056456405544354375432563452335435251622662446662444111033361461144035453334732604522356255503616320356413566751351356250611122112225023655513333562532444326664566246135657436426325043154156045765465442506622446222556332532533376660431234166334411104441145605514625515455553475533545600301010101010104271010001131f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c8061010101010101300013000130001300013000";
            }
        }
        protected override string ExtraString => "1300013000130001300013000";
        #endregion

        public DoubleFruitsGameLogic()
        {
            _gameID     = GAMEID.DoubleFruits;
            GameName    = "DoubleFruits";
        }
    }
}
