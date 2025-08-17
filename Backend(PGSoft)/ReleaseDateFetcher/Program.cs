using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetMoneyFetcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Fetcher.Instance.fetch().Wait();
        }
    }
}
