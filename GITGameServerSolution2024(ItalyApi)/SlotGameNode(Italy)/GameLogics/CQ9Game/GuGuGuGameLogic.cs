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
    class GuGuGuGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "15";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 15;
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
                return new int[] { 300, 600, 1250, 1800, 3000, 6000, 12500, 2, 4, 8, 10, 20, 30, 50, 100 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 12500;
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
                return "GuGuGu";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"GuGuGu\"}," +
                    "{\"lang\":\"es\",\"name\":\"GuGuGu\"}," +
                    "{\"lang\":\"id\",\"name\":\"GuGuGu\"}," +
                    "{\"lang\":\"ja\",\"name\":\"グググ\"}," +
                    "{\"lang\":\"ko\",\"name\":\"구구구\"}," +
                    "{\"lang\":\"th\",\"name\":\"กูกูกู\"}," +
                    "{\"lang\":\"vn\",\"name\":\"GuGuGu\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"咕咕咕\"}]";
            }
        }

        #endregion

        public GuGuGuGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.GuGuGu;
            GameName                = "GuGuGu";
        }
    }
}
