using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace AkkaTest
{
    public class CurrencyPriceChangeHandlerActor
       : TypedActor, IHandle<CurrencyPriceChangeMessage>
    {
        public CurrencyPriceChangeHandlerActor()
        {

        }

        public void Handle(CurrencyPriceChangeMessage message)
        {
            Console.WriteLine($"{Context.Self.Path} received: {message}");
        }
    }
}
