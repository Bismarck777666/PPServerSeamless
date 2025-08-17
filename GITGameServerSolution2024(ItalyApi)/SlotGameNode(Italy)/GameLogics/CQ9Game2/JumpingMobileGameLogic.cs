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
   
    class JumpingMobileGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "105";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };

            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 60, 120, 250, 360, 600, 1200, 1, 2, 5, 10, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1200;
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
                return "Jumping Mobile";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"zh-cn\",\"name\":\"单手跳高高\"}," +
                    "{\"lang\":\"en\",\"name\":\"Jumping Mobile\"}," +
                    "{\"lang\":\"ko\",\"name\":\"점핑 모바일\"}," +
                    "{\"lang\":\"th\",\"name\":\"กระโดดสูง M\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Jumping Mobile\"}," +
                    "{\"lang\":\"es\",\"name\":\"Salto Móvil\"}," +
                    "{\"lang\":\"ja\",\"name\":\"ジャンピング モバイル\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Pulo Mobile\"}," +
                    "{\"lang\":\"id\",\"name\":\"Lompat Tinggi Bergerak\"}]";
            }
        }
        #endregion

        public JumpingMobileGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.JumpingMobile;
            GameName                = "JumpingMobile";
        }
    }
}
