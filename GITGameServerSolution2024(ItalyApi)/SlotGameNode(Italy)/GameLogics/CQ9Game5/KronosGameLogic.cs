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
   
    class KronosGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "154";
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
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 5000, 5, 10, 30 };
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
                return "Kronos";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Kronos\"}," +
                    "{\"lang\":\"ko\",\"name\":\"Kronos\"}," +
                    "{\"lang\":\"th\",\"name\":\"โครนอส\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"宙斯他爹\"}]";
            }
        }
        #endregion

        public KronosGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Kronos;
            GameName                = "Kronos";
        }
    }
}
