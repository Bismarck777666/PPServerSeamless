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
   
    class FaCaiFuWaGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "143";
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
                return "Fa Cai Fu Wa";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Fa Cai Fu Wa\"}," +
                    "{\"lang\":\"ko\",\"name\":\"파카이후아\"}," +
                    "{\"lang\":\"th\",\"name\":\"ฟา ไฉ ฟู วา\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"发财福娃\"}]";
            }
        }
        #endregion

        public FaCaiFuWaGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FaCaiFuWa;
            GameName                = "FaCaiFuWa";
        }
    }
}
