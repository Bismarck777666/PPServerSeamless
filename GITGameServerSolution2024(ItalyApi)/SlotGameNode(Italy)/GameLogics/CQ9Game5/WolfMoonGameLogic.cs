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
   
    class WolfMoonGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "46";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 40;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 5, 10 };
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
                return "Wolf Moon";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Wolf Moon\"}," +
                    "{\"lang\":\"ko\",\"name\":\"울프 문\"}," +
                    "{\"lang\":\"th\",\"name\":\"ดวงจันทร์หมาป่า\"}," +
                    "{\"lang\":\"id\",\"name\":\"Bulan Serigala\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Lobo da Lua\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Trăng sói\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"狼月\"}]";
            }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        protected override double[] MoreBetMultiples
        {
            get 
            {
                return new double[] { 1, 1.25 };
            }
        }
        #endregion

        public WolfMoonGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.WolfMoon;
            GameName                = "WolfMoon";
        }
    }
}
