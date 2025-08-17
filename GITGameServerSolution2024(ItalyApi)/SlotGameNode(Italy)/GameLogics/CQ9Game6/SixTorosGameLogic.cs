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
   
    class SixTorosGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "173";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 20;
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
                return new int[] { 50, 100, 200, 400, 1000, 2000, 4000, 8000, 1, 2, 5, 10, 20, 30 };
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
                return "6 Toros";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"6 Toros\"}," +
                    "{\"lang\":\"ko\",\"name\":\"6 토로\"}," +
                    "{\"lang\":\"th\",\"name\":\"6 โทรอส\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"666\"}]";
            }
        }
        #endregion

        public SixTorosGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.SixToros;
            GameName                = "SixToros";
        }
    }
}
