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
   
    class DetectiveDee2GameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "221";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 40;
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
        protected override string CQ9GameName
        {
            get
            {
                return "Detective Dee 2";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Detective Dee 2\"}," +
                    "{\"lang\":\"ko\",\"name\":\"명탐정 디2\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"狄仁杰四大天王\"}]";
            }
        }
        #endregion

        public DetectiveDee2GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.DetectiveDee2;
            GameName                = "DetectiveDee2";
        }
    }
}
