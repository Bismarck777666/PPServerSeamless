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
   
    class GoodFortuneGameLogic : BaseCQ9MultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "50";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 8;
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
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 5000, 5, 10, 20, 30 };
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
                return "Good Fortune";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Good Fortune\"}," +
                    "{\"lang\":\"ko\",\"name\":\"굿 포춘\"}," +
                    "{\"lang\":\"th\",\"name\":\"โชคดีมีชัย\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Boa Sorte\"}," +
                    "{\"lang\":\"es\",\"name\":\"Buena fortuna\"}," +
                    "{\"lang\":\"id\",\"name\":\"Nasib Baik\"}," +
                    "{\"lang\":\"ja\",\"name\":\"グッドフォーチュン\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Vận mệnh tốt\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"鸿福齐天\"}]";
            }
        }
        protected override double[] MoreBetMultiples
        {
            get
            {
                return new double[] { 1.0, 18.0 / 8.0, 38.0 / 8.0, 68.0 / 8.0, 88.0 / 8.0 };
            }
        }

        protected override int _extraCount => 5;
        #endregion

        public GoodFortuneGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.GoodFortune;
            GameName                = "GoodFortune";
        }
    }
}
