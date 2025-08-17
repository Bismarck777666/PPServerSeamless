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
   
    class NightCityGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "229";
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
                return new int[] { 30, 50, 100, 200, 400, 800, 1000, 1600, 2000, 1, 2, 5, 10 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 2000;
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
                return "Night City";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Night City\"}," +
                    "{\"lang\":\"ko\",\"name\":\"불야성\"}," +
                    "{\"lang\":\"th\",\"name\":\"ตะลุยราตรี\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"不夜城\"}]";
            }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        protected override double[] MoreBetMultiples
        {
            get 
            {
                return new double[] { 1, 1.25 };
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

        public NightCityGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            List<CQ9ExtendedFeature2> extendFeatureByGame2 = new List<CQ9ExtendedFeature2>()
            {
                new CQ9ExtendedFeature2(){ name = "FeatureMinBet",value = "1600" },
            };
            _initData.ExtendFeatureByGame2 = extendFeatureByGame2;

            _gameID                 = GAMEID.NightCity;
            GameName                = "NightCity";
        }
    }
}
