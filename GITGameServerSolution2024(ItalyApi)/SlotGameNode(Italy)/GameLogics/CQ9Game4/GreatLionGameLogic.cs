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
   
    class GreatLionGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "17";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 20;
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
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 1, 2, 5, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 8000;
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
                return "Great Lion";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Great Lion\"}," +
                    "{\"lang\":\"ko\",\"name\":\"그레이트 라이언\"}," +
                    "{\"lang\":\"th\",\"name\":\"สิงโตให้มงคล\"}," +
                    "{\"lang\":\"id\",\"name\":\"Singa Besar\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Grande Leão\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Sư tử lớn\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"祥狮献瑞\"}]";
            }
        }
        #endregion

        public GreatLionGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.GreatLion;
            GameName                = "GreatLion";
        }
    }
}
