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
   
    class WukongAndPeachesGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "68";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 80, 125, 250, 500, 750, 1250, 2500, 1, 2, 5, 10, 30, 50 };
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
                return "WukongAndPeaches";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Wukong & Peaches\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Wukong e Pêssegos\"}," +
                    "{\"lang\":\"ko\",\"name\":\"우공과 피치\"}," +
                    "{\"lang\":\"th\",\"name\":\"หงอคงและลูกท้อ\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"悟空偷桃\"}]";
            }
        }

        #endregion

        public WukongAndPeachesGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.WukongAndPeaches;
            GameName                = "WukongAndPeaches";
        }
    }
}
