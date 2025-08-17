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
    class GaneshaJrGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GB6";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 10;
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
                return new int[] { 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 2, 5, 10 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 20000;
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
                return "Ganesha Jr";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"ko\", \"name\": \"작은 코끼리신\"}," +
                        "{ \"lang\": \"zh-cn\", \"name\": \"富贵小象神\"}," +
                        "{ \"lang\": \"en\", \"name\": \"Ganesha Jr\"}," +
                        "{ \"lang\": \"th\", \"name\": \"พระพิฆเนศน้อย  ผู้มั่งคั่ง\"}," +
                        "{ \"lang\": \"vn\", \"name\": \"Thần Voi Phú Quý\"}]";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 60.0; }
        }
        #endregion
        public GaneshaJrGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            List<CQ9ExtendedFeature2> extendFeatureByGame2 = new List<CQ9ExtendedFeature2>()
            {
                new CQ9ExtendedFeature2(){ name = "FeatureMinBet",      value = "600" },
                new CQ9ExtendedFeature2(){ name = "RSP",                value = "6,24,2,42,6" },
                new CQ9ExtendedFeature2(){ name = "AllWinMultiplier",   value = "120" },
                new CQ9ExtendedFeature2(){ name = "MultiplierReel1",    value = "0" },
                new CQ9ExtendedFeature2(){ name = "MultiplierReel2",    value = "0" },
                new CQ9ExtendedFeature2(){ name = "MultiplierReel3",    value = "0" },
                new CQ9ExtendedFeature2(){ name = "MultiplierReel4",    value = "0" },
                new CQ9ExtendedFeature2(){ name = "MultiplierReel5",    value = "0" },
                new CQ9ExtendedFeature2(){ name = "StripTable",         value = "0" },
            };
            _initData.ExtendFeatureByGame2 = extendFeatureByGame2;

            _gameID     = GAMEID.GaneshaJr;
            GameName    = "GaneshaJr";
        }
    }
}
