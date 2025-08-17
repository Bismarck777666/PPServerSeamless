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
   
    class UproarInHeavenGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "5009";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 125, 250, 500, 750, 1250, 2500, 5000, 12500, 25000, 1, 2, 5, 8, 10, 30, 50, 80 };
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
                return "Uproar In Heaven";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"ko\", \"name\": \"천궁의 소란\"}," +
                        "{ \"lang\": \"en\", \"name\": \"UproarInHeaven\"}]";
            }
        }
        #endregion

        public UproarInHeavenGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.UproarInHeaven;
            GameName                = "UproarInHeaven";
        }
    }
}
