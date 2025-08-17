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
   
    class Eight88GameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "20";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 10;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 300, 600, 1250, 1800, 3000, 6000, 12500, 20, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 12500;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "6T8yQTFxgUUABvtetpS71PBSefDu0rQCzyG9fZHqyQcfYW8omV/4GLpi1xj/vufX5pECVoPGxmc96Gz9h3JmN1xUlKo+5J2Fu5QEgoVBKoY5vPQXVh0e7rhdvuRoEFn0g6Soba/PUXGqux6KYRK4IxElaZ+MtftA0glrXxKRHhNLZwFEvTf0+Kt+IiHajFa5wqCBQKbX3cGdaFHJeh62WjqofCx9vyGf+43Zb6EuIToYsciGn8Xbw5V2lN7WIg+CGZ71VnRxMyoX1EcNkNagjIyWTA1ivNQErkKrBWn3xZoM8opM6AX+8U76g9CRyKKA9whPVAmjUxsMT/cStgwapJta2OdrjeJasTMS8Y9y01n34hhoss8TfxVk2dkKXL2AYI7e3Ht9FFXuPymB2KyhYScZ+IAtlNXfgVdnCGTKh0V4QD+nOet8uqT1qPY6zYeYG2E4nA+P5Al3DsAnLHOdlFVFzb/3LWVBj/WYz6lGHOhtfU0Zand2zZgAdwnkv2qzQnK5Ra4BsdHBRRtH/hYOHjxk6q0JOx8NCzyDXK0bDTSezF/Qt58QqWryVqlwbPo2Uw4jeWal7du4VLvF7Ubx9eTzeQnZOBtoXBqRofOKhEUKg/NwSUrrBTH3MXLIXcTNw42qhWqNbYHhA4tPBDp3491/h8PHDagPRcr2rl4Al+Cz13xTDRhP6azOzREGuxgl0CHMvut4Dtf8sqg6Omud+kZofRYIgmEC9Lmm9Yr4wYCs9mp48oNZl2LJvCAxRlS7e1QuhyG3aDGCBhnUeiVYOSCGa+zIM2iWb1+fLy/RyiHhahsp9BeQlT2titC7RAVEDt2GPY7iY1ch9zA1wNDOqxF9RqJq0bJyiuATJEJ2PzJiBs+pMQ/tcaXmFY7D0kg6wfg1mIlDK+1AezBfGQCODETn5BfX3BtZQDJRklLP81zdkY7udkxl5QF6QFLnnh/6pOkFhhJV/KqE04ds16i+yo03YEdEA0d01cSj8AvrkZQdjNbqiFPZ5FGgrtJLDuqt1j3Q2vWiu4dEfr9mpY2dsKdSmlc3vN8cfvTOzdWLiAqDS7HByfaGEQ0JhRexYGooyyTn1nT+nvP46XXikmwPzdxx0JbjrCMzBuxnvR2BFHopFUExtQo98b10ljm/WzKBb509abRWGez5YKawTq+u9N42vYMuUPG4r4r5IK3hwsc7r6BLNXNqJjAVaIkPEVQ2dVGadOShvlryRovxGegh49ijenLGuC2x9+PHoYhc1SagXwIABCT6tLoGEYN2KyUJafdfys5Fy8gpqa5x8XnViWwVe3BEtKS6JPs4Yor8MvVJJSz1oLsmHbFmORd5SIxHVonkoLoHvolljkocX9+ViYW5jH3/G6l5zxU3LCmpS2ZgixZ6ZP0h7+6oqH7PSj3DVXhbocrSmDwF/lPZnhZMs+aSA5QnM00PDnBm0Q==";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "888";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"888\"}," +
                    "{\"lang\":\"ko\",\"name\":\"888\"}," +
                    "{\"lang\":\"th\",\"name\":\"888\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"888\"}," +
                    "{\"lang\":\"id\",\"name\":\"888\"}," +
                    "{\"lang\":\"vn\",\"name\":\"888\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"发发发\"}]";
            }
        }
        #endregion

        public Eight88GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.Eight88;
            GameName                = "Eight88";
        }
    }
}
