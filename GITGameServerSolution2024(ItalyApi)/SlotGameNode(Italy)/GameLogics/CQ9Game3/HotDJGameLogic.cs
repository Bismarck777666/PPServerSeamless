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
    class HotDJGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GB5";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 20;
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
                return new int[] { 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 1, 2, 5, 10 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 10000;
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
                return "Hot DJ";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"ko\", \"name\": \"핫 DJ\"}," +
                        "{ \"lang\": \"zh-cn\", \"name\": \"热音DJ\"}," +
                        "{ \"lang\": \"en\", \"name\": \"Hot DJ\"}," +
                        "{ \"lang\": \"th\", \"name\": \"ดีเจสุดฮอต\"}]";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 50.0; }
        }
        #endregion
        public HotDJGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            List<CQ9ExtendedFeature2> extendFeatureByGame2 = new List<CQ9ExtendedFeature2>()
            {
                new CQ9ExtendedFeature2(){ name = "FeatureMinBet",value="1000" }
            };
            _initData.ExtendFeatureByGame2 = extendFeatureByGame2;

            _gameID     = GAMEID.HotDJ;
            GameName    = "HotDJ";
        }
    }
}
