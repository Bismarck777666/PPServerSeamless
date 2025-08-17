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
   
    class TreasureHouseGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "12";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 30;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 120, 250, 500, 750, 1250, 2500, 5000, 1, 2, 4, 5, 8, 10, 30 };
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
                return "Treasure House";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Treasure House\"}," +
                    "{\"lang\":\"ko\",\"name\":\"트래져 하우스\"}," +
                    "{\"lang\":\"th\",\"name\":\"บ้านขุมทรัพย์\"}," +
                    "{\"lang\":\"id\",\"name\":\"Rumah Harta Karun\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Tesouraria\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Kho báu\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"金玉满堂\"}]";
            }
        }
        #endregion

        public TreasureHouseGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.TreasureHouse;
            GameName                = "TreasureHouse";
        }
    }
}
