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
   
    class TreasureBowlGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "74";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 88;
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
                return new int[] { 30, 50, 100, 200, 300, 500, 1000, 1, 2, 3, 5, 10, 20 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1000;
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
                return "TreasureBowl";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Treasure Bowl\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Tigela do Tesouro\"}," +
                    "{\"lang\":\"ko\",\"name\":\"트레저 보울\"}," +
                    "{\"lang\":\"th\",\"name\":\"กระถางมหาสมบัติ\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"聚宝盆\"}]";
            }
        }

        #endregion

        public TreasureBowlGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.TreasureBowl;
            GameName                = "TreasureBowl";
        }
    }
}
