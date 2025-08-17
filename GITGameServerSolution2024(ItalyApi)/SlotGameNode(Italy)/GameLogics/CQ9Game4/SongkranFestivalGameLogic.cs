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
   
    class SongkranFestivalGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "199";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 20;
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
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 1, 2, 5, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 8000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "n5zeO2Fpk3Ytgrl5NdEg0ys6MBRdWzV8xyG3L/kqNsQpzfN+mR0Dyfto+ky0Tx/TPpfoTymgnpo7+GvlInNEXljqKRbTIiG6BiHmtmvjP2UF1eAcsc+JJPut8nFt+vdFmJ9J9PEtqibVVwyD8Q+h5UmE0kq/UdZyY275l8Hb5Wxsf0rph0IKSLIyeDuxPTOFHja+8emovKbjunWzoBy+mkMf3ceLn9k5jr5O9UGNlq5hHmTTet5xxI2/F0Ps2D/EYh+YBkcxDSh85j8YiP3nTLpu/ltMNGD6nGOYhenAbQJgI+K8Fm82pL9KUh/ImCODLZlXXC1dWAcEfnJIDl3y0vZO7oEzUTaR/CLfJANndt2eoel8iTBJbO4QdbxPPSyG1vEXEnO3aRbNfNql2sY5XEKlS5g3CevjLrUqf17m3Gy0sFkfFf/lSBH86o9GZtTFMinHEs6CJG2TGCzNuDYjSl8XIT9yHD0UW4raMGwZu8o+ft7YhHAXRO53lnzbqdZipVRwgin62Ao/ikPPl7HxqYHdmAnZoSXzTnht3I8IkvhjHwcZddeoC9i8I6hVwOv1AxOzrUZsKqJ4URrWL0GaRg98Hv0GoxmM6aahMkp16BUAO331bjgMBfshfC6c8MpXFJt7FXae1lbYEwrw6u88CXAYWXFa8cho0Rn++/ETSeCgXoElqcFLBeKcmptrWNrXJOP6nceZ/FZNDYGYfNApN8iFU4M2LgL2+xwUQvmnb4n7OxLtdLeJKwEJ83/ivDYuROvlGLhHYDF6UkwyeFYkO5ttCps4JwRO/yyGy7YyI1nr4sd3LHRPTGpBqhao+610cGDeFlhv51jftZQVXMvKo74ljd6K7hBW3vVioSCe3yH1od693uHs6u0uGbFDWPwg8rWO6TDgDx8eS6BVXTGcFsV8TUINm0JwNcx8fQvXllCcFPnExjc94l/f8GJfaavZGdIJUDNBRKH7Ma2NSjRN9tg5oUzb1NA0r/t/AVcybe6rXwiuPmyDBpa1DNIhUJa9gd9wlyVR/waygKwulq26OQclJzNcGsbYfyxg/ldXOnOeasTkY4vl7/V17ey5uRDaIY4M6jaN2zgWA+lYtsNiMPDyZfWZtm6YHfeWt8P7QJ565ePbqtLIovZkfMttkWo2WGP3wvMK83GcSctPgR/0hFYpBVpou6nLo+vBvQO2SMcmSRZmHRj3rWw4jSQn2ziepCSEU3dmcFUKWHT5GC+aMachQbhj0Lho/l/97fyOWqHuNMk2RkqPVDUGHxUHKDnkR71tiwpjvpzgrFg65/6lqTLvw51YoPmij6j1pJp7zLZQyrSsF3CretTx9zbb59hE4oC0eZZFWJhV66YGLikIH1iMfbk7GzsOPMcnSgW6BkFusjk6GCr2SLYRErnVoLek+nITx3BNmG9pWKm0IQUB1CdSrsYGrhLEhhvVebtampzPIOH8QSpoFGBubbsdeMj4uPgQSRj5jDGRaxhFys1j3BQwt3pfmHe4XCnpSMglYmVXOvR5m6wg5kKqB0kQnpD7GDl4BtCg93OXG4cXdOkw769wh0C401pEOFBx9Wsf2TZaZ9ZZC9S4VcKVPHXZ6/02dUw4GYlVuoVCAql+ee0fawlndADF2lvQS+iDVuz0uVEJ6er9MHr73GAAK02yWBrBdi8VdlLG0IJYKzWAOUoLNaO/Qgjio1L+H2A+QP/kyKdtf8D+xhvZC5+e/j2kWp5rxue1vANhi/fx59u/Epqe0qnWyz7zQ2lDqcLJ51cUozJ6RUBcerMuyyJb6mvd5xOTWMV4zyrukNBLTmb0vmMrPcwL9xZx9vGDCVh8zIsSgZykVYq6g2cOQ1kqC3gNIrzBx7sdTU41UsYo9sg3JDClL2ny6t1JbAyFNZAqvDkW8gpgBwqGLHljy9FZDCFeRQa+wdYksErOUkUYwWm7CKhNeUPm7Yk2CZDan9EUGuDx5uCNsU5aWVC4pkRIrm9XWP5yUnAwZjlUBOaj+GmDqtXU9UV4rBOuck3P+s5oB9qxWbldfjKyY6p/ogbQPFnCbFV3Uend04Q107MYNwqrGd1A2E2OD9y1Y+U3jyfASoNodapAk8gvmNnV+WlYkBxOTQEaZtmMN9o6TTxNgNbitU/1bpggBVauoA8D4IyHI5h2UlTEey2dpZlKzZZwTPyDTyvZBrq+kyAY7ErVj4QyoWkGcr5kg3SeO04DhKT6lv6gVUAZotlHCkYVXoj4VZe+XuHnUCKP/cRXpLMfHBDAtlZsOlu7AOy5SXb2DP+cq0ECtQKpcjfqbPlVrP+RLx6L1Bdw4Tom9jnAfj6izioU0PaJRnEsuPp7GVIXAIzeBv4tg9BpsO8rcpkhXWbWveLLcI2vNs6aJX83aPMHyH8VsdqAHREKPGjzUitUqKULiKL240YToOpZqkQ6+R6ka98DQQ+EKGglsaYFVz1t+9JD1w9syZmHOPbY2hWe9JGHOxUKewNoS84UWisCIxoGxSPHhBqUS+udlw70+wzhK2nghJI+rHppfi0XcYDW+gwjyrKOGb20NUQDGRirpUzG8/gA1QZKFVY0v4D+/r+2ycZoYNyR98Lm64ZuWpkpMW4W1xQ7gg2MYMH1nnOuRbh6vlPRlpdNdjhl+ixQSUQXLh1clhgcKnD8J+kZaut/9T6ee1sLFRWJALVnOAThhcL2N/4WDp8f/lv/ykoslAXKDHYiL/lzMyGUgOEoNua8Pc7lmnfNejsFtNqYPUITS64Cq9IDW/yI7W04rGjRYyAldsZTj0jQEkI0Ctbi2mfvpkhnKT1EQRvKrC0GLlbYy5+EmOEuWYqYC/stUa2ual2YvRrjJN+unI3kGfMqBWHmrVFLcACYMGNkXNA2u8f7Lvl9zmxyji1W0nEPveXoUaFhOzWaJADza12vwpykDXoKs15wXLccm6Y6jHeG9FXRdsAGlg5kHcTKtjotuOzSqS01+DFP5cFmk031RScyX/JLmhm4k2tw978YlUZgoI87VkdfNQgOytmnVo87W0Vy2gcdtWHr/ppkFRGiMi2PyMAZ+hyC2Y+YQrMW66kL6wMXtgPrg7VZDIgn6WpUaqfOrEP2IzAHYT0jPDgdN+fsSF/OcB30JRDoXkaYfFXHpvogA7gd6//2ZH9No2FIk2nd2A9ty8ZN6GfPS310BnivZuaI2YZ7LgK0oQ1RnrkmPSZST3oSkrUMEtSS2fecXmGw51BhTGa1FLhMmakOveRUMGB0w9d7HM/DTSdUA83VJdHoekPmw3VJBmpOZLxvUctIH/SBCIs+cjvo66mPVu0O/+J3ouXbiSdJNVNDsBeNv41Za5UtllyBrJqIxDsAZguqVH2BseKFJCHcl4Zbz8MjtF0ImT66GPfOUSZIRXtjBuB5Qc+fjuI/3LOfa8Hd+g4X3VMr5Ow4C6FLdNgVn1HBEaaaCkjaNxUqVn8wnNn9Q/ognDkdVx+3c0N972K3nKkJkRbElLBizewcNjMJonrFjxJng95c0glyMuvvBaJ+VQE14mYDWLpuEYdTCFAYD7NP/hJ92MS4cJEtsl8mS7OofVPmCKuYZD0Xjfz0KBMh5TouCxX5EXdRhxnBHm6rJqyzhRlkmHja/S/YyxXi72W8z6aDv9X1Un3FX6klpxLrRj7D//rda4N1QatbTDYd+HLxM1hOP5l/X78uBZPjhNab5S7Ksp0ijsYcel/CZDVE3l7A8Ugbxbwa2QTwsVSh/z4/3eh8w234XAV8fN4ExOXXEMPH7A9JEeR6RSicKinDJm1tE/K1U1bnE3YN0f1EZrupmjEhB+Jca9ZGZA5xPNOCSfyQqWxGZffJpG+bSLmgL2W8Aip8UYpL9+4ErSrYyX8HByhmZc/BBXCzfo1ZIulbFUWejOxxStIZRYtW7bC3EsHT4osa5vpKK0+ARPX4w5HuwSTRUhoXjRRUu1YvPO+xVounj14M/37kxyXOMAMxHGhrGMNIZx4LeFjEjM4j4YHbrq0dH8GvCdBb1fcYxvzpFCbpw4ore6xnIqkNKWG5bT4NPGOVB5JDfCMv2+G90TRC2WDQUBlQ0OtMPyedy9M5SEYHO8YMp2ZCCI4xyczILelmNnAxTuFsK5Os8XPQsocHZEe7ppoYf2C+FQUsdPlZQBi4PoXKI/kfh+6+mbSB3m4MqUViYX+CyKQyXi0zv4i138zcIUVlny1eh3mSOCqzXruveWVNfAXXx3ZpIvnrPechJ420FMQb3fQ/vAzwzdzaemDJV+aJOxQrKremZSs/ZjEDNUXAzq+bcfmuEJf3ieQajYqdO3jnuhLF8bXxG/d2C3pXalz//d9VhcroONCKmqxJg3hKp1Nhk/HLRJLnsxRq4aQxmcDctJDvVu/UUTwnVVIZ/2gCuwxi+lNSpWc0TvnP6J2g/Q==";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Songkran Festival";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Songkran Festival\"}," +
                    "{\"lang\":\"ko\",\"name\":\"송크란 축제\"}," +
                    "{\"lang\":\"th\",\"name\":\"เทศกาล สงกรานต์\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"欢乐泼水节\"}]";
            }
        }
        #endregion

        public SongkranFestivalGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.SongkranFestival;
            GameName                = "SongkranFestival";
        }
    }
}
