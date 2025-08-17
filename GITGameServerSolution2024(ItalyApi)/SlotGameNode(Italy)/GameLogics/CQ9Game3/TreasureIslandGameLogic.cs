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
   
    class TreasureIslandGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "81";
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
                return "Treasure Island";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Treasure Island\"}," +
                    "{\"lang\":\"ko\",\"name\":\"보물섬\"}," +
                    "{\"lang\":\"th\",\"name\":\"เทรเชอร์ ไอส์แลนด์\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"金银岛\"}]";
            }
        }

        #endregion

        public TreasureIslandGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.TreasureIsland;
            GameName                = "TreasureIsland";
        }
    }
}
