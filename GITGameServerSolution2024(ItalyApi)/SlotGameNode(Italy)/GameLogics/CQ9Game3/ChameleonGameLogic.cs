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
   
    class ChameleonGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "79";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 60, 120, 250, 360, 600, 1200, 1, 2, 5, 10, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1200;
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
                return "Chameleon";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Chameleon\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Camaleão\"}," +
                    "{\"lang\":\"ko\",\"name\":\"카멜레온\"}," +
                    "{\"lang\":\"th\",\"name\":\"กิ้งก่า\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"变色龙\"}]";
            }
        }

        #endregion

        public ChameleonGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.Chameleon;
            GameName                = "Chameleon";
        }
    }
}
