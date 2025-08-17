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
   
    class TheCupidsGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "209";
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
                return "The Cupids";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"The Cupids\"}," +
                    "{\"lang\":\"ko\",\"name\":\"The Cupids\"}," +
                    "{\"lang\":\"th\",\"name\":\"คิวปิด\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"小爱神\"}]";
            }
        }
        #endregion

        public TheCupidsGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.TheCupids;
            GameName                = "TheCupids";
        }
    }
}
