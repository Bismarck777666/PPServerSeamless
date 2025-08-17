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
   
    class JumpHighGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "52";
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
                return "Jump High";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Jump High\"}," +
                    "{\"lang\":\"es\",\"name\":\"Saltar alta\"}," +
                    "{\"lang\":\"id\",\"name\":\"Lompat tinggi\"}," +
                    "{\"lang\":\"ja\",\"name\":\"ジャンプハイ\"}," +
                    "{\"lang\":\"ko\",\"name\":\"점프 하이\"}," +
                    "{\"lang\":\"th\",\"name\":\"กระโดดสูง\"}," +
                    "{\"lang\":\"vn\",\"name\":\"nhảy cao\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"跳高\"}]";
            }
        }

        #endregion

        public JumpHighGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.JumpHigh;
            GameName                = "JumpHigh";
        }
    }
}
