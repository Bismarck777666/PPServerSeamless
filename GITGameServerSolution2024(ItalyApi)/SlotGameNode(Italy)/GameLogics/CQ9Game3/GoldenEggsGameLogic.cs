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
   
    class GoldenEggsGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "67";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 80;
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
                return new int[] { 30, 50, 100, 200, 300, 500, 1000, 1, 2, 3, 5, 10, 20 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1000;
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
                return "Golden Eggs";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Golden Eggs\"}," +
                    "{\"lang\":\"ko\",\"name\":\"골든 에그\"}," +
                    "{\"lang\":\"th\",\"name\":\"ไข่ทองคำ\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"赚金蛋\"}]";
            }
        }

        #endregion

        public GoldenEggsGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.GoldenEggs;
            GameName                = "GoldenEggs";
        }
    }
}
