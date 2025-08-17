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
   
    class FunnyAlpacaGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "54";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 5;
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
                return new int[] { 100, 200, 400, 600, 1000, 2000, 4000, 4, 10, 20, 30, 50 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 4000;
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
                return "FunnyAlpaca";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"zh-cn\",\"name\": \"火草泥马\"}," +
                    "{\"lang\": \"en\",\"name\": \"Funny Alpaca\"}," +
                    "{\"lang\": \"ko\",\"name\": \"퍼니 알파카\"}," +
                    "{\"lang\": \"th\",\"name\": \"อัลปากาสุขสันต์\"}," +
                    "{\"lang\": \"vn\",\"name\": \"LẠC ĐÀ NGỘ NGHĨNH\"}," +
                    "{\"lang\": \"id\",\"name\": \"FUNNY ALPACA\"}]";
            }
        }
        #endregion

        public FunnyAlpacaGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FunnyAlpaca;
            GameName                = "FunnyAlpaca";
        }
    }
}
