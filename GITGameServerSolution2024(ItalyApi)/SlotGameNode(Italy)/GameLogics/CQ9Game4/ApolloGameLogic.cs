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
   
    class ApolloGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "78";
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
                return "Apollo";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Apollo\"}," +
                    "{\"lang\":\"ko\",\"name\":\"아폴로\"}," +
                    "{\"lang\":\"th\",\"name\":\"อพอลโล\"}," +
                    "{\"lang\":\"id\",\"name\":\"Apollo\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Apollo\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"阿波罗\"}]";
            }
        }
        #endregion

        public ApolloGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Apollo;
            GameName                = "Apollo";
        }
    }
}
