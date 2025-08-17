using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BetMoneyFetcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Fetcher.Instance.fetch("BRL").Wait();
            //HandFetcher.Instance.fetch("BRL", 1).Wait();        //1:1


            //HandFetcher.Instance.fetch("THB", 5).Wait();        //5:1
            //HandFetcher.Instance.fetch("USD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("EUR", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("TND", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("TRY", 10).Wait();       //10:1
            //HandFetcher.Instance.fetch("MYR", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("IDR", 1000).Wait();     //1000:1
            //HandFetcher.Instance.fetch("RT", 5).Wait();         //5:1
            //HandFetcher.Instance.fetch("JC", 5).Wait();         //5:1
            //HandFetcher.Instance.fetch("GC", 5).Wait();         //5:1
            //HandFetcher.Instance.fetch("CHF", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("MAD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("AED", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("SYP", 1000).Wait();     //1000:1
            //HandFetcher.Instance.fetch("IQD", 1000).Wait();     //1000:1
            //HandFetcher.Instance.fetch("ARS", 1000).Wait();     //1000:1
            //HandFetcher.Instance.fetch("EGP", 10).Wait();       //10:1
            //HandFetcher.Instance.fetch("BWP", 10).Wait();       //10:1
            //HandFetcher.Instance.fetch("ZAR", 10).Wait();       //10:1
            //HandFetcher.Instance.fetch("RUB", 10).Wait();       //10:1
            //HandFetcher.Instance.fetch("UAH", 10).Wait();       //10:1
            //HandFetcher.Instance.fetch("NZD", 1).Wait();        //1:1 
            //HandFetcher.Instance.fetch("AUD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("CAD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("DKK", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("SEK", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("RON", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("ILS", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("NAD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("GHS", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("PEN", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("SZL", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("MXN", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("HKD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("SGD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("PHP", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("AZN", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("GBP", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("BOB", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("GEL", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("KWD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("PLN", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("TMT", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("TVD", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("BAM", 1).Wait();        //1:1
            //HandFetcher.Instance.fetch("LBP", 10000).Wait();    //10000:1
            //HandFetcher.Instance.fetch("MNT", 500).Wait();      //500:1
            //HandFetcher.Instance.fetch("INR", 50).Wait();       //50:1
            //HandFetcher.Instance.fetch("JPY", 5).Wait();        //5:1
            //HandFetcher.Instance.fetch("MMK", 500).Wait();      //500:1
            //HandFetcher.Instance.fetch("KRW", 500).Wait();      //500:1
            //HandFetcher.Instance.fetch("VND", 5000).Wait();     //5000:1
            HandFetcher.Instance.fetch("LKR", 10).Wait();     //10:1
            HandFetcher.Instance.fetch("KZT", 50).Wait();     //50:1
            HandFetcher.Instance.fetch("PKR", 50).Wait();     //50:1
            HandFetcher.Instance.fetch("KES", 50).Wait();     //50:1
            HandFetcher.Instance.fetch("BYN", 1).Wait();     //1:1
        }
    }
}
