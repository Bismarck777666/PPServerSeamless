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
   
    class AladdinsLampGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "177";
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
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 4, 5, 10 };
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
                return "Aladdin's lamp";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Aladdin's lamp\"}," +
                    "{\"lang\":\"ko\",\"name\":\"알라딘의 마술등\"}," +
                    "{\"lang\":\"th\",\"name\":\"จินนี่จ๋า\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"小伙与神灯\"}]";
            }
        }
        #endregion

        public AladdinsLampGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.AladdinsLamp;
            GameName                = "AladdinsLamp";
        }
    }
}
