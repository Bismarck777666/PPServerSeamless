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
   
    class WanbaoDinoGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "70";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 25;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 1, 2, 4, 8, 10, 30 };
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
                return "Wanbao Dino";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Wanbao Dino\"}," +
                    "{\"lang\":\"ko\",\"name\":\"완바오 다이너소어\"}," +
                    "{\"lang\":\"th\",\"name\":\"ไดโนหวั่นเบ๋า\"}," +
                    "{\"lang\":\"id\",\"name\":\"Wanbao Dino\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Wanbao Dino\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Wanbao Dino\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"万饱龙\"}]";
            }
        }
        #endregion

        public WanbaoDinoGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.WanbaoDino;
            GameName                = "WanbaoDino";
        }
    }
}
