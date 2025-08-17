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
   
    class FruitKing2GameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "44";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 50;
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
                return new int[] { 60, 120, 250, 360, 600, 1200, 1, 2, 5, 10, 20, 30 };
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
                return "821d9f504d7cc3584+tn9Rt8qH67838zARzjG66LYQNC2ID8yyrb7ck5h34rjJPhWNfsV2OymE1TqJpldmZml+e4wugW+owLFvAfbemOHAl3lYYiT08QPDabVTLbSe7oBNbuOWrhDVPgUsvE0ri1tjPeIdn0gGHiXq49V5sjrC7z3yYtYTZ3rkNQoqHYbR1Yf8cVqeASl94ggpcNHMOAcPVV7qXZmZFvSSdot9mr4pIBSwD9fHmRvtohF4bvn89ABCZcMpPIaEaWYuAQGfyyoKwaxYEqFB74i9eDxxv4BxQ97zKHd8U0P+XB/4mCZgcMheE1G5CjVpxqSabDNy0nVvjPLkCGU13KkZbZea5q/3ZyMsfSbTevabyGh6lgr0pkzvev+oNmCftnwXk5uq8Z/7xVk+ng/IMLnAKwuzNIWb0Q5cF58gCYzvNoZnTlnU9+F7WI5t8W8dC0SSil0AZ+xazt9WLNF9vAeONYqOmIf7lteum5nHMNfm3opybPR8Aoh6QYHMwG+deNPLypd+qWSCGbtr8QK0HS70YD4UA10jj07tzK+0yJafFmAj1MHbHI+/6i10N2pwBt8w1UjKqaZyI4G53eu+ITpGdlAqok4DoApUJoYUGKXWWgdAjOAdaoTbVjrnM7bdoTw682sIzrfaHwm5XtHMrEy0qStsPz6ajnpJ7eKLOlLa7J54+3eQtY5s6vIa5GbWxce9hTpHzLXoW6s7dZh8VmANIQiXjrg8MM4lVGxXME0+RF6P/wVJfupU1LYDY0GRjh3vd9QrBe3nIv3LbzTTjMqg6Jo3ZkQy/yMD9raKqhXfGOnf0U1QomTm/tHy217k3tDLw+ZOCEr+YQk0trBd30EVnO25JOHrlhLc/zXbkUs9dxcoGK0yp++N/OKU+Jj2KVUFv511pT0n0NcGWo34eSh8Ovxys/guma96y+30UvT8rT/0qBTGBEL4nAyzpfheaF2izxvLL/aPiRD44yorbJ00O4N4UEJFWB8eXbPAaIGR6TWHEoiz/5fQK0B6QT+5LufbpKXBwQr7VPVxuc8rYXjPxa+euAbrXoqpvp/ZV85wdxQFKGliiwX3sccV3yqa+r8RvVONGt/bva0K0Lzex0zaS+ljDVJ2bG8QSSIrYJ+kRQ7H0UZQvO4cXulcGN6s5glAjbLl1/UsgtHLqejvWTjKPThi3n4pLrZ3qPkBNUw7BPEQAaeQfcnQhr4NBoY0DwM46gk98oR2YImLfae5G5dvVKxxyhWUTTMGkffNkTBRThnG4YIJuPmsX99WYmlkHQJ9FkIpB+pL66879FUhm1F3UZx7NK2884XXMJJAoP0ADRuB57YcOjLJ8w31QnuGBCjTD1sTq5/mIWxLr1LtZzDZn7qcEhuCn0Ox6yIUSwtbVyVbwxNJGdTfXV5OzNb6/JIG1nvkYqF4HSAJXxAdnki5rieidcOC467bDM1sTfyRg266IfJn6kpSLXeEMDGcazW3XQeU0uvawIfQypgVbcp4gN6Q==";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Fruit King II";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Fruit King II\"}," +
                    "{\"lang\":\"ko\",\"name\":\"후르츠 킹 II\"}," +
                    "{\"lang\":\"th\",\"name\":\"คิงผลไม้อัพเกร็ท\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"豪华水果王\"}]";
            }
        }
        #endregion

        public FruitKing2GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FruitKing2;
            GameName                = "FruitKing2";
        }
    }
}
