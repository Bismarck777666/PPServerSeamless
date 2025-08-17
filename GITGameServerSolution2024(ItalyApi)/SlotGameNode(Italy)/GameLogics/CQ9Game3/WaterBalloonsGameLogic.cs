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
   
    class WaterBalloonsGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "41";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 60, 120, 250, 360, 600, 1200, 1, 2, 5, 10, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1200;
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
                return "WaterBalloons";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"WaterBalloons\"}," +
                    "{\"lang\":\"ko\",\"name\":\"워터볼룬즈\"}]";
            }
        }

        #endregion

        public WaterBalloonsGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.WaterBalloons;
            GameName                = "WaterBalloons";
        }
    }
}
