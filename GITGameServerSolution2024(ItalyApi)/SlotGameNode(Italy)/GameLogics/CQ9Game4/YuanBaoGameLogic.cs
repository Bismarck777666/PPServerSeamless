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
   
    class YuanBaoGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "23";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 16;
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
                return new int[] { 50, 100, 300, 600, 1250, 1800, 3000, 6000, 12500, 5, 10, 15, 20, 30 };
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
                return "Yuan Bao";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Yuan Bao\"}," +
                    "{\"lang\":\"ko\",\"name\":\"금원보\"}," +
                    "{\"lang\":\"th\",\"name\":\"หยวนเบา\"}," +
                    "{\"lang\":\"id\",\"name\":\"Yuan Bao\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Yuan Bao\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Yuan Bao\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"金元宝\"}]";
            }
        }
        #endregion

        public YuanBaoGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.YuanBao;
            GameName                = "YuanBao";
        }
    }
}
