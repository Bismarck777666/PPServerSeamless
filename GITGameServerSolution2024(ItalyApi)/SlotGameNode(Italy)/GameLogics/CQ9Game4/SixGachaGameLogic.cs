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
   
    class SixGachaGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "184";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 4, 5, 10 };
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
                return "Six Gacha";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Six Gacha\"}," +
                    "{\"lang\":\"ko\",\"name\":\"식스 가차\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"六颗扭蛋\"}]";
            }
        }
        #endregion

        public SixGachaGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.SixGacha;
            GameName                = "SixGacha";
        }
    }
}
