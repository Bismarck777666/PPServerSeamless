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
   
    class AcrobaticsGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "223";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 9;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 5, 10, 20, 30, 50, 100 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 20000;
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
                return "Acrobatics";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Acrobatics\"}," +
                    "{\"lang\":\"ko\",\"name\":\"곡예\"}," +
                    "{\"lang\":\"th\",\"name\":\"นักกายกรรม\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"百戏\"}]";
            }
        }
        #endregion

        public AcrobaticsGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Acrobatics;
            GameName                = "Acrobatics";
        }
    }
}
