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
   
    class SoSweetGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "8";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };

            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 100, 200, 400, 800, 1200, 2000, 4000, 1, 2, 4, 8, 10, 30, 50 };
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
                return "So Sweet";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"zh-cn\",\"name\":\"甜蜜蜜\"}," +
                    "{\"lang\":\"en\",\"name\":\"So Sweet\"}," +
                    "{\"lang\":\"ko\",\"name\":\"소 스위트\"}," +
                    "{\"lang\":\"th\",\"name\":\"เถี่ยนมี่มี่\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Thật ngọt ngào\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Tão Doce\"}," +
                    "{\"lang\":\"id\",\"name\":\"Manis Sekali\"}]";
            }
        }
        #endregion

        public SoSweetGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.SoSweet;
            GameName                = "SoSweet";
        }
    }
}
