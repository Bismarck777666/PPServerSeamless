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
   
    class DragonTreasureGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "197";
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
                return new int[] { 50, 100, 300, 600, 1250, 1800, 3000, 6000, 12500, 5, 10, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 12500;
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
                return "Dragon's Treasure";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Dragon's Treasure\"}," +
                    "{\"lang\":\"ko\",\"name\":\"용의 보물\"}," +
                    "{\"lang\":\"th\",\"name\":\"สมบัติของมังกร\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"盘龙秘宝\"}]";
            }
        }
        #endregion

        public DragonTreasureGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.DragonTreasure;
            GameName                = "DragonTreasure";
        }
    }
}
