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
   
    class SweetPopGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "206";
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
                return "Sweet POP";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Sweet POP\"}," +
                    "{\"lang\":\"ko\",\"name\":\"스위트 팝\"}," +
                    "{\"lang\":\"th\",\"name\":\"ป๊อป หวาน\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"甜蜜暴击\"}]";
            }
        }
        #endregion

        public SweetPopGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.SweetPop;
            GameName                = "SweetPop";
        }
    }
}
