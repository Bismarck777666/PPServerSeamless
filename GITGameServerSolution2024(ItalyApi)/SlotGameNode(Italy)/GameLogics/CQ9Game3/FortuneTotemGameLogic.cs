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
   
    class FortuneTotemGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "148";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 60;
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
                return new int[] { 20, 30, 60, 120, 250, 360, 600, 1200, 2500, 1, 2, 5, 10 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 2500;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "d7d7b92e8f37df59wexuiCM1j9ASCslDYYxeo3Z74ExNLaTVZDW8308mm+FMkahN8HkrXC7CMcIv9NqNhPJDhgRRzFnwEOVsmQz1adJRUDHPp2RJIIqNz/UuQJey2t6RX7pOEN2qy3cqeMOiVaLRoQVJKCI3Im+ceU94u7LmZNRzfxcWN+3rXWgxFJRkOgTFqj13bWnB3jFDEugpXq9qXjItFdvJQ8B2lQhCDhsrBkPbZZvoYCxsy6BaDqI09Er6rW6i65vRcdfWxKyEK47AELO1qReY4cPRK7oF8TE+HRmRt9H5GTPkeCgxgPBJ3O0sN+zWg1s6oZV+IzaYnRLprJ1ad06eadBhViul8dMQ4LOnz/h7vRX83YZ/yw5SbpokErIErr8K86qWr6Jj4wAPCAo5N2aRb1olY55qqj/3Xhhpo1wJ3r8BNdJ1idPyQUHq7w8t7CglbYzfEDH4JHbS+GutAyEh9Yxhqq95Jkw6peLXpNOsN8SQo6s2hPss+fa8374uTNmfOZkveuEtMoL2iUnx6GnRDn7wHvZf+tJNa+WkK/YhgzmTzMK5/ip4npzZLzmV+PZWgUY4x8uqzeoUpJCvdt9Y2w6SwytRaWttEpFLRE4aQrF9freQJoYu4CIw5eht2HSIHLElQ2x2pjU7o/oon295ejxTylKe41iuQFkne0OR5zIsjSrPvXiCvCRTaVuQqJEFenms7lp4Nf/NGfcYimoSbQHGofOK/l5lXrGwD5bAKoBLziUjqOY/fbmnZ4BTz3CeLh817KdF7CbjmQP4uuPFAMh6JENazJ1oSoLz8C7kO9A789VUuDjsXXoP6DUydRAtX0FYGABwD7Ms6sprtA37dr9AQ8351hBqeEKwEuaNt6WD+rhhHhN+oaFiivjU8t/1q9RAbdphVIQq24Wu7MvZBIb/4uHIHktsFniI/huN3ysqzWIei6FELNBqCITgjgfb+r2gO4+Hvo+cDz58+feejvwCfxY/ZTwSCKimzo/jwZNTVPCCv3IswPSNoQbO12VN3XTRil74QZ3dHsUQdKB7nB0v7UvWa4IiGNfjm6v3ML0Y50Al8e/hJAiKTO44xz0m41V1mKGcRJWogr8gJu+qqV1CKOwcdTMkHr5/pKmGxJ2N1dFdf9chrdQc30jWUolfAzZkqkEmGHydkPvEPPvxH5ExadZlc9GDJ3w0T3rJyewDqfjkmvVV2gxoQlPWqDkKjOYcfkbE/vFaySepfMquAiw6jzVcvu1O7XIMP5Rn0nkJvwqDxZAx8/lRb2kMIN9YdhjV02d5WCQyeW+KsifbJVHlr86s7dc0YFlRnVkRHgntT1wMTZ4+lX275SxsAGhpNubS/6mwzrenVCO+tevbABsbv1gh8KAxICdbIes/A2AQ6dLBG47SfRSXKcVZ1D8KqY5ZD8A8ysGouu4TmdqKF5DxI5xj63TysHmD2pSV6j2VndcRTYzqRLkuP9giXZy1ADCIwPt4LvPYXxkzUi65E0oG0H8lOUh1A9HcVLJyIXZOZxeZkrPrI5S33CC24a2sLf4D01OIgAXE0Pz4aSEYY9uOFHhDybjM8BNhxCGq5/YB+BvLfJHL08FmZzhUT1CP+s07PWdqeaM4UgqIVGAwbqXflAulA3YyE8MQUV/8BaaxB3bn/0W7V3wYaPY/DqvaERIT/FuPCSj2SELAiABcfAg4w7aBHsQbyNikl8Kp/Ex92Rc4dclqQ/bXgU5jpbtCbJSRnqztS9YToAHmeQsBZI6ElPFtIAgOsVJiKHNhN5yruejTqLum1kvd3pkliAy8AWkcOln5d6G5udgyYAU3QLLLw934nUuXGYLdQCc9dMmc33OEnM55eRpg2uLtmbq1a6SyYiDVCSzktm4vIQC5p7Ub6W8+y/yk2fExD0KxZ14NEavHt7QiRO3Mf11droxH+DW8K115y+GH3olroeRD1KjOrYIDuZ3RT9ZE4OgU4Vk+pKGVVf0+k/6NlOVDvg8HsDNBFr/nDqaloeT5SufLg47f/VvUfaNGOGTtG0E6SLumCG52upRTa/nDeCM4O5QemQ369H2HlRnC+KAfDTU0TxEytmGrS/Fixof+1aZwM/aYo41JfNJ0rY/ytFjno3TB1qnV+DPZuK/J5D3LUaJSSWCm3Hd4/h7PD47pxhyvg1F0FVdHEEdCl193wJ3238PrNZP6xzp8+7nVgUe72SMPVph+hVrkFWmw6xpuqeXLoighyZwMYxno0MO3cIT7n/upgbdnvN/Fqgo0lYXec6mD5LyqIHuMC4lySSnpBfW6orl73L/nrgHyLZh9cwTTXF/CnGTBgjWF+ld2sozleKTQHrGq9QbmmRrFpA+w+TnAe8xb+nO5cu9E2QPz8jp/5WdKipN8FsK7qSf10cS94cVqZ43ER+6TrT3P3PqafLlcEmxuK5uOdvs4Yqka+11LYUTKjHZBFj7hlxsGgw4FuD2a88sMvpeLpQgjbQuVuHLWD6UQHUhmufwkoqPoLHyTAE6N2ZywdRsWBXpKl2VWL7aDqIFRSRXFrxQzuRXCER9ZSPY4Gy4yPnam2mS1Evi8F55LbCJZd88NADMjmmEFH957L562Kn3Ba04+sl9CtugheRNP1mzulM9nx7feOLbZs39q18jcgunF5peYXjNkJEkucYpBQ/JPSPd99Xpy2kSzk2OnIrnh0KQyEpQJcxUTfYxtxDGMjXfsErGUVQt9BHutEBDG5uF6nF7Tdh12Gb9y3pK5JlvcaQu99uQgqKXTLMYW0tHrdZi5CWtbAqmLy9e4Bzz3VB1Q/5cGhSiTxHvtdzYxdI/jtAhFXUDqY87YzcGr/akRjbhHBGestNHU2zZHMVcEFJsyJOKi7vjc99OOt+mcpneA7cBT8fd0tZu8qbbELEH/sM6ZB+EFgxs3b+O1/BcnDZwf+pMLqHUO/pAuEv+JZBs5AwPOb7NtmCfkFyCa7fpuB3Z/dnCAdRVLsOpsq9TuP8P0J1Z48kQNM5IgHu1vUT9k/7yrhM5mcwGqVrgTv9e/nG2dHeAFKa7reADCphNm7sE7yeDi/GreXdTMPxuEFRUL7vplP1xv9mV3xB+MagD5VJ35S+5f5qaAdjOmFVJEY0iFz8bMi3K7MeqtGi61XozuDQccqLLjR/Mz5/QcJ6ED+hgIuCtwY5akpoobvg74fXKnNdtsdmJX0bDYmnUCvHGUJEj9X+fNFoP9wFoKDCGmhNENBZfji153AW1P/nm/0z412dt1mu2GBYUZ6vvyRzFAFwUjCa2b7Z98HvYF2uaSecJSzBMYoaU+VNvWi+l8TuYHo3oI3B/CYwdI5//uzWO1UMwhRh0z/24o7671JClI7KGxFg9MCBD+3UiQYhdYH9XtZEHi8pa6XYaaUjPd6bmgw2hn4BIINq5kSyz2CXy6bA6jdN20GRbtIARczyUJqk6vZSSgxTT94lVmoIJjc0bM7gR2MYsKEoL430roFJpjNtoDTRD630tAoXBobFfvyx7oQDd+EhqnqXtXv+vU9xRx7RYQHKyv5QgH8HKLDq+2RBLVc59hn4Y+s4Ci9x1McCZzc3vIvvblvvtEdtFY1qSisWmQpAmJkmd96eNrBNEfsWfFABcxN8Cn2YQmTulevonAf9p2V+HEjmB7UqtR9j5kDyK7CDAagCKbuQvNkDRUx1bIuqq4P9ySXtTXsJJ5Dry01Q/6lAqhaXdFe+lStW+iFVAIY9v+4w2Q/zNHs80XwzLLUffAON2w9adV7u55m+S8vv64JeqaFd7bGgzGjdb7igngmYOtq20Ev1T0qAU8P4EFG75R7QPNFKZKgkwiyhtGEKugHEq9KsJcX+eyo83EwjhHrHNvPIvmd5V1MluO0vqBhXRXdcX62YdeUoF4lmQerKDj8seRnleDJXEXCqtx5gdEoouBmOVC4SQ8YE8D8odXdJJo3xKJIuG4KNVGTnkpmuKzCiUX/JVdOsdFKlMPHE5gHoPiG7Aow00olulGGCzpaWXE0XHzFbBGhI0FojRrEJzVwrdaXFL0PJKSYW6Z5nQJ4sk14JHzzSXp7FG0TTFlT0AtalitsYI9uqW6qmFH3zpnhouTsHqoeWTezuwdvs1j2rHmkFNrL5V6qnbw+7vveVc6UGpvgbincQdMyrXIaV/leoXQgSojybfbogSsyV7MGRp8oYuGwao81SLzgt7axnV5zgxmYJREgVYD1zGrGpuusbk4RrqNL6chOnV/8i/HNinWnPmLmpEEZrVblAPAMOdmuI3ctHZOvpsMHgwxgmX7jbF5vdj9A0DpFzdER6x6JxykgQrzeuLftzbIs8+dhyAa+ktLmQEFNFw30PpodQTwp/FfrAW+rx6DXbRhPgZEfU6uYFzAO0Ai6Laq0+3RzrZ4cdvkpnblRFXvR+QxBTDIKJBSUhxNKhviQ9YblKdy1wH08kT0EXTiQ4zfLlsw7HqnYawCaMugSUZvV8uRIoROwfSadDMK65A1Iy55TX+x5YHOB4fMIGH1uB5jJN3EqenlNbo9sUc2qw9pLvC2e29t7JjDYpwUdRf541zSsHdHMfzRBEBrcS8PuDHAE3iV9aIaBCOZRi3NcUfqq2a3y2rAeQ0gsJAJs+nFLK9pEeV/GbIWWjTTai1RcmXELC/CdNpXAbylJ7aQDuff0N2jI44uKj1JOFUx4GdtWdDB9GrCSdka0S6epzp0wBqNGMpLJ3Z3CoJe4r2xC7E9apExPtfbbk9DPyMUSGXzKYG/cD28viKRHQlVc8WepOadIIfZFffVKBcHAyGAeQ2rQjUuXsIthjKYn24F57v2rzM/moO5JjXueYpH+NebK0FmynzesEzN/1Cll59o34Va3ay7jo5KFk2R6GXs0ypfajAJLrA4Qv4fF/JmUqIuoiHyBjqEJbLNiXsrkpe/SiJmcmEgGGG5isNhetmBePQIVqVA446MAUaCG9wdYfu/hhG7dqIL";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Fortune Totem";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Fortune Totem\"}," +
                    "{\"lang\":\"ko\",\"name\":\"Fortune Totem\"}," +
                    "{\"lang\":\"th\",\"name\":\"โทเทมนำโชค\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"有如神柱\"}]";
            }
        }

        #endregion

        public FortuneTotemGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FortuneTotem;
            GameName                = "FortuneTotem";
        }
    }
}
