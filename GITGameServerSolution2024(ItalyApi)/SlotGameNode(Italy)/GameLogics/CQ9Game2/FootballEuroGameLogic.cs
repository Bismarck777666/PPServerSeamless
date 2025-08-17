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
   
    class FootballEuroGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "191";
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
                return "Football Euro";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"zh-cn\",\"name\":\"欧洲足球\"}," +
                    "{\"lang\":\"en\",\"name\":\"Football Euro\"}," +
                    "{\"lang\":\"ko\",\"name\":\"축구 유로\"}," +
                    "{\"lang\":\"th\",\"name\":\"ฟุตบอลยูโร\"}]";
            }
        }
        #endregion

        public FootballEuroGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FootballEuro;
            GameName                = "FootballEuro";
        }
    }
}
