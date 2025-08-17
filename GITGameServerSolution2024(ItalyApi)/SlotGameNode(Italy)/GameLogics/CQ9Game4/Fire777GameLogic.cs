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
   
    class Fire777GameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "66";
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
                return "Fire 777";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Fire 777\"}," +
                    "{\"lang\":\"ko\",\"name\":\"파이어 777\"}," +
                    "{\"lang\":\"th\",\"name\":\"กงล้อเพลิง 777\"}," +
                    "{\"lang\":\"id\",\"name\":\"Fire 777\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Fogo 777\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Hỏa hoạn 777\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"火爆777\"}]";
            }
        }
        #endregion

        public Fire777GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Fire777;
            GameName                = "Fire777";
        }
    }
}
