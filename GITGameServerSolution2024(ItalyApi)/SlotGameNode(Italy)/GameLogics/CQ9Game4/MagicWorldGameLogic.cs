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
   
    class MagicWorldGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "27";
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
        protected override string CQ9GameName
        {
            get
            {
                return "MagicWorld";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Magic World\"}," +
                    "{\"lang\":\"ko\",\"name\":\"매직 월드\"}," +
                    "{\"lang\":\"th\",\"name\":\"เมจิกเวิลด์\"}," +
                    "{\"lang\":\"id\",\"name\":\"Dunia Sihir\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Mundo Mágico\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Thế giới phép thuật\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"魔法世界\"}]";
            }
        }
        #endregion

        public MagicWorldGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.MagicWorld;
            GameName                = "MagicWorld";
        }
    }
}
