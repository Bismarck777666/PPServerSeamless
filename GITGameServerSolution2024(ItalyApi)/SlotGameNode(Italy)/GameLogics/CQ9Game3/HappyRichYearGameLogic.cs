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

namespace SlotGamesNode.GameLogics
{
   
    class HappyRichYearGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "72";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 30;
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
                return new int[] { 80, 125, 250, 500, 750, 1250, 2500, 1, 2, 5, 10, 30, 50 };
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
                return "HappyRichYear";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Happy Rich Year\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Feliz ano rico\"}," +
                    "{\"lang\":\"ko\",\"name\":\"해피 리치 이어\"}," +
                    "{\"lang\":\"th\",\"name\":\"เกมเศรษฐี\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"好运年年\"}]";
            }
        }

        #endregion

        public HappyRichYearGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.HappyRichYear;
            GameName                = "HappyRichYear";
        }
    }
}
