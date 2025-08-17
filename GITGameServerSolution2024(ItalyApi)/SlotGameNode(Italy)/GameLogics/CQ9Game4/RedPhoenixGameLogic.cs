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
   
    class RedPhoenixGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "77";
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
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 4, 5, 10 };
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
                return "Red Phoenix";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Red Phoenix\"}," +
                    "{\"lang\":\"ko\",\"name\":\"레드 포닉스\"}," +
                    "{\"lang\":\"th\",\"name\":\"โฟนิกซ์สีแดง\"}," +
                    "{\"lang\":\"id\",\"name\":\"Red Phoenix\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Fênix Vermelha\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Phượng Hoàng Đỏ\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"火凤凰\"}]";
            }
        }
        #endregion

        public RedPhoenixGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.RedPhoenix;
            GameName                = "RedPhoenix";
        }
    }
}
