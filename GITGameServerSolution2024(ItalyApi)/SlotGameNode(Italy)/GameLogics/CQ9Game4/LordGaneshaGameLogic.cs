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
   
    class LordGaneshaGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "195";
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
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 300, 600, 1250, 1800, 3000, 6000, 12500, 2, 5, 10, 20, 30 };
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
                return "FLWLxhrhszyag1ek9sHE9l/TIqbGfxIPmMiZNyb6mT7gJSsZdW82JCHiGD8+c5B1PpnZg6/QInm8XxFFTTzOHUTGc5i4WBH57iuqeL63f2u2R9Eivrop9lpkBHA/XbBbr6l/G7N99JeSKGirES2NtxbRjo5xJJsUvxTMLDevkBEQgYmBmN2UcD9/HOzUT7bWi4HJpVIOU/eTmaM8KARh0av4z3XqHxCcssarurtxYoTluZSfgWzmVGDnl/8azyAnrdGSScZ+V8VQiwNVJfwLDUs3KdaU2Gp9+GMz70XqwHEC7TKb2p63PXrfGdjWyvA24QDXqqfc6soScTNf8RkP14byIotmM3dXtVNEtc+YGBipnUA44ObV/isjcn+JiF1nnG9VmrfYgH+z8jDabZxcfmWSNBFakLVsCHKN2LdxEROE0reD952z890tzZKL2zU8CXp2tbJ/dw7UedlP0P56FTfHumLDkKd95IUSj+JB9UN3lb5KMyidYjPCW0dM7mLJs0XElpF/DGHMYD6AtGet5ld+0x7P8MhuW82wCscII4eWZAzFCBUJK42WSnIyn06m2IPDFP0iT/Gv2wUgIvjnyRl55HUPmt50LoNNUHKSfyUsCTxgpVcU81RuoSKUJLMyjiJgW/K+bwtVQAjj389THzSNDEvSNaAS9A7UKUipg8LhLn415k90nGj8rhYNDOyVyxrImgq4vc4NGiUWbb9DO3JqL3t5KaUP3EwvC+q2xpPC8RgLsOyomtVXUH9ri/em2LTMruTHI8iyDqoxzYNhDDnnfp/2GM+czl8mYZOJzpX6ZFJHbL4GjA6QALZXUjRKxDqaMUhQl8Doc4rNq+ezgmTfSa59Paxrj6ZXGfsO6YP18qmFLxlMMvqfJcRWb+XYndassu2HO9hZRSXgt8OcBWUfUadwHpj/thFpBFQ1+MZHJENwg2wIRmYeT9Hs1uakI5Op0Ja4kt0he+g5xIsNNQp/2UxL625WsZwSMNPf9NBU3T6PACTUGFCJO81oVU0xingAlCaH7oxBNH/f1XsZH8Bo9uv9VKlWRfEggYYH/y3aA9Z1l+7QzO1X9sBflmZqYo+NMfvBjILA8zQ1rxqUY6dHi3NfCUrZMZx7KM9UcE1KF/XzHxdEJdHIjbSHVhCrcqE+SPZe/Dx/DWA2fR/vQXjwFdVOcPyyDjJE5YKwaFV+lGvsbbStiTzN0ja6I86pmRMBlwfpw9id2dIASUX/QR5at8nGoWno/2Vm1CtrG1nD00cHtwJmDiI/HwlNfKjxbTgi7wXizWPLuyuAbHaQZI9x3V0Advh3FB/2SKHR6aLd6OG8UNzHt/e5Rul7X9390LkNUV9vMF7eu/O9rTXddXcaM4G/VT4Y3nmeWyAbxrcI7jBsCWhBCc5LB/YjmMw9I7FwEsERI0AODsFVQu5mBNs7AW4YF7Wc6sBs/m2RLzt1dXsXLFRGU7ALyTMc9qqWCU72KbOmhStEDVpfTJXUjgpCHDdMYZMkH6XgrCVJn3fuiVDZzY0oYWvlNaGPU4T+GbRdsM8oMedCtd1K+KCrER04cax+X//A+Jyo+gL8OX3xzIJSCzYY6IPamrmA4AHltZX8iWDVC8pfpVwfN/bPWqNNNr+MWBAGm8zmYXVZtP5b/myYlAN6pvFREnponn1GbRrP3PbcsKzP4G1NUxRmMTRytR3LVr3m4QZNlkuxSK/5B2K6LFJdZvDoVC57LNLcWF7cyPAWIoJsVyjdeM1deqZRxD2ICeLzWkZOdDtJ3Il0u1sDo9L1+Kl/e8jsHaF+0NqsDSfkb3z8pXYYsLq8WqVq2nXtjX34toRp6UmzPyo5AvcH8Lk0bf8PFJH5UsS09aKG7M39gt02VDRgEnN2UBV8bYrr2w/WczXeeJUpJiyEp/FbpMGRMQKAXvcIPqrWzpOrKMjjhCIEeWyP2490Zpc7Zqcz/R2enqoSlGRrrJvh9MVSVfdoXHERuqQ9FxabkaCaBGU4bmgAMydDWXaxZyTpQEI0BzLIG5S9MyK/iNYT9at9hUvwLAOVHa8tqCBsX6gWnmL/LRX54q4fYTMpzgr2mOTMWvNbZ7gP6cd1rC4wCym4HXcJA0tdJ9KCcYMbwVFIep46T218A0zamU2inGM3/OTIiHhsbHgbdG7xuadhlFP/tsZlk4eYoYUqXUA2GBpFR6ZXTwXSeLbDYhBP0fH5ONsEx37AoYcrFkqoyoQYkgkt9qOq9frEZ6bUAlHpPqQQnPZLcf1ULzCFN0DQQ5x4QvfmE3FKNDt2HH6f5XvmLTZxshty+Bb2qOZ6NRyJCnCoc7YGEVjpQGejq6YFu4jakN5XsZI+gvSrH63NF5VC73E7XxVYdKAQVFGpZU4sdmeQYhItzshjhdXLKdq8JkDjCQulKWGbZgGpEaYeGmBzOlAGWeGbIbtafLO7qMQp/bYrGINBV71liMaYYFIO/Tx3HWB5zugbsRzHKdH59kLcbywmVx0nTeuwHAg8lz6kyt+csSkAT/BUnSt0vw2KwZK6cRphraz74q/16lOuhOQNwRKu5gkoTtbtNDQEeCwzs86C+9gsixvgXqVK0jHNgHhYPsYtFt7TIKyfmNjvRDJ7ekY0Gy6CDbjYREe/0oukVA39PrTA2syEQ93M8QshxsyJV4htpOkP5imj3DS3Ff42e+OtFlYguZ6NxW3VdBMqBjCfFEnVmDLwz2QFCFgAjRe4CPfjDVZnjbIA3Z3WKf38Qw8hcaxASQEnm+ZbwRgLIXIKGDNCxueSpfq5uuK51eUGqXvv05uYz6wMoHEmyigm8tD91mQchmBVQWzRJAoivs/Z2HkRoCoSW6F+fLP6VcebcOUycQNwigNPGs2nYb0RSEmnAe0G41Dmmyq/qhOYhKrjE02xWjHiuPoePpZ7amIeP2hQF3g/rrQSFCj5fnpHdS3BrUlA4ol5tltrn+j9ucp3p/bdhFGm62NcRKvpUmkNp60TedGh9+tylBqrVC0e4tgzdw9nMu6hbUEiNRmom+dq33Ut1GTte39G/LJ0oiWz+ovcDSaCIKfEOg4xoco+NfkAKeBXD5qyrDSfIwtKCeccsqo457ygD55TCc0BggZzM4uHuu3LLYBKXlbzsVKdtDGlkrrA1acoGMOrTZDUurNTF+tt95vaqCiKfANhWFH28YJ9yQDYfqX6Md4D8MPWmg+Xkz+8GrySmzNPGA1VB5aCq7DG9asuJpeoxf2/wDOeL9ZVvr84xoWcnc7xCvXnutisuo5YqtWMO66noKoAmCcO7nZ9rz6vSsWRz/y+aeS4XGdU3Y0qihox/+QXtlcRcMcvqoEu52jgMWDF0TjQYlv54tDclQ1SjkTrPYvwBZcvwhP6AjhrqtFuDD3UCzjEK4QugIFhyYo7aQ0t1oEyQ5NnYDdmP75WFxdTlzQoEgF8r1Tmz+aZ6n6YldKLIHxEj0gOrbZVvjZ87dQkP00qW46zTCZ7fyiReSAO13enxW0kwNp+NXbJHO1yVAaaTmHflg7XXvYPbhvsS0n8N9p7tHZ76+hp9AzKTu4yCz93JUr5XQpDWueJMD4hTS9jDPBjl9jcwvDMRDn6Xx0Nkn4cOLrU0YE8wNVt8IuwwB8qxBcrNmPUTgvqEEwCCaJa1aaacPuUhX/wqC127rUWFC7YB8xhdEMWCKWO4rkLPBXcbPhE/yLVCZOYNYr2CLnMqHqGgIDvhDAqrHWxVWOodgNDY4J+/Pmc+kCf+awc6PM/DJKATFe/EG03al5ars1c50aHVLgB7pMxwMjhQo/nl1UeCplaVrubRZT3d9GFwmN2g4v5/i1jpG6PxN+a5g5ACa36Xb8SJUn+/TFRlHjTBwzHHos6Xmq4MVKonGAJNpvL8WVDv1M1XbU0l5LANQSue4j+IB5yFDq6KBtyLRKCNdptTJb28KWm7x7WzHOkZJdM7K7x6+ai9fEpmoll2XQDlFzeBfkaRgQn3yr+TUTdnEJe8jfncrOxjmV3Lrl/cS0vESewVlCqjfPtiZeAdY5FjtBlYtQ0/tsqtM1HEnXKFCO2Qi4OCEMu0tLkt/9iwmNw02jevk6QrtudQW0iJAIgQgaUChDHQp5zpXpQ2uYf4eJof0zHjT1rjLDmNELymGz+Ir4GRburJjGnX1O13ILa17T2Z8S58m5UDpx8CnlZX0UIN4z+qTGW+6cRGH/W+cFnuGq1AatEqIH/u3cZnz/shMl2p1LFFFmod8o8XPb56eTQis5zbEoy+vFpGchZag7zTFLlNg8305QIO2wF45WU9OaFwrYWrnCrtLGUGXcnh+ZQP1p0WhU0DpcH7V5bqm53Qaaw+ZhIJm5ME2sSIUcSRdn3f6ymsPmgsX8PlAd8HFtnZRvJcUFqVQDTLRk0mqAPCiihQGfzLAnP+GY/IO+s+2EbniV60/59Vdipl22L/CsyoB9rqF7Pm5zP8m1HDYqexVhUt/Esf8fyqYeuEh77l5PSLxBOi1DACMTUaZ/XdQR2GL5KOO+jAiFz7s6cZX4WOr1K4/IEHjQ4xPngyoj6SBp8RZOnimQBoN5CyX7rL511hCJiO2/CuYlRWtGf+5KDELZcieiyqC2at5yU/obS/Y/z1+4S1nzqnFzx8y6b8dGkrHTB8AHAazgthpbpVboRDgZ+m0oByWXCN+iiKnkmD2UQee6xSgdnim974U4jEDti/EMJYulp7Su/rTZELobzkbk2hI8LMYeaZ7pYTgC5hhFNCSCdI7GDOVfKxQxg7peOpJQPvTdcHR4JDUV0FslZr4pf159oCo7K0OoRNkgiDRgt37ITupS3AYrKtxFsoOGCMwdirR5jb21Ft1yo6YzJYwYs9dmBchYH+s01L+rZbGRYJj7Rb+ap8paZZ1D9iipjsEuw5Zi2zB1bz/I9t2AzuKMbVU4R5iVZUOJiVLNgy32at4MGw0P0ti/WKcL4NTg0EryE+JKF5lXG9ZNnq30vDAmtxrl31wTY+D5SpgDWeQYqQYq1XDnCcy6z8cwtQobpRTvSSkrS5KkA3ZT4EQewyhVdHlnVq6ZMkKBNRKOzvp31pMbqeV04B/psX7bTlpMD5Xk/mill/pIWAn8vO42Natw4Z0y61jvbWHO80EI3Mhcgn1JXuR5WWHwSUVIF3oJysCndfAGu1BuAsV0ZOazVRrVXZKn06Fcu2SNwjMbqywk+TOarptOMfhrXNLWmyo1UZFTVZZEWl9fyDy5FKKbLJqt9jgPqy43MKTEsUh/1vA1udc0MpCW2M1CWzjWcNQbm8qgioh7e2qvrYxuzunN61WHIt/eJHZxj2fZOdI1/UNlPTDfW6Ppq08Tn2Mzii9u1Uq/kw2lOle1VWiBJqnabMYu7dcwRXatPG45XBf2iElJqXqo7aUJLPzeW0VNGTwjiiwo0FontYQ0jBGTgfU1XHekAgnovgTOmbJncQy9LOYeeWUta9Xm+NcqJMmeUxQoc9lDdbkyNu+t9Rb55/qu21d78Rlb5tAT0MGqEpuB1CLasw6/msvZ1vz9ODtZXeBMfukfFwOggMwMLHp2cciDl2r4wsPwYIVQym/7KYgm3QGt+CAm4w3uKO2pjxCng4Jq8bikGoHjU/uhF7X5VoDaQcv5Cm0jg9bHzyo+0IOTvjrNWXWwdGJvneEI4WQQ=";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "LordGanesha";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Lord Ganesha\"}," +
                    "{\"lang\":\"ko\",\"name\":\"가네샤\"}," +
                    "{\"lang\":\"th\",\"name\":\"พระพิฆเนศ\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"富贵象\"}]";
            }
        }
        #endregion

        public LordGaneshaGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.LordGanesha;
            GameName                = "LordGanesha";
        }
    }
}
