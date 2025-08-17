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
   
    class StrikerWildGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "231";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 25;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 1, 2, 4, 8, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 8000;
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
                return "Striker WILD";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Striker WILD\"}," +
                    "{\"lang\":\"ko\",\"name\":\"태극전사\"}," +
                    "{\"lang\":\"th\",\"name\":\"กองหน้าเพชฌฆาต\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"狂野前锋\"}]";
            }
        }
        #endregion

        public StrikerWildGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.StrikerWild;
            GameName                = "StrikerWild";
        }
    }
}
