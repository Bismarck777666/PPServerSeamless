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
   
    class FlowerFortunesGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "147";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 25;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 200, 400, 800, 1200, 2000, 4000, 8000, 20000, 40000, 1, 2, 4, 8, 10, 30, 50, 100 };
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
                return "ed6ce8bf0c2d7bc1qv0wO3D27j+g8Xybe8jStHNqWolnjFkduizFB2FeRcrP9+uE+7Kt1dX5dyXTR6LSh5gKlIPeFkFx9ANK9hNhCk6RVqYCID7eXw0Xo2tNjkSbjEMVezydbWBjKjWiIMy07YS4pkj97FD+muDpmkYdfsUwrewmWPvff0PCGjgsWuK9jQ4Uv8FG0HnepfXwL3F7guXMws13TPn9K2mY60sk9pB7Y2laqZsdKc4g8gaf7OAnjlAC61nzcZsCM2569EKVMsrESm4uVFEIeONVynsOf9h73pvrLTv0gPllaEv+VsKoIixroqxYYFMGK+lSFJa6Ss5OKPfq9JbN5UdQ9NaCW7vuoqXQnmcd7gyGvMcz68jxs9e3hW4z80q4hczC/YzygE7p4AbT3HvkE5dgNEmOO+VIiokPgCQVY40yOkOaa47IvXsNY9Q1QCKJvXDK/osOvdZARdburZG5b8vKuAwQs9Ml+rX8YpZcu5Q62TcW9xQt/k7EIbf7D3Lzsq8VEE99QMltqOTPktcjOxMLnAU6cKIxcCp4YuSpK3JbJz96zUgFAJ+tdjmBa4X28iIfZM8Zi3rLz53K0iQ5q7Y4oyU60arZBlDayNRanj1ThEuo+b2qhuXqN0vTYfh4eyi7e2uXXQWrXfniECEo9NQ2YxzStNUogDVghqPkV3SSjuwFHp86fNuGlEC9/KfGGa/OzBWRJOb2A090YzUnJbyzP0cVfxuCUSCNYI5mVlArWXusUki9yGc09E0eAl/SZwlWu4t0EoYCUI/Gb+64xplIpzbkh5mPSByV5yXOIDNohosIr0d81rkgGqyXnsTNgsDBOjUxrM5nxSjsOGYNTEGIEVvjQwdhKY5QAvx0xHD0EcJGGS2RJfE4HeGCY4evF80T3K0a1E7Zh23jL2fEfVg/Ksi4IDl8iUEJGQwX1yJ/UnT0+O3uInC2WAN2lXDrObpf5hKODHoASKHHzRrT/qNT0M++gLQzEmbVK3ZlE09l79J9R4DLM4+gAVA8/udBhiuCtsUaYAPnJfIuAF7g/rKjwpcjZkjyaZ+tBxr5h9HCk1MZuFcjl4UxeQaSsu0rs3BNXkYTL+hnW5BkjCh/wv/ydNrf94RcCOHLqOy73ifUwb463KEGzqsWSvSyyRfWLi3qYtaBD4aTSHnTG88szs5PYssGOmQLGYsa8S8q+tInQgGDqwmi9eHFLvAXJQ9iwsKDbMequ+SpPOZT/2FESV4fFbG0F7GGpgq9NS6hK17j/pwl+2Q+TCWrXAa9rXUOPaAH2Y08eT+I35wTgViGaqvNcgwHXCDb0/BstSTstDDzjj6atCvlaKuJ5NJKFMptRExf1PWaNETdj2eS0Mas66v1NZ4xNxk933CRjIfJPtiom1o1FmdgEeXgFUghewUkn7nhrpMb9Uzi+Up+bn+9pPM/lPEyXtlCvC4YnEt4Atz1EtS7ijdaU9JUHkAgjQX/T8TsJ3EPA4e+XjMUm11j77f67zdnmVbEGpYoV4qTCP5OrsRYPixk4Y08KpmChFYmE1PYZocaGbWl0bL6yGCV6Kqwji0tZ7g67QYOrM2/27ya4Aoz411wGaKSuuc40euytxs6FgzYHxwoCfilxe6g3peauCFqk6rDP61djja/d8iKwDfl93x4s9rJwHYdporEnUdHxhQHf3IseBSo8bJ+mU4S32VqvTDWmbay2Bgxb7lb1XsDZg0nRj3Ldl1cJdYE367M2sZhIXk4NbUyTlpRdY3TbPhzF7PNnV9bM7V/TS2U5eBLJANPqj0fOivixhmAgI05NtNsw0sGLb9lrvStu7l3/qffInr+eYFHeA0oGiXPWEN+aWq4JBRNzDOxjrjKP2vP9nbctRfjnKPfiMTaVMewEIfOb0g7nVPkxXgA4S7IJWB8gdRpIDLGJpgIQ5kHhQ6r3MqABKIf1W0+oLJRIXBhaZZKaoJo7EMdBNDS5tg6Mh3Ox4RKEr6dL8miEMSp6Hqe7+8IgI2Fuubzmav40j9Lu2T7hF3W1ZKILx+3wnGF5BTi2DBqBr//F/4zYS8TTfbsN51EmDorTXPKwfSo0W3iYlf0smVb55paxdQn704JdlUk84QjhzF2mwl9R++QFp+KBsthETapIon8ITBwOW84JzIiFmxiYWxGZf3ONwoPFtXm+PTCqc3nZ62DfH8fhJAkkKectSdJePl03A3fmTYBxZLz9pZvDvNwHiLj9xqekGmI/VitIuMg3mMxuFRi5uM4/UF4EJkypnHkrZXKj6ZMwDUeAxzp9CA7dMVo9ZErqzZ10PXa0JkPSbW6B1QYXIzQ+64UKX/VJodgSpLS+0xjn6hqDRNV/pC9A5dSbQzxNDtQlfD8bqcyIlUhUoszYDxxnhtYkXSa9E0hw88PQE0GYiCt1y/ac8a+uMUSm0GGwZiB0Mx0Kt2ZKlVbthWS1RH8D7gMpe/MH3mvQw+MzCzIFzUFUmkxGvZHbXiDu1Jin/DORjk=";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "FlowerFortunes";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"zh-cn\",\"name\": \"花开富贵\"}," +
                    "{\"lang\": \"en\",\"name\": \"Flower Fortunes\"}," +
                    "{\"lang\": \"th\",\"name\": \"ดอกไม้นำโชค\"}]";
            }
        }
        #endregion

        public FlowerFortunesGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            _gameID                 = GAMEID.FlowerFortunes;
            GameName                = "FlowerFortunes";
        }
    }
}
