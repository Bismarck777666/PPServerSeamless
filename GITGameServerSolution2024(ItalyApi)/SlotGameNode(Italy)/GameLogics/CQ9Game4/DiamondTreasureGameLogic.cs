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
   
    class DiamondTreasureGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "144";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 4, 5, 10 };
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
        protected override int FreeSpinTypeCount
        {
            get { return 3; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202 };
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Diamond Treasure";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Diamond Treasure\"}," +
                    "{\"lang\":\"ko\",\"name\":\"다이아몬드 트레져\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"钻更多\"}]";
            }
        }
        #endregion

        public DiamondTreasureGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.DiamondTreasure;
            GameName                = "DiamondTreasure";
        }
    }
}
