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
   
    class HappyMagpiesGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "88";
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
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 5000, 8, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 5000;
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
            get { return 2; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201 };
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Happy Magpies";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Happy Magpies\"}," +
                    "{\"lang\":\"ko\",\"name\":\"행복한 까치\"}]";
            }
        }
        #endregion

        public HappyMagpiesGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.HappyMagpies;
            GameName                = "HappyMagpies";
        }
    }
}
