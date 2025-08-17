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
   
    class HerculesGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "161";
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
                return new int[] { 20, 30, 50, 100, 200, 300, 500, 1000, 2000, 1, 2, 3, 5, 10 };
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
                return "Hercules";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Hercules\"}," +
                    "{\"lang\":\"ko\",\"name\":\"Hercules\"}," +
                    "{\"lang\":\"th\",\"name\":\"เฮอร์คิวลิส\"}," +
                    "{\"lang\":\"es\",\"name\":\"Hercules\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"大力神\"}]";
            }
        }
        #endregion

        public HerculesGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Hercules;
            GameName                = "Hercules";
        }
    }
}
