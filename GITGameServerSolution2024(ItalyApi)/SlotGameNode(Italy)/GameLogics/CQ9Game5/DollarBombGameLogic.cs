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
   
    class DollarBombGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "218";
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
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 4, 5, 10 };
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
                return "Dollar Bomb";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Dollar Bomb\"}," +
                    "{\"lang\":\"ko\",\"name\":\"달러 봄브\"}," +
                    "{\"lang\":\"th\",\"name\":\"ดอลล่าร์ บอม\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"达拉崩吧\"}]";
            }
        }
        #endregion

        public DollarBombGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.DollarBomb;
            GameName                = "DollarBomb";
        }
    }
}
