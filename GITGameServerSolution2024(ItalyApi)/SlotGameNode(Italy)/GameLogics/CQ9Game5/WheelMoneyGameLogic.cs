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
   
    class WheelMoneyGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "128";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 1;
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
                return new int[] { 1000, 2500, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 300, 500 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 200000;
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
                return "Wheel Money";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Wheel Money\"}," +
                    "{\"lang\":\"ko\",\"name\":\"휠 머니\"}," +
                    "{\"lang\":\"th\",\"name\":\"วีลมันนี่\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Dinheiro da Roda\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"转大钱\"}]";
            }
        }
        #endregion

        public WheelMoneyGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.WheelMoney;
            GameName                = "WheelMoney";
        }
    }
}
