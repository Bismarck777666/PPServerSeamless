using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    public enum Currencies
    {
        USD = 0,    //미딸라
        EUR = 1,    //유로
        TND = 2,    //뛰니지 디나르
        KRW = 3,    //원
    }

    public class CurrencyObj
    {
        public string   CurrencyText    { get; set; }
        public string   CurrencySymbol  { get; set; }
        public int      Rate            { get; set; }
    }

    public class DicCurrencyInfo
    {
        public Dictionary<int, CurrencyObj> _currencyInfo = new Dictionary<int, CurrencyObj>()
        {
            { (int)Currencies.USD, new CurrencyObj(){ CurrencyText = "USD",CurrencySymbol = "$",  Rate = 1        } },
            { (int)Currencies.EUR, new CurrencyObj(){ CurrencyText = "EUR",CurrencySymbol = "€",  Rate = 1        } },
            { (int)Currencies.TND, new CurrencyObj(){ CurrencyText = "TND",CurrencySymbol = "TND",Rate = 1        } },
            { (int)Currencies.KRW, new CurrencyObj(){ CurrencyText = "KRW",CurrencySymbol = "₩",  Rate = 1000     } },
        };
    }
}

    
