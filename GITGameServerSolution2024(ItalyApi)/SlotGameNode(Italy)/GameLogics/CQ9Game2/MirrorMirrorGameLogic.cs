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
   
    class MirrorMirrorGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "228";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 100, 200, 400, 800, 1200, 2000, 2, 5, 10, 30, 50 };
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
                return "Mirror Mirror";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"ko\", \"name\": \"미러미러\"}," +
                        "{ \"lang\": \"zh-cn\", \"name\": \"魔镜魔镜\"}," +
                        "{ \"lang\": \"en\", \"name\": \"Mirror Mirror\"}," +
                        "{ \"lang\": \"th\", \"name\": \"กระจกวิเศษ\"}," +
                        "{ \"lang\": \"es\", \"name\": \"Mirror Mirror\"}," +
                        "{ \"lang\": \"ja\", \"name\": \"鏡 鏡\"}]";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 60; }
        }
        #endregion
        public MirrorMirrorGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            List<CQ9ExtendedFeature2> extendFeatureByGame2 = new List<CQ9ExtendedFeature2>()
            {
                new CQ9ExtendedFeature2(){ name = "FeatureMinBet",value = "1200" },
            };
            _initData.ExtendFeatureByGame2 = extendFeatureByGame2;

            _gameID                 = GAMEID.MirrorMirror;
            GameName                = "MirrorMirror";
        }
    }
}
