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
    class FootballFeverMGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GB13";
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
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 5, 10, 20, 50, 100, 200, 500, 1000, 2000, 1, 2 };
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
                return "Football Fever M";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"ko\", \"name\": \"축구 열풍 M\"}," +
                        "{ \"lang\": \"zh-cn\", \"name\": \"足球狂热 M\"}," +
                        "{ \"lang\": \"en\", \"name\": \"Football Fever M\"}," +
                        "{ \"lang\": \"th\", \"name\": \"ฟุตบอลฟีเวอร์\"}]";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 90.0; }
        }
        #endregion
        public FootballFeverMGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            List<CQ9ExtendedFeature2> extendFeatureByGame2 = new List<CQ9ExtendedFeature2>()
            {
                new CQ9ExtendedFeature2(){ name = "FeatureMinBet",value = "4500" }
            };
            _initData.ExtendFeatureByGame2 = extendFeatureByGame2;

            _gameID     = GAMEID.FootballFeverM;
            GameName    = "FootballFeverM";
        }
    }
}
