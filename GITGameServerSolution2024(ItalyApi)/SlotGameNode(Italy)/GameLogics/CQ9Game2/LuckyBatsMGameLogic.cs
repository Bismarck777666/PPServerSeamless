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
   
    class LuckyBatsMGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "123";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 55;
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
                return new int[] { 60, 120, 250, 360, 600, 1200, 1, 2, 4, 10, 20, 30 };
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
                return "Lucky Bats M";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Lucky Bats M\"}," +
                    "{\"lang\":\"ko\",\"name\":\"럭키 뱃 M\"}," +
                    "{\"lang\":\"th\",\"name\":\"ลัคกี้ แบท M\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Morcegos de Sorte M\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"直式五福临门\"}]";
            }
        }
        #endregion

        public LuckyBatsMGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.LuckyBatsM;
            GameName                = "LuckyBatsM";
        }
    }
}
