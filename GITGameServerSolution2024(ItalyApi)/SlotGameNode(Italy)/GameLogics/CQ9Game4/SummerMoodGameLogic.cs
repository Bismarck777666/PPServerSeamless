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
   
    class SummerMoodGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "59";
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
                return "Summer Mood";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Summer Mood\"}," +
                    "{\"lang\":\"ko\",\"name\":\"섬머 무드\"}," +
                    "{\"lang\":\"th\",\"name\":\"กอริลลาซัมเมอร์\"}," +
                    "{\"lang\":\"id\",\"name\":\"Gorilla Summer\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Humor de Verão\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Gorilla Summer\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"夏日猩情\"}]";
            }
        }
        #endregion

        public SummerMoodGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.SummerMood;
            GameName                = "SummerMood";
        }
    }
}
