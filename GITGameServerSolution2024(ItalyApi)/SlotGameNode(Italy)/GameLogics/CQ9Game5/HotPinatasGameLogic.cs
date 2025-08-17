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
   
    class HotPinatasGameLogic : BaseCQ9TembleGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "215";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 15;
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
                return new int[] { 50, 100, 300, 600, 1200, 1800, 3000, 6000, 12000, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 12000;
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
                return "Hot Pinatas";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Hot Pinatas\"}," +
                    "{\"lang\":\"ko\",\"name\":\"핫 피나타스\"}," +
                    "{\"lang\":\"th\",\"name\":\"เผ็ชซี้ด\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"辣爆了\"}]";
            }
        }
        #endregion

        public HotPinatasGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.HotPinatas;
            GameName                = "HotPinatas";
        }
    }
}
