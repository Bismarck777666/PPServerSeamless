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
   
    class ZhongKuiGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "9";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 30, 50, 100, 200, 300, 500, 1000, 1, 2, 3, 5, 10, 20 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1000;
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
                return "Zhong Kui";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Zhong Kui\"}," +
                    "{\"lang\":\"es\",\"name\":\"Zhong Kui\"}," +
                    "{\"lang\":\"ja\",\"name\":\"鍾馗様\"}," +
                    "{\"lang\":\"ko\",\"name\":\"종규\"}," +
                    "{\"lang\":\"th\",\"name\":\"จงขุ่ย\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"钟馗运财\"}]";
            }
        }

        #endregion

        public ZhongKuiGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.ZhongKui;
            GameName                = "ZhongKui";
        }
    }
}
