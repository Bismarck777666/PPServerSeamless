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
   
    class RunningToroGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "86";
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
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 8, 10, 30 };
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
                return "Running Toro";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Running Toro\"}," +
                    "{\"lang\":\"ko\",\"name\":\"러닝 토로\"}," +
                    "{\"lang\":\"th\",\"name\":\"เกมส์ราชากระทิงนําโชค\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"牛逼快跑\"}]";
            }
        }
        #endregion

        public RunningToroGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.RunningToro;
            GameName                = "RunningToro";
        }
    }
}
