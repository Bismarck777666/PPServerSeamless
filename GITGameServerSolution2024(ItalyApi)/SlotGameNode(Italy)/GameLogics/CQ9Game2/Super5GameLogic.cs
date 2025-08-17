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
   
    class Super5GameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "16";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 15;
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
                return new int[] { 300, 600, 1250, 1800, 3000, 6000, 2, 4, 10, 20, 30, 50, 100 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 6000;
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
                return "Super5";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"zh-cn\",\"name\":\"五行\"}," +
                    "{\"lang\":\"en\",\"name\":\"Super5\"}," +
                    "{\"lang\":\"ko\",\"name\":\"오행\"}," +
                    "{\"lang\":\"th\",\"name\":\"5 ธาตุสะท้านภพ\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Super5\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Super5\"}," +
                    "{\"lang\":\"id\",\"name\":\"Super5\"}]";
            }
        }
        #endregion

        public Super5GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Super5;
            GameName                = "Super5";
        }
    }
}
