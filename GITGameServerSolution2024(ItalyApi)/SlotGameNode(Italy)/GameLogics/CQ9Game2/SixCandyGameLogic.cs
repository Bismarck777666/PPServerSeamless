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
   
    class SixCandyGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "153";
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
                return "Six Candy";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Six Candy\"}," +
                    "{\"lang\":\"es\",\"name\":\"Seis carameros\"}," +
                    "{\"lang\":\"ja\",\"name\":\"シックスキャンディ\"}," +
                    "{\"lang\":\"ko\",\"name\":\"식스 캔디\"}," +
                    "{\"lang\":\"th\",\"name\":\"ซิกแคนดี้\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"六颗糖\"}]";
            }
        }

        #endregion

        public SixCandyGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.SixCandy;
            GameName                = "SixCandy";
        }
    }
}
