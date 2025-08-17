using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvertBatch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Converter.Instance.startConvert().Wait();
        }
    }
}
