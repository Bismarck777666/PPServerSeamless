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
   
    class NinjaRaccoonGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "214";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 3;
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
                return new int[] { 300, 500, 1000, 2000, 4000, 6000, 10000, 20000, 40000, 80, 100, 200 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 40000;
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
                return "Ninja Raccoon";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Ninja Raccoon\"}," +
                    "{\"lang\":\"ko\",\"name\":\"닌자 래큰\"}," +
                    "{\"lang\":\"th\",\"name\":\"นินจา แรคคูน\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"忍者浣熊\"}]";
            }
        }
        #endregion

        public NinjaRaccoonGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.NinjaRaccoon;
            GameName                = "NinjaRaccoon";
        }
    }
}
