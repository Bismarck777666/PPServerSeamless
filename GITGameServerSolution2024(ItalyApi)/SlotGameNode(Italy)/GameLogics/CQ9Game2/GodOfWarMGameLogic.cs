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
   
    class GodOfWarMGameLogic : BaseSelFreeCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "127";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 88;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 200, 300, 500, 1000, 2000, 5000, 10000, 1, 2, 3, 5, 10, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 10000;
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
            get { return 5; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204 };
        }
        protected override string CQ9GameName
        {
            get
            {
                return "God of War M";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"God of War M\"}," +
                    "{\"lang\":\"es\",\"name\":\"Dios de la Guerra M\"}," +
                    "{\"lang\":\"id\",\"name\":\"Dewa Perang M\"}," +
                    "{\"lang\":\"ja\",\"name\":\"ゴッドオブウォー M\"}," +
                    "{\"lang\":\"ko\",\"name\":\"갓 오브 워 M\"}," +
                    "{\"lang\":\"th\",\"name\":\"สงครามเทพ M\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Thần chiến tranh M\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"直式武圣\"}]";
            }
        }
        #endregion

        public GodOfWarMGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.GodOfWarM;
            GameName                = "GodOfWarM";
        }
    }
}
