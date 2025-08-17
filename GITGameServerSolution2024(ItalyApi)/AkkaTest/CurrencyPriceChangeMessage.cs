using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Routing;

namespace AkkaTest
{
    public class CurrencyPriceChangeMessage : IConsistentHashable
    {
        public string CurrencyPair { get; }
        public decimal Price { get; }

        public object ConsistentHashKey
        {
            get
            {
                return this.CurrencyPair;
            }
        }

        public CurrencyPriceChangeMessage(string currencyPair, decimal price)
        {
            this.CurrencyPair = currencyPair;
            this.Price = price;
        }

        public override string ToString()
        {
            return $"{this.CurrencyPair}: {this.Price}";
        }
    }
}
