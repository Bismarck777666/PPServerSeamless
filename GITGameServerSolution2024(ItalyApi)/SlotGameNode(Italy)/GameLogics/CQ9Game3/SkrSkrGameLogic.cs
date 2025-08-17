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
   
    class SkrSkrGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "118";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 80, 125, 250, 500, 750, 1250, 2500, 5000, 1, 2, 5, 10, 30 };
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
                return "SkrSkr";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"SkrSkr\"}," +
                    "{\"lang\":\"ko\",\"name\":\"SkrSkr\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"SkrSkr\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"老司机\"}]";
            }
        }

        #endregion

        public SkrSkrGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.SkrSkr;
            GameName                = "SkrSkr";
        }
    }
}
