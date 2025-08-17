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
   
    class OoGaChaKaGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "210";
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
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30, 50, 100 };
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
                return "Oo Ga Cha Ka";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Oo Ga Cha Ka\"}," +
                    "{\"lang\":\"ko\",\"name\":\"오가차\"}," +
                    "{\"lang\":\"th\",\"name\":\"อู กา ชา ก้า\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"呜嘎吓嘎\"}]";
            }
        }
        #endregion

        public OoGaChaKaGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.OoGaChaKa;
            GameName                = "OoGaChaKa";
        }
    }
}
