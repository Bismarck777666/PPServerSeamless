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
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
   
    class GoldStealerGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "130";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 40;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 5000, 1, 2, 5, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 5000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "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";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Gold Stealer";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Gold Stealer\"}," +
                    "{\"lang\":\"ko\",\"name\":\"황금 도둑\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Ladrão de Ouro\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"偷金妹子\"}]";
            }
        }
        #endregion

        public GoldStealerGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.GoldStealer;
            GameName                = "GoldStealer";
        }
    }
}
