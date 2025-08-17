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
   
    class RaveJump2MGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "121";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 1, 2, 5, 10, 30 };
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
                return "Rave Jump 2 M";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Rave Jump 2 M\"}," +
                    "{\"lang\":\"es\",\"name\":\"Rave Jump 2 M\"}," +
                    "{\"lang\":\"id\",\"name\":\"Rave Jump 2 M\"}," +
                    "{\"lang\":\"ko\",\"name\":\"레이브 점프 2 M\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"直式跳起来2\"}]";
            }
        }

        #endregion

        public RaveJump2MGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.RaveJump2M;
            GameName                = "RaveJump2M";
        }
    }
}
