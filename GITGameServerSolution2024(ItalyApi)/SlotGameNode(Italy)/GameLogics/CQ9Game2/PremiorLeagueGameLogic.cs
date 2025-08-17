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
   
    class PremiorLeagueGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "190";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 88;
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
                return new int[] { 30, 50, 100, 200, 300, 500, 1000, 1, 2, 3, 5, 10, 20 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1000;
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
                return "Premior League";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"zh-cn\",\"name\":\"超级联赛\"}," +
                    "{\"lang\":\"en\",\"name\":\"Premior League\"}," +
                    "{\"lang\":\"th\",\"name\":\"พรีเมียร์ลีก\"}," +
                    "{\"lang\":\"ja\",\"name\":\"プレミアリーグ\"}," +
                    "{\"lang\":\"ko\",\"name\":\"프리미오 리그\"}]";
            }
        }
        #endregion

        public PremiorLeagueGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.PremiorLeague;
            GameName                = "PremiorLeague";
        }
    }
}
