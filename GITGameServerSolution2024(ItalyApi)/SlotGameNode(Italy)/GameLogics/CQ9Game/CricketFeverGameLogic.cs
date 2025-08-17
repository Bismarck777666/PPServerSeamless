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
   
    class CricketFeverGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "188";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 125, 250, 500, 750, 1250, 2500, 5000, 1, 2, 3, 5, 10, 30, 50, 80 };
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
                return "Cricket Fever";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{ \"lang\": \"th\", \"name\": \"คริกเก็ตบ้าคลั่ง\"}," + 
                        "{ \"lang\": \"ko\", \"name\": \"크리켓 열병\"}," +
                        "{ \"lang\": \"zh-cn\", \"name\": \"板球狂热\"}," +
                        "{ \"lang\": \"en\", \"name\": \"Cricket Fever\"}]";
            }
        }

        #endregion

        public CricketFeverGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.CricketFever;
            GameName                = "CricketFever";
        }
    }
}
