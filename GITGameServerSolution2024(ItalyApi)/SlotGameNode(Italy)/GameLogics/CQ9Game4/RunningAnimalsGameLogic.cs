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
   
    class RunningAnimalsGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "136";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 5000, 1, 2, 5, 10, 30 };
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
        protected override string CQ9GameName
        {
            get
            {
                return "Running Animals";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Running Animals\"}," +
                    "{\"lang\":\"ko\",\"name\":\"러닝 애니몰\"}," +
                    "{\"lang\":\"es\",\"name\":\"Animales corriendo\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Animais em execução\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"奔跑吧猛兽\"}]";
            }
        }
        #endregion

        public RunningAnimalsGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.RunningAnimals;
            GameName                = "RunningAnimals";
        }
    }
}
