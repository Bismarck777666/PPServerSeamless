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
   
    class MoveNJumpGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "138";
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
                return "Move n' Jump";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Move n' Jump\"}," +
                    "{\"lang\":\"es\",\"name\":\"Salta pa'ca\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Mover e pular\"}," +
                    "{\"lang\":\"ja\",\"name\":\"ムーブ＆ジャンプ\"}," +
                    "{\"lang\":\"ko\",\"name\":\"움직이고 점프하기\"}," +
                    "{\"lang\":\"th\",\"name\":\"มูฟแอนด์จัมป์\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"跳过来\"}]";
            }
        }

        #endregion

        public MoveNJumpGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.MoveNJump;
            GameName                = "MoveNJump";
        }
    }
}
