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
   
    class KingKongShakeGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "219";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 5;
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
                return new int[] { 300, 500, 1000, 2000, 4000, 6000, 10000, 20000, 40000, 40, 100, 200 };
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
                return "King Kong Shake";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"King Kong Shake\"}," +
                    "{\"lang\":\"ko\",\"name\":\"King Kong Shake\"}," +
                    "{\"lang\":\"th\",\"name\":\"คิงคอง เชค\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"金猩特调\"}]";
            }
        }
        #endregion

        public KingKongShakeGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.KingKongShake;
            GameName                = "KingKongShake";
        }
    }
}
