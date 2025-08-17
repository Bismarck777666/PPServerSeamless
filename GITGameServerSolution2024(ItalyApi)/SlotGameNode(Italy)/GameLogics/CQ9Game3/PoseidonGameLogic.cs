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
   
    class PoseidonGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "80";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 5, 10 };
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
                return "Poseidon";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Poseidon\"}," +
                    "{\"lang\":\"ko\",\"name\":\"포세이돈\"}," +
                    "{\"lang\":\"th\",\"name\":\"โพไซดอน\"}," +
                    "{\"lang\":\"vn\",\"name\":\"VUA THỦY TỀ\"}," +
                    "{\"lang\":\"id\",\"name\":\"Poseidon\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"传奇海神\"}]";
            }
        }

        #endregion

        public PoseidonGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Poseidon;
            GameName                = "Poseidon";
        }
    }
}
