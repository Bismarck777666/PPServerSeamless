using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    public enum CurrencyEnum
    {
        USD = 0,    //美元
        EUR = 1,    //欧元
        TND = 2,    //突尼斯(第纳尔)
        KRW = 3,    //韩国(韩元)
        GMD = 4,    //冈比亚(达拉西)
        CNY = 5,    //中国(人民币)
        JPY = 6,    //日本(日元)
        MYR = 7,    //马来西亚(林吉特)
        THB = 8,    //泰国(泰铢)
        PHP = 9,    //菲律宾(比索)
        VND = 10,   //越南(越南盾)
        INR = 11,   //印度(卢比)
        IDR = 12,   //印度尼西亚(卢比)
        PKR = 13,   //巴基斯坦(卢比)
        BDT = 14,   //孟加拉国(塔卡)
        NPR = 15,   //尼泊尔(卢比)
        UGX = 16,   //乌干达(先令)
        TRY = 17,   //土耳其(里拉)
        RUB = 18,   //俄罗斯(卢布)
    }
        
    public class CurrencyObj
    {
        public string   CurrencyText    { get; set; }
        public string   CurrencySymbol  { get; set; }
        public int      Rate            { get; set; }
    }

    public class Currencies
    {
        public Dictionary<int, CurrencyObj> _currencyInfo = new Dictionary<int, CurrencyObj>()
        {
            { (int)CurrencyEnum.USD, new CurrencyObj(){ CurrencyText = "USD",CurrencySymbol = "$",  Rate = 1        } },
            { (int)CurrencyEnum.EUR, new CurrencyObj(){ CurrencyText = "EUR",CurrencySymbol = "€",  Rate = 1        } },
            { (int)CurrencyEnum.TND, new CurrencyObj(){ CurrencyText = "TND",CurrencySymbol = "D",  Rate = 1        } },
            { (int)CurrencyEnum.KRW, new CurrencyObj(){ CurrencyText = "KRW",CurrencySymbol = "₩",  Rate = 1000     } },
            { (int)CurrencyEnum.GMD, new CurrencyObj(){ CurrencyText = "GMD",CurrencySymbol = "D",  Rate = 100      } },
            { (int)CurrencyEnum.CNY, new CurrencyObj(){ CurrencyText = "CNY",CurrencySymbol = "¥",  Rate = 10       } },
            { (int)CurrencyEnum.JPY, new CurrencyObj(){ CurrencyText = "JPY",CurrencySymbol = "¥",  Rate = 100      } },
            { (int)CurrencyEnum.MYR, new CurrencyObj(){ CurrencyText = "MYR",CurrencySymbol = "RM", Rate = 10       } },
            { (int)CurrencyEnum.THB, new CurrencyObj(){ CurrencyText = "THB",CurrencySymbol = "฿",  Rate = 10       } },
            { (int)CurrencyEnum.PHP, new CurrencyObj(){ CurrencyText = "PHP",CurrencySymbol = "₱",  Rate = 100      } },
            { (int)CurrencyEnum.VND, new CurrencyObj(){ CurrencyText = "VND",CurrencySymbol = "₫",  Rate = 20000    } },
            { (int)CurrencyEnum.INR, new CurrencyObj(){ CurrencyText = "INR",CurrencySymbol = "₹",  Rate = 100      } },
            { (int)CurrencyEnum.IDR, new CurrencyObj(){ CurrencyText = "IDR",CurrencySymbol = "Rp", Rate = 10000    } },
            { (int)CurrencyEnum.PKR, new CurrencyObj(){ CurrencyText = "PKR",CurrencySymbol = "₨",  Rate = 100      } },
            { (int)CurrencyEnum.BDT, new CurrencyObj(){ CurrencyText = "BDT",CurrencySymbol = "৳",  Rate = 100      } },
            { (int)CurrencyEnum.NPR, new CurrencyObj(){ CurrencyText = "NPR",CurrencySymbol = "रु",  Rate = 100      } },
            { (int)CurrencyEnum.UGX, new CurrencyObj(){ CurrencyText = "UGX",CurrencySymbol = "UGX",Rate = 1000     } },
            { (int)CurrencyEnum.TRY, new CurrencyObj(){ CurrencyText = "TRY",CurrencySymbol = "₺",  Rate = 10       } },
            { (int)CurrencyEnum.RUB, new CurrencyObj(){ CurrencyText = "RUB",CurrencySymbol = "₽",  Rate = 100      } },
        };
    }
}

    
