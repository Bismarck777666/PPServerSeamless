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
   
    class SuperDiamondsGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "62";
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
                return "Super Diamonds";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Super Diamonds\"}," +
                    "{\"lang\":\"ko\",\"name\":\"슈퍼 다이야몬드\"}]";
            }
        }

        #endregion

        public SuperDiamondsGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.SuperDiamonds;
            GameName                = "SuperDiamonds";
        }
    }
}
