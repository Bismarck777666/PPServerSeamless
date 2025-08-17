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
   
    class DiscoNightMGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "137";
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
                return "Disco Night M";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Disco Night M\"}," +
                    "{\"lang\":\"es\",\"name\":\"Noche de discoteca M\"}," +
                    "{\"lang\":\"ja\",\"name\":\"ディスコナイト M\"}," +
                    "{\"lang\":\"ko\",\"name\":\"디스코나이트 M\"}," +
                    "{\"lang\":\"th\",\"name\":\"ดิสโก้ไนท์ M\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"直式蹦迪\"}]";
            }
        }

        #endregion

        public DiscoNightMGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.DiscoNightM;
            GameName                = "DiscoNightM";
        }
    }
}
