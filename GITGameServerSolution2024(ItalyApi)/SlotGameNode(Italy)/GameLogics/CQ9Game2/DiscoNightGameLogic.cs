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
   
    class DiscoNightGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "205";
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
                return "DiscoNight";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"DiscoNight\"}," +
                    "{\"lang\":\"es\",\"name\":\"Noche de discoteca\"}," +
                    "{\"lang\":\"ja\",\"name\":\"ディスコナイト\"}," +
                    "{\"lang\":\"ko\",\"name\":\"디스코나이트\"}," +
                    "{\"lang\":\"th\",\"name\":\"เกมส์ดิสโก้ไนท์\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"蹦迪\"}]";
            }
        }

        #endregion

        public DiscoNightGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.DiscoNight;
            GameName                = "DiscoNight";
        }
    }
}
