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
   
    class ZeusMGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "125";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 88;
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
                return new int[] { 50, 100, 200, 300, 500, 1000, 2000, 1, 2, 3, 5, 10, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 2000;
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
                return "Zeus M";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"zh-cn\",\"name\":\"直式宙斯\"}," +
                    "{\"lang\":\"en\",\"name\":\"Zeus M\"}," +
                    "{\"lang\":\"th\",\"name\":\"เทพซีอุส M\"}," +
                    "{\"lang\":\"ko\",\"name\":\"제우스 M\"}]";
            }
        }
        #endregion

        public ZeusMGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.ZeusM;
            GameName                = "ZeusM";
        }
    }
}
