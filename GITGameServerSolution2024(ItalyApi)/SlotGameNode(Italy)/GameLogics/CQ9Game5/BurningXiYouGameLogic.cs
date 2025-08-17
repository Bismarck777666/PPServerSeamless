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
   
    class BurningXiYouGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "212";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 9;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 30, 50, 100 };
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
                return "Burning Xi-You";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Burning Xi-You\"}," +
                    "{\"lang\":\"ko\",\"name\":\"버닝 씨유\"}," +
                    "{\"lang\":\"th\",\"name\":\"ไซอิ๋วลุกเป็นไฟ\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"火爆西游\"}]";
            }
        }
        #endregion

        public BurningXiYouGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.BurningXiYou;
            GameName                = "BurningXiYou";
        }
    }
}
