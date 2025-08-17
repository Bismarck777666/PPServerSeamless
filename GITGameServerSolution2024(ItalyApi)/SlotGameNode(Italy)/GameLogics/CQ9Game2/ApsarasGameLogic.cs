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
   
    class ApsarasGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "39";
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
                return new int[] { 60, 120, 250, 360, 600, 1200, 1, 2, 4, 10, 20, 30 };
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
                return "Apsaras";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Apsaras\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Apsaras\"}," +
                    "{\"lang\":\"ko\",\"name\":\"압사라스\"}," +
                    "{\"lang\":\"th\",\"name\":\"นางอัปสร\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"飞天\"}]";
            }
        }

        #endregion

        public ApsarasGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Apsaras;
            GameName                = "Apsaras";
        }
    }
}
