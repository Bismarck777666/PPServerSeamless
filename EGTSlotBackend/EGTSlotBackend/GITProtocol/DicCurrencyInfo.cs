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
        USD     = 0,    //美元
        EUR     = 1,    //欧元
        TND     = 2,    //突尼斯第纳尔
        TRY     = 3,    //土耳其里拉
        MAD     = 4,    //摩洛哥迪拉姆
        BRL     = 5,    //巴西里拉
        RTS     = 6,    //资源代币
        KWD     = 7,    //科威特第纳尔
        CHF     = 8,    //瑞士法郎
        EGP     = 9,    //埃及镑
        PEN     = 10,   //秘鲁索尔
        AED     = 11,   //阿拉伯酋长国第纳尔
        GBP     = 12,   //英镑
        BOB     = 13,   //玻利维亚玻利维亚诺
        NGN     = 14,   //尼日利亚奈拉
        ZMW     = 15,   //赞比亚克瓦查
        ARS     = 16,   //阿根廷比索
        COP     = 17,   //哥伦比亚比索
        CRC     = 18,   //哥斯达黎加科朗
        DZD     = 19,   //阿尔及利亚第纳尔
        XAF     = 20,   //中非法郎
        XOF     = 21,   //西非法郎
        GHS     = 22,   //加纳塞地
        GNF     = 23,   //几内亚法郎
        KES     = 24,   //肯尼亚先令
        RWF     = 25,   //卢旺达法郎
        SYP     = 26,   //瑞士法郎
        IQD     = 27,   //伊拉克第纳尔
        UAH     = 28,   //乌克兰格里夫纳
        RUB     = 29,   //俄罗斯卢布
        KQD     = 30,   //科威特第纳尔
        IDR     = 31,   //印度尼西亚卢比
        ZWL     = 32,   //津巴布韦元
        ZAR     = 33,   //南非兰特
        NAD     = 34,   //纳米比亚元
        AOA     = 35,   //安哥拉宽扎
        MZN     = 36,   //莫桑比克梅蒂卡尔
        AUD     = 37,   //澳大利亚元
        CAD     = 38,   //加拿大元
        NZD     = 39,   //新西兰元
        LBP     = 40,   //黎巴嫩镑
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

    
