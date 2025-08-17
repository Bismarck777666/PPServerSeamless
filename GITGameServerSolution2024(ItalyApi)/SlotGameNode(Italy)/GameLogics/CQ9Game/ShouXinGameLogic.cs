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
   
    class ShouXinGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "150";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 25;
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
                return new int[] { 200, 400, 800, 1200, 2000, 4000, 8000, 2, 4, 8, 10, 30, 50, 100 };
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

        protected override int FreeSpinTypeCount
        {
            get { return 3; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202 };
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Shou-Xin";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"th\", \"name\": \"โชล - ซิน\"}, {\"lang\": \"zh-cn\", \"name\": \"寿星大发\"}, {\"lang\": \"en\", \"name\": \"Shou-Xin\"}]";
            }
        }

        #endregion

        public ShouXinGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.ShouXin;
            GameName                = "ShouXin";
        }
    }
}
