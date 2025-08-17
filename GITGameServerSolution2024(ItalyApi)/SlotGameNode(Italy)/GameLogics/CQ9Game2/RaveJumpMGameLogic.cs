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
   
    class RaveJumpMGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "109";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 125, 250, 500, 750, 1250, 2500, 1, 2, 3, 5, 10, 30, 50, 80 };
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
                return "Rave Jump Mobile";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Rave Jump Mobile\"}," +
                    "{\"lang\":\"es\",\"name\":\"Salto rave Móvil\"}," +
                    "{\"lang\":\"id\",\"name\":\"Rave Jump Bergerak\"}," +
                    "{\"lang\":\"ja\",\"name\":\"レイヴ・ジャンプ モバイル\"}," +
                    "{\"lang\":\"ko\",\"name\":\"레이브 점프 모바일\"}," +
                    "{\"lang\":\"th\",\"name\":\"เรพจั๊ม M\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Rave Jump Mobile\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"单手跳起来\"}]";
            }
        }

        #endregion

        public RaveJumpMGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.RaveJumpM;
            GameName                = "RaveJumpM";
        }
    }
}
