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
   
    class FootballJerseysGameLogic : BaseCQ9MultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "94";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 1;
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
                return new int[] { 1000, 2500, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 50, 100, 200, 300, 500 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 200000;
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
                return "Football Jerseys";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Football Jerseys\"}," +
                    "{\"lang\":\"ko\",\"name\":\"Football Jerseys\"}]";
            }
        }
        protected override double[] MoreBetMultiples
        {
            get
            {
                return new double[] { 1.0, 2.0, 3.0 };
            }
        }

        protected override int _extraCount => 3;
        #endregion

        public FootballJerseysGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.FootballJerseys;
            GameName                = "FootballJerseys";
        }
    }
}
