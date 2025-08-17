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
   
    class MonkeyOfficeLegendGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "22";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 1;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 1000, 2500, 5000, 10000, 20000, 30000, 50000, 100000, 200000, 20, 50, 100, 200, 300, 500 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 200000;
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
                return "Monkey Office Legend";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Monkey Office Legend\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Lenda do Macaco\"}," +
                    "{\"lang\":\"ko\",\"name\":\"몽키 오피스 레전드\"}," +
                    "{\"lang\":\"th\",\"name\":\"ไซอิ๋ว\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Truyền thuyết ngôi nhà khỉ\"}," +
                    "{\"lang\":\"id\",\"name\":\"Legenda Pejabat Kera\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"庶务西游二课\"}]";
            }
        }

        #endregion

        public MonkeyOfficeLegendGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.MonkeyOfficeLegend;
            GameName                = "MonkeyOfficeLegend";
        }
    }
}
