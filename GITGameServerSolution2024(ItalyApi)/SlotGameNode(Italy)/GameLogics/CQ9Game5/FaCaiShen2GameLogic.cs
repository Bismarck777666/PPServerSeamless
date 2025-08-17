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
   
    class FaCaiShen2GameLogic : BaseCQ9MultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "160";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 38;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 10 };
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
                return "Fa Cai Shen2";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Fa Cai Shen2\"}," +
                    "{\"lang\":\"ko\",\"name\":\"재물신 2\"}," +
                    "{\"lang\":\"th\",\"name\":\"ฟาชัยเซ็น 2\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"发财神2\"}]";
            }
        }
        protected override double[] MoreBetMultiples
        {
            get
            {
                return new double[] { 1.0, 58.0 / 38.0, 88.0 / 38.0 };
            }
        }

        protected override int _extraCount => 3;
        #endregion

        public FaCaiShen2GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.FaCaiShen2;
            GameName                = "FaCaiShen2";
        }
    }
}
