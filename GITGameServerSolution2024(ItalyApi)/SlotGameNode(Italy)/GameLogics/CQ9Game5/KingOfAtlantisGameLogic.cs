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
   
    class KingOfAtlantisGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "211";
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
                return "King of Atlantis";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"King of Atlantis\"}," +
                    "{\"lang\":\"ko\",\"name\":\"바다의 왕\"}," +
                    "{\"lang\":\"th\",\"name\":\"คิงออฟแอตแลนติส\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"海王\"}]";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 69.1; }
        }
        #endregion

        public KingOfAtlantisGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.KingOfAtlantis;
            GameName                = "KingOfAtlantis";
        }
    }
}
