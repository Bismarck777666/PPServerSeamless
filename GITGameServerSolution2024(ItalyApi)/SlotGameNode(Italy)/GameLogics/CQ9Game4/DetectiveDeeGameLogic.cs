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
   
    class DetectiveDeeGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "32";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 10;
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
                return new int[] { 50, 100, 300, 600, 1250, 1800, 3000, 6000, 12500, 5, 10, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 12500;
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
                return "Detective Dee";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Detective Dee\"}," +
                    "{\"lang\":\"ko\",\"name\":\"명탐정 디\"}," +
                    "{\"lang\":\"th\",\"name\":\"นักสืบคดี\"}," +
                    "{\"lang\":\"id\",\"name\":\"Dee Detektif\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Detetive Dee\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Thám tử Dee\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"通天神探狄仁杰\"}]";
            }
        }
        #endregion

        public DetectiveDeeGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.DetectiveDee;
            GameName                = "DetectiveDee";
        }
    }
}
