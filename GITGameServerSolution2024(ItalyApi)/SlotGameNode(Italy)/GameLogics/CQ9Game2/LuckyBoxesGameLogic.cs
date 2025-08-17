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
   
    class LuckyBoxesGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "204";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 1, 2, 5, 10, 30 };
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
                return "LuckyBoxes";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"LuckyBoxes\"}," +
                    "{\"lang\":\"ko\",\"name\":\"럭키 박스\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"百宝箱\"}]";
            }
        }
        #endregion

        public LuckyBoxesGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.LuckyBoxes;
            GameName                = "LuckyBoxes";
        }
    }
}
