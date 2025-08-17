using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    public enum Currencies
    {
        USD     = 0,
        EUR     = 1,
        TND     = 2, 
        TRY     = 3,
        MAD     = 4,
        BRL     = 5,
        RTS     = 6, 
        KWD     = 7,
        CHF     = 8,   
        EGP     = 9,  
        PEN     = 10,  
        AED     = 11, 
        GBP     = 12, 
        BOB     = 13, 
        NGN     = 14,
        ZMW     = 15, 
        ARS     = 16,
        COP     = 17,
        CRC     = 18, 
        DZD     = 19, 
        XAF     = 20,
        XOF     = 21,
        GHS     = 22,
        GNF     = 23, 
        KES     = 24, 
        RWF     = 25,
        SYP     = 26, 
        IQD     = 27,
        UAH     = 28,
        RUB     = 29, 
        KQD     = 30,
        IDR     = 31, 
        ZWL     = 32, 
        ZAR     = 33, 
        NAD     = 34, 
        AOA     = 35,
        MZN     = 36,  
        AUD     = 37,  
        CAD     = 38,  
        NZD     = 39,
        LBP     = 40, 
        COUNT   = 41,
    }

    public class CurrencyObj
    {
        public string   CurrencyText    { get; set; }
        public string   CurrencySymbol  { get; set; }
        public int      Rate            { get; set; }
    }

    public class DicCurrencyInfo
    {
        private static  DicCurrencyInfo _sInstance  = new DicCurrencyInfo();
        public static   DicCurrencyInfo Instance    => _sInstance;

        public Dictionary<Currencies, CurrencyObj> _currencyInfo = new Dictionary<Currencies, CurrencyObj>()
        {
            { Currencies.USD, new CurrencyObj(){ CurrencyText = "USD",CurrencySymbol = "$",     Rate = 1        } },
            { Currencies.EUR, new CurrencyObj(){ CurrencyText = "EUR",CurrencySymbol = "€",     Rate = 1        } },
            { Currencies.TND, new CurrencyObj(){ CurrencyText = "TND",CurrencySymbol = "TND",   Rate = 1        } },
            { Currencies.TRY, new CurrencyObj(){ CurrencyText = "TRY",CurrencySymbol = "₺",     Rate = 10       } },
            { Currencies.MAD, new CurrencyObj(){ CurrencyText = "MAD",CurrencySymbol = "MAD",   Rate = 10       } },
            { Currencies.BRL, new CurrencyObj(){ CurrencyText = "BRL",CurrencySymbol = "R$",    Rate = 1        } },
            { Currencies.RTS, new CurrencyObj(){ CurrencyText = "RTS",CurrencySymbol = "RT",    Rate = 1        } },
            { Currencies.KWD, new CurrencyObj(){ CurrencyText = "KWD",CurrencySymbol = "KWD",   Rate = 1        } },
            { Currencies.CHF, new CurrencyObj(){ CurrencyText = "CHF",CurrencySymbol = "CHF",   Rate = 1        } },
            { Currencies.EGP, new CurrencyObj(){ CurrencyText = "EGP",CurrencySymbol = "EGP",   Rate = 10       } },
            { Currencies.PEN, new CurrencyObj(){ CurrencyText = "PEN",CurrencySymbol = "PEN",   Rate = 1        } },
            { Currencies.AED, new CurrencyObj(){ CurrencyText = "AED",CurrencySymbol = "AED",   Rate = 1        } },
            { Currencies.GBP, new CurrencyObj(){ CurrencyText = "GBP",CurrencySymbol = "GBP",   Rate = 1        } },
            { Currencies.BOB, new CurrencyObj(){ CurrencyText = "BOB",CurrencySymbol = "BOB",   Rate = 1        } },
            { Currencies.NGN, new CurrencyObj(){ CurrencyText = "NGN",CurrencySymbol = "NGN",   Rate = 1000     } },
            { Currencies.ZMW, new CurrencyObj(){ CurrencyText = "ZMW",CurrencySymbol = "ZMW",   Rate = 1        } },
            { Currencies.ARS, new CurrencyObj(){ CurrencyText = "ARS",CurrencySymbol = "ARS",   Rate = 100      } },
            { Currencies.COP, new CurrencyObj(){ CurrencyText = "COP",CurrencySymbol = "COP",   Rate = 1000     } },
            { Currencies.CRC, new CurrencyObj(){ CurrencyText = "CRC",CurrencySymbol = "CRC",   Rate = 500      } },
            { Currencies.DZD, new CurrencyObj(){ CurrencyText = "DZD",CurrencySymbol = "DZD",   Rate = 100      } },
            { Currencies.XAF, new CurrencyObj(){ CurrencyText = "XAF",CurrencySymbol = "XAF",   Rate = 500      } },
            { Currencies.XOF, new CurrencyObj(){ CurrencyText = "XOF",CurrencySymbol = "XOF",   Rate = 500      } },
            { Currencies.GHS, new CurrencyObj(){ CurrencyText = "GHS",CurrencySymbol = "GHS",   Rate = 10       } },
            { Currencies.GNF, new CurrencyObj(){ CurrencyText = "GNF",CurrencySymbol = "GNF",   Rate = 1000     } },
            { Currencies.KES, new CurrencyObj(){ CurrencyText = "KES",CurrencySymbol = "KES",   Rate = 100      } },
            { Currencies.RWF, new CurrencyObj(){ CurrencyText = "RWF",CurrencySymbol = "RWF",   Rate = 1000     } },
            { Currencies.SYP, new CurrencyObj(){ CurrencyText = "SYP",CurrencySymbol = "SYP",   Rate = 10       } },
            { Currencies.IQD, new CurrencyObj(){ CurrencyText = "IQD",CurrencySymbol = "IQD",   Rate = 1000     } },
            { Currencies.UAH, new CurrencyObj(){ CurrencyText = "UAH",CurrencySymbol = "UAH",   Rate = 10       } },
            { Currencies.RUB, new CurrencyObj(){ CurrencyText = "RUB",CurrencySymbol = "RUB",   Rate = 1        } },
            { Currencies.KQD, new CurrencyObj(){ CurrencyText = "KQD",CurrencySymbol = "KQD",   Rate = 1        } },
            { Currencies.IDR, new CurrencyObj(){ CurrencyText = "IDR",CurrencySymbol = "IDR",   Rate = 1000     } },
            { Currencies.ZWL, new CurrencyObj(){ CurrencyText = "ZWL",CurrencySymbol = "ZWL",   Rate = 1        } },
            { Currencies.ZAR, new CurrencyObj(){ CurrencyText = "ZAR",CurrencySymbol = "R",     Rate = 1        } },
            { Currencies.NAD, new CurrencyObj(){ CurrencyText = "NAD",CurrencySymbol = "NAD",   Rate = 1        } },
            { Currencies.AOA, new CurrencyObj(){ CurrencyText = "AOA",CurrencySymbol = "AOA",   Rate = 10       } },
            { Currencies.MZN, new CurrencyObj(){ CurrencyText = "MZN",CurrencySymbol = "MZN",   Rate = 1        } },
            { Currencies.AUD, new CurrencyObj(){ CurrencyText = "AUD",CurrencySymbol = "$",     Rate = 1        } },
            { Currencies.CAD, new CurrencyObj(){ CurrencyText = "CAD",CurrencySymbol = "$",     Rate = 1        } },
            { Currencies.NZD, new CurrencyObj(){ CurrencyText = "NZD",CurrencySymbol = "$",     Rate = 1        } },
            { Currencies.LBP, new CurrencyObj(){ CurrencyText = "LBP",CurrencySymbol = "LBP",   Rate = 10000    } },
        };
    }
}

    
