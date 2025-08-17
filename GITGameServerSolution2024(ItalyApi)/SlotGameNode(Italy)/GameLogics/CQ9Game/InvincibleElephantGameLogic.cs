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
   
    class InvincibleElephantGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "124";
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
                return "InvincibleElephant";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"th\",\"name\": \"ช้างคงกระพัน\"}," +
                        "{\"lang\": \"zh-cn\",\"name\": \"锁象无敌\"}," +
                        "{\"lang\": \"en\",\"name\": \"Invincible Elephant\"}," +
                        "{\"lang\": \"es\",\"name\": \"Elefante invencible\"}]";
            }
        }
        #endregion

        public InvincibleElephantGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.InvincibleElephant;
            GameName                = "InvincibleElephant";
        }
    }
}
