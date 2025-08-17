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
   
    class GreekGodsGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "171";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 30;
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
                return new int[] { 80, 125, 250, 500, 750, 1250, 2500, 1, 2, 5, 10, 30, 50 };
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
        protected override int FreeSpinTypeCount
        {
            get { return 4; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203 };
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Greek Gods";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Greek Gods\"}," +
                    "{\"lang\":\"ko\",\"name\":\"그리스 신들\"}," +
                    "{\"lang\":\"th\",\"name\":\"เทพเจ้ากรีก\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"希腊众神\"}]";
            }
        }

        #endregion

        public GreekGodsGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.GreekGods;
            GameName                = "GreekGods";
        }
    }
}
