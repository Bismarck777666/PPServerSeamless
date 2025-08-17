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
   
    class DoubleFlyGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "152";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 25;
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
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 8, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 8000;
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
                return "Double Fly";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Double Fly\"}," +
                    "{\"lang\":\"ko\",\"name\":\"Double Fly\"}," +
                    "{\"lang\":\"th\",\"name\":\"ดับเบิลฟลาย\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"双飞\"}]";
            }
        }
        #endregion

        public DoubleFlyGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.DoubleFly;
            GameName                = "DoubleFly";
        }
    }
}
