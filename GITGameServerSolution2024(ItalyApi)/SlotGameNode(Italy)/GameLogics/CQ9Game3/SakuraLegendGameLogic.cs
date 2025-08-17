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
   
    class SakuraLegendGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "13";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 5, 10 };
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
                return "SakuraLegend";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Sakura Legend\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Lenda Sakura\"}," +
                    "{\"lang\":\"ko\",\"name\":\"사쿠라 레전드\"}," +
                    "{\"lang\":\"th\",\"name\":\"ตำนานซากุระ\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Truyền thuyết Sakura\"}," +
                    "{\"lang\":\"id\",\"name\":\"Legenda Sakura\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"樱花妹子\"}]";
            }
        }

        #endregion

        public SakuraLegendGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.SakuraLegend;
            GameName                = "SakuraLegend";
        }
    }
}
