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
   
    class ZumaWildGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "122";
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
                return "Zuma Wild";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Zuma Wild\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Zuma Louco\"}," +
                    "{\"lang\":\"ko\",\"name\":\"캔디 주마\"}," +
                    "{\"lang\":\"es\",\"name\":\"Comodín Zuma\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"印加祖玛\"}]";
            }
        }

        #endregion

        public ZumaWildGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.ZumaWild;
            GameName                = "ZumaWild";
        }
    }
}
