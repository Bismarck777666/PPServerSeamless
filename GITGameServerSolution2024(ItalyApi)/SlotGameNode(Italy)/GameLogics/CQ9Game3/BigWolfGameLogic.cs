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
   
    class BigWolfGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "21";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 3;
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
                return new int[] { 300, 500, 1000, 2000, 4000, 6000, 10000, 20000, 40000, 10, 20, 50, 100, 200 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 40000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "255a88a006b811acceigpcSV7JOo0KCg09nJNg6AaoAzzwWV7yyDav50v/iucB5JsZ2H80BG4LyCL4+wDVww5BwdO3siPWtl1xQaYzfuBsTf06f73gD/yiaFzGlJECd5l/cy+23Sws5KNsCcjavmny+V0AhCBwZz0TfHIPvk898d6zlkeOND+KejjWN2UsofinKXalp4GKsJtIvVInf6rYgrK1u2X+P+z/X5gLezYiSNccg5Yu6nC9E7DGrnXO6zmy63/LcBXDtjjVSKYhQf5MZcZ1BnW8zOr2pb3aU8WcuX5zEdR2A3/dwYpZZH/pVOc+Q6AgT42cg1/uDStMA/CWb/A4O+u0JzxzzqoMqOcHdDucGfm4dH6vuzsBPScr7hPT7heAtUUYEFZJW7M0qViiLRcl2fSDJ9QFU4WHwpoFJFgj2t4hROY/AJf5La0gZj68NS5reGKyfLbaIHt6aWWRJPm/nl30dlquEy+JtSheQiLQ4Sf0c8TpxAseO9Qsz13cwlJUIDGn/63uAhEgoo+Ue8hfgtTgaCzi0k3s9OrgYC4N5jbDU9xLfL//N27nuULJehsW8iULVsIlmOWu18GAS8WepCX4p6yuGD1A/WkF7aOGiaywN6OKjlQHgy92ROD36BSwPxDYzW8LFVA5EaMnixXKIAxuM8Ejbu91g4XQG0YyMo4ry0SSA4VjJ71A2aL/1QoV9IBc69HO3JA2IWBSZB1PFTFLmLbRePvMAb7deimj7ZX/LQJRk0uQFax6ZJqIZFBDtp9BZ/wB+TbvqGhU+MIDeUaBUQaa8QLJObMExvbck/APRlZap1JHVg4FhVg1IPlXu4o+laxy+LmqqmQ9xn0ubMevEySDHySiTM+EtBRkug66V1sUN5CClbExUnJvwyIjf522XgTrBnGnxv2OWlwOMkX4VYdcjtkUNn0CN3Nd95CDFuGcp3+cbvhD2BIyvsKtvXS4dyFSInChS1BCoLMbz7ZFswdWSCq2ISLqZKkP1ywWHPe886UnVX8McsNek9ORT0Zl3SINt2NO9rY1ArdxPz2lN34D67LurUHxpzm0/HinRvC2EpBsHmh/NX+SOaNOd26Z+rAKS8+CVmxVBfMgUr4zEBBtZ5jw52jsYNeXdPi7IlGSVy0JABHPtYPOVbTiTErt+gDaa3";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "BigWolf";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Big Wolf\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Lobo Grande\"}," +
                    "{\"lang\":\"ko\",\"name\":\"빅 울프\"}," +
                    "{\"lang\":\"th\",\"name\":\"ตำนานหมาป่า\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Truyền thuyết chó sói\"}," +
                    "{\"lang\":\"id\",\"name\":\"Legenda Serigala\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"野狼传说\"}]";
            }
        }

        #endregion

        public BigWolfGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.BigWolf;
            GameName                = "BigWolf";
        }
    }
}
