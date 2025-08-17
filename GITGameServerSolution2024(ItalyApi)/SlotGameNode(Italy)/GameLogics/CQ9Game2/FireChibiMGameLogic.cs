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
   
    class FireChibiMGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "139";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 60;
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
                return new int[] { 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 3, 5, 10, 20, 30 };
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
                return "FireChibi M";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"zh-cn\",\"name\":\"直式火烧连环船\"}," +
                    "{\"lang\":\"en\",\"name\":\"Fire Chibi M\"}," +
                    "{\"lang\":\"ko\",\"name\":\"파이어 치비 M\"}," +
                    "{\"lang\":\"th\",\"name\":\"เปลวไฟแห่งความร่ำรวย M\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Ngọn lửa Chibi M\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Chibi de Fogo M\"}," +
                    "{\"lang\":\"id\",\"name\":\"Chibi Api\"}]";
            }
        }
        #endregion

        public FireChibiMGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FireChibiM;
            GameName                = "FireChibiM";
        }
    }
}
