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
   
    class WolfDiscoGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "183";
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
                return "WolfDisco";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"WolfDisco\"}," +
                    "{\"lang\":\"ko\",\"name\":\"늑대 디스코\"}," +
                    "{\"lang\":\"th\",\"name\":\"วูฟดิสโก้\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"野狼Disco\"}]";
            }
        }

        #endregion

        public WolfDiscoGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.WolfDisco;
            GameName                = "WolfDisco";
        }
    }
}
