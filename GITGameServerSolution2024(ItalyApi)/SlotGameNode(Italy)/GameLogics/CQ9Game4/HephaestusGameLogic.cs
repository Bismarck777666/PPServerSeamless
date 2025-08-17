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
   
    class HephaestusGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "142";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 50;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 5, 10 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 2500;
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
                return "Hephaestus";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Hephaestus\"}," +
                    "{\"lang\":\"ko\",\"name\":\"헤파이토스\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"火神\"}]";
            }
        }

        #endregion

        public HephaestusGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Hephaestus;
            GameName                = "Hephaestus";
        }
    }
}
