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
   
    class AllStarTeamGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "98";
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
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 5000, 8, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 5000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "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";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 5; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204 };
        }
        protected override string CQ9GameName
        {
            get
            {
                return "All Star Team";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"All Star Team\"}," +
                    "{\"lang\":\"id\",\"name\":\"Piala Dunia All-Star\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Equipe All Star\"}," +
                    "{\"lang\":\"th\",\"name\":\"ฟุตบอลโลกทีมสตาร์\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Đội bóng toàn ngôi sao\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"世界杯全明星\"}," +
                    "{\"lang\":\"ko\",\"name\":\"모든 스타 팀\"}]";
            }
        }
        #endregion

        public AllStarTeamGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.AllStarTeam;
            GameName                = "AllStarTeam";
        }
    }
}
