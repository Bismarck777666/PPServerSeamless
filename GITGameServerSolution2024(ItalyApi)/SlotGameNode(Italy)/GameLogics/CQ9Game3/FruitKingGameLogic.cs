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
   
    class FruitKingGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "1";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 9;
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
                return new int[] { 300, 500, 1000, 2000, 3000, 5000, 10000, 10, 20, 30, 50, 100, 200 };
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
                return "bb19ccbf06eb02bcECVEKR66AZotETwECHuhrMKadnAMPEESfybK9JhCiVQuDOO6qryHM3pENV0nd/axJ1Qw/A32wJlrfr3/J7DG7dSQL/51QzetAfAILtlhhbYeUY+t+sYiZAtAL0WLW6fwqfJ8gNIQdLa+ZJnYG3o9/FTZALQIZHUJVsqPNdYu0SJZfH27xvMsey5FrOLVJyh6WuY86fQWHHr8fSY4vpERq3csQDV2UNtynuxKHJ5XQfjYqNznLfntJONw7GWqAt7rAff9JXtxph4rzse+n80NjaB/XVxKKUIxrwmMmpPTn6Swcol/LMRm2nllpb3vJ7QcmdSUOGmIY7oKX1/Tz/+uhk06aUKcoJaTnCVwrHxo3PrQrufmn5DF65BsmlhOLWA1cNKcFaZRrcL9Htl7xpS6ozB5wnwDfJ8A1pqde5Iw0Nb+d0AC5IrCkITLwE9ZqWIwWNT2z3CEfrI5ZV94VCv37GzJSFG7N33XVyCrUcvkMi2jyxUMvC6zaKvhlgdspDWciBW+rLdxZDTeqcs6tuhgQev8G3a1BFYHqJHP57GITKIAondlf6i8VMjHr0H4qbSQevOjG17mYscGCZcJeWQa8kMT+fYeSUYvfFvTRpsHNiYyx4p+tLBOZdhut/9pI41eyyIpwJ3XgeD/8YJVa3Wns2h02EjBSxQSzDGh4pP2CV591aILrTPwma97e0vPxylV07XFV4NoWIfoxQMDlOZN9MMXaEMeyA0k4Zng0blZhU71Dz2bOb8nW/3JrsjSBoB1LBu1NZuyb1wwdEXUMiBRynJKZcLlK4BtV+/UN0aoO0XhQmxjg/grOS7ClzVEfC7H2fNpsjXpoCDQK0jGDDMdkGAqxkKtQ45P2ZP/g1z5m7dKX5d+ku/kmI2TSRtRrqx1xnt1qFFBgLlZ8PQvzRy0/L+LMUfH941CaW4oCfnYvOZvrSzVq8xg/go8HZMBaUjb2khyAhZmIWpH70gCMSEQ3q6zxN0BEApVxg4CsOH+YfeGTwBnnfrqT2w2VwzZAiIHpUbbXWmcdZF0x7g1fRSsGThLVbhVpdF4Ebhh9rMrHQhfOeMJFkCOk3/y9WgM+r6Q6WDHAy+HooQYOsTAGWbrmMyK+Dofj4fjU4/W/7tlEwcwKCoqmPRcgOsG9kpQ99rFakm95F9qtAroSyX1cUbvfQUyPPNR0us/uhszZFgXjsgsS4VJ3NP8QI70ngt47EOc2RRsWAdRItuZAGqx/SuzAWpkh3lUUDJ9ajkjRQIcBoblV3O7u/rqWSYrcybw6r4fw16gLGUIMdikA0BX7LH7Y+vUgmrexqchU6g7koMSSqBrCArmb14WrjSxOxbKIR/Vtdw04LL2Bm+HDCVBXTlm/tV061YCF5Vyn8ynuNEIKaFrxZ1U2Vws1z9ugLo1LyOgS3oNxIzDPDsSIy4xMU/8kD36Ex0db4++NX3V+lLwqsGjYQjvhsSOwWsweH1dHuMxWcpmiI3SPF2Gk8N3tbfoct6LY1Ytg96EHZUtmfgs16w=";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Fruit King";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Fruit King\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Fruta Rei\"}," +
                    "{\"lang\":\"ko\",\"name\":\"후르츠 킹\"}," +
                    "{\"lang\":\"th\",\"name\":\"ราชาผลไม้\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"钻石水果王\"}]";
            }
        }

        #endregion

        public FruitKingGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FruitKing;
            GameName                = "FruitKing";
        }
    }
}
