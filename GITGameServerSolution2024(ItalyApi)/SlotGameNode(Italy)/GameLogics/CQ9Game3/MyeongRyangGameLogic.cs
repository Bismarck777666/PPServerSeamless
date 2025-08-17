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
    class MyeongRyangGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GB12";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 1, 2, 5 };
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
                return "Myeong Ryang";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"ko\", \"name\": \"명량\"}," +
                        "{ \"lang\": \"zh-cn\", \"name\": \"鸣梁\"}," +
                        "{ \"lang\": \"en\", \"name\": \"Myeong-ryang\"}," +
                        "{ \"lang\": \"th\", \"name\": \"ขุนพลคลื่นคำราม\"}," +
                        "{ \"lang\": \"vn\", \"name\": \"Đại thủy chiến\"}]";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 80.0; }
        }
        #endregion
        public MyeongRyangGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            List<CQ9ExtendedFeature2> extendFeatureByGame2 = new List<CQ9ExtendedFeature2>()
            {
                new CQ9ExtendedFeature2(){ name = "FeatureMinBet",value = "3200" }
            };
            _initData.ExtendFeatureByGame2 = extendFeatureByGame2;

            _gameID     = GAMEID.MyeongRyang;
            GameName    = "MyeongRyang";
        }
    }
}
