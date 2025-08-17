using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    public enum CurrencyEnum
    {
        USD = 0,    //미딸라
        EUR = 1,    //유로
        TND = 2,    //뜌니지(디나르)
        KRW = 3,    //코리아(원)
        GMD = 4,    //감비아(다르실)
        CNY = 5,    //중국(위엔)
        JPY = 6,    //일본(엔)
        MYR = 7,    //말레이시아(링기트)
        THB = 8,    //타이(바흐트)
        PHP = 9,    //필리핀(페소)
        VND = 10,   //베트남(동)
        INR = 11,   //인디아(루피)
        IDR = 12,   //인도네시아(루피아)
        PKR = 13,   //파키스탄(루피)
        BDT = 14,   //방글라데슈(타카)
        NPR = 15,   //네팔(루피)
        UGX = 16,   //우간다(쉴링)
        TRY = 17,   //터키(리라)
        RUB = 18,   //러시아(루불)
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

    
