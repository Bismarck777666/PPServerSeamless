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
   
    class MrMiserGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "225";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 20;
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
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 1, 2, 5, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 8000;
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
                return "MrMiser";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Mr. Miser\"}," +
                    "{\"lang\":\"ko\",\"name\":\"재물광 요정\"}," +
                    "{\"lang\":\"th\",\"name\":\"ภูติจิ๋วเฝ้าทรัพย์\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"财迷小妖\"}]";
            }
        }
        #endregion

        public MrMiserGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.MrMiser;
            GameName                = "MrMiser";
        }
    }
}
