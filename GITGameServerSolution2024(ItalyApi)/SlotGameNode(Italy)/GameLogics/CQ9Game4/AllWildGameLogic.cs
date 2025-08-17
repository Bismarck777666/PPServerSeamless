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
   
    class AllWildGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "38";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 5000, 1, 2, 5, 10, 30 };
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
                return "All Wilds";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"All Wilds\"}," +
                    "{\"lang\":\"ko\",\"name\":\"올 와일드\"}," +
                    "{\"lang\":\"th\",\"name\":\"ป่าทั้งหมด\"}," +
                    "{\"lang\":\"id\",\"name\":\"Semua Binatang Liar\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Todos os loucos\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Tất cả vùng hoang dã\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"舞力全开\"}]";
            }
        }
        #endregion

        public AllWildGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.AllWild;
            GameName                = "AllWild";
        }
    }
}
