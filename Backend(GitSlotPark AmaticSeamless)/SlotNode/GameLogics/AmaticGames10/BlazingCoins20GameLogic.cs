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
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
    class BlazingCoins20GameLogic : BaseAmaticSpecAnteGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BlazingCoins20";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "0525166664444444400055555553366334455441111175511556633333389a222224455bc224433224411624e622446611155222255556666111222233445576666111554422000444444555555333335589336251622662244666666664444441111144333333344783300022222555555336633665511336655911556251611111222222555555533333662244444442266666661133554466223355441155000446655784496254666662244222225522553333337896633441166334411111444444400055115555ab55554466c553355600301010101010104271010002142641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010102960f16030405080a0f16030405080a0f16030405080a0f16030405080a0f16030405080a0f";
            }
        }
        protected override string ExtraAntePurString => "10296";
        protected override string ExtraString => "0f16030405080a0f16030405080a0f16030405080a0f16030405080a0f16030405080a0f";
        protected override bool SupportMoreBet => true;
        protected override double MoreBetMultiple => 1.5;
        #endregion

        public BlazingCoins20GameLogic()
        {
            _gameID     = GAMEID.BlazingCoins20;
            GameName    = "BlazingCoins20";
        }
    }
}
