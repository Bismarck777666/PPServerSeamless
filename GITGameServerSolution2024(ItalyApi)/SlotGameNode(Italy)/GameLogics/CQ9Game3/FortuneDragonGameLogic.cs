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
   
    class FortuneDragonGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "194";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 5, 10 };
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
                return "Fortune Dragon";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Fortune Dragon\"}," +
                    "{\"lang\":\"ko\",\"name\":\"포춘 드래곤\"}," +
                    "{\"lang\":\"th\",\"name\":\"มังกรแห่งโชคลาภ\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"飞龙在天\"}]";
            }
        }

        #endregion

        public FortuneDragonGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FortuneDragon;
            GameName                = "FortuneDragon";
        }
    }
}
