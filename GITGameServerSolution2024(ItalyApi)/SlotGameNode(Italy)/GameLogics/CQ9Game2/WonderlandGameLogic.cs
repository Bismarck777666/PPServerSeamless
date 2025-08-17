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
   
    class WonderlandGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "116";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 88;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 30, 50, 100, 200, 300, 500, 1000, 1, 2, 3, 5, 10, 20 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1000;
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
                return "Wonderland";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Wonderland\"}," +
                    "{\"lang\":\"ko\",\"name\":\"이상한 나라\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"País das Maravilhas\"}," +
                    "{\"lang\":\"th\",\"name\":\"แดนมหัศจรรย์\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"梦游仙境\"}]";
            }
        }

        #endregion

        public WonderlandGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Wonderland;
            GameName                = "Wonderland";
        }
    }
}
