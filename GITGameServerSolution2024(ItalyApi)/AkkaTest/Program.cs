using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
namespace AkkaTest
{
    class Program
    {
        static void Main(string[] args)
        {

            long totla = (long) DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            string strTest = Guid.NewGuid().ToString().Replace("-", "");

            string strData = File.ReadAllText("1.txt");

            string strWriteData = "";
            dynamic jsonObject = JsonConvert.DeserializeObject(strData);

            int index = 1;
            foreach(dynamic lobbyCategory in jsonObject["lobbyCategories"])
            {
                if (lobbyCategory["categorySymbol"] != "all")
                    continue;

                foreach(dynamic gameData in lobbyCategory["lobbyGames"])
                {
                    string strGameName = gameData["name"];
                    string strGameSymbol = gameData["symbol"];

                    strWriteData += string.Format("{2},{0},\"{1}\"\r\n", strGameSymbol, strGameName, index);
                    index++;
                }
            }

            File.WriteAllText("1.csv", strWriteData);
            Console.Title = "Akka .NET Consistent Hashing";

            var random = new Random();
            var currencyPairs = new string[]
            {
                "EUR/GBP",
                "USD/CAD",
                "NZD/JPY",
                "EUR/USD",
                "USD/JPY",
                "NZD/EUR"
            };

            using (var actorSystem = ActorSystem.Create("MyActorSystem"))
            {

                var worker1 = actorSystem.ActorOf(Akka.Actor.Props.Create(() => new CurrencyPriceChangeHandlerActor()), "a");
                var worker2 = actorSystem.ActorOf(Akka.Actor.Props.Create(() => new CurrencyPriceChangeHandlerActor()), "b");
                var worker3 = actorSystem.ActorOf(Akka.Actor.Props.Create(() => new CurrencyPriceChangeHandlerActor()), "c");
                var worker4 = actorSystem.ActorOf(Akka.Actor.Props.Create(() => new CurrencyPriceChangeHandlerActor()), "d");

                string[] workers = new string[] { "/user/a", "/user/b", "/user/c" };

                var router = actorSystem.ActorOf(Props.Empty.WithRouter(new ConsistentHashingGroup(workers)), "orderbooks");

                for (int i = 0; i < 20; i++)
                    SendRandomMessage(router, random, currencyPairs);

                Console.WriteLine("Added d");

                Routee routee = new ActorSelectionRoutee(actorSystem.ActorSelection("/user/d"));
                router.Tell(new AddRoutee(routee));

                for (int i = 0; i < 20; i++)
                    SendRandomMessage(router, random, currencyPairs);

                Console.ReadLine();
            }

        }
        private static void SendRandomMessage(IActorRef router, Random random,
            string[] currencyPairs)
        {
            var randomDelay = random.Next(100, 1500);
            var randomCurrencyId = random.Next(0, currencyPairs.Length);
            var randomPrice = Convert.ToDecimal(random.NextDouble());

            var currencyPair = currencyPairs[randomCurrencyId];

            var message = new CurrencyPriceChangeMessage(
                currencyPair, randomPrice);
            router.Tell(message);

            Thread.Sleep(TimeSpan.FromMilliseconds(randomDelay));
        }
    }
}
