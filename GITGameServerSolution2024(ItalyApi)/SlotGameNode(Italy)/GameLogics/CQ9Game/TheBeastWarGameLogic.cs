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
   
    class TheBeastWarGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "57";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 125, 250, 500, 750, 1250, 2500, 5000, 1, 2, 5, 10, 30, 50, 80 };
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
                return "The Beast War";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"th\",\"name\":\"สงครามสัตว์ในตำนาน\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Cuộc chiến quái thú\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"神兽争霸\"}," +
                    "{\"lang\":\"en\",\"name\":\"The Beast War\"}," +
                    "{\"lang\":\"id\",\"name\":\"Mythical Beings Melee\"}," +
                    "{\"lang\":\"ko\",\"name\":\"더 비스트 워\"}]";
            }
        }

        #endregion

        public TheBeastWarGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.TheBeastWar;
            GameName                = "TheBeastWar";
        }
    }
}
