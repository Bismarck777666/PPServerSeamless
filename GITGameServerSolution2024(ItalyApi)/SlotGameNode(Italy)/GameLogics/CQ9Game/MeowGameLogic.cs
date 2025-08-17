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
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
   
    class MeowGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "132";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 3, 5, 10, 20, 30 };
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
                return "Meow";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Meow\"}," +
                    "{\"lang\":\"ko\",\"name\":\"야옹\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"再喵一个\"}]";
            }
        }
        #endregion

        public MeowGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Meow;
            GameName                = "Meow";
        }

    }
}
