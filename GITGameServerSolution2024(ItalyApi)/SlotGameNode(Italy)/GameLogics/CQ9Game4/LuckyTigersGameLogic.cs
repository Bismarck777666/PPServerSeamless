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
   
    class LuckyTigersGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "226";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 1, 2, 5, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 8000;
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
                return "LuckyTigers";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Lucky Tigers\"}," +
                    "{\"lang\":\"ko\",\"name\":\"럭키 타이거\"}," +
                    "{\"lang\":\"th\",\"name\":\"เสือน้อยสุดเฮง\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"多虎多财\"}]";
            }
        }
        #endregion

        public LuckyTigersGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.LuckyTigers;
            GameName                = "LuckyTigers";
        }
    }
}
