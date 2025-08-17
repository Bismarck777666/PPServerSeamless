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

namespace SlotGamesNode.GameLogics
{
   
    class NeZhaAdventGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "163";
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
                return "Ne Zha Advent";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Ne Zha Advent\"}," +
                    "{\"lang\":\"ko\",\"name\":\"나타\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"哪吒再临\"}]";
            }
        }
        #endregion

        public NeZhaAdventGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.NeZhaAdvent;
            GameName                = "NeZhaAdvent";
        }
    }
}
