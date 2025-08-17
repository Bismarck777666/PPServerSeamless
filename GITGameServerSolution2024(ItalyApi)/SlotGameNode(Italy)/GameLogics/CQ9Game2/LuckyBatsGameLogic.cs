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
   
    class LuckyBatsGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "10";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 55;
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
                return new int[] { 60, 120, 250, 360, 600, 1200,1, 2, 4, 10, 20, 30 };
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
                return "Lucky Bats";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Lucky Bats\"}," +
                    "{\"lang\":\"es\",\"name\":\"Bates de la suerte\"}," +
                    "{\"lang\":\"id\",\"name\":\"Kelelawar Mujur\"}," +
                    "{\"lang\":\"ja\",\"name\":\"五福臨門\"}," +
                    "{\"lang\":\"ko\",\"name\":\"럭키 뱃\"}," +
                    "{\"lang\":\"th\",\"name\":\"ลัคกี้ แบท\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Chú dơi may mắn\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Morcegos de Sorte\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"五福临门\"}]";
            }
        }
        #endregion

        public LuckyBatsGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.LuckyBats;
            GameName                = "LuckyBats";
        }
    }
}
