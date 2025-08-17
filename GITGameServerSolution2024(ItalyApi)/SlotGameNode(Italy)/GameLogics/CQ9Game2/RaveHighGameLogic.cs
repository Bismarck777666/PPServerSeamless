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
   
    class RaveHighGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "203";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 30;
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
                return new int[] { 80, 125, 250, 500, 750, 1250, 2500, 1, 2, 5, 10, 30, 50 };
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
                return "Rave High";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Rave High\"}," +
                    "{\"lang\":\"es\",\"name\":\"Rave High\"}," +
                    "{\"lang\":\"ko\",\"name\":\"레이브 하이\"}," +
                    "{\"lang\":\"th\",\"name\":\"มันส์ๆหน่อยเซ่\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"嗨起来\"}]";
            }
        }

        #endregion

        public RaveHighGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.RaveHigh;
            GameName                = "RaveHigh";
        }
    }
}
